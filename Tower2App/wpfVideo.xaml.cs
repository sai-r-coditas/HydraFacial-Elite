using System;
using System.Windows;
using System.Windows.Input;

// For DispatcherPriority
using System.Windows.Threading;

using System.Threading;
using System.IO;

// sww add tax
using System.Configuration;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfVideo.xaml
    /// </summary>
    public partial class wpfVideo : Window
    {
        private string l_TrainingVideo { get; set; }
        public wpfVideo()
        {
            InitializeComponent();

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            l_TrainingVideo = ConfigurationManager.AppSettings["Pretreatment_Training"];

            dispatchertimer.Tick += new EventHandler(Timer_Tick);

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\HF-QuickStart-v18-pop-up-video1.png");  // 0102-39

            Utility.Lib.LoadImageNoLock(imgReplay, "\\Skin\\Images\\Video_restart.png");  // 0106-05

            Utility.Lib.LoadImageNoLock(imgPlayStop, "\\Skin\\Images\\n_Play.png");  // 0106-05

            lblMessage.Content = ""; // 0106-05

            lblMessage.Visibility = Visibility.Hidden; // 0106-05
        }

        private void OnEnter() 
        {
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Home_Page;  // 2014 12/01

            LoadVideoFile(l_TrainingVideo);  // 0102-38     

            imgPlayStop.Tag = "";

            btnPlay_Click(null, null);
        }

        private void OnLeave()  
        {
            dispatchertimer.Stop(); // 0020-03
     
            MediaElement1.Close();

            //2014 12/02
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
        }

        System.Windows.Threading.DispatcherTimer dispatchertimer = new System.Windows.Threading.DispatcherTimer();
        private void LoadVideoFile(String FileName)
        {
            Thread.Sleep(300); // 2014 10/27

            var FileLocation = Environment.CurrentDirectory + "\\Videos\\" + ControlParams.Params.p_SecondLanguage + "\\Application\\" + FileName;  // 0106-15

            if (!File.Exists(FileLocation))
            {
                lblMessage.Content = App.getTextMessages("Video File Not Found!"); // 0106-05

                lblMessage.Visibility = Visibility.Visible; // 0106-05

                DoEvents();

                return;
            }

            lblMessage.Visibility = Visibility.Hidden; // 0106-05

            try
            {
                MediaElement1.Source = new Uri(FileLocation);

            }
            catch { new NullReferenceException("Error"); }

            // 2014 12/03
            VolumeSlider.Value = ControlParams.Params.p_AudioVolume;

            MediaElement1.Volume = VolumeSlider.Value;

            dispatchertimer.Interval = new TimeSpan(0, 0, 1);

            // 2014 10/27
            Thread.Sleep(300);

            if (MediaElement1.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds); // default
                pbrVideo.Maximum = ts.TotalSeconds;
            }
        }

        private void MediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaElement1.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds); // default
               
                pbrVideo.Maximum = ts.TotalSeconds;

                string str = ts.ToString(@"mm\:ss");

                lblProgressValue.Content = "0:00";

                lblProgressValue1.Content = " / " + str;
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (imgPlayStop.Tag == "")
            {
                SetvideoOn(true);
            }
            else
            {
                SetvideoOn(false);
            }
        }

        private void SetvideoOn(bool On)
        {
            if (On)
            {
                dispatchertimer.Start(); // 0020-03
                MediaElement1.Play();
                imgPlayStop.Tag = "Stop";
                
                Utility.Lib.LoadImageFromAppDir(imgPlayStop, "/Skin/Images/n_Stop.png");
            }
            else
            {
                MediaElement1.Pause();
                imgPlayStop.Tag = "";
                Utility.Lib.LoadImageFromAppDir(imgPlayStop, "/Skin/Images/n_Play.png");
                dispatchertimer.Stop(); // 0020-03
            }
        }
       
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement1.Volume = VolumeSlider.Value;
        }

        public void setVolume(double V)
        {
            VolumeSlider.Value = V;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pbrVideo.Value = MediaElement1.Position.TotalSeconds;

            TimeSpan ts = TimeSpan.FromSeconds(MediaElement1.Position.TotalSeconds);
            string str = ts.ToString(@"mm\:ss");

            lblProgressValue.Content = str;
        }

        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void imgPlayStop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (imgPlayStop.Tag == "")
            {
                SetvideoOn(true);
            }
            else
            {
                SetvideoOn(false);
            }
        }

        private void imgReplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MediaElement1.Stop();
            DoEvents();
            SetvideoOn(true);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatchertimer.Tick -= new EventHandler(Timer_Tick);  // 0020-08
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            NavBar.setVolume(ControlParams.Params.p_AudioVolume); // 0101-03
            
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Home_Page; 

            LoadVideoFile(l_TrainingVideo);  // 0102-38     

            imgPlayStop.Tag = "";

            btnPlay_Click(null, null);
        }

        private void Me_Deactivated(object sender, EventArgs e)  // 0103-02
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden; // 0101-03

            dispatchertimer.Stop(); // 0020-03

            MediaElement1.Stop();   // 0103-02

            MediaElement1.Source = null;

            //2014 12/02
            //GC.Collect(2);
            //GC.WaitForPendingFinalizers();
            //GC.Collect(2);
        }
    }
}
