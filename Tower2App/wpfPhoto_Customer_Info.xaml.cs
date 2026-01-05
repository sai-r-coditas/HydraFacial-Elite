using System;
using System.Windows;
using System.Windows.Input;
using System.Data;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for Photo_Customer_Info.xaml
    /// </summary>
    public partial class Photo_Customer_Info
    {
        #region Loading
        private App App
        {
            get { return (App)Application.Current; }
        }

        public Photo_Customer_Info()
        {
            InitializeComponent();

            // 0103-08
            lblHeader.Content = App.getTextMessages("Profile");
            lblProfileShot.Content = App.getTextMessages("ProfileShot");
            lblLeftShot.Content = App.getTextMessages("Left");
            lblRightShot.Content = App.getTextMessages("Right");
            lblCloseUpShot1.Content = App.getTextMessages("Close Up #1");
            lblCloseUpShot2.Content = App.getTextMessages("Close Up #2");
            lblCloseUpShot3.Content = App.getTextMessages("Close Up #3");

            tbkPastPhotos.Text = App.getTextMessages("Past Photos");
            tbkBack.Text = App.getTextMessages("Back");

            Utility.Lib.LoadImageNoLock(img_Profile, "\\Skin\\Images\\profile_1.png");
            Utility.Lib.LoadImageNoLock(img_Left, "\\Skin\\Images\\profile_2.png");
            Utility.Lib.LoadImageNoLock(img_Right, "\\Skin\\Images\\profile_3.png");
            Utility.Lib.LoadImageNoLock(img_CloseUp1, "\\Skin\\Images\\profile_4.png");
            Utility.Lib.LoadImageNoLock(img_CloseUp2, "\\Skin\\Images\\profile_4.png");
            Utility.Lib.LoadImageNoLock(img_CloseUp3, "\\Skin\\Images\\profile_4.png");

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\WindowBackground_DB.png");   // 0106-05

            page_init();
        }
        #endregion

        #region Init
        private void page_init()
        {

            lblName.Content = ControlParams.DBInfo.Name;

            tbkNotes_Profile.Text = "";
            tbkNotes_Left.Text = "";
            tbkNotes_Right.Text = "";
            tbkNotes_CloseUp1.Text = "";
            tbkNotes_CloseUp2.Text = "";
            tbkNotes_CloseUp3.Text = "";
           
            // Load Today's Image
            LoadCustomerPhotos(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup);  // 0103-06

            Search_PhotoDB(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup); // 0103-06 for description

        }

        //Only Show Today's date
        private void LoadCustomerPhotos(string ID, string Date_String)
        {
            Utility.Lib.LoadImageFile(img_Profile, ID, Date_String, "Profile");
            Utility.Lib.LoadImageFile(img_Left, ID, Date_String, "Left");
            Utility.Lib.LoadImageFile(img_Right, ID, Date_String, "Right");
            Utility.Lib.LoadImageFile(img_CloseUp1, ID, Date_String, "CloseUp1");
            Utility.Lib.LoadImageFile(img_CloseUp2, ID, Date_String, "CloseUp2");
            Utility.Lib.LoadImageFile(img_CloseUp3, ID, Date_String, "CloseUp3");
        }
        #endregion
 
        #region DB Control
        //- Excute search function for Photos Description
        private bool Search_PhotoDB(string ID, string Date_String)
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
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", ID, 3, 2); // match ID only // 1 for text ,2 for integer
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitDate ", Date_String, 3, 3);

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

            tbkNotes_Profile.Text = (string)DV[0]["Desc_Profile"].ToString();
            tbkNotes_Left.Text = (string)DV[0]["Desc_Left"].ToString();
            tbkNotes_Right.Text = (string)DV[0]["Desc_Right"].ToString();
            tbkNotes_CloseUp1.Text = (string)DV[0]["Desc_CloseUp1"].ToString();
            tbkNotes_CloseUp2.Text = (string)DV[0]["Desc_CloseUp2"].ToString();
            tbkNotes_CloseUp3.Text = (string)DV[0]["Desc_CloseUp3"].ToString();
         
            return true;
        }
        #endregion

        #region Image Box Control
        private void img_Profile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Profile;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }

        private void img_Left_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoL;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }

        private void img_Right_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.PhotoR;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }

        private void img_CloseUp1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp1;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }

        private void img_CloseUp2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp2;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }

        private void img_CloseUp3_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.CloseUp3;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }
        #endregion

        private void EditClick(object sender, RoutedEventArgs e)
        {
            //App.Go(Mode.HotCold);

            App.Go(Mode.Photo_Comparison);

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07

            App.Go(Mode.ImageCapture);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            page_init();

            NavBar.setVolume(ControlParams.Params.p_AudioVolume);
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }
    }
}
