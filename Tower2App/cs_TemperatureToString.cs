using System;
using System.Globalization;
using System.Windows.Data;

namespace Edge.Tower2.UI
{
    [ValueConversion(typeof(int), typeof(string))]
    public class TemperatureToString : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return "";

            return string.Format("{0}°", (int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(((string)value).Substring(0, ((string)value).Length-1));
        }

        #endregion
    }
}