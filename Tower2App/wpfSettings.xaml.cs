using System;
using System.Windows;
using JetBrains.Annotations;

using System.Diagnostics;
using System.ComponentModel;                                                                    // 0106-15

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfSettings.xaml
    /// </summary>
    public partial class wpfSettings : Window
    {
        public wpfSettings()
        {
            InitializeComponent();

            init();                                                                             // 0106-14

            existprocessID = GetExistProcessByName("explorer");                                 // 0102-29

            App.cs_Events_Language.PropertyChanged += OnPropertyChanged;                        // 0106-15
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0106-15
        {
            if (propertyChangedEventArgs.PropertyName == "NewLanguage")
            {
                init();
            }
        }

        public void init()                                                                      // 0106-14
        {
            Utility.Lib.LoadImageFromAppDir(imgSettings, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\HF-Settings.png"); // 0106-05  0106-13
        }

        private void getExistProcess(string name)                                               // 0102-29
        {
            Process[] localByName = Process.GetProcessesByName(name);
        }

        [NotNull]
        public App App
        {
            get { return (App) Application.Current; }
        }

        #region Button Control
        // T2
        private void btnUsageReport_Click(object sender, RoutedEventArgs e)
        {
            //App.Go(Mode.UsageReport);                                                         // 0102-35
        }

        private void btnCreditCode_Click(object sender, RoutedEventArgs e)
        {
            Utility.Lib.CreditLog("Creditcode button click in setting - Show credit code page");// 0103-05

            App.StopCreditCodeTimer();                                                          // 0020-05

            App.Init_creditcode(true);                                                          // 0020-05
        }

        // T2
        private void btnMultipleUser_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfUser);
        }

        // T2
        private void btnDemoMode_Click(object sender, RoutedEventArgs e)
        {
            // App.Go(Mode.wpfDemo);                                                            // 0102-35
        }

        private void btnDailyEssentials_Click(object sender, RoutedEventArgs e)
        {
            return;                                                                             // 00115a1-0005  
            
            App.Go(Mode.wpfProductMenu);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfUpdate);
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)                          // 0102-38
        {
            return;                                                                             // 00115a1-0005

            App.Go(Mode.wpfBackup);
            ((wpfBackup)App._mainWindows[Mode.wpfBackup]).LoadBackupDir();                      // 0103-01
        }
        #endregion
     
        #region Bring up windows API
        //private Process proc;
        private int processID { set; get; }                                                     // 0102-29
        private IntPtr processHandle { set; get; }                                              // 0102-29
        private int existprocessID { set; get; }                                                // 0102-29
        private void btnWifi_Click(object sender, RoutedEventArgs e)                            // 0102-29
        {
            try
            {
              
                KillProcessByname("explorer");

                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        //FileName = "cmd.exe",
                        //Arguments = "shell:::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                        //UseShellExecute = false,
                        //RedirectStandardOutput = true,
                        //CreateNoWindow = true

                        //FileName = "explorer.exe",
                        //Arguments = "shell:::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                        //UseShellExecute = false,
                        //RedirectStandardOutput = true,
                        //CreateNoWindow = true

                        FileName = "explorer.exe",
                        Arguments = "shell:::{1fa9085f-25a2-489b-85d4-86326eedcd87}",
                     
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true

                    }
                };

                // read back
                proc.Start();
                processID = proc.Id;

                processHandle = proc.Handle;
             
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion

        #region Bring up Windows Network API
        private void btnNetwork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KillProcessByname("explorer");

                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        //FileName = "cmd.exe",
                        //Arguments = "shell:::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                        //UseShellExecute = false,
                        //RedirectStandardOutput = true,
                        //CreateNoWindow = true

                        //FileName = "shell:::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                        //Arguments = "",
                        //UseShellExecute = false,
                        //RedirectStandardOutput = true,
                        //CreateNoWindow = true

                        FileName = "explorer.exe",
                        Arguments =
                            @"/N,::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\::{21EC2020-3AEA-1069-A2DD-08002B30309D}\::{7007ACC7-3202-11D1-AAD2-00805FC1270E}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true

                        //FileName = "explorer.exe",
                        //Arguments = @"shell:::{26EE0668-A00A-44D7-9371-BEB064C98683}\3\::{1fa9085f-25a2-489b-85d4-86326eedcd87}",
                        //UseShellExecute = false,
                        //RedirectStandardOutput = true,
                        //CreateNoWindow = true

                    }
                };

                // read back
                proc.Start();
                processID = proc.Id;
            
            }
            catch (Exception)
            {
                { }
                throw;
            }
        }
        #endregion

        #region Process Functions
        private void KillProcessByname(string Name)
        {
            try
            {
                int count;
                Process[] prs = Process.GetProcessesByName(Name);
                count = prs.Length;
                int i = 0;
                foreach (Process pr in prs)
                {
                    if (existprocessID != pr.Id && existprocessID != 0)
                    {
                        pr.Kill();
                        pr.WaitForExit();
                        //break;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private int GetExistProcessByName(string Name)
        {
            Process[] prs = Process.GetProcessesByName(Name);
            int i = 0;
            foreach (Process pr in prs)
            {
                return pr.Id;
                i++;
            }

            return 0;
        }
        #endregion

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
                Dispatcher.Invoke((Action)(() =>
                {
                    KillProcessByname("explorer");
                }));
        }

        private void Me_Deactivated(object sender, System.EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;                                    // 0101-03
        }

        private void Me_Activated(object sender, System.EventArgs e)
        {
            NavBar.setVolume(ControlParams.Params.p_AudioVolume);                               // 0101-03

            Dispatcher.Invoke((Action)(() =>
            {
                KillProcessByname("explorer");
            }));                                                                                // 0102-29
        }

        private void Me_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                KillProcessByname("explorer");
            }));                                                                                // 0102-29
        }

        private void btnSystem_Click(object sender, RoutedEventArgs e)                          // 0106-06
        {
            App.Go(Mode.wpfSystem);                                                             // 0106-18 disabled for this version  
        }

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLanguage);                                                           // 0106-18 disabled for this version 115a1-0001
        }

        private void Me_Unloaded(object sender, RoutedEventArgs e)
        {
            App.cs_Events_Language.PropertyChanged -= OnPropertyChanged;                        // 0106-18
        }
    }
}
