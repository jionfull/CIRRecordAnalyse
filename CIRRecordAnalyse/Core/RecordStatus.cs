using System;
using System.Collections.Generic;
using System.Text;
using CIRRecordAnalyse.Utilities;

namespace CIRRecordAnalyse.Core
{
   public class RecordStatus
    {
        

        DateTime recordTime;

        byte status ;
        ushort voltage ;
        public RecordStatus(DateTime time,byte status,ushort voltage)
        {
            this.recordTime = time;
            this.status = status;
            this.voltage = voltage;
        }

        public DateTime RecordTime
        {
            get { return recordTime; }
        }

        public string ExternPower
        {
            get
            {
                return (status & 0x01) == 0 ? "低电平" : "高电平";
            }
        }
        public string RecordSignal
        {
            get
            {
                return ((status >> 1) & 0x01) == 0 ? "低电平" : "高电平";
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
                return Helper.BCD2Int((byte)(voltage & 0xff)) + "." + Helper.BCD2Int((byte)((voltage >> 8) & 0xff));
            }
        }



    }
}
