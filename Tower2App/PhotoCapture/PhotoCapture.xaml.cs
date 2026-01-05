using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DirectShowLib;
using JetBrains.Annotations;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;

// For Database
using System.Data;

// For USB Button
using System.Windows.Input;

// For SolidColorBrush
using System.Windows.Media;

// For DispatcherPriority
using System.Windows.Threading;

namespace Edge.Tower2.UI.PhotoCapture
{
    public partial class PhotoCapture : INotifyPropertyChanged
    {

        VideoCaptureElement VideoCapturePanel;
        
        public PhotoCapture()
        {
            CameraDevice = MultimediaUtil.VideoInputDevices.FirstOrDefault(device => device.Name == Settings.CameraDeviceName);
            
            // 2014 11/30
            DataContext = new PhotoCaptureModel(this);

            InitializeComponent();
            
            Loaded += Window1_Loaded;

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            camera_init();
            page_init();

            btnTest.Visibility = Visibility.Hidden;

            lblTitle.Content=App.getTextMessages("Profile");                                    // 0103-09

            lblProfileShot.Content = App.getTextMessages("ProfileShot");
            lblLeftShot.Content = App.getTextMessages("Left");
            lblRightShot.Content = App.getTextMessages("Right");
            lblCloseUpShot1.Content = App.getTextMessages("Close Up #1");
            lblCloseUpShot2.Content = App.getTextMessages("Close Up #2");
            lblCloseUpShot3.Content = App.getTextMessages("Close Up #3");

            tbkCapture.Text = App.getTextMessages("Capture");
            tbkEdit.Text = App.getTextMessages("Notes");
            tbkReflash.Text = App.getTextMessages("Reflash");
            tbkBack.Text = App.getTextMessages("Back");
            tbkNext.Text = App.getTextMessages("Next");

            Utility.Lib.LoadImageNoLock(ImgProfile, "\\Skin\\Images\\profile_1.png");
            Utility.Lib.LoadImageNoLock(ImgLeft, "\\Skin\\Images\\profile_2.png");
            Utility.Lib.LoadImageNoLock(ImgRight, "\\Skin\\Images\\profile_3.png");
            Utility.Lib.LoadImageNoLock(ImgCloseUp1, "\\Skin\\Images\\profile_4.png");
            Utility.Lib.LoadImageNoLock(ImgCloseUp2, "\\Skin\\Images\\profile_4.png");
            Utility.Lib.LoadImageNoLock(ImgCloseUp3, "\\Skin\\Images\\profile_4.png");

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\WindowBackground_DB.png");      // 0106-05
        }

        Thickness Thick_2 = new Thickness(2.0);

        //- sww added
        #region Page Init
        public void page_init()  // 0103-07
        {
            var bc = new BrushConverter();

            try
            {
                bdrProfile.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                bdrLeft.BorderBrush = bdrProfile.BorderBrush;
                bdrRight.BorderBrush = bdrProfile.BorderBrush;
                bdrCloseUp1.BorderBrush = bdrProfile.BorderBrush;
                bdrCloseUp2.BorderBrush = bdrProfile.BorderBrush;
                bdrCloseUp3.BorderBrush = bdrProfile.BorderBrush;

                bdrProfile.BorderThickness = new Thickness(2.0);
                bdrLeft.BorderThickness = bdrProfile.BorderThickness;
                bdrRight.BorderThickness = bdrProfile.BorderThickness;
                bdrCloseUp1.BorderThickness = bdrProfile.BorderThickness;
                bdrCloseUp2.BorderThickness = bdrProfile.BorderThickness;
                bdrCloseUp3.BorderThickness = bdrProfile.BorderThickness;

                CameraDevice =
                    MultimediaUtil.VideoInputDevices.FirstOrDefault(device => device.Name == Settings.CameraDeviceName);

                if (CameraDevice != null)                                                       // 0102-39
                    Utility.Lib.SaveErrorLog("Found device-" + CameraDevice.Name.ToString());   // 0103-09
                else
                    Utility.Lib.SaveErrorLog("Camera device is null");                          // 0103-09
  
                // 2014 11/30
                DataContext = new PhotoCaptureModel(this);

                VideoCapturePanel.Play();

                // Update Page Title
                if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Create ||
                    ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Search ||
                    ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Edit)  // 0103-07
                {
                    // Need clear last captures
                    Clear_LastAllCaptures();

                    if (ControlParams.DBInfo.ID != "" && ControlParams.DBInfo.ID != null)       // 0103-06
                    {
                        LoadCustomerPhotos(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup);          // get photos

                        Search_DBPhotoDescription(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup);   // get photo description
                    }
 
                    ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Profile;
                }
               
                if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Create)
                {
                    lblCustomerName.Content = "";
                    ControlParams.Params.p_SessionName = "Information";
                }
                else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
                {
                    lblCustomerName.Content = ControlParams.DBInfo.Name;
                    ControlParams.Params.p_SessionName = "Profile";
                    bdrProfile.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                    bdrProfile.BorderThickness = new Thickness(6.0);
                }
                else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
                {
                    lblCustomerName.Content = ControlParams.DBInfo.Name;
                    ControlParams.Params.p_SessionName = "Left";
                    bdrLeft.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                    bdrLeft.BorderThickness = new Thickness(6.0);
                }
                else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
                {
                    lblCustomerName.Content = ControlParams.DBInfo.Name;
                    ControlParams.Params.p_SessionName = "Right";
                    bdrRight.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                    bdrRight.BorderThickness = new Thickness(6.0);
                }
                else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
                {
                    lblCustomerName.Content = ControlParams.DBInfo.Name;
                    ControlParams.Params.p_SessionName = "CloseUp1";
                    bdrCloseUp1.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                    bdrCloseUp1.BorderThickness = new Thickness(6.0);
                }
                else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
                {
                    lblCustomerName.Content = ControlParams.DBInfo.Name;
                    ControlParams.Params.p_SessionName = "CloseUp2";
                    bdrCloseUp2.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                    bdrCloseUp2.BorderThickness = new Thickness(6.0);
                }
                else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
                {
                    lblCustomerName.Content = ControlParams.DBInfo.Name;
                    ControlParams.Params.p_SessionName = "CloseUp3";
                    bdrCloseUp3.BorderBrush = (System.Windows.Media.Brush) bc.ConvertFrom("#FFb3aa9c");
                    bdrCloseUp3.BorderThickness = new Thickness(6.0);
                }

                //sww  Load Description
                Search_DBPhotoDescription(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup);  // ??? May be duplicated 

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Only Show Today's photo if have any
        private void LoadCustomerPhotos(string ID, string Date_String)
        {
            Utility.Lib.LoadImageFile(ImgProfile, ID, Date_String, "Profile");
            Utility.Lib.LoadImageFile(ImgLeft, ID, Date_String, "Left");
            Utility.Lib.LoadImageFile(ImgRight, ID, Date_String, "Right");
            Utility.Lib.LoadImageFile(ImgCloseUp1, ID, Date_String, "CloseUp1");
            Utility.Lib.LoadImageFile(ImgCloseUp2, ID, Date_String, "CloseUp2");
            Utility.Lib.LoadImageFile(ImgCloseUp3, ID, Date_String, "CloseUp3");
        }
        #endregion

        #region Camera Init
        private void camera_init()
        {
            //sww  For initialize Camera 

            if (VideoCapturePanel != null)
            {
                VideoCapturePanel.NewVideoSample -= videoCapElement_NewVideoSample;
                VideoCapturePanel.Close();
            }

            VideoCapturePanel =  new VideoCaptureElement();

            VideoCapturePanel.BeginInit();
            VideoCapturePanel.DesiredPixelHeight = 1080;                                        // 720;
            VideoCapturePanel.DesiredPixelWidth = 1920;                                         // 1280;

            VideoCapturePanel.FPS = 30;

            if (WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputNames.Length > 0)
            {
                CameraDevice = MultimediaUtil.VideoInputDevices.FirstOrDefault(device => device.Name == Settings.CameraDeviceName);
                
                // 2014 11/30
                //DataContext = new PhotoCaptureModel(this);

                if (CameraDevice !=null)                                                        // 0102-39
                    Utility.Lib.SaveErrorLog("Init again on-" + CameraDevice.Name.ToString());  // 0103-09

                VideoCapturePanel.VideoCaptureDevice = WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices[0];
            }

            VideoCapturePanel.EnableSampleGrabbing = true;
            VideoCapturePanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VideoCapturePanel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        
            this.VideoCapturePanel.NewVideoSample += new System.EventHandler<WPFMediaKit.DirectShow.MediaPlayers.VideoSampleArgs>(this.videoCapElement_NewVideoSample);
            
            VideoCapturePanel.EndInit();

            // 2014 11/30
            G_LiveImage.Children.Insert(0,VideoCapturePanel);

            System.Threading.Thread.Sleep(200);
        }
        #endregion

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
        
        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        private void OnEnter()
        {
            Outputs.LogHeader("ImageCapture", "Enter");
            VideoCapturePanel.Play();
        }

        private void OnLeave()
        {
            VideoCapturePanel.Stop();
          
            Outputs.LogHeader("ImageCapture", "Exit");

            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public DsDevice CameraDevice { get; private set; }

        private bool _captureRequested;

        private void videoCapElement_NewVideoSample(object sender, VideoSampleArgs e)
        {
            if (_captureRequested)
            {
                _captureRequested = false; // It has to set to false here 

                try
                {
                    var bitmap = (Bitmap)e.VideoFrame.Clone();

                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    if (!Directory.Exists(ControlParams.Params.Photos_Default))
                    {
                        Directory.CreateDirectory(ControlParams.Params.Photos_Default);
                    }

                    string newFileName;
                    newFileName = ControlParams.Params.Photos_Default + "\\" + ControlParams.DBInfo.ID + "_" + ControlParams.Params.p_SelectedGroup + "_" + ControlParams.Params.p_SessionName + ".edge";  // 0103-06

                    if (File.Exists(newFileName))
                    {
                        //- Delete the exist file
                        File.Delete(newFileName);
                    }

                    bitmap.Save(newFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap.Dispose();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                 
                //======================================================================

                GC.Collect(2);
                GC.WaitForPendingFinalizers();
                GC.Collect(2);
 
                // add by sww
                System.Threading.Thread.Sleep(380);
                btnCapture.Dispatcher.Invoke((Action)(CaptureProgressDone));
                
            }
        }

        private void CaptureProgressDone()
        {
            // 2014 12/02
            DisplayCapturePhoto(ControlParams.Params.p_SelectedGroup);

            btnCapture.IsEnabled = true;
            btnNext.IsEnabled = true;
            AllButtonEnabled(true);
        }

        public void DisplayCapturePhoto(string Date_string)
        {
            // Assign photo to 6 image box
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                Utility.Lib.LoadImageFile(ImgProfile, ControlParams.DBInfo.ID, Date_string, "Profile");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                Utility.Lib.LoadImageFile(ImgLeft, ControlParams.DBInfo.ID, Date_string, "Left");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                Utility.Lib.LoadImageFile(ImgRight, ControlParams.DBInfo.ID, Date_string, "Right");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp1, ControlParams.DBInfo.ID, Date_string, "CloseUp1");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp2, ControlParams.DBInfo.ID, Date_string, "CloseUp2");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp3, ControlParams.DBInfo.ID, Date_string, "CloseUp3");
            }

            // If PhotoDB record exist, do nothing , otherwise do DB insert
            if (Search_DBPhotoDescription(ControlParams.DBInfo.ID, Date_string))
            {
                // do nothing
            }
            else
                Insert_PhotoDBVisiting(ControlParams.DBInfo.ID, Date_string);
        }

        private void Clear_LastAllCaptures()
        {
            Utility.Lib.LoadImageFile(ImgProfile, "", "", "Profile");
            Utility.Lib.LoadImageFile(ImgLeft, "", "", "Left");
            Utility.Lib.LoadImageFile(ImgRight, "", "", "Right");
            Utility.Lib.LoadImageFile(ImgCloseUp1, "", "", "CloseUp1");
            Utility.Lib.LoadImageFile(ImgCloseUp2, "", "", "CloseUp2");
            Utility.Lib.LoadImageFile(ImgCloseUp3, "", "", "CloseUp3");
            lblProfile.Text = "";
            lblLeft.Text = "";
            lblRight.Text = "";
            lblCloseUp1.Text = "";
            lblCloseUp2.Text = "";
            lblCloseUp3.Text = "";
        }

        #region DB Insert
        //- Insert single record to database
        private void Insert_PhotoDBVisiting(string c_ID, string VType)
        {
            string TableName = "Photos";
            //- Insert Data
            string selectCmd = "";
            string Command1 = "";
            string Command2 = "";

            selectCmd = "Insert Into " + TableName + " (";

            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "ID", c_ID, 1, 1);  //1= text ,2 =integer 
            
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "VisitDate", VType, 1, 1);  //1= text ,2 =integer 

            string dt;
            string dt2;
            DateTime date = DateTime.Now;
            DateTime date2 = DateTime.Now;
            dt = date.ToLongTimeString();        // display format:  11:45:44 AM
            dt2 = date2.ToShortDateString();
            string strDateTime = string.Concat(dt2, " ", dt);
            
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Date_Created", strDateTime, 1, 4);
           
            selectCmd = selectCmd + Command1;
            
            //- No date in all fields
            if (Command2 == "")
            {
                MessageBox.Show("Failed to Create Data!");
                return;
            }

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + ") Values (" + Command2;
            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + ")";

            string ErrMsg = "";
            bool R;
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            R = dbLibrary.InsertData_sw(selectCmd, ref ErrMsg);

            if (R == false)
            {
                MessageBox.Show("Failed to Insert Data!");
                c_ID = "";
                return;
            }
            else
            {
                // do nothing
            }
        }
        #endregion

        //- Excute search function
        private bool Search(string c_ID, string VisitType)
        {

            string selectCmd = "";
            string selectCmd1 = "";
            selectCmd = "Select * ";

            string TableName = "Photos";

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", "" + c_ID + "", 1, 2);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitType ", "" + VisitType + "", 1, 2);
 
            if (selectCmd == "")
            {
                return false;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;

            //- Check for order field
            string OrderCmd = "";

            //- Cut off comma
            DB.DBCommend.CutOffStringComma(ref  OrderCmd);

            if (OrderCmd != "")
                selectCmd = selectCmd + " Order by " + OrderCmd;                                //- TITLE ASC, ARTIST ASC, FILE ASC ";

            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg);
          
            if (DV == null || DV.Count == 0)
            {
                 return false;
            }

            else
            {
                if (DV.Count == 0)
                {
                    return false;
                }
            }
       
            return true;
        }

        private void UIElement_OnPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputNames.Length < 1)
            {
                MessageBox.Show(App.getTextMessages("Invalid Photo Capture Device!"));          // 0103-09
                return;
            }
              
            CaptureButtonClick();
        }

        private void CaptureButtonClick()
        {
            btnCapture.IsEnabled = false;

            //Add by sww
            btnNext.IsEnabled = false;

            _captureRequested = true;

            AllButtonEnabled(false);
        }

        private void AllButtonEnabled(bool Mode)
        {
            grd_L1_root1.IsEnabled = Mode;

            btnCapture.IsEnabled = Mode;

            btnNext.IsEnabled = Mode;

            btnEdit.IsEnabled = Mode;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Enable save button
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.InfoSave;

            App.Go(Mode.Photo_Customer_Search);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Create)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Profile;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoL;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoR;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp1;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp2;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp3;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            {
                //--Show All 6 Photos
                App.Go(Mode.Photo_Customer_Info);
                return;
            }
            //Screen Updating
            page_init();
        }

        private bool CheckCameraExist(string str)
        {
            System.Management.ManagementObjectSearcher search = default(System.Management.ManagementObjectSearcher);
            string deviceName = null;
            search = new System.Management.ManagementObjectSearcher("SELECT * From Win32_PnPEntity");
            string s;
            foreach (System.Management.ManagementObject inf in search.Get())
            {
                deviceName = Convert.ToString(inf["Caption"]);

                if (deviceName == str) 
                    return true;
            }
          
            return false;
         }

        private void btnReflash_Click(object sender, RoutedEventArgs e)
        {
            // 2014 12/02
            if (btnCapture.IsEnabled == true)
            {
                btnCapture.IsEnabled = false;

                // 2014 11/30
                btnReflash.IsEnabled = false;

                G_LiveImage.Visibility = Visibility.Visible;
                G_StillImage.Visibility = Visibility.Collapsed;

                camera_init();

                //// 2014 12/02
                //GC.Collect(2);
                //GC.WaitForPendingFinalizers();
                //GC.Collect(2);

                // 2014 11/30
                btnReflash.IsEnabled = true;
                btnCapture.IsEnabled = true;
             }
        }
 
        private bool Update_SystemDB(string DBField, string newCaptureDate)
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string ID;
            //- Use current ID
            ID = ControlParams.DBInfo.ID;

            string selectCmd = "";

            selectCmd = "UPDATE " + "SystemDB" + " Set ";

            DB.DBCommend.GetSqlCommand(ref selectCmd, DBField, newCaptureDate, 1, 2);
 
            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' ";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
            {   
                MessageBox.Show("Update Failed!");
                return false;
            }
            else
            {
                //- It has to call up again to get information update
                //LoadDefaultValue();
                //- Confirm message
                
                return true;
            }
        }

        private bool Update_SystemDB_V1_V2(string Field_V2, string Field_V1, string V2_Date, string V1_Date)
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string ID;
            //- Use current ID
            ID = ControlParams.DBInfo.ID;

            string selectCmd = "";

            selectCmd = "UPDATE " + "SystemDB" + " Set ";
            //- 1

            DB.DBCommend.GetSqlCommand(ref selectCmd, Field_V2, V2_Date, 1, 2);
            DB.DBCommend.GetSqlCommand(ref selectCmd, Field_V1, V1_Date, 1, 3);
          
            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' ";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
                return false;
            else
            {
             
                return true;
            }
        }

        public void NewPhotoGroup_Verify()  // 0103-07
        {
            // org -- V2 -- V1
            // Check org date first, if is null, it is first time visiter, no pop up question to ask
            if (ControlParams.Params.p_Date_Org == "" || ControlParams.Params.p_Date_Org == null)
            {   
                // New Customer
                ControlParams.Params.p_NCaptureDate = "0";

                ControlParams.Params.p_SelectedGroup = "0";

                Update_SystemDB("Org", ControlParams.Params.p_NCaptureDate);

                ControlParams.Params.p_Date_Org = ControlParams.Params.p_NCaptureDate;

                lblTitle.Content = App.getTextMessages("Profile - (Original)");                 // 0103-09
                
                // May need delete exist file here?

                Clear_LastAllCaptures();                                                        // 0103-06
            }
            
            //-Second time visiting or after first treatment
            //-When ControlParams.Params.p_Date_Org != today &&  != null
            else if (ControlParams.Params.p_Date_V2 == "" || ControlParams.Params.p_Date_V2 == null)
            {
                App.wpfyesno.WarningMessage(App.getTextMessages("Create new group photos, are you sure?"));  // 0103-09
                App.wpfyesno.ShowDialog();
                var ans = App.wpfyesno.ans;
                if (ans != "yes")
                {
                    // Load cureent photos
                    ControlParams.Params.p_SelectedGroup = "0";                                 // 0103-10
                    return;
                }
                
                ControlParams.Params.p_NCaptureDate = "G1";
                Update_SystemDB("V2", ControlParams.Params.p_NCaptureDate);
                ControlParams.Params.p_Date_V2 = ControlParams.Params.p_NCaptureDate;
                ControlParams.Params.p_SelectedGroup = ControlParams.Params.p_Date_V2;

                lblTitle.Content = App.getTextMessages("Profile - (Current)");                  // 0103-09
            }

            //-Second time visiting at same date
            else if (ControlParams.Params.p_Date_V2 != "" && ControlParams.Params.p_Date_V2 != null)
            {

                if (ControlParams.Params.p_Date_V1 != "" && ControlParams.Params.p_Date_V1 != null)
                {
                    App.wpfyesno.WarningMessage(App.getTextMessages("Create new group photos the previous photos will be removed, are you sure?"));  // 0103-09
                    App.wpfyesno.ShowDialog();
                    var ans = App.wpfyesno.ans;
                    
                    if (ans != "yes")
                    {
                        // Load cureent photos
                        ControlParams.Params.p_SelectedGroup = ControlParams.Params.p_Date_V2;  // 0103-10
                        return;
                    }

                    // Delete files
                    // if second time visit is not today, do shifting,
                    ControlParams.Params.p_NCaptureDate = ControlParams.Params.p_Date_V2;

                    // Delete Old Date Photos Here
                    Delete_Old_Photos(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V1);
                  
                }
                else
                {
                    App.wpfyesno.WarningMessage(App.getTextMessages("Create new group photos, are you sure?"));   // 0103-09
                    App.wpfyesno.ShowDialog();
                    
                    var ans = App.wpfyesno.ans;
                    if (ans != "yes")
                    {
                        ControlParams.Params.p_SelectedGroup = ControlParams.Params.p_Date_V2;  // 0103-10
                        return;
                    }
                }

                ControlParams.Params.p_Date_V1 = ControlParams.Params.p_Date_V2;
                if (ControlParams.Params.p_Date_V2 == "G2")
                {
                    ControlParams.Params.p_NCaptureDate = "G1";
                    ControlParams.Params.p_Date_V2 = "G1";
                }
                else
                {
                    ControlParams.Params.p_NCaptureDate = "G2";
                    ControlParams.Params.p_Date_V2 = "G2";
                }

                Update_SystemDB_V1_V2("V2", "V1", ControlParams.Params.p_Date_V2, ControlParams.Params.p_Date_V1);
                ControlParams.Params.p_SelectedGroup = ControlParams.Params.p_Date_V2;

                lblTitle.Content =  App.getTextMessages("Profile - (Current)");                 // 0103-09
            }

            Clear_LastAllCaptures();                                                            // 0103-06
        }

        #region CheckVisitDate Bk
        private void CheckVisitDate_bk()
        {
            // org -- V2 -- V1
            // Check org date first, if is null, it is first time visiter.
            if (ControlParams.Params.p_Date_Org == "" || ControlParams.Params.p_Date_Org == null)
            {   // New Customer
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd");

                Update_SystemDB("Org", ControlParams.Params.p_NCaptureDate);
                ControlParams.Params.p_Date_Org = ControlParams.Params.p_NCaptureDate;
                // May need delete exist file here?

            }
            // Same date, first time visiting
            else if (ControlParams.Params.p_Date_Org == "0")
            {
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd");

            }

            //-Second time visiting
            //-When ControlParams.Params.p_Date_Org != today &&  != null
            else if (ControlParams.Params.p_Date_V1 == "" || ControlParams.Params.p_Date_V1 == null)
            {
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd");
                Update_SystemDB("V1", ControlParams.Params.p_NCaptureDate);
                ControlParams.Params.p_Date_V1 = ControlParams.Params.p_NCaptureDate;
            }
            //-Second time visiting at same date
            else if (ControlParams.Params.p_Date_V1 == DateTime.Now.ToString("yyyyMMdd"))
            {
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd");


            }
            // Check if is multiple time visiting, if it is not the second time visiting
            else if (ControlParams.Params.p_Date_V1 != "" && ControlParams.Params.p_Date_V1 != null &&
                ControlParams.Params.p_Date_V1 != DateTime.Now.ToString("yyyyMMdd"))
            {
                // if second time visit is not today, do shifting,
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd");
                // Delete Old Date Photos Here
                Delete_Old_Photos(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V2);
                // Shift date from V1 to V2
                Update_SystemDB_V1_V2("V2", "V1", ControlParams.Params.p_Date_V1, ControlParams.Params.p_NCaptureDate);
                // Update Parameters
                ControlParams.Params.p_Date_V2 = ControlParams.Params.p_Date_V1;
                ControlParams.Params.p_Date_V1 = ControlParams.Params.p_NCaptureDate;
            }
        }
        #endregion

        private void Delete_Old_Photos(string ID, string OldDate)
        { 
            string OldFiles;
            try
            {
                OldFiles = ControlParams.DBInfo.ID + "_" + OldDate + "_" + "*.edge";

                foreach (FileInfo f in new DirectoryInfo(ControlParams.Params.Photos_Default+"\\").GetFiles(OldFiles))
                {
                    f.Delete();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            NavBar.sldVolume.Value = ControlParams.Params.p_AudioVolume;
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
           NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }

        #region Search
        //- Excute search function for Photos Description
        private bool Search_DBPhotoDescription(string ID, string Date_String)
        {
            string selectCmd = "";
            string selectCmd1 = "";
            selectCmd = "Select * ";

            string TableName = "Photos";

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ",  ID , 3, 2); // match ID only
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitDate ",  Date_String , 3, 3);
        
            if (selectCmd == "")
            {
                // Show Message Here
                MessageBox.Show("No Data Found!");
                return false;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;

            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg);

            if (DV == null || DV.Count == 0)
            {
                 return false;
            }

            else
            {
                if (DV.Count == 0)
                {
                    return false;
                }
            }

            lblProfile.Text = (string)DV[0]["Desc_Profile"].ToString();
            lblLeft.Text = (string)DV[0]["Desc_Left"].ToString();
            lblRight.Text = (string)DV[0]["Desc_Right"].ToString();
            lblCloseUp1.Text = (string)DV[0]["Desc_CloseUp1"].ToString();
            lblCloseUp2.Text = (string)DV[0]["Desc_CloseUp2"].ToString();
            lblCloseUp3.Text = (string)DV[0]["Desc_CloseUp3"].ToString();
          
            return true;
        }
        #endregion

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
           
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Create)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Edit;       // 0103-07
                App.Go(Mode.Photo_Customer_Search);
                return;                                                                         // 2014-07-01
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Edit;       // 0103-07
                ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Search;
                
                App.Go(Mode.Photo_Customer_Search);
                return;                                                                         // 2014-07-01
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Profile;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoL;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoR;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp1;
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            {
                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp2;
            }
            page_init();
        }

        private void ImgProfile_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Profile;
            page_init();
        }

        private void ImgLeft_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoL;
            page_init();
        }

        private void ImgRight_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoR;
            page_init();
        }

        private void ImgCloseUp1_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp1;
            page_init();
        }

        private void ImgCloseUp2_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp2;
            page_init();
        }

        private void ImgCloseUp3_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp3;
            page_init();
        }

        private void Me_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile ||
                ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL ||
                ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR ||
                ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1 ||
                ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2 ||
                ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3
                )
            {
                if (e.Key == Key.Escape)
                {
                    if (btnCapture.IsEnabled == true) ;
                    {
                        CaptureButtonClick();
                        DoEvents();
                    }
                }
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            
            // sww 
            string newFileName;
            newFileName = ControlParams.Params.Photos_Default+"\\" + ControlParams.DBInfo.ID + "_" + ControlParams.Params.p_SelectedGroup + "_" + ControlParams.Params.p_SessionName + ".edge";
            
            string SourceFile;
            SourceFile = Environment.CurrentDirectory + "\\edgephotos\\default\\1.edge";  // 0103-05

            if (File.Exists(SourceFile))
            {
                System.IO.File.Copy(SourceFile, newFileName, true);

            }

            DisplayCapturePhoto(ControlParams.Params.p_SelectedGroup);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void Me_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Me_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 2014 12/12
            if (VideoCapturePanel != null)
               VideoCapturePanel.NewVideoSample -= videoCapElement_NewVideoSample;
        }
    }

    //============================================================
    //============================================================

    public class ImageCacheConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {

            var path = (string)value;
            
            // load the image, specify CacheOption so the file is not locked
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();

            return image;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not implemented.");
        }
    }
}