using System;
using System.ComponentModel;
using System.Windows;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for pg_PulseSelect.xaml
    /// Design for Facial Therapy and Body Therapy
    /// </summary>
    public partial class pg_PulseSelect 
    {
        public pg_PulseSelect()
        {
            InitializeComponent();
        }

        public App App
        {
            get { return (App)System.Windows.Application.Current; }
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
    }
}
