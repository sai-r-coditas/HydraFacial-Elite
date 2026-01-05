using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Diagnostics;
using System.Drawing;
using System.IO;

using System.Text;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfCreditCode.xaml
    /// </summary>
    public partial class wpfCreditCode : Window
    {
        private Dictionary<string, Bitmap> bitmapCache;

        private CreditCodeState cState;

        private PictureSetting pSetting;

        public wpfCreditCode()
        {
            InitializeComponent();

            btnExit.Visibility = Visibility.Hidden;
          
            // 2014 11/07
            cState = CreditCodeState.LoadState(false);  //sww 102-32

            pSetting = PictureSetting.Deserialize();    //sww 102-32
            
            UpdateUserInterface();                      //sww 102-32

            Utility.Lib.LoadImageNoLock(imgTopMenu, "\\Skin\\Images\\"+App.getTextMessages("top_menu_bg_dim")+".png");  // 0103-05  0106-06
        }

        #region Page init
        public bool EntryMode { get; set; }  // Ver 002639-08

        public void page_init()
        {

            Utility.Lib.CreditLog("credit-page init"); // 0103-05

            cState = CreditCodeState.LoadState(EntryMode);
          
            bitmapCache = new Dictionary<string, Bitmap>();

            UpdateUserInterface();

            this.Topmost = true; // 0102-32 
        }
        #endregion

        #region UpdateUserInterface
        private void UpdateUserInterface()
        {
            Utility.Lib.CreditLog("UpdateUserInterface ->" + cState.Current.ToString()); // 0103-10

            State enterState = cState.Current;

            //-this.TopMost = (enterState != State.MainScreen);
            //-this.Visible = (enterState != State.MainScreen);

            textBoxAccountNumber.Text = cState.Kilimanjaro;

            if (enterState == State.CodeEntry)
                this.textBoxAccountNumber.Visibility = Visibility.Visible;
            else
                this.textBoxAccountNumber.Visibility = Visibility.Hidden;

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
                    
                    this.textBoxEntry.Visibility = Visibility.Hidden;  // sww
                    cvsKeyBoard.Visibility = textBoxEntry.Visibility;

                    cState.Next(null);
                    if (cState.Current != enterState)
                    {
                        UpdateUserInterface();
                    }
                   
                    break;

                case State.InitialInstructions:
                case State.NoCreditRemaining:
                case State.LockOut:
                case State.LockWarning:
             
                case State.CodeNotAccepted:
                    
                    SetPictures(pSetting.GetSetting(enterState));
                 
                    this.textBoxEntry.Visibility = Visibility.Hidden;  // sww
                    cvsKeyBoard.Visibility = textBoxEntry.Visibility;
                   
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

                    //sww
                    //-this.textBoxEntry.Visible = false;
                    this.textBoxEntry.Visibility = Visibility.Visible;
                    cvsKeyBoard.Visibility = textBoxEntry.Visibility;
                 
                    break;

                case State.InitialConfirm:
                case State.CodeAccepted:

                    SetPictures(pSetting.GetSetting(enterState));
                    this.textBoxEntry.Text = cState.StateParam;
                   
                    //-this.textBoxEntry.Visible = false;
                    this.textBoxEntry.Visibility = Visibility.Visible;  // 0020-05
                    cvsKeyBoard.Visibility = textBoxEntry.Visibility;
                  
                    this.textBoxEntry.IsEnabled = false; // default ->false;
                  
                    break;

                case State.MainScreen:
                    
                    //event to launch main program
                    //LaunchMain();
                    //cState.Next(null);
                    //UpdateUserInterface();

                    Utility.Lib.CreditLog("OK, show main screen!"); // 0103-10

                    this.Topmost = false;  //sww  0102-32
                    System.Windows.Application.Current.MainWindow.Show();
                    if (((App)System.Windows.Application.Current).MainWindow == ((App)System.Windows.Application.Current)._mainWindows[Mode.wpfLogin])
                    {
                        ((App)System.Windows.Application.Current).StartCreditCodeTimer();
                    }

                    // Ver 002633
                    this.Visibility = Visibility.Hidden;

                    break;
                default:
                    break;
            }
        }

        private void SetPictures(string background)
        {
            //Set background picture
            Utility.Lib.LoadImageNoLock(pictureBoxNumericEntry, background);  // 0102-38
        }
        #endregion

        // Button Confirm / Cancel
        #region Handle Confirm / Cancel button
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

                    allowChangeState = CurrentTextValid();
                    break;
                default:
                    allowChangeState = false;

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

        private bool CurrentTextValid()  // call from confirm button
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

            return isValid;
        }
        #endregion

        #region Others
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

        //This is a hack to make it work with the vb software. 
        private void LaunchMain()   // not in use
        {
            string endTimePath = @"\Temp\e";
            if (File.Exists(endTimePath))
            {
                File.Delete(endTimePath);
            }

            StreamWriter w = new StreamWriter(endTimePath, false, Encoding.ASCII);  // 0106-09
            w.Write(cState.Kosciusko.ToString());
            w.Close();
          
            this.Topmost = false;

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

            this.Topmost = true;
        }
        #endregion

        #region Button Control
        private void pictureBoxNumericEntry_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.textBoxEntry.IsEnabled)
            {
              
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            HandleConfirm();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            HandleCancel();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == btn0)
                SetText("0");
            else if ((sender as Button) == btn1)
                SetText("1");
            else if ((sender as Button) == btn2)
                SetText("2");
            else if ((sender as Button) == btn3)
                SetText("3");
            else if ((sender as Button) == btn4)
                SetText("4");
            else if ((sender as Button) == btn5)
                SetText("5");
            else if ((sender as Button) == btn6)
                SetText("6");
            else if ((sender as Button) == btn7)
                SetText("7");
            else if ((sender as Button) == btn8)
                SetText("8");
            else if ((sender as Button) == btn9)
                SetText("9");
            else if ((sender as Button) == btnDash)
                SetText("-");
        }

        private void SetText(string text)
        {
            textBoxEntry.Text = textBoxEntry.Text + text;
        }
 
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Task.Delay(250).ContinueWith((task => Environment.Exit(0)));
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
    
}
