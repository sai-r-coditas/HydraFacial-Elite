using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Edge.Tower2.UI
{
    /// <summary>
    ///     Interaction logic for HotColdNavButton.xaml
    /// </summary>
    public partial class HotColdNavButton
    {
        private readonly Dictionary<int, Brush> _colorForTemperature = new Dictionary<int, Brush>();

        public HotColdNavButton()
        {
            InitializeComponent();

            _colorForTemperature.Add(0, Brushes.Gray);
            _colorForTemperature.Add(Settings.HotColdTemperatureCoolest, Brushes.White);
            _colorForTemperature.Add(Settings.HotColdTemperatureCooler, Brushes.LightBlue);
            _colorForTemperature.Add(Settings.HotColdTemperatureCool, Brushes.MediumBlue);
            _colorForTemperature.Add(Settings.HotColdTemperatureWarm, Brushes.Yellow);
            _colorForTemperature.Add(Settings.HotColdTemperatureWarmer, Brushes.Orange);
            _colorForTemperature.Add(Settings.HotColdTemperatureWarmest, Brushes.Red);

            App.BoardManager.PropertyChanged += BoardManagerPropertyChanged;

            App.Outputs.PropertyChanged += OutputsOnPropertyChanged;

            UpdateButtonState();
        }

        public App App
        {
            get { return (App) Application.Current; }
        }

        private int TargetTemperature
        {
            get { return App.Outputs.TargetTemperature; }
        }
       

        private void OutputsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "TargetTemperature":
                    Dispatcher.Invoke((Action) (UpdateButtonState));
                    break;

                //case "PulseDuration":
                //    Dispatcher.Invoke((Action) (UpdateButtonState));
                //    break;
            }
        }

        private void BoardManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action) (() =>
                                        {
                                            switch (e.PropertyName)
                                            {
                                                case "TemperatureAtoD":
                                                    UpdateButtonState();
                                                    break;
                                                
                                                case "PowerPressed":
                                                    break;
                                            }
                                        }));
        }

        private void UpdateButtonState()
        {
            ButtonTemperatureText.Foreground = _colorForTemperature[TargetTemperature];
            ButtonTemperatureText.Text = TargetTemperature == 0 ? "Off " : TargetTemperature + "°";
            ButtonTemperatureTextActual.Text = App.BoardManager.TemperatureAtoD + "°";
        }

        private void HotColdClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)                     // 0102-07
        {
            App.BoardManager.PropertyChanged -= BoardManagerPropertyChanged;

            App.Outputs.PropertyChanged -= OutputsOnPropertyChanged;
        }
    }
}