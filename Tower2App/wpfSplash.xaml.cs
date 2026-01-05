using System;
using System.Windows;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfSplash.xaml
    /// </summary>
    public partial class wpfSplash : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private int pgb_V { set; get; }

        public wpfSplash()
        {
            InitializeComponent();

            // 2014 12/04
            pgb_V = 2;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\Edge_Logo_1280.png");  // 0102-39
            lblLoading.Visibility = Visibility.Hidden;  // 0103-06
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            pgbLoading.Value = pgb_V;
            pgb_V = pgb_V + 1;

            if (pgb_V > 10)
            {
                pgb_V = 10;
                dispatcherTimer.Stop();
                this.Visibility = Visibility.Hidden;  // 0100
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= dispatcherTimer_Tick;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
