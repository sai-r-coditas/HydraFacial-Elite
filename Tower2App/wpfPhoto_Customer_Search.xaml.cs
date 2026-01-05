using System;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

// For Database
using System.Data;

using System.IO;

using Application = System.Windows.Application;
using TextBox = System.Windows.Controls.TextBox;

using System.Windows.Threading;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for Photo_Customer_Search.xaml
    /// </summary>
    public partial class Photo_Customer_Search
    {

        private string _highlightcolor;
        public string HighLightColor
        {
            get { return _highlightcolor; }
            set { _highlightcolor = value; }
        }

        private App App
        {
            get { return (App)Application.Current; }
        }

        private bool doClearTextBoxFlag = false;

        #region Loading
        public Photo_Customer_Search()
        {
            InitializeComponent();

            txtBirthday.MaxLength = 8;
            txtName.MaxLength = 50;
            txtTelNo.MaxLength = 20;

            txtDate.IsEnabled = false;  // 0102-37

            // 0106-01
            tbkEditBack.Text=App.getTextMessages("Back");   
            tbkBack.Text = App.getTextMessages("Back");
            tbkClear.Text = App.getTextMessages("Clear");
            tbkSave.Text = App.getTextMessages("Save");

            tbkDelete.Text=App.getTextMessages("Delete");
            tbkNext.Text=App.getTextMessages("Photo Capture");
            tbkPastSession.Text=App.getTextMessages("Past Photos");

            lblNotes.Content = App.getTextMessages("Notes");
            tbkSearch.Text = App.getTextMessages("Search");
            tbkCreate.Text=App.getTextMessages("Create");
 
            lblDate.Content = App.getTextMessages("Date");
            lblName.Content = App.getTextMessages("Name");
            lblBirthday.Content = App.getTextMessages("Birthday");
            lblDateFormat.Content = App.getTextMessages("(mm/dd/yy)");
            lblPhoneNo.Content = App.getTextMessages("Tel.No");

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\WindowBackground_DB.png");       // 0106-05
           
        }
        #endregion

        #region Page_init
        public void page_init()                                                                 // 2 Modes => Notes Editing, Search, Resoult found, Create 
        {
            //-Set Default to Caps on
            UC_Keyboard.CapsLockFlag = false;                                                   // true // 0106-09

            ControlParams.Params.p_PhotoCaptureSelected = false;

            grpMessage1.Visibility = Visibility.Collapsed;

            //- Hiding the ID 
            txtID.Visibility = Visibility.Collapsed;
            
            // ========================================

            // Disable Notes
            txtNotes.Visibility = Visibility.Collapsed;
            lblNotes.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Collapsed;                                          // For notes

            btnSearch.Visibility = Visibility.Collapsed;
  
            DBGrid1.Visibility = Visibility.Collapsed;

            btnBack.Visibility = Visibility.Visible;                                            // 0103-06
            btnEditBack.Visibility = Visibility.Hidden;                                         // 0103-06 back button on notes editing
            
            btnDelete.Visibility = Visibility.Collapsed;
            btnDelete.IsEnabled = false;
            btnClear.IsEnabled = false;                                                         // 0102-37

            tbkNext.Text = App.getTextMessages("Next");                                         // 0103-09  // defult is Next then overwrite with Photo Capture if required
            btnNext.Visibility = Visibility.Collapsed;                                          //0103-06
            btnPastSession.Visibility = Visibility.Collapsed;
           
            btnSave.Visibility = Visibility.Collapsed;
            btnCreate.Visibility = Visibility.Collapsed;

            // update keyboard background image  0106-11
            string str = Environment.CurrentDirectory + "\\Skin\\Images\\KB2-" + ControlParams.Params.p_SecondLanguage + ".png";  // 0106-11
            if (File.Exists(str))
                ucKeyboard.SetImageData(str);                                                   // 0106-11
            else
            {
                // set back to US default keyboard
                str = Environment.CurrentDirectory + "\\Skin\\Images\\KB2-en-US.png";           // 0106-11
                ucKeyboard.SetImageData(str);
            }

            // ========================================

            // For Database Edit, Save, Search, Clear  buttons control
            // New Customer
            switch (ControlParams.Params.PhotoCapture_State)
            {
                case  ControlParams.e_DB.Create:
                    
                        btnCreate.Visibility = Visibility.Visible;
                        btnCreate.IsEnabled = true;
                        btnClear.IsEnabled = true;                                              // 0103-06

                        btnEdit.IsEnabled = false;
                        btnSave.IsEnabled = false;
                        ClearTextbox();

                        txtDate.IsEnabled = false;
                        txtDate.Text = DateTime.Now.ToString("MM/dd/yy");                       // 2014 07/18

                        txtName.Focusable = true;
                        Keyboard.Focus(txtName);
                        UC_Keyboard.DigitOnly = false;
                        UC_Keyboard._CurrentControl = txtName;
                        break;

                case  ControlParams.e_DB.Search:
                    
                        // Search  mode
                        btnClear.IsEnabled = true;                                              // 0102-37

                        DBGrid1.Visibility = Visibility.Visible;
                        btnDelete.Visibility = Visibility.Visible;
                        btnNext.Visibility = Visibility.Visible;                                // 0103-06

                        btnEdit.IsEnabled = false;
                        btnSave.IsEnabled = true;

                        txtName.Focusable = true;
                        Keyboard.Focus(txtName);
                        UC_Keyboard.DigitOnly = false;
                        UC_Keyboard._CurrentControl = txtName;
                        break;

                case  ControlParams.e_DB.InfoSave:                                              // notes
                    
                        btnSave.Visibility = Visibility.Visible;

                        btnBack.Visibility = Visibility.Hidden;                                 // 0103-06
                        btnEditBack.Visibility = Visibility.Visible;                            // 0103-06

                        btnEdit.IsEnabled = false;
                        btnSave.IsEnabled = true;
                        break;

                case  ControlParams.e_DB.InfoEdit:
                    
                        btnEdit.IsEnabled = false;
                        btnSave.IsEnabled = true;
                        break;
            }

            //==============================================================================
            //==============================================================================
            
            // Info Mode is for entering user information
            switch (ControlParams.Params.PhotoCapture_Session)
            {
                case ControlParams.e_Session.Create:
                    
                        InfoTextboxEnabled(true);
                        lblTitle.Content = App.getTextMessages("New Member");                   // 0103-09
                        txtNotes.Visibility = Visibility.Collapsed;
                        lblNotes.Visibility = Visibility.Collapsed;

                        btnNext.IsEnabled = false;
                        tbkNext.Text = App.getTextMessages("Photo Capture");                    // 0103-09
                        btnPastSession.IsEnabled = false;

                        //- If it is exist customer
                        if (ControlParams.Params.p_NewCustomer == false)
                        {
                            btnClear.IsEnabled = true;                                          // 0102-37
                        }

                        txtName.Focusable = true;
                        Keyboard.Focus(txtName);
                        UC_Keyboard.DigitOnly = false;
                        UC_Keyboard._CurrentControl = txtName;
                        break;

                        // Back from Photo capture or photo info , not a really for edit
                case  ControlParams.e_Session.Edit:                                             // 0103-07
                    
                        InfoTextboxEnabled(true);
                        lblTitle.Content = App.getTextMessages("Member Info");                  // 0103-09
                        txtNotes.Visibility = Visibility.Collapsed;
                        lblNotes.Visibility = Visibility.Collapsed;

                        DBGrid1.Visibility = Visibility.Visible;
                        btnDelete.Visibility = Visibility.Visible;

                        tbkNext.Text = App.getTextMessages("Photo Capture");                    // 0103-09

                        btnPastSession.Visibility = Visibility.Visible;

                        btnSearch.Visibility = Visibility.Visible;

                        //- If it is exist customer
                        if (ControlParams.Params.p_NewCustomer == false)
                        {
                            btnClear.IsEnabled = true;                                          // 0102-37
                        }

                        txtName.Focusable = true;
                        Keyboard.Focus(txtName);
                        UC_Keyboard.DigitOnly = false;
                        UC_Keyboard._CurrentControl = txtName;
                        break;
                       
                        // Info Mode is for entering user information, DB Search
                case  ControlParams.e_Session.Search:
                    
                        InfoTextboxEnabled(true);
                        lblTitle.Content = App.getTextMessages("Search Member");                // 0103-09
                        txtNotes.Visibility = Visibility.Collapsed;
                        lblNotes.Visibility = Visibility.Collapsed;
                        DBGrid1.Visibility = Visibility.Visible;
                        btnDelete.Visibility = Visibility.Visible;

                        btnNext.IsEnabled = false;
                        tbkNext.Text = App.getTextMessages("Photo Capture");                    // 0103-09
                        btnPastSession.IsEnabled = false;
                        btnPastSession.Visibility = Visibility.Visible;

                        //- If it is exist customer
                        if (ControlParams.Params.p_NewCustomer == false)
                        {
                            btnClear.IsEnabled = true;                                          // 0102-37
                        }

                        Search_dataset(true); 

                        setSearchMode();

                        txtName.Focusable = true;
                        Keyboard.Focus(txtName);
                        UC_Keyboard.DigitOnly = false;
                        UC_Keyboard._CurrentControl = txtName;
                        break;

                case  ControlParams.e_Session.Profile:
                    
                        lblTitle.Content = App.getTextMessages("Profile");                      // 0103-09
                        EnableNoteEditing();
                        break;

                case ControlParams.e_Session.PhotoL:
                   
                        lblTitle.Content = App.getTextMessages("Left");                         // 0103-09
                        EnableNoteEditing();
                        break;

                case  ControlParams.e_Session.PhotoR:
                    
                        lblTitle.Content = App.getTextMessages("Right");                        // 0103-09
                        EnableNoteEditing();
                        break;

                case ControlParams.e_Session.CloseUp1:
                  
                        lblTitle.Content = App.getTextMessages("Close Up #1");                  // 0103-09
                        EnableNoteEditing();
                        break;

                case  ControlParams.e_Session.CloseUp2:
                    
                        lblTitle.Content = App.getTextMessages("Close Up #2");                  // 0103-09;
                        EnableNoteEditing();
                        break;

                case  ControlParams.e_Session.CloseUp3:
                    
                        lblTitle.Content = App.getTextMessages("Close Up #3");                  // 0103-09;
                        EnableNoteEditing();
                        break;
            }
        }
        #endregion

        #region DB Control
        //- Excute search function, search if the client is a member.
        private bool Search_dataset(bool FindMore)
        {
            string selectCmd = "";
            string selectCmd1 = "";

            selectCmd = "Select * ";

            string TableName = "SystemDB";

            DB.DBCommend.CutOffStringComma(ref  selectCmd);

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "Birthday ", "" + DB.DBCommend.Quoting(txtBirthday.Text.Trim()) + "", 3, 3);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "Name ", "" + DB.DBCommend.Quoting(txtName.Text.Trim()) + "%", 1, 4);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "TelNo ", "" + DB.DBCommend.Quoting(txtTelNo.Text.Trim()) + "%", 1, 5);  // 0102-38
 
            if (selectCmd == "")
            {
                // No messages need show 
                return false;
            }

            DB.DBCommend.CutOffStringAnd(ref selectCmd);

            selectCmd = selectCmd1 + selectCmd;

            //- Check for order field
            string OrderCmd = "";

            //--For Sorting
            DB.DBCommend.GetSqlCommandForOrder(ref OrderCmd, "ID ", txtID.Text, 1);
            DB.DBCommend.GetSqlCommandForOrder(ref OrderCmd, "Date_Created ", txtDate.Text, 2);
            DB.DBCommend.GetSqlCommandForOrder(ref OrderCmd, "Birthday ", txtBirthday.Text, 3);
            DB.DBCommend.GetSqlCommandForOrder(ref OrderCmd, "Name ", txtName.Text, 4);
            DB.DBCommend.GetSqlCommandForOrder(ref OrderCmd, "TelNo ", txtTelNo.Text, 5);

            DB.DBCommend.GetSqlCommandForOrder(ref OrderCmd, "Notes ", txtNotes.Text, 6);

            //- Cut off comma
            DB.DBCommend.CutOffStringComma(ref  OrderCmd);

            if (OrderCmd != "")
                selectCmd = selectCmd + " Order by " + OrderCmd;   //- TITLE ASC, ARTIST ASC, FILE ASC ";

            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string ErrMsg = "";
            DataView DV = dbLibrary.LoadDataView_sw(selectCmd, TableName, ref ErrMsg);

            if (DV == null)                                                                     // 0106-19
            {
                return false;
            }

            DBGrid1.ItemsSource = DV;
         
            if (DV == null || DV.Count == 0)
            {
                DBGrid1.Columns[2].Header = App.getTextMessages("Name");      // 0103-09
                DBGrid1.Columns[2].Width = 180;                               // 0103-10
                DBGrid1.Columns[4].Header = App.getTextMessages("Birthday");  // 0103-09
                DBGrid1.Columns[4].Width = 100;                               // 0103-10
                DBGrid1.Columns[5].Header = App.getTextMessages("Tel.No");    // 0103-09
                DBGrid1.Columns[5].Width = 134;                               // 0103-10

                return false;
            }
            else
            {
                if (DV.Count == 0)
                {
                    return false;
                }
            }
            
            DBGrid1.Columns[2].Header =  App.getTextMessages("Name");     // 0103-09
            DBGrid1.Columns[2].Width = 180;                               // 0103-10
            DBGrid1.Columns[3].Header = "";
            DBGrid1.Columns[4].Header =  App.getTextMessages("Birthday"); // 0103-09
            DBGrid1.Columns[4].Width = 100;                               // 0103-10
            DBGrid1.Columns[5].Header = App.getTextMessages("Tel.No");    // 0103-09
            DBGrid1.Columns[5].Width = 134;                               // 0103-10
            
            DBGrid1.Columns[6].Header = "";
            DBGrid1.Columns[7].Header = "";

            return true;
        }

        //- Excute search function when creating member
        private bool FindExistMember()
        {
            string selectCmd = "";
            string selectCmd1 = "";

            selectCmd = "Select * ";

            string TableName = "SystemDB";

            DB.DBCommend.CutOffStringComma(ref  selectCmd);

            selectCmd = selectCmd + " from " + TableName + "  ";
            selectCmd = selectCmd + " Where ";

            selectCmd1 = selectCmd;
            selectCmd = "";

            //- 1
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "Birthday ", "" + DB.DBCommend.Quoting(txtBirthday.Text.Trim()) + "", 3, 3);
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "Name ", "" + DB.DBCommend.Quoting(txtName.Text.Trim()) + "", 3, 4);
   
            if (selectCmd == "")
            {
                // Show Message Here
                MessageBox.Show(App.getTextMessages("Please Entering Search Data!"));  // 0103-09
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
             return true;
        }

        //- Excute search function for Photos Description
        private bool Find_PhotoDB_Exist(string ID, string Date_String)
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
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", ID, 3, 2); // match ID only
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
            return true;
        }

        //- Excute search function for Photos Description
        private bool Load_PhotoDB_Description(string ID, string Date_String)
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
            DB.DBCommend.GetSqlCommandForFind(ref  selectCmd, "ID ", ID, 3, 2); // match ID only
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

            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
                txtNotes.Text = (string)DV[0]["Desc_Profile"].ToString().Trim();
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
                txtNotes.Text = (string)DV[0]["Desc_Left"].ToString().Trim();
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
                txtNotes.Text = (string)DV[0]["Desc_Right"].ToString().Trim();
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
                txtNotes.Text = (string)DV[0]["Desc_CloseUp1"].ToString().Trim();
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
                txtNotes.Text = (string)DV[0]["Desc_CloseUp2"].ToString().Trim();
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
               txtNotes.Text = (string)DV[0]["Desc_CloseUp3"].ToString().Trim();

            return true;
        }

        //- Insert single record to database
        private bool Insert_SystemDB()   // 0102-39
        {
            string TableName = "SystemDB";
            //- Insert Data
            string selectCmd = "";
            string Command1 = "";
            string Command2 = "";

            string ID;
            // generate unique 14 Digits
            ID = "E" + string.Format("{0}", DateTime.Now.ToString("yyMMddHHmmssf"));
            
            selectCmd = "Insert Into " + TableName + " (";

            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "ID", DB.DBCommend.Quoting(ID), 1,  1);  //1= text ,2 =integer 
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Name", DB.DBCommend.Quoting(txtName.Text), 1,  2);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Birthday", DB.DBCommend.Quoting(txtBirthday.Text), 1,  3);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "TelNo", DB.DBCommend.Quoting(txtTelNo.Text), 1,  4);
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Notes", DB.DBCommend.Quoting(txtNotes.Text), 1, 4);

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
                MessageBox.Show("No Data Save!");
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
                MessageBox.Show("Data Insert Failed!");
                ID = "";
                return false;
            }
            else
            {
                ControlParams.DBInfo.ID = ID;
                ControlParams.DBInfo.Name = txtName.Text;
            }

            ControlParams.Params.p_Date_Org = "";
            ControlParams.Params.p_Date_V1 = "";
            ControlParams.Params.p_Date_V2 = "";
            ControlParams.Params.p_Last_Visit = "";
 
            btnSearch.Visibility = Visibility.Collapsed;

            return true;
        }

        // Update System DB
        private bool Update_SystemDB()
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string ID;
            //- Use current ID
            ID = ControlParams.DBInfo.ID;

            string selectCmd = "";

            selectCmd = "UPDATE " + "SystemDB" + " Set ";
            //- 1
          
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Name", txtName.Text, 1, 2);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "Birthday", txtBirthday.Text, 1, 3);
            DB.DBCommend.GetSqlCommand(ref selectCmd, "TelNo", txtTelNo.Text, 1, 4);

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "'" ;

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
                return true;
            }
        }

        //- Insert single record to database
        private bool Insert_Description(string c_ID, string date_string)  // 0103-07
        {
            string TableName = "Photos";
            //- Insert Data
            string selectCmd = "";
            string Command1 = "";
            string Command2 = "";

            selectCmd = "Insert Into " + TableName + " (";
           
            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "ID", c_ID, 1, 1);  //1= text ,2 =integer 
            
            if ( ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile )
                DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_Profile", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            else if ( ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile )
                DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_Left", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            else if ( ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile )
                DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_Right", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            else if ( ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile )
                DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_CloseUp1", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            else if ( ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile )
                DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_CloseUp2", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);
            else if ( ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile )
                 DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "Desc_CloseUp3", DB.DBCommend.Quoting(txtNotes.Text), 1, 1);

            DB.DBCommend.GetSqlCommandForInsert(ref Command1, ref Command2, "VisitDate", date_string, 1, 1);

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
                MessageBox.Show("No Data Save!");
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
                MessageBox.Show("Data Insert Failed!");
                c_ID = "";
                return false;
            }
            else
            {
                 ControlParams.DBInfo.ID = c_ID;
            }

            return true;
        }

        private void Delete_Customer(string ID)
        {
            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string selectCmd = "";

            selectCmd = "Delete * from SystemDB Where Id ='" + ID + "'";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
            {
                MessageBox.Show("Delete Customer Failed !");
            }
            else
            {
                ClearTextbox();
                DBGrid1.ItemsSource = null;
            }

            DeleteFiles(ID);  //Delete photos
        }

        private void Delete_PhotoDB(string ID)
        {

            DB.DBLibrary dbLibrary = new DB.DBLibrary();
            string selectCmd = "";

            selectCmd = "Delete * from Photos Where Id ='" + ID + "'";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)
            {
                // No message required 
                //MessageBox.Show("No Photo Deleted!");
            }
            else
            {
                ClearTextbox();
                DBGrid1.ItemsSource = null;
            }
        }

        // When Save button press
        private void Update_PhototoDB_Description(string ID, string Date_String)
        {
            string TableName = "Photos";
            DB.DBLibrary dbLibrary = new DB.DBLibrary();

            string selectCmd = "";

            selectCmd = "UPDATE " + TableName + " Set ";

            // Update Page Title
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Profile)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_Profile", txtNotes.Text, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoL)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_Left", txtNotes.Text, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.PhotoR)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_Right", txtNotes.Text, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp1)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_CloseUp1", txtNotes.Text, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp2)
            {
                DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_CloseUp2", txtNotes.Text, 1, 1);
            }
            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.CloseUp3)
                DB.DBCommend.GetSqlCommand(ref selectCmd, "Desc_CloseUp3", txtNotes.Text, 1, 1);

            DB.DBCommend.CutOffStringComma(ref selectCmd);
            selectCmd = selectCmd + " Where ID='" + ID + "' and VisitDate ='" + Date_String + "'";

            string ErrMsg = "";
            bool R;
            R = dbLibrary.Update_sw(selectCmd, ref ErrMsg);
            if (R == false)

                MessageBox.Show("Update Failed!");
            else
            {
                // Update Success
            }
        }
        #endregion

        #region DBGrid
        private void DBGrid1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ID") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "Date_Created") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "Operate") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "Last_Name") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "PhoneTypeH") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "PhoneTypeW") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "PhoneTypeC") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "Notes") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "V1") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "V2") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "LastVisit") e.Column.Visibility = System.Windows.Visibility.Collapsed;
            if (e.PropertyName == "Org") e.Column.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void DBGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DBGrid1.SelectedItem != null)
            {
                lblTitle.Content = App.getTextMessages("Member Editing");  // 0103-09

                DataRowView row = (DataRowView)DBGrid1.SelectedItems[0];
                txtName.Text = row["Name"].ToString();
                txtDate.Text = row["Date_Created"].ToString();
                txtBirthday.Text = row["Birthday"].ToString();
                txtTelNo.Text = row["TelNo"].ToString();
                txtID.Text = row["ID"].ToString();
                ControlParams.Params.p_Date_V1 = row["V1"].ToString();
                ControlParams.Params.p_Date_V2 = row["V2"].ToString();
                ControlParams.Params.p_Date_Org = row["Org"].ToString(); ;
             
                ControlParams.DBInfo.ID = txtID.Text;
                ControlParams.DBInfo.Name = txtName.Text;

                btnNext.IsEnabled = true;
                btnPastSession.IsEnabled = true;

                btnDelete.IsEnabled = true;
                btnEdit.IsEnabled = true;

                btnClear.IsEnabled = false;
 
                btnSearch.IsEnabled = true;
                btnSearch.Visibility = Visibility.Visible;
                btnSave.IsEnabled = true;
                btnSave.Visibility = Visibility.Visible;
                ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.InfoSave;
            }

        }

        private void DBGrid1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DBGrid1.SelectedItem != null)
            {
                lblTitle.Content = App.getTextMessages("Member Editing"); // 0103-09

                DataRowView row = (DataRowView)DBGrid1.SelectedItems[0];
                txtName.Text = row["Name"].ToString();
                txtDate.Text = row["Date_Created"].ToString();
                txtBirthday.Text = row["Birthday"].ToString();
                txtTelNo.Text = row["TelNo"].ToString();
                txtID.Text = row["ID"].ToString();
               
                ControlParams.DBInfo.ID = txtID.Text;
                ControlParams.DBInfo.Name = txtName.Text;

                ControlParams.Params.p_Date_V1 = row["V1"].ToString();
                ControlParams.Params.p_Date_V2 = row["V2"].ToString();
                ControlParams.Params.p_Date_Org = row["Org"].ToString(); ;
                btnNext.IsEnabled = true;
                btnPastSession.IsEnabled = true;

                btnDelete.IsEnabled = true;
                btnEdit.IsEnabled = true;

                btnClear.IsEnabled = false;

                btnSearch.IsEnabled = true;
                btnSearch.Visibility = Visibility.Visible;

                btnSave.IsEnabled = true;
                btnSave.Visibility = Visibility.Visible;
                ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.InfoSave;

            }
        }
        #endregion

        #region Functions
        private void CopyDataToScreen(Control C, string Data)
        {
            if (Data == "" || Data == null)
            {
                //Do Nothing
            }
            else
                ((TextBox)C).Text = Data;
        }

        private void setSearchMode()
        {
            lblTitle.Content = App.getTextMessages("Search Member");  // 0103-09

            txtDate.Text = ""; // 0102-37

            btnSearch.IsEnabled = false;
            btnSearch.Visibility = Visibility.Collapsed;

            btnClear.IsEnabled = true;

            ClearTextbox();
            btnDelete.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnPastSession.IsEnabled = false;

            btnEdit.IsEnabled = false;
            btnSave.IsEnabled = false;
        }

        private bool VerifyCustomerName()
        {
            if (txtName.Text == "")
            {
                lblMsg.Content = App.getTextMessages("Please enter customer name!");  // 0103-06 0103-09 
                DoEvents();
                System.Threading.Thread.Sleep(3000);
                lblMsg.Content = "";

                Keyboard.Focus(txtName);
                UC_Keyboard.DigitOnly = false;
                UC_Keyboard._CurrentControl = txtName;
                return false;
            }
            return true;
        }

        private bool VerifyBirthday()
        {
            if (txtBirthday.Text == "")
                return true;

            if (txtBirthday.Text.Length != 8)  // 0102-39
            {
                lblMsg.Content = App.getTextMessages("Invalid Birthday!");   // 0103-06  0103-09
                DoEvents();
                System.Threading.Thread.Sleep(3000);
                lblMsg.Content = "";

                Keyboard.Focus(txtBirthday);
                UC_Keyboard.DigitOnly = false;
                UC_Keyboard._CurrentControl = txtBirthday;
                return false;
            }

            // for avoiding char '/' appears more than 2
            if (checkDateString(txtBirthday.Text))
            {
                lblMsg.Content = App.getTextMessages("Invalid Birthday!");   // 0103-06  0103-09
                DoEvents();
                System.Threading.Thread.Sleep(3000);
                lblMsg.Content = "";

                Keyboard.Focus(txtBirthday);
                UC_Keyboard.DigitOnly = false;
                UC_Keyboard._CurrentControl = txtBirthday;
                return false;
            }

            DateTime value; // 0103-05
            if (!DateTime.TryParse(txtBirthday.Text, out value))
            {
                lblMsg.Content = App.getTextMessages("Invalid Birthday!");   // 0103-06  0103-09
                DoEvents();
                System.Threading.Thread.Sleep(3000);
                lblMsg.Content = "";

                Keyboard.Focus(txtBirthday);
                UC_Keyboard.DigitOnly = false;
                UC_Keyboard._CurrentControl = txtBirthday;
                return false;
            }

            return true;

            // =========================================
            // Check Birthday - temporary diesabled
            // Because Some country may have different 
            // date format
            // =========================================
            //if (!checkDate(txtBirthday.Text))
            //{
            //    MessageBox.Show("Invalid Birthday!");
            //    Keyboard.Focus(txtBirthday);
            //    UC_Keyboard.DigitOnly = false;
            //    UC_Keyboard._CurrentControl = txtBirthday;
            //    return;
            //}
        }

        private bool checkDateString(string DateStr)
        {
            int count = DateStr.Count(f => f == '/');
            if (count > 2)
                return true;
            else
                return false;
        }

        private bool checkDate(string DateStr)
        {
            try
            {
                Convert.ToDateTime(DateStr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DeleteFiles(string ID)
        {
            try
            {
                string sourceDir = ControlParams.Params.Photos_Default;

                foreach (FileInfo f in new DirectoryInfo(sourceDir).GetFiles(ID + "*.*"))
                {
                    f.Delete();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void EnableNoteEditing()
        {
            InfoTextboxEnabled(false);
            txtNotes.Visibility = Visibility.Visible;
            lblNotes.Visibility = Visibility.Visible;
            btnSearch.Visibility = Visibility.Collapsed;

            btnClear.IsEnabled = false;  // 0102-37

            Load_PhotoDB_Description(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup);

            txtDate.IsEnabled = false;

            txtNotes.Focusable = true;
            Keyboard.Focus(txtNotes);
            UC_Keyboard.DigitOnly = false;
            UC_Keyboard._CurrentControl = txtNotes;
        }

        private void ButtonController(ControlParams.e_DB Mode)
        {
            //- Disable all button first
            btnSearch.Visibility = Visibility.Collapsed;
            btnCreate.Visibility = Visibility.Collapsed;
            lblNotes.Visibility = Visibility.Collapsed;
            txtNotes.Visibility = Visibility.Collapsed;

            btnClear.IsEnabled = false;  // 0102-37

            btnEdit.IsEnabled = false;
            btnSave.IsEnabled = false;

            if (Mode == ControlParams.e_DB.InfoEdit)  // Edit Mode
            {
                btnSave.IsEnabled = true;
            }
            else if (Mode == ControlParams.e_DB.InfoSave) // Saved
            {
                btnSave.IsEnabled = false;
                btnEdit.IsEnabled = true;
            }
            else if (Mode == ControlParams.e_DB.Search) // Search Mode
            {
                btnClear.IsEnabled = true;  // 0102-37
                InfoTextboxEnabled(true);
            }
        }

        private void InfoTextboxEnabled(bool V)
        {
            txtName.IsEnabled = V;
            txtBirthday.IsEnabled = V;
            txtTelNo.IsEnabled = V;
        }

        private void ClearTextbox()
        {
            doClearTextBoxFlag = true;
            txtName.Text = "";
            txtBirthday.Text = "";
            txtTelNo.Text = "";
            txtNotes.Text = "";
            doClearTextBoxFlag = false;
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }
        #endregion

        #region Button Control
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (doClearTextBoxFlag == false)
            {
                if (Search_dataset(true))
                {
                    //btnNext1.IsEnabled = true;
                }

                if (txtName.Text.Length > 30)
                {
                    txtName.Text = txtName.Text.Substring(0, 30);
                }
            }
        }

        private void txtBirthday_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (doClearTextBoxFlag == false)
            {
                // Check date format
                if (txtBirthday.Text.Length >= 3 && txtBirthday.Text.Substring(2, 1) != "/")
                {
                    string s1 = txtBirthday.Text.Substring(0, 2);
                    string s2 = txtBirthday.Text.Substring(2, txtBirthday.Text.Length - 2);
                    txtBirthday.Text = s1 + "/" + s2;

                    // Position the Cursor
                    txtBirthday.Select(txtBirthday.Text.Length, 3);
                }
                else if (txtBirthday.Text.Length >= 6 && txtBirthday.Text.Substring(5, 1) != "/")
                {
                    string s1 = txtBirthday.Text.Substring(0, 5);
                    string s2 = txtBirthday.Text.Substring(5, txtBirthday.Text.Length - 5);
                    txtBirthday.Text = s1 + "/" + s2;

                    // Position the Cursor
                    txtBirthday.Select(txtBirthday.Text.Length, 6);
                }

                if (txtBirthday.Text.Length > 8)
                    txtBirthday.Text = txtBirthday.Text.Substring(0, 7);

                if (Search_dataset(true))
                {
                    //Do nothing
                }
            }
        }

        private void txtTelNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (doClearTextBoxFlag == false)
            {
                if (Search_dataset(true))
                {
                    //btnNext1.IsEnabled = true;
                }

                if (txtTelNo.Text.Length > 20)
                {
                    txtTelNo.Text = txtTelNo.Text.Substring(0, 20);
                }
            }
        }

        private void txtNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNotes.Text.Length > 200)
            {
                txtNotes.Text = txtNotes.Text.Substring(0, 200);
            }
        }

        //- For Key Board Data Retrun [============================================== 
        private void txtName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UC_Keyboard.DigitOnly = false;
            UC_Keyboard._CurrentControl = txtName;
        }

        private void txtBirthday_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UC_Keyboard.DigitOnly = true;
            UC_Keyboard._CurrentControl = txtBirthday;
        }

        private void txtTelNo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UC_Keyboard.DigitOnly = true;
            UC_Keyboard._CurrentControl = txtTelNo;
        }

        private void txtNotes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UC_Keyboard.DigitOnly = false;
            UC_Keyboard._CurrentControl = txtNotes;
        }

        private void txtBirthday_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) & e.Key != Key.Back | e.Key == Key.Space)
            {
                if (e.Key == Key.OemQuestion)
                { }
                else
                {
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region Buttons
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTextbox();
            btnDelete.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnPastSession.IsEnabled = false;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.Photo_Customer_Type2);
        }

        private void btnMessageOK_Click(object sender, RoutedEventArgs e)
        {
            grpMessage1.Visibility = Visibility.Collapsed;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            string DeleteID = txtID.Text;

            Delete_Customer(DeleteID);
            Delete_PhotoDB(DeleteID);

            btnSearch_Click(sender, e);
            grpMessage1.Visibility = Visibility.Collapsed;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            grpMessage1.Visibility = Visibility.Collapsed;
        }

        private void btnPastSession_Click(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.p_PhotoCaptureSelected = false;
            App.Go(Mode.Photo_Comparison);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            btnNext.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF48C5D8"));
        }

        private void btnEditBack_Click(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.p_PhotoCaptureSelected = true;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
            App.Go(Mode.ImageCapture);
        }
    
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            setSearchMode();

            // 2014 12/15
            btnNext.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD1D2D4"));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Create)
            {
                if (ControlParams.Params.p_NewCustomer == true)
                {
                    var R = Insert_SystemDB();
                }
            }

            else if (ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Search || ControlParams.Params.PhotoCapture_Session == ControlParams.e_Session.Edit)  // 0106-06
            {
                if (!VerifyCustomerName())
                    return;

                if (!VerifyBirthday())
                    return;

                Update_SystemDB();

                //Refresh DBGrid
                Search_dataset(true);
            }
            else   // for save Notes
            {
                // in Photo DB
                if (Find_PhotoDB_Exist(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup))  // 0103-06
                {
                    // do updating
                    Update_PhototoDB_Description(ControlParams.DBInfo.ID, ControlParams.Params.p_SelectedGroup);  // 0103-06
                }
                else
                {
                    bool R = Insert_Description(ControlParams.DBInfo.ID,ControlParams.Params.p_SelectedGroup);  // 0103-07
                }

                ControlParams.Params.p_PhotoCaptureSelected = true;   // 0103-06

                ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07

                App.Go(Mode.ImageCapture);                            // 0103-06
            }

            // Enable edit button
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.InfoEdit;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.InfoSave;
            btnSave.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnEdit.IsEnabled = false;
        }

        // Next Button
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.p_PhotoCaptureSelected = true;

            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).NewPhotoGroup_Verify();  // 0103-07
            ((PhotoCapture.PhotoCapture)App._mainWindows[Mode.ImageCapture]).page_init();  // 0103-07
          
            App.Go(Mode.ImageCapture);
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            btn_OK.Visibility = Visibility.Collapsed;
            btn_Cancel.Visibility = Visibility.Collapsed;
            btnMessageOK.Visibility = Visibility.Visible;

            if (!VerifyCustomerName())
                return;

            if (!VerifyBirthday())
                return;

            if (FindExistMember())
            {
                lblMsg.Content = "Member Exist!";  // 0103-06
                DoEvents();
                System.Threading.Thread.Sleep(3000);

                lblMsg.Content = "";
            }
            else
            {
                if (!Insert_SystemDB())  // 0102-39
                    return;

                btnNext.IsEnabled = true;   // Photo capture
                btnPastSession.IsEnabled = true;
                btnCreate.IsEnabled = false;

                btnNext_Click(null, null); // 0101-03    
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btnSearch.IsEnabled = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (txtID.Text != "" && txtID.Text != null)
            {
                App.wpfyesno.WarningMessage(App.getTextMessages("Are you sure you want to delete this record?"));   // 0103-09
                App.wpfyesno.ShowDialog();
                var data = App.wpfyesno.ans;

                if (data == "yes")
                {
                    string DeleteID = txtID.Text;
                    Delete_Customer(DeleteID);
                    Delete_PhotoDB(DeleteID);
                    btnSearch_Click(sender, e);
                }
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

        private void Me_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
