using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.IO;
// sww add tax
using System.Configuration;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for Photo_Custom_Type.xaml
    /// </summary>
    public partial class Photo_Customer_Type 
    {
        private App App
        {
            get { return (App)Application.Current; }
        }

        public Photo_Customer_Type()
        {
            InitializeComponent();
           
            // sww, add document
            page_init();

        }

        private void page_init()
        {
            // sww, add document
            Load_AppSettings_Doc_orImage("Customer_Doc");

            SP1.Visibility = Visibility.Visible;
            //SP2.Visibility = Visibility.Collapsed;
            SP1.Width = (double)700;
        }

        //Add by sww
        string currentDoc;
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private bool mouseDown;

        private void Load_AppSettings_Doc_orImage(string name)
        {

            string filename = ConfigurationManager.AppSettings[name];
            if (System.IO.Path.GetExtension(filename) == ".docx")
            {
                LoadDocument(filename);
            }
            else if (System.IO.Path.GetExtension(filename) == ".jpg")
                Utility.Lib.LoadImageFromAppDir(imgViewer, "\\Docs\\" + filename);
        }

        private void LoadDocument(string docs)
        {
            if (docs == "" || docs == null)
                System.Windows.MessageBox.Show("Invalid document file");

            FlowDocument flowDoc = new FlowDocument();

            Utility.Lib.loadWordML(flowDoc, docs);
            flowDocViewer.Document = flowDoc;
        }

        private void NewClick(object sender, RoutedEventArgs e)
        {
            //App.Go(Mode.HotCold);
            //App.Go(Mode.ImageCapture);
        }

        //-- Not in use
        private void ExistClick(object sender, RoutedEventArgs e)
        {
            // Show six photos & allow user to edit each one
            //App.Go(Mode.HotCold);
            
            App.Go(Mode.Photo_Customer_Search);
        }

        private void btnExistCustomer_Click(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.p_NewCustomer = false;
            // Find the Exist Customer
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Search;
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Search;
            App.Go(Mode.Photo_Customer_Search);
            
            //((Photo_Customer_Search)MainWindow).page_init();
        }

        private void btnNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            // New Customer
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Create;
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Create;
            ControlParams.Params.p_NewCustomer = true;
            App.Go(Mode.Photo_Customer_Search);
        }
        
        // For Video
        private void PlayVideoFile(String FileName)
        {
            //var FileLocation = Environment.CurrentDirectory + "\\Videos\\Application\\" + FileName;
            //if (!File.Exists(FileLocation))
            //{
            //    System.Windows.MessageBox.Show("Video File Not Found!");
            //    return;
            //}

            //try
            //{
            //    MediaElement1.Source = new Uri(FileLocation);

            //}
            //catch { new NullReferenceException("Error"); }

            //System.Windows.Threading.DispatcherTimer dispatchertimer = new System.Windows.Threading.DispatcherTimer();
            //dispatchertimer.Tick += new EventHandler(Timer_Tick);

            //VolumeSlider.Value = MediaElement1.Volume;

            //dispatchertimer.Interval = new TimeSpan(0, 0, 1);
            //dispatchertimer.Start();

            ////-Play Video
            //MediaElement1.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            //MediaElement1.Pause();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //MediaElement1.Play();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //MediaElement1.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //TimelineSlider.Value = MediaElement1.Position.TotalSeconds; //default
            //pbrVideo.Value = MediaElement1.Position.TotalSeconds;
        }

        private void MediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            //if (MediaElement1.NaturalDuration.HasTimeSpan)
            //{
            //    TimeSpan ts = TimeSpan.FromMilliseconds(MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds); // default
            //    //TimeSpan ts = TimeSpan.FromSeconds(mediaElement1.NaturalDuration.TimeSpan.TotalSeconds);
            //    //TimelineSlider.Maximum = ts.TotalSeconds;
            //    pbrVideo.Maximum = ts.TotalSeconds;
            //}
        }

        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);  //default
            //TimeSpan ts = TimeSpan.FromMilliseconds(e.NewValue);
            //MediaElement1.Position = ts;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //MediaElement1.Volume = VolumeSlider.Value;
        }

        private void TimelineSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MediaElement1.Pause();
        }

        private void TimelineSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //MediaElement1.Play();
        }

        private void btn_VideoClose_Click(object sender, RoutedEventArgs e)
        {
            SP1.Visibility = Visibility.Visible;
            //SP2.Visibility = Visibility.Collapsed;
            //MediaElement1.Stop();
            //MediaElement1.Close();
            SP1.Width = (double)700;
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            //if (btnSplit.Content == "|")
            //{
            //    SP2.Width = (double)350;
            //    SP1.Width = (double)350;
            //    SP2.Visibility = Visibility.Visible;
            //    SP1.Visibility = Visibility.Visible;
            //    LoadDocument(currentDoc);
            //    MediaElement1.Height = (Double)200;
            //    btnSplit.Content = "O";
            //}
            //else
            //{
            //    SP2.Width = (double)700;
            //    SP1.Width = (double)0;
            //    SP2.Visibility = Visibility.Visible;
            //    SP1.Visibility = Visibility.Visible;
            //    LoadDocument(currentDoc);
            //    MediaElement1.Height = (Double)400;
            //    btnSplit.Content = "|";
            //}
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            page_init();
            NavBar.setVolume( ControlParams.Params.p_AudioVolume);
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }

        // Flowdocument
        private void ScrollViewer1_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Get the new scroll position.
            Point point = e.GetPosition(this);

            if (mouseDown)
            {
                // Determine the new amount to scroll.
                Point delta = new Point((point.X > this.scrollStartPoint.X) ?
                                 -(point.X - this.scrollStartPoint.X) :
                                    (this.scrollStartPoint.X - point.X),

                                (point.Y > this.scrollStartPoint.Y) ?
                                -(point.Y - this.scrollStartPoint.Y) :
                                    (this.scrollStartPoint.Y - point.Y));

                // Scroll to the newer location
                ScrollViewer1.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                ScrollViewer1.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        private void ScrollViewer1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;

            // Save starting point, used later when determining 
            //how much to scroll.
            scrollStartPoint = e.GetPosition(this);
            scrollStartOffset.X = ScrollViewer1.HorizontalOffset;
            scrollStartOffset.Y = ScrollViewer1.VerticalOffset;

            // Update the cursor if can scroll or not.
            this.Cursor = (ScrollViewer1.ExtentWidth > ScrollViewer1.ViewportWidth) ||
                (ScrollViewer1.ExtentHeight > ScrollViewer1.ViewportHeight) ?
               System.Windows.Input.Cursors.Arrow : System.Windows.Input.Cursors.Arrow;

        }

        private void ScrollViewer1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }

        private void ScrollViewer1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void flowDocViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }

        private void btnVolUp_Click(object sender, RoutedEventArgs e)
        {
            //if (VolumeSlider.Value + 0.1 < 1)
            //    VolumeSlider.Value = VolumeSlider.Value + 0.1;
        }

        private void btnVolDown_Click(object sender, RoutedEventArgs e)
        {
            //if (VolumeSlider.Value - 0.1 > 0)
            //    VolumeSlider.Value = VolumeSlider.Value - 0.1;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }
    }
}
