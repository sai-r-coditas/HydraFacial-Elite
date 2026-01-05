using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Edge.Tower2.UI.PhotoCapture
{
    public class Photo
    {
        public string Path { get; internal set; }
        public DateTime CreateDateTime { get; internal set; }
        //sww add
        public string Info { get; internal set; }
        public string Notes { get; internal set; }
    }

    public class PhotoCaptureModel : INotifyPropertyChanged
    {
        FileSystemWatcher fsw;

        private readonly ObservableCollection<Photo> _photos = new ObservableCollection<Photo>();
        public ReadOnlyObservableCollection<Photo> Photos { get; private set; }

        private int _numberOfPhotos;
        
        public int NumberOfPhotos
        {
            get
            {
                return _numberOfPhotos;
            }
            set
            {
                _numberOfPhotos = value;
                OnPropertyChanged("NumberOfPhotos");
            }
        }

        private Dispatcher _dispatcher;
        private PhotoCapture _photoCapture;

        public PhotoCaptureModel(PhotoCapture photoCapture)
        {
        }

        //sww
        private string getNotes(string ID)
        {
            return "NNN...";
        }

        private BitmapSource _largeSelectedImage;
        private BitmapSource _largeSelectedImage2;

        public BitmapSource LargeSelectedImage
        {
            get
            {
                return _largeSelectedImage;

            }
            internal set
            {
                _largeSelectedImage = value;
                OnPropertyChanged("LargeSelectedImage");
            }
        }

        public BitmapSource LargeSelectedImage2
        {
            get
            {
                return _largeSelectedImage2;

            }
            internal set
            {
                _largeSelectedImage2 = value;
                OnPropertyChanged("LargeSelectedImage2");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}