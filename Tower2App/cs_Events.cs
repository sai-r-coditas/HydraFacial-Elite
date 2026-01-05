using System.ComponentModel;
using System.Windows.Data;


using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;
using System.Globalization;


namespace Edge.Tower2.UI
{
    public class cs_Events : INotifyPropertyChanged                                             // 0102-01
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        private string _wifiState;
        public string WifiState
        {
            get { return _wifiState; }
            set
            {
                if (value != _wifiState)
                {
                    _wifiState = value;
                    OnPropertyChanged("WifiState");
                }
            }
        }
    }

    public class cs_Events_User : INotifyPropertyChanged                                        // 0102-01
    {
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        private bool _singleUser;
        public bool SingleUser
        {
            get { return _singleUser; }
            set
            {
                if (value != _singleUser)
                {
                    _singleUser = value;
                    OnPropertyChanged("UserManagement");
                }
            }
        }
    }

    public class cs_Events_Language : INotifyPropertyChanged                                    // 0106-15
    {
        private string _language;
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged("NewLanguage");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class ProgressForegroundConverter : IValueConverter                                  // 0106-18
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double progress = (double)value;
            Brush foreground = Brushes.Green;

            if (progress >= 90d)
            {
                foreground = Brushes.Green;
            }
            else if (progress >= 60d)
            {
                foreground = Brushes.Yellow;
            }
            else if (progress >= 30d)
            {
                foreground = Brushes.Orange;
            }
            else if (progress >= 0d)
            {
                foreground = Brushes.Red;
            }

            return foreground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
