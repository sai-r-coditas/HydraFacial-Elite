using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Edge.EdgeObject;
using JetBrains.Annotations;

//Add by sww
using System.Windows.Threading;

namespace Edge.IOBoard
{
    /// <summary>
    /// This handles the serial communications
    /// </summary>
    public class BoardManager : INotifyPropertyChanged
    {
        #region Serial Port Settings & Variables

        public int BaudRate = 9600; // 0102-36

        public const int DataBits = 8;
        
        public string PortName = "COM1";
               
        private static bool logCommunication;
        public static int DumpCount;
       
        private bool _WaitForGroupParams;  // 0020-11
        public bool WaitForGroupParams
        {
            get { return _WaitForGroupParams; }
            set { value = _WaitForGroupParams; }
        }

        private bool mIsOpen;
        private SerialPort mPort;

        public bool IsOpen
        {
            get { return mIsOpen; }
        }

        #endregion

        #region Delegates

        public delegate void IOBoardDataReceivedEventHandler(object sender, EventArgs e);

        public delegate void IOBoardDataSendEventHandler(object sender, EventArgs e);

        #endregion

        private readonly bool[] getNeeded = new bool[Constants.NUMBER_OF_STATIONS];

        private readonly bool[] getTagAgain = new bool[Constants.NUMBER_OF_STATIONS];  // 0020-05

        private bool doing;
        private bool getAllNeeded;

        private DateTime requestTime = DateTime.MaxValue;
        private bool setNeeded;

        private bool enableTimer = true; // 0102-21 Control for download

        //sww
        private string sw_ReceivedData;

        private DateTime timeOfLastInputIORequest = DateTime.MinValue;
         
        private readonly Timer _communicationTimer;
 
        private static int GetParameter(string name, int defaultValue)
        {
            var parameterString = ConfigurationManager.AppSettings[name];
            return parameterString == null ? defaultValue : int.Parse(parameterString);
        }

        //-Add by sww
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        
        private static DateTime dtWaitProcess;
        private static DateTime dtWait;

        public static int[] LedBatteryLevelCount = new int[Constants.NUMBER_OF_LED_STATIONS]; // 0101-08
        public static int[] LedBatteryLevel_buf = new int[Constants.NUMBER_OF_LED_STATIONS];  // 0101-08  // not in use
        public static int[,] LedBatteryData = new int[Constants.NUMBER_OF_LED_STATIONS,3]; // 010-08

        public void Start_Timer()
        {
            // 2014 12/01
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500); // new TimeSpan (0, 0, 0, 0, 500);
            dispatcherTimer.Start();
        }

        public void Stop_Timer()  // 0102-21
        {
            enableTimer = false;
            dispatcherTimer.Stop();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            AddLogMessage("Timer On");
            DoCommunication();
        }
        //=========================================================================================================

        public BoardManager()
        {

            LedBatteryLevelCount[0] = 0;  // 0101-08
            LedBatteryLevelCount[1] = 0;
            LedBatteryLevelCount[2] = 0;
            LedBatteryLevelCount[3] = 0;

            LedBatteryLevel_buf[0] = 0;
            LedBatteryLevel_buf[1] = 0;
            LedBatteryLevel_buf[2] = 0;
            LedBatteryLevel_buf[3] = 0;

            LedBatteryData[0, 0] = 0;
            LedBatteryData[1, 0] = 0;
            LedBatteryData[2, 0] = 0;
            LedBatteryData[3, 0] = 0;
            LedBatteryData[0, 1] = 0;
            LedBatteryData[1, 1] = 0;
            LedBatteryData[2, 1] = 0;
            LedBatteryData[3, 1] = 0;
            LedBatteryData[0, 2] = 0;
            LedBatteryData[1, 2] = 0;
            LedBatteryData[2, 2] = 0;
            LedBatteryData[3, 2] = 0;


            mStationStates = new Station[Constants.NUMBER_OF_STATIONS];
            mSendDataQueue = new Queue<string>();
           
            var logOutputsString = ConfigurationManager.AppSettings["LogCommunication"];
            logCommunication = logOutputsString != null && Boolean.Parse(logOutputsString);

            LedIsCharging = new bool[Constants.NUMBER_OF_LED_STATIONS];
            LedBatteryLevel = new int[Constants.NUMBER_OF_LED_STATIONS];

            WaitForGroupParams = false; // 0020-11

            // 2014 12/04
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            // 2014 12/30 for recapture RFID data
            for (int i = 0; i < Constants.NUMBER_OF_STATIONS; i++)
                getTagAgain[i] = true;
        }

        private readonly TimeSpan _communicationTimeout;

        private bool RequestInProgress
        {
            get { return requestTime != DateTime.MaxValue; }

            set { requestTime = value ? DateTime.Now : DateTime.MaxValue; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public event IOBoardDataSendEventHandler DataSent;
        public event IOBoardDataReceivedEventHandler DataReceived;

        // sww Modified
        public void DoCommunication()
        {
            dispatcherTimer.Stop();
 
            doing = true;

            try
            {

                //lock (this)
                //{
                    
                    //Write to Log file
                    if (RequestInProgress && (DateTime.Now - requestTime) > _communicationTimeout)
                    {
                        //sww  
                        AddLogMessage("No response from IO board to a request" );
                        RequestInProgress = false;
                    }
      
                    // setNeeded == if need send request IO Board state
                    if (setNeeded && (!IsOpen || logCommunication))                             // Diagnostics
                    {
                        var app = Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if (DumpCount++ % 10 == 0)
                                AddOutputLog(RequestSetState.DumpHeader());                     // 002639-07 0020-05

                            AddOutputLog(requestSetStateObject.Dump());                         // 002639-07 0020-05
                        }));
                    }

                    if (!IsOpen)
                    {
                        setNeeded = false;
                        return;
                    }

                    if (setNeeded)
                    {
                        setNeeded = false;
                        //~S
                        mPort_AddDataToQueue(requestSetStateObject.GetRequestSetAllOutputs());
                        SendData();
                        //return;
                        doWaitForProcess(5000);
                    }

                    if (DateTime.Now - timeOfLastInputIORequest > TimeSpan.FromMilliseconds(2000))
                    {
                        timeOfLastInputIORequest = DateTime.Now;

                        //2014 08/08 modified
                        mPort_AddDataToQueue("~I");                                             // Get all inputs
                        SendData();

                        doWaitForProcess(5000);
                        doWait(100);
                    }
                    
                    for (var i = 0; i < getNeeded.Length; i++)                                  // Check Tag data if requested
                    {
                        if (getNeeded[i])
                        {
                            getNeeded[i] = false;
                            mPort_AddDataToQueue(RequestGetStationTagData.Message(i));
                            SendData();
                            doWaitForProcess(5000);
                        }
                    }

                    if(enableTimer)
                        dispatcherTimer.Start();
              //}
            }
            finally
            {
                doing = false;
                doWait(500);   
            }
        }

        // 2014 08/08 Add 
        private void doWaitForProcess(double ms)
        {
            dtWaitProcess = DateTime.Now.AddMilliseconds(ms);
            while (RequestInProgress == true)
            {
                if (DateTime.Now > dtWaitProcess)
                    break;

                DoEvents();
            }
        }
 
        private static void doWait(double ms)
        {
            dtWait = DateTime.Now.AddMilliseconds(ms);
            while (DateTime.Now <= dtWait)
            {
                DoEvents();
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        //sww 
        private FileInfo f;
        public void AddLogMessage(string s)
        {
            if (!logCommunication)
                return;

            try {
                    // 2014 11/04
                    var filename = "Diagnostics.txt";

                    f = new FileInfo(filename);
                    long L = f.Length;
                    if (L > 900000)                                                             // if file size > 900k
                    {
                        var lines = System.IO.File.ReadAllLines(filename,Encoding.ASCII);       // 0106-09
                        int cnt = lines.Count();
                        
                        // Cut all lines in half 
                        File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);  // 0106-09
                    }

                    File.AppendAllText(filename,
                            string.Format("{0}-{1}{2}",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), s, Environment.NewLine),
                            Encoding.ASCII);                                                    // 0106-09
                }
                        
            catch (Exception ex)
            {
               
            }
         }

        /// <summary>
        /// Add to outputlog.txt file  002639-07
        /// </summary>
        /// <param name="s"></param>
        private void AddOutputLog(string s)
        {
            if (!logCommunication)
                return;

            CutFileSize("OutputLog.txt");

            File.AppendAllText("OutputLog.txt", s,Encoding.ASCII);  // 0106-09
        }

        private void CutFileSize(string filename)
        {
            try
            {
                FileInfo f = new FileInfo(filename);
                long L = f.Length;
                if (L > 900000)  // if file size > 900k
                {
                    var lines = System.IO.File.ReadAllLines(filename,Encoding.ASCII);  // 0106-09
                    int cnt = lines.Count();

                    // Cut all lines in half 
                    File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);  // 0106-09
                }
            }
            catch (Exception ex)
            {

            }
        }
       
        private void DebugMessage(bool b,string msg)
        {
            if (!logCommunication)
                return;
           
            Debug.Assert(b, msg);
        }

        public bool Open()
        {
            mPort = new SerialPort
                    {
                        PortName = PortName,
                        BaudRate = BaudRate,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One
                    };

            // add by sww
            mPort.ReadBufferSize = 500;
            mPort.ReadTimeout = 500;
            mPort.Handshake = System.IO.Ports.Handshake.None;

            mPort.DataReceived += mPort_DataReceived;
            mPort.Open();
            mIsOpen = true;

            return mIsOpen;
        }

        public void Close()
        {
            if ( _communicationTimer !=null)
                _communicationTimer.Dispose();

            if (dispatcherTimer != null)
                dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick); // 0020-08

            if (mPort != null)
            {
                mPort.Close();
            }
        }

        public void setMonitorBottleChanges()
        {
            lock (mStationStates)
            {
                for (var i = 0; i < mStationStates.Length; i++)
                {
                    if (mStationStates[i] != null && mStationStates[i].RequireFullBottleData)
                    {
                        switch ((StationNumber) i)
                        {
                            case StationNumber.Station1:
                                getNeeded[0] = true;
                                break;
                            case StationNumber.Station2:
                                getNeeded[1] = true;
                                break;
                            case StationNumber.Station3:
                                getNeeded[2] = true;
                                break;
                            case StationNumber.Station4:
                                getNeeded[3] = true;
                                break;
                            case StationNumber.Station5:
                                getNeeded[4] = true;
                                break;
                        }
                    }
                }
            }
        }

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #region Sends Data to Serial Port

        /// <summary>
        /// Adds data to queue
        /// </summary>
        /// <param name="data"></param>
        private void mPort_AddDataToQueue(string data)
        {
            //Throttle the data send
            lock (mSendDataQueue)
            {
                mSendDataQueue.Enqueue(data);
            }
        }

        /// <summary>
        /// Sends data to the serial port
        /// </summary>
        private void SendData()
        {
            lock (mSendDataQueue)
            {
                if (logCommunication)
                   Debug.Assert(mSendDataQueue.Count == 1);

                //2014 8/8 Add checksum
                string s = mSendDataQueue.Dequeue();
                byte[] fs = System.Text.Encoding.ASCII.GetBytes(s);
                var checksum = custom_CRC16.c_crc16(fs);

                var data = s + checksum + Convert.ToChar(13); // Modified by sww 2014 07/14, char10 will add when use writeline function

                int i = data.Length;
 
                mPort.WriteLine(data);  // sww changed

                if (DataSent != null)
                    DataSent("DataSent; " + data, EventArgs.Empty);

                AddLogMessage("<Sent->" + data); // sww_IO changed
 
                RequestInProgress = true;
            }
        }

        /// <summary>
        /// Sends the latest board IO state request 
        /// To modify the state, use the requestSetStateObject, update with changes and then ask to SetBoardIOState
        /// </summary>
        public void SendRequestSetBoardIOState()
        {
            if (!WaitForGroupParams)  // 0020-11
                setNeeded = true;
        }

        #endregion

        #region Receive and Process dAta From Serial

        public int VacuumAtoD { get; private set; }
        public int TemperatureAtoD { get; private set; }
        public bool PowerPressed { get; private set; }
        public bool PowerLatched { get; private set; }

        public int[] LedBatteryLevel { get; set; }
        public bool[] LedIsCharging { get; set; }


        public void datarec()  // for testing only
        {
            var dataIn = "@i0000400000011001001001000BE00401501A4735D2E00401501A4721930000000000000000E00401501A47324E00000000000000007FA6";
 
            var res = Response.ParseResponse(dataIn);
            AddLogMessage("<Received>" + dataIn);


            if (res.ParseStatus == Response.ParseStatusType.Valid)
            {

                switch (res.ResponseType)
                {
                    case Response.ResponseSetAllOutputs: //@S
                        ////If time out show the message
                        //DebugMessage(RequestInProgress, "Received set response time out." );
                        break;

                    case Response.ResponseGetAllInputs://@I
                        ////If time out show the message
                        //DebugMessage(RequestInProgress, "Received get response time out."); 

                        var response = (ResponseGetAllInputs)res;
                        UpdateInputs(response);

                        break;

                    case Response.ResponseGetTagData: //@D
                        if (logCommunication && !RequestInProgress)
                        {
                            //sww
                            AddLogMessage("Received response to unrequested get tag data.");
                        }

                        //System.Media.SystemSounds.Beep.Play();
                        UpdateOneStationTagData((ResponseGetTagData)res);

                        if (logCommunication)
                        {
                            var r = (ResponseGetTagData)res;
                            //sww
                            AddLogMessage("UpdateOneStationTagData for " + r.StationId + ": " + r.RfidData);
                        }
                        break;

                    default:
                        //// Otherwise show the message  
                        DebugMessage(false, string.Format("Received response to unrequested message: '{0}'", res.ResponseType));

                        break;
                }

                DataReceived("DataReceived; " + dataIn, EventArgs.Empty);
            }
            else
            {
                // Invalid Data
                //sww
                AddLogMessage("Invalid Data->" + dataIn + "<-");
            }
            //}
            doWait(500);  //500
            RequestInProgress = false;
        }


        // 2014 12/04
        SerialPort sp;
        string dataIn;
        private void mPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //lock (this)
            //{

                sp = (SerialPort)sender;
                dataIn = sp.ReadExisting();

                int len = dataIn.Length;
              
                dataIn = sw_ReceivedData + dataIn;

                int pos = dataIn.IndexOf("\r" + "\n");
                
                if (pos != -1)  
                {

                    string ValidString;
                    
                    ValidString = dataIn.Substring(0, pos);
                    
                    // When String Found
                    sw_ReceivedData = dataIn.Substring(pos + 2); ;
                }
                else
                {
                    sw_ReceivedData = dataIn;
                    // Wait for next loop
                    return;
                }

                var res = Response.ParseResponse(dataIn);
                AddLogMessage("<Received>" + dataIn);
               

                if (res.ParseStatus == Response.ParseStatusType.Valid)
                {
                    switch (res.ResponseType)
                    {
                        case Response.ResponseSetAllOutputs: //@S
                            
                            ////If time out show the message
                            break;

                        case Response.ResponseGetAllInputs://@I
                            
                            ////If time out show the message
                            var response = (ResponseGetAllInputs)res;
                            UpdateInputs(response);

                            if (logCommunication)
                            {
                                var builder = new StringBuilder();
                                
                                // 2014 10/22
                                builder.AppendLine();

                                for (var i = 0; i < response.DeviceStations.Length; i++)
                                {
                                    var station = response.DeviceStations[i];

                                    builder.AppendLine(string.Format("{0}-Station-- {7}: {1},{2},{3},{4},{5},{6}",
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        mStationStates[i].Uid,
                                        mStationStates[i].PartNumber,
                                        mStationStates[i].PartNumberRevisionLevel, mStationStates[i].SerialNumber, mStationStates[i].FillCode,
                                        mStationStates[i].BatchFillNumber, i));
                                }

                                AddLogMessage(builder.ToString()); //sww
                            }
                           
                            break;

                        case Response.ResponseGetTagData: //@D
                            if (logCommunication && !RequestInProgress)
                            {
                                AddLogMessage("Received response to unrequested get tag data."); //sww
                            }

                            //System.Media.SystemSounds.Beep.Play();
                            UpdateOneStationTagData((ResponseGetTagData)res);
                            
                            if (logCommunication)
                            {
                                var r = (ResponseGetTagData)res;

                                AddLogMessage("UpdateOneStationTagData for " + r.StationId + ": " + r.RfidData); //sww
                            }
                            break;

                        default:
                            
                            //// Otherwise show the message  
                            DebugMessage (false,  string.Format("Received response to unrequested message: '{0}'" ,res.ResponseType));
                           
                            break;
                    }

                    DataReceived("DataReceived; " + dataIn, EventArgs.Empty);
                }
                else
                {
                    AddLogMessage("Invalid Data->" + dataIn + "<-");
                }
                //}
                doWait(500);  //500
                RequestInProgress = false;
       
           //}
        }

        private void UpdateInputs(ResponseGetAllInputs r)
        {
            getInputAllStations(r);
            
            // Set flag to send request
            setMonitorBottleChanges();

            if (r.PowerPressed != PowerPressed)
            {
                PowerPressed = r.PowerPressed;
                RaisePropertyChanged("PowerPressed");
            }

            if (r.PowerLatched != PowerLatched)
            {
                PowerLatched = r.PowerLatched;
                RaisePropertyChanged("PowerLatched");
            }

            if (requestSetStateObject.HotEnabled && r.HotTemperatureAtoD != TemperatureAtoD)
            {
                TemperatureAtoD = r.HotTemperatureAtoD;
                RaisePropertyChanged("TemperatureAtoD");
            }
            else if (!requestSetStateObject.HotEnabled && r.ColdTemperatureAtoD != TemperatureAtoD)
            {
                TemperatureAtoD = r.ColdTemperatureAtoD;
                RaisePropertyChanged("TemperatureAtoD");
            }
        
            if (r.VacuumAtoD != VacuumAtoD)
            {
                VacuumAtoD = r.VacuumAtoD;
                RaisePropertyChanged("VacuumAtoD");
            }

            //if (!r.LedIsCharging.SequenceEqual(LedIsCharging))
            //{
            //    LedIsCharging = (bool[])r.LedIsCharging.Clone();  // 0101-08
            //    //RaisePropertyChanged("LedIsCharging");
            //}

            if (!r.LedBatteryLevel.SequenceEqual(LedBatteryLevel))
            {
                LedBatteryLevel = (int[])r.LedBatteryLevel.Clone();
                //RaisePropertyChanged("LedBatteryLevel");
            }
        }
       
        /// <summary>
        /// For each station, if require full bottle data, then send command to request it
        /// </summary>
        /// <param name="r"></param>
        private void getInputAllStations(ResponseGetAllInputs r)
        {
            lock (mStationStates)
            {
                for (var i = 0; i < mStationStates.Length; i++)
                {
                    //if (r.DeviceStations[i].Uid == Station.NoBottleUid)
                    //    continue;

                    if (mStationStates[i] == null)
                    {
                        mStationStates[i] = new Station();
                    }

                    mStationStates[i].UpdateStation(r.DeviceStations[i]);
                }
            }
        }

        /// <summary>
        /// This processes the response of one station's tag data
        /// </summary>
        /// <param name="r"></param>
        private void UpdateOneStationTagData(ResponseGetTagData r)
        {
            //lock (mStationStates)
            //{
            //    // 2014 11/25
            //    // mStationStates[r.StationId - 1].UpdateOneStationFullData(r.RfidData);
            //    mStationStates[r.StationId - 1].UpdateOneStationFullData(mStationStates[r.StationId - 1].Uid, r.RfidData);
            //}

            mStationStates[r.StationId - 1].UpdateOneStationFullData(mStationStates[r.StationId - 1].Uid, r.RfidData);  // 0020-05

            if ((int)r.RfidData[0] == 0 && getTagAgain[r.StationId - 1] == true)
            {
                getTagAgain[r.StationId - 1] = false;  // Only allow recapture once
                getNeeded[r.StationId - 1] = true;
            }
            else if ((int)r.RfidData[0] != 0)
            {
                getTagAgain[r.StationId - 1] = true;
            }
        }

        #endregion

        #region Board Status

        private readonly Queue<string> mSendDataQueue;

        public Station[] mStationStates;

        [NotNull]
        public RequestSetState requestSetStateObject = new RequestSetState();

        #endregion
    }
}