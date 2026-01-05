using System;
using System.Windows;
using System.IO;

using JetBrains.Annotations;
using System.Text;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfDemo.xaml
    /// </summary>
    public partial class wpfDemo : Window
    {
        public wpfDemo()   // 0102-06
        {
            InitializeComponent();
            
            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            lblMessage.Visibility = Visibility.Hidden;  // 0102-09
        }

        private void OnEnter()  
        {
            UC_Keyboard.CapsLockFlag = false;
            UC_Keyboard._CurrentControl = txtCode;  // 0102-08
            lblMessage.Visibility = Visibility.Hidden;  // 0102-09
        }

        private void OnLeave()
        {

        }

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        #region Button Control
        private void btnVerify_Click(object sender, RoutedEventArgs e)                          // 0102-06
        {
            if (txtCode.Text.Length == 10 && isValidDemoCode16(txtCode.Text) && !isInDamoCodeLog(txtCode.Text.Substring(2, 2)))  // ??  for T2
            {
                ControlParams.Params.p_BottleCountOn = false;                                   // disable bottle counting
                ControlParams.Params.p_DemoMode = true;
                ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDemo.Visibility = Visibility.Visible;
                ((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Visibility = Visibility.Visible;

                ((Home)App._mainWindows[Mode.Home]).lblDemo.Visibility = Visibility.Visible;    // 0102-09
                ((Home)App._mainWindows[Mode.Home]).lblDayRemain.Visibility = Visibility.Visible;  // 0102-09

                SaveDemoCode(Convert.ToInt32(getHourRemain()) + Convert.ToInt32(txtCode.Text.Substring(0, 2)) * 24, txtCode.Text.Substring(2, 2));  //0102-09
                SaveDemoCodeLog(txtCode.Text.Substring(2, 2));

                txtCode.Text = "";                                                              // Clear TextBox

                App.Go(Mode.wpfSettings);                                                       // Back to settings
            }
            else
            {
                lblMessage.Visibility = Visibility.Visible;                                     // 0102-09
                txtCode.Text = ""; 

                //ControlParams.Params.p_BottleCountOn = true;
                //ControlParams.Params.p_DemoMode = false;
                //((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDemo.Visibility = Visibility.Hidden;
                //((HydraFacial)App._mainWindows[Mode.HydraFacial]).lblDayRemain.Visibility = Visibility.Hidden;
            }
        }

        private void txtCode_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;                                          // 0102-09
            UC_Keyboard1.Visibility = Visibility.Visible;
            UC_Keyboard._CurrentControl = txtCode;
        }
        #endregion

        #region DemoCode Control
        private bool isValidDemoCode(string code)
        {
            string str = code; // "0581487885675f";
            if (str.Substring(6) ==CheckCode(str.Substring(0, 6)))
                return true;
            else
                return false;
        }

        // for crc32
        private string CheckCode(string s)  // for DemoCode
        {
            String hash = String.Empty;

            byte[] fs = System.Text.Encoding.ASCII.GetBytes(s);

            c_Dongle n_crc32 = new c_Dongle();
            foreach (byte b in n_crc32.ComputeChecksumBytes(fs))
                hash += b.ToString("x2").ToLower();

            string data = hash.ToString();

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        private bool isValidDemoCode16(string code)
        {
            string str = code; // "0581487885675f";
            if (str.Substring(6) == getCRC16(str.Substring(0, 6)))
                return true;
            else
                return false;
        }
        private string getCRC16(string s)
        {
            String hash = String.Empty;
            byte[] fs = System.Text.Encoding.ASCII.GetBytes(s);

            N_crc16 n_crc16 = new N_crc16();
            foreach (byte b in n_crc16.ComputeChecksumBytes(fs))
                hash += b.ToString("x2").ToLower();

            string data = hash.ToString();

            return data;
        }

        private double getHourRemain()  //  Check current demo code valid, day remain, 
        {
            try
            {
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\disk\\dmcode.dat", Encoding.ASCII))  // 0106-09
                {
                    string line;
                    string dateExpired;
                    if ((line = file.ReadLine()) != null)
                    {
                        // skip first line 

                        if ((dateExpired = file.ReadLine()) != null)
                        {
                            DateTime dt = Convert.ToDateTime(dateExpired);
                            if (dt > DateTime.Now)
                            {
                                TimeSpan diff = dt - DateTime.Now;
                          
                                return diff.TotalHours;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        // Save Demo Code Date Expired, Add current day
        public void SaveDemoCode(int noOfHours, string newCode)    // 0102-06
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\disk\\dmcode.dat", false, Encoding.ASCII))  // 0106-09
                {
                    sw.WriteLine(newCode);      
                    DateTime today = DateTime.Now;
                    DateTime answer = today.AddHours(noOfHours);  // 0102-07
                    sw.WriteLine(answer.ToString("yyyy/MM/dd HH:mm:ss"));  // write expire day
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data save failed!");
            }
        }

        // Write serial demo code to log 
        private void SaveDemoCodeLog(string newCode)  // 0102-06
        {
            try
            {
                //using (StreamWriter sw = File.AppendText(Environment.CurrentDirectory + "\\disk\\dml.dat"))
                using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\disk\\dml.dat", true, Encoding.ASCII))   // 0106-09
                {
                    sw.WriteLine(newCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data save failed!");
            }
        }

        // Check if the Demo code has been used, 
        private bool isInDamoCodeLog(string Code)
        {
            try
            {
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\disk\\dml.dat", Encoding.ASCII))  // 0106-09
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line == Code) // found it has been used, so reject it.
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        private void Window_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;  // 0102-06
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //IsVisibleChanged -= (, ) => { };
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            NavBar.setVolume(ControlParams.Params.p_AudioVolume);
            lblMessage.Visibility = Visibility.Hidden;  // 0102-09
            txtCode.Text = ""; // 0102-09
        }
    }
}
