using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

// For USB
using System.Management;

// for usb thread
using System.Windows.Threading;
using System.Text;

using System.Windows.Media;

using System.Xml.Serialization;

using System.Linq;
using System.Net.NetworkInformation;

namespace Edge.Tower2.UI
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class Home : INotifyPropertyChanged
    {

        private DateTime dt;
        System.Windows.Threading.DispatcherTimer TimerProgress = new System.Windows.Threading.DispatcherTimer();

        private string DemoDayRemain;                                                           // 0102-06

        #region Loading
        public Home()
        {
            InitializeComponent();
            NavBar.SelectedButton = "Home";

            lblVersion.Content = "Ver 1.5-00115";

            Closing += ExitClosing;

            // add by sww, set parameters
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Search; // State of start up 

            //=============================================
            //Hide cursor
            if (Settings.DisplayCursorOn == false)
                this.Cursor = Cursors.None;
            //=============================================

            //=============================================
            // USB Insert
            //=============================================

            //WqlEventQuery q_creation = new WqlEventQuery();
            //q_creation.EventClassName = "__InstanceCreationEvent";
            //q_creation.WithinInterval = new TimeSpan(0, 0, 2);    //How often do you want to check it? 2Sec.
            //q_creation.Condition = @"TargetInstance ISA 'Win32_DiskDriveToDiskPartition'";
            //ManagementEventWatcher  mwe_creation = new ManagementEventWatcher(q_creation);
            //mwe_creation.EventArrived += new EventArrivedEventHandler(USBEventArrived_Creation);
            //mwe_creation.Start(); // Start listen for events

            //=============================================
            // USB Remove
            //=============================================
            //WqlEventQuery q_deletion = new WqlEventQuery();
            //q_deletion.EventClassName = "__InstanceDeletionEvent";
            //q_deletion.WithinInterval = new TimeSpan(0, 0, 2);    //How often do you want to check it? 2Sec.
            //q_deletion.Condition = @"TargetInstance ISA 'Win32_DiskDriveToDiskPartition'  ";
            //ManagementEventWatcher mwe_deletion = new ManagementEventWatcher(q_deletion);
            //mwe_deletion.EventArrived += new EventArrivedEventHandler(USBEventArrived_Deletion);
            //mwe_deletion.Start(); // Start listen for events

            // Load system params
            LoadParams();

            NavBar.setVolume(ControlParams.Params.p_AudioVolume);

            setButtonColor(null);

            // set warning message hidden
            imgShutDown.Visibility = Visibility.Hidden;

            // 2014 10/24 Credit code expired message display
            dt = getTimeRemain();
            TimerProgress.Tick += new EventHandler(TimerProgress_Tick);
            TimerProgress.Interval = TimeSpan.FromSeconds(1);

            //0102-39
            Utility.Lib.LoadImageNoLock(imgPopup, "\\Skin\\images\\"+ControlParams.Params.p_SecondLanguage +"\\pretreatment-pop-up.png");   // 0106-13

            Utility.Lib.LoadImageNoLock(imgPopupMenu, "\\Skin\\images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");                // 0106-06

            Utility.Lib.LoadImageNoLock(imgShutDown,"\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\creditRemain.png");        // 0106-05  0106-13

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\"  +ControlParams.Params.p_SecondLanguage +"\\"+ App.getTextMessages("hydra-facial-UI-v17-home-1") + ".png");  // 0102-39  // 0106-06  // 0106-14

            DemoDayRemain = "";                                             // 0102-06

            if (ControlParams.Params.p_BottleCountOn)                       // 0102-09
            {
                lblDemo.Visibility = Visibility.Hidden;
                lblDayRemain.Visibility = Visibility.Hidden;
            }
            else
            {
                lblDemo.Visibility = Visibility.Visible;
                lblDayRemain.Visibility = Visibility.Visible;
            }

            lblVersion.Visibility = Visibility.Hidden;                      // 0102-39

            lblDemo.Content=App.getTextMessages("Demo Mode");
            lblDayRemain.Content = App.getTextMessages("Day remain:");

            btnExit1.Visibility = Visibility.Hidden;                        // 0107
          
            App.cs_Events_Language.PropertyChanged += OnPropertyChanged;    // 0106-15
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0106-15
        {
            if (propertyChangedEventArgs.PropertyName == "NewLanguage")
            {
                Utility.Lib.LoadImageNoLock(imgPopup, "\\Skin\\images\\" + ControlParams.Params.p_SecondLanguage + "\\pretreatment-pop-up.png");   // 0106-13

                Utility.Lib.LoadImageNoLock(imgPopupMenu, "\\Skin\\images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");                // 0106-06

                Utility.Lib.LoadImageNoLock(imgShutDown, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\creditRemain.png");        // 0106-05  0106-13

                Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\" + App.getTextMessages("hydra-facial-UI-v17-home-1") + ".png");  // 0102-39  // 0106-06  // 0106-14
  
            }
        }

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }
        #endregion

        #region Main Credit Control, Demo mode control
        public void StartCreditCodeExpiredTimer()
        {
            MachineOn();                            // 0102-38
            UpdateTimeInterval();                   // 0020-05
            TimerProgress.Start();
        }

        public void StopCreditCodeExpiredTimer()    // 0020-05
        {
            MachineOff();                           // 0102-38
            TimerProgress.Stop();
        }
      
        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            UpdateTimeInterval();
        }

        private void UpdateTimeInterval()  
        {
            dt = getTimeRemain();
            TimeSpan timeDiff = dt - DateTime.Now;

            if (timeDiff.TotalSeconds > 60)
            {
                isDemoMode();               // 002639-08-03 , if less than 60 seconds, give up, no need to check the dongle
                                            // Check Dangle exist  or Soft Democode Exist

                DoEvents();                 // 0102-07

                if (ControlParams.Params.p_Downloading == false)        // 0102-26, stop check if system is updating
                {
                    if (!IsInternetActive())                            // update internet icon  // 0102-07
                        App.cs_Events.WifiState = "off";
                    else
                        CheckInternet();                                // 0102-23
                }

            }

            if (timeDiff.TotalDays > Settings.CreditCode_WarningDays)
            {
                if (TimerProgress.Interval != TimeSpan.FromMinutes(1))       // 0102-08
                    TimerProgress.Interval = TimeSpan.FromMinutes(1);        // 0102-08  

                lblShutDown.Content = "";
                imgShutDown.Visibility = Visibility.Collapsed;
                return;
            }

            // 2014 11/07
            // Display warning message
            imgShutDown.Visibility = Visibility.Visible;
            if (timeDiff.TotalDays <= Settings.CreditCode_WarningDays && timeDiff.TotalDays > 1)
            {
                lblShutDown.Content = Convert.ToInt32(Math.Abs(timeDiff.TotalDays)).ToString() + " days";
              
                if (TimerProgress.Interval != TimeSpan.FromMinutes(1))       // 0102-08
                    TimerProgress.Interval = TimeSpan.FromMinutes(1);        // 0102-08  
            }
            else if (timeDiff.TotalHours <= 24 && timeDiff.TotalHours > 1)
            {
                lblShutDown.Content = Convert.ToInt32(Math.Abs(timeDiff.TotalHours)).ToString() +" hours";

                if (TimerProgress.Interval != TimeSpan.FromMinutes(1))
                    TimerProgress.Interval = TimeSpan.FromMinutes(1);
            }
            else if (timeDiff.TotalMinutes <= 60 && timeDiff.TotalMinutes > 1)  // minutes
            {
                lblShutDown.Content = Convert.ToInt32(Math.Abs(timeDiff.TotalMinutes)).ToString() + " minutes";

                if (TimerProgress.Interval != TimeSpan.FromMinutes(1))          // 0020-07
                    TimerProgress.Interval = TimeSpan.FromMinutes(1);
            }
            else if (timeDiff.TotalSeconds <= 60 && timeDiff.TotalSeconds > 0)  // seconds
            {
                lblShutDown.Content = Convert.ToInt32(Math.Abs(timeDiff.TotalSeconds)).ToString() + " seconds";

                if (TimerProgress.Interval != TimeSpan.FromSeconds(1))
                    TimerProgress.Interval = TimeSpan.FromSeconds(1);
            }
            else if (timeDiff.TotalSeconds <= 0)
            {
                TimerProgress.Stop(); 
 
                MachineOff();// 0102-38

                Utility.Lib.CreditLog("Time expired, Init_creditcode ... ");    // 0103-05

                App.Init_creditcode(false);
               
            }
        }

        private void MachineOff() // 0102-38
        {
            App.sp.SerialCmdSend("LF" + "\r"); 
            App.sp.SerialCmdSend("Lf" + "\r"); 
            App.sp.SerialCmdSend("VF" + "\r"); 
        }

        private void MachineOn() // 0102-38
        {
            //App.sp.SerialCmdSend("LN" + "\r");  // 0103-06
            //App.sp.SerialCmdSend("Ln" + "\r");  // 0103-06
        }

        private void CheckInternet()
        {
            Ping myPing = new Ping();
            myPing.PingCompleted += new PingCompletedEventHandler(EH_PingCompletedCallback);
            try
            {
                myPing.SendAsync(Settings.InternetConnection, Settings.InternetTimeout /*3 secs timeout*/, new byte[32], new PingOptions(64, true));
            }
            catch
            {
                App.cs_Events.WifiState = "off";
                return;
            }
        }

        private void EH_PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            try
            {
                if (e.Reply.Status == IPStatus.Success)
                {
                    SetWifi("on");
                }
                else
                {
                    SetWifi("off");
                }
            }
            catch (Exception ex)
            {
                SetWifi("off");
            }

            if ((IDisposable)sender != null)
                ((IDisposable)sender).Dispose();
        }

        private void SetWifi(string state)              // 0102-23
        {
            Dispatcher.Invoke((Action)(() =>
            {
                App.cs_Events.WifiState = state;
            }));
        }

        private DateTime getTimeRemain()
        {
            CreditCodeState cs;

            cs = CreditCodeState.LoadState(false);      // 2014 11/07
            DateTime d = Convert.ToDateTime(cs.Kosciusko);
            return d;
        }

        private void imgShutDown_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            TimerProgress.Stop();                       // ver 002633

            Utility.Lib.CreditLog("Timer stop in home page, call Init_creditcode ... "); // 0103-05
  
            App.Init_creditcode(true);                  // ver 00263
        }
        #endregion

        #region Check Dongle and DemoCode control, a vaild democode is allow to bypass the RFID bottle control 
        public void isDemoMode()
        {
            // 002639-08-03
            if (Dongle_Presented())                                                         // Verify dongle exist and date is not expired
            {
                ControlParams.Params.p_BottleCountOn = false;
                ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDemo.Visibility = Visibility.Visible;
                ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Visibility = Visibility.Visible;
                ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Content = App.getTextMessages("Remain") + " " + DemoDayRemain; // 0103-08

                lblDemo.Visibility = Visibility.Visible;                                     // 0102-09
                lblDayRemain.Visibility = Visibility.Visible;
                lblDayRemain.Content = App.getTextMessages("Remain") + " " + DemoDayRemain;  // 0103-08
            }
            else
            {
                if (isDemoCodeValid())                                                      // Verify DemoCode exist 
                {
                    ControlParams.Params.p_BottleCountOn = false;                           // disable bottle counting
                    ControlParams.Params.p_DemoMode = true;
                    ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDemo.Visibility = Visibility.Visible;
                    ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Visibility = Visibility.Visible;
                    ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Content = "Remain " + DemoDayRemain;

                    lblDemo.Visibility = Visibility.Visible;
                    lblDayRemain.Visibility = Visibility.Visible;
                    lblDayRemain.Content = "Remain " + DemoDayRemain;
                }
                else
                {
                    ControlParams.Params.p_BottleCountOn = true;
                    ControlParams.Params.p_DemoMode = false;
                    ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDemo.Visibility = Visibility.Hidden;
                    ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Visibility = Visibility.Hidden;

                    lblDemo.Visibility = Visibility.Hidden;
                    lblDayRemain.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion

        #region USB Dongle Control
        private bool Dongle_Presented()                                                     // Verify dongle exist and date not expired
        {
            int numDevices = 0;
            int ChipID = 0;
            int i = 0;
            string SerialBuffer = "";
            string chipid, serialno;
            
            FTChipID.ChipID.GetNumDevices(ref numDevices);
            if (numDevices <= 0)
                return false;

            FTChipID.ChipID.GetDeviceChipID(0, ref ChipID);
            chipid = ChipID.ToString("X");
            if (chipid == "")
                return false;

            FTDI F = new FTDI();
            FTDI.FT_STATUS r;
            if (F.OpenByIndex(0) != 0)
                return false;

            if (F.GetSerialNumber(out SerialBuffer) != 0)
            {
                r = F.Close();
                return false;
            }

            serialno = SerialBuffer.ToString();

            UInt32 numBytes = 0;
            byte[] DataBuffer = new byte[100];
            string strBuffer;

            if (F.EEReadUserArea(DataBuffer, ref numBytes) == 0)
            {
                strBuffer = System.Text.Encoding.UTF8.GetString(DataBuffer).Trim();
                strBuffer = strBuffer.Replace("\0", string.Empty);
            }
            else
            {
                r = F.Close();
                return false;
            }

            if (F.Close() != 0)
                return false;
            
            return Dongle_VerifyCode(chipid, serialno, strBuffer);
        }

        private bool Dongle_VerifyCode(string chipid, string serialnumber, string code)
        {
            try
            {
                DemoDayRemain = ""; // 0102-06

                if (chipid.Length != 8 || serialnumber.Length != 8 || code.Length != 40)
                    return false;

                ControlParams.Params.p_dongle_Created = code.Substring(0, 14);
                ControlParams.Params.p_dongle_Expired = code.Substring(15, 14);

                if (code.Length != 40)
                    return false;
                else
                {
                    string s = code.Substring(32, 8);
                  
                    if (code.Substring(32, 8) != Dongle_CheckCode(chipid + serialnumber + code.Substring(0, 32)))
                        return false;
                }

                DateTime _dt;

                _dt = Convert.ToDateTime("20" + ControlParams.Params.p_dongle_Expired + ":00");
                if (DateTime.Now > _dt)
                    return false;
                else
                {
                    _dt = Convert.ToDateTime("20" + ControlParams.Params.p_dongle_Created + ":00");
                    if (_dt > DateTime.Now)
                        return false;
                    else
                    {
                        TimeSpan diff = _dt - DateTime.Now;
                        DemoDayRemain = diff.Days.ToString() + " "+App.getTextMessages("days")+" " +
                            diff.Hours.ToString() + " " + App.getTextMessages("hours");  // 0102-07

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string Dongle_CheckCode(string s)
        {
            String hash = String.Empty;

            byte[] fs = System.Text.Encoding.ASCII.GetBytes(s);

            c_Dongle n_crc32 = new c_Dongle();
            foreach (byte b in n_crc32.ComputeChecksumBytes(fs))
                hash += b.ToString("x2").ToLower();

            string data = hash.ToString();

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region DemoCode without XML  not done yet
        public void SaveDemoCode(string newCode)  // 0102-06
        {
            try
            {
                //using (StreamWriter sw = File.AppendText(Environment.CurrentDirectory + "\\disk\\democode.dat"))
                using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\disk\\democode.dat", true, Encoding.ASCII))  // 0106-09
                {
                    sw.WriteLine(newCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data save failed!");
            }
        }
        #endregion

        #region Soft democode control 
        private void CheckDemoCodeFile()                                                        // not in use
        {
            c_XmlDemoCode P = null;
            string path = Environment.CurrentDirectory + "\\disk\\dmcode.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(c_XmlDemoCode));

            StreamReader reader = new StreamReader(path);                                       // ???  for T2
            P = (c_XmlDemoCode)serializer.Deserialize(reader);
            reader.Close();

            string s = P.items[0].Code ;
            foreach (Items P1 in P.items)
            {
                s = P1.Date;
            }
        }
        
        private bool isDemoCodeValid()                      // 0101-08  not in use
        {
            return DamoCodeExist();
        }

        private bool DamoCodeExist()
        {
            try
            {
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\disk\\dmcode.dat", Encoding.ASCII))  // 0106-09
                {
                    string line;
                    string dateExpired;
                    if ((line = file.ReadLine()) != null)
                    {
                        // Skip the first one
                        if ((dateExpired = file.ReadLine()) != null)
                        {
                            DateTime dt = Convert.ToDateTime(dateExpired);
                            if (dt > DateTime.Now)
                            {
                                TimeSpan diff = dt - DateTime.Now;

                                DemoDayRemain = diff.Days.ToString() + " days " + diff.Hours.ToString() + " hours " + diff.Minutes.ToString() + " minutes"; ;  // 0102-07
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Button Control
        private void ExitClosing(object sender, CancelEventArgs e)
        {
            App.Outputs.ClearAll();
            Task.Delay(250).ContinueWith((task => Environment.Exit(0)));
        }
       
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void HotColdClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void LEDsClick(object sender, RoutedEventArgs e)
        {
        
        }

        private void VideosClick(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfVideo);
        }

        private void UsageReportClick(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.UsageReport;             // "UsageReport";
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.UsageReport;             // "UsageReport";
            App.Go(Mode.UsageReport);
        }


        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            Protocol();
        }
        
        private void btn2_Click(object sender, RoutedEventArgs e)                               // 0102-37
        {
            return;                                                                             // 00115a1-0005

            ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.Printing;                // "Printing";
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.Printing;                // "Printing";
            App.Go(Mode.Printer);
        }

        private void btn3_Click(object sender, RoutedEventArgs e)                               // 0102-37
        {
            LoadMainfolder("mt_mainfolder.txt");                                                // 0020-12  
            LoadSubfolder("mt_subfolder_v2.txt");                                               // 0020-12  

            // Modalities
            ControlParams.Params.p_ProtoMode = true;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;           // "Proto";
            ControlParams.Params.p_SelectDir = "MT\\";                                          // 0100
          
            ((wpfModalities)App._mainWindows[Mode.wpfModalities]).setMenuPage(Menu.Main);       // set to main menu  0102-39
            App.Go(Mode.wpfModalities);
        }
     
        private void btn4_Click(object sender, RoutedEventArgs e)                               // 0102-39 Camera
        {
            return;                                                                             // 00115a1-0005

            ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.Camera;                  // "Camera";
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.Camera;                  // "Camera";
            App.Go(Mode.Photo_Customer_Type2);
        }

        private void btn5_Click(object sender, RoutedEventArgs e)                               // 0102-39  115a1-0001
        {

            LoadMainfolder("clean_mainfolder.txt");
            LoadSubfolder("clean_subfolder_v2.txt");

            ControlParams.Params.p_InCleaningPage = true;                                       // 00115a1-0006 it is in cleanup page

            // Modalities
            ControlParams.Params.p_ProtoMode = true;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;           // "Proto";
            ControlParams.Params.p_SelectDir = "Clean\\";                                       // 0100

            ((wpfModalities)App._mainWindows[Mode.wpfModalities]).setMenuPage(Menu.Main);       // set to main menu
            App.Go(Mode.wpfModalities);

            return;
            // ===========================================================================================

            ControlParams.Params.p_Protopage_selected = 0;                                      // 0106-18

            ControlParams.Params.p_InCleaningPage = true;                                       // 0103-10

            LoadMainfolder("clean_mainfolder.txt");                                             // 0020-12
            LoadSubfolder("clean_subfolder_v2.txt");                                            // 0020-12
         
            // Modalities
            ControlParams.Params.p_ProtoMode = true;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;           // "Proto";
            ControlParams.Params.p_SelectDir = "CLEAN\\";                                       // 0100

            ((wpfModalities)App._mainWindows[Mode.wpfModalities]).setMenuPage(Menu.Main);       // set to main menu
           
            int id = 5; // cleanup id
            ControlParams.Params.p_ProtoSelect = id;
            
            ((wpfModalities)App._mainWindows[Mode.wpfModalities]).LoadOperationMode(ControlParams.Params.p_Mainfolder[id,0] + ".txt");

            // ProtoPage_init
            ControlParams.Params.p_ProtoOpt = 0;                                                // read first array of IntegrateMode[]

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).l_Signature = 1;
            ControlParams.Params.p_TreatmentSteps = ControlParams.Params.IntegrateMode[0, 0];
            ControlParams.Params.p_control_mode = ControlParams.Params.IntegrateMode[0, 1];

            //continue button
            App.Go(Mode.HydraFacial);
        
            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).protoMode_init(false);
        }

        private void btn6_Click(object sender, RoutedEventArgs e)                               // 0102-37
        {
            cvsPopup.Visibility = Visibility.Visible;                                           // 00115a1-0005
            return;                                                                             // 00115a1-0005

            ControlParams.Params.p_last_QS_Mode = ControlParams.e_Mode.None;                    //  for Video
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.None;                    //  for Video
            App.Go(Mode.VPlayer);
        }

        private void CurrentClientClick(object sender, RoutedEventArgs e)
        {

        }
        
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfBackup);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTreatment_Click(object sender, RoutedEventArgs e)
        {
            // App.Go(Mode.wpfVideo);                                                           // 00115a1-0006
        }

        private void btnPatientInfo_Click(object sender, RoutedEventArgs e)
        {
            return;                                                                             // 00115a1-0006

            cvsPopup.Visibility = Visibility.Visible;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            cvsPopup.Visibility = Visibility.Hidden;
        }

        private void setButtonColor(System.Windows.Controls.Button B)
        {
            // T2
            // not in use
            btn1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8"));
            btn2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8")); // 0102-36
            btn3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8"));
            btn4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8"));
            btn5.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8"));
            btn6.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8"));

            // Highlight the button
            //if (B !=null)
            //B.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff0094ab"));
        }

        private void Protocol()                                                                 // 0102-37
        {
            // ver 00263 0102-35
            ControlParams.Params.p_ProtoMode = true;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;           // "Proto";
            ControlParams.Params.p_SelectDir = "PC\\";                                          // 0100

            LoadMainfolder("proto_mainfolder.txt");                                             // 0020-12
            LoadSubfolder("proto_subfolder_v2.txt");                                            // 0020-12

            ((wpfProto)App._mainWindows[Mode.wpfProto]).setMenuPage(ControlParams.e_Proto.Option);
            App.Go(Mode.wpfProto);
        }

        private void Modalities()                                                               // 0102-37
        {
            LoadMainfolder("mt_mainfolder.txt");                                                // 0020-12
            LoadSubfolder("mt_subfolder_v2.txt");                                               // 0020-12

            // Modalities
            ControlParams.Params.p_ProtoMode = true;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;           // "Proto";
            ControlParams.Params.p_SelectDir = "MT\\";                                          // 0100

            ((wpfModalities)App._mainWindows[Mode.wpfModalities]).setMenuPage(Menu.Main);       // set to main menu
            App.Go(Mode.wpfModalities);
        }

        private void QuickStart()                                                               // 0102-37
        {
            // 2015 01/16
            ControlParams.Params.p_ProtoMode = false;

            ControlParams.Params.p_control_mode = ControlParams.e_Mode.VortexFusion;            // "Vortex-Fusion"; //2014 09/12
            ControlParams.Params.p_hydrafacialLoaded = false;
            App.Go(Mode.HydraFacial);
            setButtonColor(btn1);
        }
        #endregion

        #region Audio Control
        private void LoadParams()
        {
            try
            {
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\..\\UsageLogs\\params.dat", Encoding.ASCII))
                {
                    string line;
                    line = file.ReadLine();

                    if (!String.IsNullOrEmpty(line))   // 0106-09
                    {
                        if (line.Substring(0, 5) == "Audio")
                            ControlParams.Params.p_AudioVolume = ParseData(line);
                        else
                            ControlParams.Params.p_AudioVolume = 0.0;
                    }
                    else
                        ControlParams.Params.p_AudioVolume = 0.0;

                }
            }
            catch (Exception e)
            {
                 MessageBox.Show(e.Message);
            }
        }

        private double ParseData(string s)
        {
            if (s != null)
            {
                string[] fields = s.Split(';');
                if (fields[1] != null)
                {
                    return Convert.ToDouble(fields[1]);
                }
                else
                    return 0.0;
            }
            else
                return 0.0;
        }

        private void ParseData(string s, string data)
        {
            string[] fields = s.Split(';');
            if (fields[1] != null)
                data =fields[1];
        }

        #endregion
     
        #region Load files for Protocol
        private void LoadSubfolder(string filename)                                             // 0020-12
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\"+Settings.Operation_Mode+"\\" + filename)) // 0106-07
            {
                MessageBox.Show(filename + " Operation file not found!");
                return;
            }

            string line;
            using (var sr = new StreamReader(Environment.CurrentDirectory + "\\" + Settings.Operation_Mode + "\\" + filename, Encoding.ASCII))   // 0106-07  0106-09
            {
                int iCountLine = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;

                    string str, str1;
                    if (line.IndexOf(";") > 0)
                        str1 = line.Substring(0,line.IndexOf(";") );                            // 0102-36
                    else
                        str1 = line;

                    if (str1.IndexOf(',') == -1)                                                // 0106-17 Invalid string
                        continue;

                    str1 = str1.Replace("\t", "");
                    str = str1.Replace(" ", "");

                    var answers = str.Split(',');
                    for (int iCountAnswer = 0; iCountAnswer <= 10; iCountAnswer++)              // 0106-17
                    {
                        ControlParams.Params.p_Subfolder[iCountLine, iCountAnswer] = answers[iCountAnswer];
                    }

                    iCountLine++;
                }
            }
        }

        private void LoadMainfolder(string filename)                                            // 0020-12 for proto and modality
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\"+Settings.Operation_Mode+"\\" + filename))  // 0106-07
            {
                MessageBox.Show("Operation key file not found!");
                return;
            }

            string line;
            int totalline = 0;                                                                  // 0106-17
            using (var sr = new StreamReader(Environment.CurrentDirectory + "\\" + Settings.Operation_Mode + "\\" + filename, Encoding.ASCII))  // 0106-07  0106-09
            {
                for (int iContinue = 0; iContinue <= 3; iContinue++)                            // only read 3 pages
                {
                    if ((line = sr.ReadLine()) != null)
                    {
                        if (line == "")
                            break;                                                              // 0106-17

                        string str, str1;
                        if (line.IndexOf(";") > 0)
                            str1 = line.Substring(0, line.IndexOf(";"));                        // 0102-36
                        else
                            str1 = line;

                        if (str1.IndexOf(',') == -1)                                            // 0106-17 Invalid string
                            continue;


                        str1 = str1.Replace("\t", "");
                        str = str1.Replace(" ", "");

                        var answers = str.Split(',');
                        
                        if (answers[0] == "0")                                                  // 0106-17 set the limit, if the first column is nothing (0), then end of reading
                            break;
                      
                        for (int iCountAnswer = 0; iCountAnswer <= 14; iCountAnswer++)          // 0106-17
                        {
                            ControlParams.Params.p_Mainfolder[iCountAnswer, iContinue] = answers[iCountAnswer];
                        }

                        totalline++;                                                            // 0106-17

                    }
                }

                ControlParams.Params.p_MainfolderMax = totalline;                               // 0106-17 Keep total main page
            }
        }
        #endregion

        #region Others
        private void Me_Activated(object sender, EventArgs e)
        {
            if (ControlParams.Params.p_LoginRole == "Administrator")
                btnSettings.IsEnabled = true;
            else
                btnSettings.IsEnabled = false;

            NavBar.sldVolume.Value = ControlParams.Params.p_AudioVolume;    // must keep in here

            isDemoMode(); // 002639-08-03
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;    // must keep in here 
            NavBar.cvsVolume.Tag = "";
        }

        private void Me_Closing(object sender, CancelEventArgs e)
        {
            App.sp.SerialCmdSend("LF" + "\r"); // 0102-3638
            App.sp.SerialCmdSend("Lf" + "\r"); // 0102-3638

            TimerProgress.Tick -= new EventHandler(TimerProgress_Tick);
            base.OnClosed(e);
        }

        internal void USBEventArrived_Creation(object sender, EventArrivedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                this.btnCode.Content = "Renew";
                this.btnCode.IsEnabled = true;
            }));
        }

        internal void USBEventArrived_Deletion(object sender, EventArrivedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                this.btnCode.Content = "";
                this.btnCode.IsEnabled = false;
            }));
        }

        private void UIElement_OnTouchDown(object sender, TouchEventArgs e)
        {
            var p = Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + Path.DirectorySeparatorChar + "osk.exe");

            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.WaitForExit();
        }

        private static bool IsInternetActive()   // 0102-07
        {
            // Check related internet adapters
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                return (from face in interfaces
                        where face.OperationalStatus == OperationalStatus.Up
                        where (face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && (face.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        select face.GetIPv4Statistics()).Any(statistics => (statistics.BytesReceived > 0) && (statistics.BytesSent > 0));
            }

            return false;
        }
       
        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void btnExit1_Click(object sender, RoutedEventArgs e) // 0102-35
        {
            App.SaveBottleDataFile();

            App.Outputs.ClearAll();

            Task.Delay(250).ContinueWith((task => Environment.Exit(0)));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfShutdown);
        }
        #endregion

        private void Me_Unloaded(object sender, RoutedEventArgs e)
        {
            App.cs_Events_Language.PropertyChanged -= OnPropertyChanged;                        // 0106-18
        }
    }
}