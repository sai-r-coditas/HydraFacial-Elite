using System;
using System.Text;
using System.Windows;
using System.Threading;
using System.IO;

using System.Timers;                                                                            // 0103-01
using System.Windows.Threading;                                                                 // 0103-01
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;                                                                    // 0106-15

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wSetup.xaml
    /// </summary>
    public partial class wpfBackup : Window
    {
        #region Loading
        private App App
        {
            get { return (App)Application.Current; }
        }

        private System.Timers.Timer timer;
        private string USBDrive { set; get; }                                                   // 0103-01
        public wpfBackup()
        {
            InitializeComponent();
            LoadBackupDir();

            btnSendMail.Visibility = Visibility.Hidden;
            btnUserManagement.Visibility = Visibility.Hidden;                                   // 0102-38

            timer = new System.Timers.Timer(1000);                                              // 0103-01
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);                            // 0103-01
     
            btnRemove.Visibility = Visibility.Hidden;                                           // 0103-03
            txtBackup.Visibility = Visibility.Hidden;                                           // 0103-03
            lstFolder.Visibility = Visibility.Hidden;                                           // 0103-03

            init();                                                                             // 0106-14

            progressbar.Visibility = Visibility.Hidden;

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
            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\images\\" + ControlParams.Params.p_SecondLanguage + "\\WindowBackground_restore.png");  // 0106-13

            tbkBackup.Text = App.getTextMessages("backup_Backup");                              //"Backup"; 0103-09  0106-01
            tbkRestore.Text = App.getTextMessages("backup_Restore");                            //"Restore";  // 0103-09  0106-01
        }

        private static string GetUSBLetter()                                                    // 0103-01
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    return drive.Name;
                }
            }
            return "";
        }

        public void LoadBackupDir()
        {
            lstFolder.Items.Clear();
            string backupDirectory = GetUSBLetter()+ControlParams.Params.Photos_Backup + "\\";

            if (!Directory.Exists(backupDirectory))                                             // 0103-01 no directory available
                return;
            
            int dirLength = backupDirectory.Length;
            foreach (string folder in System.IO.Directory.GetDirectories(backupDirectory))
            {
                if (folder.Length > dirLength)
                    lstFolder.Items.Add(folder.Substring(dirLength, folder.Length - dirLength));
            }
        }

        static double GetDirectorySize(string p)
        {
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, "*.*");
          
            // Calculate total bytes of all files in a loop.
            double b = 0;
            foreach (string name in a)
            {
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
          
            // Return total size
            return b;
        }

        private double GetNoOfFile(string P)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(P);
            double count = (double) dir.GetFiles().Length;

            return count;
        }
        #endregion

        #region Backup button
        private void btnBackup_Click(object sender, RoutedEventArgs e)  // 0103-01
        {
            btnBackup.IsEnabled = false;
            btnRestore.IsEnabled = false;

            if (GetUSBLetter() =="" )
            {
                BackupMessage(App.getTextMessages("USB drive is not available!"), true); // 0103-07
                return;
            }

            App.wpfyesno.WarningMessage(App.getTextMessages("Continue backing-up your photos?"));
            App.wpfyesno.ShowDialog();
            var ans = App.wpfyesno.ans;
            if (ans != "yes")
            {
                btnBackup.IsEnabled = true;
                btnRestore.IsEnabled = true;
                return;
            }

            if (txtBackup.Text == "" || txtBackup.Text == null) // 0103-02
            {
                txtBackup.Text = "photobackup";
            }
          
            progressbar.Maximum = GetNoOfFile(ControlParams.Params.Photos_Default);
            progressbar.Value = 0;
            progressbar.Visibility = Visibility.Visible;

            string Target;
            Target = GetUSBLetter() + ControlParams.Params.Photos_Backup + "\\" + txtBackup.Text.Trim();// + "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");

            // Delete old directory
            if (Directory.Exists(Target))
            {
                var dir = new DirectoryInfo(Target);
                dir.Delete(true);
            }
           
            // Create directory
            DirectoryInfo di = Directory.CreateDirectory(Target);
               
            //XcopyFiles(ControlParams.Params.Photos_Default, Target);

            CopyFiles(ControlParams.Params.Photos_Default, Target);
                
          
            if (Directory.Exists(Target))  // 0102-38
            {
                if (File.Exists(Target + "\\edge_v001.mdb"))
                    File.Delete(Target + "\\edge_v001.mdb");

                File.Copy(Environment.CurrentDirectory + "\\..\\DB\\edge_v001.mdb", Target + "\\edge_v001.mdb");  // 0102-38
            }

            BackupMessage(App.getTextMessages("Photos Backup Completed!"), false); // 0103-06

            Savebackuplog(); // 0106-05

            txtBackup.Text = "";
       
            btnBackup.IsEnabled = true;
            btnRestore.IsEnabled = true;
            LoadBackupDir();
        }

        string[] Files;
        string strFileName = "";
        private void CopyFiles1(string sourcepath, string targetpath)
        {
            Files = Directory.GetFiles(sourcepath);
            double i = 0;
         
            DispatcherTimer timer = new DispatcherTimer();
            foreach (string s in Files)
            {
                Thread t = new Thread(
                    new ThreadStart(
                        delegate()
                            {
                                DispatcherOperation dispOp =
                                    progressbar.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                    new Action(
                                        delegate()
                                        {
                                            strFileName = s;
                                            string fileName = System.IO.Path.GetFileName(strFileName);
                                            string destFile = System.IO.Path.Combine(targetpath, fileName);
                                          
                                            System.IO.File.Copy(strFileName, destFile, true);
                                          
                                            progressbar.Value = i;
                                            i++;
                                          
                                            Thread.Sleep(100);
                                        }
                                        ));
                                dispOp.Completed += new EventHandler(dispOp_Completed);
                            }
                        ));
                t.Start();
            }
            progressbar.Visibility = Visibility.Hidden;
        }

        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        private void CopyFiles(string sourcepath, string targetpath)
        {
            Files = Directory.GetFiles(sourcepath);
            double i = 0;
            progressbar.Minimum = 0;
          
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progressbar.SetValue);
           
            DispatcherTimer timer = new DispatcherTimer();
            foreach (string s in Files)
            {
                strFileName = s;
                string fileName = System.IO.Path.GetFileName(strFileName);
                string destFile = System.IO.Path.Combine(targetpath, fileName);

                System.IO.File.Copy(strFileName, destFile, true);
              
                progressbar.Value = i;
                i++;

                Dispatcher.Invoke(updatePbDelegate,
                       System.Windows.Threading.DispatcherPriority.Background,
                       new object[] { ProgressBar.ValueProperty, i });

                Thread.Sleep(100);
            }
            progressbar.Visibility = Visibility.Hidden;
        }
        
        void dispOp_Completed(object sender, EventArgs e)
        {
          
        }

        private void BackupMessage(string msg, bool ColorRed)
        {
            if (ColorRed)
                lblMessageBackup.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC1212"));  // Red
            else
                lblMessageBackup.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00a5e2"));  // Blue

            btnBackup.Visibility = Visibility.Hidden;
            btnRestore.Visibility = Visibility.Hidden;
            lblMessageBackup.Content = msg;
            DoEvents();
            System.Threading.Thread.Sleep(3000);
            btnBackup.Visibility = Visibility.Visible;
            btnRestore.Visibility = Visibility.Visible;
            lblMessageBackup.Content = "";
            btnBackup.IsEnabled = true;
            btnRestore.IsEnabled = true;
        }
        #endregion

        #region Restore
        private void btnRestore_Click(object sender, RoutedEventArgs e)  // 0103-03
        {
            string backupfolder = "photobackup";  // 0103-03
            
            btnRestore.IsEnabled = false;
            btnBackup.IsEnabled = false;

            if (GetUSBLetter() == "")
            {
                RestoreMessage(App.getTextMessages("Please insert USB backup drive!"), true);
                return;
            }

            string SrcDir = GetUSBLetter() + ControlParams.Params.Photos_Backup + "\\" + backupfolder; // 0103-03

            if (!Directory.Exists(SrcDir))
            {
                RestoreMessage(App.getTextMessages("Backup directory not found!"), true);
                return;
            }

            App.wpfyesno.WarningMessage(App.getTextMessages("Continue with photo restore?"));
            App.wpfyesno.ShowDialog();
            var ans = App.wpfyesno.ans;
            if (ans != "yes")
            {
                btnRestore.IsEnabled = true;
                btnBackup.IsEnabled = true;
                return;
            }

            try
            {
                if (Directory.Exists(ControlParams.Params.Photos_Default))
                {
                    // Delete default directory
                    string defaultBackupDirectory = ControlParams.Params.Photos_Default;
                    var dir = new DirectoryInfo(defaultBackupDirectory);
                    dir.Delete(true);
                }

                DoEvents();
                Thread.Sleep(300);

                // Create "default" directory
                if (!Directory.Exists(ControlParams.Params.Photos_Default))
                {
                    Directory.CreateDirectory(ControlParams.Params.Photos_Default);
                }

                progressbar.Maximum = GetNoOfFile(SrcDir);
                progressbar.Value = 0;
                progressbar.Visibility = Visibility.Visible;

                // Copy selected directory to the "default" directory
                string Target;
                Target = ControlParams.Params.Photos_Default+"\\";
 
                if (File.Exists( Environment.CurrentDirectory+"\\..\\DB\\edge_v001.mdb"))  // 0102-38
                    File.Delete( Environment.CurrentDirectory+"\\..\\DB\\edge_v001.mdb");

                File.Copy(SrcDir + "\\edge_v001.mdb",  Environment.CurrentDirectory+"\\..\\DB\\edge_v001.mdb"); // 0102-38

                if (Directory.Exists(SrcDir))

                    //XcopyFiles(SrcDir, ControlParams.Params.Photos_Default);

                    CopyFiles(SrcDir, ControlParams.Params.Photos_Default);

                else
                {
                    RestoreMessage("Directory not exist!", true);
                }
            }
            catch (Exception ex)
            {
                RestoreMessage("Restore Failed!",true);

                progressbar.Visibility = Visibility.Hidden;
                btnRestore.IsEnabled = true;
                btnBackup.IsEnabled = true;
                return;
            }

            RestoreMessage(App.getTextMessages("Restore Completed!"), false);

            btnRestore.IsEnabled = true;
            btnBackup.IsEnabled = true;
        }

        private void RestoreMessage(string msg, bool ColorRed)
        {
            if (ColorRed)
                lblMessageRestore.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC1212"));  // Red
            else
                lblMessageRestore.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00a5e2"));  // Blue

            btnRestore.Visibility = Visibility.Hidden;
            btnBackup.Visibility = Visibility.Hidden;
            lblMessageRestore.Content = msg;
            DoEvents();

            System.Threading.Thread.Sleep(3000);
            btnRestore.Visibility = Visibility.Visible;
            btnBackup.Visibility = Visibility.Visible;
            lblMessageRestore.Content = "";
            btnRestore.IsEnabled = true;
            btnBackup.IsEnabled = true;
        }

        private void XcopyFiles(string Source, string Target)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "xcopy";

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.Arguments = Source + " " + Target + " /e /h /c /I /B";
            process.Start();
        }
        #endregion

        #region Remove Directory
        private void btnRemove_Click(object sender, RoutedEventArgs e)  // For Tower II
        {
           btnRemove.IsEnabled =false;
            if (lstFolder.SelectedItem.ToString() == "" ||lstFolder.SelectedItem.ToString() == null)
            {
                MessageBox.Show("Please select a restore name!");
                btnRemove.IsEnabled =true;
                return;
            }    
            
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show("Remove " +lstFolder.SelectedItem.ToString()+ ", are you sure?", "Warning", btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                     RemoveDir(GetUSBLetter()+ControlParams.Params.Photos_Backup+"\\"+lstFolder.SelectedItem.ToString());  // 0103-01
                     MessageBox.Show("Process Completed");
                     LoadBackupDir();
                     break;

                case MessageBoxResult.No:
                     break;
              
            }
          
            btnRemove.IsEnabled =true;

        }

        private void RemoveDir(string dirTarget)
        {
            if (Directory.Exists(dirTarget))
            {
                // Delete default directory
                string defaultBackupDirectory = dirTarget;
                var dir = new DirectoryInfo(defaultBackupDirectory);
                dir.Delete(true);
            }
        }
        #endregion

        #region Others
        private void btnSendMail_Click(object sender, RoutedEventArgs e)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add("to@edgeforlife.com");
            message.Subject = "This is the Subject line";
            message.From = new System.Net.Mail.MailAddress("From@edgeforlife.com");
            message.Body = "This is the message body";
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("yoursmtphost");
            smtp.Send(message);
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void btnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfUser);
        }
        #endregion

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                if (progressbar.Value < 30)
                {
                    progressbar.Value += 1;
                }
                else
                {
                    timer.Stop();
                }
            }));
        }

        private void Savebackuplog()                                                            // 0106-05
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "backup.log");

            if (!File.Exists(filename))
               
                using (StreamWriter w = new StreamWriter(filename, false, Encoding.ASCII)) ;    // 0106-09

            using (StreamWriter info = new StreamWriter(filename, false,Encoding.ASCII))        // 0106-09
            {
                info.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            }
        }

        private void Me_Activated(object sender, EventArgs e)                                   // 0103-02
        {
            txtBackup.Text = "photobackup";
        }

        private void Me_Unloaded(object sender, RoutedEventArgs e)
        {
            App.cs_Events_Language.PropertyChanged -= OnPropertyChanged;                        // 0106-18
        }
    }
}
