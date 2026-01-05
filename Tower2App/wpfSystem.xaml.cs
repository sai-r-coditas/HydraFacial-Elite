using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using System.Windows.Media;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfSystem.xaml
    /// </summary>
    public partial class wpfSystem : Window
    {
        public wpfSystem()
        {
            InitializeComponent();

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            lblMessage.Visibility = Visibility.Hidden;

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\WindowBackground_datetimechanges.png");   // 0106-09 0106-13
        }
        
        private void OnEnter()
        {
            gettime();
        }

        private void OnLeave()
        {
            
        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        DateTime l_datetime; // Keep app loaded datetime
        private void gettime()
        {
            l_datetime = DateTime.Now;
            txtYear.Text = l_datetime.ToString("yyyy");
            txtMonth.Text = l_datetime.ToString("MM");
            txtDay.Text = l_datetime.ToString("dd");

            txtHour.Text = l_datetime.ToString("HH");
            txtMinute.Text = l_datetime.ToString("mm");
            txtSecond.Text = l_datetime.ToString("ss");
        }
 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gettime();
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)   // 0106-09
        {
            SYSTEMTIME time = new SYSTEMTIME();

            if (txtYear.Text.Length != 4 || 
                txtYear.Text == "" ||
                txtMonth.Text == "" ||
                txtDay.Text == "" ||
                txtHour.Text == "" ||
                txtMinute.Text == "" ||
                txtSecond.Text == "" ||
                !isYearValid(txtYear.Text))                                                     // 00115a1-0006
            {
                ShowMessage(App.getTextMessages("Invalid data!"),true);  // 0106-06
                return;
            }

            try
            {
                time.wYear = Convert.ToUInt16(txtYear.Text);
                time.wMonth = Convert.ToUInt16(txtMonth.Text);
                time.wDay = Convert.ToUInt16(txtDay.Text);

                time.wHour = Convert.ToUInt16(txtHour.Text);
                time.wMinute = Convert.ToUInt16(txtMinute.Text);
                time.wSecond = Convert.ToUInt16(txtSecond.Text);

                StringBuilder builder = new StringBuilder();
                builder.Append(txtMonth.Text);
                builder.Append("/");
                builder.Append(txtDay.Text);
                builder.Append("/");
                builder.Append(txtYear.Text);
                builder.Append(" ");
                builder.Append(txtHour.Text);
                builder.Append(":");
                builder.Append(txtMinute.Text);
                builder.Append(":");
                builder.Append(txtSecond.Text);

                string str = builder.ToString();

                if (!checkdateformat(str))
                {
                    ShowMessage(App.getTextMessages("Invalid data!"),true);  // 0106-06
                    return;
                }

                App.wpfyesno.WarningMessage(App.getTextMessages("Change date time, are you sure?"));
                App.wpfyesno.ShowDialog();

                var ans = App.wpfyesno.ans;
                if (ans != "yes")
                {
                    return;
                }

                // =======================================

                // If current time is less the state.bin, then 
                // set the local time and init the credit code.
                // else verify the remaining days and save datetime 
                // to state.bin 

                string dateTime = txtMonth.Text+"/"+txtDay.Text+"/"+txtYear.Text+" "+ txtHour.Text+":"+txtMinute.Text +":"+txtSecond.Text;
                DateTime inputdatetime = Convert.ToDateTime(dateTime);

                l_datetime = DateTime.Now;

                if (inputdatetime == l_datetime)
                    return;

                if (inputdatetime > l_datetime)
                {
                   
                    // set new expired date of creditcode
                    DateTime currenydatetime;
                    currenydatetime = DateTime.Now;

                    // get creditcode remaining days
                    DateTime expiredatetime = getTimeExpired();
                    
                    double maxminutesallow;
                    double totalminutes;

                    maxminutesallow = Convert.ToDateTime("12/31/9999 23:59:59.9999999").Subtract(expiredatetime).TotalMinutes;

                    #region Checkexpiredatetime
                    
                    if (expiredatetime < Convert.ToDateTime("12/31/9999 23:59:59.9999999"))
                    {
                        // time remain
                        totalminutes = expiredatetime.Subtract(currenydatetime).TotalMinutes;

                        if (totalminutes < maxminutesallow )
                        {
                            currenydatetime = inputdatetime.AddMinutes(totalminutes);

                            if (!SetLocalTime(ref time))
                            {
                                // if the native function call failed, throw an exception
                                ShowMessage(App.getTextMessages("System protected or invalid data!"), true); // 0106-06
                                
                                return;
                            }

                            UpdateCreditCodeDateTime(currenydatetime);

                        }
                        else
                        {
                            // no update required on credit code
 
                            if (!SetLocalTime(ref time))
                            {
                                // if the native function call failed, throw an exception
                                ShowMessage(App.getTextMessages("System protected or invalid data!"), true); // 0106-06

                                return;
                            }
                        }
                    }
                    else
                    {
                        // For condition of expiredatetime >= Convert.ToDateTime("12/31/9999 23:59:59.9999999"), do nothing

                        if (!SetLocalTime(ref time))
                        {
                            // if the native function call failed, throw an exception
                            ShowMessage(App.getTextMessages("System protected or invalid data!"), true); // 0106-06

                            return;
                        }
                    }
                    
                    #endregion
                }
                else
                {   
                    // When input datetime less than current time
                    // do nothing, use original creditcode to take care
                    if (!SetLocalTime(ref time))
                    {
                        // If the native function call failed, throw an exception
                        // throw new Win32Exception(Marshal.GetLastWin32Error());
                        ShowMessage(App.getTextMessages("System protected or invalid data!"), true); // 0106-06

                        return;
                    }
                }

  
                ShowMessage(App.getTextMessages("Data saved!"), false); // 0106-06

                //((Home)App._mainWindows[Mode.Home]).StartCreditCodeExpiredTimer();

                App.Init_creditcode(false);
            }
            catch (Exception ex)
            {
                ShowMessage(App.getTextMessages("Invalid data!"),true);  // 0106-06
                return;
            }
        }

        private DateTime getTimeExpired()
        {
            CreditCodeState cs;

            cs = CreditCodeState.LoadState(false);  
            DateTime d = Convert.ToDateTime(cs.Kosciusko);
            return d;
        }

        private void UpdateCreditCodeDateTime(DateTime d)
        {
            CreditCodeState cs;

            cs = CreditCodeState.LoadState(false);
            cs.Kosciusko = d;
            cs.SaveState();
        }

        private void ShowMessage(string str, bool ColorRed)
        {
            lblMessage.Content = str;

            if (ColorRed)
                lblMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC1212"));  // Red
            else
                lblMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00a5e2"));  // Blue  

            lblMessage.Visibility = Visibility.Visible;
            
            DoEvents();

            System.Threading.Thread.Sleep(3000);

            lblMessage.Visibility = Visibility.Hidden;
        }

        private bool checkdateformat(string strDate) // 0106-06
        {
            DateTime value;
 
            if (!DateTime.TryParse(strDate, out value))
                return false;
           
            return true;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetLocalTime(ref SYSTEMTIME lpSystemTime);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;    // ignored for the SetLocalTime function
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
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
            else if ((sender as Button) == btnDel)
                DeleteChar();
        }

        private TextBox txtbox;
        private void SetText(string str)
        {
            if (txtbox != null)
            {
                if(txtbox.Text.Length < txtbox.MaxLength )
                   txtbox.Text = txtbox.Text + str;
            }
        }

        private void DeleteChar()
        {
            if (txtbox != null)
            {
                if (txtbox.Text.Length != 0)
                    txtbox.Text = txtbox.Text.Remove(txtbox.Text.Length - 1);
            }
        }

        private bool isYearValid(string Year)                                                   // 00115a1-0006
        {
            try
            {
                bool isYearValid = !string.Equals(Year, "all", StringComparison.OrdinalIgnoreCase);

                if (isYearValid)
                {
                    if (Convert.ToInt32(Year) >= 2099)
                        isYearValid = false;
                }
                return isYearValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void txtYear_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtYear.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            txtbox = txtYear;
        }

        private void txtMonth_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtMonth.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            txtbox = txtMonth;
        }

        private void txtDay_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtDay.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            txtbox = txtDay;
        }

        private void txtHour_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtHour.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            txtbox = txtHour;
        }

        private void txtMinute_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtMinute.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            txtbox = txtMinute;
        }

        private void txtSecond_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSecond.CaretBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            txtbox = txtSecond;
        }

        private void txtMonth_KeyDown(object sender, KeyEventArgs e)
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

        private void txtDay_KeyDown(object sender, KeyEventArgs e)
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

        private void txtYear_KeyDown(object sender, KeyEventArgs e)
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

        private void txtHour_KeyDown(object sender, KeyEventArgs e)
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

        private void txtMinute_KeyDown(object sender, KeyEventArgs e)
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

        private void txtSecond_KeyDown(object sender, KeyEventArgs e)
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

        [DllImport("user32.dll")]  // 0106-06
        static extern bool HideCaret(IntPtr hWnd);
       
    }
}
