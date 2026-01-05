using System;
using System.Windows;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for Photo_Custom_Type.xaml
    /// </summary>
    public partial class Photo_Customer_Type2 
    {
        private App App
        {
            get { return (App)Application.Current; }
        }

        public Photo_Customer_Type2()
        {
            InitializeComponent();

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\member1.png");  // 0106-05 0106-13

            Utility.Lib.LoadImageNoLock(imgTopMenu, "\\Skin\\Images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");  // 0102-39  0106-06

            // sww, add document
            page_init();
        }

        private void page_init()
        {
           
        }
  
        private void btnExistCustomer_Click(object sender, RoutedEventArgs e)
        {
            ControlParams.Params.p_NewCustomer = false;
           
            // Find the Exist Customer
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Search;
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Search;
            App.Go(Mode.Photo_Customer_Search);
        }

        private void btnNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            // New Customer
            ControlParams.Params.PhotoCapture_Session = ControlParams.e_Session.Create;
            ControlParams.Params.PhotoCapture_State = ControlParams.e_DB.Create;
            ControlParams.Params.p_NewCustomer = true;
            App.Go(Mode.Photo_Customer_Search);
        }
 
        private void Me_Activated(object sender, EventArgs e)
        {
            page_init();
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
          
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.Home);
        }

    }
}
