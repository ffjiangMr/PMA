using System;
using System.Globalization;
using System.Windows.Data;

namespace PMA_Project.Other
{
    public class unit_mvToV_Converter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 1000;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 1000;
        }
    }
    public class DV_CAN_RlyCtrlAllowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;
            if (temp == 0) return "Normal";
            else if (temp == 1) return "OBC_PreRly_CtrlAllow";
            else if (temp == 2) return "OBC_PosRly_CtrlAllow";
            else if (temp == 3) return "OBC_NegRly_CtrlAllow";
            else if (temp == 4) return "FCH_PreRly_CtrlAllow";
            else if (temp == 5) return "FCH_PosRly_CtrlAllow";
            else if (temp == 6) return "DCH_PreRly_CtrlAllow";
            else if (temp == 7) return "DCH_PosRly_CtrlAllow";
            else if (temp == 8) return "DCH_NegRly_CtrlAllow";
            else if (temp == 9) return "Reserved";
            else if (temp == 10) return "All Rly can be contrled";
            else
                return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String str = (String)value;
            switch (str)
            {
                case "Normal": return 0;
                case "OBC_PreRly_CtrlAllow": return 1;
                case "OBC_PosRly_CtrlAllow": return 2;
                case "OBC_NegRly_CtrlAllow": return 3;
                case "FCH_PreRly_CtrlAllow": return 4;
                case "FCH_PosRly_CtrlAllow": return 5;
                case "DCH_PreRly_CtrlAllow": return 6;
                case "DCH_PosRly_CtrlAllow": return 7;
                case "DCH_NegRly_CtrlAllow": return 8;
                case "Reserved": return 9;
                case "All Rly can be contrled": return 10;
                default: return 0;
            }
        }

    }
    public class RadioButton_CC_IsoResStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;
            return System.Convert.ToDouble(parameter) == temp;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean temp = (Boolean)value;
            var result = 0;
            switch (temp)
            {
                case true:
                    result = 1;
                    break;
                case false:
                    result = 0;
                    break;
            }
            return result;
        }

    }
    public class RadioButton_ZeroAsTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;
            return temp < 1;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean temp = (Boolean)value;
            var result = 1;
            switch (temp)
            {
                case true:
                    result = 0;
                    break;

                case false:
                    result = 1;
                    break;
            }
            return result;
        }
    }
    public class RadioButton_ZeroAsFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = (double)value;
            return temp >= 1;

        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean temp = (Boolean)value;
            var result = 0;
            switch (temp)
            {
                case true:
                    result = 1;
                    break;
                case false:
                    result = 0;
                    break;
            }
            return result;
        }
    }

    //不用
    public class FrmtypeConverter0 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String temp = (String)value;
            switch (temp)
            {
                case "eStdFrm":
                    return "标准帧";

                case "eExtFrm":
                    return "扩展帧";
                default:
                    return "未标识";
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string temp = value.ToString();
            switch (temp)
            {
                case "标准帧":
                    return "eStdFrm";

                case "扩展帧":
                    return "eExtFrm";
                default:
                    return "";
            }
        }





    }
}

