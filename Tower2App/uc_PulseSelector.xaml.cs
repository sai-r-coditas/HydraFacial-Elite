using System.Windows;
using System.Windows.Controls;

namespace Edge.Tower2.UI
{
    public partial class PulseSelector : UserControl
    {
        private App App
        {
            get { return (App)Application.Current; }
        }

        public PulseSelector()
        {
            InitializeComponent();

            chkLOW.Visibility = Visibility.Hidden;
            chkMED.Visibility = Visibility.Hidden;
            chkHIGH.Visibility = Visibility.Hidden;
            chkOFF.Visibility = Visibility.Hidden;
 
            imgOff.Visibility = Visibility.Visible;
            imgLow.Visibility = Visibility.Visible;
            imgMed.Visibility = Visibility.Visible;
            imgHigh.Visibility = Visibility.Visible;
        }

        private string _displayMode;
        public string displayMode
        {
            get { return _displayMode; }
            set 
            {
                _displayMode = value;

                if (_displayMode == "PulseFusion")
                {
                    cvs1.Visibility = Visibility.Visible;
                    cvsSelectPulse1.Visibility = Visibility.Visible;
                    cvsSelectPulse2.Visibility = Visibility.Collapsed;
                    imgOff.Visibility = Visibility.Hidden;
                }
                else
                {
                    cvs1.Visibility = Visibility.Visible;
                    cvsSelectPulse1.Visibility = Visibility.Collapsed;
                    cvsSelectPulse2.Visibility = Visibility.Visible;
                    imgOff.Visibility = Visibility.Visible;
                }
            }
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty = 
            DependencyProperty.Register("Caption", typeof(string), typeof(PulseSelector), new PropertyMetadata("Pulse"));

        private void SetpointUnchecked(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                CheckBox chk = (CheckBox)sender;
                
                if (chk.Name == "chkLOW")
                {
                    Selection_Default_Single(imgLow, "/Skin/images/t2/pulse_low_u.png");
                }
                else if (chk.Name == "chkMED")
                {
                    Selection_Default_Single(imgMed, "/Skin/images/t2/pulse_med_u.png");
                }
                else if (chk.Name == "chkHIGH")
                {
                    Selection_Default_Single(imgHigh, "/Skin/images/t2/pulse_high_u.png");
                }
                else if (chk.Name == "chkOFF")
                {
                    Selection_Default_Single(imgOff, "/Skin/images/t2/pulse_off_u.png");
                }

            }

            if (App.Outputs.PropertyIsChanging)
                return;

            var cb = (sender as CheckBox);

            if (cb.IsFocused)
            {
                cb.IsChecked = true;
                e.Handled = true;
            }
            
        }
       
        /// 
        /// PulseFusion does not need single click turn off Pulse
        /// 
        private void imgLow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.Outputs.VacuumPump != false)
                chkLOW.IsChecked = true;

            Selection_HighLight(imgLow, "/Skin/images/t2/pulse_low_d.png", true);
            ControlParams.Params.p_last_PulseSelect = "L";
        }

        private void imgMed_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.Outputs.VacuumPump != false)
                chkMED.IsChecked = true;

            Selection_HighLight(imgMed, "/Skin/images/t2/pulse_med_d.png", true);
            ControlParams.Params.p_last_PulseSelect = "M";
        }

        private void imgHigh_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (App.Outputs.VacuumPump != false)
                chkHIGH.IsChecked = true;

            Selection_HighLight(imgHigh, "/Skin/images/t2/pulse_high_d.png", true);
            ControlParams.Params.p_last_PulseSelect = "H";
        }
        
        private void imgOff_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            chkOFF.IsChecked = true;
            Selection_HighLight(imgOff, "/Skin/images/t2/pulse_off_d.png", true);
            ControlParams.Params.p_last_PulseSelect = "O";
        }

        private void Selection_HighLight(System.Windows.Controls.Image C, string filename, bool highlight)
        {
            Utility.Lib.LoadImageFromAppDir(imgOff, "/Skin/images/t2/pulse_off_u.png");
            Utility.Lib.LoadImageFromAppDir(imgLow, "/Skin/images/t2/pulse_low_u.png");
            Utility.Lib.LoadImageFromAppDir(imgMed, "/Skin/images/t2/pulse_med_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHigh, "/Skin/images/t2/pulse_high_u.png");
            
            imgHigh.Tag = "";
            imgMed.Tag = "";
            imgLow.Tag = "";
            imgOff.Tag = "";
            if (highlight)
            {
                Utility.Lib.LoadImageFromAppDir(C, filename);
                C.Tag = "on";
            }
        }

        private void Selection_Default_Single(System.Windows.Controls.Image C, string filename)
        {
            //Utility.Lib.LoadImageFromAppDir(C, filename);
            //C.Tag = "";
        }

        public void setDisplayPulse(string Mode)
        {
            if (Mode == "L")
                imgLow_MouseDown(null, null);
            else if (Mode == "M")
                imgMed_MouseDown(null, null);
            else if (Mode == "H")
                imgHigh_MouseDown(null, null);
            else
                imgOff_MouseDown(null, null);
        }

        public void setPulse(string power, string pulse)
        {
            if (power == "ON")
            {
                if (ControlParams.Params.p_last_PulseSelect == "H")
                    chkHIGH.IsChecked = true;
                else if (ControlParams.Params.p_last_PulseSelect == "M")
                    chkMED.IsChecked = true;
                else if (ControlParams.Params.p_last_PulseSelect == "L")
                    chkLOW.IsChecked = true;
            }
            else
                chkOFF.IsChecked = true;
        }
      
    }
}