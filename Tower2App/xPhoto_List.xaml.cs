using System;
using System.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Media.Imaging;

// add by sww
using System.IO;
using System.ComponentModel;
using System.Drawing;
using DirectShowLib;
using JetBrains.Annotations;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for Photo_Comparison.xaml
    /// </summary>
    public partial class Photo_List
    {

        [NotNull]
        private App App
        {
            get { return (App)Application.Current; }
        }

        public DsDevice CameraDevice { get; private set; }
        private bool _captureRequested;

        public Photo_List()
        {
            CameraDevice = MultimediaUtil.VideoInputDevices.FirstOrDefault(device => device.Name == Settings.CameraDeviceName);
            //DataContext = new PhotoCapture.PhotoCaptureModel(this);

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
            VideoCapturePanel.NewVideoSample += videoCapElement_NewVideoSample;
        }

        private void OnEnter()
        {
            Outputs.LogHeader("ImageCapture", "Enter");
            VideoCapturePanel.Play();
        }

        private void OnLeave()
        {
            PhotoListView.UnselectAll();
            VideoCapturePanel.Stop();

            Outputs.LogHeader("ImageCapture", "Exit");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //public DsDevice CameraDevice { get; private set; }
        //private bool _captureRequested;

        private void videoCapElement_NewVideoSample(object sender, VideoSampleArgs e)
        {
            if (_captureRequested)
            {
                _captureRequested = false;

                var bitmap = (Bitmap)e.VideoFrame.Clone();
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                if (!Directory.Exists(ControlParams.Params.Photos_Default))
                {
                    Directory.CreateDirectory(ControlParams.Params.Photos_Default);
                }

                // sww
                bitmap.Save(ControlParams.Params.Photos_Default+"\\" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss.fff") + "_Profile" + ".edge", System.Drawing.Imaging.ImageFormat.Jpeg);
                bitmap.Dispose();

                CaptureButton.Dispatcher.Invoke((Action)(() => { CaptureButton.IsEnabled = true; }));
            }
        }

        private void CaptureImageClick(object sender, RoutedEventArgs e)
        {
            _captureRequested = true;
            CaptureButton.IsEnabled = false;
        }

        private void DeleteImageClick(object sender, RoutedEventArgs e)
        {
            try 
            { 
                foreach (PhotoCapture.Photo photo in PhotoListView.SelectedItems)
                {
                    File.Delete(photo.Path);
                }
            }
            catch (Exception ex)
            {   // sww added 
                System.Windows.MessageBox.Show("Delete Failed>>" + ex.Message);
            }
        }

        private void PhotoListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = PhotoListView.SelectedItems.Count != 0;

            if (PhotoListView.SelectedItems.Count == 1 || PhotoListView.SelectedItems.Count == 2)
            {
                var path = ((PhotoCapture.Photo)PhotoListView.SelectedItem).Path;
                // load the image, specify CacheOption so the file is not locked
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path);
                image.EndInit();

                ((PhotoCapture.PhotoCaptureModel)DataContext).LargeSelectedImage = image;
            }

            if (PhotoListView.SelectedItems.Count == 2)
            {
                var path = ((PhotoCapture.Photo)PhotoListView.SelectedItems[1]).Path;
                // load the image, specify CacheOption so the file is not locked
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path);
                image.EndInit();

                ((PhotoCapture.PhotoCaptureModel)DataContext).LargeSelectedImage2 = image;
            }

            VideoCapturePanel.Visibility = (PhotoListView.SelectedItems.Count == 0 || PhotoListView.SelectedItems.Count > 2)
                ? Visibility.Visible : Visibility.Collapsed;

            LargeSavedImage.Visibility = (PhotoListView.SelectedItems.Count == 1 || PhotoListView.SelectedItems.Count == 2)
                ? Visibility.Visible : Visibility.Collapsed;

            LargeSavedImage2.Visibility = PhotoListView.SelectedItems.Count == 2
                ? Visibility.Visible : Visibility.Collapsed;

            ImageGrid.RowDefinitions[1].Height = PhotoListView.SelectedItems.Count == 2
                ? new GridLength(1, GridUnitType.Star)
                : new GridLength(0, GridUnitType.Star);

            if (DeleteTextBlock.IsEnabled)
            {
                DeleteTextBlock.Text = string.Format("Delete {0} Photo{1}",
                    PhotoListView.SelectedItems.Count,
                    PhotoListView.SelectedItems.Count > 1 ? "s" : "");
            }
            else
            {
                DeleteTextBlock.Text = "Delete Photo(s)";
            }
        }

        private void UIElement_OnPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PhotoListView.UnselectAll();
        }

        private void SelectAllClick(object sender, RoutedEventArgs e)
        {
            PhotoListView.SelectAll();
        }

     }


    public class ImageCacheConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {

            var path = (string)value;
            // load the image, specify CacheOption so the file is not locked
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();

            return image;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not implemented.");
        }
    }
}
