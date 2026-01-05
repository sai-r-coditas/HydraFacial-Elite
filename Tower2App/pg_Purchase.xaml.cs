using System.Windows;
using System.Windows.Controls;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for pg_Purchase.xaml
    /// </summary>
    public partial class pg_Purchase : Page
    {
        public pg_Purchase()
        {
            InitializeComponent();

            Utility.Lib.LoadImageNoLock(imgDe_purchase, "\\Skin\\Images\\"+ ControlParams.Params.p_SecondLanguage + "\\de_purchase.png"); // 0106-05  0106-15
        }

        public App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.Printer);
        }
    }
}
