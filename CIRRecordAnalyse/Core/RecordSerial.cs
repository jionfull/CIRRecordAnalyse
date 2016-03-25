using System;
using System.Collections.Generic;
using System.Text;
using CIRRecordAnalyse.Utilities;

namespace CIRRecordAnalyse.Core
{
    public class RecordSerial
    {
        IntPtr fileMemBase = IntPtr.Zero;
        int frameLength;
        int offsetBegin = -1;

        int srcPort = -1;
        int dstPort = -1;
        int recordType = -1;
        int command = -1;
        DateTime recordTime;     


        //--------------------
        static readonly string NewLine = Environment.NewLine;

        public RecordSerial(IntPtr memFile, int offsetBegin, int frameLength, DateTime recordTime)
        {
            this.fileMemBase = memFile;
            this.offsetBegin = offsetBegin;
            this.frameLength = frameLength;
            this.recordTime = recordTime;
        }

        public DateTime RecordTime
        {
            get { return recordTime; }
        }

        public int SrcPort              //源端口
        {
            set { srcPort = value; }
            get { return srcPort; }
        }

        public byte my_first_number              //第一位数据信息
        {
            get
            {
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];
                byte lenDst = frame[5 + lenSrc + 2];
                int tHead = 5 + lenSrc + 2 + lenDst + 3;
                return frame[tHead];
            }
        }

        public int DstPort              //目的端口
        {
            get { return dstPort; }
            set { dstPort = value; }
        }

        public int RecordType           //业务类型?
        {
            get { return recordType; }
            set { recordType = value; }
        }
        public int Command              //命令
        {
            get { return command; }
            set { command = value; }
        }

        public string SrcPortName       //源端口名
        {
            get { return Helper.GetPoartName(srcPort); }
        }

        public string SrcPortName2      //目的端口名
        {
            get { return srcPort.ToString() + "—" + Helper.GetPortName2(srcPort); }
        }

        public string SrcAddress        //源地址
        {
            get
            {
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    byte len = p[offsetBegin + 5];
                    if (len != 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < len; i++)
                        {
                            if (i != 0)
                            {
                                sb.Append(".");
                            }
                            sb.Append(p[offsetBegin + 6 + i].ToString());
                        }
                        return sb.ToString();
                    }
                    return "—";

                }
            }
        }
        //public string longitude       //目的端口名
        //{
        //    get { return Helper.GetPoartName(longitude); }
        //}
        public string DstPortName       //目的端口名
        {
            get { return Helper.GetPoartName(dstPort); }
        }

        public string DstPortName2      //目的端口名2
        {
            get { return dstPort.ToString() + "—" + Helper.GetPortName2(dstPort); }
        }

        public string DstAddress        //目的端口地址
        {
            get
            {
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    byte lenSrc = p[offsetBegin + 5];
                    byte lenDst = p[offsetBegin + 5 + lenSrc + 2];
                    if (lenDst != 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < lenDst; i++)
                        {
                            if (i != 0)
                            {
                                sb.Append(".");
                            }
                            sb.Append(p[offsetBegin + 5 + lenSrc + 3 + i].ToString());
                        }
                        return sb.ToString();
                    }
                    return "—";

                }
            }
        }

        public string TypeName          //业务类型名
        {
            get
            {
                if (srcPort == 0x11 && dstPort == 0x01 && (recordType == 5 && command == 0x21))
                {
                    return "TAX箱信息";
                }
                return Helper.GetStyleName(recordType);
            }
        }

        public string DetailInfo        //详细信息?
        {
            get { return Helper.GetExplainString(srcPort, dstPort, recordType, command, frameLength - 6); }
        }

        public string ExplainInfo               //命令解析
        {
            get
            {
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];
                byte lenDst = frame[5 + lenSrc + 2];

                int tHead = 5 + lenSrc + 2 + lenDst + 3;               

                return Helper.GetExplainData(srcPort, dstPort, recordType, command, frame, tHead);
            }
        }
        //------------------ 命令解析(表格显示)-----------------------
        public string ExplainInfo2
        {
            get
            {
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];
                byte lenDst = frame[5 + lenSrc + 2];

                int tHead = 5 + lenSrc + 2 + lenDst + 3;

                string str = Helper.GetExplainData(srcPort, dstPort, recordType, command, frame, tHead).Replace(NewLine + NewLine, NewLine).Replace(NewLine, "    ").TrimEnd(';');
                if (str.Contains("命令解析："))
                    str = str.Substring(str.IndexOf("命令解析：") + "命令解析：    ".Length);
                //if (str.Contains("GPS信息有效"))            //查询GPS信息时，命令解析列不显示经纬度
                //    str = str.Substring(0, str.IndexOf("经度") - 1) + str.Substring(str.IndexOf("时间"));
                return str;
            }
        }
        
        //----------------------线路信息------------------------
        public string RouteInfo
        {
            get
            {               
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];
                byte lenDst = frame[5 + lenSrc + 2];

                int tHead = 5 + lenSrc + 2 + lenDst + 3;

                int tlen = (frame[2] * 0x100) + frame[3];
                string routeinfo = "";
                if (srcPort == 0x13 && dstPort == 0x02 && command == 0x01 && (frame[tHead] == 0x01 || frame[tHead] == 0x02))
                {
                    routeinfo = Encoding.Default.GetString(frame, tHead + 20, 8).Trim();  
                }
                if (srcPort == 0x13 && dstPort == 0x02 && command == 0x01 && (frame[tHead] == 0x03 || frame[tHead] == 0x04))
                {
                    routeinfo = Encoding.Default.GetString(frame, tHead + 9, 8).Trim();
                }
                if (srcPort == 0x13 && dstPort == 0x02 && command == 0x01 && (frame[tHead] == 0x05 || frame[tHead] == 0x06))
                {
                    routeinfo = Encoding.Default.GetString(frame, tHead + 9, 8).Trim();
                }
                
                if ((srcPort == 6) && (tlen == 0x9e) && frame[tHead + 2] != 0x56)           //gps
                {
                    routeinfo = Encoding.Default.GetString(frame, tHead + 3, 8).Trim() + "(" + Encoding.Default.GetString(frame, tHead + 13, 0x15).Trim() + ")";
                }
                else if (frame[4] == 0x01 && frame[8 + lenDst + lenSrc] == 0x03 && frame[9 + lenDst + lenSrc] == 0x55)          //调度信息
                {
                    tHead = 5 + lenSrc + 2 + lenDst + 3 + 18;       //重定义tHead
                    int j = 0;
                    j = 0;
                    while (j < 20)
                    {
                        if (frame[tHead + j] == 0x3b)
                        {
                            break;
                        }
                        j++;
                    }
                    tHead += j + 1;
                    j = 0;
                    while (j < 20)
                    {
                        if (frame[tHead + j] == 0x3b)
                        {
                            break;
                        }
                        j++;
                    }
                    tHead += j + 1;
                    j = 0;
                    while (j < 50)
                    {
                        if (frame[tHead + j] == 0x3b)
                        {
                            break;
                        }
                        j++;
                    }
                    routeinfo = Encoding.Default.GetString(frame, tHead, j);
                }
                else if (recordType == 3 && command == 8)
                {
                    int tLen = (((frame[2] * 0x100) + frame[3]) - tHead) + 2;
                    if ((srcPort == 2 || srcPort == 3 || srcPort == 4) && (dstPort == 1))
                    {
                        routeinfo = Encoding.Default.GetString(frame, tHead + 1, tLen - 2);
                    }
                    else routeinfo = "";
                }
                else if (recordType == 3 && command == 0x4e)
                {
                    routeinfo = Encoding.Default.GetString(frame, tHead + 1, 8).Trim(' '); ;
                    routeinfo += "(" + Encoding.Default.GetString(frame, tHead + 9, 21).Trim(' ') + ")";
                }
                return routeinfo;
            }
        }

        //-----------------------车次号/机车号---------------------
        public string TrainNumber
        {
            get
            {
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];                               //源通信地址长度
                byte lenDst = frame[5 + lenSrc + 2];            //目的通信地址长度

                string str1 = string.Empty;
                int tHead = 0;               
               
                if (frame[4] == 0x01 && (frame[8 + lenDst + lenSrc] == 0x03 || recordType == 0x0b) && frame[9 + lenDst + lenSrc] == 0x55)//0355
                {
                    tHead = 5 + lenSrc + 2 + lenDst + 3 + 18;
                    int j = 0;
                    j = 0;
                    while (j < 20)
                    {
                        if (frame[tHead + j] == 0x3b)
                        {
                            break;
                        }
                        j++;
                    }
                    str1 = "[" + Encoding.Default.GetString(frame, tHead, j) + "]" ;
                    tHead += j + 1;
                    j = 0;
                    while (j < 20)
                    {
                        if (frame[tHead + j] == 0x3b)
                        {
                            break;
                        }
                        j++;
                    }
                    str1 = str1 + Encoding.Default.GetString(frame, tHead, j);
                    tHead += j + 1;
                    j = 0;
                    while (j < 50)
                    {
                        if (frame[tHead + j] == 0x3b)
                        {
                            break;
                        }
                        j++;
                    }
                }

                tHead = 5 + lenSrc + 2 + lenDst + 3;
                int tLen = (((frame[2] * 0x100) + frame[3]) - tHead) + 2;
                if (recordType == 3 && command == 0x11)
                {
                    str1 = "[" + Encoding.Default.GetString(frame, tHead + 1, tLen - 2) + "]";
                }
                else if (recordType == 3 && command == 0x20)
                {
                    str1 = Encoding.Default.GetString(frame, tHead, tLen - 1);
                }
                else if (recordType == 3 && command == 0x58)
                {
                    if (tLen > 2)
                    {
                        str1 = "[" + Encoding.Default.GetString(frame, tHead + 3, tLen - 3) + "]";
                    }
                }
                else if (recordType == 6 && command == 0x20)
                {
                    try
                    {
                        string cch = Encoding.Default.GetString(frame, tHead + 10, 7).TrimEnd(' ');
                        string jch = Encoding.Default.GetString(frame, tHead + 0x11, 8);
                        str1 = "[" + cch + "]" + jch;
                    }
                    catch { }
                }
                else if ((recordType == 7 && command == 0x02) || (recordType == 7 && command == 0x03) || (recordType == 5 && command == 0x21) || (recordType == 0x0b&& command == 0))
                {
                    if (srcPort == 0x11 && dstPort == 0x21 && (recordType == 5 && command == 0x21)) //TAX2信息
                    {

                    }
                    else
                    {
                        str1 = "[" + (frame[tHead + 30] * 256 * 256 + frame[tHead + 29] * 256 + frame[tHead + 28]).ToString() + "]";
                        str1 += (frame[tHead + 64] + frame[tHead + 65] * 256).ToString();
                        if (recordType == 0x0b) return str1;
                        if (tHead + 119 < frame.Length)
                        {
                            if (frame[tHead + 119] == 0x56) str1 = "";
                        }
                    }
                    
                }
                else if (recordType == 6 && (command == 0x51 || command == 0x53 || command == 0x61))
                {
                    try
                    {
                        string cch = Encoding.Default.GetString(frame, tHead + 8, 7).TrimEnd(' ');
                        string jch = Encoding.Default.GetString(frame, tHead + 15, 8);
                        str1 = "[" + cch + "]" + jch;
                    }
                    catch { }
                }
                else if (recordType == 3 && command == 0x59)
                {
                    try
                    {
                        if (tLen > 2)
                        {
                            str1 = Encoding.Default.GetString(frame, tHead + 3, tLen - 3);
                        }
                    }
                    catch { }
                }
                else if (recordType == 4)
                {
                    ulong jch1 = 0;
                    for (int i = 0; i < 4; i++) jch1 = (jch1 * 100) + BCDToHex((ulong)frame[i + tHead], 2);
                    return jch1.ToString();
                }
                else if (recordType == 3 && command == 0x56)
                {
                    if (frame[tHead + 1] == 0x01)
                    {
                        int cchLength = frame.Length - tHead - 2 - 4;
                        string cch = Encoding.Default.GetString(frame, tHead + 2, cchLength).TrimEnd(' ');
                        str1 = "[" + cch + "]";
                    }
                    else //if(frame[tHead+1]==0x00)
                    {
                        str1 = "";
                    }
                }
               else if (srcPort == 0x13 && dstPort == 0x02 && command == 0x01 && (frame[tHead] == 0x01 || frame[tHead] == 0x02))
                {
                    ulong jch1 = 0;
                    //string cch = Encoding.Default.GetString(frame, tHead + 5, 3).TrimEnd(' ');
                    string cchtype = Encoding.Default.GetString(frame, tHead + 1, 4).Trim();                   
                    int cchnum = (frame[tHead + 5] + (frame[tHead + 6] << 8)) + (frame[tHead + 7] << 0x10);
                    for (int i = 0; i < 4; i++) jch1 = (jch1 * 100) + BCDToHex((ulong)frame[i + tHead + 8], 2);
                    str1 = "[" + cchtype + cchnum + "]" + jch1.ToString();                   
                }
                return str1;
            }
        }
        //----------------------经度-------------------------
        public string Longitude
        {
            get
            {
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];                         //  源通信地址长度
                byte lenDst = frame[5 + lenSrc + 2];            //目的通信地址长度
                int tHead = 5 + lenSrc + 2 + lenDst + 3;
                string str1 = string.Empty;
                if ((srcPort == 6) && (frame[2] * 0x100 + frame[3] == 0x9e) && frame[tHead + 2] != 0x56)
                {
                    if ((frame[tHead + 0x23] == 0xff) && (frame[tHead + 0x24] == 0xff)) str1 = "无效";
                    else
                    {
                        string longitude1 = BCDToHex((ulong)frame[tHead + 0x23], 2).ToString() + BCDToHex((ulong)frame[tHead + 0x24], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 0x25], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 0x26], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 0x27], 2).ToString("00") + "\"";
                        str1 = longitude1;// +"/" + latitude1;
                    }
                }
                else if (recordType == 6 && (command == 0x51 || command == 0x53 || command == 0x61))
                {
                    string longitude1 = BCDToHex((ulong)frame[tHead + 0x21], 2).ToString() + BCDToHex((ulong)frame[tHead + 0x22], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 0x23], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 0x24], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 0x25], 2).ToString("00") + "\"";
                    if (frame[tHead + 0x21] == 0xff) return "";    //经纬度无效时
                    str1 = longitude1;
                }
                else if ((recordType == 7 && command == 0x02) || (recordType == 7 && command == 0x03) || (recordType == 5 && command == 0x21))
                {
                    if (srcPort == 0x11 && dstPort == 0x01 && (recordType == 5 && command == 0x21))
                    {

                    }
                    else
                    {
                        string longitude = BCDToHex((ulong)frame[tHead + 120], 2).ToString() + BCDToHex((ulong)frame[tHead + 121], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 122], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 123], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 124], 2).ToString("00") + "\"";
                        str1 = longitude;
                        if (frame[tHead + 119] == 0x56) str1 = "";
                    }
                   
                }
                else if (recordType == 4)
                {
                    if (srcPort == 0x13 || dstPort == 0x13) return "";
                    if (srcPort == 5 || srcPort == 0x26)
                    {
                        if (command >= 0x21 && command <= 0x23)
                            return BCDToHex((ulong)frame[tHead + 6], 2).ToString() + BCDToHex((ulong)frame[tHead + 7], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 8], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 9], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 10], 2).ToString("00") + "\"";
                        else if ((command == 0x24) || (command == 0x26))
                            return BCDToHex((ulong)frame[tHead + 4], 2).ToString() + BCDToHex((ulong)frame[tHead + 5], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 6], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 7], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 8], 2).ToString("00") + "\"";
                        else if (command == 0x25)
                            return BCDToHex((ulong)frame[tHead + 8], 2).ToString() + BCDToHex((ulong)frame[tHead + 9], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 12], 2).ToString("00") + "\"";
                    }
                    else if (dstPort == 5 || dstPort == 0x26)
                    {
                        if (command >= 21 && command <= 0x25) return BCDToHex((ulong)frame[tHead + 4], 2).ToString() + BCDToHex((ulong)frame[tHead + 5], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 6], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 7], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 8], 2).ToString("00") + "\"";
                        else if (command == 0x26) return BCDToHex((ulong)frame[tHead + 8], 2).ToString() + BCDToHex((ulong)frame[tHead + 9], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 12], 2).ToString("00") + "\"";
                    }
                }
                return str1;
            }
        }
        //----------------------纬度-------------------------
        public string Latitude
        {
            get
            {
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];                         //  源通信地址长度
                byte lenDst = frame[5 + lenSrc + 2];            //目的通信地址长度
                int tHead = 5 + lenSrc + 2 + lenDst + 3;
                string str1 = string.Empty;
                if ((srcPort == 6) && (frame[2] * 0x100 + frame[3] == 0x9e) && frame[tHead + 2] != 0x56)
                {
                    if ((frame[tHead + 0x23] == 0xff) && (frame[tHead + 0x24] == 0xff))
                        str1 = "无效";
                    else
                    {
                        //string longitude1 = BCDToHex((ulong)frame[tHead + 0x23], 2).ToString() + BCDToHex((ulong)frame[tHead + 0x24], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 0x25], 2).ToString("00") + BCDToHex((ulong)frame[tHead + 0x26], 2).ToString("00") + BCDToHex((ulong)frame[tHead + 0x27], 2).ToString("00");
                        string latitude1 = BCDToHex((ulong)frame[tHead + 40], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 0x29], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 0x2a], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 0x2b], 2).ToString("00") + "\"";
                        str1 = /*longitude1 + "/" + */latitude1;
                    }
                }
                else if (recordType == 6 && (command == 0x51 || command == 0x53 || command == 0x61))
                {
                    string latitude1 = BCDToHex((ulong)frame[tHead + 0x26], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 0x27], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 40], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 0x29], 2).ToString("00") + "\"";
                    if (frame[tHead + 0x21] == 0xff) return "";    //经纬度无效时
                    str1 = latitude1;
                }
                else if ((recordType == 7 && command == 0x02) || (recordType == 7 && command == 0x03) || (recordType == 5 && command == 0x21))
                {
                    if (srcPort == 0x11 && dstPort == 0x01 && (recordType == 5 && command == 0x21)) //TAX箱信息
                    {

                    }
                    else
                    {
                        string latitude = BCDToHex((ulong)frame[tHead + 125], 2).ToString("00") + "°" + BCDToHex((ulong)frame[tHead + 126], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 127], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 128], 2).ToString("00") + "\"";
                        str1 = latitude;
                        if (frame[tHead + 119] == 0x56) str1 = "";
                    }
                }
                else if (recordType == 4)
                {
                    if (srcPort == 0x13 || dstPort == 0x13) return "";
                    if (srcPort == 5 || srcPort == 0x26)
                    {
                        if (command >= 0x21 && command <= 0x23)
                            return BCDToHex((ulong)frame[tHead + 11], 2).ToString() + "°" + BCDToHex((ulong)frame[tHead + 12], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 13], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 14], 2).ToString("00") + "\"";
                        else if ((command == 0x24) || (command == 0x26))
                            return BCDToHex((ulong)frame[tHead + 9], 2).ToString() + "°" + BCDToHex((ulong)frame[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 12], 2).ToString("00") + "\"";
                        else if (command == 0x25)
                            return BCDToHex((ulong)frame[tHead + 13], 2).ToString() + "°" + BCDToHex((ulong)frame[tHead + 14], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 15], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 16], 2).ToString("00") + "\"";
                    }
                    else if (dstPort == 5 || dstPort == 0x26)
                    {
                        if (command >= 21 && command <= 0x25) return BCDToHex((ulong)frame[tHead + 9], 2).ToString() + "°" + BCDToHex((ulong)frame[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 12], 2).ToString("00") + "\"";
                        else if (command == 0x26) return BCDToHex((ulong)frame[tHead + 13], 2).ToString() + "°" + BCDToHex((ulong)frame[tHead + 14], 2).ToString("00") + "'" + BCDToHex((ulong)frame[tHead + 15], 2).ToString("00") + "." + BCDToHex((ulong)frame[tHead + 16], 2).ToString("00") + "\"";
                    }
                }
                return str1;
            }
        }
        private static ulong BCDToHex(ulong pbcd, uint num)         //BCD 转 16进制
        {
            int i;
            byte[] tmp = new byte[num];
            ulong pdiv = pbcd;
            for (i = 0; i < num; i++)
            {
                tmp[i] = (byte)(pdiv % 0x10);
                pdiv /= 0x10;
            }
            ulong pmul = 1;
            pdiv = 0;
            for (i = 0; i < num; i++)
            {
                pdiv += pmul * tmp[i];
                pmul *= 10;
            }
            return pdiv;
        }

        //-------------------------------------------------------

        public string OriginData            //原始数据报文
        {
            get
            {
                unsafe
                {
                    byte* p = (byte*)fileMemBase;

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < frameLength; i++)
                    {
                        sb.Append(p[offsetBegin + i].ToString("X2") + " ");
                    }
                    return sb.ToString();
                }
            }
        }


        public string LocationNum
        {
            get
            {
                //hbq
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];                         //  源通信地址长度
                byte lenDst = frame[5 + lenSrc + 2];            //目的通信地址长度
                int tHead = 5 + lenSrc + 2 + lenDst + 3;
                //
                if ((command == 0x21) && (recordType == 0x05))
                {
                    int ln = 0;
                    unsafe
                    {
                        byte* p = (byte*)fileMemBase;
                        ln = p[offsetBegin + frameLength - 23] + (p[offsetBegin + frameLength - 24] << 8);
                    }
                    return ln.ToString("X4");
                }
                else if (recordType == 7 && (command == 0x02 || command == 0x03))
                    return frame[tHead + 115].ToString("X2") + frame[tHead + 116].ToString("X2");

                return "";
            }
        }

        public string CellNum
        {
            get
            {
                //hbq
                byte[] frame = new byte[frameLength];
                unsafe
                {
                    byte* p = (byte*)fileMemBase;
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = p[offsetBegin + i];
                    }
                }
                byte lenSrc = frame[5];                         //  源通信地址长度
                byte lenDst = frame[5 + lenSrc + 2];            //目的通信地址长度
                int tHead = 5 + lenSrc + 2 + lenDst + 3;
                //
                if ((command == 0x21) && (recordType == 0x05))
                {
                    int cn = 0;
                    unsafe
                    {
                        byte* p = (byte*)fileMemBase;
                        cn = p[offsetBegin + frameLength - 21] + (p[offsetBegin + frameLength - 22] << 8);
                    }
                    return cn.ToString("X4");
                }
                else if (recordType == 7 && (command == 0x02 || command == 0x03))
                    return frame[tHead + 117].ToString("X2") + frame[tHead + 118].ToString("X2");
                return "";
            }
        }
    }
}