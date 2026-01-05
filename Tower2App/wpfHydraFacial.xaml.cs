using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using JetBrains.Annotations;

// sww add doc
using System.Windows.Data;
using System.Windows.Documents;

// sww add tax
using System.Configuration;

using System.Windows.Media.Animation;

// For DispatcherPriority
using System.Windows.Threading;

using System.Timers;
using System.Threading;

namespace Edge.Tower2.UI
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class HydraFacial : INotifyPropertyChanged
    {
        #region declare
        private readonly string[] _bottleNames = {"", "", "", "", ""};
        private readonly Dictionary<int, Brush> _colorForTemperature = new Dictionary<int, Brush>();
        private readonly Queue<System.Windows.Controls.CheckBox> _selectedCheckBoxes = new Queue<System.Windows.Controls.CheckBox>();
        private bool _inRinseCheckHandler;

        private Point scrollStartPoint;
        private Point scrollStartOffset;

        private bool mouseDown { get; set; }
        private string l_TrainingVideo { get; set; }

        public int l_Signature { get; set; }                                                    // 002639-08-03 current step

        pgSlideShow pg1 = new pgSlideShow();
        Page1 p1 = new Page1();
        Page2 p2 = new Page2();
        
        pg_PulseSelect pgPulseSelect = new pg_PulseSelect();
        pg_HotCold pgHotCold = new pg_HotCold();
        pg_LEDs pgLED =new pg_LEDs();
        pg_PulseFusion pgPulseFusion = new pg_PulseFusion();

        pg_Purchase pgPurchase = new pg_Purchase();  // 0020-01

        private System.Timers.Timer timer;
        FlowDocument flowDoc;

        // 2014 11/25
        System.Windows.Threading.DispatcherTimer dispatchertimer = new System.Windows.Threading.DispatcherTimer();

        private int l_TargetVacuum { get; set; }                                                // 0020-05 Save Vacuum before doing the cleanning

        public string[] BottleNames
        {
            get { return _bottleNames; }
        }

        // Add by sww
        public string[] BottleSize
        {
            get { return _bottleNames; }
        }
        #endregion

        #region Loading
        public HydraFacial()
        {
            InitializeComponent();

            VacuumPumpControl.de_Vacuum_PowerStatus += new VacuumPumpControl.de_Vacuum(On_uiVacuumPower);  // 2014 11/17 

            // 2014 11/25
            dispatchertimer.Tick += new EventHandler(Timer_Tick);
            dispatchertimer.Interval = new TimeSpan(0, 0, 1);

            NavBar.SelectedButton = "HydraFacial";
            NavBar.setVolume (ControlParams.Params.p_AudioVolume);

            VacuumPumpControl.VacuumPressure = Settings.HydraFacialDefaultVacuum;

            B0.Tag = (Action<bool>) (v => { App.Outputs.Solenoid1 = App.Outputs.VacuumPump && v; });
            B1.Tag = (Action<bool>) (v => { App.Outputs.Solenoid2 = App.Outputs.VacuumPump && v; });
            B2.Tag = (Action<bool>) (v => { App.Outputs.Solenoid3 = App.Outputs.VacuumPump && v; });
            B3.Tag = (Action<bool>) (v => { App.Outputs.Solenoid4 = App.Outputs.VacuumPump && v; });
            B4.Tag = (Action<bool>) (v => { App.Outputs.Solenoid5 = App.Outputs.VacuumPump && v; });

            App.BottleChanged += AppBottleChanged;

            App.Outputs.PropertyChanged += OutputsOnPropertyChanged;

            _colorForTemperature.Add(0, Brushes.Gray);
            _colorForTemperature.Add(Settings.HotColdTemperatureCoolest, Brushes.White);
            _colorForTemperature.Add(Settings.HotColdTemperatureCooler, Brushes.LightBlue);
            _colorForTemperature.Add(Settings.HotColdTemperatureCool, Brushes.MediumBlue);
            _colorForTemperature.Add(Settings.HotColdTemperatureWarm, Brushes.Yellow);
            _colorForTemperature.Add(Settings.HotColdTemperatureWarmer, Brushes.Orange);
            _colorForTemperature.Add(Settings.HotColdTemperatureWarmest, Brushes.Red);

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            Thickness margin = cvsVideo.Margin;
            margin.Left = 25;
            cvsVideo.Margin = margin;
            
            ControlParams.Params.p_hydrafacialLoaded = false;
            
            // Add Timer for system clean up
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            // Set default 
            uc_Buttons.SelectedButton = "btnVortexFusion";
            
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.None;                    // 0102-08

            // Set clean up start button hidden
            btnStart.Visibility = Visibility.Hidden;

            // For Slide Show page control, disabled, not for this version
            btnLeft.Visibility = Visibility.Hidden;
            btnRight.Visibility = Visibility.Hidden;

            // a message below on the right hand panel of video
            lblVideo.Content = "";

            ControlParams.Params.p_HotColdSelected = "";

            // For testing .....
            btnLoad.Visibility = Visibility.Collapsed;

            if (ControlParams.Params.p_BottleCountOn)                                           // 0020-03
            {
                lblDemo.Visibility = Visibility.Hidden;
                lblDayRemain.Visibility = Visibility.Hidden;
            }
            else
            {
                lblDemo.Visibility = Visibility.Visible;
                lblDayRemain.Visibility = Visibility.Visible;
            }

            B0.Visibility = Visibility.Hidden;
            B1.Visibility = Visibility.Hidden;
            B2.Visibility = Visibility.Hidden;
            B3.Visibility = Visibility.Hidden;

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\WindowBackground_v3.png");                                                      // 0102-39

            Utility.Lib.LoadImageNoLock(imgViewAll, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\proto_view_all.png");       // 0102-39 0106-13
            Utility.Lib.LoadImageNoLock(imgViewAllmt, "\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\modalities_view_all.png");   // 0102-39  0106-13

            Utility.Lib.LoadImageNoLock(imgTopmenu, "\\Skin\\Images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");                      // 0102-39  0106-06
            Utility.Lib.LoadImageNoLock(imgCleanupTopmenu, "\\Skin\\Images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");               // 0102-39  0106-06

            Utility.Lib.LoadImageNoLock(imgStart, "\\Skin\\Images\\Start.png");                                                                 // 0106-05
            Utility.Lib.LoadImageNoLock(imgStart_PF, "\\Skin\\Images\\Start.png");                                                              // 0106-05

            Utility.Lib.LoadImageNoLock(imgProtoAudio, "\\Skin\\Images\\proto_audio.png");

            Utility.Lib.LoadImageNoLock(imgProtoPrevious, "\\Skin\\Images\\proto_Arrow_l.png");                                                 // 0106-05
            Utility.Lib.LoadImageNoLock(imgProtoNext, "\\Skin\\Images\\proto_Arrow_r.png");                                                     // 0106-05

            Utility.Lib.LoadImageNoLock(imgCleanup, "\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\cleanup_vortex.png");          // 0106-05   0106-15

            Utility.Lib.LoadImageNoLock(imgReplay, "\\Skin\\Images\\Video_restart.png");                                                        // 0106-05

            Utility.Lib.LoadImageNoLock(imgClose, "\\Skin\\Images\\Close.png");                                                                 // 0106-05

            btnTest.Visibility=Visibility.Hidden;

        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }
        #endregion 

        #region Page_Init
        public void page_init(bool do_cleanup)                                                  // 0103-10
        {
            App.HydrafacialSelect = "0";
 
            btnStop.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Collapsed;
            btnVolDown.Visibility = Visibility.Collapsed;
            btnVolUp.Visibility = Visibility.Collapsed;
            VolumeSlider.Visibility = Visibility.Collapsed;
                 
            btnReplay.Visibility = Visibility.Visible;

            cvsVideo.Visibility = Visibility.Hidden;
            cvsSlideShow.Visibility = Visibility.Visible;
            tbkHideContent.Text = "HIDE PROTOCOL";
            btnHideContent_Click(null, null);

            VolumeSlider.Value = ControlParams.Params.p_AudioVolume;

            if (ControlParams.Params.p_SelectDir == "PC\\")                                     // 0100
            {
                btnViewAll.Visibility = Visibility.Visible;
                btnViewAll_mt.Visibility = Visibility.Hidden;
            }
            else if (ControlParams.Params.p_SelectDir == "MT\\")                                // 0103-05
            {
                btnViewAll.Visibility = Visibility.Hidden;
                btnViewAll_mt.Visibility = Visibility.Visible;
            }
            else
            {
                btnViewAll.Visibility = Visibility.Visible;
                btnViewAll_mt.Visibility = Visibility.Visible;
            }

            fmeSlideShow.Content = p1;

            Thickness margin = cvsComponent.Margin;
            margin.Left = 0;
            margin.Top = 0;
            cvsComponent.Margin = margin;

            cvsComponent.Visibility = Visibility.Visible;  

            cvsChargerBars.Visibility = Visibility.Collapsed;

            cvsVacumPump.Visibility = Visibility.Visible;

            VacuumPumpControl.Visibility = Visibility.Visible;

            Utility.Lib.LoadImage(imgIcon, "\\icon-vortex.png");

            Audio_Stop();                                                                       // 0020-10

            // If it is in Perk mode, the background color need be changed.
            VacuumPumpControl.ColorChanged(ControlParams.Params.p_Protopage_selected);          // 0106-18

            if (ControlParams.Params.p_Protopage_selected == 1)
            {
                Utility.Lib.LoadImageNoLock(imgProtoAudio, "\\Skin\\Images\\proto_audio1.png");
                Application.Current.Resources["st_mainblue_default"] = Application.Current.Resources["st_mainblue_pg2"];
            }
            else
            {
                Utility.Lib.LoadImageNoLock(imgProtoAudio, "\\Skin\\Images\\proto_audio.png");
                Application.Current.Resources["st_mainblue_default"] = Application.Current.Resources["st_mainblue_pg1"];
            }

            #region Set Proto Mode Visibility

            // 2015 01/13
            if (ControlParams.Params.p_ProtoMode == false)                                      // for T2 quick start
            {
                bdrProto.Visibility = Visibility.Hidden;                                        // Right side panel
                cvsQT_Control.Visibility = Visibility.Visible;
                cvsProto_Control.Visibility = Visibility.Hidden;

                // 002639-01
                bdrLeftPanel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(App.getTextMessages("st_leftpanelcolor1")));   // 0106-18
                bdrProto.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(App.getTextMessages("st_leftpanelcolor2")));       // 0102-39 FFf5f0eb
                pgHotCold.SetDefaultColor(1);                                                   // 0020-01
            }
            else
            {
                // ver002635  002639-01
                // Set pulseselect buttons hidden 
 
                bdrProto.Visibility = Visibility.Visible;                                       // Right side panel
                cvsProto_Control.Visibility = Visibility.Visible;
                cvsQT_Control.Visibility = Visibility.Hidden;

                bdrLeftPanel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(App.getTextMessages("st_leftpanelcolor2"))); // 0102-39 f5f0eb
                bdrProto.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(App.getTextMessages("st_leftpanelcolor1")));
               
                LoadTips(l_Signature);                                                          // 002639-03
                LoadTitle(l_Signature);                                                         // 002639-03
                LoadBanner(l_Signature);                                                        // 0020-12

                setAudioVisiblility(l_Signature);                                               // 0105

                pgHotCold.SetDefaultColor(0);                                                   // 0020-01
            }
            #endregion

            // Set default to slideshow no video on display
            HideVideo();

            #region Tower 1.5 Clean up requirement
 
            // ======================================
            // Tower 1.5 Clean up requirement
            // ======================================

            if (ControlParams.Params.p_CleanupRequired && do_cleanup)
            {
                // ==============================
                // Display clean up message
                // ==============================

                cvsCleanup.Visibility = Visibility.Visible;

                cvsCleanup_Msg1.Visibility = Visibility.Visible;

                setRinseawayCheckBox(true);

                Utility.Lib.LoadImageFromAppDir(imgCleanup, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage +"/cleanup_vortex.png"); // 0101  0106-13

                // For Vortex-Fusion, it only has start button if the rinseaway exist
                btnStart_PF.Visibility = Visibility.Hidden;

                SetVisibleonStartButton(1);                     // enabled always 0102-36

                cvsCleanup_Msg2.Visibility = Visibility.Hidden;

                ControlParams.Params.p_CleanupRequired = false; // 0103-10

            }
            #endregion

            // ==========================
            // Turn off all control IO
            // ==========================

            TurnOffHardwareIO(ControlParams.Params.p_last_QS_Mode);

            // ====================================================
            // Cleaning Process
            // to avoid clean up message on the first time load
            // ====================================================

            if (ControlParams.Params.p_hydrafacialLoaded == false)
            {
                cvsCleanup.Visibility = Visibility.Hidden;
                ControlParams.Params.p_hydrafacialLoaded = true;
            }
         
            // ==============================
            // This is for Tower 2 
            // ==============================

            //else if (ControlParams.Params.p_last_QS_Mode == ControlParams.e_Mode.VortexFusion ||
            //        ControlParams.Params.p_last_QS_Mode == ControlParams.e_Mode.PulseFusion)
            //{
            //    // ==============================
            //    // Display clean up message
            //    // ==============================

            //    if (ControlParams.Params.p_CleanupRequired) // 0103-10 show clean up screen only by requirement
            //    {
                    
            //    }
            //}

            else if (ControlParams.Params.p_last_QS_Mode == ControlParams.e_Mode.LightTherapy)      // 0100
            {
                // Disable timer and others
                pgLED.page_init(true);
            }
            else if (ControlParams.Params.p_last_QS_Mode == ControlParams.e_Mode.LightBodyTherapy)  // 0100
            {
                // Disable timer and others
                pgLED.page_init(false);
            }

            VacuumPumpControl.Max = (Settings.MD_SPA_Mode == "MD") ? Settings.LymphMax_M : Settings.LymphMax_S; // 0106-15 set default 
           
            // Position H line
            lneH.Width = 470;

            // =====================================
            // Set page format for T2
            // =====================================

            if (btnVideo.Tag == "1")
            {
                btnVideo.Tag = "";
                cvsVideo.Visibility = Visibility.Hidden;
                cvsSlideShow.Visibility = Visibility.Visible;
                tbkVideo.Text = "INFORMATION VIDEO";
            }

            #region Setting for each page

            // =====================================
            // Setting for each page
            // =====================================

            VacuumPumpControl.RingVisible(true);                                                // 0020-02

            switch (ControlParams.Params.p_control_mode)                                        // 0106-06
            {
                case  ControlParams.e_Mode.VortexFusion:
                    
                        Utility.Lib.SaveErrorLog("vortex");                                     // 0106-07
                        
                        Load_AppSettings_Doc_orImage("HydraFacial_Doc");
                        
                        l_TrainingVideo = ConfigurationManager.AppSettings["HydraFacial_Training"];

                        // Highlight the button when call from Home page
                        uc_Buttons.SelectedButton = "btnVortexFusion";

                        VacuumPumpControl.Max = (Settings.MD_SPA_Mode == "MD") ? Settings.VortexMax_M : Settings.VortexMax_S;  // 0106-15 set default 
                       
                        lblControls.Content = App.getTextMessages("VACUUM CONTROLS");           // 0103-09
                        
                        Utility.Lib.LoadImage(imgIcon, "\\icon-vortex.png");

                        cvsComponent.Visibility = Visibility.Hidden;

                        fmeComponent.Content = pgPulseSelect;

                        lblTitle.Content = "Vortex-Fusion®";

                        grdBottle.Visibility = Visibility.Visible;

                        // ===========================

                        // 2014 10/31
                        VacuumPumpControl.cvsRing.Margin = new Thickness(0, 0, 0, 0);

                        VacuumPumpControl.VacuumPressureDisplay = Settings.HydraFacialDefaultVacuum;
                        VacuumPumpControl.VacuumPressure = Settings.HydraFacialDefaultVacuum;

                        DoEvents();

                        Outputs.LogHeader("HydraFacial", "Enter");
                        App.sp.SerialCmdSend("X0000" + "\r");                                   // 0102-36
                        App.Outputs.TargetTemperature = Settings.HotColdTemperatureCoolest;
                        App.Outputs.PulseDuration = 0;
                        App.Outputs.BlueRfidLeds = true;
                        break;

                case ControlParams.e_Mode.PulseFusion:
                    
                        Utility.Lib.SaveErrorLog("pulsefusion");                                // 0106-07

                        Load_AppSettings_Doc_orImage("Pulse_Doc");
                        
                        l_TrainingVideo = ConfigurationManager.AppSettings["Pulse_Training"];

                        margin.Top = 50;

                        cvsComponent.Margin = margin;

                        lblControls.Content = App.getTextMessages("PULSE-FUSION CONTROLS");     // 0103-09
                        
                        Utility.Lib.LoadImage(imgIcon, "\\icon-pulsefusion.png");

                        VacuumPumpControl.RingVisible(false);                                   // 0020-02

                        fmeComponent.Content = pgPulseFusion;

                        lblTitle.Content = "Pulse-Fusion® Treatment";

                        ControlParams.Params.p_last_PulseSelect = "M";

                        grdBottle.Visibility = Visibility.Hidden;

                        pgPulseFusion.uiPulseSelect.setDisplayPulse("M");

                        // ===========================

                        Outputs.LogHeader("VacJet", "Enter");
                        App.Outputs.PulseDuration = 0;
                        App.Outputs.PulseDutyCycle = 0;
                        App.Outputs.ExMass3 = true;
                        App.Outputs.ExMass4 = true;
                        break;

                case  ControlParams.e_Mode.ThermalTherapy:
                   
                        Utility.Lib.SaveErrorLog("Thermal");                                    // 0106-07

                        Load_AppSettings_Doc_orImage("HotCold_Doc");
                        
                        l_TrainingVideo = ConfigurationManager.AppSettings["HotCold_Training"];

                        lblControls.Content = App.getTextMessages("THERMAL CONTROLS");          // 0103-09
                        
                        Utility.Lib.LoadImage(imgIcon, "\\icon-thermal.png");

                        fmeComponent.Content = pgHotCold;

                        lblTitle.Content = "Thermal Therapy";

                        grdBottle.Visibility = Visibility.Hidden;

                        pgHotCold.uiPulseSelect.setDisplayPulse("M");

                        pgHotCold.setHotColdDefaultMode(ControlParams.Params.p_HotColdSelected);

                        // ===========================

                        // 2014 11/20
                        App.Outputs.TargetTemperature = Settings.HotColdTemperatureCoolest;

                        App.Outputs.VacuumPressure = 0;
                        VacuumPumpControl.VacuumPressureDisplay = Settings.HotColdLowVacuum;

                        Outputs.LogHeader("HotCold", "Enter");
                        App.Outputs.PulseDuration = 0;
                        App.Outputs.PulseDutyCycle = 0;
                        App.Outputs.ExMass1 = true;
                        App.Outputs.ExMass4 = false;                                                    // 0102-01
                        break;

                case ControlParams.e_Mode.LightTherapy:

                case ControlParams.e_Mode.LightBodyTherapy:
                    
                        Utility.Lib.SaveErrorLog("Light");                                              // 0106-07

                        lneH.Width = 570;

                        Load_AppSettings_Doc_orImage("Lighting_Doc");
                        
                        l_TrainingVideo = ConfigurationManager.AppSettings["Lighting_Training"];

                        lblControls.Content = App.getTextMessages("TIMER CONTROLS");                    // 0103-09
                        
                        Utility.Lib.LoadImage(imgIcon, "\\icon-led.png");

                        VacuumPumpControl.Visibility = Visibility.Collapsed;

                        fmeComponent.Content = pgLED;
 
                        lblTitle.Content = "LED Light Therapy";

                        grdBottle.Visibility = Visibility.Hidden;

                        if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LightTherapy)   // 0100
                            pgLED.page_init(true);
                        else
                            pgLED.page_init(false);

                        // ===========================

                        Outputs.LogHeader("LEDs", "Enter");
                        App.Outputs.BlueLEDs = true;                                    // 2014 07/22 was false
                        App.Outputs.RedLEDs = false;
                        break;

                case ControlParams.e_Mode.LymphaticFacial:
                  
                        Utility.Lib.SaveErrorLog("facial");                             // 0106-07

                        Load_AppSettings_Doc_orImage("FaceLymph_Doc");
                        
                        l_TrainingVideo = ConfigurationManager.AppSettings["FaceLymph_Training"];

                        lblControls.Content = App.getTextMessages("VACUUM CONTROLS");   // 0103-09
                        
                        Utility.Lib.LoadImage(imgIcon, "\\icon-facialtherapy.png");

                        cvsComponent.Visibility = Visibility.Hidden;                    // 0102-36  for T1.5
                        
                        fmeComponent.Content = pgPulseSelect;

                        margin.Left = 130;

                        cvsComponent.Margin = margin;

                        lblTitle.Content = "Lymphatic Facial Therapy";

                        grdBottle.Visibility = Visibility.Hidden;

                        pgPulseSelect.uiPulseSelect.setDisplayPulse("M");

                        // ===========================

                        // 2014 11/20
                        App.Outputs.TargetTemperature = Settings.HotColdTemperatureCoolest;

                        App.sp.SerialCmdSend("X4095" + "\r");                           // 0102-36

                        // 2014 10/31
                        VacuumPumpControl.cvsRing.Margin = new Thickness(0, 0, 0, 0);   // 0102-36 (0, 90, 0, 0)

                        VacuumPumpControl.VacuumPressure = 0;
                        VacuumPumpControl.VacuumPressureDisplay = Settings.FaceLymphLowVacuum;

                        Outputs.LogHeader("Face Lymph", "Enter");
                        App.Outputs.PulseDuration = 0;
                        App.Outputs.PulseDutyCycle = 0;
                        App.Outputs.ExMass1 = true;
                        App.Outputs.ExMass4 = false;                                    // 0102-01
                        break;

                case  ControlParams.e_Mode.LymphaticBody:
                    
                        Utility.Lib.SaveErrorLog("body");                               // 0106-07

                        Load_AppSettings_Doc_orImage("BodyLymph_Doc");
                        
                        l_TrainingVideo = ConfigurationManager.AppSettings["BodyLymph_Training"];

                        lblControls.Content = App.getTextMessages("VACUUM CONTROLS");   // 0103-09
                        
                        Utility.Lib.LoadImage(imgIcon, "\\icon-bodytherapy.png");

                        cvsComponent.Visibility = Visibility.Hidden;                    // 0102-36  For T1.5
                        
                        fmeComponent.Content = pgPulseSelect;

                        margin.Left = 130;

                        cvsComponent.Margin = margin;

                        lblTitle.Content = "Lymphatic Body Therapy";

                        grdBottle.Visibility = Visibility.Hidden;

                        pgPulseSelect.uiPulseSelect.setDisplayPulse("M");

                        // ===========================

                        // 2014 11/20
                        App.Outputs.TargetTemperature = Settings.HotColdTemperatureCoolest;

                        App.sp.SerialCmdSend("X4095" + "\r");   // 0102-36

                        // 2014 10/31
                        VacuumPumpControl.cvsRing.Margin = new Thickness(0, 0, 0, 0);   // 0102-36 (0, 90, 0, 0)

                        VacuumPumpControl.VacuumPressure = 0;
                        VacuumPumpControl.VacuumPressureDisplay = Settings.BodyLymphLowVacuum;

                        Outputs.LogHeader("Body Lymph", "Enter");
                        App.Outputs.PulseDuration = 0;
                        App.Outputs.PulseDutyCycle = 0;
                        App.Outputs.ExMass1 = true;
                        App.Outputs.ExMass4 = false;                                    // 0102-01
                        break;

                case ControlParams.e_Mode.Purchase:                                     // 0020-01
                    
                        Utility.Lib.SaveErrorLog("purchase");                           // 0106-07
                        
                        lblControls.Content = "";

                        Utility.Lib.LoadImage(imgIcon, "\\Daily_Essentials_icon.png");  // no need for icon
                        
                        fmeComponent.Content = pgPurchase;

                        VacuumPumpControl.Visibility = Visibility.Collapsed;
                        break;
            }

            #endregion

            Utility.Lib.SaveErrorLog("leave");                                                  // 0106-07
            DoEvents();

            // 2014 12/02
            //GC.Collect(2);
            //GC.WaitForPendingFinalizers();
            //GC.Collect(2);
        }
        #endregion

        #region Clean Up Page

        private void CleanUpMessage()
        {
            // ==============================
            // Display clean up message
            // ==============================

            cvsCleanup.Visibility = Visibility.Visible;

            cvsCleanup_Msg1.Visibility = Visibility.Visible;

            setRinseawayCheckBox(true);

            Utility.Lib.LoadImageFromAppDir(imgCleanup, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage + "/cleanup_vortex.png"); // 0101  0106-13

            // For Vortex-Fusion, it only has start button if the rinseaway exist
            btnStart_PF.Visibility = Visibility.Hidden;

            SetVisibleonStartButton(1);                                     // enabled always 0102-36

            cvsCleanup_Msg2.Visibility = Visibility.Hidden;

            ControlParams.Params.p_CleanupRequired = false;                 // 0103-10
        }

        #endregion

        #region Protomode_init

        // this will overwrite the pgae init settings
        public void protoMode_init(bool doCleanup)
        {
            if (ControlParams.Params.p_TreatmentSteps <= 1)                 // 0101
            {
                btnProtoNext.Visibility = Visibility.Hidden;
                btnProtoPrevious.Visibility = Visibility.Hidden;
            }
            else
            {
                btnProtoNext.Visibility = Visibility.Visible;               // 0105
                btnProtoPrevious.Visibility = Visibility.Visible;
            }

            // Overwrite the current settings
            if (ControlParams.Params.IntegrateMode[2, l_Signature] != 0)    //  check pump 0 is do nothing 
            {
                VacuumPumpControl.VacuumPressureDisplay = ControlParams.Params.IntegrateMode[2, l_Signature];
                VacuumPumpControl.VacuumPressure = ControlParams.Params.IntegrateMode[2, l_Signature];
            }

            if (ControlParams.Params.IntegrateMode[3, l_Signature] != 0)    // check pulse 0 is do nothing 
            {
                if (ControlParams.Params.IntegrateMode[3, l_Signature] == 1)
                    pgPulseFusion.uiPulseSelect.setDisplayPulse("L");
                else if (ControlParams.Params.IntegrateMode[3, l_Signature] == 2)
                    pgPulseFusion.uiPulseSelect.setDisplayPulse("M");
                else if (ControlParams.Params.IntegrateMode[3, l_Signature] == 3)
                    pgPulseFusion.uiPulseSelect.setDisplayPulse("H");
            }

            if (ControlParams.Params.IntegrateMode[4, l_Signature] != 0)    // check Hot/Cold 0 => do nothing 
            {
                if (ControlParams.Params.IntegrateMode[4, l_Signature] == 1)
                    pgHotCold.setHotColdDefaultMode("HOT");
                else if (ControlParams.Params.IntegrateMode[4, l_Signature] == 2)
                    pgHotCold.setHotColdDefaultMode("COLD");
            }
        }

        #endregion
 
        #region OnEnter/OnLeave Event

        private void OnEnter()
        {
            Outputs.LogHeader("HydraFacial", "Enter");

            App.Outputs.TargetTemperature = Settings.HotColdTemperatureCoolest;
            App.Outputs.PulseDuration = 0;
            App.Outputs.VacuumPressure = VacuumPumpControl.VacuumPressure;
            App.Outputs.BlueRfidLeds = true;
            VacuumPumpControl.VacuumPressure = Settings.HydraFacialDefaultVacuum;
        }

        private void OnLeave()  
        {
            Audio_Stop();                                   // 0020-10

            App.BoardManager.WaitForGroupParams = true;     // 0020-11

            VacuumPumpControl.VacuumPump = false;
            App.Outputs.BlueRfidLeds = false;

            App.Outputs.Solenoid1 = false;
            App.Outputs.Solenoid2 = false;
            App.Outputs.Solenoid3 = false;
            App.Outputs.Solenoid4 = false;
            App.Outputs.Solenoid5 = false;

            B0.IsChecked = false;
            B1.IsChecked = false;
            B2.IsChecked = false;
            B3.IsChecked = false;

            App.BoardManager.WaitForGroupParams = false;    // 0020-11

            B4.IsChecked = false;

            Outputs.LogHeader("HydraFacial", "Exit");
        }

        #endregion 

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Propertychange Event

        private void OutputsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "VacuumPump":
                    if (!App.Outputs.VacuumPump)
                    {
                        App.Outputs.Solenoid1 = false;
                        App.Outputs.Solenoid2 = false;
                        App.Outputs.Solenoid3 = false;
                        App.Outputs.Solenoid4 = false;
                        App.Outputs.Solenoid5 = false;
                    }
                    else
                    {
                        App.Outputs.Solenoid1 = B0.IsChecked.Value;
                        App.Outputs.Solenoid2 = B1.IsChecked.Value;
                        App.Outputs.Solenoid3 = B2.IsChecked.Value;
                        App.Outputs.Solenoid4 = B3.IsChecked.Value;
                        App.Outputs.Solenoid5 = B4.IsChecked.Value;
                    }
                    break;
            }
        }

        // Event , sww modified
        private void AppBottleChanged(object sender, BottleChangedEventArgs ea)
        {
            Dispatcher.BeginInvoke((Action) (() =>
            {
                AppBottleChanged_Run(ea);
            }));
        }
        #endregion 

        #region Bottle Control

        // When Bottole Changed
        private void AppBottleChanged_Run(BottleChangedEventArgs ea)
        {
            try
            {
                // T2
                // 2014 10/29
                //getProductSize((string)Product.Name(ea.Station.PartNumber));

                string _pdname, _pdsize;
                if (DB_Product.FindProductName(ea.Station.PartNumber, out _pdname, out _pdsize))
                {
                    pd_name = _pdname;
                    pd_size = _pdsize;
                }
                else
                {
                    pd_name = "";
                    pd_size = "";
                    if (ea.Station.PartNumber != "" && ea.Station.PartNumber != null)
                    {
                        DB_Product.InsertProductInfo(ea.Station.PartNumber, ea.Station.ProductName, ea.Station.ProductSize);

                        if (DB_Product.FindProductName(ea.Station.PartNumber, out _pdname, out _pdsize))
                        {
                            pd_name = _pdname;
                            pd_size = _pdsize;
                        }
                    }
                }

                BottleNames[ea.BottleIndex] = pd_name;

                if (ea.BottleIndex == 0)
                    lblBottle0.Content = pd_size; 
                else if (ea.BottleIndex == 1)
                    lblBottle1.Content = pd_size;
                else if (ea.BottleIndex == 2)
                    lblBottle2.Content = pd_size; 
                else if (ea.BottleIndex == 3)
                    lblBottle3.Content = pd_size;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BottleNames"));

                if (BottleNames[ea.BottleIndex] == "")
                {
                    if (ea.BottleIndex == 0)
                        B0.IsChecked = false;
                    else if (ea.BottleIndex == 1)
                        B1.IsChecked = false;
                    else if (ea.BottleIndex == 2)
                        B2.IsChecked = false;
                    else if (ea.BottleIndex == 3)
                        B3.IsChecked = false;
                    else if (ea.BottleIndex == 4)
                        B4.IsChecked = false;
                }

                if (ControlParams.Params.p_BottleCountOn) // 0020-03
                {
                    if (ea.Station.PartNumber == "70140")
                        ControlParams.Params.valid_insertedbottle[ea.BottleIndex] = ((UsageReport.UsageReport)App._mainWindows[Mode.UsageReport]).Model.ValidUIDInsertion(ea.Station.Uid, Settings.MaxRinseawayInsertions);
                    else
                        ControlParams.Params.valid_insertedbottle[ea.BottleIndex] = ((UsageReport.UsageReport)App._mainWindows[Mode.UsageReport]).Model.ValidUIDInsertion(ea.Station.Uid, Settings.MaxBottleInsertions);
                }
                else
                { 
                    ControlParams.Params.valid_insertedbottle[ea.BottleIndex] = true;
                }

                B0.IsEnabled = (ControlParams.Params.valid_insertedbottle[0] && IsStationEnabled(0)) ? true : false;
                B1.IsEnabled = (ControlParams.Params.valid_insertedbottle[1] && IsStationEnabled(1)) ? true : false;
                B2.IsEnabled = (ControlParams.Params.valid_insertedbottle[2] && IsStationEnabled(2)) ? true : false;
                B3.IsEnabled = (ControlParams.Params.valid_insertedbottle[3] && IsStationEnabled(3)) ? true : false;
                B4.IsEnabled = (ControlParams.Params.valid_insertedbottle[4] && IsStationEnabled(4)) ? true : false;

                // 2014 12/09
                ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B0.IsEnabled = B0.IsEnabled;
                ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B1.IsEnabled = B1.IsEnabled;
                ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B2.IsEnabled = B2.IsEnabled;
                ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B3.IsEnabled = B3.IsEnabled;
                ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B4.IsEnabled = B4.IsEnabled;
               
                var r = FindRinseawayStation();                                                 // 0102-36
                SetVisibleonStartButton(r);                                                     // for clean up

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Bottle Changes >>"+e.Message);                  // need check  ???
            }
        }

        private bool ValidBottle()
        {
            return true;
        }

        private string pd_name { get; set; }
        private string pd_size { get; set; }
        private void getProductSize(string Str)
        {
            try
            {
                if (Str == "" || Str == null)
                {
                    pd_name = "";
                    pd_size = "";
                    return;
                }
                if (Str.IndexOf("(") == -1 || Str.IndexOf(")") == -1)
                {
                    pd_name = Str;
                    pd_size = "";
                    return;
                }

                pd_name = Str.Substring(0, Str.IndexOf("(") - 1);
                pd_size = Str.Substring(Str.IndexOf("("), Str.IndexOf(")") - Str.IndexOf("(") + 1);
            }
            catch (Exception e)
            {
                pd_name = "";
                pd_size = "";
            }
        }

        private bool IsStationEnabled(int stationId)
        {
            const string rinseawayPn = "70140";             // 0020-05 00262

            var stationState = App.BoardManager.mStationStates[stationId];

            if (stationState == null) 
                return false;

            if (stationId == 4)
                return stationState.DataIsValid() && stationState.PartNumber == rinseawayPn;

            return stationState.DataIsValid() /*&& stationState.PartNumber != rinseawayPn*/;
        }

        private string getStationUID(int stationId)
        {
            var stationState = App.BoardManager.mStationStates[stationId];

            if (stationState == null)
                return "";

            return stationState.Uid;
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            ((Action<bool>) ((FrameworkElement) sender).Tag).Invoke(true);

            B4.IsChecked = false;
           
            if (_selectedCheckBoxes.Count == 1)   // 2014 09/17 was 2
            {
                _selectedCheckBoxes.Peek().IsChecked = false;
            }

            _selectedCheckBoxes.Enqueue((System.Windows.Controls.CheckBox)sender);

            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B0.IsChecked = B0.IsChecked;
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B1.IsChecked = B1.IsChecked;
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B2.IsChecked = B2.IsChecked;
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B3.IsChecked = B3.IsChecked;
        }

        private int FindRinseawayStation()
        { 
            for (int i=0 ;i<=4 ;i++ )
            {
                if (BottleNames[i].Length >= 9)
                {
                    if (BottleNames[i].Substring(0, 9) == "Rinseaway")
                        return i;
                }
            }

            return -1;
        }

        private void SetVisibleonStartButton(int i)
        {
            if (i == -1)
                btnStart.Visibility = Visibility.Hidden;
            else
            {
                btnStart.Visibility = Visibility.Visible;
                ControlParams.Params.p_RinseawayStation = i;
            }
        }

        private void RinseCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            ((Action<bool>) ((FrameworkElement) sender).Tag).Invoke(true);

            _inRinseCheckHandler = true;

            while (_selectedCheckBoxes.Count != 0)
            {
                _selectedCheckBoxes.Dequeue().IsChecked = false;
            }

            _inRinseCheckHandler = false;
        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            ((Action<bool>) ((FrameworkElement) sender).Tag).Invoke(false);

            if (_inRinseCheckHandler)
                return;

            var checkBox = _selectedCheckBoxes.Dequeue();

            if (checkBox != sender)
            {
                _selectedCheckBoxes.Dequeue();
                _selectedCheckBoxes.Enqueue(checkBox);
            }
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B0.IsChecked = B0.IsChecked;
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B1.IsChecked = B1.IsChecked;
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B2.IsChecked = B2.IsChecked;
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).B3.IsChecked = B3.IsChecked;

            // The 5th station will be "RinseCheckBoxUnchecked"
        }

        private void RinseCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            ((Action<bool>) ((FrameworkElement) sender).Tag).Invoke(false);
        }

        #endregion

        #region Video Control

        private void LoadVideoFile(String FileName)
        {
            MediaElement1.Close();
            
            // 2014 10/27
            Thread.Sleep(300);

            var FileLocation = Environment.CurrentDirectory + "\\Videos\\" + ControlParams.Params.p_SecondLanguage + "\\Application\\" + FileName;
            if (!File.Exists(FileLocation))
            {
                System.Windows.MessageBox.Show("Video File Not Found!");
                return;
            }
            try
            {
                MediaElement1.Source = new Uri(FileLocation);

            }
            catch { new NullReferenceException("Error"); }
             
            VolumeSlider.Value = MediaElement1.Volume;
            
            // 2014 10/27
            Thread.Sleep(300);

            dispatchertimer.Start();

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

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Pause();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (btnPlay.Tag == "")
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
                MediaElement1.Play();
                btnPlay.Tag = "Stop";
                SetButtonImage(btnPlay, "imgPlay", "/Skin/Images/n_Stop.png");
            }
            else
            {
                MediaElement1.Pause();
                btnPlay.Tag = "";
                SetButtonImage(btnPlay, "imgPlay", "/Skin/Images/n_Play.png");
            }
        }
 
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pbrVideo.Value = MediaElement1.Position.TotalSeconds;

            TimeSpan ts = TimeSpan.FromSeconds(MediaElement1.Position.TotalSeconds);
            string str = ts.ToString(@"mm\:ss");
            lblProgressValue.Content = str ;
        }
  
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement1.Volume = VolumeSlider.Value;
        }

        public void setVolume(double V)
        {
            VolumeSlider.Value = V;
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            // set protocol to hidden 
            HideContent();
            cvsProtocol.Visibility = Visibility.Hidden;

            if (btnVideo.Tag == "1")
            {
                btnVideo.Tag = "";
                cvsVideo.Visibility = Visibility.Hidden;
                cvsSlideShow.Visibility = Visibility.Visible;

                MediaElement1.Stop();

                tbkVideo.Text = "INFORMATION VIDEO";
            }
            else
            {
                btnVideo.Tag = "1";
                cvsVideo.Visibility = Visibility.Visible;
                cvsSlideShow.Visibility = Visibility.Hidden;

                // Disply slideshow left right controller
                btnLeft.Visibility = Visibility.Hidden;
                btnRight.Visibility = Visibility.Hidden;

                tbkVideo.Text = "CLOSE VIDEO";

                LoadVideoFile(l_TrainingVideo);                // l_TrainingVideo will be dynamic assign video file for each mode
                SetvideoOn(true);
            }
        }

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Stop();
            DoEvents();
            MediaElement1.Play();
        }

        #endregion

        #region Screen Control

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            fmeSlideShow.Content = p1;
            Storyboard sb = this.FindResource("sbSlideL") as Storyboard;
            
            // Assign button to story board
            Storyboard.SetTarget(sb, this.fmeSlideShow);
            sb.Begin();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            fmeSlideShow.Content = p2;
            Storyboard sb = this.FindResource("sbSlideR") as Storyboard;
            
            // Assign button to story board
            Storyboard.SetTarget(sb, this.fmeSlideShow);
            sb.Begin();
        }
 
        private void btnHideContent_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Stop();
            
            if (tbkHideContent.Text == "HIDE PROTOCOL")
            {
                tbkHideContent.Text = "ABOUT THE TREATMENT";
                cvsProtocol.Visibility = Visibility.Hidden;
            }
            else
            {
                tbkHideContent.Text = "HIDE PROTOCOL";
                cvsProtocol.Visibility = Visibility.Visible;
            }

            //===========================================

            if (btnVideo.Tag == "1")
            {
                btnVideo.Tag = "";
                cvsVideo.Visibility = Visibility.Hidden;
                cvsSlideShow.Visibility = Visibility.Visible;

                MediaElement1.Stop();

                tbkVideo.Text = "INFORMATION VIDEO";
            }
        }

        private void HideContent()
        {
            if (tbkHideContent.Text == "Hide Protocol")
            {
                tbkHideContent.Text = "Show Protocol";
                cvsProtocol.Visibility = Visibility.Hidden;
            }
        }

        private void HideVideo()
        {
            // set protocol to hidden 
            HideContent();
                       
            btnVideo.Tag = "";
            cvsVideo.Visibility = Visibility.Hidden;
            cvsSlideShow.Visibility = Visibility.Visible;

            if( btnPlay.Tag == "Stop" )                                                         // if Video is playing, stop it
                MediaElement1.Stop(); 

            tbkVideo.Text = "INFORMATION VIDEO";
        }

        #endregion

        #region Hardware Control

        private void TurnOffHardwareIO(int hardwaremode)
        {
            App.BoardManager.WaitForGroupParams = true; // 0020-11

            App.Outputs.DC_Motor = false;               // 2014 09/15

            App.sp.SerialCmdSend("VF" + "\r");          // 0102-36

            switch (hardwaremode)
            {
                case ControlParams.e_Mode.VortexFusion: // "Vortex-Fusion":// Hydrafacial
                   
                    App.Outputs.BlueRfidLeds = false;

                    App.Outputs.Solenoid1 = false;
                    App.Outputs.Solenoid2 = false;
                    App.Outputs.Solenoid3 = false;
                    App.Outputs.Solenoid4 = false;
                    App.Outputs.Solenoid5 = false;

                    B0.IsChecked = false;
                    B1.IsChecked = false;
                    B2.IsChecked = false;
                    B3.IsChecked = false;
                    B4.IsChecked = false;
                    break;

                case ControlParams.e_Mode.PulseFusion:      // Pulse fusion
                   
                    App.Outputs.PulseDuration = 0;
                    App.Outputs.ExMass3 = false;
                    App.Outputs.DC_Motor = false;           // 2014 07/21
                    break;

                case ControlParams.e_Mode.ThermalTherapy:   // HotCold
                   
                    App.Outputs.PulseDuration = 0;
                    App.Outputs.ExMass1 = false;
                     
                    break;

                case ControlParams.e_Mode.LightTherapy:     // "LightTherapy": // LEDs
                    
                    App.Outputs.BlueLEDs = true;            //2014 07/22 was false, changed by sww
                    App.Outputs.RedLEDs = false;

                    // To be add =============================
                    //ClockIsRunning = false;
                    //TimeString.Foreground = Brushes.Black;
                    //btnLedReset.IsEnabled = false;
                    //_accumlatedTime = TimeSpan.Zero;
                    //TimeSpan = TimeSpan.Zero;
                    //========================================

                    break;

                case ControlParams.e_Mode.LymphaticFacial:

                    // Turn off all control IO
                    // FaceLymph
                   
                    App.Outputs.VacuumPressure = 0;
                    App.Outputs.PulseDuration = 0;
                    App.Outputs.PulseDutyCycle = 0;
                    App.Outputs.ExMass1 = false;
                    break;

                case ControlParams.e_Mode.LymphaticBody: // Body
                   
                    App.Outputs.VacuumPressure = 0;
                    App.Outputs.PulseDuration = 0;
                    App.Outputs.PulseDutyCycle = 0;
                    App.Outputs.ExMass1 = false;
                    break;
            }

            App.BoardManager.WaitForGroupParams = false; // 0020-11
            VacuumPumpControl.VacuumPump = false;
        }
 
        private void TurnOffHardwareIO_ALL()
        {
            App.BoardManager.WaitForGroupParams = true; // 0020-11

            App.Outputs.DC_Motor = false; 

            VacuumPumpControl.VacuumPump = false;
            App.Outputs.BlueRfidLeds = false;

            App.Outputs.Solenoid1 = false;
            App.Outputs.Solenoid2 = false;
            App.Outputs.Solenoid3 = false;
            App.Outputs.Solenoid4 = false;
            App.Outputs.Solenoid5 = false;

            B0.IsChecked = false;
            B1.IsChecked = false;
            B2.IsChecked = false;
            B3.IsChecked = false;
            B4.IsChecked = false;

            //================================

            App.Outputs.VacuumPump = false;
            
            App.sp.SerialCmdSend("VF" + "\r"); // 0102-36
          
            App.Outputs.PulseDuration = 0;
          
            //================================
         
            App.Outputs.ExMass1 = false;
            App.Outputs.ExMass3 = false;
            
            //================================
           
            App.Outputs.BlueLEDs = true;  
            App.Outputs.RedLEDs = false;

            //================================
           
            App.Outputs.VacuumPressure = 0;
 
            App.BoardManager.WaitForGroupParams = false;                        // 0020-11

            App.Outputs.PulseDutyCycle = 0;
        }

        #endregion

        #region Clean up Control with start button

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            cvsCleanup.Visibility = Visibility.Visible;

            ControlParams.Params.p_CleanupRequired = false;                     // 0103-10

            App.BoardManager.WaitForGroupParams = true;                         // 0020-11

            App.Outputs.PulseDuration = 0;
            App.Outputs.ExMass1 = false;
            App.Outputs.ExMass2 = false;
            App.Outputs.ExMass3 = true;
            App.Outputs.ExMass4 = false;

            App.Outputs.VacuumPump = true;
            App.Outputs.DC_Motor = false;

            App.Outputs.VacuumPressure = 16;

            cvsCleanup_Msg1.Visibility = Visibility.Hidden;
            cvsCleanup_Msg2.Visibility = Visibility.Visible;

            l_TargetVacuum = VacuumPumpControl.VacuumPressureDisplay;                           // ver 0026212  0020-05

            // Start Timer
            if (ControlParams.Params.p_last_QS_Mode == ControlParams.e_Mode.PulseFusion)        // 0106-08
            {
                // 0020-02
                App.Outputs.ExMass3 = true;
                App.Outputs.VacuumPressure = Settings.CleanupVacuum_PulseFusion;
                VacuumPumpControl.VacuumPressureDisplay = Settings.CleanupVacuum_PulseFusion;

                // turn on pump and set vortex mode
                TurnOnPump_Vortex(Settings.CleanupVacuum_PulseFusion);                          // 0106-08   // 0106-14

                InitCleanupProgressBar(Settings.CleanupSeconds_PulseFusion);
            }
            else    
            {
                // Find CheckRinseaway Location

                // 0020-02
                App.Outputs.ExMass3 = false;
                App.Outputs.VacuumPressure = Settings.CleanupVacuum_Vortex;
                VacuumPumpControl.VacuumPressureDisplay = Settings.CleanupVacuum_Vortex;

                // Turn on pump and set vortex mode
                TurnOnPump_Vortex(Settings.CleanupVacuum_Vortex);                               // 0106-08  // 0106-14

                // Set to 2 minutes or ...
                InitCleanupProgressBar(Settings.CleanupSeconds_Vortex);
            }

            App.BoardManager.WaitForGroupParams = false;                                        // 0020-11

            App.Outputs.VacuumPump = true;                                                      // 0020-02
 
            this.Dispatcher.Invoke((Action)(() =>
            {
                timer.Start(); 
            }), DispatcherPriority.ContextIdle, null);
            
        }

        // turn on pump and set vortex mode
        private void TurnOnPump_Vortex(int value)                                               // 0106-08  for Tower 1.5  // 0106-14
        {
            App.BoardManager.AddLogMessage("pressure=" + value);                                // 0106-16

            App.sp.SerialCmdSend("X0000" + "\r");                                               // 0102-36  

            int V = (int)(value * 64.62 + 55.38);
            string s = (V).ToString().PadLeft(4, '0');
            if (App.sp != null)
            {
                App.sp.SerialCmdSend("v" + s + "\r");
                App.sp.SerialCmdSend("VN" + "\r");                                              // 0102-36
            }
        }

        // skip cleanning and exit
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            cvsCleanup.Visibility = Visibility.Hidden;
            setRinseawayCheckBox(false);
            
            // If it was click for Home page, then continue to home page
            if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Home_Page)
                App.Go(Mode.Home);

            else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Proto_Page)   
                App.Go(Mode.wpfProto);

            else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.Settings)              // 0102-11
                App.Go(Mode.wpfSettings);

            else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Modalities_Page)    // 0106-06
                App.Go(Mode.wpfModalities);
        }

        private void setRinseawayCheckBox(bool B)
        {
            if ( ControlParams.Params.p_RinseawayStation == 0)
                B0.IsChecked=B;
            else if ( ControlParams.Params.p_RinseawayStation == 1)
                B1.IsChecked=B;
            else if ( ControlParams.Params.p_RinseawayStation == 2)
                B2.IsChecked=B;
            else if ( ControlParams.Params.p_RinseawayStation == 3)
                B3.IsChecked=B;
            else if ( ControlParams.Params.p_RinseawayStation == 4)
                B4.IsChecked = B;
        }

        private void InitCleanupProgressBar(int max)
        {
            pbrClean.Value = 0;
            pbrClean.Maximum = max; 
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                UpdateCleanupProgressBar();
            }
            ));
        }

        #endregion 
 
        #region Cleanup

        // 0020-02
        private void UpdateCleanupProgressBar()   // 0106-14
        {
            if (pbrClean.Value < pbrClean.Maximum)
            {
                pbrClean.Value += 1;
            }
            else
            {
                timer.Stop();
                CloseCleanupInformation();

                App.Outputs.VacuumPressure = l_TargetVacuum;                // 0026212 set back the value
                VacuumPumpControl.VacuumPressureDisplay = l_TargetVacuum;   // 0026212 set back the value

                // Turn off hardware IO
                TurnOffHardwareIO(ControlParams.Params.p_last_QS_Mode);

                // 2015 01/05 It need turn on pulse back when go to LymphaticFacial, LymphaticBody and hotcold
                if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LymphaticFacial ||
                    ControlParams.Params.p_control_mode == ControlParams.e_Mode.LymphaticBody ||
                    ControlParams.Params.p_control_mode == ControlParams.e_Mode.ThermalTherapy)
                    App.Outputs.ExMass1 = true;

                // ver 002621
                if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.PulseFusion)
                {
                    App.sp.SerialCmdSend("X0000" + "\r");                                       // 0106-14
                    App.sp.SerialCmdSend("VF" + "\r");                                          // 0106-14

                    App.Outputs.ExMass3 = true;
                }
                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.VortexFusion) 
                {
                    App.sp.SerialCmdSend("X0000" + "\r");                                       // 0106-14
                    App.sp.SerialCmdSend("VF" + "\r");                                          // 0106-14

                    App.Outputs.ExMass3 = false;
                }
                else
                {
                    App.sp.SerialCmdSend("X4095" + "\r");                                       // 0106-14
                    App.sp.SerialCmdSend("VF" + "\r");                                          // 0106-14
                    App.Outputs.ExMass3 = false;
                }

                // ver 002633
                if (ControlParams.Params.p_ProtoMode == true)
                {
                    if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Home_Page)           // 0101-02
                        App.Go(Mode.Home);
                    
                    else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Proto_Page)     // 0102-09
                        App.Go(Mode.wpfProto);

                    else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.Settings)          // 0102-11
                        App.Go(Mode.wpfSettings);

                    else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Modalities_Page)// 0106-06
                        App.Go(Mode.wpfModalities);
                    
                    return;
                }
                
                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.To_Proto_Page)         //  Ver 002633
                    App.Go(Mode.wpfProto);

                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.Settings)              // 0102-11
                    App.Go(Mode.wpfSettings);
            }
        }

        private void CloseCleanupInformation()
        {
            // Turn off clean up message
            cvsCleanup.Visibility = Visibility.Hidden;
            cvsCleanup_Msg1.Visibility = Visibility.Hidden;
            cvsCleanup_Msg2.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Battery Control

        public void setBattery(int[] V, bool[] Charging)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                try
                {
                    if (pgLED != null)
                    {
                        pgLED.pgbB0.Value = V[0];
                        pgLED.pgbB1.Value = V[1];
                        pgLED.pgbB2.Value = V[2];
                        pgLED.pgbB3.Value = V[3];

                        // 2014 12/08
                        pgLED.lblBatteryLevel0.Content = Avoid0and100(V[0]);
                        pgLED.lblBatteryLevel1.Content = Avoid0and100(V[1]);
                        pgLED.lblBatteryLevel2.Content = Avoid0and100(V[2]);
                        pgLED.lblBatteryLevel3.Content = Avoid0and100(V[3]);

                        ChangeFontColor(pgLED.lblBatteryLevel0, Charging[0]);
                        ChangeFontColor(pgLED.lblBatteryLevel1, Charging[1]);
                        ChangeFontColor(pgLED.lblBatteryLevel2, Charging[2]);
                        ChangeFontColor(pgLED.lblBatteryLevel3, Charging[3]);
                    }
                }
                catch
                {
                  
                  
                }
            }));
        }

        private string Avoid0and100(int value)
        {
            if (value == 0 )
                return "~";
            else
                return value.ToString();
        }

        private void ChangeFontColor(System.Windows.Controls.Label L, bool value)
        {
            if (value)
                L.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF08C5D8"));
            else
                L.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA8A8A8"));
        }

        #endregion

        #region Bottole Control

        private void OnTargetUpdated_B0(object sender, DataTransferEventArgs e)
        {
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).tbkB0.Text = tbkB0.Text;
        }

        private void OnTargetUpdated_B1(object sender, DataTransferEventArgs e)
        {
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).tbkB1.Text = tbkB1.Text;
        }

        private void OnTargetUpdated_B2(object sender, DataTransferEventArgs e)
        {
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).tbkB2.Text = tbkB2.Text;
        }

        private void OnTargetUpdated_B3(object sender, DataTransferEventArgs e)
        {
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).tbkB3.Text = tbkB3.Text;
        }

        // ver 00262 0020-05
        private void OnTargetUpdated_B4(object sender, DataTransferEventArgs e)
        {
            ((wpfStatus)App._mainWindows[Mode.wpfStatus]).tbkB4.Text = tbkB4.Text;
        }

        #endregion

        #region Proto control buttons

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            ((wpfProto)App._mainWindows[Mode.wpfProto]).setMenuPage(ControlParams.e_Proto.Option);
           
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;//  "To_Proto_Page";

            if (ControlParams.Params.p_CleanupRequired == true)

                // when go to the clean up page, the target proto page will be set on the clean up buttons.
                App.Go(Mode.HydraFacial);

            else
            {
                App.Go(Mode.wpfProto);
            }
        }

        private void btnViewAll_mt_Click(object sender, RoutedEventArgs e)                      // 0100
        {
            ControlParams.Params.p_ProtoMode = true;
            
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Modalities_Page;      // "Proto"; // 0106-06

            if (ControlParams.Params.p_CleanupRequired == true)                                 // 0106-05
            {
                App.Go(Mode.HydraFacial);
                ((wpfModalities) App._mainWindows[Mode.wpfModalities]).setMenuPage(Menu.Main);  // 0106-05
            }
            else
            {
                ((wpfModalities) App._mainWindows[Mode.wpfModalities]).setMenuPage(Menu.Main);  // 0102-39
                App.Go(Mode.wpfModalities);
            }
        }

        private void btnProtoPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (!ControlParams.Params.p_InCleaningPage)                                         // 0106-03
            {
                if (App.sp != null)
                    App.sp.SerialCmdSend("VF" + "\r");                                          // 0103-05
            }

            this.IsEnabled = false;

            Audio_Stop();                                                                       // 0020-10

            if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LightTherapy)       // 0100
                pgLED.page_init(true);
            else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LightBodyTherapy)
                pgLED.page_init(false);

            if (l_Signature - 1 <= 1)
                l_Signature = 1;
            else
                l_Signature--;
            
            if (!ControlParams.Params.p_InCleaningPage)                                         // 0106-03
            {
                VacuumPumpControl.VacuumPump = false;                                           // 002639-05
                VacuumPumpControl.VacuumPressure = 0;                                           // 002639-05
            }

            LoadTips(l_Signature);                                                              // 002639-01
            LoadTitle(l_Signature);
            LoadBanner(l_Signature);                                                            // 0020-12

            ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.To_Proto_Page;

            if (ControlParams.Params.p_control_mode !=
                    ControlParams.Params.IntegrateMode[ControlParams.Params.p_ProtoOpt, l_Signature])
            {
                ControlParams.Params.p_control_mode =
                    ControlParams.Params.IntegrateMode[ControlParams.Params.p_ProtoOpt, l_Signature];
                page_init(false);
            }

            protoMode_init(false);

            this.IsEnabled = true;
        }

        private void btnProtoNext_Click(object sender, RoutedEventArgs e)
        {

            if (!ControlParams.Params.p_InCleaningPage)                                         // 0106-03
            {
                if (App.sp != null)
                    App.sp.SerialCmdSend("VF" + "\r");                                          // 0103-05
            }

            this.IsEnabled = false;

            Audio_Stop();                                                                       // 0020-10

            if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LightTherapy)       // 0100
                pgLED.page_init(true);
            else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LightBodyTherapy)
                pgLED.page_init(false);

            if (l_Signature + 1 >= ControlParams.Params.p_TreatmentSteps)
                l_Signature = ControlParams.Params.p_TreatmentSteps;
            else
                l_Signature++;

            if (!ControlParams.Params.p_InCleaningPage) // 0106-03
            {
                VacuumPumpControl.VacuumPump = false;   // 002639-05
                VacuumPumpControl.VacuumPressure = 0;   // 002639-05
            }

            LoadTips(l_Signature);                      // 002639-01
            LoadTitle(l_Signature);
            LoadBanner(l_Signature);                    // 0020-12

            ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.To_Proto_Page; 

            if (ControlParams.Params.IntegrateMode[1, l_Signature] == 1)        // Vortex clean up
            {
                ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.VortexFusion;
                ControlParams.Params.p_control_mode = ControlParams.Params.IntegrateMode[ControlParams.Params.p_ProtoOpt, l_Signature];
                
                page_init(true);
            }
            else if (ControlParams.Params.IntegrateMode[1, l_Signature] == 2)   // Pulse Fusion clean up
            {
                ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.PulseFusion;
                ControlParams.Params.p_control_mode = ControlParams.Params.IntegrateMode[ControlParams.Params.p_ProtoOpt, l_Signature];
                
                page_init(true);
            }
            else if (ControlParams.Params.p_control_mode != ControlParams.Params.IntegrateMode[ControlParams.Params.p_ProtoOpt, l_Signature])
            {
                ControlParams.Params.p_control_mode = ControlParams.Params.IntegrateMode[ControlParams.Params.p_ProtoOpt, l_Signature];
                
                page_init(false);
            }

            protoMode_init(false);

            this.IsEnabled = true;
        }

        private void Audio_Stop()
        {
            try
            {
                if (Audioplayer != null)
                    Audioplayer.Stop();
            }
            catch (Exception ex)                                                                // 0020-11 
            {
                MessageBox.Show(ex.Message);
            }

            if (ControlParams.Params.p_Protopage_selected == 1)                                 // 0106-18
                Utility.Lib.LoadImageFromAppDir(imgProtoAudio, "/Skin/images/proto_audio1.png");    
            else
                Utility.Lib.LoadImageFromAppDir(imgProtoAudio, "/Skin/images/proto_audio.png");

            imgProtoAudio.Tag = ""; 
        }

        private void btnProtoAudio_Click(object sender, RoutedEventArgs e)                      // 0020-10
        {
            try
            {
                if (imgProtoAudio.Tag == "" || imgProtoAudio.Tag == null)
                {
                    Playsound(Environment.CurrentDirectory + "\\marketing\\"+ControlParams.Params.p_SecondLanguage+"\\_audio\\" +"audio_" + ControlParams.Params.IntegrateKey[4, l_Signature] + ".wav");   // 0106-13

                    if (ControlParams.Params.p_Protopage_selected == 1)                                       // 0106-18
                        Utility.Lib.LoadImageFromAppDir(imgProtoAudio, "/Skin/images/proto_audio_stop1.png");
                    else
                        Utility.Lib.LoadImageFromAppDir(imgProtoAudio, "/Skin/images/proto_audio_stop.png");

                    imgProtoAudio.Tag = "On";
                }
                else
                {
                    if (Audioplayer != null)                                                    // 0100
                        Audioplayer.Stop();

                    if (ControlParams.Params.p_Protopage_selected == 1)                         // 0106-18
                        Utility.Lib.LoadImageFromAppDir(imgProtoAudio, "/Skin/images/proto_audio1.png");
                    else
                        Utility.Lib.LoadImageFromAppDir(imgProtoAudio, "/Skin/images/proto_audio.png");

                    imgProtoAudio.Tag = "";
                }
            }
            catch (Exception ex)                                                                // 0020-11 
            {
                
            }
        }

        private System.Media.SoundPlayer Audioplayer;
        private void Playsound(string soundfile)
        {
            if (!File.Exists(soundfile))
                return;
            try        
            {
                using (Audioplayer = new System.Media.SoundPlayer(soundfile))                   // System.Media.SystemSounds.Beep.Play();
                {
                    Audioplayer.Play();
                }
               
            }
            catch (Exception ex)                                                                // 0020-11 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private WMPLib.WindowsMediaPlayer wplayer;
        private void WMPlaysound(string soundfile)                                              // 0103-09
        {
            if (!File.Exists(soundfile))
                return;
            try
            {
                wplayer = new WMPLib.WindowsMediaPlayer();  
                wplayer.URL =soundfile;
                wplayer.controls.play();
            }
            catch (Exception ex)      
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadTitle(int no)
        {
              Utility.Lib.LoadImageFromAppDir(imgProtoTitle, "\\marketing\\"+ ControlParams.Params.p_SecondLanguage +"\\_header\\" + "header_" + ControlParams.Params.IntegrateKey[0, no] + ".png");  // 0106-13
        }

        private void LoadBanner(int no)
        {
              Utility.Lib.LoadImageFromAppDir(imgBanner, "\\marketing\\"+ControlParams.Params.p_SecondLanguage +"\\_banner\\" + "banner_" + ControlParams.Params.IntegrateKey[1, no] + ".png");  // 0106-13
        }

        private void LoadTips(int no)
        {
            // 002639-03
            if (l_Signature == 1)
                btnProtoPrevious.IsEnabled = false;
            else
                btnProtoPrevious.IsEnabled = true;

            if (l_Signature == ControlParams.Params.p_TreatmentSteps)
                btnProtoNext.IsEnabled = false;
            else
                btnProtoNext.IsEnabled = true;

            Utility.Lib.LoadImageFromAppDir(imgSignature, "\\marketing\\"+ ControlParams.Params.p_SecondLanguage +"\\_tips\\" +  "tips_" + ControlParams.Params.IntegrateKey[2, no] + ".png");  // 0106-13
        }

        private void setObjectEnabled(System.Windows.Controls.Button b , bool settoenabled)
        {
            if(b.IsEnabled != settoenabled) 
                 b.IsEnabled=settoenabled;
        }

        private void setAudioVisiblility(int no)                                                // 0105
        {
            if (ControlParams.Params.IntegrateKey[4, no] == "0")
                imgProtoAudio.Visibility = Visibility.Hidden;
            else
                imgProtoAudio.Visibility = Visibility.Visible;
            
        }

        #endregion

        //==========================
        //==========================

        #region Load Document

        private void Load_AppSettings_Doc_orImage(string name)
        {
            string filename = ConfigurationManager.AppSettings[name];
            if (Path.GetExtension(filename).ToLower() == ".docx")                               // 0103-08
            {
                LoadDocument(filename);
            }
            else if (Path.GetExtension(filename).ToLower() == ".png")                           // 0103-08  0106-16
                Utility.Lib.LoadImageFromAppDir(imgViewer, "\\Docs\\" + filename);
        }

        private void LoadDocument(string docs)
        {
            if (docs == "" || docs == null)
                System.Windows.MessageBox.Show("Invalid document file");

            flowDoc = new FlowDocument();

            Utility.Lib.loadWordML(flowDoc, docs);
            flowDocViewer.Document = flowDoc;
        }

        #endregion

        #region ScrollViewer // Flowdocument

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
            // how much to scroll.
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

        #endregion

        #region Others

        private void SetButtonImage(System.Windows.Controls.Button B, string ImageName, string imgFile)
        {
            ControlTemplate ct = B.Template;

            // Get access of the image tag that is present in the xaml side
            Image TargetImage = (Image)ct.FindName(ImageName, B);

            //0102-39
            Utility.Lib.LoadImageNoLock(TargetImage, imgFile);
        }
        private void btnVolUp_Click(object sender, RoutedEventArgs e)
        {
            if (VolumeSlider.Value + 0.1 < 1)
                VolumeSlider.Value = VolumeSlider.Value + 0.1;
        }

        private void btnVolDown_Click(object sender, RoutedEventArgs e)
        {
            if (VolumeSlider.Value - 0.1 > 0)
                VolumeSlider.Value = VolumeSlider.Value - 0.1;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            WidenObject(150, TimeSpan.FromSeconds(1));
        }

        private void WidenObject(int Width, TimeSpan duration)
        {
            DoubleAnimation animation = new DoubleAnimation(Width, duration);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void btnHideProtocol_Click(object sender, RoutedEventArgs e)
        {
            btnHideContent_Click(null, null);
        }

        private void btnHideVideo_Click(object sender, RoutedEventArgs e)
        {
            btnVideo_Click(null, null);
        }

        private void On_uiVacuumPower(String power, string pulse)
        {
            pgPulseSelect.uiPulseSelect.setPulse(power, pulse);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            c_Volume.SetVolume(65535, 65535);
        }

        public void setHotCold(string hotcoldmode)
        {
            pgHotCold.setHotColdDefaultMode(hotcoldmode);
        }

        private void btnSuggestion_Click(object sender, RoutedEventArgs e)
        {
            cvsSuggest.Visibility = Utility.Lib.rLoadImageFromAppDir(imgSuggest, "\\marketing\\"+ControlParams.Params.p_SecondLanguage +"\\_pattern\\" + "pattern_" + ControlParams.Params.IntegrateKey[3, l_Signature] + ".png") ? Visibility.Visible : Visibility.Hidden; // 0106-13
        }

        private void btnSuggestClose_Click(object sender, RoutedEventArgs e)
        {
            cvsSuggest.Visibility = Visibility.Hidden;
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //long mem3 = GC.GetTotalMemory(false);
            //System.Windows.MessageBox.Show(mem3.ToString());
        }

        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        // 0020-06
        private void Me_Closing(object sender, CancelEventArgs e)
        {
            dispatchertimer.Tick -= new EventHandler(Timer_Tick);
            timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);                            // 0020-08
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            MediaElement1.Close();
            NavBar.setVolume(ControlParams.Params.p_AudioVolume);
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
            MediaElement1.Close();

            // Reset LED setting ...
            pgLED.page_init(true);                                                              // 0100

            TurnOffHardwareIO_ALL();

            if (Audioplayer != null)
                Audioplayer.Stop();                                                             // 0103-03 
        }

        private void Me_Unloaded(object sender, RoutedEventArgs e)                              // 0102-07
        {
            App.BottleChanged -= AppBottleChanged;

            App.Outputs.PropertyChanged -= OutputsOnPropertyChanged;
        }
        #endregion

        private void fmeComponent_Navigating(object sender, NavigatingCancelEventArgs e)        // 0106-06
        {
            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Forward)
            {
                e.Cancel = true;
            }
        }

    } 
  }