using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using Edge.IOBoard;
using JetBrains.Annotations;

using System.Linq;
using System.Text;

namespace Edge.Tower2.UI
{
    public class Outputs : INotifyPropertyChanged
    {
        [NotNull] private readonly BoardManager _bm;

        private static bool _logOutputs;

        public Outputs([NotNull] BoardManager bm)
        {
            var logOutputsString = ConfigurationManager.AppSettings["LogCommunication"];
            _logOutputs = logOutputsString != null && bool.Parse(logOutputsString);
            _bm = bm;
        }

        [NotNull]
        private App App
        {
            get { return (App)Application.Current; }
        }

        #region Set_Hardware_Flag
        public bool Solenoid1
        {
            get { return _bm.requestSetStateObject.Solenoid1; }
            set
            {
                _bm.requestSetStateObject.Solenoid1 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("Solenoid1");
            }
        }

        public bool Solenoid2
        {
            get { return _bm.requestSetStateObject.Solenoid2; }
            set
            {
                _bm.requestSetStateObject.Solenoid2 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("Solenoid2");
            }
        }

        public bool Solenoid3
        {
            get { return _bm.requestSetStateObject.Solenoid3; }
            set
            {
                _bm.requestSetStateObject.Solenoid3 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("Solenoid3");
            }
        }

        public bool Solenoid4
        {
            get { return _bm.requestSetStateObject.Solenoid4; }
            set
            {
                _bm.requestSetStateObject.Solenoid4 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("Solenoid4");
            }
        }

        public bool Solenoid5
        {
            get { return _bm.requestSetStateObject.Solenoid5; }
            set
            {
                _bm.requestSetStateObject.Solenoid5 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("Solenoid5");
            }
        }

        public bool BlueRfidLeds
        {
            get { return _bm.requestSetStateObject.BlueRfidLeds; }
            set
            {
                _bm.requestSetStateObject.BlueRfidLeds = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("BlueRfidLeds");
            }
        }
        #endregion

        private void UpdateImplicitlySetOutputs()
        {
            _bm.requestSetStateObject.VacJetEnabled = PulseDuration != 0 && VacuumPump && !ExMass1;
            _bm.requestSetStateObject.VacJetHotColdMode = PulseDuration != 0 && VacuumPump && ExMass1;
        }

        #region Property declare ,ExMass1, ExMass2, ExMass3,...
        public bool VacuumPump
        {
            get { return _bm.requestSetStateObject.VacuumPump; }
            set
            {
                _bm.requestSetStateObject.VacuumPump = value;

                _bm.requestSetStateObject.Exmass3 = value && _exmass3;

                if (!value)
                    PulseDuration = 0;

                UpdateImplicitlySetOutputs();

                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("VacuumPump");
            }
        }

        public int VacuumPressure
        {
            get { return _bm.requestSetStateObject.VacuumPumpPercentage; }
            set
            {
                _bm.requestSetStateObject.VacuumPumpPercentage = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("VacuumPressure");
            }
        }

        public int TargetTemperature
        {
            get
            {
                if (!_bm.requestSetStateObject.HotColdEnabled)
                    return 0;

                if (_bm.requestSetStateObject.HotEnabled)
                    return _bm.requestSetStateObject.HotTempSetpoint;

                return _bm.requestSetStateObject.ColdTempSetpoint;
            }
            set
            {
                if (value == 0)
                {
                    _bm.requestSetStateObject.HotColdEnabled = false;
                    _bm.requestSetStateObject.HotEnabled = false;
                }
                else
                {
                    _bm.requestSetStateObject.HotColdEnabled = true;
                    _bm.requestSetStateObject.HotEnabled = (value > 90);

                    if (_bm.requestSetStateObject.HotEnabled)
                    {
                        _bm.requestSetStateObject.ColdTempSetpoint = 0;
                        _bm.requestSetStateObject.HotTempSetpoint = value;
                    }
                    else
                    {
                        _bm.requestSetStateObject.ColdTempSetpoint = value;
                        _bm.requestSetStateObject.HotTempSetpoint = 0;
                    }
                }

                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("TargetTemperature");
            }
        }
       
        public bool ExMass1
        {
            get { return _bm.requestSetStateObject.Exmass1; }
            set
            {
                _bm.requestSetStateObject.Exmass1 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("ExMass1");
            }
        }
       
        private bool _exmass2;
        public bool ExMass2
        {
            get { return _bm.requestSetStateObject.Exmass2; }
            set
            {
                _exmass2 = value;
                _bm.requestSetStateObject.Exmass2 = value && VacuumPump;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("ExMass2");
            }
        }

        private bool _exmass3;
        public bool ExMass3
        {
            get { return _bm.requestSetStateObject.Exmass3; }
            set
            {
                _exmass3 = value;
                _bm.requestSetStateObject.Exmass3 = value && VacuumPump;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("ExMass3");
            }
        }
      
        private bool _exmass4;
        public bool ExMass4
        {
            get { return _bm.requestSetStateObject.Exmass4; }
            set
            {
                _exmass4 = value;
                _bm.requestSetStateObject.Exmass4 = value && VacuumPump;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("ExMass4");
            }
        }

        public bool DC_Motor // Add by sww 2014 07/14 for JP24, JP25
        {
            get { return _bm.requestSetStateObject.DC_Motor; }
            set
            {
                _bm.requestSetStateObject.DC_Motor = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("DC_Motor");
            }
        }

        public bool BlueLEDs
        {
            get { return _bm.requestSetStateObject.BlueLed1; }  //TBD: 2nd Bit redundant
            set
            {
                _bm.requestSetStateObject.BlueLed1 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("BlueLEDs");
            }
        }

        public bool RedLEDs
        {
            get { return _bm.requestSetStateObject.BlueLed1; }  //TBD: 2nd Bit redundant
            set
            {
                _bm.requestSetStateObject.RedLed1 = value;
                _bm.requestSetStateObject.RedLed2 = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("RedLEDs");
            }
        }
        #endregion

        public bool PrinterPower
        {
            get { return _bm.requestSetStateObject.PrinterPower; }  
            set
            {
                _bm.requestSetStateObject.PrinterPower = value;
                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("PrinterPower");
            }
        }

        public int PulseDuration
        {
            get
            {
                return _bm.requestSetStateObject.VacJetPeriod;
            }
            set
            {
                if (value != 0)  // 2014 07/21 Modified for new IO board ====
                {
                    VacuumPump = true;

                    if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.PulseFusion)
                    {
                       _bm.requestSetStateObject.DC_Motor = true;

                       _bm.requestSetStateObject.Exmass4 = true;   // 2014 08/21 firmware changed
                    }
                }
                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.ThermalTherapy)
                {
                    if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.PulseFusion)
                    {
                        _bm.requestSetStateObject.VacuumPump = false;
                        _bm.requestSetStateObject.DC_Motor = false;
                    }
                    _bm.requestSetStateObject.Exmass3 = false;
                }
                
                // ===========================================

                // *******************************************

                var hotColdDurations = new[] { Settings.HotColdPulseLowDuration, Settings.HotColdPulseMediumDuration, Settings.HotColdPulseHighDuration };
                var vacJetDurations = new[] { Settings.VacJetPulseLowDuration, Settings.VacJetPulseMediumDuration, Settings.VacJetPulseHighDuration };
                var bodyLymphDurations = new[] { Settings.BodyLymphPulseLowDuration, Settings.BodyLymphPulseMediumDuration, Settings.BodyLymphPulseHighDuration };
                var faceLymphDurations = new[] { Settings.FaceLymphPulseLowDuration, Settings.FaceLymphPulseMediumDuration, Settings.FaceLymphPulseHighDuration };

                var hotColdPercentages = new[] { Settings.HotColdPulseLowPercentage, Settings.HotColdPulseMediumPercentage, Settings.HotColdPulseHighPercentage };
                var vacJetPercentages = new[] { Settings.VacJetPulseLowPercentage, Settings.VacJetPulseMediumPercentage, Settings.VacJetPulseHighPercentage };
                var bodyLymphPercentages = new[] { Settings.BodyLymphPulseLowPercentage, Settings.BodyLymphPulseMediumPercentage, Settings.BodyLymphPulseHighPercentage };
                var faceLymphPercentages = new[] { Settings.FaceLymphPulseLowPercentage, Settings.FaceLymphPulseMediumPercentage, Settings.FaceLymphPulseHighPercentage };

                var hotColdVacuums = new[] { Settings.HotColdLowVacuum, Settings.HotColdMediumVacuum, Settings.HotColdHighVacuum };
                var vacJetVacuums = new[] { Settings.VacJetLowVacuum, Settings.VacJetMediumVacuum, Settings.VacJetHighVacuum };
                var bodyLymphVacuums = new[] { Settings.BodyLymphLowVacuum, Settings.BodyLymphMediumVacuum, Settings.BodyLymphHighVacuum };
                var faceLymphVacuums = new[] { Settings.FaceLymphLowVacuum, Settings.FaceLymphMediumVacuum, Settings.FaceLymphHighVacuum };

                var durations = new[] { hotColdDurations, vacJetDurations, bodyLymphDurations, faceLymphDurations };
                var percentages = new[] { hotColdPercentages, vacJetPercentages, bodyLymphPercentages, faceLymphPercentages };
                var vacuums = new[] { hotColdVacuums, vacJetVacuums, bodyLymphVacuums, faceLymphVacuums };

                // sww changed
                //var indexOfMode = new[] { -1, 0, 1, 2, 3, -1, -1, -1, -1, -1, -1, -1 };
                // 0:Home,        1:HotCold,       2: VacJet,       3: BodyLymph,        FaceLymph,
                var indexOfMode = new[] { -1, 0, 1, 2, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
                
                //var modeIndex = indexOfMode[(int)App.Mode];
                
                // 2014 08/19 sww changed
                var modeIndex = 0;
 
                if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.ThermalTherapy)  //0
                    modeIndex = 0;//indexOfMode[1];
                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.PulseFusion)
                    modeIndex = 1;//indexOfMode[2];
                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LymphaticBody)
                    modeIndex = 2;//indexOfMode[3];
                else if (ControlParams.Params.p_control_mode == ControlParams.e_Mode.LymphaticFacial)
                    modeIndex = 3; //indexOfMode[4];
                else
                {
                    modeIndex = 0;  // 0102-39  modeIndex = -1
                }

                var percentage = 0;
                var duration = 0;

                var vacuum = modeIndex == -1 ? 0 : vacuums[modeIndex][0];

                // The incoming value from the selector buttons is always for the HotCold, but it gets mapped to the right data here based on the current mode
                if (value == Settings.HotColdPulseHighDuration)
                {
                    percentage = percentages[modeIndex][2];
                    duration = durations[modeIndex][2];
                    vacuum = vacuums[modeIndex][2];
                }
                else if (value == Settings.HotColdPulseMediumDuration)
                {
                    percentage = percentages[modeIndex][1];
                    duration = durations[modeIndex][1];
                    vacuum = vacuums[modeIndex][1];
                }
                else if (value == Settings.HotColdPulseLowDuration)
                {
                    percentage = percentages[modeIndex][0];
                    duration = durations[modeIndex][0];
                    vacuum = vacuums[modeIndex][0];
                }

                // *******************************************
                
                _bm.requestSetStateObject.VacJetPeriod = duration;
                PulseDutyCycle = percentage;

                //if (_lastMode != App.Mode || value != 0 && value != _lastPulseDuration)
                //{
                    VacuumPressure = vacuum;
                //}

                _lastMode = App.Mode;

                if (value != 0)
                {
                    _lastPulseDuration = value;
                }

                UpdateImplicitlySetOutputs();

                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("PulseDuration");
                OnPropertyChanged("VacuumPump");
            }
        }

        // For PulseDuration only
        private int _lastPulseDuration;
        private Mode _lastMode;

        private int _pulseDutyCycle;
        public int PulseDutyCycle
        {
            get
            {
                return _pulseDutyCycle;
            }
            set
            {
                _pulseDutyCycle = value;
                _bm.requestSetStateObject.VacJetDutyCycle = (int)(PulseDuration*value/100.0);

                _bm.SendRequestSetBoardIOState();
                OnPropertyChanged("PulseDutyCycle");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void ClearAll()
        {
            _bm.requestSetStateObject.ResetAll();
            _bm.SendRequestSetBoardIOState();

            OnPropertyChanged("TargetTemperature");
            OnPropertyChanged("PulseDuration");
            OnPropertyChanged("VacuumPressure");
            OnPropertyChanged("VacuumPump");

            OnPropertyChanged("DC_Motor"); // 2014 07/14 Add by sww

            OnPropertyChanged("Solenoid1");
            OnPropertyChanged("Solenoid2");
            OnPropertyChanged("Solenoid3");
            OnPropertyChanged("Solenoid4");
            OnPropertyChanged("Solenoid5");
            OnPropertyChanged("ExMass1");
            OnPropertyChanged("ExMass3");
            OnPropertyChanged("RedLEDs");
            OnPropertyChanged("BlueLEDs");

        }
 
        public static void LogHeader(string screenName, string enterExitString)  // 0106-09
        {
            if (!((App) Application.Current).BoardManager.IsOpen || _logOutputs)
            {
                CutFileSize("OutputLog.txt"); // 002639-07 0020-05

                if (enterExitString == "Enter")
                {
                    File.AppendAllText("OutputLog.txt",
                        string.Format("-------------------------{0}{1}: {2}{0}{0}", Environment.NewLine, enterExitString, screenName),
                        Encoding.ASCII);
                }
                else if (enterExitString == "Exit")
                {
                    File.AppendAllText("OutputLog.txt",
                        string.Format("{0}{1}: {2}{0}-------------------------{0}", Environment.NewLine, enterExitString, screenName),
                        Encoding.ASCII);
                }
                else
                {
                    File.AppendAllText("OutputLog.txt",
                        string.Format("-------------------------{0}-------------------------{0}-------------------------{0}-------------------------{0}", Environment.NewLine),
                        Encoding.ASCII);
                }
            }

            BoardManager.DumpCount = 0;
        }

        private static void CutFileSize(string filename)  // 002639-07
        {
            try
            {
                FileInfo f = new FileInfo(filename);
                long L = f.Length;
                if (L > 900000)  // if file size > 900k
                {
                    var lines = System.IO.File.ReadAllLines(filename, Encoding.ASCII);  // 0106-09
                    int cnt = lines.Count();

                    // Cut all lines in half 
                    File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);  // 0106-09
                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool PropertyIsChanging;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            PropertyIsChanging = true;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));

            PropertyIsChanging = false;

            //if (!_bm.IsOpen || LogOutputs)  // Diagnostics
            //{
            //    if (DumpCount++ % 10 == 0)
            //        File.AppendAllText("OutputLog.txt", RequestSetState.DumpHeader());

            //    File.AppendAllText("OutputLog.txt", _bm.requestSetStateObject.Dump());
            //}

        }
    }
}