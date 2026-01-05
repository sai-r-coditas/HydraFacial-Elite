
using System.Windows;
using JetBrains.Annotations;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfInfo.xaml
    /// </summary>
    public partial class wpfInfo : Window
    {
        public wpfInfo()
        {
            InitializeComponent();
          
            // 0102-39
            Utility.Lib.LoadImageNoLock(imgBG1,  @"/Skin/images/t2/popup_hot.png");
            Utility.Lib.LoadImageNoLock(imgBG2,  @"/Skin/images/t2/popup_cold.png");
            Utility.Lib.LoadImageNoLock(imgBG3,  @"/Skin/images/t2/popup_cold.png");
        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private string currentMode { set; get; }

        public void Page_init(string mode)
        {
            imgBG0.Visibility = Visibility.Collapsed;
            imgBG1.Visibility = Visibility.Collapsed;
            imgBG2.Visibility = Visibility.Collapsed;
            imgBG3.Visibility = Visibility.Collapsed;
            
            if (mode=="SuggestPattern") 
                 imgBG0.Visibility= Visibility.Visible;
            else if (mode == "Hot") 
                 imgBG1.Visibility = Visibility.Visible;
            else if (mode == "Cold") 
                imgBG2.Visibility = Visibility.Visible;
            else if (mode == "BottleInsertion")  // message for bottle insertion over the limitation
            {
                imgBG3.Visibility = Visibility.Visible;
            }

            currentMode = mode;
            winMain.Visibility = Visibility.Visible;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (currentMode != "SuggestPattern")
            {
                // For HOT or COLD , click close means  turn off HOT/COLD 
                if (ControlParams.Params.p_HotColdSelected == "")
                    ControlParams.Params.p_HotColdSelected = "NONE";
            }
            App.Go(Mode.Home);
        }

        // When Hot Button Selected
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            if ( currentMode == "Hot")
                ControlParams.Params.p_HotColdSelected ="HOT";
            else if (currentMode == "Cold")
                ControlParams.Params.p_HotColdSelected = "COLD";

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).setHotCold(ControlParams.Params.p_HotColdSelected);

            App.Go(Mode.Home);
        }
    }
}
