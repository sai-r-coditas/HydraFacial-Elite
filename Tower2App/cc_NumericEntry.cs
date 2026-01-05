using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Threading.Tasks;

using System.Text;  // 0106-09

namespace Edge.Tower2.UI
{
    public partial class cc_NumericEntry : Form
    {
        private enum DisplayMode
        {
            Input,
            InstructionOnly
        }

        private Dictionary<string, Bitmap> bitmapCache;

        private CreditCodeState cState;
        private PictureSetting pSetting;

        public cc_NumericEntry()
        {
            InitializeComponent();
        }

        public bool EntryMode { get; set; }
        private void NumericEntry_Load(object sender, EventArgs e)
        {
            // Ver 002633
            //cState = CreditCodeState.LoadState(EntryMode);
            //pSetting = PictureSetting.Deserialize();
            //bitmapCache = new Dictionary<string, Bitmap>();

            //UpdateUserInterface();
        }

        // Ver 002633
        public void page_init()  // not in use
        {
            this.Visible = false;

            cState = CreditCodeState.LoadState(EntryMode);
          
            bitmapCache = new Dictionary<string, Bitmap>();
           
            UpdateUserInterface();
        }

        #region UpdateUserInterface
        private void UpdateUserInterface()
        {
            State enterState = cState.Current;

            //this.TopMost = (enterState != State.MainScreen);
            //this.Visible = (enterState != State.MainScreen);
            this.textBoxAccountNumber.Text = cState.Kilimanjaro;
            this.textBoxAccountNumber.Visible = (enterState == State.CodeEntry);

            switch (enterState)
            {
                case State.PowerUp:
                case State.CheckClock:
                case State.CheckCredit:
                    //If power up, automatically select next and if the result
                    //is not power up, then refresh the user interface

                    //If check clock, then call "next"
                    //this will check the clock - update the user interface if
                    //the next state is not check clock.  
                    //The next state should either be check credit, or invalid clock
                    SetPictures(pSetting.GetSetting(enterState));
                    this.textBoxEntry.Visible = false;
                    cState.Next(null);
                    if (cState.Current != enterState)
                    {
                        UpdateUserInterface();
                    }

                    // Ver 002633
                    //this.Visible = true;

                    break;
                case State.InitialInstructions:
                case State.NoCreditRemaining:
                case State.LockOut:
                case State.LockWarning:

                case State.CodeNotAccepted: 
                    SetPictures(pSetting.GetSetting(enterState));
                    this.textBoxEntry.Visible = false;

                    // Ver 002633
                    this.Visible = true;

                    break;
                case State.InitialAccountNum:
                case State.InitialAccountPIN:
                case State.CodeEntry:
                case State.ClockYear:
                case State.ClockMonth:
                case State.ClockDay:
                case State.ClockHour:
                case State.ClockMinute:
                    SetPictures(pSetting.GetSetting(enterState));
                    this.textBoxEntry.Text = cState.StateParam;
                    this.textBoxEntry.Visible = true;
                    this.textBoxEntry.Enabled = true;

                    // Ver 002633
                    this.Visible = true;

                    break;
                case State.InitialConfirm:
                case State.CodeAccepted:
                    SetPictures(pSetting.GetSetting(enterState));
                    this.textBoxEntry.Text = cState.StateParam;
                    
                    // swww
                    this.textBoxEntry.Visible = true;
                    this.textBoxEntry.Enabled = true; //false;   
                    
                    this.Visible = true; // Ver 002633
                    
                    break;
                case State.MainScreen:
                    
                    //event to launch main program
                    //LaunchMain();
                     
                    //this.TopMost = false;
                    //System.Windows.Application.Current.MainWindow.Show();
                    
                    // event to launch main program
                    this.TopMost = false;
                    System.Windows.Application.Current.MainWindow.Show();
                    if (((App)System.Windows.Application.Current).MainWindow == ((App)System.Windows.Application.Current)._mainWindows[Mode.wpfLogin])
                    {
                        ((App)System.Windows.Application.Current).StartCreditCodeTimer();
                    }
                    
                    this.Visible = false; // Ver 002633
                   
                    // 2015 01/18
                    //cState.Next(null);
                    //UpdateUserInterface();

                    break;
                default:
                    break;
            }
        }
        #endregion

        private void SetPictures(string background)
        {
            //Set background picture
            pictureBoxNumericEntry.Image = GetImage(background);            
        }

        //sww modified
        private Bitmap GetImage(string fileName)
        {
            Bitmap ret = null;

            if (bitmapCache.ContainsKey(fileName))
            {
                bitmapCache.TryGetValue(fileName, out ret);
            }
            else if (File.Exists(Environment.CurrentDirectory + fileName))
            {
                ret = new Bitmap(Environment.CurrentDirectory + fileName);
                //sww
                bitmapCache.Add(fileName, ret);
            }

            return ret;
        }

        private void SetText(string text)
        {
            textBoxEntry.Text = textBoxEntry.Text + text;
        }

        #region Handle Confirm and cancel button
        // confirm
        private void HandleConfirm()
        {
            bool allowChangeState = false;
            string param = string.Empty;
            State enterState = cState.Current;

            switch (cState.Current)
            {
                case State.InitialInstructions:
                case State.InitialConfirm:
                case State.NoCreditRemaining:
                case State.CodeAccepted:
                case State.CodeNotAccepted:
                case State.LockOut:
                case State.LockWarning:
                    
                    allowChangeState = true;
                    break;
                case State.InitialAccountNum:
                case State.InitialAccountPIN:
                case State.CodeEntry:
                case State.ClockYear:
                case State.ClockMonth:
                case State.ClockDay:
                case State.ClockHour:
                case State.ClockMinute:
                    param = textBoxEntry.Text;
                   
                    allowChangeState =  CurrentTextValid();
                    break;
                default:
                    allowChangeState = false;    //default
                    
                    //sww
                    //allowChangeState = true;
                    break;
            }

            if (allowChangeState)
            {
                cState.Next(param);
                if (enterState != cState.Current)
                {
                    UpdateUserInterface();
                }
            }
        }

        private void HandleCancel()
        {
            bool allowChangeState = false;
            string param = string.Empty;
            State enterState = cState.Current;

            switch (cState.Current)
            {
                case State.InitialConfirm:
                    allowChangeState = true;
                    break;
                case State.InitialAccountNum:
                case State.InitialAccountPIN:
                case State.CodeEntry:
                case State.ClockYear:
                case State.ClockMonth:
                case State.ClockDay:
                case State.ClockHour:
                case State.ClockMinute:
                    if (cState.Current == State.InitialAccountNum 
                        && string.Compare(textBoxEntry.Text, CreditCodeState.DEFAULT_ACCOUNT_PREFIX) == 0)
                    {
                        allowChangeState = true;
                    }
                    else if (string.IsNullOrEmpty(textBoxEntry.Text))
                    {
                        allowChangeState = true;
                    }
                    else
                    {
                        allowChangeState = false;
                        textBoxEntry.Text = textBoxEntry.Text.Substring(0, textBoxEntry.Text.Length - 1);
                    }
                    break;
                default:
                    allowChangeState = false;
                    break;
            }

            if (allowChangeState)
            {
                cState.Back(param);
                if (enterState != cState.Current)
                {
                    UpdateUserInterface();
                }
            }
        }
        #endregion

        #region TextBox control
        private bool CurrentTextValid()
        {
            bool isValid = false;
            string text = textBoxEntry.Text;

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            
            switch (cState.Current)
            {
                case State.InitialAccountNum:
                    isValid = (text.StartsWith(CreditCodeState.DEFAULT_ACCOUNT_PREFIX) 
                        && text.Length > CreditCodeState.DEFAULT_ACCOUNT_PREFIX.Length);
                    break;
                case State.InitialAccountPIN:
                    //this should be 6 digits
                    isValid = (textBoxEntry.Text.Length == 6);
                    break;                
                case State.CodeEntry:
                    //as long as it is not empty, allow user to proceed
                    isValid = true; 
                    break;
                case State.ClockYear:
                    //is 4 digits
                    isValid = (textBoxEntry.Text.Length == 4);
                    break;
                case State.ClockMonth:
                    //1-12
                    isValid = NumberIsInRange(ParseInt(textBoxEntry.Text), 1, 12);
                    break;
                case State.ClockDay:
                    isValid = NumberIsInRange(ParseInt(textBoxEntry.Text), 1, 31);
                    //1-31
                    break;
                case State.ClockHour:
                    isValid = NumberIsInRange(ParseInt(textBoxEntry.Text), 0, 23);
                    //0-23 
                    break;
                case State.ClockMinute:
                    isValid = NumberIsInRange(ParseInt(textBoxEntry.Text), 0, 60);
                    //0 - 60
                    break;
                default:
                    break;
            }

            //sww for testing
            //isValid = true;
            
            return isValid;
        }
        #endregion

        /// <summary>
        /// number is equal to or greater than min 
        /// and number is equal to or less than max  
        /// (inclusive)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool NumberIsInRange(int num, int min, int max)
        {
            return (num >= min && num <= max);
        }

        /// <summary>
        /// Parses non negative int numbers, return -1 if error
        /// </summary>
        /// <param name="pInput"></param>
        /// <returns></returns>
        private int ParseInt(string pInput)
        {
            int ret = -1;

            try
            {
                ret = int.Parse(pInput);
            }
            catch (Exception)
            {
                ret = -1;
            }

            return ret;
        }

        private void ButtonLocationCalc(Point pClickedPoint)
        {
            String text = string.Empty;

            int numbericButtonSizeX = 77;
            int numbericButtonSizeY = 77;
            int actionButtonSizeX = 119;
            int actionButtonSizeY = 70;

            int column_1_Start = 228;
            int column_2_Start = 364;
            int column_3_Start = 505;

            int row_1_Start = 206;
            int row_2_Start = 313;
            int row_3_Start = 418;
            int row_4_Start = 520;

            int column_Action_Start = 654;
            int cancel_Action_Start = 304;
            int next_Action_Start = 418;

            //Check column, then row 

            //7,4,1
            if (pClickedPoint.X >= column_1_Start &&
                pClickedPoint.X <= (column_1_Start + numbericButtonSizeX))
            {
                if (pClickedPoint.Y >= row_1_Start &&
                    pClickedPoint.Y <= (row_1_Start + numbericButtonSizeY))
                {
                    text = "7";//sww  "1";
                }
                else if (pClickedPoint.Y >= row_2_Start &&
                        pClickedPoint.Y <= (row_2_Start + numbericButtonSizeY))
                        
                {
                    text = "4";
                }
                else if (pClickedPoint.Y >= row_3_Start &&
                        pClickedPoint.Y <= (row_3_Start + numbericButtonSizeY))
                {
                    text = "1";//sww "7";
                }
                else if (pClickedPoint.Y >= row_4_Start &&
                        pClickedPoint.Y <= (row_4_Start + numbericButtonSizeY))
                {
                    text = "-";
                }
            }
            //8,5,2,0
            else if (pClickedPoint.X >= column_2_Start &&
                    pClickedPoint.X <= (column_2_Start + numbericButtonSizeX))
            {
                if (pClickedPoint.Y >= row_1_Start &&
                    pClickedPoint.Y <= (row_1_Start + numbericButtonSizeY))
                {
                    text = "8";//sww "2";
                }
                else if (pClickedPoint.Y >= row_2_Start &&
                        pClickedPoint.Y <= (row_2_Start + numbericButtonSizeY))
                {
                    text = "5";
                }
                else if (pClickedPoint.Y >= row_3_Start &&
                        pClickedPoint.Y <= (row_3_Start + numbericButtonSizeY))
                {
                    text = "2";//sww "8";
                }
                else if (pClickedPoint.Y >= row_4_Start &&
                        pClickedPoint.Y <= (row_4_Start + numbericButtonSizeY))
                {
                    text = "0";
                }
            }
            //9,6,3
            else if (pClickedPoint.X >= column_3_Start &&
                    pClickedPoint.X <= (column_3_Start + numbericButtonSizeX))
            {
                if (pClickedPoint.Y >= row_1_Start &&
                    pClickedPoint.Y <= (row_1_Start + numbericButtonSizeY))
                {
                    text = "9";//sww "3";
                }
                else if (pClickedPoint.Y >= row_2_Start &&
                        pClickedPoint.Y <= (row_2_Start + numbericButtonSizeY))
                {
                    text = "6";
                }
                else if (pClickedPoint.Y >= row_3_Start &&
                        pClickedPoint.Y <= (row_3_Start + numbericButtonSizeY))
                {
                    text = "3";//sww "9";
                }
            }
           
                //Cancel, Confirm
            else if (pClickedPoint.X >= column_Action_Start &&
                    pClickedPoint.X <= (column_Action_Start + actionButtonSizeX))
            {
                if (pClickedPoint.Y >= cancel_Action_Start &&
                    pClickedPoint.Y <= (cancel_Action_Start + actionButtonSizeY))
                {
                    HandleCancel();
                }
                else if (pClickedPoint.Y >= next_Action_Start &&
                        pClickedPoint.Y <= (next_Action_Start + actionButtonSizeY))
                {
                    HandleConfirm();
                }
            }

            if (!string.IsNullOrEmpty(text))
            {
                SetText(text);
            }
        }

        private void pictureBoxNumericEntry_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.textBoxEntry.Enabled)
            {
                ButtonLocationCalc(new Point(e.X, e.Y));
            }
        }

        #region Launch Main
        /// <summary>
        /// Launch Main Window  -- not in use
        /// </summary>
        private void LaunchMain()   // not in use
        {
            string endTimePath  = @"\Temp\e";
            if (File.Exists(endTimePath))
            {
                File.Delete(endTimePath);
            }

            StreamWriter w = new StreamWriter(endTimePath, false,Encoding.ASCII); // 0106-09
            w.Write(cState.Kosciusko.ToString());
            w.Close();

            this.TopMost = false;

            Process mainProgram = new Process();
            ProcessStartInfo mainProgramStartInfo = new ProcessStartInfo(@"\Disk\care.exe", "");
            mainProgram.StartInfo = mainProgramStartInfo;
            mainProgram.Start();
            if (!mainProgram.WaitForExit((int)(cState.Kosciusko.Subtract(DateTime.Now).TotalMilliseconds)))
            {
                try
                {
                    mainProgram.Kill();
                }
                catch { }
            }

            this.TopMost = true;
        }
        #endregion

        private void pictureBoxNumericEntry_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            HandleConfirm();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Task.Delay(250).ContinueWith((task => Environment.Exit(0)));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //var wpfwindow = new WPFWindow.Window1();
            //ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            //wpfwindow.Show();
            //=====================================================
            //this.Close();
            //System.Windows.Application.Current.MainWindow.Show();
            //=====================================================

            Read_PictureSettings();
        }

        // Ver 002633
        private StreamReader reader;
        private void Read_PictureSettings()  // UTF-8 format
        {
            CreditCode.CreditCodePictureSettings Pics = null;
            string path = Environment.CurrentDirectory + @"\Disk\creditPictureSetting.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(CreditCode.CreditCodePictureSettings));

            reader = new StreamReader(path);
            Pics = (CreditCode.CreditCodePictureSettings)serializer.Deserialize(reader);
            reader.Close();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // cc_NumericEntry
        //    // 
        //    this.ClientSize = new System.Drawing.Size(784, 562);
        //    this.Name = "cc_NumericEntry";
        //    this.ResumeLayout(false);

        //}

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // cc_NumericEntry
        //    // 
        //    this.ClientSize = new System.Drawing.Size(284, 262);
        //    this.Name = "cc_NumericEntry";
        //    this.ResumeLayout(false);

        //}

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // cc_NumericEntry
        //    // 
        //    this.ClientSize = new System.Drawing.Size(522, 317);
        //    this.Name = "cc_NumericEntry";
        //    this.ResumeLayout(false);

        //}

    }
}