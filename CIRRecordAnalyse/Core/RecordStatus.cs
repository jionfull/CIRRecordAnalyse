using System;
using System.Collections.Generic;
using System.Text;
using CIRRecordAnalyse.Utilities;

namespace CIRRecordAnalyse.Core
{
   public class RecordStatus
    {
        

        DateTime recordTime;
        char version;
        byte status ;
        ushort voltage ;
        public RecordStatus(DateTime time,byte status,ushort voltage,char version)
        {
            this.recordTime = time;
            this.status = status;
            this.voltage = voltage;
            this.version = version;
        }

        public DateTime RecordTime
        {
            get { return recordTime; }
        }

        public string ExternPower
        {
            get
            {
                return (status & 0x01) == 0 ? "无效" : "有效";
            }
        }
        public string RecordSignal
        {
            get
            {
                return ((status >> 1) & 0x01) == 0 ? "录音中" : "无录音";
            }
        }

        public string ResetSignal
        {
            get
            {
                return ((status >> 2) & 0x01) == 0 ? "低电平" : "高电平";
            }
        }

        public string GPSData
        {
            get
            {
                return ((status >> 3) & 0x01) == 0 ? "正常" : "故障";
            }
        }

        public string GPSStatus
        {
            get
            {
                return ((status >> 4) & 0x01) == 0 ? "可用" : "无效";
            }
        }
        public string MainUnit
        {
            get
            {
                return ((status >> 5) & 0x01) == 0 ? "正常" : "故障";
            }
        }

        public string BatteryStatus
        {
            get
            {
                return ((status >> 6) & 0x01) == 0 ? "正常" : "故障";
            }
        }

        public string BatteryVoltage
        {
            get
            {
                if (version == 0)
                {
                    return Helper.BCD2Int((byte)(voltage & 0xff)) + "." + Helper.BCD2Int((byte)((voltage >> 8) & 0xff));
                }
                else
                {
                    return (voltage / 1000) + "." + (voltage % 1000);
                }
            }
        }



    }
}
