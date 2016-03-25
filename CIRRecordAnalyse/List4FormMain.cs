using System;
using System.Collections.Generic;
using System.Text;
using CIRRecordAnalyse.Core;
using System.IO;

namespace CIRRecordAnalyse
{
    class ListInfo
    {
        string pathName = "";
        string fullPath = "";
        DateTime stateTimeStart = DateTime.Now;
        DateTime stateTimeEnd = DateTime.Now;
        DateTime waveTimeStart = DateTime.Now;
        DateTime waveTimeEnd = DateTime.Now;
        bool isInUse = true;

        public bool IsLoaded
        {
            get { return isInUse; }
            set { isInUse = value; }
        }

        public string RecordPath
        {
            get { return pathName; }
        }

        public string FullPath
        {
            get { return fullPath; }
        }

        public DateTime VoiceBeginTime
        {
            get { return waveTimeStart; }
        }
        public DateTime VoiceEndTime
        {
            get { return waveTimeEnd; }
        }

        public DateTime StatusBeginTime
        {
            get { return stateTimeStart; }
        }

        public DateTime StatusEndTime
        {
            get { return stateTimeEnd; }
        }


        public string UsingState
        {
            get { return isInUse == true ? "使用中" : ""; }
            
        }

        public void AddNewRecordInfo(RecordManager rm)
        {
            this.stateTimeStart = rm.StatusBeginTime;
            this.stateTimeEnd = rm.StatusEndTime;
            this.waveTimeStart = rm.VoiceBeginTime;
            this.waveTimeEnd = rm.VoiceEndTime;
            this.pathName = rm.RecordPath;
            this.fullPath = rm.FullPath;
        }

    }
}