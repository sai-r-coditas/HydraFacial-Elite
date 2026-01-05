using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using System.Windows.Media.Imaging;

namespace Edge.Tower2.UI
{
    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }

    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed) { }
    }
    public sealed class BooleanToInvertedVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToInvertedVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible) { }
    }

    public class BooleanToStringConverter : IValueConverter  // 0102-28
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "ready";
                else
                    return "off";
            }
            return "off";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           
            switch (value.ToString().ToLower())
            {
                case "ready":
                    return true;
                case "off":
                    return false;
            }
            return false;
        }
    }

    //public sealed class BooleanToVisibility1 : IValueConverter  // 0102-28
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value is Visibility && (Visibility)value == Visibility.Visible)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value is bool)
    //        {
    //            if ((bool)value == true)
    //                return Visibility.Visible;
    //            else
    //                return Visibility.Hidden;
    //        }
    //        return Visibility.Hidden;
    //    }
    //}

    #region ImageCacheConverter
    // 0102-39 <Image Source="{Binding MyImageUrl, Converter={StaticResource uriToImageConv}}"/>
    public class UriToCachedImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (!string.IsNullOrEmpty(value.ToString()))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(value.ToString());
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                return bi;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }
    }
    #endregion
}
