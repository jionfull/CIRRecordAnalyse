using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CIRRecordAnalyse.Utilities;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CIRRecordAnalyse.Core
{
    public class RecordManager : IDisposable                //读取和解析MENU.bin等数据文件需要的类???,提取一帧里面的信息:源端口,业务类型等
    {
        const int MaxMapSize = 20 * 1024 * 1024;
        private bool FileLoaded = false;
        private bool disposed = false;
        static readonly string[] RecordFileNames = new string[] { "MENU.bin", "DATA.bin", "WAVE.bin", };

        readonly static long[] FileMaxMapSize = new long[] { 10L * 1024L * 1024L, 100L * 1024L * 1024L, 400L * 1024L * 1024L, }; //10M,100M,400M
        IntPtr[] FileMappingHandles = new IntPtr[RecordFileNames.Length];   //内存映射文件句柄
        IntPtr[] FileMemBase = new IntPtr[RecordFileNames.Length];          //内存映射文件基地址

        long[] FilesSize = new long[RecordFileNames.Length];                //文件大小

        IntPtr[] fileHandles = new IntPtr[3];

        //400M内存映射文件
        IntPtr memFileHandle = IntPtr.Zero;
        IntPtr memFileBase = IntPtr.Zero;
        int memFileFrameIndex = 0;

        List<RecordVoice> listVoice = new List<RecordVoice>();
        List<RecordStatus> listStatus = new List<RecordStatus>();
        List<RecordSerial> listSerial = new List<RecordSerial>();
        //串口数据帧
        byte[] SerialFrame = new byte[1024];
        int FrameLength = 0;
        bool MetHeader = false;
        //记录时间
        DateTime timeVoiceBegin = DateTime.Now;
        DateTime timeVoiceEnd = DateTime.Now;
        DateTime timeStatusBegin = DateTime.Now;
        DateTime timeStatusEnd = DateTime.Now;
        //FileStream

        FileStream VoiceFileStream = null;
        //路径
        string pathRecord = "";

        public event EventHandler<DataParseArgs> DataParseEvent;


        public int Version = 0; //legacy=0 1=new 
        public List<RecordSerial> SerialRecordList
        {
            get { return listSerial; }
        }

        public bool IsLoaded
        {
            get { return FileLoaded; }
        }

        public string RecordPath
        {
            get
            {
                return Path.GetFileName(pathRecord);
            }
        }
        public string FullPath
        {
            get
            {
                return pathRecord;
            }
        }

        public DateTime VoiceBeginTime
        {
            get
            {
                if (listVoice.Count > 0)
                {
                    return listVoice[0].RecordTime;
                }
                return timeVoiceBegin;
            }
        }
        public DateTime VoiceEndTime
        {
            get
            {
                if (listVoice.Count > 0)
                {
                    return listVoice[listVoice.Count - 1].RecordTime;
                }
                return timeVoiceEnd;
            }
        }

        public DateTime StatusBeginTime
        {
            get
            {
                if (listStatus.Count > 0)
                {
                    return listStatus[0].RecordTime;
                }
                return timeStatusBegin;
            }
        }
        public DateTime StatusEndTime
        {
            get
            {
                if (listStatus.Count > 0)
                {
                    return listStatus[listStatus.Count - 1].RecordTime;
                }
                return timeStatusEnd;
            }
        }

        public int CreateFileMem(string path)
        {
            if (FileLoaded) return 0;
            int result = 1;
            if (Directory.Exists(path) == false) return result; //目录不存在
            result++;
            for (int i = 0; i < 3; i++)
            {
                string fileName = Path.Combine(path, RecordFileNames[i]);
                if (File.Exists(fileName) == false) return result; //文件不存在

                if (i == 2)
                {
                    try
                    {
                        VoiceFileStream = File.Open(fileName, FileMode.Open);
                    }
                    catch { }
                }
                result++;
            }
            //创建内存映射
            for (int i = 0; i < 2; i++)
            {
                string filePath = Path.Combine(path, RecordFileNames[i]);
                IntPtr fileHandle = Win32API.CreateFile(filePath, Win32API.GENERIC_READ | Win32API.GENERIC_WRITE, FileShare.Read | FileShare.Write, IntPtr.Zero, FileMode.Open, Win32API.FILE_ATTRIBUTE_NORMAL | Win32API.FILE_FLAG_SEQUENTIAL_SCAN, IntPtr.Zero);
                fileHandles[i] = fileHandle;
                if (Win32API.INVALID_HANDLE_VALUE == (int)fileHandle)
                {
                    DisposeHandles();
                    return result;
                }
                result++;
                FileMappingHandles[i] = Win32API.CreateFileMapping(fileHandle, IntPtr.Zero, Win32API.PAGE_READWRITE, 0, 0, null);
                if (FileMappingHandles[i] == IntPtr.Zero)
                {
                    //DisposeHandles();
                    //return result;
                    continue;
                }
                result++;
                SYSTEM_INFO systemInfo = new SYSTEM_INFO();
                Win32API.GetSystemInfo(ref systemInfo);
                //得到系统页分配粒度
                uint allocationGranularity = systemInfo.dwAllocationGranularity;
                uint blockBytes = allocationGranularity;
                uint fileSizeHigh = 0;
                //get file size
                uint fileSize = Win32API.GetFileSize(fileHandle, out fileSizeHigh);
                fileSize |= (((uint)fileSizeHigh) << 32);
                FilesSize[i] = fileSize;
                //关闭文件句柄 
                Win32API.CloseHandle(fileHandle);

                blockBytes = fileSize;
                if (fileSize < 1 * allocationGranularity)
                {
                    //blockBytes = allocationGranularity;
                }
                if (blockBytes > FileMaxMapSize[i])
                {
                    blockBytes = MaxMapSize;
                }
                // 映射视图，得到地址
                FileMemBase[i] = Win32API.MapViewOfFile(FileMappingHandles[i], Win32API.FILE_MAP_COPY | Win32API.FILE_MAP_READ | Win32API.FILE_MAP_WRITE,
                   0, 0, blockBytes);
                if (FileMemBase[i] == IntPtr.Zero)
                {
                    DisposeHandles();
                    return result;
                }
                unsafe
                {
                    if (i == 1) //串口数据
                    {
                        if ((*(uint*)(FileMemBase[i])) == 0x01524553)
                        {
                            Version = 1;
                        }
                    }
                }
                
                result++;
            }

            //400M的临时文件缓存
            uint memFileSize = 400 * 1024 * 1024;
            memFileHandle = Win32API.CreateFileMapping((IntPtr)Win32API.INVALID_HANDLE_VALUE, IntPtr.Zero, Win32API.PAGE_READWRITE, 0, memFileSize, "CIRRecord");
            fileHandles[2] = memFileHandle;
            if (memFileHandle == IntPtr.Zero)
            {
                DisposeHandles();
                return result;
            }
            result++;
            memFileBase = Win32API.MapViewOfFile(memFileHandle, Win32API.FILE_MAP_READ | Win32API.FILE_MAP_WRITE,
                   0, 0, memFileSize);
            if (memFileBase == IntPtr.Zero)
            {
                DisposeHandles();
                return result;
            }
            result++;
            memFileFrameIndex = 0;
            FileLoaded = true;
            pathRecord = path;
            return 0;
        }

        public void ParseFile()
        {
            if (Version == 0)
            {
                ParseVoiceFile();
                ParseLegacySerialFile();
            }
            else
            {
                ParseVoiceFile();
                ParseNewSerialFile();
            }

            
         
        }


        private unsafe void ParseVoiceFile()
        {
            if (FileLoaded == false) return;
            //解析Menu.bin文件
            //直接读取语音目录扇区
            VoiceContent* pVC = (VoiceContent*)((uint)FileMemBase[0] + (180 - 171) * 512);
            int count = (int)(FilesSize[0] / 512 - (180 - 171)) * 32;
            for (int i = 0; i < count; i++)
            {
                int year = Helper.BCD2Int(pVC->Year);
                int month = Helper.BCD2Int(pVC->Month);
                int day = Helper.BCD2Int(pVC->Day);
                int hour = Helper.BCD2Int(pVC->Hour);
                int minute = Helper.BCD2Int(pVC->Minute);
                int second = Helper.BCD2Int(pVC->Second);
                uint blockStart = pVC->BlockBegin;
                uint blockCount = pVC->BlockCount;

                if (year >= 0 && year <= 99 &&
                    month >= 1 && month <= 12 &&
                    day >= 1 && day <= 31 &&
                    hour >= 0 && hour <= 23 &&
                    minute >= 0 && minute <= 59 &&
                    second >= 0 && second <= 59 &&
                    blockStart >= 2001 &&
                    blockStart <= 2099999 &&
                    blockCount > 0)
                {
                    DateTime time = new DateTime(year + 2000, month, day, hour, minute, second);
                    RecordVoice recordVoice = new RecordVoice(time, blockStart, blockCount, VoiceFileStream);
                    listVoice.Add(recordVoice);
                }
                else if (blockStart >= 2001 &&
                    blockStart <= 2099999 &&
                    blockCount > 0)
                {
                    DateTime time = new DateTime();
                    RecordVoice recordVoice = new RecordVoice(time, blockStart, blockCount, VoiceFileStream);
                    listVoice.Add(recordVoice);
                }
                pVC++;
            }
            //排序


            listVoice.Sort(new CompareVoiceRecord());

            int recordCnt = listVoice.Count;

            int IntersectIndex = -1;

            for (int i = 0; i < recordCnt; i++)
            {
                for (int j = i + 1; j < recordCnt; j++)
                {
                    if (listVoice[i].IntersectsWith(listVoice[j]))
                    {
                        IntersectIndex = i;

                        break;
                    }
                }
                if (IntersectIndex >= 0) break;
            }

            if (IntersectIndex >= 0)
            {
                //删除重叠的部分
                for (int k = recordCnt - 1; k <= IntersectIndex; k--)
                {
                    listVoice.RemoveAt(k);
                }
            }

        }


        private unsafe void ParseNewSerialFile()
        {
            if (FileLoaded == false) return;
            long parseSize = 0;
            long percent = 0;
            while (parseSize<FilesSize[1])
            {
                SerialBlock* blockSerial = (SerialBlock*)((long)FileMemBase[1] + parseSize);

                if (blockSerial->Tag0 != 'S') break;
                if (blockSerial->Tag1 != 'E') break;
                if (blockSerial->Tag2 != 'R') break;
                if (blockSerial->Type != 1) break;

                int frameLength = blockSerial->FrameLen;

                int blockSize = (frameLength + 15) / 16 * 16+16;

                parseSize += blockSize;


                int year = blockSerial->Year;
                int month = blockSerial->Month;
                int day = blockSerial->Day;
                int hour = blockSerial->Hour;
                int minute = blockSerial->Minute;
                int second = blockSerial->Second;
                int milSec = blockSerial->MilliSecond;

                if (year >= 0 && year <= 99 && month >= 1 && month <= 12 && day >= 1 && day <= 31 && hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59 && second >= 0 && second <= 59 && milSec >= 0 && milSec <= 999)
                {
                    DateTime time = new DateTime(year + 2000, month, day, hour, minute, second, milSec);
                    RecordStatus recordStatus = new RecordStatus(time, blockSerial->Status, blockSerial->Lattery,(char)1);
                    listStatus.Add(recordStatus);

                    byte* frame = blockSerial->Reserved;
                    RecordSerial rs = new RecordSerial((IntPtr)frame, 0, frameLength, time);

                    
                    rs.SrcPort = frame[4]; 
                    rs.DstPort = frame[frame[5] + 6];
                    rs.RecordType = frame[frame[frame[5] + 7] + frame[5] + 8];
                    rs.Command = frame[frame[frame[5] + 7] + frame[5] + 9];
                    listSerial.Add(rs);
                }

                long tPer = parseSize * 100 / FilesSize[1];
                if (tPer < 0) {
                    Console.WriteLine();
                }
                if (tPer != percent)
                {
                    percent = tPer;
                    if (DataParseEvent != null)
                    {
                        DataParseEvent(this, new DataParseArgs(1, tPer));
                    }
                }

            }
            if (DataParseEvent != null)
            {
                DataParseEvent(this, new DataParseArgs(2, 0));
            }
            listStatus.Sort(new CompareStatusRecord());
            listSerial.Sort(new CompareSerialRecord());

        }

        private unsafe void ParseLegacySerialFile()
        {
            if (FileLoaded == false) return;

            int count = (int)(FilesSize[1] / 256);
            int percent = 0;
            int curMapIndex = 0;
            for (int i = 0; i < count; i++)
            {
                int mapIndex = i * 256 / MaxMapSize;
                if (mapIndex != curMapIndex)
                {
                    if (Win32API.UnmapViewOfFile(FileMemBase[1]))
                    {
                        uint mapSize = MaxMapSize;
                        if (FilesSize[1] < (mapIndex + 1) * MaxMapSize)
                        {
                            mapSize = (uint)(FilesSize[1] % MaxMapSize);
                        }
                        FileMemBase[1] = Win32API.MapViewOfFile(FileMappingHandles[1], Win32API.FILE_MAP_COPY | Win32API.FILE_MAP_READ | Win32API.FILE_MAP_WRITE,
                   0, (uint)(i * 256), mapSize);
                        if (FileMappingHandles[1] == IntPtr.Zero)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                    curMapIndex = mapIndex;
                }
                SerialDataSection* pSDS = (SerialDataSection*)FileMemBase[1] + (i % (MaxMapSize / 256));
                ushort tag = pSDS->SectionTag;
                if (tag == 0xffff || tag == 0xaa55)
                {
                    int year = Helper.BCD2Int(pSDS->Year);
                    int month = Helper.BCD2Int(pSDS->Month);
                    int day = Helper.BCD2Int(pSDS->Day);
                    int hour = Helper.BCD2Int(pSDS->Hour);
                    int minute = Helper.BCD2Int(pSDS->Minute);
                    int second = Helper.BCD2Int(pSDS->Second);
                    int milSec = Helper.BCD2Int(pSDS->MilSec);

                    if (year >= 0 && year <= 99 && month >= 1 && month <= 12 && day >= 1 && day <= 31 && hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59 && second >= 0 && second <= 59 && milSec >= 0 && milSec <= 9)
                    {
                        DateTime time = new DateTime(year + 2000, month, day, hour, minute, second, milSec * 100);
                        RecordStatus recordStatus = new RecordStatus(time, pSDS->Status, pSDS->Voltage,(char)0);
                        listStatus.Add(recordStatus);

                        PushSerialData((byte*)(pSDS), 16, 256 - 16, time);
                    }
                }


                int tPer = i * 100 / count;
                if (tPer != percent)
                {
                    percent = tPer;
                    if (DataParseEvent != null)
                    {
                        DataParseEvent(this, new DataParseArgs(1, tPer));
                    }
                }
            }

            if (DataParseEvent != null)
            {
                DataParseEvent(this, new DataParseArgs(2, 0));
            }
            listStatus.Sort(new CompareStatusRecord());
            listSerial.Sort(new CompareSerialRecord());
        }

        private unsafe void PushSerialData(byte* fileMem, int offset, int length, DateTime time)
        {
            for (int i = 0; i < length; i++)
            {
                byte data = fileMem[offset + i];
                if (data == 0x10)
                {
                    if (FrameLength == 0)
                    {
                        SerialFrame[0] = 0x10;
                        MetHeader = true;
                        FrameLength = 1;
                    }
                    else if (FrameLength >= 2)
                    {
                        if (MetHeader == false)
                        {
                            SerialFrame[FrameLength] = 0x10;
                            FrameLength++;
                            MetHeader = true;
                            if (FrameLength >= SerialFrame.Length)
                            {
                                FrameLength = 0;
                                MetHeader = false;
                            }
                        }
                        else
                        {
                            MetHeader = false;
                        }
                    }
                }
                else if (data == 0x02)
                {
                    if (FrameLength >= 1)
                    {
                        SerialFrame[FrameLength] = 0x02;

                        FrameLength++;
                        if (FrameLength >= SerialFrame.Length)
                        {
                            FrameLength = 0;
                            MetHeader = false;
                        }
                    }
                }
                else if (data == 0x03)
                {
                    if (FrameLength >= 2)
                    {
                        SerialFrame[FrameLength] = 0x03;
                        FrameLength++;
                        if (MetHeader)
                        {
                            ParseSerialFrame(SerialFrame, FrameLength, time);
                            FrameLength = 0;
                            MetHeader = false;
                        }
                        else
                        {
                            if (FrameLength >= SerialFrame.Length)
                            {
                                FrameLength = 0;
                                MetHeader = false;
                            }
                        }
                    }
                }
                else
                {
                    if (FrameLength >= 2)
                    {
                        MetHeader = false;
                        SerialFrame[FrameLength] = data;
                        FrameLength++;
                        if (FrameLength >= SerialFrame.Length)
                        {
                            FrameLength = 0;
                            MetHeader = false;
                        }
                    }
                }
            }
        }

        private unsafe void ParseSerialFrame(byte[] frame, int length, DateTime time)
        {
            //解析一个数据帧
            if (length >= 8)
            {
                ushort len = (ushort)((frame[2] << 8) + frame[3]);
                if (len == length - 6)
                {
                    ushort crc = CRC16.ComputeCRC16(frame, 2, len);
                    ushort crcR = (ushort)(frame[len + 3] + (frame[len + 2] << 8));
                    if (crc == crcR)
                    {
                        byte* pBuffer = (byte*)memFileBase;
                        for (int i = 0; i < length; i++)
                        {
                            pBuffer[memFileFrameIndex + i] = frame[i];
                        }
                        RecordSerial rs = new RecordSerial(memFileBase, memFileFrameIndex, length, time);
                        memFileFrameIndex += length;
                        rs.SrcPort = frame[4];
                        rs.DstPort = frame[frame[5] + 6];
                        rs.RecordType = frame[frame[frame[5] + 7] + frame[5] + 8];
                        rs.Command = frame[frame[frame[5] + 7] + frame[5] + 9];
                        listSerial.Add(rs);
                    }
                }
            }
        }

        private void DisposeHandles()
        {
            for (int i = 0; i < RecordFileNames.Length; i++)
            {
                if (FileMemBase[i] != IntPtr.Zero)
                {
                    if (Win32API.UnmapViewOfFile(FileMemBase[i]))
                    {
                        FileMemBase[i] = IntPtr.Zero;
                    }
                }

                if (FileMappingHandles[i] != IntPtr.Zero)
                {
                    if (Win32API.CloseHandle(FileMappingHandles[i]))
                    {
                        FileMappingHandles[i] = IntPtr.Zero;
                    }
                }
                if (fileHandles[i] != IntPtr.Zero)
                {
                    Win32API.CloseHandle(fileHandles[i]);
                    fileHandles[i] = IntPtr.Zero;
                }
            }
            //清空400M内存文件
            if (memFileBase != IntPtr.Zero)
            {
                if (Win32API.UnmapViewOfFile(memFileBase))
                {
                    memFileBase = IntPtr.Zero;
                }
            }

            if (memFileHandle != IntPtr.Zero)
            {
                if (Win32API.CloseHandle(memFileHandle))
                {
                    memFileHandle = IntPtr.Zero;
                }
            }
        }


        private int SearchSerialIndex(DateTime time)
        {
            if (listSerial.Count <= 0) return -1;
            if (time <= listSerial[0].RecordTime) return 0;
            if (time >= listSerial[listSerial.Count - 1].RecordTime) return listSerial.Count - 1;

            int startIndex = 0;
            int endIndex = listSerial.Count - 1;
            int midIndex = 0;
            while (true)
            {
                midIndex = (startIndex + endIndex) / 2;
                if (midIndex == startIndex)
                {
                    if (listSerial[midIndex].RecordTime < time)
                    {
                        midIndex += 1;
                        if (midIndex >= listSerial.Count) midIndex = listSerial.Count - 1;
                    }
                    break;
                }
                else if (midIndex == endIndex) break;
                else
                {
                    if (listSerial[midIndex].RecordTime < time) { startIndex = midIndex; }
                    else if (listSerial[midIndex].RecordTime > time) { endIndex = midIndex; }
                    else break;
                }
            }
            return midIndex;
        }

        private int SearchStatusIndex(DateTime time)
        {
            if (time <= listStatus[0].RecordTime) return 0;
            if (time >= listStatus[listStatus.Count - 1].RecordTime) return listStatus.Count - 1;

            int startIndex = 0;
            int endIndex = listStatus.Count - 1;
            int midIndex = 0;
            while (true)
            {
                midIndex = (startIndex + endIndex) / 2;
                if (midIndex == startIndex)
                {
                    if (listStatus[midIndex].RecordTime < time)
                    {
                        midIndex += 1;
                        if (midIndex >= listStatus.Count) midIndex = listStatus.Count - 1;
                    }
                    break;
                }
                else if (midIndex == endIndex) break;
                else
                {
                    if (listStatus[midIndex].RecordTime < time) { startIndex = midIndex; }
                    else if (listStatus[midIndex].RecordTime > time) { endIndex = midIndex; }
                    else break;
                }
            }
            return midIndex;
        }

        private int SearchVoiceIndex(DateTime time)
        {
            if (time <= listVoice[0].RecordTime) return 0;
            if (time >= listVoice[listVoice.Count - 1].RecordTime) return listVoice.Count - 1;

            int startIndex = 0;
            int endIndex = listVoice.Count - 1;
            int midIndex = 0;
            while (true)
            {
                midIndex = (startIndex + endIndex) / 2;
                if (midIndex == startIndex)
                {
                    if (listVoice[midIndex].RecordTime < time)
                    {
                        midIndex += 1;
                        if (midIndex >= listVoice.Count) midIndex = listStatus.Count - 1;
                    }
                    break;
                }
                else if (midIndex == endIndex) break;
                else
                {
                    if (listVoice[midIndex].RecordTime < time) { startIndex = midIndex; }
                    else if (listVoice[midIndex].RecordTime > time) { endIndex = midIndex; }
                    else break;
                }
            }
            return midIndex;
        }

        public void SearchRecord(SearchCondition cond, IList<RecordSerial> list)        //根据 SearchCondition 的 Port , Style , Command 查找数据
        {
            int startIndex = 0;
            int endIndex = listSerial.Count - 1;
            
            if (cond.AllTime == false)
            {
                startIndex = SearchSerialIndex(cond.TimeBegin);
                endIndex = SearchSerialIndex(cond.TimeEnd);
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                RecordSerial rs = listSerial[i];
                //int jj = rs.my_first_number;
                if (cond.Port > 0 && rs.SrcPort != cond.Port && rs.DstPort != cond.Port) continue;              //  端口/类型/命令大于零(不等于-1),且不飞符合"条件"=>跳过..否则增加此记录
                if (cond.Style > 0 && rs.RecordType != cond.Style) continue;
                if (cond.Command >= 0 && rs.Command != cond.Command) continue;

                if (cond.Style == 3 && cond.Command == 0x20)
                    if (rs.SrcPort == 1 && rs.DstPort == 5 && rs.RecordType == 3 && rs.Command == 0x20) continue;   //直接把:源端口,目的端口,业务类型,命令=(01,05,03,0x20)的记录(主机向450M发送机车号)去掉,
                //因为在查询(03/04,01/05,03,0x20的记录,即"MMI设置机车号"时会将450M的记录搜索进来,以后需要搜索(01,05,03,0x20)的记录时在此处修改)
                //仅在查询具体类型时排除

                if (cond.Style == 3 && cond.Command == 0x19 && cond.my_number == 0xaa)
                {
                    if ((rs.SrcPort == 3 || rs.SrcPort == 4) && (rs.DstPort == 3 || rs.DstPort == 4) && rs.RecordType == 3 && rs.Command == 0x19 && rs.my_first_number == 0xaa)
                        list.Add(listSerial[i]);
                    continue;
                }

                if (cond.Style == 3 && cond.Command == 0x19 && cond.my_number == 0xab)
                {
                    if ((rs.SrcPort == 3 || rs.SrcPort == 4) && (rs.DstPort == 3 || rs.DstPort == 4) && rs.RecordType == 3 && rs.Command == 0x19 && rs.my_first_number == 0xab)
                        list.Add(listSerial[i]);
                    continue;                   
                }

                if (cond.Style == 3 && cond.Command == 0x19 && cond.my_number == 0xac)
                {
                    if ((rs.SrcPort == 3 || rs.SrcPort == 4) && (rs.DstPort == 3 || rs.DstPort == 4) && rs.RecordType == 3 && rs.Command == 0x19 && rs.my_first_number == 0xac)
                        list.Add(listSerial[i]);
                    continue;   
                    
                }

                if (cond.Style == 3 && cond.Command == 0x19 && cond.my_number == 0x0F)
                {
                    if ((rs.SrcPort == 3 || rs.SrcPort == 4) && (rs.DstPort == 3 || rs.DstPort == 4) && rs.RecordType == 3 && rs.Command == 0x19 && rs.my_first_number == 0x0F) 
                    list.Add(listSerial[i]);
                    continue;                     
                }

                list.Add(listSerial[i]);
            }
        }

        //------------------------搜索满足条件的记录的并集（重载）---------------------
        public void SearchRecord(List<SearchCondition> cond, IList<RecordSerial> list, bool isAllTime, DateTime timeBegin, DateTime timeEnd)        //根据 SearchCondition 的 Port , Style , Command 查找数据
        {
            int startIndex = 0;
            int endIndex = listSerial.Count - 1;
            if (isAllTime == false)
            {
                startIndex = SearchSerialIndex(timeBegin);
                endIndex = SearchSerialIndex(timeEnd);
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                RecordSerial rs = listSerial[i];

                //bool canBeFilter = false;
                foreach (SearchCondition sc in cond)
                {
                    if (sc.Port > 0 && rs.SrcPort != sc.Port && rs.DstPort != sc.Port) continue;          //  端口/类型/命令大于零,且不等于"条件"=>跳过..否则增加此记录
                    if (sc.Style > 0 && rs.RecordType != sc.Style) continue;
                    if (sc.Command >= 0 && rs.Command != sc.Command) continue;

                    if (sc.Style == 3 && sc.Command == 0x20)
                        if (rs.SrcPort == 1 && rs.DstPort == 5 && rs.RecordType == 3 && rs.Command == 0x20) continue;
                    list.Add(rs);
                    break;
                }
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------
        public void SearchRecordDetail(List<int> srcPortList,                              //多重查询->搜索对应 源端口,目的端口,业务类型 &命令(新增) 符合条件的记录
                                        List<int> dstPortList,
                                        List<int> typeList,
                                        List<int> comList,
                                        DateTime tBegin,
                                        DateTime tEnd,
                                        bool isAllTime,
                                        IList<RecordSerial> list)
        {
            int startIndex = 0;
            int endIndex = listSerial.Count - 1;

            if (isAllTime == false)
            {
                startIndex = SearchSerialIndex(tBegin);
                endIndex = SearchSerialIndex(tEnd);
            }

            for (int i = startIndex; i <= endIndex; i++)        //检测每条记录
            {
                RecordSerial rs = listSerial[i];
                bool srcRight = srcPortList.Count == 0 ? true : false;     //如果源端口条件列表为空,则表示不限制
                bool dstRight = dstPortList.Count == 0 ? true : false;    //...
                bool typeRight = typeList.Count == 0 ? true : false;
                bool comRight = comList.Count == 0 ? true : false;

                foreach (int a in srcPortList)            //源端口符合条件->加入列表
                {
                    if (rs.SrcPort == a)
                    {
                        //list.Add(rs);
                        srcRight = true;
                        break;                                   //只要该条记录符合此源端口,则表示此记录在"源端口"条件上已成立,不必再检查其他源端口条件
                    }
                }
                foreach (int b in dstPortList)             //远程端口...
                {
                    if (rs.DstPort == b)
                    {
                        dstRight = true;
                        break;
                    }
                }
                foreach (int c in typeList)             //类型名...
                {
                    if (rs.RecordType == c)
                    {
                        typeRight = true;
                        break;
                    }
                }

                foreach (int d in comList)             //类型名...
                {
                    if (rs.Command == d)
                    {
                        comRight = true;
                        break;
                    }
                }

                if (srcRight && dstRight && typeRight && comRight)
                    list.Add(rs);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------------------------

        public void SearchRecord(SearchCondition cond, IList<RecordStatus> list)        //状态查询
        {
            int startIndex = 0;
            int endIndex = listStatus.Count - 1;
            if (cond.AllTime == false)
            {
                startIndex = SearchStatusIndex(cond.TimeBegin);
                endIndex = SearchStatusIndex(cond.TimeEnd);
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                RecordStatus rs = listStatus[i];
                list.Add(listStatus[i]);
            }
        }

        public void SearchRecord(SearchCondition cond, IList<RecordVoice> list)         //语音查询
        {
            int startIndex = 0;
            int endIndex = listVoice.Count - 1;
            if (cond.AllTime == false)
            {
                startIndex = SearchVoiceIndex(cond.TimeBegin);
                endIndex = SearchVoiceIndex(cond.TimeEnd);
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                list.Add(listVoice[i]);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (VoiceFileStream != null) { VoiceFileStream.Close(); }
                    listSerial.Clear();
                    listStatus.Clear();
                    listVoice.Clear();
                    GC.Collect();
                    // Release managed resources
                }

                // Release unmanaged resources
                DisposeHandles();
                GC.Collect();
                disposed = true;
            }
        }

        ~RecordManager()
        {
            Dispose(false);
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class SearchCondition
    {
        DateTime timeBegin;
        DateTime timeEnd;
        bool allTime;
        int port = -1;
        int number = -1;

        int srcport = 0;
        int dstport = 0;

        int style = -1;
        int command = -1;
        public SearchCondition()
        {

        }
        public SearchCondition(DateTime timeBegin, DateTime timeEnd, bool allTime)
        {
            this.timeBegin = timeBegin;
            this.timeEnd = timeEnd;
            this.allTime = allTime;
        }

        public DateTime TimeBegin
        {
            get { return timeBegin; }
            set { timeBegin = value; }
        }

        public int my_number
        {
            get { return number; }
            set { number = value; }
        }

        public DateTime TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }

        public bool AllTime
        {
            get { return allTime; }
            set { allTime = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        //-------------------------------------------------------------------
        public int SrcPort
        {
            get { return srcport; }
            set { srcport = value; }
        }
        public int DstPort
        {
            get { return dstport; }
            set { dstport = value; }
        }
        //-------------------------------------------------------------------
        public int Command
        {
            get { return command; }
            set { command = value; }
        }
        public int Style
        {
            get { return style; }
            set { style = value; }
        }
    }

    public class CreateCondition
    {
        bool selectFile;
        bool selectIndex;
        string indexTxt = "";

        public CreateCondition(bool selectFile, bool selectIndex, string indexTxt)
        {
            this.selectFile = selectFile;
            this.selectIndex = selectIndex;
            this.indexTxt = indexTxt;
        }

        public bool SelectFile
        {
            get 
            { 
                return selectFile;
            }
        }

        public string IndexText
        {
            get
            {
                return indexTxt;
            }
        }
    }

    public class DataParseArgs : EventArgs
    {
        long position = 0;
        int type = -1;
        public long Position
        {
            get { return position; }
        }

        public DataParseArgs(int type, long pos)
        {
            this.type = type;
            this.position = pos;
        }
        public int EventType
        {
            get { return type; }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ContentTable
    {
        [FieldOffset(0)]
        public uint VoiceCurrentBlock;
        [FieldOffset(4)]
        public byte VoiceCurrentIndex;
        [FieldOffset(5)]
        public byte Flag;
        [FieldOffset(6)]
        public ushort Reserved;
        [FieldOffset(8)]
        public uint DataStartBlock;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct VoiceContent
    {
        [FieldOffset(0)]
        public ushort ItemIndex;
        [FieldOffset(2)]
        public byte Year;
        [FieldOffset(3)]
        public byte Month;
        [FieldOffset(4)]
        public byte Day;
        [FieldOffset(5)]
        public byte Hour;
        [FieldOffset(6)]
        public byte Minute;
        [FieldOffset(7)]
        public byte Second;
        [FieldOffset(8)]
        public uint BlockBegin;
        [FieldOffset(12)]
        public uint BlockCount;

    }


    [StructLayout(LayoutKind.Explicit)]
    struct SerialDataSection
    {
        [FieldOffset(0)]
        public ushort SectionTag;
        [FieldOffset(2)]
        public byte Year;
        [FieldOffset(3)]
        public byte Month;
        [FieldOffset(4)]
        public byte Day;
        [FieldOffset(5)]
        public byte Hour;
        [FieldOffset(6)]
        public byte Minute;
        [FieldOffset(7)]
        public byte Second;
        [FieldOffset(8)]
        public byte MilSec;
        [FieldOffset(9)]
        public byte Status;
        [FieldOffset(10)]
        public ushort Voltage;
        [FieldOffset(12)]
        public uint SectionNum;
        [FieldOffset(16)]
        public unsafe fixed byte Reserved[240];

    }

    [StructLayout(LayoutKind.Explicit)]
    struct SerialBlock
    {
        [FieldOffset(0)]
        public byte Tag0;
        [FieldOffset(1)]
        public byte Tag1;
        [FieldOffset(2)]
        public byte Tag2;
        [FieldOffset(3)]
        public byte Type;

        [FieldOffset(4)]
        public byte FrameLen;
        [FieldOffset(5)]
        public byte Status;
        [FieldOffset(6)]
        public ushort Lattery;
        [FieldOffset(8)]
        public byte Year;
        [FieldOffset(9)]
        public byte Month;
        [FieldOffset(10)]
        public byte Day;
        [FieldOffset(11)]
        public byte Hour;
        [FieldOffset(12)]
        public byte Minute;
        [FieldOffset(13)]
        public byte Second;
        [FieldOffset(14)]
        public UInt16 MilliSecond;

        [FieldOffset(16)]
        public unsafe fixed byte Reserved[240]; //最多240个


    }

    class CompareVoiceRecord : IComparer<RecordVoice>
    {

        #region IComparer<RecordVoice> 成员

        public int Compare(RecordVoice x, RecordVoice y)
        {
            return x.RecordTime.CompareTo(y.RecordTime);
        }

        #endregion
    }

    class CompareSerialRecord : IComparer<RecordSerial>
    {

        #region IComparer<RecordSerial> 成员

        public int Compare(RecordSerial x, RecordSerial y)
        {
            return x.RecordTime.CompareTo(y.RecordTime);
        }

        #endregion
    }

    class CompareStatusRecord : IComparer<RecordStatus>
    {

        #region IComparer<RecordStatus> 成员

        public int Compare(RecordStatus x, RecordStatus y)
        {
            return x.RecordTime.CompareTo(y.RecordTime);
        }

        #endregion
    }
}