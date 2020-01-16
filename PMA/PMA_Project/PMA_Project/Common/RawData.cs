using PMA_Project.Models;

namespace PMA_Project.Common
{
    public class RawData : ViewModelBase
    {
        public string _zero;
        public string _first;
        public string _second;
        public string _three;
        public string _four;
        public string _five;
        public string _six;
        public string _seven;
        public bool _focus = false;

        public bool Focus
        {
            get { return _focus; }
            set { _focus = value; RaisePropertyChanged("Focus"); }
        }

        public string ZeroByte
        {
            get { return _zero; }
            set { _zero = value; RaisePropertyChanged("ZeroByte"); }
        }

        public string FirstByte
        {
            get { return _first; }
            set { _first = value; RaisePropertyChanged("FirstByte"); }
        }

        public string SecondByte
        {
            get { return _second; }
            set { _second = value; RaisePropertyChanged("SecondByte"); }
        }

        public string ThreeByte
        {
            get { return _three; }
            set { _three = value; RaisePropertyChanged("ThreeByte"); }
        }

        public string FourByte
        {
            get { return _four; }
            set { _four = value; RaisePropertyChanged("FourByte"); }
        }

        public string FiveByte
        {
            get { return _five; }
            set { _five = value; RaisePropertyChanged("FiveByte"); }
        }

        public string SixByte
        {
            get { return _six; }
            set { _six = value; RaisePropertyChanged("SixByte"); }
        }

        public string SevenByte
        {
            get { return _seven; }
            set { _seven = value; RaisePropertyChanged("SevenByte"); }
        }
    }
}
