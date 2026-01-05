using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfLogin.xaml
    /// </summary>
    public partial class wpfLogin : Window
    {
        #region Loading
        private App App
        {
            get { return (App)Application.Current; }
        }

        public wpfLogin()
        {
            InitializeComponent();
             
            lblVersion.Content = "Ver 1.5-00115a-IB";

            lblVersion.Visibility = Visibility.Hidden;                                          // 0106-26

            lblMessage.Content = "";                                                            // 0102-11

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\"+App.getTextMessages("hydra-facial-UI-v15-enter")+".png");  // 0102-39  0106-06

            btnExit1.Visibility = Visibility.Hidden;                                            // 0106-12
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            Search_dataset();
        }
        #endregion

        #region DB Control
        //- Excute search function when creating member
        private bool FindExistMember()
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
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "UserID ", "" + DB.DBCommend.Quoting(txtUserID.Text.Trim()) + "", 3, 3);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "userPassword ", "" + DB.DBCommend.Quoting(pbxPassword.Password) + "", 3, 4);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "Active ", "" + "True" + "", 3, 4);

            if (selectCmd == "")
            {
                lblMessage.Content = "Invalid!";  // 0102-28
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

            ControlParams.Params.p_LoginUser= (string)DV[0]["userID"].ToString().Trim();
            ControlParams.Params.p_LoginRole = (string)DV[0]["Role"].ToString().Trim();

            return true;
        }

        //- Excute search Admin function when login
        private string FindExistMember(string UserID)    // 0102-02
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
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "UserID ", "" + DB.DBCommend.Quoting(UserID) + "", 3, 3);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "Active ", "" + "True" + "", 3, 4);

            if (selectCmd == "")
            {
                lblMessage.Content = "Invalid!";  // 0102-28
                return null;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg);

            if (DV == null || DV.Count == 0)
            {
                return null;
            }
            else
            {
                if (DV.Count == 0)
                {
                    return null;
                }
            }

            ControlParams.Params.p_LoginUser = (string)DV[0]["userID"].ToString().Trim();
            ControlParams.Params.p_LoginRole = (string)DV[0]["Role"].ToString().Trim();

            return ControlParams.Params.p_LoginUser = (string)DV[0]["userPassword"].ToString().Trim(); ;
        }
        #endregion

        #region Get All Users
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

            cboUser.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cboUser.Items.Add(dt.Rows[i]["UserID"].ToString());
            }

            return true;
        }
        #endregion

        #region Button Control
        private void pbxPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblMessage.Content = ""; // 0102-11
            UC_Keyboard1.Visibility = Visibility.Visible;
            UC_Keyboard._CurrentControl = pbxPassword;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cvs1.Visibility = Visibility.Hidden;  // 0102-02
            lblMessage.Content = ""; // 0102-11
            pbxPassword.Password = "";
            txtUserID.Text = "";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            txtUserID.Text = cboUser.Text; // 0102-28

            lblMessage.Content = ""; // 0102-11
            if (pbxPassword.Password == "" || txtUserID.Text == "")
            {
                lblMessage.Content = "Invalid!"; // 0102-28
                return;
            }

            if (FindExistMember() || isMaintenance())
            {
                App.cs_Events_User.UserName = "Logged in as " + txtUserID.Text;  // 0102-02

                pbxPassword.Password = "";
                txtUserID.Text = "";

                if (Settings.Default_Hot_Cold_None == "None")
                {
                    App.Go(Mode.Home);
                    cvs1.Visibility = Visibility.Hidden;  // 0102-02
                    return;
                }

                if (Settings.Default_Hot_Cold_None == "Hot")
                    ControlParams.Params.p_HotColdSelected = "HOT";  // 0020-09
                else
                    ControlParams.Params.p_HotColdSelected = "COLD"; // 0020-09

                ((HydraFacial)App._mainWindows[Mode.HydraFacial]).setHotCold(ControlParams.Params.p_HotColdSelected);

                App.Go(Mode.Home);
                cvs1.Visibility = Visibility.Hidden;  // 0102-02
            }
            else
            {
                lblMessage.Content = "Invalid!";  // 0102-28
                return;
            }
        }

        // This is an universal user and password
        private bool isMaintenance()
        {
            if (txtUserID.Text == "ADMIN" && pbxPassword.Password == "17760704")
            {
                ControlParams.Params.p_LoginRole = "Administrator";
                ControlParams.Params.p_LoginUser = "ADMIN";
                return true;
            }
            else
                return false;
        }

        private void txtUserID_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            lblMessage.Content = ""; // 0102-11
            UC_Keyboard1.Visibility = Visibility.Visible;
            UC_Keyboard._CurrentControl = txtUserID;
        }

        private void txtUserID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtUserID.CharacterCasing = CharacterCasing.Upper;  // 0102-10
        }

        private void btnHere_Click(object sender, RoutedEventArgs e)
        {

            lblMessage.Content = ""; // 0102-11
            if (FindExistMember("ADMIN") == "")  // if no password
            {
                App.cs_Events_User.UserName = "Logged in as ADMIN";  // 0102-02
                App.cs_Events_User.SingleUser = true;  // 0102-21
            }
            else // if password required, go to login window
            {
                cvs1.Visibility = Visibility.Visible;
                App.cs_Events_User.SingleUser = false;    // 0102-21
                UC_Keyboard._CurrentControl = txtUserID;  // 0102-11
                return;
            }
            
            if (Settings.Default_Hot_Cold_None == "None")
            {
                App.Go(Mode.Home);
                return;
            }

            if( Settings.Default_Hot_Cold_None == "Hot")
                ControlParams.Params.p_HotColdSelected = "HOT";  // 0020-09
            else
                ControlParams.Params.p_HotColdSelected = "COLD"; // 0020-09

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).setHotCold(ControlParams.Params.p_HotColdSelected);

            App.Go(Mode.Home);
            
            // 2015/03/25 disabled, remove the Hot/Cold Message  
            //if (ControlParams.Params.p_HotColdSelected == "NONE")
            //    App.Go(Mode.Home);
            //else
            //{
            //    App.Go(Mode.wpfInfo);
            //    if (ControlParams.Params.p_HotColdSelected == "" || ControlParams.Params.p_HotColdSelected == null || ControlParams.Params.p_HotColdSelected == "COLD")
            //        ((wpfInfo)App.currentWindow).Page_init("Hot");   // bring up turn on hot meesage 
            //    else if (ControlParams.Params.p_HotColdSelected == "HOT")
            //        ((wpfInfo)App.currentWindow).Page_init("Cold");  // bring up turn on cold meesage 
            //}
        }

        private void btnExit1_Click(object sender, RoutedEventArgs e)
        {
            // 2014 11/05
            App.SaveBottleDataFile();

            App.Outputs.ClearAll();

            Task.Delay(250).ContinueWith((task => Environment.Exit(0)));
        }
        #endregion

        public void ApplicationClose()
        {
            // 2014 11/05
            App.SaveBottleDataFile();

            App.Outputs.ClearAll();

            Task.Delay(250).ContinueWith((task => Environment.Exit(0))); 
        }

        private void cboUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pbxPassword.Focus();  // 0102-35
            UC_Keyboard._CurrentControl = pbxPassword;  // 0102-35
        }
    }
}
