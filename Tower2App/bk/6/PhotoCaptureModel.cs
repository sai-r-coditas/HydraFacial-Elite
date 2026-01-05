using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
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
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Photos"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Photos");

            fsw = new FileSystemWatcher(Environment.CurrentDirectory + "\\Photos");

            _photoCapture = photoCapture;
            _dispatcher = Dispatcher.CurrentDispatcher;

            Photos = new ReadOnlyObservableCollection<Photo>(_photos);

            // sww add
            string strPath = Environment.CurrentDirectory + "\\Photos\\"; 

            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory + "\\Photos"))
            {
                var fi = new FileInfo(file);
                
                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    // sww
                    //_photos.Add(new Photo { Path = file, CreateDateTime = fi.CreationTime });

                    _photos.Add(new Photo { Path = file, CreateDateTime = fi.CreationTime, Info = Utility.Lib.getFileName(file, strPath.Length), Notes = getNotes("1") });
                }
            }

            // ??? for ListView
            fsw.Created += (sender, args) =>
            {
                Thread.Sleep(300);
                _dispatcher.Invoke((Action)(() =>
                {
                    var photo = new Photo { Path = args.FullPath, CreateDateTime = DateTime.Now };
                    //sww
                    //var photo = new Photo { Path = args.FullPath, CreateDateTime = DateTime.Now, getFileName(args.FullPath, pathlefth.Length) };
                    _photos.Add(photo); ;
                    _photoCapture.PhotoListView.ScrollIntoView(photo);
                    _photoCapture.PhotoListView.UnselectAll();
                    
                    //_photoCapture.DisplayCapturePhoto();
                    _photoCapture.test();


                }));
            };

            fsw.Deleted += (sender, args) => _dispatcher.Invoke((Action)(() => _photos.Remove(_photos.First(p => p.Path == args.FullPath))));
            fsw.EnableRaisingEvents = true;
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