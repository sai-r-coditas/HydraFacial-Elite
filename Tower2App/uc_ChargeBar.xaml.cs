using System.Windows;
using System.Windows.Controls;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for ChargeBar.xaml
    /// </summary>
    public partial class ChargeBar : UserControl
    {
        public ChargeBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof (int), typeof (ChargeBar), new PropertyMetadata(default(int)));

        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty IsChargingProperty = DependencyProperty.Register(
            "IsCharging", typeof(bool), typeof(ChargeBar), new PropertyMetadata(default(bool)));

        public bool IsCharging
        {
            get { return (bool)GetValue(IsChargingProperty); }
            set { SetValue(ValueProperty, IsChargingProperty); }
        }

    }
}
