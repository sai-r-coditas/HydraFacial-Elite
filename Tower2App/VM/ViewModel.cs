
using System.ComponentModel;

namespace Edge.Tower2.UI.VM
{
    public class ViewModel: INotifyPropertyChanged
    {
 
        public event PropertyChangedEventHandler PropertyChanged;
        
        private int _fontSizeSetting = 10;
        public int FontSizeSetting
        {
            get { return _fontSizeSetting; }
            set
            {
                _fontSizeSetting = value;
                OnPropertyChanged("FontSizeSetting");
            }
        }

        private int _VolumeSetting = 10;
        public int VolumeSetting
        {
            get { return _VolumeSetting; }
            set
            {
                _VolumeSetting = value;
                OnPropertyChanged("VolumeSetting");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
