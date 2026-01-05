using System;
using System.Text;
using System.Windows;
using JetBrains.Annotations;
using System.Windows.Threading;
using System.Timers;
using System.IO;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfShutdown.xaml
    /// </summary>
    public partial class wpfShutdown : Window
    {
        public wpfShutdown()
        {
            InitializeComponent();
            
            Utility.Lib.LoadImageFromAppDir(imgYesNo, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage+"/cleansystem.png");          // 0102-39  0106-13 0106-18

            Utility.Lib.LoadImageFromAppDir(imgCleanup, "/Skin/Images/" + ControlParams.Params.p_SecondLanguage +"/cleanup_vortex_v2.png"); // 0103-06  0106-13
            
            Utility.Lib.LoadImageNoLock(imgTopmenu, "\\Skin\\Images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");                  // 0106-06

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            Utility.Lib.LoadImageNoLock(imgStart, "\\Skin\\Images\\Start.png");                                                             // 0106-05

            Utility.Lib.LoadImageNoLock(imgClose, "\\Skin\\Images\\Close.png");                                                             // 0106-05
        }

        private System.Timers.Timer timer;
        System.Windows.Threading.DispatcherTimer dispatchertimer = new System.Windows.Threading.DispatcherTimer();

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            cvsYesNo.Visibility = Visibility.Hidden;
            cvsCleanup.Visibility = Visibility.Visible;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            ShutdownProcess();
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            ShutdownProcess();
        }

        private void InitCleanupProgressBar(int max)
        {
            pbrClean.Value = 0;
            pbrClean.Maximum = max;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnDone.IsEnabled = false;

            TurnOnPump_Vortex(Settings.CleanupVacuum_Vortex);  // 0106-08  // 0106-14

            btnStart.Visibility = Visibility.Hidden;
            cvsCleanup_Msg2.Visibility=Visibility.Visible;

            InitCleanupProgressBar(Settings.CleanupSeconds_Vortex);

            this.Dispatcher.Invoke((Action)(() =>
            {
                timer.Start();
            }), DispatcherPriority.ContextIdle, null);

        }

        private void TurnOnPump_Vortex(int value)                               // 0106-08  for Tower 1.5  // 0106-14
        {
            App.BoardManager.AddLogMessage("pressure=" + value);                // 0106-16

            App.sp.SerialCmdSend("X0000" + "\r");                               // 0102-36

            int V = (int)(value * 64.62 + 55.38);
            string s = (V).ToString().PadLeft(4, '0');
            if (App.sp != null)
            {
                App.sp.SerialCmdSend("v" + s + "\r");
                App.sp.SerialCmdSend("VN" + "\r");                              // 0102-36
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                updateCleanupProgressBar();
            }
            ));
        }

        private void updateCleanupProgressBar()
        {
            if (pbrClean.Value < pbrClean.Maximum)
            {
                pbrClean.Value += 1;
            }
            else
            {
                timer.Stop();

                // Shut down the machine
                ShutdownProcess();
            }
        }

        private void ShutdownProcess()
        {
            // Turn off hardware
            if (App.sp != null)
            {
                App.sp.SerialCmdSend("X4095" + "\r"); // 0102-36
                App.sp.SerialCmdSend("VF" + "\r"); // 0102-36

                App.sp.SerialCmdSend("LF" + "\r"); // 0102-36
                App.sp.SerialCmdSend("Lf" + "\r"); // 0102-36
              
                App.sp.SerialCmdSend("OK" + "\r"); // 0103-03
            }

            // It has to use IO board shut down for power button
            //var psi = new ProcessStartInfo("shutdown", "/s /t 0");   // shut down 0103-03
            //psi.CreateNoWindow = true;
            //psi.UseShellExecute = false;
            //Process.Start(psi);
        }

        private string ReadLastBackupLog()
        {
            try
            {   
                var filename = Path.Combine(Environment.CurrentDirectory, "backup.log");

                if (!File.Exists(filename))
                    return "";
                
                String line;
                using (StreamReader sr = new StreamReader(filename,Encoding.ASCII))  // 0106-09
                {
                    // Read the stream to of date & time
                    line = sr.ReadToEnd();
                }

                return App.getTextMessages("Last Backup:") + line;
            }
            catch (Exception e)
            {
                Utility.Lib.SaveErrorLog("read backup.log file failed");
                return "";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed); // 0020-08
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            lblBackupMessage.Content = ReadLastBackupLog();
        }
    }
}
