using System;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

using System.Globalization;

using WindowsInput;                                                                             // 0106-10

using System.ComponentModel;

using System.IO;
using System.Windows.Media.Imaging;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for UC_Keyboard.xaml
    /// </summary>
    public partial class UC_Keyboard : UserControl, INotifyPropertyChanged
    {
        #region Binding
        private string _currentLanguage;
        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                _currentLanguage = value;
                RaisePropertyChanged("CurrentLanguage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool _borderVisible;
        public bool BorderVisible
        {
            get { return _borderVisible; }
            set
            {
                _borderVisible = value;
                RaisePropertyChanged("BorderVisible");
            }
        }

        private string _imagepath;
        public string ImagePath
        {
            get { return _imagepath; }
            set
            {
                _imagepath = value;
                RaisePropertyChanged("ImagePath");
            }
        }

        public void SetImageData(string str)                                                    // 0106-11  0106-16
        {
            try
            {
                if (!File.Exists(str))
                    return;

                byte[] data = File.ReadAllBytes(str);

                var source = new BitmapImage();
                source.BeginInit();
                source.StreamSource = new MemoryStream(data);
                source.EndInit();

                // use public setter
                ImageSource = source;
            }
            catch(Exception)
            {
                Utility.Lib.SaveErrorLog("Invalid file =>" + str);
            }
        }

        ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                RaisePropertyChanged("ImageSource");
            }
        }

        #endregion

        public UC_Keyboard()
        {
            InitializeComponent();

            ucKeyBoard.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#0064676c"));  // 0102-38

            // 0102-38
            Image img = new Image();
            Utility.Lib.LoadImageNoLock(img, "/Skin/Images/KB2-en-US.png");                     //0106-11

            ImageBrush BrushImg = new ImageBrush();
            BrushImg.ImageSource = img.Source;
            BrushImg.Stretch = Stretch.Fill;
            Resources["dy_Keyboard"] = BrushImg;

            this.DataContext = this;                                                            // 0106-10
            CurrentLanguage = "en-US";                                                          // 0106-10
            
            LoadKeyBoardImage();                                                                // 0106-12
        }

        public void LoadKeyBoardImage()                                                         // 0106-12 0106-16
        {
            string str = Environment.CurrentDirectory + "\\Skin\\Images\\KB2-" + ControlParams.Params.p_SecondLanguage + ".png";  // 0106-11

            try
            {
                if (File.Exists(str))
                    SetImageData(str);                                                          // 0106-11
                else
                {
                    // set back to US default keyboard
                    str = Environment.CurrentDirectory + "\\Skin\\Images\\KB2-en-US.png";       // 0106-11
                    SetImageData(str);
                }
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("Invalid file =>" + str);
            }
        }

        #region Declare
        private static double _WidthTouchKeyboard = 830;
        public static double WidthTouchKeyboard
        {
            get { return _WidthTouchKeyboard; }
            set { _WidthTouchKeyboard = value; }
        }

        private static bool _ShiftFlag;
        protected static bool ShiftFlag
        {
            get { return _ShiftFlag; }
            set { _ShiftFlag = value; }
        }

        private static bool _CapsLockFlag;
        public static bool CapsLockFlag
        {
            get { return _CapsLockFlag; }
            set { _CapsLockFlag = value; }
        }

        private static bool _DigitOnly;
        public static bool DigitOnly
        {
            get { return _DigitOnly; }
            set { _DigitOnly = value; }
        }

        public static Window _InstanceObject;

        private static Brush _PreviousTextBoxBackgroundBrush = null;
        private static Brush _PreviousTextBoxBorderBrush = null;
        private static Thickness _PreviousTextBoxBorderThickness;

        public static Control _CurrentControl;
        public static string TouchScreenText
        {
            get
            {
                if (_CurrentControl is TextBox)
                {
                    return ((TextBox)_CurrentControl).Text;
                }
                else if (_CurrentControl is PasswordBox)
                {
                    return ((PasswordBox)_CurrentControl).Password;
                }
                else return "";
            }
            set
            {
                if (_CurrentControl is TextBox)
                {
                    ((TextBox)_CurrentControl).Text = value;
                }
                else if (_CurrentControl is PasswordBox)
                {
                    ((PasswordBox)_CurrentControl).Password = value;
                }
            }
        }

        public static RoutedUICommand CmdTlide = new RoutedUICommand();
        public static RoutedUICommand Cmd1 = new RoutedUICommand();
        public static RoutedUICommand Cmd2 = new RoutedUICommand();
        public static RoutedUICommand Cmd3 = new RoutedUICommand();
        public static RoutedUICommand Cmd4 = new RoutedUICommand();
        public static RoutedUICommand Cmd5 = new RoutedUICommand();
        public static RoutedUICommand Cmd6 = new RoutedUICommand();
        public static RoutedUICommand Cmd7 = new RoutedUICommand();
        public static RoutedUICommand Cmd8 = new RoutedUICommand();
        public static RoutedUICommand Cmd9 = new RoutedUICommand();
        public static RoutedUICommand Cmd0 = new RoutedUICommand();
        public static RoutedUICommand CmdMinus = new RoutedUICommand();
        public static RoutedUICommand CmdPlus = new RoutedUICommand();
        public static RoutedUICommand CmdBackspace = new RoutedUICommand();


        public static RoutedUICommand CmdTab = new RoutedUICommand();
        public static RoutedUICommand CmdQ = new RoutedUICommand();
        public static RoutedUICommand CmdW = new RoutedUICommand();  // 0106-10
        public static RoutedUICommand CmdE = new RoutedUICommand();
        public static RoutedUICommand CmdR = new RoutedUICommand();
        public static RoutedUICommand CmdT = new RoutedUICommand();
        public static RoutedUICommand CmdY = new RoutedUICommand();
        public static RoutedUICommand CmdU = new RoutedUICommand();
        public static RoutedUICommand CmdI = new RoutedUICommand();
        public static RoutedUICommand CmdO = new RoutedUICommand();
        public static RoutedUICommand CmdP = new RoutedUICommand();
        public static RoutedUICommand CmdOpenCrulyBrace = new RoutedUICommand();
        public static RoutedUICommand CmdEndCrultBrace = new RoutedUICommand();
        public static RoutedUICommand CmdOR = new RoutedUICommand();

        public static RoutedUICommand CmdCapsLock = new RoutedUICommand();
        public static RoutedUICommand CmdA = new RoutedUICommand();
        public static RoutedUICommand CmdS = new RoutedUICommand();
        public static RoutedUICommand CmdD = new RoutedUICommand();
        public static RoutedUICommand CmdF = new RoutedUICommand();
        public static RoutedUICommand CmdG = new RoutedUICommand();
        public static RoutedUICommand CmdH = new RoutedUICommand();
        public static RoutedUICommand CmdJ = new RoutedUICommand();
        public static RoutedUICommand CmdK = new RoutedUICommand();
        public static RoutedUICommand CmdL = new RoutedUICommand();
        public static RoutedUICommand CmdColon = new RoutedUICommand();
        public static RoutedUICommand CmdDoubleInvertedComma = new RoutedUICommand();
        public static RoutedUICommand CmdEnter = new RoutedUICommand();

        public static RoutedUICommand CmdShift = new RoutedUICommand();
        public static RoutedUICommand CmdZ = new RoutedUICommand();
        public static RoutedUICommand CmdX = new RoutedUICommand();
        public static RoutedUICommand CmdC = new RoutedUICommand();
        public static RoutedUICommand CmdV = new RoutedUICommand();
        public static RoutedUICommand CmdB = new RoutedUICommand();
        public static RoutedUICommand CmdN = new RoutedUICommand();
        public static RoutedUICommand CmdM = new RoutedUICommand();
        public static RoutedUICommand CmdGreaterThan = new RoutedUICommand();
        public static RoutedUICommand CmdLessThan = new RoutedUICommand();
        public static RoutedUICommand CmdQuestion = new RoutedUICommand();

        public static RoutedUICommand CmdSpaceBar = new RoutedUICommand();
        public static RoutedUICommand CmdClear = new RoutedUICommand();
        #endregion
 
        #region Verify Parameters
        static void VerifyParam(string str, string currentlang)                                 // 0106-12
        {
            #region Digitkey only (0-9)
            if (DigitOnly)
            {
                //Not allow use shift key
                if (ShiftFlag)
                    return;

                switch (str)
                { 
                
                    //UC_Keyboard.TouchScreenText += str.Substring(3);
               
                    case "Cmd1": SendSimulator(VirtualKeyCode.VK_1); break;
                    case "Cmd2": SendSimulator(VirtualKeyCode.VK_2); break;
                    case "Cmd3": SendSimulator(VirtualKeyCode.VK_3); break;
                    case "Cmd4": SendSimulator(VirtualKeyCode.VK_4); break;
                    case "Cmd5": SendSimulator(VirtualKeyCode.VK_5); break;
                    case "Cmd6": SendSimulator(VirtualKeyCode.VK_6); break;
                    case "Cmd7": SendSimulator(VirtualKeyCode.VK_7); break;
                    case "Cmd8": SendSimulator(VirtualKeyCode.VK_8); break;
                    case "Cmd9": SendSimulator(VirtualKeyCode.VK_9); break;
                    case "Cmd0": SendSimulator(VirtualKeyCode.VK_0); break;
                        
                    case "CmdQuestion":   //  "/" 
                        SendSimulatorChar(VirtualKeyCode.OEM_2); 
                        //UC_Keyboard.TouchScreenText += "/";
                        break;
                    case  "CmdMinus":
                        //UC_Keyboard.TouchScreenText += "-";
                        SendSimulator(VirtualKeyCode.OEM_MINUS);
                        break;
                    
                    case  "CmdBackspace":
                
                        if (!string.IsNullOrEmpty(UC_Keyboard.TouchScreenText))
                        {
                            //UC_Keyboard.TouchScreenText = UC_Keyboard.TouchScreenText.Substring(0, UC_Keyboard.TouchScreenText.Length - 1);
                            SendSimulator(VirtualKeyCode.BACK);  // 0106-11
                        }
                        break;

                    case  "CmdClear" : // 0103-06
                
                        if (!string.IsNullOrEmpty(UC_Keyboard.TouchScreenText))
                        {
                            UC_Keyboard.TouchScreenText = "";
                        }
                        break;

                    case   "CmdEnter":
                
                        if (_InstanceObject != null)
                        {
                            //Close keyboard
                            //sww
                            //_InstanceObject.Close();
                            //_InstanceObject = null;
                        }
                        //_CurrentControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);  // 0106-10
                        break;
                }
                return;
            }
            #endregion

            #region check string
            if (currentlang != "en-US")
            {
                if (isInvalidKey(str))                                                          // 0106-12
                    return;
            }
            
            switch (str)
            {
                case "CmdTlide": SendSimulator(VirtualKeyCode.OEM_3); break;

                case "Cmd1": SendSimulator(VirtualKeyCode.VK_1); break;
                case "Cmd2": SendSimulator(VirtualKeyCode.VK_2); break;
                case "Cmd3": SendSimulator(VirtualKeyCode.VK_3); break;
                case "Cmd4": SendSimulator(VirtualKeyCode.VK_4); break;
                case "Cmd5": SendSimulator(VirtualKeyCode.VK_5); break;
                case "Cmd6": SendSimulator(VirtualKeyCode.VK_6); break;
                case "Cmd7": SendSimulator(VirtualKeyCode.VK_7); break;
                case "Cmd8": SendSimulator(VirtualKeyCode.VK_8); break;
                case "Cmd9": SendSimulator(VirtualKeyCode.VK_9); break;
                case "Cmd0": SendSimulator(VirtualKeyCode.VK_0); break;
               
                case "CmdMinus": SendSimulator(VirtualKeyCode.OEM_MINUS); break;
                case "CmdPlus": SendSimulator(VirtualKeyCode.OEM_PLUS); break;

                case "CmdBackspace" :

                    if (!string.IsNullOrEmpty(UC_Keyboard.TouchScreenText))
                    {
                        SendSimulator(VirtualKeyCode.BACK);                                     // 0106-11
                    }
                    break;

                case "CmdTab" :
                         //Second Row
                        {
                            SendSimulator(VirtualKeyCode.TAB);                                  // 0106-11
                        }
                    break;
                    
                case "CmdQ": { SendSimulatorChar(VirtualKeyCode.VK_Q); } break;
                case "CmdW": { SendSimulatorChar(VirtualKeyCode.VK_W); } break;                 // 0106-09 
                case "CmdE": { SendSimulatorChar(VirtualKeyCode.VK_E); } break;
                case "CmdR": { SendSimulatorChar(VirtualKeyCode.VK_R); } break;
                case "CmdT": { SendSimulatorChar(VirtualKeyCode.VK_T); } break;
                case "CmdY": { SendSimulatorChar(VirtualKeyCode.VK_Y); } break;
                case "CmdU": { SendSimulatorChar(VirtualKeyCode.VK_U); } break;
                case "CmdI": { SendSimulatorChar(VirtualKeyCode.VK_I); } break;
                case "CmdO": { SendSimulatorChar(VirtualKeyCode.VK_O); } break;
                case "CmdP": { SendSimulatorChar(VirtualKeyCode.VK_P); } break;

                case "CmdOpenCrulyBrace": { SendSimulatorChar(VirtualKeyCode.OEM_4); } break;
                case "CmdEndCrultBrace": { SendSimulatorChar(VirtualKeyCode.OEM_6); } break;
                case "CmdOR": { SendSimulatorChar(VirtualKeyCode.OEM_5); } break;

                ///Third Row
                case "CmdCapsLock": CapsLockFlag = !CapsLockFlag;     break;

                case "CmdA": { SendSimulatorChar(VirtualKeyCode.VK_A); } break;
                case "CmdS": { SendSimulatorChar(VirtualKeyCode.VK_S); } break;
                case "CmdD": { SendSimulatorChar(VirtualKeyCode.VK_D); } break;
                case "CmdF": { SendSimulatorChar(VirtualKeyCode.VK_F); } break;
                case "CmdG": { SendSimulatorChar(VirtualKeyCode.VK_G); } break;
                case "CmdH": { SendSimulatorChar(VirtualKeyCode.VK_H); } break;
                case "CmdJ": { SendSimulatorChar(VirtualKeyCode.VK_J); } break;
                case "CmdK": { SendSimulatorChar(VirtualKeyCode.VK_K); } break;
                case "CmdL": { SendSimulatorChar(VirtualKeyCode.VK_L); } break;

                case "CmdColon": { SendSimulatorChar(VirtualKeyCode.OEM_1); } break;
           
                case "CmdDoubleInvertedComma": { SendSimulatorChar(VirtualKeyCode.OEM_7); } break;
                     
                case "CmdEnter" :
              
                
                    UC_Keyboard._CurrentControl.Focus();
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);                     // 0106-10
                    
                    break;
                   
                case "CmdShift" :    
                     ShiftFlag = true; 
                     break;

                case "CmdZ": { SendSimulatorChar(VirtualKeyCode.VK_Z); } break;
                case "CmdX": { SendSimulatorChar(VirtualKeyCode.VK_X); } break;
                case "CmdC": { SendSimulatorChar(VirtualKeyCode.VK_C); } break;
                case "CmdV": { SendSimulatorChar(VirtualKeyCode.VK_V); } break;
                case "CmdB": { SendSimulatorChar(VirtualKeyCode.VK_B); } break;
                case "CmdN": { SendSimulatorChar(VirtualKeyCode.VK_N); } break;
                case "CmdM": { SendSimulatorChar(VirtualKeyCode.VK_M); } break;
          
                case "CmdLessThan": { SendSimulatorChar(VirtualKeyCode.OEM_COMMA); } break;
                case "CmdGreaterThan": { SendSimulatorChar(VirtualKeyCode.OEM_PERIOD); } break;
                case "CmdQuestion": { SendSimulatorChar(VirtualKeyCode.OEM_2); } break; 
  
                case "CmdSpaceBar" :
                  
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);                      // 0106-10
               
                    break;
                    
                case "CmdClear" :
                    UC_Keyboard.TouchScreenText = "";
                    break;

                case "CmdHiragana":
                    if (currentlang =="ja-JP")
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.CAPITAL);  // ok
                    else
                        UC_Keyboard.TouchScreenText = "";
                    break;

                case "CmdKatakana":
                    if (currentlang == "ja-JP")
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.CAPITAL);  // ok
                    else
                        UC_Keyboard.TouchScreenText = "";
                    break;

            } // switch
            #endregion

        }

        private static bool isInvalidKey(string s)                                              // 0106-12
        {
            bool r = false;
            switch  (ControlParams.Params.p_SecondLanguage )
            {
                case "es-MX":

                    if (s == "CmdColon" ) //|| s == "CmdDoubleInvertedComma")
                        r = true;
                    break;

                case "pt-PT":

                    if (s == "CmdColon" || s == "CmdQuestion")
                        r = true;
                    break;

                case "fr-FR":

                    if (s == "CmdMinus" || s == "CmdEndCrultBrace")
                        r = true;
                    break;
                   
                case "de-DE":   // 0106-13

                    if (s == "CmdEndCrultBrace" || s == "CmdOR")
                        r = true;
                    break;

                case "ja-JP":
                case "ru-RU":
                    break;

            }

            return r;
        }
        #endregion

        public static readonly DependencyProperty TouchScreenKeyboardProperty =
         DependencyProperty.RegisterAttached("TSKeyBoard", typeof(bool), typeof(UC_Keyboard), new UIPropertyMetadata(default(bool), TSKeyboardPropertyChanged));

        public static void TSKeyboardPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement host = sender as FrameworkElement;
            if (host != null)
            {
                //host.GotFocus += new RoutedEventHandler(OnGotFocus);
                //host.LostFocus += new RoutedEventHandler(OnLostFocus);
            }
        }

        public void SendKey(object sender, RoutedEventArgs e)
        {
            if (_CurrentControl is TextBox)
            {
                var button = sender as Button;
                var s = button.Tag;
                VerifyParam(s.ToString(), CurrentLanguage); 
            }

            else if (_CurrentControl is PasswordBox)
            {
                var button = sender as Button;
                var s = button.Tag;
                VerifyParam(s.ToString(), CurrentLanguage);
            }
        }

        private static void SendToKeyboard(string str)                                          // 0106-10
        {
            UC_Keyboard._CurrentControl.Focus();

            new System.Threading.Thread(
            new System.Threading.ThreadStart(() =>
            {
                System.Windows.Forms.SendKeys.SendWait(str);
            })).Start();
        }

        private static void SendToKeyboard(string str, string strwithshift)                     // 0106-10  not in use
        {
            if (!ShiftFlag)
                SendToKeyboard(str);
            else
            {
                SendToKeyboard(strwithshift);
                ShiftFlag = false;
            }
        }

        private static void SendSimulator(VirtualKeyCode VK)                                    // 0106-10
        {
            if (ShiftFlag)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.SHIFT);
                InputSimulator.SimulateKeyPress(VK);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT);   
                ShiftFlag = false;
            }
            else
                InputSimulator.SimulateKeyPress(VK);
        }

        private static void SendSimulatorChar(VirtualKeyCode VK)                                // 0106-10
        {
            if (ShiftFlag ^ CapsLockFlag)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.SHIFT);
                InputSimulator.SimulateKeyPress(VK);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT); 
                ShiftFlag = false;
            }
            else
                InputSimulator.SimulateKeyPress(VK);
        }

        private static void SendSimulator(VirtualKeyCode VK, VirtualKeyCode VKwithShift)        // 0106-10  not in use
        {
            UC_Keyboard._CurrentControl.Focus();

            if (ShiftFlag)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.SHIFT);
                InputSimulator.SimulateKeyPress(VKwithShift);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT);    
                ShiftFlag = false;
            }
            else
                InputSimulator.SimulateKeyPress(VK);
        }

        private static void AddKeyBoardINput_bk(char input)                                     // 0106-09
        {
            UC_Keyboard._CurrentControl.Focus();
 
            new System.Threading.Thread(
            new System.Threading.ThreadStart(() =>
            {
                System.Windows.Forms.SendKeys.SendWait(input.ToString());
            })).Start();

            return;



            if (CapsLockFlag)
            {
                if (ShiftFlag)
                {
                    UC_Keyboard.TouchScreenText += char.ToLower(input).ToString();
                    ShiftFlag = false;

                }
                else
                {
                    UC_Keyboard.TouchScreenText += char.ToUpper(input).ToString();
                }
            }
            else
            {
                if (!ShiftFlag)
                {
                    UC_Keyboard.TouchScreenText += char.ToLower(input).ToString();
                }
                else
                {
                    UC_Keyboard.TouchScreenText += char.ToUpper(input).ToString();
                    ShiftFlag = false;
                }
            }
        }

        #region not in use
        private void ButtonPress(string BTN)
        {
            switch (BTN)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (!ShiftFlag)
                        TouchScreenText += BTN;
                    else
                    {
                        if (_CurrentControl is TextBox)
                            ((TextBox)_CurrentControl).Text = ((TextBox)_CurrentControl).Text + BTN;
                        else
                        {
                            if (_CurrentControl is PasswordBox)
                            {
                                ((PasswordBox)_CurrentControl).Password = ((PasswordBox)_CurrentControl).Password + BTN;
                            }
                        }
                    }
                    break;
                case "a":
                case "b":
                case "c":
                case "d":
                case "e":
                case "f":
                case "g":
                case "h":
                case "i":
                case "j":
                case "k":
                case "l":
                case "m":
                case "n":
                case "o":
                case "p":
                case "q":
                case "r":
                case "s":
                case "t":
                case "u":
                case "v":
                case "w":
                case "x":
                case "y":
                case "z":

                    if (!ShiftFlag)
                        TouchScreenText += BTN;
                    else
                        if (_CurrentControl is TextBox)
                            ((TextBox)_CurrentControl).Text = ((TextBox)_CurrentControl).Text + BTN;
                        else
                        {
                            if (_CurrentControl is PasswordBox)
                            {
                                ((PasswordBox)_CurrentControl).Password = ((PasswordBox)_CurrentControl).Password + BTN;
                            }
                        }
                    break;

                case "Enter":
                case "BackSpace":
                case "Space":
                case "CapLock":
                case "Shift":
                    break;

            }
        }

        static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            Control host = sender as Control;
            host.Background = _PreviousTextBoxBackgroundBrush;
            host.BorderBrush = _PreviousTextBoxBorderBrush;
            host.BorderThickness = _PreviousTextBoxBorderThickness;
        }
        #endregion
 
        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)              // not in use
        {
            string str = ((Image)sender).Tag.ToString();

            if (str == "") return;

            if (_CurrentControl is TextBox)
            {
                VerifyParam(str, CurrentLanguage);                                              // 006-12
            }
            else if (_CurrentControl is PasswordBox)
            {
                VerifyParam(str, CurrentLanguage);                                              // 0106-12
            }
        }

        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)              // number 0-9  0106-12
        {
            string str = ((Label)sender).Tag.ToString();

            if (str == "") return;

            if(str=="CmdLanguage")

                SwitchLanguage();

            else if (_CurrentControl is TextBox)
            {
                VerifyParam(str, CurrentLanguage);                                              // 0106-12
            }
            else if (_CurrentControl is PasswordBox)
            {
                VerifyParam(str, CurrentLanguage);                                              // 0106-12
            }
        }

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            SwitchLanguage();
        }
  
        private void SwitchLanguage()
        {
            try
            {
                if (InputLanguageManager.Current.CurrentInputLanguage.Name == "en-US")
                {
                    InputLanguageManager.Current.CurrentInputLanguage =
                        new CultureInfo(ControlParams.Params.p_SecondLanguage);

                    CurrentLanguage = ControlParams.Params.p_SecondLanguage;
                    BorderVisible = true; 
                }
                else
                {
                    InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
                    
                    CurrentLanguage = "en-US";
                    BorderVisible = false;    // hide the second language keyboard
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Language!");
            }
        }

        private void SetHiraganaMode()
        {
            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.CAPITAL);      // ok
        }

        private void SetKatakanaMode()
        {
            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.CAPITAL);   // ok
        }
       
        private void lblChar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string str = ((Label)sender).Tag.ToString();

            if (str == "") return;

            if (_CurrentControl is TextBox)
            {
                VerifyParam(str, CurrentLanguage);                                              // 0106-12
            }
        }
       
    }
}
