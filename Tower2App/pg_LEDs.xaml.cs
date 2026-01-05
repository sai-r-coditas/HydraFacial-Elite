
using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for pg_PulseSelect.xaml
    /// </summary>
    public partial class pg_LEDs 
    {
        private readonly Timer _updateTimer = new Timer(150);

        public pg_LEDs()
        {
            InitializeComponent();
            
            LEDMode = "onelight";   // set to Default
            lastSection = 0;
            currentSection = 0;
            dt = DateTime.Now;

            cvsChargebar.Visibility = Visibility.Hidden; // 0102-35
            lneChargebar.Visibility = Visibility.Hidden; // 0102-35

            // For !.5
            Loadimage(); // 0100

            Utility.Lib.LoadImageNoLock(imgOneLight,  "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\1light_u.png");    // 0106-05  // 00115a1-0005
            Utility.Lib.LoadImageNoLock(imgTwoLights, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\2lights_u.png");   // 0106-05  // 00115a1-0005

            Utility.Lib.LoadImageNoLock(img_LEDStart, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\led_start.png");   // 0106-05  // 00115a1-0005
            Utility.Lib.LoadImageNoLock(img_LEDStop,  "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\led_pause.png");   // 0106-05  // 00115a1-0005
            Utility.Lib.LoadImageNoLock(img_LEDReset, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\led_next.png");    // 0106-05  // 00115a1-0005

            Utility.Lib.LoadImageNoLock(imgFace, "\\Skin\\Images\\face.png");      // 0106-05
            Utility.Lib.LoadImageNoLock(imgBody, "\\Skin\\Images\\led_body.png");  // 0106-05
        }

        public App App
        {
            get { return (App)System.Windows.Application.Current; }
        }
   
        private void BoardManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                switch (e.PropertyName)
                {
                    case "VacuumAtoD":
                        break;

                    case "PowerPressed":
                        break;
                }
            }));
        }

        private void SetpointUnchecked(object sender, RoutedEventArgs e)
        {
            if (App.Outputs.PropertyIsChanging)
                return;

            var cb = (sender as System.Windows.Controls.CheckBox);

            if (cb.IsFocused)
            {
                cb.IsChecked = true;
                e.Handled = true;
            }
        }

        public void page_init(bool facialmode)
        {
            setTimerRunning = false;
            lblTimeString.Foreground = Brushes.Black;   // show time amount for each session
            btn_LedReset.IsEnabled = false;
            _accumlatedTime = TimeSpan.Zero;
            TimeLeft = TimeSpan.Zero;

            imgOneLight_PreviewMouseDown(null, null);

            FacialMode(facialmode); // 0100

            DisplayOffAllSection();
        }

        #region Load images
        private void Loadimage()
        {
            Utility.Lib.LoadImageFromAppDir(imgTL, "/Skin/Images/led_face_lt.png");
            Utility.Lib.LoadImageFromAppDir(imgTR, "/Skin/Images/led_face_rt.png");
            Utility.Lib.LoadImageFromAppDir(imgBL, "/Skin/Images/led_face_lb.png");
            Utility.Lib.LoadImageFromAppDir(imgBR, "/Skin/Images/led_face_rb.png");
            
            Utility.Lib.LoadImageFromAppDir(imgBodyTL, "/Skin/Images/led_body_lt.png");
            Utility.Lib.LoadImageFromAppDir(imgBodyTR, "/Skin/Images/led_body_rt.png");
            Utility.Lib.LoadImageFromAppDir(imgBodyBL, "/Skin/Images/led_body_lb.png");
            Utility.Lib.LoadImageFromAppDir(imgBodyBR, "/Skin/Images/led_body_rb.png");
        }
        #endregion

        #region Facial/Body Mode
        // Facial mode or body mode
        private void FacialMode(bool mode) // 0100
        {
            if (mode)  // Facial
            {
                imgTL.Visibility = Visibility.Visible;
                imgTR.Visibility = Visibility.Visible;
                imgBL.Visibility = Visibility.Visible;
                imgBR.Visibility = Visibility.Visible;
                imgBodyTL.Visibility = Visibility.Hidden;
                imgBodyTR.Visibility = Visibility.Hidden;
                imgBodyBL.Visibility = Visibility.Hidden;
                imgBodyBR.Visibility = Visibility.Hidden;
                
                imgFace.Visibility = Visibility.Visible;
                imgBody.Visibility = Visibility.Hidden;
            }
            else   // Body
            {
                imgTL.Visibility = Visibility.Hidden;
                imgTR.Visibility = Visibility.Hidden;
                imgBL.Visibility = Visibility.Hidden;
                imgBR.Visibility = Visibility.Hidden;
                imgBodyTL.Visibility = Visibility.Visible;
                imgBodyTR.Visibility = Visibility.Visible;
                imgBodyBL.Visibility = Visibility.Visible;
                imgBodyBR.Visibility = Visibility.Visible;

                imgFace.Visibility = Visibility.Hidden;
                imgBody.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Timer
        //==================================================
        // Declare
        //==================================================
      
        private double timeSecondsLeft { get; set; }
        private double totaltime { get; set; }
        public int desireMin { get; set; }
        public int desireSeconds { get; set; }

        private DateTime dt;
        private string LEDMode { get; set; }
        private int lastSection { get; set; }
        private int currentSection { get; set; }

        private DateTime _startTime = DateTime.MinValue;
        private TimeSpan _accumlatedTime = TimeSpan.Zero;

        // Clock Control
        private bool setTimerRunning
        {
            get
            {
                return _startTime != DateTime.MinValue;
            }
            set
            {
                if (value)
                {
                    _startTime = DateTime.Now;
                    _updateTimer.Start();
                }
                else
                {
                    _startTime = DateTime.MinValue;
                    _updateTimer.Stop();
                }
            }
        }

        // This is a running timer   // 0101-07
        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            TimeLeft = TimeSpan.FromMinutes(desireMin) + TimeSpan.FromSeconds(desireSeconds) - (DateTime.Now - _startTime);
        
            // Stop timer
            if ((_timeLeft.Minutes*60 + _timeLeft.Seconds) <= 0)
            {
                currentSection = currentSection - 1;

                if (LEDMode == "onelight")
                {
                    desireMin = Settings.LED_One_Light_Minutes;
                    desireSeconds = Settings.LED_One_Light_Seconds;
                }
                else
                {
                    desireMin = Settings.LED_Two_Lights_Minutes;
                    desireSeconds = Settings.LED_Two_Lights_Seconds;
                }

                setTimerRunning = false;  

                if (currentSection >= 0)
                    setTimerRunning = true;  
                else
                {
                    ClearAllSession(); 
                }
            }
        }
      
        private void ClearAllSession()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                ProcessResume();
                // Playsound("Sound\\Windows Shutdown.wav");  // 0103-06
            }));
        }
        #endregion

        #region Display time left and blinking the section
        private TimeSpan _timeLeft = new TimeSpan(0, 0, 0);
        public TimeSpan TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                _timeLeft = value;

                DisplayTimeandSession();
            }
        }

        private void DisplayTimeandSession()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                // 2014 08/15 Modified
                doDisplayTimeandSession();
            }));
        }

        private void doDisplayTimeandSession()   // andf blink the section
        {
            lblTimeString.Content = _timeLeft.Minutes.ToString("00") + ":" + _timeLeft.Seconds.ToString("00");
            timeSecondsLeft = _timeLeft.Minutes * 60 + _timeLeft.Seconds;
          
            int value;

            if (setTimerRunning)
            {
                if (LEDMode == "onelight")
                {
                    value = currentSection;

                    BlinkingTheSection("onelight", value);
                }
                else
                {
                    value = currentSection;

                    BlinkingTheSection("twolights", value);
                }
            }
        }
        #endregion

        #region Blinking the section
        private void BlinkingTheSection(string mode, int section)
        {
            TimeSpan timeDiff = DateTime.Now - dt;
            if (timeDiff.Milliseconds > 800)  
            {
                if (mode == "twolights")
                {
                    if (section == 1)
                    {
                        cvsTL.Visibility = (cvsTL.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                        cvsTR.Visibility = cvsTL.Visibility;
                    }
                    else
                    {
                        cvsBL.Visibility = (cvsBL.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                        cvsBR.Visibility = cvsBL.Visibility;
                    }
                }
                else if (mode == "onelight")
                {
                    if (section == 3)
                        cvsTL.Visibility = (cvsTL.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                    else if (section == 2)
                        cvsTR.Visibility = (cvsTR.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                    else if (section == 1)
                        cvsBL.Visibility = (cvsBL.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                    else
                        cvsBR.Visibility = (cvsBR.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
                }

                if (lastSection != section)
                {
                    DisplayOffAllSection();
                    Playsound("Sound\\led_nextsession.wav");
                }
                
                lastSection = section;

                dt = DateTime.Now;
            }
        }
        #endregion

        #region Button Control
        private void btn_LedStart_Click(object sender, RoutedEventArgs e)
        {
            setTimerRunning = true;
            lblTimeString.Foreground = Brushes.Black;

            btn_LedStart.IsEnabled = false;
            btn_LedStop.IsEnabled = true;
            btn_LedReset.IsEnabled = true;
        }

        private void btn_LedStop_Click(object sender, RoutedEventArgs e)
        {
            _accumlatedTime += DateTime.Now - _startTime;

            setTimerRunning = false;

            lblTimeString.Foreground = Brushes.Black;
           
            desireMin = _timeLeft.Minutes;
            desireSeconds = _timeLeft.Seconds;

            btn_LedStart.IsEnabled = true;
            btn_LedStop.IsEnabled = false;
            btn_LedReset.IsEnabled = false;   // 0101-07

            DisplayOffAllSection();
        }
       
        private void btn_LedReset_Click(object sender, RoutedEventArgs e)  // this is next button
        {
            if (currentSection <= 0)
            {
                ProcessResume();
                return;
            }

            btn_LedStop.IsEnabled = true;

            if (LEDMode == "onelight")
            {
                desireMin = Settings.LED_One_Light_Minutes;
                desireSeconds = Settings.LED_One_Light_Seconds;
            }
            else
            {
                desireMin = Settings.LED_Two_Lights_Minutes;
                desireSeconds = Settings.LED_Two_Lights_Seconds;
            }

            currentSection = currentSection - 1;

            setTimerRunning = true; 

            TimeLeft = TimeSpan.FromMinutes(desireMin) + TimeSpan.FromSeconds(desireSeconds) - (DateTime.Now - _startTime);

        }
        #endregion

        #region Others
        private void ProcessResume()  
        {
            setTimerRunning = false;
            DisplayOffAllSection();
            btn_LedReset.IsEnabled = false;
            btn_LedStop.IsEnabled = false;

            if (LEDMode == "onelight")
            {
                SetDefaultTime();
                currentSection = 3; // 0101-07
                lastSection = 3;
            }
            else
            {
                SetDefaultTime();
                currentSection = 1;   // 0101-07
                lastSection = 1;
            }
        }

        private void SetDefaultTime()
        {
            if (LEDMode == "onelight")
            {
                desireMin = Settings.LED_One_Light_Minutes;
                desireSeconds = Settings.LED_One_Light_Seconds;
            }
            else
            {
                desireMin = Settings.LED_Two_Lights_Minutes;
                desireSeconds = Settings.LED_Two_Lights_Seconds;
            }

            TimeLeft = TimeSpan.FromMinutes(desireMin) + TimeSpan.FromSeconds(desireSeconds);
            lblTimeString.Content = _timeLeft.Minutes.ToString("00") + ":" + _timeLeft.Seconds.ToString("00");

            totaltime = _timeLeft.Minutes * 60 + _timeLeft.Seconds;
            TimeLeft = TimeSpan.FromMinutes(desireMin) + TimeSpan.FromSeconds(desireSeconds);
            
            MakeStartButtonEnabled();
        }

        private void MakeStartButtonEnabled()
        {
            if (btn_LedStart.IsEnabled == false)  // when timer is on
                btn_LedStop_Click(null, null);

            btn_LedStart.IsEnabled = true;
            btn_LedStop.IsEnabled = false;
            btn_LedReset.IsEnabled = false;
        }

        private void Me_Loaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Elapsed += TimerOnElapsed; // 0101-07
        }

        private void imgTwoLights_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSection = 1;   // 0101-07
            lastSection = 1;      // 0101-07

            LEDMode = "twolights";
            Utility.Lib.LoadImageFromAppDir(imgOneLight, "/Skin/Images/"+ ControlParams.Params.p_SecondLanguage+"/1light_u.png");       // 00115a1-0005
            Utility.Lib.LoadImageFromAppDir(imgTwoLights, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage + "/2lights_d.png");  // 00115a1-0005
            SetDefaultTime();
        }

        private void imgOneLight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            currentSection = 3;   // 0101-07
            lastSection = 3;      // 0101-07

            LEDMode = "onelight";
            Utility.Lib.LoadImageFromAppDir(imgOneLight, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage + "/1light_d.png");    // 00115a1-0005
            Utility.Lib.LoadImageFromAppDir(imgTwoLights, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage + "/2lights_u.png"); // 00115a1-0005 
            SetDefaultTime();
        }

        private void DisplayOffAllSection()
        {
            cvsTL.Visibility = Visibility.Hidden;
            cvsTR.Visibility = Visibility.Hidden;
            cvsBL.Visibility = Visibility.Hidden;
            cvsBR.Visibility = Visibility.Hidden;
        }

        private void Playsound(string soundfile)
        {
            using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(soundfile))
            {
                player.Play();
            }
        }

        private void Me_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Elapsed -= TimerOnElapsed;  // 0101-07
        }
        #endregion
    }
}
