using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Edge.Tower2.UI.PhotoCapture
{
    public class Client: INotifyPropertyChanged
    {
        private string _phoneNumber;
        private string _name;
        private string _path;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (value == _phoneNumber) return;
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (value == _path) return;
                _path = value;

                var lastSpaceIndex = _path.LastIndexOf(" ", StringComparison.InvariantCulture);
                var lastBackslashIndex = _path.LastIndexOf(@"\", StringComparison.InvariantCulture);
                
                PhoneNumber = _path.Substring(lastSpaceIndex + 1);
                Name = _path.Substring(lastBackslashIndex + 1, lastSpaceIndex-lastBackslashIndex-1);

                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}