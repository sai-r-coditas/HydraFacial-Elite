using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Edge.Tower2.UI
{
    [ValueConversion(typeof (int), typeof (bool))]
    public class NumberIsChecked : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return false;

            int parameterValue = parameter is int ? (int) parameter : int.Parse((string) parameter);
            return (int) value == parameterValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is int ? (int) parameter : int.Parse((string) parameter);
        }

        #endregion
    }

    [ValueConversion(typeof(int), typeof(bool))]
    public class PulseOptionIsChecked : IValueConverter
    {
        private int LowDuration
        {
            get
            {
                switch (ControlParams.Params.p_control_mode)
                {
                    case ControlParams.e_Mode.LymphaticBody: // "LymphaticBody":
                        return Settings.BodyLymphPulseLowDuration;

                    case ControlParams.e_Mode.LymphaticFacial: // "LymphaticFacial":
                        return Settings.FaceLymphPulseLowDuration;

                    case ControlParams.e_Mode.PulseFusion: // "PulseFusion":
                        return Settings.VacJetPulseLowDuration;

                    case ControlParams.e_Mode.ThermalTherapy: // "ThermalTherapy":
                        return Settings.HotColdPulseLowDuration;

                    default:
                        //throw new NotSupportedException();
                        return 0;
                }
            }
        }

        private int MediumDuration
        {
            get
            {
                switch (ControlParams.Params.p_control_mode)
                {
                    case ControlParams.e_Mode.LymphaticBody:
                        return Settings.BodyLymphPulseMediumDuration;

                    case ControlParams.e_Mode.LymphaticFacial:
                        return Settings.FaceLymphPulseMediumDuration;

                    case ControlParams.e_Mode.PulseFusion:
                        return Settings.VacJetPulseMediumDuration;

                    case ControlParams.e_Mode.ThermalTherapy:
                        return Settings.HotColdPulseMediumDuration;

                    default:
                        //throw new NotSupportedException();
                        return 0;
                }
            }
        }

        private int HighDuration
        {
            get
            {
                switch (ControlParams.Params.p_control_mode)
                {
                    case ControlParams.e_Mode.LymphaticBody:
                        return Settings.BodyLymphPulseHighDuration;

                    case ControlParams.e_Mode.LymphaticFacial:
                        return Settings.FaceLymphPulseHighDuration;

                    case ControlParams.e_Mode.PulseFusion:
                        return Settings.VacJetPulseHighDuration;

                    case ControlParams.e_Mode.ThermalTherapy:
                        return Settings.HotColdPulseHighDuration;

                    default:
                        //throw new NotSupportedException();
                        return 0;
                }
            }
        }

        private int Target(object parameter)
        {
            int target = 0;

            switch ((string)parameter)
            {
                case "Off":
                    target = 0;
                    break;

                case "Low":
                    target = LowDuration;
                    break;

                case "Medium":
                    target = MediumDuration;
                    break;

                case "High":
                    target = HighDuration;
                    break;
            }
            return target;
        }

        private App App
        {
            get { return (App)Application.Current; }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = Target(parameter);

            if (!(value is int))
                return false;

            return (int) value == target;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)parameter)
            {
                case "Off":
                    return 0;

                case "Low":
                    return Settings.HotColdPulseLowDuration;

                case "Medium":
                    return Settings.HotColdPulseMediumDuration;

                case "High":
                    return Settings.HotColdPulseHighDuration;
            }

            return null;
        }

        #endregion
    }
}