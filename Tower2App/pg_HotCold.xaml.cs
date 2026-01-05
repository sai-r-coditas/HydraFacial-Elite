using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfSplash.xaml
    /// </summary>
    public partial class pg_HotCold    
    {
        public pg_HotCold()
        {
            InitializeComponent();

            lblHotCold.Visibility = Visibility.Hidden;
            lblHot.Visibility = Visibility.Hidden;
            lblCold.Visibility = Visibility.Hidden;
            lbl_Title.Visibility = Visibility.Hidden;

            // Set default to Cold
            cvsCold.Visibility = Visibility.Hidden;
            cvsHot.Visibility = Visibility.Hidden;

            Utility.Lib.LoadImageFromAppDir(imgCold, "/Skin/Images/t2/Cold_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHot, "/Skin/Images/t2/hot_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHotColdOff, "/Skin/Images/t2/hotcold_u.png");
           
            imgL.Tag = "";
            imgM.Tag = "";
            imgH.Tag = "";

            imgHotColdOff.Visibility = Visibility.Hidden; // 0101

            // 0101
            cvslblHot.Visibility = Visibility.Hidden;
            cvslblCold.Visibility = Visibility.Hidden;
            cvsHotColdButton.Visibility = Visibility.Hidden;
            cvsSelectDegree.Visibility = Visibility.Hidden;
        }

        public App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void OnEnter()
        {
            
        }

        private void OnLeave()
        {
            App.Outputs.VacuumPump = false;
            App.Outputs.PulseDuration = 0;
            App.Outputs.ExMass1 = false;

            Outputs.LogHeader("HotCold", "Exit");
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

        // 0020-01
        public void SetDefaultColor(int mode)
        {
            if (mode == 1)
                cvsBackColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffffff"));
            else
                cvsBackColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFf5f0eb"));
        }

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

        private void Selection_Default_Single(System.Windows.Controls.Image C, string filename)
        {
            Utility.Lib.LoadImageFromAppDir(C, filename);
            C.Tag = "";
        }

        private void Me_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
      
        private void HighLightSelection(string Mode )
        {
            Utility.Lib.LoadImageFromAppDir(imgL, "/Skin/images/t2/Temp-graph0_01.png");
            Utility.Lib.LoadImageFromAppDir(imgM, "/Skin/images/t2/Temp-graph0_02.png");
            Utility.Lib.LoadImageFromAppDir(imgH, "/Skin/images/t2/Temp-graph0_03.png");
            imgL.Tag = "";
            imgM.Tag = "";
            imgH.Tag = "";

            if (ControlParams.Params.p_HotColdSelected == "COLD")
            {
                if (Mode == "H")
                {
                    Utility.Lib.LoadImageFromAppDir(imgH, "/Skin/images/t2/Temp-graph1_03.png");
                    imgH.Tag = "on";
                }
                else if (Mode == "M")
                {
                    Utility.Lib.LoadImageFromAppDir(imgM, "/Skin/images/t2/Temp-graph1_02.png");
                    imgM.Tag = "on";
                }
                else if (Mode == "L")
                {
                    Utility.Lib.LoadImageFromAppDir(imgL, "/Skin/images/t2/Temp-graph1_01.png");
                    imgL.Tag = "on";
                }
            }
            else if (ControlParams.Params.p_HotColdSelected == "HOT")
            {
                if (Mode == "H")
                {
                    Utility.Lib.LoadImageFromAppDir(imgH, "/Skin/images/t2/Temp-graph2_03.png");
                    imgH.Tag = "on";
                }
                else if (Mode == "M")
                {
                    Utility.Lib.LoadImageFromAppDir(imgM, "/Skin/images/t2/Temp-graph2_02.png");
                    imgM.Tag = "on";
                }
                else if (Mode == "L")
                {
                    Utility.Lib.LoadImageFromAppDir(imgL, "/Skin/images/t2/Temp-graph2_01.png");
                    imgL.Tag = "on";
                }
            }   
        }

        private void imgHot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Utility.Lib.LoadImageFromAppDir(imgHot, "/Skin/Images/t2/hot_d.png");
            Utility.Lib.LoadImageFromAppDir(imgCold, "/Skin/Images/t2/cold_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHotColdOff, "/Skin/Images/t2/hotcold_off_u.png");

            // Set labels 0101
            //cvslblHot.Visibility = Visibility.Visible;
            //cvslblCold.Visibility = Visibility.Hidden;
            //cvsHotColdButton.Visibility = Visibility.Visible;
            //cvsSelectDegree.Visibility = Visibility.Visible;
            
            //??
            //CbTemperatureOff.IsChecked = true;

            ControlParams.Params.p_HotColdSelected = "HOT";

            setTemp(ControlParams.Params.p_HotColdSelected);
        }

        private void imgCold_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Utility.Lib.LoadImageFromAppDir(imgCold, "/Skin/Images/t2/cold_d.png");
            Utility.Lib.LoadImageFromAppDir(imgHot, "/Skin/Images/t2/hot_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHotColdOff, "/Skin/Images/t2/Hotcold_off_u.png");

            // Set labels  0101
            //cvslblHot.Visibility = Visibility.Hidden;
            //cvslblCold.Visibility = Visibility.Visible;
            //cvsHotColdButton.Visibility = Visibility.Visible;
            //cvsSelectDegree.Visibility = Visibility.Visible;
 
            //CbTemperatureOff.IsChecked = true;

            ControlParams.Params.p_HotColdSelected ="COLD";

            setTemp(ControlParams.Params.p_HotColdSelected);
        }

        private void setTemp(string HotCold)
        {
            if (HotCold == "HOT")
            {    
                if( ControlParams.Params.p_LastHotTempSelected =="H")
                    imgH_MouseDown(null,null);
                else if( ControlParams.Params.p_LastHotTempSelected =="M")
                    imgM_MouseDown(null,null);
                else if( ControlParams.Params.p_LastHotTempSelected =="L")
                    imgL_MouseDown(null,null);
            }
            else if (HotCold == "COLD")
            {    
                if( ControlParams.Params.p_LastColdTempSelected =="H")
                    imgH_MouseDown(null,null);
                else if( ControlParams.Params.p_LastColdTempSelected =="M")
                    imgM_MouseDown(null,null);
                else if( ControlParams.Params.p_LastColdTempSelected =="L")
                    imgL_MouseDown(null, null);
            }
        }

        private void imgHotColdOff_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Utility.Lib.LoadImageFromAppDir(imgCold, "/Skin/Images/t2/cold_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHot, "/Skin/Images/t2/hot_u.png");
            Utility.Lib.LoadImageFromAppDir(imgHotColdOff, "/Skin/Images/t2/hotcold_off_d.png");

            // Set labels
            cvslblHot.Visibility = Visibility.Hidden;
            cvslblCold.Visibility = Visibility.Hidden;
            cvsHotColdButton.Visibility = Visibility.Hidden;
            cvsSelectDegree.Visibility = Visibility.Hidden;

            HighLightSelection("None");

            CbTemperatureOff.IsChecked = true;

            ControlParams.Params.p_HotColdSelected ="NONE";
        }

        private void imgH_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (ControlParams.Params.p_HotColdSelected == "COLD")
            {
                CbTemperature50.IsChecked = true;
                ControlParams.Params.p_LastColdTempSelected = "H";
                HighLightSelection(ControlParams.Params.p_LastColdTempSelected);
            }
            else if (ControlParams.Params.p_HotColdSelected == "HOT")
            {
                CbTemperature110.IsChecked = true;
                ControlParams.Params.p_LastHotTempSelected = "H";
                HighLightSelection(ControlParams.Params.p_LastHotTempSelected);
            }
           
        }

        private void imgM_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (ControlParams.Params.p_HotColdSelected == "COLD")
            {
                CbTemperature45.IsChecked = true;
                ControlParams.Params.p_LastColdTempSelected = "M";
                HighLightSelection(ControlParams.Params.p_LastColdTempSelected);
            }
            else if (ControlParams.Params.p_HotColdSelected == "HOT")
            {
                CbTemperature105.IsChecked = true;
                ControlParams.Params.p_LastHotTempSelected = "M";
                HighLightSelection(ControlParams.Params.p_LastHotTempSelected);
            }
            
        }

        private void imgL_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (ControlParams.Params.p_HotColdSelected == "COLD")
            {
                CbTemperature40.IsChecked = true;
                ControlParams.Params.p_LastColdTempSelected = "L";
                HighLightSelection(ControlParams.Params.p_LastColdTempSelected);
            }
            else if (ControlParams.Params.p_HotColdSelected == "HOT")
            {
                CbTemperature100.IsChecked = true;
                ControlParams.Params.p_LastHotTempSelected = "L";
                HighLightSelection(ControlParams.Params.p_LastHotTempSelected);
            }
           
        }

        public void setHotColdDefaultMode(string HotCold)
        {
            ControlParams.Params.p_LastHotTempSelected ="H";                                    // 0020-06
            ControlParams.Params.p_LastColdTempSelected = "L";                                  // 0020-06

             if (HotCold == "HOT")
            {
                imgHot_MouseDown(null, null);
               
            }
            else if (HotCold == "COLD")
            {
                imgCold_MouseDown(null, null);
            }
            else
            {
                imgHotColdOff_MouseDown(null, null);
            }
        }
    }
}
