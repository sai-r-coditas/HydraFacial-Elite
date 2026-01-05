using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

using System;
using System.Windows.Threading;

using System.Windows.Media.Animation;

using System.ComponentModel; // 0102-01

using System.Text; // 0106-11

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for NavBar.xaml
    /// </summary>
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
            
            cvsVolume.Visibility = Visibility.Hidden;

            App.cs_Events.PropertyChanged += OnPropertyChanged;  // 0102-01

            IsVisibleChanged += (sender, args) =>   // 0102-06
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            Utility.Lib.LoadImageFromAppDir(imgTopMenu, "\\Skin\\Images\\"+ App.getTextMessages("top_menu_bg")+".png"); // 0102-38  0106-06

            btnInternet_off.Visibility = Visibility.Hidden;  // 0102-37
            btnInternet.Visibility = Visibility.Hidden;  // 0102-37
            btnHelp.Visibility = Visibility.Hidden;  // 0102-37
            btnStatus.Visibility = Visibility.Hidden;  // 0102-37

            Utility.Lib.LoadImageFromAppDir(imgSliderVolume, "\\Skin\\Images\\Volume_bg1.png"); // 0106-05
        }

        private void OnEnter()
        {
            setVolume(ControlParams.Params.p_AudioVolume); // 0101-06
        }

        private void OnLeave()
        {
            cvsVolume.Visibility = Visibility.Hidden;  // 0102-06
            cvsVolume.Tag = "";
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0102-01
        {
            return;  // 0102-37

            // T2
            if (propertyChangedEventArgs.PropertyName == "WifiState")
            {
                if (App.cs_Events.WifiState == "off")
                    btnInternet.Visibility = Visibility.Hidden;
                else
                    btnInternet.Visibility = Visibility.Visible;
            }
        }

        private void UpdateState()
        {
            
        }

        [NotNull]
        public App App
        {
           get { return (App)Application.Current; }
        }
 
        public string SelectedButton
        {
            get { return (string)GetValue(SelectedButtonProperty); }
            set
            {
                SetValue(SelectedButtonProperty, value);

                var control = (Control)this.FindName(value);
            }
        }
 
        // Using a DependencyProperty as the backing store for SelectedButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedButtonProperty =
            DependencyProperty.Register("SelectedButton", typeof(string), typeof(NavBar), new UIPropertyMetadata(null));

        private void HydraFacialClick(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.HydraFacial);
        }

        private void VacJetClick(object sender, RoutedEventArgs e)
        {
            //App.Go(Mode.VacJet);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            // 2014 09/15 
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;

            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Home_Page;

            // 0101-03
            //=========================================
            if (ControlParams.Params.p_CleanupMode == 1 && ControlParams.Params.p_ProtoMode == true) // Vortex clean up
            {
                // it needs show pop up clean up message
                App.Go(Mode.HydraFacial);

                ControlParams.Params.p_InCleaningPage = false; // 0103-10

                return;
            }
            else if (ControlParams.Params.p_CleanupMode == 2 && ControlParams.Params.p_ProtoMode == true) // Pulse Fusion clean up
            {
                // it needs show pop up clean up message
                App.Go(Mode.HydraFacial);

                ControlParams.Params.p_InCleaningPage = false; // 0103-10

                return;
            }
   
            if (ControlParams.Params.p_CleanupRequired == false)

                App.Go(Mode.Home);

            else
            {
                // it needs show pop up clean up message
                App.Go(Mode.HydraFacial);
            }

            ControlParams.Params.p_InCleaningPage = false; // 0103-10
        }

        private void btnVolume_Click(object sender, RoutedEventArgs e)
        {
            if (cvsVolume.Tag == "On")
            {   
                cvsVolume.Visibility = Visibility.Hidden;
                cvsVolume.Tag = "";
            }
            else
            {    
                cvsVolume.Visibility = Visibility.Visible;
                cvsVolume.Tag = "On";
                cvsVolume_On();
            }
        }
 
        private void sldVolume_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void sldVolume_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)  // 0106-13
        {
            ControlParams.Params.p_AudioVolume = sldVolume.Value;

            SaveParams();

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).setVolume(sldVolume.Value);
            ((wpfVideo)App._mainWindows[Mode.wpfVideo]).setVolume(sldVolume.Value);
            ((VPlayer)App._mainWindows[Mode.VPlayer]).setVolume(sldVolume.Value);

            setApplicationVolume(ControlParams.Params.p_AudioVolume);
        }

        public void setVolume(double V)
        {
            sldVolume.Value = V;
            setApplicationVolume(ControlParams.Params.p_AudioVolume);
        }

        private void setApplicationVolume(double V)
        {
            int Vol = Convert.ToInt32( 65535 * V );
            c_Volume.SetVolume(Vol, Vol);
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
       
        private void SaveParams()  // 0106-09 0106-11 0106-13
        {
            try
            {
                string lines = "Audio;" + ControlParams.Params.p_AudioVolume.ToString();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + "\\..\\UsageLogs\\params.dat",false,Encoding.ASCII))   // 0106-09
                {
                    file.WriteLine(lines);
                }

                DoEvents();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            // T2
            //App._mainWindows[Mode.wpfStatus].Show();  // 0102-35
            //((wpfStatus)App._mainWindows[Mode.wpfStatus]).Page_init(); // 0102-35
        }

        private void cvsVolume_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 2014 11/12
            DoubleAnimation ani = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            cvsVolume.BeginAnimation(Canvas.OpacityProperty, ani);
            cvsVolume.Tag = ""; // Set Off state
        }

        private void cvsVolume_On()
        {
            DoubleAnimation ani = new DoubleAnimation(1, TimeSpan.FromMilliseconds(10));
            cvsVolume.BeginAnimation(Canvas.OpacityProperty, ani);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e) // 0103-10
        {
            // T2
            // if (ControlParams.Params.p_LoginRole =="Administrator")
            //     App._mainWindows[Mode.wpfSettings].Show();

            if (ControlParams.Params.p_LoginRole != "Administrator")  // 0102-11
                return;

            // 2014 09/15 
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;

            ControlParams.Params.p_control_mode = ControlParams.e_Mode.Settings;

            // 0101-03
            //=========================================

            if (ControlParams.Params.p_CleanupMode == 1 && ControlParams.Params.p_ProtoMode == true) // Vortex clean up
            {
                // it needs show pop up clean up message
                App.Go(Mode.HydraFacial);
                return;
            }
            else if (ControlParams.Params.p_CleanupMode == 2 && ControlParams.Params.p_ProtoMode == true) // Pulse Fusion clean up
            {
                // it needs show pop up clean up message
                App.Go(Mode.HydraFacial);
                return;
            }

            //=======================================
                  
            if (ControlParams.Params.p_CleanupRequired == false) 
               
                App.Go(Mode.wpfSettings);

            else
            {
                // it needs show pop up clean up message
                App.Go(Mode.HydraFacial);
            }

            ControlParams.Params.p_InCleaningPage = false; // 0103-10
        }

        private void btnInternet_Click(object sender, RoutedEventArgs e)
        {
            // T2
            // App.Go(Mode.wpfWifi);  // 0102-07  // 0102-35
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            App.cs_Events.PropertyChanged -= OnPropertyChanged;  // 0102-06
        }

       
    }
}
