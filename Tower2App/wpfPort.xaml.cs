using System;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;
using System.Text;

using JetBrains.Annotations;
using System.Configuration;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfPort.xaml
    /// </summary>
    public partial class wpfPort : Window
    {
        public wpfPort()
        {
            InitializeComponent();

            serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);
        }

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        #region Init
        SerialPort serial = new SerialPort();
        string recieved_data;

        public void Connect_Open()
        {
            if (serial.IsOpen) return;
            try
            {
                //Sets up serial port
                serial.PortName = ConfigurationManager.AppSettings["COMPORT"];   // 0103-05
                serial.BaudRate = Convert.ToInt32(ConfigurationManager.AppSettings["BAUDRATE"]); // 0103-05
                serial.Handshake = System.IO.Ports.Handshake.None;
                serial.Parity = Parity.None;
                serial.DataBits = 8;
                serial.StopBits = StopBits.Two;
                serial.ReadTimeout = 200;
                serial.WriteTimeout = 50;
                serial.Open();
            }
            catch ( Exception ex)
            {
                MessageBox.Show("Port com1 is not available!");
            }
        }
        #endregion

        #region Recieving
        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            recieved_data = serial.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(ReadData), recieved_data);
        }
       
        private void ReadData(string data)  
        {
            if (data == "S" + (char) 13)
                App.Go(Mode.wpfShutdown);
        }

        private void ShutDownProcess()
        {
            
        }
        #endregion

        #region Sending
        public void SerialCmdSend(string data)
        {
            App.BoardManager.AddLogMessage(data);                                               // 0106-16

            if (serial.IsOpen)
            {
                try
                {
                    // Send the binary data out the port
                    byte[] hexstring = Encoding.ASCII.GetBytes(data);
                   
                    foreach (byte hexval in hexstring)
                    {
                        byte[] _hexval = new byte[] { hexval }; // need to convert byte to byte[] to write
                        serial.Write(_hexval, 0, 1);
                        Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Send data failed!");
                }
            }
            else
            {
            }
        }
        #endregion

        #region Form Controls
         private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serial.IsOpen) serial.Close();
            this.Close();
        }
        #endregion
    }
}
