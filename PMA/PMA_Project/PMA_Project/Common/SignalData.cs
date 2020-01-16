using Neusoft.Reach.DBCAnalysis.Infrastructure;

using PMA_Project.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PMA_Project.Common
{
    public class SignalData: ViewModelBase
    {
        public SignalData()
        {
        }

        public string _name;
        public UInt32 _rawdata;
        public float _physicaldata;
        public string _strUnit;
        public UInt32 _beginbit;
        public UInt32 _length;
        public string _valdesc;
        //
        // Summary:
        //     The multiplexer indicator defines whether the signal is a normal signal,
        //     a multiplexer switch for multiplexed signals, or a multiplexed signal.  针对359信号
        public MultiplexerIndicator MultiplexerIndicator { get; set; }
        //
        // Summary:
        //     The multiplexed signal is transferred in the message if the switch value
        //     of the multiplexer signal is equal to its multiplexer_switch_value.
        public ushort MultiplexerSwitchValue { get; set; }
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        public UInt32 RawData
        {
            get { return _rawdata; }
            set { _rawdata = value; RaisePropertyChanged("RawData"); }
        }

        public string ValDesc
        {
            get { return _valdesc; }
            set { _valdesc = value; RaisePropertyChanged("ValDesc"); }
        }

        public float PhysicalData
        {
            get { return _physicaldata; }
            set { _physicaldata = value; RaisePropertyChanged("PhysicalData"); }
        }

        public string StrUnit
        {
            get { return _strUnit; }
            set { _strUnit = value; RaisePropertyChanged("StrUnit"); }
        }

        public UInt32 BeginBit
        {
            get { return _beginbit; }
            set { _beginbit = value; RaisePropertyChanged("BeginBit"); }
        }

        public UInt32 Length
        {
            get { return _length; }
            set { _length = value; RaisePropertyChanged("Length"); }
        }

        public float fFact = 1;
        public float fOffSet = 0;
        public UInt32 nOldRawVal = 0;
        public ByteOrder eEndi;

        private List<string> _lstvaldesc = new List<string>();
        public List<string> LstValDesc
        {
            get { return _lstvaldesc; }
            set
            {
                _lstvaldesc = value;
                RaisePropertyChanged("LstValDesc");
            }
        }
        public List<UInt32> LstSigVal = new List<uint>();//意义型数据取值
   }
}
