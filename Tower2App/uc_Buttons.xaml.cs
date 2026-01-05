using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using JetBrains.Annotations;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for uc_Buttons.xaml
    /// </summary>
    public partial class uc_Buttons : UserControl
    {
        public uc_Buttons()
        {
            InitializeComponent();

            btnPulseFusion.Visibility = Visibility.Hidden; //0102-36
            btnThermalTherapy.Visibility = Visibility.Hidden; //0102-36
        }

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        public string SelectedButton
        {
            get { return (string)GetValue(SelectedButtonProperty); }
            set
            {
                SetValue(SelectedButtonProperty, value);

                var control = (Control)this.FindName(value);
                
                // 2014 09/02
                if (control !=null)
                {
                    SetButtonBackColor(control); // Highlight  the color
                }
             }
        }

        public static readonly DependencyProperty SelectedButtonProperty =
            DependencyProperty.Register("SelectedButton", typeof(string), typeof(uc_Buttons), new UIPropertyMetadata(null));

        private void btnBodyTherapy_Click(object sender, RoutedEventArgs e)
        {
            setAllButtons(btnBodyTherapy);
  
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.LymphaticBody;
            App.Go(Mode.HydraFacial);
        }
 
        private void btnPulseFusion_Click(object sender, RoutedEventArgs e)
        {
            setAllButtons(btnPulseFusion);
 
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.PulseFusion;
            App.Go(Mode.HydraFacial);
        }

        private void btnFacialTherapy_Click(object sender, RoutedEventArgs e)
        {
            setAllButtons(btnFacialTherapy);
            
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.LymphaticFacial;
            App.Go(Mode.HydraFacial);
        }

        private void btnVortexFusion_Click(object sender, RoutedEventArgs e)
        {
            ////SetButtonBackColor(btnVortexFusion);
            //BitmapImage btm = new BitmapImage(new Uri("/Skin/Images/vortex_fusion_on.png", UriKind.Relative));
            //Image img = new Image();
            //img.Source = btm;
            //img.Stretch = Stretch.Fill;
            //btnVortexFusion.Content = img;

            setAllButtons(btnVortexFusion);
           
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.VortexFusion; 
            App.Go(Mode.HydraFacial);
        }

        private void btnThermalTherapy_Click(object sender, RoutedEventArgs e)
        {
            // 2014 12/12
            this.IsEnabled = false;

            setAllButtons(btnThermalTherapy);
           
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.ThermalTherapy;
            App.Go(Mode.HydraFacial);
            this.IsEnabled = true;
        }

        private void btnLightTherapy_Click(object sender, RoutedEventArgs e)
        {
            setAllButtons(btnLightTherapy);
           
            ControlParams.Params.p_last_QS_Mode = ControlParams.Params.p_control_mode;
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.LightTherapy; 
            App.Go(Mode.HydraFacial);
        }

        private void SetButtonBackColor(System.Windows.Controls.Control B)
        {
            setAllButtons(B);
        }

        private void setAllButtons(System.Windows.Controls.Control B)
        {
            Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/Images/t2/vortex_fusion_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu2, "/Skin/Images/t2/pulse_fusion_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu3, "/Skin/Images/t2/thermal_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu4, "/Skin/Images/t2/light_therapy_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu5, "/Skin/Images/t2/facial_therapy_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu6, "/Skin/Images/t2/body_therapy_off.png");

            B.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffffff"));
            if (B == btnVortexFusion)
                Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/Images/vortex_fusion_on.png");
            else if (B == btnPulseFusion)
                Utility.Lib.LoadImageFromAppDir(imgSubMenu2, "/Skin/Images/pulse_fusion_on.png");
            if (B == btnThermalTherapy)
                Utility.Lib.LoadImageFromAppDir(imgSubMenu3, "/Skin/Images/thermal_on.png");
            if (B == btnLightTherapy)
                Utility.Lib.LoadImageFromAppDir(imgSubMenu4, "/Skin/Images/light_therapy_on.png");
            if (B == btnFacialTherapy)
                Utility.Lib.LoadImageFromAppDir(imgSubMenu5, "/Skin/Images/facial_therapy_on.png");
            if (B == btnBodyTherapy)
                Utility.Lib.LoadImageFromAppDir(imgSubMenu6, "/Skin/Images/body_therapy_on.png");

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/Images/t2/vortex_fusion_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu2, "/Skin/Images/t2/pulse_fusion_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu3, "/Skin/Images/t2/thermal_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu4, "/Skin/Images/t2/light_therapy_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu5, "/Skin/Images/t2/facial_therapy_off.png");
            Utility.Lib.LoadImageFromAppDir(imgSubMenu6, "/Skin/Images/t2/body_therapy_off.png");
        }
    }
}
