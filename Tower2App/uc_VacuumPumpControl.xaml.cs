using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Edge.Tower2.UI
{
    /// <summary>
    ///     Interaction logic for VacuumPumpControl.xaml
    /// </summary>
    public partial class VacuumPumpControl : INotifyPropertyChanged
    {
        private readonly Timer _fasterDownTimer = new Timer(180);                               // was 150
        private readonly Timer _slowerDownTimer = new Timer(180);                               // was 150

        public delegate void de_Vacuum(String power, string pulse);                             // 2014 11/17
        public event de_Vacuum de_Vacuum_PowerStatus;

        private int _vacuumPressure = 18;

        public VacuumPumpControl()
        {
            InitializeComponent();

            App.Outputs.PropertyChanged += OutputsOnPropertyChanged;

            _Max = 50;                                                                          // 2014 08/22
           
            slider.Visibility = Visibility.Hidden;

            Utility.Lib.LoadImageNoLock(imgPlus, "\\Skin\\Images\\plus1.png");                  // 0106-05

            Utility.Lib.LoadImageNoLock(imgMinus, "\\Skin\\Images\\minus1.png");                // 0106-05

            Utility.Lib.LoadImageNoLock(imgRing, "\\Skin\\Images\\ring_my2.png");               // 0106-05

            Utility.Lib.LoadImageNoLock(imgRingBG2, "\\Skin\\Images\\ring_my2_bg2.png");        // 0106-05

            Utility.Lib.LoadImageNoLock(imgRingBG, "\\Skin\\Images\\t2\\Vacuum_ring_bg.png");   // 0106-05
        }

        private void OutputsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
 
            switch (e.PropertyName)
            {
                case "VacuumPump":
                    VacuumSpeedLabel.Opacity = VacuumPump ? 1.0 : 0.2;
                    UpdateVacuumBarOpacity();

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("VacuumPump"));
                        PropertyChanged(this, new PropertyChangedEventArgs("OnEnabled"));
                        PropertyChanged(this, new PropertyChangedEventArgs("OffEnabled"));

                        updatePowerButtonImage("VacuumPump");                                   // 2014 08/29 Add by sww 
                    }
                    break;

                case "VacuumPressure":
                    if (PropertyChanged != null)
                    {
                        if (VacuumPressure != App.Outputs.VacuumPressure)
                            VacuumPressure = App.Outputs.VacuumPressure;

                        updatePowerButtonImage("VacuumPressure");                               // 2014 09/17
                    }
                    break;
            }
        }

        public void ColorChanged( int page)                                                     // 0- first page, 1 second page
        {
            if (page == 1)
            {
                Utility.Lib.LoadImageNoLock(imgPlus, "\\Skin\\Images\\plus2.png");
                Utility.Lib.LoadImageNoLock(imgMinus, "\\Skin\\Images\\minus2.png");

                Application.Current.Resources["st_ringcolor_default"] = Application.Current.Resources["st_ringcolor_pg2"];
            }
            else
            {
                Utility.Lib.LoadImageNoLock(imgPlus, "\\Skin\\Images\\plus1.png");
                Utility.Lib.LoadImageNoLock(imgMinus, "\\Skin\\Images\\minus1.png");

                Application.Current.Resources["st_ringcolor_default"] = Application.Current.Resources["st_ringcolor_pg1"];
            }
        }

        public bool OffEnabled
        {
            get { return VacuumPump; }
        }

        public bool OnEnabled
        {
            get { return !VacuumPump; }
        }

        private App App
        {
            get { return (App) Application.Current; }
        }

        public int Min { get; set; }

        private int _Max;
       
        // 2014 08/22
        public int Max { 
            get{return _Max;} 
            set{_Max = value;}
         }

        public int MinYellow { get; set; }
        public int MinOrange { get; set; }

        public int VacuumPressure
        {
            get { return _vacuumPressure; }
            set
            {
                _vacuumPressure = value;
                App.Outputs.VacuumPressure = value;

                UpdateVacuumBarOpacity();

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("VacuumPressure"));
                }
            }
        }

        public int VacuumPressureDisplay { get; set; }                                          // 2014 09/12 

        public bool VacuumPump
        {
            get { return App.Outputs.VacuumPump; }
            set { App.Outputs.VacuumPump = value; }
        }
    
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void UpdateVacuumBarOpacity()
        {
            int i = _vacuumPressure;                                                            // 2014 08/22

            int V = (int)(VacuumPressureDisplay * 64.62 + 55.38);                               // 0102-36
            string s = (V).ToString().PadLeft(4, '0');                                          // 0102-36
   
            // For forward clock ring
            slider.Value = (100 * VacuumPressureDisplay / Max);
            VacuumSpeedLabel.Content = VacuumPressureDisplay.ToString();
        }

        private void GenerateVacuumBars()
        {
        
        }

        public void RingVisible(bool V)                                                         // Ver 0020-02
        {
            if (V)
                cvsRing.Visibility = Visibility.Visible;
            else
                cvsRing.Visibility = Visibility.Hidden;
        }

        private void DecrementSpeed(object sender, ElapsedEventArgs args)
        {
            Dispatcher.Invoke((Action) (() =>
                                        {
                                            if (VacuumPressureDisplay > Min)
                                                VacuumPressureDisplay -= 1;
                                            else
                                                VacuumPressureDisplay = Min;
                                            UpdateVacuumBarOpacity();

                                            ChangePumpValue();                                  // 0102-36
                                           
                                        }));
        }

        private void IncrementSpeed(object sender, ElapsedEventArgs args)
        {
            Dispatcher.Invoke((Action) (() =>
                                        {
                                            if (VacuumPressureDisplay < Max)
                                                VacuumPressureDisplay += 1;
                                            else
                                                VacuumPressureDisplay = Max;
                                           
                                            UpdateVacuumBarOpacity();

                                            ChangePumpValue();                                  // 0102-36
                                          
                                        }));
        }

        private void ChangePumpValue()                                                          // 0102-36
        {
            App.BoardManager.AddLogMessage("pressure=" + VacuumPressureDisplay);                // 0106-16

            int V = (int)(VacuumPressureDisplay * 64.62 + 55.38);
            string s = (V).ToString().PadLeft(4, '0');
            if (App.sp != null)
                App.sp.SerialCmdSend("v" + s + "\r");
        }

        private void OffButtonPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnButton.Visibility = Visibility.Visible;
            OffButton.Visibility = Visibility.Hidden;
            VacuumPump = false;
        }

        // Power On/OFF Button
        private void OnButtonPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {   
            //=========================================================================
            // If have indicater, use the indicater value overwrite the VacuumPressure
            //=========================================================================
            if (VacuumPump == false) //2014 09/12
            {
                // Turn On
                VacuumPump = true;

                // 0103-10
                if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.VortexFusion ||
                    ControlParams.Params.p_control_mode == ControlParams.e_Mode.PulseFusion)   
                {
                    if (ControlParams.Params.p_InCleaningPage)                                  // 0103-10
                        ControlParams.Params.p_CleanupRequired = false;
                    else
                        ControlParams.Params.p_CleanupRequired = true;
                }
               

                VacuumPressure = VacuumPressureDisplay;

                if (de_Vacuum_PowerStatus != null)                                              // send power message to PulseSelect
                {
                    de_Vacuum_PowerStatus("ON", ControlParams.Params.p_last_PulseSelect);
                }

                ChangePumpValue();                                                              // 0102-36

                App.sp.SerialCmdSend("VN" + "\r");                                              // 0102-36
               
            }
            else
            {
                VacuumPump = false;                                                             // Turn Off`   
               
                // 2014 08/21 if it is PulseFusion mode 
                App.Outputs.ExMass4 = false;
                App.Outputs.DC_Motor = false;

                if (de_Vacuum_PowerStatus != null)
                {
                    de_Vacuum_PowerStatus("OFF", ControlParams.Params.p_last_PulseSelect);
                }

                App.sp.SerialCmdSend("VF" + "\r");                                              // 0102-36
            }
        }

        private void updatePowerButtonImage(string callfrom)
        {
            if (VacuumPump == true)
            {
                Utility.Lib.LoadImageFromAppDir(imgPower, "/Skin/Images/power-icon-off.png");
            }
            else
            {
                Utility.Lib.LoadImageFromAppDir(imgPower, "/Skin/Images/power-icon-on-lightblue.png");
            }
        }
 
        private void SlowerButtonPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DecrementSpeed(sender, null);
            _slowerDownTimer.Start();
            _slowerDownTimer.Elapsed += DecrementSpeed;

            VacuumPressure = VacuumPressureDisplay;
            UpdateVacuumBarOpacity();
        }

        private void SlowerButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _slowerDownTimer.Stop();
            _slowerDownTimer.Elapsed -= DecrementSpeed;

            VacuumPressure = VacuumPressureDisplay;
            UpdateVacuumBarOpacity();
        }
 
        private void Me_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateVacuumBars();
        }

        private void rightFasterButtonPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IncrementSpeed(sender, null);
            _fasterDownTimer.Start();
            _fasterDownTimer.Elapsed += IncrementSpeed;

            VacuumPressure = VacuumPressureDisplay;
            UpdateVacuumBarOpacity();
        }

        private void rightFasterButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _fasterDownTimer.Stop();
            _fasterDownTimer.Elapsed -= IncrementSpeed;

            VacuumPressure = VacuumPressureDisplay;
            UpdateVacuumBarOpacity();
        }

        private void Me_Unloaded(object sender, RoutedEventArgs e)                              // 0102-07
        {
            App.Outputs.PropertyChanged -= OutputsOnPropertyChanged;
        }
     }
}