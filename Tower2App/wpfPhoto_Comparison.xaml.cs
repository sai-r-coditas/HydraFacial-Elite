using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

// For Database
using System.Data;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for Photo_Comparison.xaml
    /// </summary>
    public partial class Photo_Comparison
    {

        #region Loading
        //[NotNull]             //sww disabled
        private App App
        {
            get { return (App)Application.Current; }
        }

        //- sww
        public string Date_V1 { get; set; }
        public string Date_V2 { get; set; }
        public string DateSelected { get; set; }
 
        System.Windows.Controls.Image srcImage;

        TextBlock SelectObject;
        Label SelectPositonShot;

        private Point origin1;
        private Point origin2;
        private Point start1;
        private Point start2;
      
        private Point pos1;
        private Point pos2;
        private Matrix mm1;
        private Matrix mm2;

        private Matrix mm_default;
        
        private Point MaxMouseMove1;
        private Point MinMouseMove1;
        private Point MaxMouseMove2;
        private Point MinMouseMove2;

        public Photo_Comparison()
        {
            InitializeComponent();

            page_init();

            mm_default = Img1.RenderTransform.Value;

            MinMouseMove1.X = -450.0;  MaxMouseMove1.X = 450.0;
            MinMouseMove1.Y = -250.0;  MaxMouseMove1.Y = 250.0;

            MinMouseMove2.X = -450.0;  MaxMouseMove2.X = 450.0;
            MinMouseMove2.Y = -250.0;  MaxMouseMove2.Y = 250.0;

            // 0103-08
            lblHeader.Content = App.getTextMessages("Profile");
            lblProfileShot.Content = App.getTextMessages("ProfileShot");
            lblLeftShot.Content = App.getTextMessages("Left");
            lblRightShot.Content = App.getTextMessages("Right");
            lblCloseUpShot1.Content = App.getTextMessages("Close Up #1");
            lblCloseUpShot2.Content = App.getTextMessages("Close Up #2");
            lblCloseUpShot3.Content = App.getTextMessages("Close Up #3");

            tbkOrg.Text = App.getTextMessages("Original");
            tbkMVisit.Text = App.getTextMessages("Previous");
            tbkLastVisit.Text = App.getTextMessages("Current");
            tbkBack.Text = App.getTextMessages("Back");

            Utility.Lib.LoadImageNoLock(ImgProfile, "\\Skin\\Images\\profile_1.png");
            Utility.Lib.LoadImageNoLock(ImgLeft, "\\Skin\\Images\\profile_2.png");
            Utility.Lib.LoadImageNoLock(ImgRight, "\\Skin\\Images\\profile_3.png");
            Utility.Lib.LoadImageNoLock(ImgCloseUp1, "\\Skin\\Images\\profile_4.png");
            Utility.Lib.LoadImageNoLock(ImgCloseUp2, "\\Skin\\Images\\profile_4.png");
            Utility.Lib.LoadImageNoLock(ImgCloseUp3, "\\Skin\\Images\\profile_4.png");

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\WindowBackground_DB.png");   // 0106-05
        }
        #endregion

        #region Init
        public void page_init()
        {

            lblTitle.Content = ControlParams.DBInfo.Name;

            DateSelected = "";
            lblPositionShot1.Content = "";
            lblPositionShot2.Content = "";
            tbkImg1.Text = "";
            tbkImg2.Text = "";

            // Fill with noPhoto image first
            Utility.Lib.LoadImageFile(Img2, "000", "", "--");
            Utility.Lib.LoadImageFile(Img1, "000", "", "--");

            // Paint selecting buttons
            ButtonBrush(btnOrg, "0");
            ButtonBrush(btnLastVisit, "0");
            ButtonBrush(btnMVisit, "0");

            // Display Date on Buttons
            ShowDateOnButton(btnLastVisit, ControlParams.Params.p_Date_V1);
            ShowDateOnButton(btnMVisit, ControlParams.Params.p_Date_V2);
            ShowDateOnButton(btnOrg, ControlParams.Params.p_Date_Org);

            //Load Last Visit of photos
            if (ControlParams.Params.p_Date_V2 == "" || ControlParams.Params.p_Date_V2 == null)
            {
                if (ControlParams.Params.p_Date_Org == "" || ControlParams.Params.p_Date_Org == null)
                {
                    // Clear description boxes, because new client
                    ClearDescription();
                    LoadPhotos_byVisit(ControlParams.DBInfo.ID, ""); // Load photos icons
                }
                else
                {
                    LoadPhotos_byVisit(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_Org);

                    if (ControlParams.Params.p_Date_Org == "" || ControlParams.Params.p_Date_Org == null)
                        ClearDescription();
                    else
                        Search_PhotoDB(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_Org);

                    DateSelected = btnOrg.Tag.ToString();  // 0102-38
                    HighlightButton(btnOrg);
                    ButtonBrush(btnOrg, "1");
                }
            }
            else
            {
                LoadPhotos_byVisit(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V2);

                if (ControlParams.Params.p_Date_V2 == "" || ControlParams.Params.p_Date_V2 == null)
                    ClearDescription();
                else
                    Search_PhotoDB(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V2);

                DateSelected = btnLastVisit.Tag.ToString();  //0102-38
                HighlightButton(btnLastVisit);
                ButtonBrush(btnLastVisit, "1");
            }
        }

        private void LoadPhotos_byVisit(string ID, string VisitDate)
        {
            Utility.Lib.LoadImageFile_wTag(ref ImgProfile, ID, VisitDate, "Profile");
            Utility.Lib.LoadImageFile_wTag(ref ImgLeft, ID, VisitDate, "Left");
            Utility.Lib.LoadImageFile_wTag(ref ImgRight, ID, VisitDate, "Right");
            Utility.Lib.LoadImageFile_wTag(ref ImgCloseUp1, ID, VisitDate, "CloseUp1");
            Utility.Lib.LoadImageFile_wTag(ref ImgCloseUp2, ID, VisitDate, "CloseUp2");
            Utility.Lib.LoadImageFile_wTag(ref ImgCloseUp3, ID, VisitDate, "CloseUp3");

            Utility.Lib.getFileDateTime(lblProfileShot, ID, VisitDate, App.getTextMessages("ProfileShot"));
            Utility.Lib.getFileDateTime(lblLeftShot, ID, VisitDate, App.getTextMessages("Left"));
            Utility.Lib.getFileDateTime(lblRightShot, ID, VisitDate, App.getTextMessages("Right"));
            Utility.Lib.getFileDateTime(lblCloseUpShot1, ID, VisitDate, App.getTextMessages("Close Up #1"));
            Utility.Lib.getFileDateTime(lblCloseUpShot2, ID, VisitDate, App.getTextMessages("Close Up #2"));
            Utility.Lib.getFileDateTime(lblCloseUpShot3, ID, VisitDate, App.getTextMessages("Close Up #3"));
        }
     
        private void ShowDateOnButton(System.Windows.Controls.Button B, string Date_String)  // 0102-38
        {
            if (Date_String == null)
            {
                B.Tag = "";
                return;
            }

            if (Date_String.Length == 8)
                B.Tag = Date_String.Substring(4, 2) + "/" + Date_String.Substring(6, 2) + "/" + Date_String.Substring(0, 4);
            else
                B.Tag = "";
        }
        #endregion

        #region Imagebox control
        private void ImgProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectPositonShot = lblProfileShot;
                SelectObject = tbkProfile;
                srcImage = ImgProfile;
            }
            catch(Exception ex)
            {

            }
        }

        private void ImgLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectPositonShot = lblLeftShot;
                SelectObject = tbkLeft;
                srcImage = ImgLeft;
            }
            catch(Exception ex)
            {

            }
        }

        private void ImgRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectPositonShot = lblRightShot;
                SelectObject = tbkRight;
                srcImage = ImgRight;
            }
            catch(Exception ex)
            {

            }
        }

        private void ImgCloseUp1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectPositonShot = lblCloseUpShot1;
                SelectObject = tbkCloseUp1;
                srcImage = ImgCloseUp1;
            }
            catch(Exception ex)
            {

            }
        }

        private void ImgCloseUp2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectPositonShot = lblCloseUpShot2;
                SelectObject = tbkCloseUp2;
                srcImage = ImgCloseUp2;
            }
            catch(Exception ex)
            {

            }
        }

        private void ImgCloseUp3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectPositonShot = lblCloseUpShot3;
                SelectObject = tbkCloseUp3;
                srcImage = ImgCloseUp3;
            }
            catch (Exception ex)
            {

            }
        }

        private void Img2_Drop(object sender, DragEventArgs e)
        {
            if (srcImage != null)
            {
                Img2.Source = srcImage.Source;
                srcImage = null;
            }
           
            if (SelectObject != null && SelectObject.Text != null)
                tbkImg2.Text = SelectObject.Text;
            else
                tbkImg2.Text = "";

            if (SelectPositonShot != null && SelectPositonShot.Content != null)
                lblPositionShot2.Content = DateSelected + " " + SelectPositonShot.Content;
            else
                lblPositionShot2.Content = "";

        }

        private void Img1_Drop(object sender, DragEventArgs e)
        {

            if (srcImage != null)
            {
                Img1.Source = srcImage.Source;
                srcImage = null;
            }
           
            if (SelectObject != null && SelectObject.Text != null)
                tbkImg1.Text = SelectObject.Text;
            else
                tbkImg1.Text = "";

            if (SelectPositonShot != null && SelectPositonShot.Content != null)
                lblPositionShot1.Content = DateSelected + " " + SelectPositonShot.Content;
            else
                lblPositionShot1.Content = "";

        }
        #endregion

        #region Button Control
        private void btnOrg_Click(object sender, RoutedEventArgs e)
        {
            btnOrg.IsEnabled = false;

            LoadPhotos_byVisit(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_Org);

            if (ControlParams.Params.p_Date_Org == "" || ControlParams.Params.p_Date_Org ==null)
                ClearDescription();
            else
               Search_PhotoDB(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_Org);

            DateSelected = btnOrg.Tag.ToString();  // 0102-38
            SetButtonBrush(btnOrg);

            HighlightButton(btnOrg);
            SetButtonBrush(btnOrg);

            btnOrg.IsEnabled  = true;
                       
            // Method for set color ====================================
            //-Change Style
            //Style style = this.FindResource("LabelTemplate") as Style;
            //label1.Style = style;

            //var bc = new BrushConverter();
            //myTextBox.Background = (Brush)bc.ConvertFrom("#FFXXXXXX");
            //==========================================================
        }

        private void btnMVisit_Click(object sender, RoutedEventArgs e)
        {
            btnMVisit.IsEnabled = false;

            LoadPhotos_byVisit(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V1);

            if (ControlParams.Params.p_Date_V1 == "" || ControlParams.Params.p_Date_V1 == null)

                // Clear the description
                ClearDescription();

            else
                Search_PhotoDB(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V1);

            DateSelected = btnMVisit.Tag.ToString(); // 0102-38
            
            HighlightButton(btnMVisit);

            SetButtonBrush(btnMVisit);

            btnMVisit.IsEnabled = true;
            
        }

        private void btnLastVisit_Click(object sender, RoutedEventArgs e)
        {

            btnLastVisit.IsEnabled = false;

            LoadPhotos_byVisit(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V2);

            if (ControlParams.Params.p_Date_V2 == "" || ControlParams.Params.p_Date_V2 == null)
                ClearDescription();
            else
                Search_PhotoDB(ControlParams.DBInfo.ID, ControlParams.Params.p_Date_V2);

            DateSelected = btnLastVisit.Tag.ToString();   // 0102-38

            HighlightButton(btnLastVisit);

            SetButtonBrush(btnLastVisit);

            btnLastVisit.IsEnabled = true;

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

            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", ID, 3, 2); // match ID only
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "VisitDate ", Date_String, 3, 3);

            if (selectCmd == "")
            {
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

            tbkProfile.Text = (string)DV[0]["Desc_Profile"].ToString();
            tbkLeft.Text = (string)DV[0]["Desc_Left"].ToString();
            tbkRight.Text = (string)DV[0]["Desc_Right"].ToString();
            tbkCloseUp1.Text = (string)DV[0]["Desc_CloseUp1"].ToString();
            tbkCloseUp2.Text = (string)DV[0]["Desc_CloseUp2"].ToString();
            tbkCloseUp3.Text = (string)DV[0]["Desc_CloseUp3"].ToString();
            
            return true;
        }
        #endregion

        #region Image Mouse Control
        private void ImgLeft_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            srcImage = null;
        }

        private void ImgProfile_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            srcImage = null;
        }

        private void ImgRight_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            srcImage = null;
        } 

        private void ImgCloseUp1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            srcImage = null;
        } 

        private void ImgCloseUp2_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            srcImage = null;
        }

        private void ImgCloseUp3_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            srcImage = null;
        }

        private void Img1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (srcImage !=null && srcImage.Tag=="ShowPhoto")
            { 
                Img1.Source = srcImage.Source;
                srcImage = null;

                if (SelectObject != null && SelectObject.Text != null)
                    tbkImg1.Text = SelectObject.Text;
                else
                    tbkImg1.Text = "";

                if (SelectPositonShot != null && SelectPositonShot.Content != null)
                    lblPositionShot1.Content = DateSelected + " " + SelectPositonShot.Content;
                else
                    lblPositionShot1.Content = "";

            }

            //===============================
            // Release Mouse
            //===============================

            Img1.ReleaseMouseCapture();
        }

        private void Img2_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (srcImage != null && srcImage.Tag=="ShowPhoto")
            { 
                Img2.Source = srcImage.Source;
                srcImage = null;

                if (SelectObject != null && SelectObject.Text != null)
                {
                    tbkImg2.Text = SelectObject.Text;
                }
                else
                    tbkImg2.Text = "";

                if (SelectPositonShot != null && SelectPositonShot.Content != null)
                    lblPositionShot2.Content = DateSelected + " " + SelectPositonShot.Content;
                else
                    lblPositionShot2.Content = "";
            }

            //===============================
            // Release Mouse
            //===============================

            Img2.ReleaseMouseCapture();
        }

        private void Img1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pos1 = e.MouseDevice.GetPosition(Img1);

            if (Img1.IsMouseCaptured) return;
            Img1.CaptureMouse();

            start1 = e.GetPosition(border1);
            origin1.X = Img1.RenderTransform.Value.OffsetX;
            origin1.Y = Img1.RenderTransform.Value.OffsetY;
        }

        private void Img1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!Img1.IsMouseCaptured) return;
            Point p = e.MouseDevice.GetPosition(border1);

            Matrix m = Img1.RenderTransform.Value;
            
            m.OffsetX = origin1.X + (p.X - start1.X);
            if (m.OffsetX < MinMouseMove1.X)
                m.OffsetX = MinMouseMove1.X;
            if (m.OffsetX > MaxMouseMove1.X)
                m.OffsetX = MaxMouseMove1.X;
           
            m.OffsetY = origin1.Y + (p.Y - start1.Y);
            if (m.OffsetY < MinMouseMove1.Y)
                m.OffsetY = MinMouseMove1.Y;
            if (m.OffsetY > MaxMouseMove1.Y)
                m.OffsetY = MaxMouseMove1.Y;

            Img1.RenderTransform = new MatrixTransform(m);
        }

        private void Img2_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pos2 = e.MouseDevice.GetPosition(Img2);

            if (Img2.IsMouseCaptured) return;
            Img2.CaptureMouse();

            start2 = e.GetPosition(border2);
            origin2.X = Img2.RenderTransform.Value.OffsetX;
            origin2.Y = Img2.RenderTransform.Value.OffsetY;
        }

        private void Img2_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!Img2.IsMouseCaptured) return;
            Point p = e.MouseDevice.GetPosition(border2);

            Matrix m = Img2.RenderTransform.Value;
            
            m.OffsetX = origin2.X + (p.X - start2.X);
            if (m.OffsetX < MinMouseMove2.X)
                m.OffsetX = MinMouseMove2.X;
            if (m.OffsetX > MaxMouseMove2.X)
                m.OffsetX = MaxMouseMove2.X;

            m.OffsetY = origin2.Y + (p.Y - start2.Y);
            if (m.OffsetY < MinMouseMove2.Y)
                m.OffsetY = MinMouseMove2.Y;
            if (m.OffsetY > MaxMouseMove2.Y)
                m.OffsetY = MaxMouseMove2.Y;

            Img2.RenderTransform = new MatrixTransform(m);
        }
        #endregion

        #region Zoom Control
        private void btnImg1_100_Click(object sender, RoutedEventArgs e)
        {
            Img1.RenderTransform = new MatrixTransform(mm_default);
 
            MinMouseMove1.X = -450.0; MaxMouseMove1.X = 450.0;
            MinMouseMove1.Y = -250.0; MaxMouseMove1.Y = 250.0; 
        }

        private void btnImg1_Enlarge1_Click(object sender, RoutedEventArgs e)
        {
            Img1.RenderTransform = new MatrixTransform(mm_default);
            mm1 = Img1.RenderTransform.Value;
            mm1.ScaleAtPrepend(1.5, 1.5, pos1.X, pos1.Y);
            Img1.RenderTransform = new MatrixTransform(mm1);

            MinMouseMove1.X = -680.0; MaxMouseMove1.X = 450.0;
            MinMouseMove1.Y = -400.0; MaxMouseMove1.Y = 250.0; 
        }

        private void btnImg1_Enlarge2_Click(object sender, RoutedEventArgs e)
        {
            Img1.RenderTransform = new MatrixTransform(mm_default);
            mm1 = Img1.RenderTransform.Value;
            mm1.ScaleAtPrepend(2, 2, pos1.X, pos1.Y);
            Img1.RenderTransform = new MatrixTransform(mm1);

            MinMouseMove1.X = -950.0; MaxMouseMove1.X = 450.0;
            MinMouseMove1.Y = -500.0; MaxMouseMove1.Y = 250.0; 
        }

        private void btnImg2_100_Click(object sender, RoutedEventArgs e)
        {
            Img2.RenderTransform = new MatrixTransform(mm_default);

            MinMouseMove2.X = -450.0; MaxMouseMove2.X = 450.0;
            MinMouseMove2.Y = -250.0; MaxMouseMove2.Y = 250.0; 
        }

        private void btnImg2_Enlarge1_Click(object sender, RoutedEventArgs e)
        {
            Img2.RenderTransform = new MatrixTransform(mm_default);
            mm2 = Img2.RenderTransform.Value;
            mm2.ScaleAtPrepend(1.5, 1.5, pos2.X, pos2.Y);
            Img2.RenderTransform = new MatrixTransform(mm2);

            MinMouseMove2.X = -680.0; MaxMouseMove2.X = 450.0;
            MinMouseMove2.Y = -400.0; MaxMouseMove2.Y = 250.0; 
        }

        private void btnImg2_Enlarge2_Click(object sender, RoutedEventArgs e)
        {
            Img2.RenderTransform = new MatrixTransform(mm_default);
            mm2 = Img2.RenderTransform.Value;
            mm2.ScaleAtPrepend(2, 2, pos2.X, pos2.Y);
            Img2.RenderTransform = new MatrixTransform(mm2);

            MinMouseMove2.X = -950.0; MaxMouseMove2.X = 450.0;
            MinMouseMove2.Y = -500.0; MaxMouseMove2.Y = 250.0; 
        }
        #endregion

        private void ClearDescription()
        {
            tbkProfile.Text = "";
            tbkLeft.Text = "";
            tbkRight.Text = "";
            tbkCloseUp1.Text = "";
            tbkCloseUp2.Text = "";
            tbkCloseUp3.Text = "";

            tbkImg1.Text = "";
            tbkImg2.Text = "";
        }

        private void HighlightButton(System.Windows.Controls.Button btn)
        {
            btnOrg.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffd1d2d4"));
            btnMVisit.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffd1d2d4"));
            btnLastVisit.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffd1d2d4"));
            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c"));

            btnOrg.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c"));
            btnMVisit.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c"));
            btnLastVisit.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c"));
            btn.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffeeeeee"));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Depending on where it come from
            if (ControlParams.Params.p_PhotoCaptureSelected == true)
            {
                App.Go(Mode.Photo_Customer_Info);
            }
            else
            {
                // Use the search mode to get into
                ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Search;

                ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Edit;  // 0103-07
                
                App.Go(Mode.Photo_Customer_Search);
            }
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            NavBar.setVolume(ControlParams.Params.p_AudioVolume);
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }
        private void SetButtonBrush(System.Windows.Controls.Button btn)
        {
            ButtonBrush(btnOrg, "0");
            ButtonBrush(btnLastVisit, "0");
            ButtonBrush(btnMVisit, "0");

            ButtonBrush(btn, "1");
        }

        private void ButtonBrush(System.Windows.Controls.Button btn, string Mode)
        {
            if (Mode == "1")  // When Selected
            {
                LinearGradientBrush myBrush = new LinearGradientBrush();
                myBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xFA, 0xFF, 0xFF, 0xFF), 0.0));
                myBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xA0, 0xCC, 0xCC, 0xEE), 1.0));

                btn.BorderBrush = myBrush;
            }
            else
            {
                LinearGradientBrush myBrush = new LinearGradientBrush();
                myBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xA0, 0x33, 0x33, 0x55), 1.0));
                myBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0xA0, 0xFF, 0xFF, 0xFF), 0.0));

                btn.BorderBrush = myBrush;
            }
        }

        private void Me_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

        }
    }
}
