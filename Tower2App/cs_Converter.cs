using System;
using System.Windows;
using System.Windows.Data;

namespace Edge.Tower2.UI
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //switch (value.ToString().ToLower())
            //{
            //    case "yes":
            //        return true;
            //    case "no":
            //        return false;

            //    default:
            //        return Binding.DoNothing;
            //}

            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            return Visibility.Hidden;
        }
    }
}
