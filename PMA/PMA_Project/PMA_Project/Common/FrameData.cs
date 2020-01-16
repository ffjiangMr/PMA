using PMA_Project.Models;

using System;

namespace PMA_Project.common
{
    public class FrameData : ViewModelBase
    {
        public int _index = 0;//序号
        public string _msgnam = "";//报文名字
        public string _triggertype = "";//触发类型
        public string _cycletime = "100";//周期
        public bool _isenabled = false;
        public string _id = "";
        public string _channel = "";//通道
        public string _frametype = "";//帧类型
        public string _dlc = "";//DLC
        public string _sndnam = "发送";//发送

        public int Index
        {
            get { return _index; }
            set { _index = value; RaisePropertyChanged("Index"); }
        }

        public string MsgNam
        {
            get { return _msgnam; }
            set { _msgnam = value; RaisePropertyChanged("MsgNam"); }
        }

        public string TriggerType
        {
            get { return _triggertype; }
            set { _triggertype = value; RaisePropertyChanged("TriggerType"); }
        }

        public string CycleTime
        {
            get { return _cycletime; }
            set { _cycletime = value; RaisePropertyChanged("RecordPath"); }
        }

        public bool IsEnabled
        {
            get { return _isenabled; }
            set { _isenabled = value; RaisePropertyChanged("IsEnabled"); }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("Id"); }
        }

        public string ChannelName
        {
            get { return _channel; }
            set { _channel = value; RaisePropertyChanged("ChannelName"); }
        }

        public string FrameType
        {
            get { return _frametype; }
            set { _frametype = value; RaisePropertyChanged("FrameType"); }
        }

        public string DLC
        {
            get { return _dlc; }
            set
            {
                _dlc = value; RaisePropertyChanged("Dlc");
            }
        }

        public string SndNam
        {
            get { return _sndnam; }
            set
            {
                _sndnam = value; RaisePropertyChanged("SndNam");
            }
        }

        public Byte[] RawData = new Byte[8];

    }

}
