using System;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.IO.Compression;

using System.Windows.Threading;
using System.Threading;

using JetBrains.Annotations;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

using System.Text;                                                                              // 0106-09
using System.Linq;                                                                              // 0106-17

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfUpdate.xaml
    /// </summary>
    public partial class wpfUpdate : Window
    {
        #region Init
        public wpfUpdate()  // 0102-11
        {
            InitializeComponent();

            lblMessage.Visibility = Visibility.Hidden;

            App.cs_Events.PropertyChanged += OnPropertyChanged;                                 // 0102-11

            //"Update requires the device to remain on and uninterrupted while the update is processing.  " & vbNewLine_
            //"The update media should also not be unplugged until prompted.  " & _
            //"Update will begin after you press OK.  Please do not touch", _

            btnStop.Visibility = Visibility.Hidden;
            Button.Visibility = Visibility.Hidden;

            lblSpeed.Content = "";
            lblDownload.Content = "";
            lblProcess.Content = "";

            cvsScreen.Visibility = Visibility.Hidden;                                           // for yes/no dialog window
            cvsProgressInfo.Visibility = Visibility.Hidden;

            webLoad = false;                                                                    // 0102-12
            lblCount.Content = "";

            UpdateLog = "\\update.log";                                                         // 0103-07

            usbZipFile =  @"Debug_update\Debug_update.zip";                                     // 0102-38
            storedZipFile = "c:\\webdownload\\debug_update.zip";                                // 0102-38
            downloadcompleted = "c:\\webdownload\\" + "downloadcompleted.dat";                  // 0102-38
            tmpDir = "c:\\webdownload\\debug_";

            progressbar2.Visibility = Visibility.Hidden;                                        // 0103-02 for usb files copy

            lblFileCount.Visibility = progressbar2.Visibility;                                  // 0103-08

            init();                                                                             // 0106-14
          
            lblHeader.Content = "";                                                             // 0106-12

            App.cs_Events_Language.PropertyChanged += OnPropertyChanged_Language;               // 0106-15

            lblVersion.Content = "CV1.5-00115a-IB-001";                                                 // 0106-26

            lblVersion.Visibility = Visibility.Hidden;                                          // 0106-26
        }

        private void OnPropertyChanged_Language(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0106-15
        {
            if (propertyChangedEventArgs.PropertyName == "NewLanguage")
            {
                init();
            }
        }

        public void init()                                                                      // 0106-14
        {
            tbkDownloadUSB.Text = App.getTextMessages("Load From USB");                         // 0103-09
 
            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\WindowBackground_update.png");   // 0106-05  0106-13
        }

        private bool _webload;
        public bool webLoad
        {
            get { return _webload; }
            set { _webload = value; }
        }

        private string _storedzipfile;
        public string storedZipFile
        {
            get { return _storedzipfile; }
            set { _storedzipfile = value; }
        }

        private string _updatelog;                                                               // 0102-33
        public string UpdateLog
        {
            get { return _updatelog; }
            set { _updatelog = value; }
        }

        private string _usbZipFile;                                                             // 0102-33
        public string usbZipFile
        {
            get { return _usbZipFile; }
            set { _usbZipFile = value; }
        }

        private string _downloadcompleted;                                                      // 0102-33
        public string downloadcompleted
        {
            get { return _downloadcompleted; }
            set { _downloadcompleted = value; }
        }

        private string _tmpDir;                                                                 // 0102-33
        public string tmpDir
        {
            get { return _tmpDir; }
            set { _tmpDir = value; }
        }

        private string _totalfiles;                                                             // 0102-33
        public string totalfiles
        {
            get { return _totalfiles; }
            set { _totalfiles = value; }
        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0102-01
        {
            if (propertyChangedEventArgs.PropertyName == "WifiState")
            {
                if (App.cs_Events.WifiState == "off")
                {
                    btnDownload.Visibility = Visibility.Hidden;
                    
                    // T2
                    //-ShowMessage("*Internet is not available!", true); // for Tower 2 use 
                }
                else
                {
                    btnDownload.Visibility = Visibility.Visible;
                    lblMessage.Visibility = Visibility.Hidden;
                    lblMessage.Content = "";
                }
            }
        }
        #endregion

        #region Web downloadfile
        // Option2
        private void WebDownloading(string location)
        {
            try
            {
                cvsUSB.Visibility = Visibility.Hidden;
                
                btnDownload.Visibility = Visibility.Hidden;
                btnDownload.IsEnabled = false;

                DoEvents();

                ShowMessage("*Finding....", false);
                DoEvents();
             
                if (!CheckUpdateFileExist(location))
                {
                    ShowMessage("*No update file available!", true);
                    DoEvents();
                    Thread.Sleep(5000);
              
                    cvsUSB.Visibility = Visibility.Visible;

                    btnDownload.Visibility = Visibility.Visible;
                    btnDownload.IsEnabled = true;

                    ShowMessage("", true);
                    return;
                }
                
                cvsScreen.Visibility = Visibility.Visible;

                ShowMessage("", false);

                App.wpfyesno.WarningMessage(App.getTextMessages("Update, are you sure?"));   // 0102-21 0103-09
                App.wpfyesno.ShowDialog();
                var data = App.wpfyesno.ans;
                DoEvents();

                cvsScreen.Visibility = Visibility.Hidden;
                if (data != "yes")
                {
                    cvsUSB.Visibility = Visibility.Visible;

                    btnDownload.Visibility = Visibility.Visible;
                    btnDownload.IsEnabled = true;

                    ShowMessage("", true);
                     DoEvents();
                    return;
                }

                ShowMessage("*Process....", false);

                cvsWebProcessing.Visibility = Visibility.Hidden; // 0102-24 complete hidden this button

                cvsProgressInfo.Visibility = Visibility.Visible;

                // hidden all buttons & wait the download process to finish
                ucUser.Visibility = Visibility.Hidden;
                NavBar.Visibility = Visibility.Hidden;
                
                DoEvents();

                if (File.Exists(storedZipFile))  // if file exist, remove it
                    File.Delete(storedZipFile);

                ShowMessage("*Downloading....", false);

                DownloadFile(location, storedZipFile, true);
            }
            catch(Exception ex)
            {
                ShowMessage("*Download failed!", true);
                DoEvents();
                Thread.Sleep(5000);
           
                cvsUSB.Visibility = Visibility.Visible;

                btnDownload.Visibility = Visibility.Visible;
                btnDownload.IsEnabled = true;

                ShowMessage("", true);
                return;
            }
        }

        private bool CheckUpdateFileExist(string url)
        {
            bool result = false;

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 1200; // miliseconds 1200
            webRequest.Method = "HEAD";

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                result = true;
            }
            catch (WebException webException)
            {
                result = false;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }
        #endregion

        #region Download Processing - for both web and USB download
        WebClient webClient;                                                        // the WebClient will be doing the downloading for us
        Stopwatch sw = new Stopwatch();
        public void DownloadFile(string address, string location, bool fromweb)     // for web download and usb download
        {
            //if (App.BoardManager != null )
            //    App.BoardManager.Close();                                         // 0102-12 Close all communication to IO board
            //DoEvents();

            //if (App.BoardManager != null)
            //    App.BoardManager.Close();

            ControlParams.Params.p_Downloading = true;                              // 0102-26 Disable checking
            App.BoardManager.Stop_Timer();                                          // 0102-21 Stop Communiction to IO Board

            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(EH_Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(EH_ProgressChanged);

                if (fromweb)
                    // The variable that will be holding the url address (making sure it starts with http://)
                    address = address.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? address : "http://" + address;

                // Start the stopwatch which we will be using to calculate the download speed
                sw.Start();

                try
                {
                    // Start downloading the file
                    webClient.DownloadFileAsync( new Uri(address), location);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Download Failed!");
                 
                    Utility.Lib.Savetoupdatelog("download error!");                 // 0103-07
                }
            }
        }

        private void EH_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)  // update download information
        {
            // Calculate download speed and output it to labelSpeed.
            lblSpeed.Content = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Update the progressbar percentage only when the value is not the same.
            progressBar.Value = e.ProgressPercentage;

            // Show the percentage on our label.
            lblProcess.Content = e.ProgressPercentage.ToString() + "%";

            // display download speed inforamtion
            lblDownload.Content = string.Format("{0} mb / {1} mb",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
        }

        // The event that will trigger when the WebClient is completed
        private void EH_Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string error = e.Error.ToString();
                
                // error = " error message here";
                MessageBox.Show("Download Failed!");
                
                Utility.Lib.Savetoupdatelog("Download Failed!");  // 0103-07

                return;
            }
              
            if (e.Cancelled == true)
            {
                ShowMessage("*Download has been canceled.", true);
            }
            else
            {
                try
                {
                    unzip3(storedZipFile, tmpDir);  // copy to temp directory 0102-38
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File Extract Failed!");
                  
                    Utility.Lib.Savetoupdatelog("File Extract Failed!");  // 0103-07
                }
            }

            sw.Reset();   // Reset the stopwatch.
           
        }
        #endregion

        #region Final state - Unzip file
        private void unzip3(string zipPath,string extractPath)
        {
            if (!webLoad)
                ShowMessageUSB("*Extract Files...", false);
            else
                ShowMessage("*Extract Files...", false);

            cvsProgressInfo.Visibility = Visibility.Visible;

            try
            {

                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);

                int i = 0;

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    int Count = archive.Entries.Count;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        i++;
                        string fullPath = System.IO.Path.Combine(extractPath, entry.FullName);
                        if (String.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        else
                        {
                            entry.ExtractToFile(fullPath);
                        }

                        progressBar.Value = Convert.ToInt32(100*i/Count);
                        lblCount.Content = progressBar.Value.ToString() + " %";                
                        DoEvents();
                    }
                }

                Create_Completefile();

                DoEvents();

                if (!webLoad)
                    ShowMessageUSB("*Processed success, system shutting down...", false);
                else
                    ShowMessage("*Processed success, system shutting down...", false);
                DoEvents();
             
                Utility.Lib.Savetoupdatelog("update success!");                                 // 0103-07

                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                if (!webLoad)
                    ShowMessageUSB("*Extract Files Failed!", true);
                else
                    ShowMessage("*Extract Files Failed!", true);
                
                DoEvents();
             
                Utility.Lib.Savetoupdatelog("*Extract Files Failed!");                          // 0103-07

                Thread.Sleep(5000);
            }
          
            if (!webLoad)   // from usb or web
                ShowMessageUSB("*System shutting down...", false);
            else
                ShowMessage("*System shutting down...", false);
           
            Thread.Sleep(8000);
            
            // Close the application
            ((wpfLogin)App._mainWindows[Mode.wpfLogin]).ApplicationClose();

            // Restart computer
            if  ( Settings.SystemRestart)                                                       // 0102-12
                 RestartComputer();
        }

        private void RestartComputer()
        {
            var psi = new ProcessStartInfo("shutdown", "/r /t 0");                              // restart
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        private void Remove_completefile()
        {
            // T2
            if (File.Exists(downloadcompleted))
            {
                File.Delete(downloadcompleted);
            }
        }

        private void Create_Completefile()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(downloadcompleted, false, Encoding.ASCII))  // 0106-09
            {
                file.WriteLine("download completed");
            }
        }
        #endregion

        #region USB Files Download
        /// <summary>
        /// DownloadUSB Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownloadUSB_Click(object sender, RoutedEventArgs e)                     // 0103-02
        {
            Utility.Lib.Savetoupdatelog("USB updating");                                        // 0103-07

            Remove_completefile();                                                              // 0102-21

            webLoad = false;                                                                    // 0102-12

            btnDownloadUSB.Visibility = Visibility.Hidden;
        
            cvsWeb.Visibility = Visibility.Hidden;

            if (GetUSBLetter() == "")
            {
                ShowMessageUSB("File Not Found!", true);

                DoEvents();

                Thread.Sleep(5000);

                btnDownloadUSB.Visibility = Visibility.Visible;

                cvsWeb.Visibility = Visibility.Visible;

                ShowMessageUSB("", true);                                                       // Clear message

                return;
            }

            // T2
            //string usbFile = GetUSBLetter() + @"Debug_update\Debug_update.zip";
            //storedZipFile = "c:\\webdownload\\debug_update.zip";  
            
            // Delete all directory end with _check, if have any
            DeleteAllCheckDirectory(Environment.CurrentDirectory + "\\");

            if (CheckFolderInuse())
            {
                ShowMessageUSB("File is in use, please restart the machine and try again!", true);

                DoEvents();

                Thread.Sleep(5000);

                btnDownloadUSB.Visibility = Visibility.Visible;

                cvsWeb.Visibility = Visibility.Visible;

                ShowMessageUSB("", true);                                                       // Clear message

                return;
            }
           
            if (!File.Exists(GetUSBLetter() + usbZipFile))                                      // if it is not a zip file, copy all files from usb
            {
                // 0102-39
                cvsScreen.Visibility = Visibility.Visible;
                App.wpfyesno.WarningMessage(App.getTextMessages("Update all files from usb drive, are you sure?"));  // 0103-09
                App.wpfyesno.ShowDialog();
                var ans = App.wpfyesno.ans;

                cvsScreen.Visibility = Visibility.Hidden;
                if (ans != "yes")
                {
                    btnDownloadUSB.Visibility = Visibility.Visible;
                    cvsWeb.Visibility = Visibility.Visible;
                    ShowMessageUSB("", true);
                    return;
                }

                AddtoUpdateFiles(Environment.CurrentDirectory + "\\..\\usagelogs\\filesread.txt", "", true);   // 0103-08
                AddtoUpdateFiles(Environment.CurrentDirectory + "\\..\\usagelogs\\filescopy.txt", "", true);   // 0103-08

                // No zip files, so copy all files from the usb drive including direcotries
                ShowMessageUSB("Get Total files ...", false);                                   // 0102-38
                progressbar2.Visibility = Visibility.Visible;
                lblFileCount.Content = "";                                                      // 0101-08
                lblFileCount.Visibility = progressbar2.Visibility;

                DoEvents();

                // ======================
                // Get no. of total files
                // ======================
                count=0;
                progressbar2.Maximum = TotalDirfiles(GetUSBLetter());
                
                ShowMessageUSB("Copy all files from USB drive ...", false);                     // 0102-38
                DoEvents();

                Thread.Sleep(3000);                                                             // 0103-07

                count = 0;                                                                      // must init again here
         
                // If success, restart the machine
                if (UpdateRoutine(GetUSBLetter()))
                {
                    ShowMessageUSB("Update success, system restart...", false);                 // 0102-38

                    Utility.Lib.Savetoupdatelog("Update success, file Copied -" + count + "/" + progressbar2.Maximum.ToString());  // 0103-08

                    DoEvents();
                    
                    System.Threading.Thread.Sleep(5000);

                    // Update finished reset the machine
                    if (Settings.SystemRestart)                                                 // 0106-17
                        RestartComputer();                                                      // 0103-02

                }

                progressbar2.Visibility = Visibility.Hidden;

                lblFileCount.Visibility = progressbar2.Visibility;                              // 0103-08
                
                ShowMessageUSB("", true);
                return;
            }

            // =========================================
            // Not in use 
            // =========================================

            // Method 1
            cvsScreen.Visibility = Visibility.Visible;

            App.wpfyesno.WarningMessage(App.getTextMessages("Update, are you sure?"));          // 0103-09
            App.wpfyesno.ShowDialog();
            var data = App.wpfyesno.ans;

            cvsScreen.Visibility = Visibility.Hidden;
            if (data != "yes")
            {
                btnDownloadUSB.Visibility = Visibility.Visible;
                cvsWeb.Visibility = Visibility.Visible;

                ShowMessageUSB("", true);
                return;
            }

            cvsProgressInfo.Visibility = Visibility.Visible;
            ShowMessageUSB("Processing ....", false);
           
            // hidden all buttons & wait the download process to finish
            ucUser.Visibility = Visibility.Hidden;
            NavBar.Visibility = Visibility.Hidden;
           
            DoEvents();

            try
            {
                // Move file
                if (File.Exists(storedZipFile))
                    File.Delete(storedZipFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("File exist already!");
                btnDownloadUSB.Visibility = Visibility.Visible;
    
                cvsWeb.Visibility = Visibility.Visible;

                ShowMessageUSB("", true);
                return;
            }
            DoEvents();

            ShowMessageUSB("Copying Files...", false);
            
            DownloadFile(GetUSBLetter() + usbZipFile, storedZipFile, false);                    // 0102-12

            DoEvents();
        }

        private string GetUSBLetter()                                                           // 0102-26
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
      
        /// <summary>
        /// UpdateRoutine - major update process
        /// </summary>
        /// <param name="updateDrive"></param>
        /// <returns></returns>
        /// 
        /// 1. Delete all tmp files
        /// 2. Rename bak to tmp
        /// 3. Create main.bak directory
        /// 4. Move current files to main.bak
        /// 5. Delete tmp files
        /// 
        private bool UpdateRoutine(string updateDrive)                                          // 0102-38  0103-03  0106-17
        {
            string originalDrive = Environment.CurrentDirectory + "\\";                         // 0103-08
            string processfile ="";
      
            try
            {
                #region Copy Root directory files
 
                //if (!AllBakDirectoryDeleted(originalDrive))                                   // 0106-15
                //{
                //    MessageBox.Show("File in use, please restart the machine and try again!");
                //    return false;
                //}

                // delete all tmp directory if have any
                DeleteAllTmpDirectory(originalDrive);                                           // 0106-17               


                // change main.bak to main.bak_tmp
                RenameAllDirectoryBakToTmp(originalDrive);                                      // 0106-17

                // Create new bak directory
                Directory.CreateDirectory(originalDrive + "main.bak");

                // Assume one level of folder for now
                DirectoryInfo rootDir = new DirectoryInfo(updateDrive);                         // updatedrive => usb source
           
                foreach (FileInfo pFile in rootDir.GetFiles())
                {
                    string updateFileFullPath = pFile.FullName;
                    string originalFileFullPath = updateFileFullPath.Replace(updateDrive, originalDrive);
                    
                    string backupFileFullPath = originalDrive + "main.bak\\" + pFile.Name;
                    
                    // if this is a zip file, skip that
                    if (Path.GetExtension(updateFileFullPath).ToLower() == ".zip" )             // 0102-38  0103-08
                        continue;

                    if (Path.GetExtension(updateFileFullPath).ToLower() == ".chk")              // 0103-08  0103-08
                        continue;
                   
                    if (pFile.Attributes == FileAttributes.Hidden)                              // 0103-08
                        continue;

                    if (pFile.Name.Substring(0, 1) == "." || pFile.Name == "IndexerVolumeGuid") // 0103-08 skip this windows system file 
                        continue;
 
                    // move original to backup
                    // move new to original
                    if (File.Exists(originalFileFullPath))
                    {
                        File.Move(originalFileFullPath, backupFileFullPath);
                    }

                    File.Copy(updateFileFullPath, originalFileFullPath);
                   
                    count++;

                    AddtoUpdateFiles(Environment.CurrentDirectory + "\\..\\usagelogs\\filescopy.txt", count.ToString() + " " + pFile.Name + Environment.NewLine, false); // 0103-08

                    UpdateProgressBar(count);

                    Thread.Sleep(100);
                }

                if (Directory.Exists(originalDrive  +"main.bak_tmp"))                           // delete tmp folder
                {
                    DeleteDirectory(originalDrive  +"main.bak_tmp");
                }
                #endregion

                #region Copy Sub folder and files
                ShowMessageUSB("Update directories ...", false);                                // 0102-38
              
                // If directory exist, delete exist bak and copy current to backup, and do updating
                foreach (DirectoryInfo pDir in rootDir.GetDirectories())
                {
                    if (pDir.Name == "edgephotos" || pDir.Name == "main.bak" || pDir.Name == "System Volume Information")  // skip photos folder 0103-01  0106-15
                        continue;

                    if ((pDir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)     // 0103-06
                        continue;

                    if (pDir.Name.Substring(0, 1) == ".")                                       // 0103-06  Skip incorrect folder name 
                        continue;

                    DirectoryInfo updateDirInfo = pDir;
                    DirectoryInfo originalDirInfo = new DirectoryInfo(updateDirInfo.FullName.Replace(updateDrive, originalDrive));

                    DirectoryInfo backupDirInfo = new DirectoryInfo(originalDrive + "main.bak" + "\\" + pDir.Name);// 0106-18

                    processfile = backupDirInfo.Name;

                    DoEvents();                                                                 // 0106-15
    
                    //System.Threading.Thread.Sleep(300);

                    //CheckIfReadytoCopy(backupDirInfo.FullName);

                    // move original to backup folder
                    if ((originalDirInfo.Exists))
                    {
                        Directory.Move(originalDirInfo.FullName, backupDirInfo.FullName);       //??
                    }

                    // Create new original folder name
                    originalDirInfo.Create();

                    Thread.Sleep(200);                                                          // 0106-18

                    // Copy new files to current directory
                    CopyDir(updateDirInfo.FullName, originalDirInfo.FullName);

                    // Delete tmp folder
                    if (Directory.Exists(backupDirInfo.FullName + "_tmp"))
                    {
                        // backupDirInfo.Delete(true);
                        DeleteDirectory(backupDirInfo.FullName + "_tmp");                              
                    }

                    System.Threading.Thread.Sleep(300);
                }
                #endregion

                // delete all tmp directory if have any, make sure tmp folders on root have been deleted 
                DeleteAllTmpDirectory(originalDrive);                                           // 0106-19       

                return true;

            }
            catch (Exception ex)
            {
             
                //MessageBox.Show("Update processing failed! " + processfile );                 // 00115a1-0003
                MessageBox.Show( processfile + App.getTextMessages("File Not Found!"));         // 00115a1-0003

                Utility.Lib.SaveErrorLog("Update processing failed! " + processfile + ex.ToString());
                return false;
            }
        }

        void UpdateProgressBar(double v)                                                        // 0020-10
        {
            UpdateUI(v);
          
            //Dispatcher.Invoke((Action)(() =>
            //{
            //    if (v > progressbar2.Maximum)
            //        progressbar2.Value = progressbar2.Maximum;
            //    else
            //        progressbar2.Value = v;

            //    lblFileCount.Content = count+"/"+progressbar2.Maximum.ToString();             // 0103-08
            //}));
        }

        private void UpdateUI(double v)                                                         // 0103-08
        {
            if (v > progressbar2.Maximum)
                progressbar2.Value = progressbar2.Maximum;
            else
                progressbar2.Value = v;

            lblFileCount.Content = count + "/" + progressbar2.Maximum.ToString();               // 0103-08

            DoEvents();
        }
        #endregion

        #region Count Total Files
        private int count;
        private double TotalDirfiles(string dir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    // if this is a zip file, skip that
                    if (Path.GetExtension(f).ToLower() == ".zip")  // 0102-38
                        continue;

                    if (Path.GetExtension(f).ToLower() == ".chk")  // 0103-08
                        continue;

                    FileInfo info = new FileInfo(f);     // 0103-06
                    if (info.Attributes == FileAttributes.Hidden)  // 0103-08
                        continue;

                    if (info.Name.Substring(0, 1) == "." || info.Name == "IndexerVolumeGuid") // 0103-08 skip this windows system file 
                        continue;

                    count++;

                    AddtoUpdateFiles(Environment.CurrentDirectory + "\\..\\usagelogs\\filesread.txt", count.ToString() + " " + info.Name + Environment.NewLine, false); // 0103-08

                }

                foreach (string d in Directory.GetDirectories(dir))  // 0103-08
                {
                    DirectoryInfo _dir = new DirectoryInfo(d);
                    if (_dir.Name == "edgephotos" || _dir.Name == "main.bak")  // skip photos folder 0103-08
                        continue;
              
                    if ((_dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        continue;

                    if (_dir.Name.Substring(0, 1) == ".")  // Skip incorrect folder name 
                        continue;

                    TotalDirfiles(d);
                }

                return count;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 0;
            }
        }
        #endregion

        #region Copy directory
        public  void CopyDir(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);

            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public  void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (Path.GetExtension(fi.Name).ToLower() == ".zip")                             // 0102-38
                    continue;

                if (Path.GetExtension(fi.Name).ToLower() == ".chk")                             // 0103-08
                    continue;
              
                if (fi.Attributes == FileAttributes.Hidden)                                     // 0103-08
                    continue;
                
                // skip invalid file name start with "."
                if (fi.Name.Substring(0,1) == "." || fi.Name == "IndexerVolumeGuid")            // 0103-08 skip this windows system file 
                    continue;
                
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);  

                count++; 

                AddtoUpdateFiles(Environment.CurrentDirectory + "\\..\\usagelogs\\filescopy.txt", count.ToString() + " " + fi.Name + Environment.NewLine, false); // 0103-08

                UpdateProgressBar(count);                                                       // 0103-03
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {

                if (diSourceSubDir.Name == "edgephotos" || diSourceSubDir.Name == "main.bak")   // skip photos folder 0103-08
                    continue;

                if ((diSourceSubDir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;

                if (diSourceSubDir.Name.Substring(0, 1) == ".")  // Skip incorrect folder name 
                    continue;

                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);

                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        #endregion

        #region Others
        private void btnDownload_Click(object sender, RoutedEventArgs e)                        // Web download
        {
            Utility.Lib.Savetoupdatelog("Web updating");                                        // 0103-07

            Remove_completefile();                                                              // 0102-21

            webLoad = true;

            WebDownloading(Settings.Web_Download);
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            webClient.CancelAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)                          // 0106-18
        {
            App.cs_Events.PropertyChanged -= OnPropertyChanged;
            App.cs_Events_Language.PropertyChanged -= OnPropertyChanged_Language;
        }

        private int GetNumberOfFiles(string strDirectory, string strFilter)                     // 0103-03
        {
            int nFiles = Directory.GetFiles(strDirectory, strFilter).Length;

            foreach (String dir in Directory.GetDirectories(strDirectory))
            {
                nFiles += GetNumberOfFiles(dir, strFilter);
            }

            return nFiles;
        }

        private void ShowMessage(string msg, bool ColorRed)
        {
            lblMessage.Content = App.getTextMessages(msg);                                      // 0103-09
            lblMessage.Visibility = Visibility.Visible;

            if (ColorRed)
                lblMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC1212"));   // Red
            else
                lblMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00a5e2"));   // Blue  // was #FF247CD4

            DoEvents();
        }

        private void ShowMessageUSB(string msg, bool ColorRed)
        {
            tbkMessageUSB.Text =  App.getTextMessages(msg);                                     // 0106-18

            lblMessageUSB.Visibility = Visibility.Visible;

            if (ColorRed)
                lblMessageUSB.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC1212"));// Red
            else
                lblMessageUSB.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00a5e2"));// Blue

            DoEvents();
        }
        #endregion

        /// <summary>
        /// Check if directory has been deleted
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static bool isAllBakDirectoryDeleted(string dir)                                // 0106-15
        {
            int r = 0;
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            DirectoryInfo[] dirINFOS = dirinfo.GetDirectories("*.bak");
            foreach (DirectoryInfo d in dirINFOS)
            {
                return false;
            }

            return true;
        }

        private static void RenameAllDirectoryBakToTmp(string dir)                              // 0106-15
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            DirectoryInfo[] dirINFOS = dirinfo.GetDirectories("*.bak");
            foreach (DirectoryInfo d in dirINFOS)
            {
                Directory.Move(d.FullName, d.FullName + "_tmp");
            }
        }

        /// <summary>
        /// Delete all .bak directory, if not delete again and check directory exist 
        /// </summary>
        /// <param name="bakdir"></param>
        /// <returns></returns>
        private static bool AllBakDirectoryDeleted(string bakdir)                               // 0106-15
        {
            try
            {
                for (int i = 0; i < 2; i++)                                                     // do 2 times to make sure file deleted   
                {
                    DeleteAllBakDirectory(bakdir);                                              // 0106-15

                    DoEvents();                                                                 // 0106-15

                    System.Threading.Thread.Sleep(200);

                    // If failed, delete one more time
                    if (isAllBakDirectoryDeleted(bakdir))                                       // 0106-15
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void DeleteAllTmpDirectory(string dir)                                   // 0106-15
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            DirectoryInfo[] dirINFOS = dirinfo.GetDirectories("*_tmp");
            foreach (DirectoryInfo d in dirINFOS)
            {
                DeleteDirectory(d.FullName);
            }
        }

        private static void DeleteAllCheckDirectory(string dir)                                 // 0106-15
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            DirectoryInfo[] dirINFOS = dirinfo.GetDirectories("*_check");
            foreach (DirectoryInfo d in dirINFOS)
            {
                DeleteDirectory(d.FullName);
            }
        }

        /// <summary>
        /// Delete all directory with format ...
        /// </summary>
        private static void DeleteAllBakDirectory(string dir)                                   // 0106-15
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            DirectoryInfo[] dirINFOS = dirinfo.GetDirectories("*.bak");
            foreach (DirectoryInfo d in dirINFOS)
            {
                DeleteDirectory(d.FullName);
            }
        }
        
        #region Delete directory
        /// <summary>
        /// Delete directory and all its sub directory 
        /// </summary>
        /// <param name="pFolderPath"></param>
        private static void DeleteDirectory(string pFolderPath)                                 // 0103-08
        {
            foreach (string Folder in Directory.GetDirectories(pFolderPath))
            {
                DeleteDirectory(Folder);
            }

            foreach (string file in Directory.GetFiles(pFolderPath))
            {
                var pPath = Path.Combine(pFolderPath, file);
                FileInfo fi = new FileInfo(pPath);
                File.SetAttributes(pPath, FileAttributes.Normal);
                File.Delete(file);
            }

            //DeleteFolder(pFolderPath);
            DeleteFolderMethod2(pFolderPath);
        }

        /// <summary>
        /// sub function from DeleteDirectory
        /// </summary>
        /// <param name="dirname"></param>
        private static void DeleteFolder(string dirname)                                        // 0103-08
        {
            try
            {
                var di = new DirectoryInfo(dirname); 
                di.Attributes &= ~FileAttributes.ReadOnly;
                Directory.Delete(dirname, false);
            }
            catch (Exception ex)
            {
                Utility.Lib.Savetoupdatelog(dirname +"-delete directory failed! " + ex.ToString());  // 0103-07
            }
        }

        /// <summary>
        /// Add repeat loop to check folder deleted
        /// </summary>
        /// <param name="dirname"></param>
        private static void DeleteFolderMethod2(string dirname)                                 // 0106-15
        {
            const int attemptcount = 10;
            for (var i = 1; i <= attemptcount; i++)
            {
                try
                {
                    var di = new DirectoryInfo(dirname);
                    di.Attributes &= ~FileAttributes.ReadOnly;
                    Directory.Delete(dirname, false);
                }
                catch (DirectoryNotFoundException)
                {
                    return;  // good!
                }
                catch (IOException)
                { 
                    // System.IO.IOException: The directory is not empty
                    Utility.Lib.Savetoupdatelog(dirname + "-delete directory failed! attempt " + i);  // 0103-07
                   
                    Thread.Sleep(50);
                    continue;
                }
                return;
            }
        }
        #endregion

        #region Check if file locked -not in use 
        private bool CheckIfFileLocked(string pFolderPath)                                      // 0106-18                                                // 0106-17
        {
            foreach (string Folder in Directory.GetDirectories(pFolderPath))
            {
                if (CheckIfFileLocked(Folder))
                    return true;
            }

            foreach (string file in Directory.GetFiles(pFolderPath))
            {
                var pPath = Path.Combine(pFolderPath, file);
                FileInfo fi = new FileInfo(pPath);
                File.SetAttributes(pPath, FileAttributes.Normal);
               
                if(IsFileLocked(file))
                    return true;
            }
            return false;
        }

        private bool IsFileLocked(string filepath)                                              // 0106-18
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(filepath,FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is in use or not exist
                Utility.Lib.SaveErrorLog("File " + filepath + "is in use!");   
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            // file is not locked
            return false;
        }
        #endregion

        #region Check folder in use
        private bool CheckFolderInuse()
        {
            string dirpath = Environment.CurrentDirectory + "\\";

            var dirs = Directory.GetDirectories(dirpath).Where(s => !s.EndsWith("_check") && !s.EndsWith(".bak"));
           
            foreach (string dir in dirs)
            {
                if (FolderInUse(dir))
                {
                    Utility.Lib.SaveErrorLog("Folder " + dir + " is in use!");
                    return true;
                }
            }

            return false;
        }

        private bool FolderInUse(string folder)
        {
            try
            {
                Directory.Move(folder, folder + "_check");

                Directory.Move(folder + "_check", folder);

                return false;
            }
            catch(Exception ex)
            {
                Utility.Lib.SaveErrorLog("Folder " + folder + " issues-"+ex.ToString()); 
                return true;
            }

        }
        #endregion

        private void AddtoUpdateFiles(string filepath, string strdata, bool clearalltext)        // 0103-08
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    System.IO.File.Create(filepath).Close();

                }
                if (clearalltext)
                    File.WriteAllText(filepath, String.Empty, Encoding.ASCII);                  // 0106-09
                else
                    File.AppendAllText(filepath, strdata, Encoding.ASCII);                      // 0106-09
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add to updatelog failed!");
            }
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void btnVersion_Click(object sender, RoutedEventArgs e)                         // 0106-26
        {
            lblVersion.Visibility = lblVersion.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

    }
}
