using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Edge.Tower2.UI.Videos
{
    public partial class Videos 
    {
        public Videos()
        {
            DataContext = new VideosModel(this);
            InitializeComponent();
            Loaded += Window1_Loaded;

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
        }

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        private void OnEnter()
        {
            Outputs.LogHeader("Videos", "Enter");
        }

        private void OnLeave()
        {
            VideoListView.UnselectAll();
            MediaPlayer.Stop();
            MediaPlayer.Close();
            Model.Opened = false;
            Model.IsPlaying = false;
            _timer.Stop();
            ProgressBarWithText.Visibility = Visibility.Collapsed;
            ProgressBar.Value = 0;

            Outputs.LogHeader("Videos", "Exit");
        }

        private void VideoListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoListView.SelectedItems.Count == 1)
            {
                var video = (Video)VideoListView.SelectedItem;
                Model.VideoSource = video.Path;

                if (Model.IsPlaying)
                {
                    MediaPlayer.Stop();
                    Model.Opened = false;
                    Model.IsPlaying = false;
                    _timer.Stop();
                }

                MediaPlayer.Play();
                Model.IsPlaying = true;
                ProgressBarWithText.Visibility = Visibility.Visible;
            }

            MediaPlayer.Visibility = (VideoListView.SelectedItems.Count == 1)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private VideosModel Model { get { return ((VideosModel) DataContext); } }

        private void MediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            //MediaPlayer.Play();
            //Model.IsPlaying = true;
        }

        private void MediaPlayer_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ResetButtonFadingDelay();
        }

        private void ResetButtonFadingDelay()
        {
            Model.IsPlaying = !Model.IsPlaying;
            Model.IsPlaying = !Model.IsPlaying;
        }

        private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            Model.Opened = true;

            ProgressBarWithText.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            Model.Duration = MediaPlayer.NaturalDuration.TimeSpan;
            ProgressBar.Maximum = Model.Duration.TotalMilliseconds;
            _timer.Tick += (o, args) =>
            {
                Model.Position = MediaPlayer.Position;
                ProgressBar.Value = MediaPlayer.Position.TotalMilliseconds;
            };
            _timer.Start();
        }

        private void MediaPlayer_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            Model.IsPlaying = false;
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Pause();
            Model.IsPlaying = false;
        }

        private void SeekBackButton_OnClick(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = TimeSpan.Zero;
            MediaPlayer.Pause();
            Model.IsPlaying = false;
        }

        private const int SkipAmount = 10;

        private void SkipBackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Position.TotalSeconds <= SkipAmount)
            {
                MediaPlayer.Position = TimeSpan.Zero;   
            }
            else
            {
                MediaPlayer.Position -= TimeSpan.FromSeconds(SkipAmount);
            }

            ResetButtonFadingDelay();
        }

        private void SkipForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Position.TotalSeconds >= MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds - SkipAmount)
            {
                MediaPlayer.Position = MediaPlayer.NaturalDuration.TimeSpan;
            }
            else
            {
                MediaPlayer.Position += TimeSpan.FromSeconds(SkipAmount);
            }
            
            ResetButtonFadingDelay();
        }

        private void SeekForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = MediaPlayer.NaturalDuration.TimeSpan;
            MediaPlayer.Pause();
            Model.IsPlaying = false;
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Play();
            Model.IsPlaying = true;
        }
    }
}