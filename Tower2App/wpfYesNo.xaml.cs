using System.Windows;
using System.Windows.Input;

using Window = System.Windows.Window;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfYesNo.xaml
    /// </summary>
    public partial class wpfYesNo : Window
    {
        public wpfYesNo()
        {
            InitializeComponent();
            
            // no image init here, because this dialog load very early
        }

        public void init()
        {
            Utility.Lib.LoadImageNoLock(imgPopup, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\popup.png");  // 0106-15
            Utility.Lib.LoadImageNoLock(imgTopmenu, "\\Skin\\Images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");      // 0106-06  0106-15
        }

        public string ans { get; set; }

        public void WarningMessage(string m)
        {
            this.Visibility = Visibility.Visible;
            this.Topmost = true;
            tbkMessage.Text = m;
        }

        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            tbkMessage.Text = "";  // 0103-07
        }

        private void lblYes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ans = "yes";
            this.Topmost = false;   //0102-20
            this.Visibility = Visibility.Hidden;
        }

        private void lblNo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ans = "no";
            this.Topmost = false;  //0102-20
            this.Visibility = Visibility.Hidden;
        }
    }
}
