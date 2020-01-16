using Neusoft.Reach.CANComponent.Infrastructure;

using PMA_Project.common;
using PMA_Project.Models.Configuration;
using PMA_Project.Other;

using System;
using System.Collections.Generic;

namespace PMA_Project.Models
{
    public class OneChannelDB
    {
        public string ChanNam;
        public int ChanNum;
        public List<ReceiveObject> ObjRcvDB;
    }
    internal class WorkSpace : ViewModelBase
    {
       public static List<String> SignalDefaultValueDescription = new List<string>(){ "off", "not allow", "not  communication", "normal", "closed", "not weld",
                                                               "not  connecte","not  communication","not connected","no crash"};

        public UltraHighAccurateTimer objUltraHighAccurateTimer_SndData = new UltraHighAccurateTimer();

        public WorkSpace()
        {
            this.VirtualNode = new ChannelCfg();
            this.BMSSts = new Common.SignalData();
            BMSSts.Name = "BMSSts";
         }

        static WorkSpace _this = new WorkSpace();
        public static WorkSpace This
        {
            get { return _this; }
        }

        public ChannelCfg VirtualNode { get; set; }

        public SendFramData ObjSendFrameData;

        private String filePath;
        public String FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
                RaisePropertyChanged("FilePath");
            }
        }

        private Int32 lintLimit;

        /// <summary>
        /// 记录最大行数
        /// </summary>
        public Int32 LineLimit
        {
            get
            {
                return this.lintLimit;
            }
            set
            {
                this.lintLimit = value;
                RaisePropertyChanged("LineLimit");
            }
        }

        private Int32 interval;

        /// <summary>
        /// 每条记录间隔时间ms
        /// </summary>
        public Int32 Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                this.interval = value;
                RaisePropertyChanged("Interval");
            }
        }

        private String fileName;

        /// <summary>
        /// 保存的文件名
        /// </summary>
        public String FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
                RaisePropertyChanged("FileName");
            }
        }
        //BMS通信状态
        public PMA_Project.Common.SignalData BMSSts ;
      
        #region thread lock

        public object SendDataCycleLock = new object();

        #endregion

        public bool IsRunSndThd = false;
        public bool bSysSts_StartStop = false;  //false:driver close    true:diver open       
        public RecordOp ObjRecordOp_Write = new RecordOp();
        public MainWindow MainWindow;
    }
}
