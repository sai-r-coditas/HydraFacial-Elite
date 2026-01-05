using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

using System.IO;

using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml;
using Edge.EdgeObject;
using Edge.IOBoard;
using Edge.Tower2.UI.Client.ViewModel;
using Edge.Tower2.UI.Printing;

// For Thread
using System.Threading;

// Add by sww
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

using System.Text;  // 0106-09

using System.Reflection;  // 0106-12

namespace Edge.Tower2.UI
{
    public class BottleChangedEventArgs : EventArgs
    {
        public int BottleIndex = -1;
        public Station Station;
    }

    #region declare
    public enum Mode
    {
        Home,
        HydraFacial,
        wpfStatus,
        ImageCapture,
        Videos,
        Printer,
        UsageReport,
        Photo_Comparison,
        Photo_Customer_Info,
        Photo_Customer_Search,
        Photo_Customer_Type2,
        wpfVideo,
        wpfBackup,
        //-wpfCreditCode,
        wpfUser,
        wpfLogin,
        wpfInfo,
        wpfProto,
        wpfModalities,
        wpfSettings,
        VPlayer,
        wpfProductMenu,
        //wpfWifi,
        wpfDemo,
        wpfUpdate,
        wpfShutdown,
        wpfSystem,
        wpfLanguage
    }

    public enum Menu  // 0102-38
    {
        Main,
        Second
    }
    #endregion

    public partial class App
    {
        #region Delegates

        public delegate void BottleChangedEventHandler(object sender, BottleChangedEventArgs ea);
        public event BottleChangedEventHandler BottleChanged;
        
        #endregion

        #region local declare
        private readonly BoardManager _boardManager = new BoardManager();                       // 0102-12
        private readonly Outputs _outputs;
        private readonly string[] _previousFullBottleData = new string[Constants.NUMBER_OF_STATIONS];

        public Mode Mode;

        // for H_AgeRefinement, .. Refinement, ...
        public string HydrafacialSelect { get; set; }
        public string Profile_State { get; set; }

        private readonly cs_Events _cs_Events = new cs_Events();                                // 0102-01
        public cs_Events cs_Events
        {
            get { return _cs_Events; }
        }

        private readonly cs_Events_User _cs_Events_User = new cs_Events_User();                 // 0102-02
        public cs_Events_User cs_Events_User
        {
            get { return _cs_Events_User; }
        }

        private readonly cs_Events_Language _cs_Events_Language = new cs_Events_Language();     // 0106-15
        public cs_Events_Language cs_Events_Language
        {
            get { return _cs_Events_Language; }
        }
        #endregion

        #region init
        public App()
        {
            ControlParams.Params.p_SystemLoading = true;
            
            ControlParams.Params.p_SystemStartUp = true;                                        // for bottle counter

            ControlParams.Params.p_BottleCountOn = true;                                        // 0020-03
            
            ControlParams.Params.p_dongle_Presented = false;                                    // for dongle 0020-03

            ControlParams.Params.p_Downloading = false;                                         // 0102-26, for downloading of message control

            ControlParams.Params.Photos_Default = Environment.CurrentDirectory + "\\..\\edgephotos\\default"; // 0102-38 0103-05

            ControlParams.Params.Photos_Backup = "edgephotos\\backup";                          // 0103-01

            ControlParams.Params.p_CleanupRequired = true;                                     // 0103-10

            ControlParams.Params.p_InCleaningPage = false;                                      // 0103-10

            ControlParams.Params.p_MainfolderMax = 0;                                           // 0106-17

            ControlParams.Params.p_Protopage_selected = 0;                                      // 0106-18

            string SavedLanguage;                                                               // 0106-11
            SavedLanguage = GetSavedLanguage();

            if(SavedLanguage == "")
                ControlParams.Params.p_SecondLanguage = "en-US";                                // 0106-10  English is the default language
            else
            {
                if (CheckInputLanguageReady(SavedLanguage))
                    ControlParams.Params.p_SecondLanguage = SavedLanguage;
                else
                    ControlParams.Params.p_SecondLanguage = "en-US";
            }

            // If directory not exist, set back to english version
            if (!Directory.Exists(Environment.CurrentDirectory + "\\skin\\images\\" + ControlParams.Params.p_SecondLanguage))   // 0106-17
                ControlParams.Params.p_SecondLanguage = "en-US";

            // Remove the previouse file first
            if (File.Exists(Environment.CurrentDirectory + "\\missingfiles.txt"))               // 0106-13
                File.Delete(Environment.CurrentDirectory + "\\missingfiles.txt");

            if (LoadFileList_XML("\\Skin\\images\\","filelist.xml") || LoadFileList_XML("\\Skin\\images\\",ControlParams.Params.p_SecondLanguage+"\\","filelist_lng.xml"))   // 0106-09   // 0106-13  // 0106-17
            {
                MessageBox.Show("Missing image files!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                
                // no stop, continue to load ...
            }

            if (CheckMarketingFiles())
            {
                MessageBox.Show("Missing image files!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (!DB.DBCommend.CheckDBAccessibility())                                           // 0106-12
            {
                MessageBox.Show("Unable to access database!");
                return;
            }

            // 2014 11/30
            var r = DB_Product.Load_ProductInfo();                                              // Load product information from DB

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            
            LoadLooseXaml();

            if(Settings.MD_SPA_Mode == "MD")                                                    // 0106-06
                LoadMenuMenuXaml("skin\\mode_md.xaml"); 
            else
                LoadMenuMenuXaml("skin\\mode_spa.xaml");

            LoadMenuMenuXaml(Settings.Operation_Mode+"\\mainmenu.xaml");                        // 0106-07

            LoadLanguageXaml("Skin\\Languages\\rsLanguage-" + ControlParams.Params.p_SecondLanguage + ".xaml"); // 0103-10 0106-09  0106-12

            wpfyesno = new wpfYesNo();                                                          // 00115a1-0005

            wpfyesno.init();                                                                    // 0106-06   init the  page 

            ClientViewModel = new ClientViewModel();

            //-InitializeHardware();                                                            // Open Port

            _outputs = new Outputs(_boardManager) { TargetTemperature = Settings.HotColdTemperatureWarmest };

            Outputs.LogHeader("", "");

            // 2014 12/08
            Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();
             
            _mainWindows = new Dictionary<Mode, Window>
            {
                {Mode.Home, new Home()},
              
                {Mode.HydraFacial, new HydraFacial()},
                {Mode.wpfStatus, new wpfStatus()},
                {Mode.ImageCapture, new PhotoCapture.PhotoCapture()},
                {Mode.Videos, new Videos.Videos()},
                {Mode.Printer, new Printer()},
                {Mode.UsageReport, new UsageReport.UsageReport()},
                {Mode.Photo_Comparison, new Photo_Comparison()},
                {Mode.Photo_Customer_Info, new Photo_Customer_Info()},
                {Mode.Photo_Customer_Search, new Photo_Customer_Search()},
                {Mode.Photo_Customer_Type2, new Photo_Customer_Type2()},
                {Mode.wpfVideo, new wpfVideo()},
                {Mode.wpfBackup, new wpfBackup()},
                {Mode.wpfUser, new wpfUser()},
                {Mode.wpfLogin, new wpfLogin()},
                {Mode.wpfInfo, new wpfInfo()},
                {Mode.wpfProto, new wpfProto()},
                {Mode.wpfModalities, new wpfModalities()},
                {Mode.wpfSettings, new wpfSettings()},
                {Mode.VPlayer, new VPlayer()},
                {Mode.wpfProductMenu, new wpfProductMenu()},
                //{Mode.wpfWifi, new wpfWifi()},
                {Mode.wpfDemo, new wpfDemo()},
                {Mode.wpfUpdate, new wpfUpdate()},
                {Mode.wpfShutdown, new wpfShutdown()},
                {Mode.wpfSystem, new wpfSystem()},
                {Mode.wpfLanguage, new wpfLanguage()}
                
            };

            ControlParams.Params.p_SystemLoading = false;                                       // sww added

            Current.MainWindow = _mainWindows[Mode.wpfLogin];                                   // 2014 11/14
 
            sp = new wpfPort();                                                                 //0102-36
            sp.Connect_Open();
            DoEvents();

            ////- Hardware control
            //sp.SerialCmdSend("LN" + "\r"); // 0102-3638 turn on Blue LED
            //System.Threading.Thread.Sleep(3000); // 0103-06
            //sp.SerialCmdSend("Ln" + "\r"); // 0102-3638 turn on Red LED

            Init_creditcode(false);                                                             // ver 0020-03
          
            newWindowThread.Abort();                                                            // 2014 12/08

            System.Windows.Threading.Dispatcher.Run();
            
        }

        public wpfPort sp;                                                                      // 0102-36

        public wpfYesNo wpfyesno;                                                               // 0102-20, 00115a1-0005

        //  Ver 002633
        public wpfCreditCode cc;
        public void Init_creditcode(bool _entrymode)
        {
            Utility.Lib.CreditLog("Init_Creditcode-( entermode) ->" + _entrymode.ToString());   // 0103-10

            if (cc == null)                                                                     // sww 0102-32
                cc = new wpfCreditCode();

            cc.EntryMode = _entrymode;
            cc.Show();
            cc.page_init();

            cc.Activate();                                                                      // sww 0102-32
          
        }

        private void ThreadStartingPoint()                                                      // 2014 12/08
        {
            wpfSplash splash = new wpfSplash();

            splash.Show();

            System.Windows.Threading.Dispatcher.Run();
        }
      
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        public BoardManager BoardManager
        {
            get { return _boardManager; }
        }

        public Outputs Outputs
        {
            get { return _outputs; }
        }

        [STAThread]
        public static void Main()
        {
            Process thisProc = Process.GetCurrentProcess();                                     // preventing load multiple time
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                MessageBox.Show("Application has been up running!");
                Process.GetCurrentProcess().Kill();
                return;
            }
       
             var app = new App();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // LogCommunication
            var errorMessage = string.Format("{2}Unexpected problem encountered:{2}{0}{2}{1}",
                ((Exception)e.ExceptionObject).Message,
                ((Exception)e.ExceptionObject).StackTrace,
                Environment.NewLine + Environment.NewLine);

            Utility.Lib.SaveErrorLog(errorMessage);                                             // 0103-09

            MessageBox.Show(errorMessage);

        }

        private bool CheckMarketingFiles()                                                      // 0106-13 0106-17
        {
            if (LoadFileList_XML("\\marketing\\" ,ControlParams.Params.p_SecondLanguage, "Marketing.xml"))
                return true;
            else
                return false;
        }

        public bool LoadFileList_XML(string dir,string filename)                                // 0106-09  // 0106-13
        {
            bool missingfile = false;

            try
            {
                string loaddir = Environment.CurrentDirectory + dir;

                using (XmlReader reader = XmlReader.Create(loaddir +filename))                  // UTF-8 format declared in file
                {
                    string line;
                   
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name.ToString() == "image")
                            {
                                line = reader.ReadString();
                                if (!File.Exists(loaddir + line))
                                {
                                    File.AppendAllText(Environment.CurrentDirectory + "\\missingfiles.txt", dir+ line + Environment.NewLine, Encoding.ASCII);   // 0106-09
                                    missingfile = true;
                                }
                            }
                        }

                    }
                }
                return missingfile;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load filelist failed!");
                return true;     // has missing files
            }
        }

        public bool LoadFileList_XML(string dir,string Lng, string filename)                    // 0106-17
        {
            bool missingfile = false;

            try
            {
                string loaddir = Environment.CurrentDirectory + dir;

                using (XmlReader reader = XmlReader.Create(loaddir + filename))                 // UTF-8 format declared in file
                {
                    string line;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name.ToString() == "image")
                            {
                                line = reader.ReadString();
                                if (!File.Exists(loaddir + Lng+ line))
                                {
                                    File.AppendAllText(Environment.CurrentDirectory + "\\missingfiles.txt", dir + line + Environment.NewLine, Encoding.ASCII);   // 0106-09
                                    missingfile = true;
                                }
                            }
                        }

                    }
                }
                return missingfile;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load filelist failed!");
                return true;                                                                    // has missing files
            }
        }

        private void InitializeHardware()
        {
            _boardManager.PortName = ConfigurationManager.AppSettings["COMPORT"];
 
            _boardManager.BaudRate = Convert.ToInt32(ConfigurationManager.AppSettings["BAUDRATE"]);

            _boardManager.DataReceived += BoardManagerDataReceived;
            try
            {
                _boardManager.Open();

                _boardManager.SendRequestSetBoardIOState();

                _boardManager.Start_Timer();                                                    // 2014 /12/04
            }
            catch (Exception e)
            {
                ShowErrorMessage(e.Message);                                                    // 0020-10
            }
        }

        void ShowErrorMessage(string err)                                                       // 0020-10
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                MessageBox.Show(err);
            }));
        }

        private static void LoadLooseXaml()                                                     // Load resource
        {
            var dictionaries = Current.Resources.MergedDictionaries;
            var skinDirectory = Path.Combine(Environment.CurrentDirectory, "Skin");

            if (!Directory.Exists(skinDirectory))
                Directory.CreateDirectory(skinDirectory);

            foreach (var filepath in Directory.GetFiles(skinDirectory))
            {
                try
                {
                    var reader = XmlReader.Create(filepath);
                    var resourceDictionary = (ResourceDictionary)XamlReader.Load(reader);
                    dictionaries.Add(resourceDictionary);
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("The file: {0} could not be loaded: \n{1}", Path.GetFileName(filepath),
                        e.Message));
                }
            }
        }

        private static void LoadLanguageXaml(string filename)                                   // Load resource 0103-10 0106-09 0106-12
        {
            var dictionaries = Current.Resources.MergedDictionaries;
            var filepath = Path.Combine(Environment.CurrentDirectory, filename);

            if (!File.Exists(filepath))
            {
                MessageBox.Show("File "+filename+" not found!");
            }
          
            try
            {
                var reader = XmlReader.Create(filepath);
                var resourceDictionary = (ResourceDictionary)XamlReader.Load(reader);

                dictionaries.Add(resourceDictionary);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("The file: {0} could not be loaded: \n{1}", Path.GetFileName(filepath),
                    e.Message));
            }
        }

        private static void UpdateLanguageXaml(string filename)                                 // 0106-12
        {
            var dictionaries = Current.Resources.MergedDictionaries;
            var filepath = Path.Combine(Environment.CurrentDirectory, filename);

            if (!File.Exists(filepath))
            {
                MessageBox.Show("File not found!");
            }

            try
            {
                var reader = XmlReader.Create(filepath);
                var resourceDictionary = (ResourceDictionary)XamlReader.Load(reader);

                PropertyInfo prop = typeof (ResourceDictionary).GetProperty("Keys");
                object[] keys= prop.GetValue(resourceDictionary,null) as object[];
                foreach (object o in keys)
                { 
                    MessageBox.Show(resourceDictionary[o.ToString()].ToString());
                }

                //foreach (XmlNode node in resourceDictionary.)

                //dictionaries.Add(resourceDictionary);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("The file: {0} could not be loaded: \n{1}", Path.GetFileName(filepath),
                    e.Message));
            }
        }

        private static void LoadMenuMenuXaml(string filename)                                   // Load resource 0106-06
        {
            var dictionaries = Current.Resources.MergedDictionaries;
            var filepath = Path.Combine(Environment.CurrentDirectory, filename);

            if (!File.Exists(filepath))
            {
                MessageBox.Show("File "+filename +" not found!");
            }

            try
            {
                var reader = XmlReader.Create(filepath);
                var resourceDictionary = (ResourceDictionary)XamlReader.Load(reader);
                dictionaries.Add(resourceDictionary);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("The file: {0} could not be loaded: \n{1}", Path.GetFileName(filepath),
                    e.Message));
            }

        }

        public readonly Dictionary<Mode, Window> _mainWindows;                                  // 2014 10/10 

        internal IEnumerable<Window> MainWindows
        {
            get { return _mainWindows.Values; }
        }

        public Window currentWindow;

        public ClientViewModel ClientViewModel { get; private set; }
        #endregion

        #region Go   -- windows control
        public void Go(Mode mode)
        {
            Mode = mode;
           
            try
            {
                var targetWindow = _mainWindows[mode];

                //-if (mode == Mode.ImageCapture)                                               // 0103
                //{
                //    targetWindow.Show();
                //    targetWindow.Activate();
                //}
                //else
                //{
                    targetWindow.Show();
                    //targetWindow.Opacity = 1;
                    targetWindow.Activate();
                //}

                if (targetWindow == _mainWindows[Mode.ImageCapture])
                {
                   //-
                }
                else if (targetWindow == _mainWindows[Mode.HydraFacial])
                {
                    ((HydraFacial)targetWindow).page_init(true);                                // 0103-10
                }
                else if (targetWindow == _mainWindows[Mode.Photo_Customer_Search])
                {
                    ((Photo_Customer_Search)targetWindow).page_init();
                }
                else if (targetWindow == _mainWindows[Mode.Photo_Comparison])
                {
                    ((Photo_Comparison)targetWindow).page_init();
                }
                else if (targetWindow == _mainWindows[Mode.UsageReport])
                {
                    ((UsageReport.UsageReport)targetWindow).page_init();
                }

                // To avoid loading page of flicker, load direct, because it does no relationship with hardware.
                if (targetWindow == _mainWindows[Mode.ImageCapture] ||
                    targetWindow == _mainWindows[Mode.Photo_Comparison] ||
                    targetWindow == _mainWindows[Mode.Photo_Customer_Type2] ||
                    targetWindow == _mainWindows[Mode.Photo_Customer_Search]
                    )
                {
                    return;
                }

                // For hardware to turn off the pump, it has to excute this foreach 
                foreach (var window in _mainWindows.Values)
                {
                    // ReSharper disable PossibleUnintendedReferenceComparison
                    if (window != targetWindow && window != _mainWindows[Mode.Home])
                        window.Hide();
                        //-window.Opacity=0;
                    else
                        currentWindow = targetWindow;

                    // ReSharper restore PossibleUnintendedReferenceComparison
                }
                //-targetWindow.GotFocus();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{1}", e.Message));
            }
        }
        #endregion

        #region Creditcode Control
        public void StartCreditCodeTimer()
        {
            // Start Credit Code Timer
            ((Home)_mainWindows[Mode.Home]).StartCreditCodeExpiredTimer();
        }

        public void StopCreditCodeTimer()
        {
            // Start Credit Code Timer
            ((Home)_mainWindows[Mode.Home]).StopCreditCodeExpiredTimer();
        }
        #endregion

        #region Board ManagerDataReceived
        private void BoardManagerDataReceived(object sender, EventArgs e)
        {
            //sww added
            if (ControlParams.Params.p_SystemLoading == true)
                return;

            //sww added
            bool readyToClearStartupFlag = false;

            // TODO: Fix this event's parameter: sender is the message received (incorrect)
            // We only care about the inputs received, not set responses

            if (!((string)sender).StartsWith("DataReceived; @i"))
                return;

            for (var i = 0; i < Constants.NUMBER_OF_STATIONS; i++)
            {
                var station = _boardManager.mStationStates[i];

                //bob's
                //if (station == null || 
                //    station.PartNumber == null || 
                //    _mainWindows[Mode.UsageReport] == null ||
                //    ((UsageReport.UsageReport)_mainWindows[Mode.UsageReport]).Model == null || 
                //    station.mFullBottleData == _previousFullBottleData[i]) 
                //    continue;

                if (station == null ||
                       _mainWindows[Mode.UsageReport] == null ||
                     ((UsageReport.UsageReport)_mainWindows[Mode.UsageReport]).Model == null ||
                     station.mFullBottleData == _previousFullBottleData[i])
                {
                    continue;
                }

                _previousFullBottleData[i] = station.mFullBottleData;

                if (BottleChanged != null)
                    BottleChanged(this, new BottleChangedEventArgs { BottleIndex = i, Station = station });

                if (station.Uid != Station.NoBottleUid && !string.IsNullOrEmpty(station.PartNumber))
                {
                    if (ControlParams.Params.p_BottleCountOn)                                   // 0020-03
                    {
                        // Update Information, count the used bottle 
                        ((UsageReport.UsageReport)_mainWindows[Mode.UsageReport]).Model.LogUsage(station, i, ControlParams.Params.p_SystemStartUp);
                    }
                    
                    readyToClearStartupFlag = true;
                }

                SaveBottleDataFile();                                                           // 0020-06
            }

            // added to avoid duplicate bottle insertion count when system boot up
            if (readyToClearStartupFlag == true)
                ControlParams.Params.p_SystemStartUp = false;

            //Reload Usage File- For T2
            //UsageReportModel.LoadProductUsages();

            ((HydraFacial)_mainWindows[Mode.HydraFacial]).setBattery(BoardManager.LedBatteryLevel, BoardManager.LedIsCharging);
            ((wpfStatus)_mainWindows[Mode.wpfStatus]).setBattery(BoardManager.LedBatteryLevel, BoardManager.LedIsCharging);
        }
        #endregion

        #region functions
        
        private string GetSavedLanguage()                                                       // 0106-11
        {
            try
            {
                string line = "";
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\language.dat",Encoding.ASCII))  // 0102-33  0106-09  0106-18
                {
                    if ((line = file.ReadLine()) != null)                                       // 0106-09
                    {
                        ControlParams.Params.p_SecondLanguage = line;
                    }
                }

                return line;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private bool CheckInputLanguageReady(string L)
        {
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                if (lang.Culture.ToString() == L)
                    return true;
            }
            return false;
        }

        public static string getTextMessages(string keyword)                                    // 0103-10
        {
            try
            {
                return Application.Current.Resources[keyword].ToString();
            }
            catch (Exception ex)
            {
                System.Media.SystemSounds.Beep.Play();
                System.Media.SystemSounds.Beep.Play();

                Utility.Lib.SaveErrorLog("Language keyword->" + keyword + " not found");        // 0103-09
                return "";
            }
        }

        // 2014 11/05
        public void SaveBottleDataFile()
        {
            try
            {
                // If system haven't get uid from RFID
                if (_boardManager.mStationStates[0] != null)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + "\\..\\UsageLogs\\bottles.dat", false,Encoding.ASCII))  // 0102-33  0106-09
                    {
                        file.WriteLine(_boardManager.mStationStates[0].Uid);
                        file.WriteLine(_boardManager.mStationStates[1].Uid);
                        file.WriteLine(_boardManager.mStationStates[2].Uid);
                        file.WriteLine(_boardManager.mStationStates[3].Uid);
                        file.WriteLine(_boardManager.mStationStates[4].Uid);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bottle info save failed!");
            }
        }
        #endregion
    }
}
