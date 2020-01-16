using PMA_Project.common;
using PMA_Project.Common;

using System;
using System.Collections.ObjectModel;

namespace PMA_Project.Models.Configuration
{
    public class SendFramData : ViewModelBase
    {

        public SendFramData()
        {
            _lstfrmtyp.Add("标准帧");
            _lstfrmtyp.Add("扩展帧");
            _lstfrmdlc.Add("0");
            _lstfrmdlc.Add("1");
            _lstfrmdlc.Add("2");
            _lstfrmdlc.Add("3");
            _lstfrmdlc.Add("4");
            _lstfrmdlc.Add("5");
            _lstfrmdlc.Add("6");
            _lstfrmdlc.Add("7");
            _lstfrmdlc.Add("8");
            _lsttriggertype.Add("手动");
            _lsttriggertype.Add("周期");
        }

        private ObservableCollection<SendFramData> _lstsendframedataviewmodel = new ObservableCollection<SendFramData>();
        private FrameData _objframedatamodel = new FrameData();
        private ObservableCollection<SignalData> _lstsignaldatamodel = new ObservableCollection<SignalData>();
        private ObservableCollection<RawData> _lstrawdatamodel = new ObservableCollection<RawData>();
        private ObservableCollection<string> _lstfrmtyp = new ObservableCollection<string>();
        private ObservableCollection<string> _lstfrmdlc = new ObservableCollection<string>();
        private ObservableCollection<string> _lsttriggertype = new ObservableCollection<string>();
        private ObservableCollection<string> _lstfrmchannelsel = new ObservableCollection<string>();
       // private bool? isselected = false;
        public bool IsCycSndData = false;
        public UInt64 RecordCurTim = 0;
        public ObservableCollection<SendFramData> LstSendFrameData
        {
            get { return _lstsendframedataviewmodel; }
            set
            {
                _lstsendframedataviewmodel = value;
                RaisePropertyChanged("LstSendFrameData");
            }
        }


        public FrameData objFrameDataModel
        {
            get { return _objframedatamodel; }
            set
            {
                _objframedatamodel = value;
                RaisePropertyChanged("objFrameDataModel");
            }
        }


        public ObservableCollection<SignalData> LstSignalDataModel
        {
            get { return _lstsignaldatamodel; }
            set
            {
                _lstsignaldatamodel = value;
                RaisePropertyChanged("LstSignalDataModel");
            }
        }


        public ObservableCollection<RawData> LstRawDataModel
        {
            get { return _lstrawdatamodel; }
            set
            {
                _lstrawdatamodel = value;
                RaisePropertyChanged("LstRawDataModel");
            }
        }


        public ObservableCollection<string> LstFrmTyp
        {
            get { return _lstfrmtyp; }
        }


        public ObservableCollection<string> LstFrmDLC
        {
            get { return _lstfrmdlc; }
        }


        public ObservableCollection<string> LstTriggerType
        {
            get { return _lsttriggertype; }
        }


        public ObservableCollection<string> LstFrmChannelSel
        {
            get { return _lstfrmchannelsel; }
            set
            {
                _lstfrmchannelsel = value;
                RaisePropertyChanged("LstFrmChannelSel");
            }
        }


        //public bool? IsSelected
        //{
        //    get { return isselected; }
        //    set { isselected = value; RaisePropertyChanged("IsSelected"); }
        //}


    }


}
