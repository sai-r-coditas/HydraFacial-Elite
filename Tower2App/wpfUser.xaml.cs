using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Data;

// For DispatcherPriority
using System.Windows.Threading;

using System.Collections.ObjectModel;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfUser.xaml
    /// </summary>
    public partial class wpfUser : Window
    {
        #region Init
        private App App
        {
            get { return (App)Application.Current; }
        }

        public wpfUser()
        {
            InitializeComponent();
          
            //IsVisibleChanged += (sender, args) =>
            //{
            //    if ((bool)args.NewValue)
            //        OnEnter();
            //    else
            //        OnLeave();
            //};

            UC_Keyboard.CapsLockFlag = true;

            var v = Search_dataset();

            txtID.Visibility = Visibility.Hidden;
            lblMessage.Visibility = Visibility.Hidden;  // 0102-10
            lblMsg.Visibility = Visibility.Hidden;  // 0102-10
        }

        private void OnEnter()
        {
            cvsDetail.Visibility = Visibility.Hidden;
            cvsList.Visibility = Visibility.Visible;
        }

        private void OnLeave()
        {

        }
        #endregion

        #region DB Control
        /// <summary> Insert single record to database 
        /// </summary>
        private bool Insert_SystemDB()
        {
            string TableName = "UserDB";
            
            //- Insert Data
            string selectCmd = "";
            string Command1 = "";
            string Command2 = "";

            string ID;
            selectCmd = "Insert Into " + TableName + " (";

            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "UserID", DB.DBCommend.Quoting(txtUserID.Text), 1, 2);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "UserPassword", DB.DBCommend.Quoting(txtPassword.Password.ToString()), 1, 3);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Role", DB.DBCommend.Quoting(cboRole.Text), 1, 2);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Active", DB.DBCommend.Quoting(cboActive.Text), 1, 2);

            selectCmd = selectCmd + Command1;

            if (Command2 == "")  //- No date in all fields
            {
                lblMessage.Content = "* Invalid Data!";
                lblMessage.Visibility = Visibility.Visible;
                return false;
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
                lblMessage.Content = "* Insert Failed, Data Invalid!";
                lblMessage.Visibility = Visibility.Visible;
                ID = "";
                return false;
            }
       
            lblMessage.Visibility = Visibility.Hidden;
            return true;
        }

        private void Delete_Member(string ID)
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string selectCmd = "";

            selectCmd = "Delete * from UserDB Where ID =" + ID + "";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
            {
                lblMsg.Content = "* Delete Failed!";
                lblMsg.Visibility = Visibility.Visible;
                return;   // 0102-10
            }

            Search_dataset();
        }

        private bool FindExistMember()   // Find ID for update purpose
        {
            string selectCmd = "";
            string selectCmd1 = "";
            selectCmd = "Select * ";

            string TableName = "UserDB";

            DB.DBCommend.CutOffStringComma(ref  selectCmd);

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", "" + DB.DBCommend.Quoting(txtID.Text.Trim()) + "", 2, 3);  
     
            if (selectCmd == "")
            {
                lblMessage.Content = "* Invalid Data!";
                lblMessage.Visibility = Visibility.Visible;
                return false;
            }

            lblMessage.Visibility = Visibility.Hidden;  // 0102-10

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

            return true;
        }

        // When Save button press
        private bool Update_Member(string ID)
        {
            string TableName = "UserDB";
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string selectCmd = "";

            selectCmd = "UPDATE " + TableName + " Set ";

            DB.DBCommend.GetSqlCommand(ref selectCmd, "UserID", txtUserID.Text, 1, 1);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "userPassword", txtPassword.Password, 1, 1);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Role", cboRole.Text, 1, 1);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Active", cboActive.Text, 1, 1);

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID=" + ID + "";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
            {
                lblMessage.Content = "* Update Failed!";
                lblMessage.Visibility = Visibility.Visible;
                return false;
            }
          
            return true;
        }
 
        class UserInfo
        {
            public string ID { get; set; }
            public string UserID { get; set; }
            public string UserPassword { get; set; }
            public string Role { get; set; }
            public string Active { get; set; }
        }

        private bool Search_dataset()   // reflash the DB
        {
            string selectCmd = "";
            string TableName = "UserDB";

            selectCmd = " Select ID, UserID, userPassword, Role, Active from " + TableName + " Order by UserID ASC";

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

            DataTable dt = DV.ToTable();  // 0102-09

            // Create a collection for your types
            ObservableCollection<UserInfo> list = new ObservableCollection<UserInfo>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UserInfo userinfo = new UserInfo
                {
                    // Casting might be needed here depending on your data types ...
                    ID = dt.Rows[i]["ID"].ToString(), 
                    UserID = dt.Rows[i]["UserID"].ToString(),
                    UserPassword = dt.Rows[i]["UserPassword"].ToString(),
                    Role = dt.Rows[i]["Role"].ToString(),
                    Active = dt.Rows[i]["Active"].ToString()
                };

                list.Add(userinfo);
            }

            lvUser.ItemsSource = list;
           
            return true;
        }
        #endregion

        #region Button Control 
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            lblMsg.Visibility = Visibility.Hidden;  // 0102-10
          
            lblHeader.Content = "New Member";

            cvsDetail.Visibility = Visibility.Visible;   // 0102-09
            cvsList.Visibility = Visibility.Hidden;
          
            txtUserID.Text = "";
            txtPassword.Password = "";
            txtConfirm.Password = "";

            txtUserID.IsEnabled = true; // 0102-11
       
            btnSaveNew.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Hidden;
        }

        private void txtConfirm_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;  // 0102-10

            UC_Keyboard1.Visibility = Visibility.Visible;
            UC_Keyboard._CurrentControl = txtConfirm;
        }

        private void txtPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;  // 0102-10

            UC_Keyboard1.Visibility = Visibility.Visible;
            UC_Keyboard._CurrentControl = txtPassword;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)      // update
        {
            Update();
        }

        private void txtUserID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtUserID.CharacterCasing = CharacterCasing.Upper;  // 0102-10
        }

        private void txtUserID_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;  // 0102-10

            UC_Keyboard1.Visibility = Visibility.Visible;
            UC_Keyboard._CurrentControl = txtUserID;
        }

        private void btnSaveNew_Click(object sender, RoutedEventArgs e)  // create a new account
        {
            if (txtUserID.Text == "")
            {
                lblMessage.Content = "* Invalid User ID!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            if (txtPassword.Password != txtConfirm.Password)
            {
                lblMessage.Content = "* Invalid Password!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            if (cboRole.Text == "")
            {
                lblMessage.Content = "* Role Invalid!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            if (cboActive.Text == "")
            {
                lblMessage.Content = "* Invalid Active Setting!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            if (!Insert_SystemDB()) // 0102-10
                return;

            lblMessage.Visibility = Visibility.Hidden;  // 0102-10

            Search_dataset();
 
            txtUserID.Text = "";
            txtPassword.Password = "";
            txtConfirm.Password = "";

            cvsDetail.Visibility = Visibility.Hidden;
            cvsList.Visibility = Visibility.Visible;
        }

        private void cboActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;  // 0102-10
        }

        private void cboRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;  // 0102-10
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            cvsDetail.Visibility = Visibility.Hidden;   // 0102-09
            cvsList.Visibility = Visibility.Visible;

            txtUserID.Text = "";
            txtPassword.Password = "";
            txtConfirm.Password = "";

            lblMessage.Visibility = Visibility.Hidden;  // 0102-10
        }
        #endregion

        #region Functions- edit, delete,
        private void EditCategory(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            UserInfo productCategory = b.CommandParameter as UserInfo;
            MessageBox.Show(productCategory.UserID);
        }

        private void Update()
        {
            if (txtUserID.Text == "")
            {
                lblMessage.Content = "* Invalid User ID!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            if (txtPassword.Password != txtConfirm.Password)
            {
                lblMessage.Content = "* Invalid Password!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            if (!FindExistMember())
            {
                lblMessage.Content = "* Invalid Member!";
                lblMessage.Visibility = Visibility.Visible;
                return;
            }

            lblMessage.Visibility = Visibility.Hidden;  // 0102-10

            if (txtUserID.Text == "ADMIN")    // 0102-02 , For admin it does not allow to change activation and role
            {
                cboActive.Text = "True";
                cboRole.Text = "Administrator";
            }

            if (!Update_Member(txtID.Text))  // 0102-10
                return;

            if (txtUserID.Text == "ADMIN" & txtPassword.Password == "")  // 0102-21
                App.cs_Events_User.SingleUser = true;  
            else
                App.cs_Events_User.SingleUser = false;  

            cvsDetail.Visibility = Visibility.Hidden;   // 0102-09
            cvsList.Visibility = Visibility.Visible;
          
            btnNew.IsEnabled = true;

            Search_dataset();
        }

        private void Deleteing_member_new (string ID,string UserID)
        {
            if (UserID == "ADMIN")
            {
                lblMsg.Content = "* This account is reserved!";
                lblMsg.Visibility = Visibility.Visible;
                return;
            }

            if (ID == "" || ID == null)
            {
                lblMsg.Content = "* Invalid Member!";
                lblMsg.Visibility = Visibility.Visible;
                return;
            }

            App.wpfyesno.WarningMessage(App.getTextMessages("Delete user") + " " + UserID + ", " + App.getTextMessages("are you sure?"));
            App.wpfyesno.ShowDialog();
            var data = App.wpfyesno.ans;

            if (data == "yes")
            {
                Delete_Member(ID);
            }
        }
        #endregion

        #region List Control for T2
        private void tbkEdit_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lblMsg.Visibility = Visibility.Hidden;                                              // 0102-10

            var tb = (TextBlock)e.OriginalSource;
            var dataCxtx = tb.DataContext;
            var dataSource = (UserInfo)dataCxtx;

            if (dataSource.ID.ToString() != null && dataSource.ID.ToString() != "")             // ?? need takes care if interger
            {
                txtID.Text = dataSource.ID.ToString();
                txtUserID.Text = dataSource.UserID.ToString();
                txtPassword.Password = dataSource.UserPassword.ToString();
                txtConfirm.Password = dataSource.UserPassword.ToString();
                cboRole.Text = dataSource.Role.ToString();
                cboActive.Text = dataSource.Active.ToString();
                DoEvents();

                if (txtUserID.Text == "ADMIN")                                                  // 0102-10 this is a reserved account
                    txtUserID.IsEnabled = false;
                else
                    txtUserID.IsEnabled = true;

                cvsDetail.Visibility = Visibility.Visible;                                      // 0102-09
                cvsList.Visibility = Visibility.Hidden;

                btnSaveNew.Visibility = Visibility.Hidden;
                btnEdit.Visibility = Visibility.Visible;

                lblHeader.Content = "Member";
            }
        }

        private void tbkDelete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblMsg.Visibility = Visibility.Hidden;                                              // 0102-10

            var tb = (TextBlock)e.OriginalSource;
            var dataCxtx = tb.DataContext;
            var dataSource = (UserInfo)dataCxtx;

            Deleteing_member_new(dataSource.ID.ToString(), dataSource.UserID.ToString());
        }

        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private bool mouseDown;
        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;

            // Save starting point, used later when determining 
            // how much to scroll.
            scrollStartPoint = e.GetPosition(this);
            scrollStartOffset.X = ScrollViewer.HorizontalOffset;
            scrollStartOffset.Y = ScrollViewer.VerticalOffset;

            // Update the cursor if can scroll or not.
            this.Cursor = (ScrollViewer.ExtentWidth > ScrollViewer.ViewportWidth) ||
                (ScrollViewer.ExtentHeight > ScrollViewer.ViewportHeight) ?
               System.Windows.Input.Cursors.Arrow : System.Windows.Input.Cursors.Arrow;
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the new scroll position.
            Point point = e.GetPosition(this);

            if (mouseDown)
            {
                // Determine the new amount to scroll.
                Point delta = new Point((point.X > this.scrollStartPoint.X) ?
                                 -(point.X - this.scrollStartPoint.X) :
                                    (this.scrollStartPoint.X - point.X),

                                (point.Y > this.scrollStartPoint.Y) ?
                                -(point.Y - this.scrollStartPoint.Y) :
                                    (this.scrollStartPoint.Y - point.Y));

                // Scroll to the newer location
                ScrollViewer.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                ScrollViewer.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }
        #endregion

        #region Others
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            NavBar.setVolume(ControlParams.Params.p_AudioVolume); // 0101-03
            cvsDetail.Visibility = Visibility.Hidden;
            cvsList.Visibility = Visibility.Visible;
            lblMessage.Visibility = Visibility.Hidden;   // 0102-10
            lblMsg.Visibility = Visibility.Hidden;  // 0102-10
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden; // 0101-03
            lblMessage.Visibility = Visibility.Hidden;   // 0102-10
            lblMsg.Visibility = Visibility.Hidden;  // 0102-10
        }
        #endregion
    }
}
