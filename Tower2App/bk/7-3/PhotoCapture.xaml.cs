using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DirectShowLib;
using JetBrains.Annotations;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;


// For Database
using System.Data;


namespace Edge.Tower2.UI.PhotoCapture
{
    public partial class PhotoCapture : INotifyPropertyChanged
    {
        public PhotoCapture()
        {
            CameraDevice = MultimediaUtil.VideoInputDevices.FirstOrDefault(device => device.Name == Settings.CameraDeviceName);
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

            Page_Init();
        }

        //- sww added
        public void Page_Init()
        {
            // No Next Button here
            //btnNext.Visibility = Visibility.Collapsed;

            // Update Page Title
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                lblTitle.Content = "Profile";
                ControlParams.Params.p_SessionName = "Profile";
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                lblTitle.Content = "Left";
                ControlParams.Params.p_SessionName = "Left";
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                lblTitle.Content = "Right";
                ControlParams.Params.p_SessionName = "Right";
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                lblTitle.Content = "Close UP #1";
                ControlParams.Params.p_SessionName = "CloseUp1";
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                lblTitle.Content = "Close UP #2";
                ControlParams.Params.p_SessionName = "CloseUp2";
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            {
                lblTitle.Content = "Close UP #3";
                ControlParams.Params.p_SessionName = "CloseUp3";
            }
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            VideoCapturePanel.NewVideoSample += videoCapElement_NewVideoSample;
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
            //sww disabled
            //PhotoListView.UnselectAll();

            VideoCapturePanel.Stop();

            Outputs.LogHeader("ImageCapture", "Exit");
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
                _captureRequested = false;

                var bitmap = (Bitmap)e.VideoFrame.Clone();
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                if (!Directory.Exists(Environment.CurrentDirectory + "\\Photos"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Photos");
                }

                //// sww
                // string newFileName;
                // newFileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss.fff") + "_Profile" + ".jpg";
                //// bitmap.Save(Environment.CurrentDirectory + "\\Photos\\" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss.fff") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                // sww 
                string newFileName;

                newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";


                //// New Client or never take a photo
                //if (ControlParams.Params.p_Org_Visit=="")
                //    //newFileName = ControlParams.DBInfo.ID + "_" + "original" + "_" + ControlParams.Params.p_SessionName + ".jpg";
                //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                //else
                //{

                //    //if (ControlParams.Params.p_LastVisit == ControlParams.Params.p_Date_V1 && ControlParams.Params.p_Date_V1 == DateTime.Now.ToString("yyyyMMdd"))
                    //{    // no delete, it is same date

                    //}
                    //else
                    //{   
                    //    // need delete old files 

                    //    // Update database Last Date Parameters
                    //}  

                    //// if this is today's date, it still is new client, will overwrite exist photos
                    //if (ControlParams.Params.p_Org_Visit == DateTime.Now.ToString("yyyyMMdd"))
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //else if (ControlParams.Params.p_LastVisit == DateTime.Now.ToString("yyyyMMdd"))
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //else if (ControlParams.Params.p_Mid_Visit == DateTime.Now.ToString("yyyyMMdd"))
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //else if ( ControlParams.Params.p_Org_Visit =="")
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //else if (ControlParams.Params.p_Last_Visit == "")
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //else if (ControlParams.Params.p_Mid_Visit == "")
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //    // it have date on Last Visit, Mid_Visit, org_Visit, then find the older  one on Mid_Visit and Last_Visit 
                    //else if (Convert.ToDouble(ControlParams.Params.p_Mid_Visit) >= Convert.ToDouble(ControlParams.Params.p_Last_Visit ))
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    //else
                    //{
                    //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
                    //}
                    

                //}

                // bitmap.Save(Environment.CurrentDirectory + "\\Photos\\" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss.fff") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                bitmap.Save(Environment.CurrentDirectory + "\\Photos\\" + newFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                bitmap.Dispose();

                CaptureButton.Dispatcher.Invoke((Action)(() => { CaptureButton.IsEnabled = true; }));
                // sww add
                btnCapture.Dispatcher.Invoke((Action)(() => { btnCapture.IsEnabled = true; }));


                ////- Save To DB ??
                ////Update_PhototoDB(newFileName);
                //if (Search(ControlParams.DBInfo.ID, "V1"))
                //{  
                //    // Do nothing, Because file id is in and only descript need updated
                //    //Update_PhototoDB(string imgFileName);
                //}
                //else
                //    Insert_Description(ControlParams.DBInfo.ID);

            }
        }


        public void DisplayCapturePhoto()
        {

            // Assign photo to 6 image box
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                //ImgProfile.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Skin\\Images\\NoPhoto.jpg"));
                Utility.Lib.LoadImageFile(ImgProfile, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Profile");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                Utility.Lib.LoadImageFile(ImgLeft, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Left");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                Utility.Lib.LoadImageFile(ImgRight, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Right");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp1, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp1");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp2, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp2");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp3, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp3");
            }

        }


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
            // visitType =>V1 ,V2, Org
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "VisitType", VType, 1, 1);  //1= text ,2 =integer 

            //if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //    DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_Profile", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //    DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_Left", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //    DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_Right", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //    DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_CloseUp1", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //    DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_CloseUp2", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //    DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_CloseUp3", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);

            string dt;
            string dt2;
            DateTime date = DateTime.Now;
            DateTime date2 = DateTime.Now;
            dt = date.ToLongTimeString();        // display format:  11:45:44 AM
            dt2 = date2.ToShortDateString();
            string strDateTime = string.Concat(dt2, " ", dt);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Date_Created", strDateTime, 1, 4);

            //DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Date_Create", Now(), 1, 4);

            selectCmd = selectCmd + Command1;
            //- No date in all fields
            if (Command2 == "")
            {
                //MessageBox.Show("Invalid Data for Insert", "New Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                MessageBox.Show("No Data Save!");
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
                MessageBox.Show("Data Insert Failed!");
                c_ID = "";
                return;
            }
            else
            {
                //if (ConfirmMsg)
                //    MessageBox.Show("Data Added Success!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                //ControlParams.DBInfo.ID = c_ID;
            }

            //ClearDetailInfo();
            ////- Update the table view
            //LoadDefaultValue();
            ////- Display apply button
            //buttonEnabled(false);
            ////- Show the last record
            //DisplayDetailInfo(this.dataGridView1.RowCount - 1);

        }

        //- Excute search function for avoid duplication
        private bool Search_PhotoDB1(string c_ID, string VisitType)
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
            //DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", "%" + DB.DBCommend.Quoting(txtTitle.Text.Trim()) + "%", 1,  1); //1 for text ,2 for integer
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", "" + c_ID + "", 1, 2);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitType ", "" + VisitType + "", 1, 2);

            if (selectCmd == "")
            {
                // Show Message Here
                //MessageBox.Show("No Data Found!");
                return false;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;

            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg);
            //this.dataGridView1.DataSource = DV;

            if (DV == null || DV.Count == 0)
            {
                //MessageBox.Show("No Record Found!", "Search Result");
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

        private void CaptureImageClick(object sender, RoutedEventArgs e)
        {
            _captureRequested = true;
            CaptureButton.IsEnabled = false;
        }

        private void DeleteImageClick(object sender, RoutedEventArgs e)
        {
            //sww
            //foreach (Photo photo in PhotoListView.SelectedItems)
            //{
            //    File.Delete(photo.Path);
            //}
        }

        // When ListView Click 
        private void PhotoListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DeleteButton.IsEnabled = PhotoListView.SelectedItems.Count != 0;

            //if (PhotoListView.SelectedItems.Count == 1 || PhotoListView.SelectedItems.Count == 2)
            //{
            //    var path = ((Photo)PhotoListView.SelectedItem).Path;
            //    // load the image, specify CacheOption so the file is not locked
            //    var image = new BitmapImage();
            //    image.BeginInit();
            //    image.CacheOption = BitmapCacheOption.OnLoad;
            //    image.UriSource = new Uri(path);
            //    image.EndInit();

            //    ((PhotoCaptureModel)DataContext).LargeSelectedImage = image;
            //    // sww 
            //    //LargeSavedImage.Source = image;
            //    //LargeSavedImage2.Source = image;
            //    //LargeSavedImage.Visibility = Visibility.Visible;
            //    //LargeSavedImage2.Visibility = Visibility.Visible;
            //    G_LiveImage.Visibility = Visibility.Collapsed;
            //    G_StillImage.Visibility = Visibility.Visible;

            //}

            //if (PhotoListView.SelectedItems.Count == 2)
            //{
            //    var path = ((Photo)PhotoListView.SelectedItems[1]).Path;
            //    // load the image, specify CacheOption so the file is not locked
            //    var image = new BitmapImage();
            //    image.BeginInit();
            //    image.CacheOption = BitmapCacheOption.OnLoad;
            //    image.UriSource = new Uri(path);
            //    image.EndInit();

            //    ((PhotoCaptureModel)DataContext).LargeSelectedImage2 = image;
            //}

            //// sww
            ////VideoCapturePanel.Visibility = (PhotoListView.SelectedItems.Count == 0 || PhotoListView.SelectedItems.Count > 2)
            ////    ? Visibility.Visible : Visibility.Collapsed;

            //LargeSavedImage.Visibility = (PhotoListView.SelectedItems.Count == 1 || PhotoListView.SelectedItems.Count == 2)
            //    ? Visibility.Visible : Visibility.Collapsed;

            //LargeSavedImage2.Visibility = PhotoListView.SelectedItems.Count == 2
            //    ? Visibility.Visible : Visibility.Collapsed;

            //// sww
            ////ImageGrid.RowDefinitions[1].Height = PhotoListView.SelectedItems.Count == 2
            ////    ? new GridLength(1, GridUnitType.Star)
            ////    : new GridLength(0, GridUnitType.Star);

            //if (DeleteTextBlock.IsEnabled)
            //{
            //    DeleteTextBlock.Text = string.Format("Delete {0} Photo{1}",
            //        PhotoListView.SelectedItems.Count,
            //        PhotoListView.SelectedItems.Count > 1 ? "s" : "");
            //}
            //else
            //{
            //    DeleteTextBlock.Text = "Delete Photo(s)";
            //}
        }

        private void UIElement_OnPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //PhotoListView.UnselectAll();
        }

        private void SelectAllClick(object sender, RoutedEventArgs e)
        {
            //PhotoListView.SelectAll();
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            _captureRequested = true;
            btnCapture.IsEnabled = false;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.Photo_Customer_Search);
            //// Update Page Title
            //if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            //{
            //    lblTitle.Content = "Profile";
            //    ControlParams.Params.p_SessionName = "Profile";
            //}
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            //{
            //    lblTitle.Content = "Left";
            //    ControlParams.Params.p_SessionName = "Left";
            //}
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            //{
            //    lblTitle.Content = "Right";
            //    ControlParams.Params.p_SessionName = "PRight";
            //}
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            //{
            //    lblTitle.Content = "Close UP #1";
            //    ControlParams.Params.p_SessionName = "CloseUp1";
            //}
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            //{
            //    lblTitle.Content = "Close UP #2";
            //    ControlParams.Params.p_SessionName = "CloseUp2";
            //}
            //else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            //{
            //    lblTitle.Content = "Close UP #3";
            //    ControlParams.Params.p_SessionName = "CloseUp3";
            //}

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Info)
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
            Page_Init();

        }

        private void btnReflash_Click(object sender, RoutedEventArgs e)
        {
            G_LiveImage.Visibility = Visibility.Visible;
            G_StillImage.Visibility = Visibility.Collapsed;
        }

        private void Update_PhototoDB(string imgFileName)
        {
            string TableName = "Photos";
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            //if (txtID.Text == "")
            //{
            //    //MessageBox.Show("Invalid ID No.!", "New Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            //    return;
            //}

            string ID;
            //- Use current ID
            ID = ControlParams.DBInfo.ID;

            string selectCmd = "";

            selectCmd = "UPDATE " + TableName + " Set ";
            //- 1
            //DB.DBCommend.GetSqlCommand(ref selectCmd, "ID", DB.DBCommend.Quoting(txtID.Text.Trim()), 1,  1);
            //DB.DBCommend.GetSqlCommand(ref selectCmd, "Name", DB.DBCommend.Quoting(txtName.Text.Trim()), 1,  2);
            //DB.DBCommend.GetSqlCommand(ref selectCmd, "Birthday", DB.DBCommend.Quoting(txtBirthday.Text.Trim()), 1,  3);
            //DB.DBCommend.GetSqlCommand(ref selectCmd, "TelNo", DB.DBCommend.Quoting(txtTelNo.Text.Trim()), 1, 4);
            //DB.DBCommend.GetSqlCommand(ref selectCmd, "Notes", DB.DBCommend.Quoting(txtNotes.Text.Trim()), 1, 5);
            // Update Page Title
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "PhotoC", imgFileName, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "PhotoL", imgFileName, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "PhotoR", imgFileName, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "CloseUp1", imgFileName, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "CloseUp2", imgFileName, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
                DB.DBCommend.GetSqlCommand(ref selectCmd, "CloseUp3", imgFileName, 1, 1);


            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' ";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
                //MessageBox.Show(ErrMsg, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                MessageBox.Show("No Data Found!");
            else
            {
                //- It has to call up again to get information update
                //LoadDefaultValue();
                //- Confirm message
                //ControlParams.DBInfo.ID = ID;
            }

        }

        //- Not in use
        //private void UpdateCustomerDB()
        //{

        //    string TableName = "SystemDB";
        //    // If the Last date is today, then do nothing, 
        //    if (ControlParams.Params.p_LastVisit == DateTime.Now.ToString("yyyyMMdd") || ControlParams.Params.p_LastVisit == "")
        //        return;

        //    //- Edit one record of database 
        //    DB.DBLibrary dbLibrary = new DB.DBLibrary();

        //    string Current_ID;
        //    //- Use current ID
        //    Current_ID = ControlParams.DBInfo.ID;

        //    string selectCmd = "";

        //    selectCmd = "UPDATE " + TableName + " Set ";
        //    //- 1
        //    //DB.DBCommend.GetSqlCommand(ref selectCmd, "ID", DB.DBCommend.Quoting(txtID.Text.Trim()), 1,  1);

        //    if (ControlParams.Params.p_LastVisit == ControlParams.Params.p_Date_V1)
        //    {   // Shift V1 to V2
        //        DB.DBCommend.GetSqlCommand(ref selectCmd, "V2", ControlParams.Params.p_Date_V1, 1, 3);
        //    }
        //    DB.DBCommend.GetSqlCommand(ref selectCmd, "V1", DateTime.Now.ToString("yyyyMMdd"), 1, 2);
        //    DB.DBCommend.GetSqlCommand(ref selectCmd, "LstaVisit", DateTime.Now.ToString("yyyyMMdd"), 1, 2);

        //    DB.DBCommend.CutOffStringComma(ref selectCmd);
        //    selectCmd = selectCmd + " Where ID=" + Current_ID + " ";

        //    string ErrMsg = "";
        //    bool R;
        //    R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
        //    if (R == false)
        //        //MessageBox.Show(ErrMsg, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        //        MessageBox.Show("No Data Found!");
        //    else
        //    {
        //        //- It has to call up again to get information update
        //        //LoadDefaultValue();
        //        //- Confirm message

        //        //ControlParams.DBInfo.ID = ID;
        //    }

        //}

        private void Me_Activated(object sender, EventArgs e)
        {
            Page_Init();
            //sww  Load Description
            Search_PhotoDB(ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"));
        }

        private bool ClearDescription_PhotosDB(string VType)
        {
            // If Record exist, do updating otherwise do insert
            // May be when click the edit, will do the clear as well
            string ID = ControlParams.DBInfo.ID;
            if (Search_PhotoDB(ID, VType))
                Update_PhotosDB(VType);
            else
                Insert_PhotoDBVisiting(ID, VType);

            return false;
        }

        // Clear out all description
        private bool Update_PhotosDB(string V1orV2)
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string ID;
            //- Use current ID
            ID = ControlParams.DBInfo.ID;

            string selectCmd = "";

            selectCmd = "UPDATE " + "Photos" + " Set ";
            //- 1
            //DB.DBCommend.GetSqlCommand(ref selectCmd, "ID", DB.DBCommend.Quoting(txtID.Text.Trim()), 1,  1);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "DateCreated", DateTime.Now.ToString("yyyyMMdd"), 1, 1);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_Profile", "", 1, 2);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_Left", "", 1, 3);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_Right", "", 1, 4);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_CloseUp1", "", 1, 5);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_CloseUp2", "", 1, 6);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_CloseUp3", "", 1, 7);

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' and VisitType =" + V1orV2 + "'";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
                //MessageBox.Show("No Data Found!");
                return false;
            else
            {
                //- It has to call up again to get information update
                //LoadDefaultValue();
                //- Confirm message

                //ControlParams.DBInfo.ID = ID;
                return true;
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
            //- 1
            
            DB.DBCommend.GetSqlCommand(ref selectCmd, DBField, newCaptureDate, 1, 2);
            //else
            //    DB.DBCommend.GetSqlCommand(ref selectCmd, "V2", newCaptureDate, 1, 3);

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' ";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
                //MessageBox.Show(ErrMsg, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                //MessageBox.Show("No Data Found!");
                return false;
            else
            {
                //- It has to call up again to get information update
                //LoadDefaultValue();
                //- Confirm message

                //ControlParams.DBInfo.ID = ID;
                return true;
            }
        }

        private bool Update_SystemDB_V1_V2(string Field_V2,string Field_V1, string OldDate,string newCaptureDate)
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string ID;
            //- Use current ID
            ID = ControlParams.DBInfo.ID;

            string selectCmd = "";

            selectCmd = "UPDATE " + "SystemDB" + " Set ";
            //- 1

            DB.DBCommend.GetSqlCommand(ref selectCmd, Field_V2, OldDate, 1, 2);
            DB.DBCommend.GetSqlCommand(ref selectCmd, Field_V1, newCaptureDate, 1, 3);
            //else
            //    DB.DBCommend.GetSqlCommand(ref selectCmd, "V2", newCaptureDate, 1, 3);

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' ";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
                //MessageBox.Show(ErrMsg, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                //MessageBox.Show("No Data Found!");
                return false;
            else
            {
                //- It has to call up again to get information update
                //LoadDefaultValue();
                //- Confirm message

                //ControlParams.DBInfo.ID = ID;
                return true;
            }
        }

        private void CheckVisitDate()
        {   
            if (ControlParams.Params.p_Date_Org == "")
            {   // New Customer
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd"); 
                Update_SystemDB("Org", ControlParams.Params.p_NCaptureDate);
                ControlParams.Params.p_Date_Org = ControlParams.Params.p_NCaptureDate;
                Update_SystemDB("Org", ControlParams.Params.p_NCaptureDate);
            }
            else if (ControlParams.Params.p_Date_V1 == "")  //-Second time visiting
            {
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd");
                Update_SystemDB("V1", ControlParams.Params.p_NCaptureDate);
                ControlParams.Params.p_Date_V1 = ControlParams.Params.p_NCaptureDate;
            }
            else if (ControlParams.Params.p_Date_V1 != "" && ControlParams.Params.p_Date_V1 != DateTime.Now.ToString("yyyyMMdd"))
            {
                ControlParams.Params.p_NCaptureDate = DateTime.Now.ToString("yyyyMMdd"); 
                // Shift date from V1 to V2
                Update_SystemDB_V1_V2("V2", "V2", ControlParams.Params.p_Date_V1,ControlParams.Params.p_NCaptureDate);
                // Update Parameters
                ControlParams.Params.p_Date_V2 = ControlParams.Params.p_Date_V1;
                ControlParams.Params.p_Date_V1 = ControlParams.Params.p_NCaptureDate;
 
            }
            //else
            //{
            //    try
            //    {
            //        if (System.Convert.ToDouble(ControlParams.Params.p_Date_V1) >= System.Convert.ToDouble(ControlParams.Params.p_Date_V2))
            //        {
            //            ControlParams.Params.p_OverwriteVisit = "V2";
            //            Update_SystemDB("V2", ControlParams.Params.p_NCaptureDate);
            //            //Clear Out the old descriptions
            //            Update_PhotosDB("V2");
            //        }
            //        else
            //        {
            //            ControlParams.Params.p_OverwriteVisit = "V1";
            //            Update_SystemDB("V1", ControlParams.Params.p_NCaptureDate);
            //            //Clear Out the old descriptions
            //            ClearDescription_PhotosDB("V1"); //???
            //        }
            //    }
            //    catch
            //    {
            //        MessageBox.Show("Invalid Data conversion");
            //    }

            //    //long l1 = (long)System.Convert.ToDouble("110025");
            //    //if (Convert.ToDouble(ControlParams.Params.p_Date_V1) >= Convert.ToDouble(ControlParams.Params.p_Date_V2))
            //    //{ 

            //    //}

            //}
        }

        // For testing only
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newFileName;

            // New Client or never take a photo
            CheckVisitDate();

            //if (ControlParams.Params.p_Org_Visit == "")
            //    newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //else
            //{

            //    //if (ControlParams.Params.p_LastVisit == ControlParams.Params.p_Date_V1 && ControlParams.Params.p_Date_V1 == DateTime.Now.ToString("yyyyMMdd"))
            //    //{    // no delete, it is same date

            //    //}
            //    //else
            //    //{   
            //    //    // need delete old files 

            //    //    // Update database Last Date Parameters
            //    //}  

            //    // if this is today's date, it still is new client, will overwrite exist photos
            //    if (ControlParams.Params.p_Org_Visit == DateTime.Now.ToString("yyyyMMdd"))
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    else if (ControlParams.Params.p_LastVisit == DateTime.Now.ToString("yyyyMMdd"))
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    else if (ControlParams.Params.p_Mid_Visit == DateTime.Now.ToString("yyyyMMdd"))
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    else if (ControlParams.Params.p_Org_Visit == "")
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    else if (ControlParams.Params.p_Last_Visit == "")
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    else if (ControlParams.Params.p_Mid_Visit == "")
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    // it have date on Last Visit, Mid_Visit, org_Visit, then find the older one on Mid_Visit and Last_Visit 
            //    else if (Convert.ToDouble(ControlParams.Params.p_Mid_Visit) >= Convert.ToDouble(ControlParams.Params.p_Last_Visit))
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //    else
            //    {
            //        newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            //    }
            //}

            newFileName = ControlParams.DBInfo.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + ControlParams.Params.p_SessionName + ".jpg";
            System.IO.File.Copy(Environment.CurrentDirectory + "\\Photos\\1.jpg", Environment.CurrentDirectory + "\\Photos\\" + newFileName, true);

            // bitmap.Save(Environment.CurrentDirectory + "\\Photos\\" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss.fff") + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            //bitmap.Save(Environment.CurrentDirectory + "\\Photos\\" + newFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            //bitmap.Dispose();

            //CaptureButton.Dispatcher.Invoke((Action)(() => { CaptureButton.IsEnabled = true; }));
            //// sww add
            //btnCapture.Dispatcher.Invoke((Action)(() => { btnCapture.IsEnabled = true; }));

            ////- Save To DB ??
            ////Update_PhototoDB(newFileName);


            //if (Search_PhotoDB(ControlParams.DBInfo.ID, "V1")) //??
            //{  
            //    //Because file id is in and only descript need updated
            //    //  Data need add 
            //    //Update_PhototoDB(string imgFileName);
            //}
            //else
            //    Insert_Description(ControlParams.DBInfo.ID);



            // Assign photo to 6 image box
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                //ImgProfile.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Skin\\Images\\NoPhoto.jpg"));
                Utility.Lib.LoadImageFile(ImgProfile, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Profile");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                Utility.Lib.LoadImageFile(ImgLeft, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Left");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                Utility.Lib.LoadImageFile(ImgRight, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Right");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp1, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp1");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp2, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp2");
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
            {
                Utility.Lib.LoadImageFile(ImgCloseUp3, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp3");
            }

        }

        private void LoadPhotos()
        {
            Utility.Lib.LoadImageFile(ImgProfile, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Profile");
            Utility.Lib.LoadImageFile(ImgLeft, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Left");
            Utility.Lib.LoadImageFile(ImgRight, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "Right");
            Utility.Lib.LoadImageFile(ImgCloseUp1, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp1");
            Utility.Lib.LoadImageFile(ImgCloseUp2, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp2");
            Utility.Lib.LoadImageFile(ImgCloseUp3, ControlParams.DBInfo.ID, DateTime.Now.ToString("yyyyMMdd"), "CloseUp3");
        }
        
        //- Excute search function for Photos Description
        private bool Search_PhotoDB(string ID, string Date_String)
        {
            string selectCmd = "";
            string selectCmd1 = "";
            selectCmd = "Select * ";

            string TableName = "Photos";

            //DB.DBCommend.CutOffStringComma(ref  selectCmd);

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            //DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", "%" + DB.DBCommend.Quoting(txtTitle.Text.Trim()) + "%", 1,  1); //1 for text ,2 for integer
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", "" + ID + "", 1, 2);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitDate ", "" + Date_String + "", 1, 3);
        
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
                //MessageBox.Show("No Record Found!", "Search Result");
                //ClearDetailInfo();
                //buttonEnabled(true);
                //tSSL_DataCount.Text = "0";
                return false;
            }

            else
            {
                if (DV.Count == 0)
                {
                    ////MessageBox.Show("Record Not Found", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    //ClearDetailInfo();
                    //buttonEnabled(true);
                    //tSSL_DataCount.Text = DV.Count.ToString();

                    MessageBox.Show("Record Not Found!");
                    return false;
                }
            }

            //Set to binding buffer
            //bs.DataSource = DV;

            //CopyDataToScreen(txtID, (string)DV[0]["ID"].ToString().Trim());
            ////CopyDataToScreen(txtID, (string)DV[0]["Date_Created"].ToString().Trim());
            //CopyDataToScreen(txtName, (string)DV[0]["Name"].ToString().Trim());
            ////CopyDataToScreen(txtBirthday, (string)DV[0]["Birthday"].ToString().Trim());
            //CopyDataToScreen(txtTelNo, (string)DV[0]["TelNo"].ToString().Trim());
            ////CopyDataToScreen(txtNotes, (string)DV[0]["Notes"].ToString().Trim());

            lblProfile.Content = (string)DV[0]["Desc_Profile"].ToString().Trim();
            lblLeft.Content = (string)DV[0]["Desc_Left"].ToString().Trim();
            lblRight.Content = (string)DV[0]["Desc_Right"].ToString().Trim();
            lblCloseUp1.Content = (string)DV[0]["Desc_CloseUp1"].ToString().Trim();
            lblCloseUp2.Content = (string)DV[0]["Desc_CloseUp2"].ToString().Trim();
            lblCloseUp3.Content = (string)DV[0]["Desc_CloseUp3"].ToString().Trim();
                        
            //ScreenMode(ControlParams.e_Session.Profile);
            return true;
        }
    }
        //==================================================================================================================
        // Different Class for Camera
        //==================================================================================================================

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