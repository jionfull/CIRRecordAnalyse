﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CIRRecordAnalyse.Utilities;

namespace CIRRecordAnalyse.Core
{
    public class RecordVoice
    {

        const int VoiceBlockOffset = 2001;
        const int VoiceBlockEnd = 2099999;
        DateTime recordTime;
        uint startBlock = 0;
        uint endBlock = 0;
        uint blockCount = 0;
        float timeLen = 0;
        FileStream fs;
        public RecordVoice(DateTime time, uint blockStart, uint blockCount, FileStream fs)
        {
            recordTime = time;
            this.startBlock = blockStart;
            this.blockCount = blockCount;
            endBlock = startBlock + blockCount - 1;
            if (endBlock > 0x200b1f)
            {
                endBlock = (endBlock - 0x200b1f) + 0x7d0;
            }
            timeLen = ((blockCount * 512f) * 4f) / 16000f;
            this.fs = fs;
        }

        public DateTime RecordTime
        {
            get { return recordTime; }
        }

        //Boki[130924]:
        public string RecordTimeString
        {
            get
            {
                if (RecordTime == (new DateTime()))
                    return "无效时间";
                else
                    return recordTime.ToString(@"yyyy-MM-dd hh:mm:ss");
            }
        }

        public uint StartBlock
        {
            get { return startBlock; }
        }

        public uint BlockCount
        {
            get { return blockCount; }
        }

        public bool IntersectsWith(RecordVoice record)
        {

            if (startBlock + blockCount <= record.startBlock) return false;
            if (record.startBlock + record.blockCount <= startBlock) return false;
            if (recordTime == record.recordTime) return false;
            return true;
        }


        public uint EndBlock
        {
            get { return endBlock; }
        }


        public float TimeLength
        {
            get { return timeLen; }
        }


        public bool IsValid
        {
            get
            {
                if (fs == null) return false;
                if (fs.Length < (startBlock - 2001) * 512) return false;

                return true;
            }
        }

        public string ValidState
        {
            get
            {
                //Boki[130924]:
                if (RecordTimeString == "无效时间")
                    return "无效";
                return IsValid ? "有效" : "无效";
            }
        }

        public bool CreateWaveFile(string path)
        {
            if (IsValid==false) return false;

            byte[] buffer = new byte[blockCount * 512];

            if (startBlock + blockCount -1<= VoiceBlockEnd)
            {

                fs.Position = (startBlock - VoiceBlockOffset) * 512;
                fs.Read(buffer, 0, buffer.Length);

            }
            else
            {
 
            }

            byte[] savearray =Helper.VoiceEncode.G726ToPCMDecode(buffer, buffer.Length);
            SavePCMWaveFile(savearray, path);


            return true;
        }

        public void SavePCMWaveFile(byte[] readbyte, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] headbyte = new byte[0x2c];
                headbyte.Initialize();
                int len = Encoding.Default.GetBytes("RIFF", 0, 4, headbyte, 0);
                uint i = (uint)((readbyte.Length + 0x2c) - 8);
                Array.Copy(BitConverter.GetBytes(i), 0, headbyte, 4, 4);
                len = Encoding.Default.GetBytes("WAVEfmt ", 0, 8, headbyte, 8);
                i = 0x10;
                Array.Copy(BitConverter.GetBytes(i), 0, headbyte, 0x10, 4);
                ushort j = 1;
                Array.Copy(BitConverter.GetBytes(j), 0, headbyte, 20, 2);
                j = 1;
                Array.Copy(BitConverter.GetBytes(j), 0, headbyte, 0x16, 2);
                i = 0x1f40;
                Array.Copy(BitConverter.GetBytes(i), 0, headbyte, 0x18, 4);
                i = 0x3e80;
                Array.Copy(BitConverter.GetBytes(i), 0, headbyte, 0x1c, 4);
                j = 2;
                Array.Copy(BitConverter.GetBytes(j), 0, headbyte, 0x20, 2);
                i = 0x10;
                Array.Copy(BitConverter.GetBytes(i), 0, headbyte, 0x22, 2);
                len = Encoding.Default.GetBytes("data", 0, 4, headbyte, 0x24);
                i = (uint)readbyte.Length;
                Array.Copy(BitConverter.GetBytes(i), 0, headbyte, 40, 4);
                bw.Write(headbyte);
                bw.Write(readbyte);
                bw.Close();
                fs.Close();
            }
        }



    }
}