using System;
using System.ComponentModel;
using System.Windows;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for pgPulseFusion.xaml
    /// </summary>
    public partial class pg_PulseFusion 
    {
        public pg_PulseFusion()
        {
            InitializeComponent();

            btnOn.Visibility = Visibility.Hidden;
            btnOff.Visibility = Visibility.Hidden;
        }

        public App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void OnLeave()
        {

            App.Outputs.VacuumPump = false;

            App.Outputs.PulseDuration = 0;
            App.Outputs.ExMass3 = false;

            App.Outputs.DC_Motor = false;                                                       // 2014 07/21 

            Outputs.LogHeader("VacJet", "Exit");
        }

        private void BoardManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                switch (e.PropertyName)
                {
                    case "VacuumAtoD":
                        break;

                    case "PowerPressed":
                        break;
                }
            }));
        }

        // Not in use
        private void SetpointUnchecked(object sender, RoutedEventArgs e)
        {
            if (App.Outputs.PropertyIsChanging)
                return;

            var cb = (sender as System.Windows.Controls.CheckBox);

            if (cb.IsFocused)
            {
                cb.IsChecked = true;
                e.Handled = true;
            }
        }

        private void btn_H_Click(object sender, RoutedEventArgs e)
        {
            uiPulseSelect.chkHIGH.IsChecked = true;
            HighlightSelection("H");
        }

        private void btn_M_Click(object sender, RoutedEventArgs e)
        {
            uiPulseSelect.chkMED.IsChecked = true;
            HighlightSelection("M");
        }

        private void btn_L_Click(object sender, RoutedEventArgs e)
        {
            uiPulseSelect.chkLOW.IsChecked = true;
            HighlightSelection("L");
        }

        private void btnOn_Click(object sender, RoutedEventArgs e)
        {
            App.Outputs.ExMass4 = true;
        }

        private void btnOff_Click(object sender, RoutedEventArgs e)
        {
            App.Outputs.ExMass4 = false;
        }

        private void HighlightSelection(string Mode)
        {
            Utility.Lib.LoadImageFromAppDir(imgH, "/Skin/images/t2/Pulse-graph_G_03.png");
            Utility.Lib.LoadImageFromAppDir(imgM, "/Skin/images/t2/Pulse-graph_G_02.png");
            Utility.Lib.LoadImageFromAppDir(imgL, "/Skin/images/t2/Pulse-graph_G_01.png");
            if (Mode=="H")
                Utility.Lib.LoadImageFromAppDir(imgH, "/Skin/images/t2/Pulse-graph_B_03.png");
            else if (Mode == "M")
                Utility.Lib.LoadImageFromAppDir(imgM, "/Skin/images/t2/Pulse-graph_B_02.png");
            else if (Mode == "L")
                Utility.Lib.LoadImageFromAppDir(imgL, "/Skin/images/t2/Pulse-graph_B_01.png");

        }
      }
}
