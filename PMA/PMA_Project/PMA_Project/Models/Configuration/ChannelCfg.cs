using Neusoft.Reach.DBCAnalysis.Handle;

using System.Collections.ObjectModel;

namespace PMA_Project.Models.Configuration
{
    //通道配置
    public class ChannelCfg : ViewModelBase
    {
        //private string _channelName;
        //private ObservableCollection<ChannelCfg> _child = new ObservableCollection<ChannelCfg>();

        private ChannelCfgDevCfg _channelCfgDevCfg = new ChannelCfgDevCfg(); //通道设备
        private ObservableCollection<DBCHandler> _lst_RcvDBCHandler = new ObservableCollection<DBCHandler>();

        //public string ChannelName
        //{
        //    get { return _channelName; }
        //    set
        //    {
        //        _channelName = value;
        //        RaisePropertyChanged("ChannelName");
        //    }
        //}

        //通道名
        //public ObservableCollection<ChannelCfg> Child
        //{
        //    get { return _child; }
        //    set
        //    {
        //        _child = value;
        //        RaisePropertyChanged("Child");
        //    }
        //}

        public ChannelCfgDevCfg ObjChannelCfgDevCfg//通道设备
        {
            get { return _channelCfgDevCfg; }
            set
            {
                _channelCfgDevCfg = value;
                RaisePropertyChanged("ObjChannelCfgDevCfg");
            }
        }

        public ObservableCollection<DBCHandler> Lst_RcvDBCHandler
        {
            get { return _lst_RcvDBCHandler; }
            set
            {
                _lst_RcvDBCHandler = value;
                RaisePropertyChanged("Lst_RcvDBCHandler");
            }
        }

    }

    //通道设备配置
    public class ChannelCfgDevCfg : ViewModelBase
    {
        private int devIndex;
        private string devname = "";//接口卡类型
        private int devid = 0;//设备号
        private int devcha = 0;

        public int DevIndex
        {
            get { return devIndex; }
            set
            {
                devIndex = value;
                RaisePropertyChanged("DevIndex");
            }
        }
        public string DevName
        {
            get { return devname; }
            set
            {
                devname = value;
                RaisePropertyChanged("DevName");
            }
        }

        public int DevID
        {
            get { return devid; }
            set
            {
                devid = value;
                RaisePropertyChanged("DevID");
            }
        }

        public int DevCha
        {
            get { return devcha; }
            set
            {
                devcha = value;
                RaisePropertyChanged("DevCha");
            }
        }
    }
}

