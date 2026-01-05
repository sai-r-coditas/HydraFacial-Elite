using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

using System.ComponentModel; // 0102-01

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for uc_User.xaml
    /// </summary>
    public partial class uc_User : UserControl
    {
        public uc_User()
        {
            InitializeComponent();
            App.cs_Events_User.PropertyChanged += OnPropertyChanged;                            // 0102-01
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0102-01
        {
            if (propertyChangedEventArgs.PropertyName == "UserName")
            {
                lblLogin.Content = App.cs_Events_User.UserName;
            }
            else if (propertyChangedEventArgs.PropertyName == "UserManagement")                 // 0102-21
            {
                if (App.cs_Events_User.SingleUser)
                {
                    lblLogin.Visibility = Visibility.Hidden;
                    btnLogin.Visibility = Visibility.Hidden;
                }
                else
                {
                    lblLogin.Visibility = Visibility.Visible;
                    btnLogin.Visibility = Visibility.Visible;
                }
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            App.cs_Events_User.PropertyChanged -= OnPropertyChanged;                            // 0102-06
        }
    }
}
