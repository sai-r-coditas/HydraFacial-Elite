using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Edge.Tower2.UI.Videos
{
    public class Video
    {
        public string Path { get; internal set; }
    }

    public class VideosModel : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;
        private readonly ObservableCollection<Video> _videos = new ObservableCollection<Video>();
        private readonly Videos _videosWindow;
        private TimeSpan _duration;
        private bool _isPlaying;
        private bool _opened;
        private TimeSpan _position;
        private string _videoSource;

        public VideosModel(Videos videosWindow)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Videos"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Videos");

            var fsw = new FileSystemWatcher(Environment.CurrentDirectory + "\\Videos");

            _videosWindow = videosWindow;
            _dispatcher = Dispatcher.CurrentDispatcher;

            Videos = new ReadOnlyObservableCollection<Video>(_videos);

            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory + "\\Videos"))
            {
                var fi = new FileInfo(file);

                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    _videos.Add(new Video {Path = file});
                }
            }

            fsw.Created += (sender, args) =>
            {
                Thread.Sleep(300);
                _dispatcher.Invoke((Action) (() =>
                {
                    var video = new Video {Path = args.FullPath};
                    _videos.Add(video);
                    _videosWindow.VideoListView.ScrollIntoView(video);
                    _videosWindow.VideoListView.UnselectAll();
                }));
            };

            fsw.Deleted +=
                (sender, args) =>
                    _dispatcher.Invoke((Action) (() => _videos.Remove(_videos.First(p => p.Path == args.FullPath))));

            fsw.EnableRaisingEvents = true;
        }

        public ReadOnlyObservableCollection<Video> Videos { get; private set; }

        public string VideoSource
        {
            get { return _videoSource; }
            internal set
            {
                _videoSource = value;
                OnPropertyChanged("VideoSource");
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (value.Equals(_isPlaying)) return;
                _isPlaying = value;
                OnPropertyChanged("IsPlaying");
            }
        }

        public TimeSpan Duration
        {
            get { return Opened ? _duration : TimeSpan.Zero; }
            set
            {
                if (value.Equals(_duration)) return;
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public bool Opened
        {
            get { return _opened; }
            set
            {
                if (value.Equals(_opened)) return;
                _opened = value;
                OnPropertyChanged("Opened");
            }
        }

        public TimeSpan Position
        {
            get { return TimeSpan.FromSeconds((int) _position.TotalSeconds); }
            set
            {
                if (value.Equals(_position)) return;
                _position = value;
                OnPropertyChanged("Position");
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