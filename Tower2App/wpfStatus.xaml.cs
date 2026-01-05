using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

// For DispatcherPriority
using System.Windows.Threading;
using System.Threading;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfStatus.xaml
    /// </summary>
    public partial class wpfStatus : INotifyPropertyChanged
    {
        private readonly string[] _bottleNames = { "", "", "", "", "" };
        private readonly Queue<System.Windows.Controls.CheckBox> _selectedCheckBoxes = new Queue<System.Windows.Controls.CheckBox>();
        private bool _inRinseCheckHandler;

        public event PropertyChangedEventHandler PropertyChanged;
        public wpfStatus()
        {
            InitializeComponent();

            App.BottleChanged += AppBottleChanged;

            this.DataContext = ((HydraFacial)App.currentWindow);

            Utility.Lib.LoadImageNoLock(imgClose, "\\Skin\\Images\\Close.png");   // 0106-05
        }

        private string pd_name { get; set; }
        private string pd_size { get; set; }
        private void getProductSize(string Str)
        {
            try
            {
                if (Str == "" || Str == null)
                {
                    pd_name = "";
                    pd_size = "";
                    return;
                }
                if (Str.IndexOf("(") == -1 || Str.IndexOf(")") == -1)
                {
                    pd_name = Str;
                    pd_size = "";
                    return;
                }

                pd_name = Str.Substring(0, Str.IndexOf("(") - 1);
                pd_size = Str.Substring(Str.IndexOf("("), Str.IndexOf(")") - Str.IndexOf("(") + 1);
            }
            catch (Exception e)
            {
                pd_name = "";
                pd_size = "";
            }
        }

        private App App
        {
            get { return (App)Application.Current; }
        }

        public string[] BottleNames
        {
            get { return _bottleNames; }
        }

        // Add by sww
        public string[] BottleSize
        {
            get { return _bottleNames; }
        }

        private void AppBottleChanged(object sender, BottleChangedEventArgs ea)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                // T2
                //AppBottleChanged_Run(ea);
            }));
        }

        private void AppBottleChanged_Run(BottleChangedEventArgs ea)
        {
            
        }

        public void Page_init()
        {
            HotCold_HighLight(ControlParams.Params.p_HotColdSelected);
        }

        private void HotCold_HighLight(string mode)
        {
            Utility.Lib.LoadImageFromAppDir(imgHot, "/Skin/Images/t2/hot_u.png");
            Utility.Lib.LoadImageFromAppDir(imgCold, "/Skin/images/t2/cold_u.png");
            
            if (mode =="HOT")
                Utility.Lib.LoadImageFromAppDir(imgHot, "/Skin/images/t2/hot_d.png");
            else if (mode =="COLD")
                Utility.Lib.LoadImageFromAppDir(imgCold, "/Skin/images/t2/cold_d.png");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // T2
            // App.Go(Mode.Home);
            this.Visibility = Visibility.Hidden;
        }

        private void imgHot_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgCold_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        public void setBattery(int[] V, bool[] Charging)
        {
            // 2014 11/18 for status page
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                try
                {
                    pgbB0.Value = V[0];
                    pgbB1.Value = V[1];
                    pgbB2.Value = V[2];
                    pgbB3.Value = V[3];

                    // 2014 /12/08
                    lblBatteryLevel0.Content = Avoid0and100(V[0]);
                    lblBatteryLevel1.Content = Avoid0and100(V[1]);
                    lblBatteryLevel2.Content = Avoid0and100(V[2]);
                    lblBatteryLevel3.Content = Avoid0and100(V[3]);

                    ChangeFontColor(lblBatteryLevel0, Charging[0]);
                    ChangeFontColor(lblBatteryLevel1, Charging[1]);
                    ChangeFontColor(lblBatteryLevel2, Charging[2]);
                    ChangeFontColor(lblBatteryLevel3, Charging[3]);
                }
                catch
                {


                }
            }));
        }

        private string Avoid0and100(int value)
        {
            if (value == 0)
                return "~";
            else
                return value.ToString();
        }

        private void ChangeFontColor(System.Windows.Controls.Label L, bool value)
        {
            if (value)
                L.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF08C5D8"));
            else
                L.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA8A8A8"));

        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
