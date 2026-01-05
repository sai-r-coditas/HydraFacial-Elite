using System;
using System.Text;

namespace Edge.IOBoard
{
    static class BoolExtension
    {
        public static string X(this bool b)
        {
            return b ? "X" : " ";
        }
    }

    public class RequestSetState
    {
        //public const string REQUEST_POWER_OFF = "~O"; 
        //public const string RESPONSE_POWER_OFF = "@O";


        private const string RequestSetAllNoAck = "~S";
        private const string RequestSetIOManual = "~M";

        #region Variables For Board State

        public bool BlueLed1 = true;   // 2014 07/22 Change from false to true by sww
        public bool PrinterPower = true;
        public bool BlueRfidLeds = false;
        public bool Exmass1 = false;
        public bool Exmass2 = false;
        public bool Exmass3 = false; //Requires M Command
        public bool Exmass4 = false;
        public bool HotColdEnabled = false;
        public bool HotEnabled = false;
        public bool RedLed1 = false;
        public bool RedLed2 = false;
        public bool Sbc12V = true; //Requires M command
        public bool Sbc_5V = false; //Requires M command

        public bool DC_Motor = false; //2014 07/14 add for JP24,25

        public bool Solenoid1 = false;
        public bool Solenoid2 = false;
        public bool Solenoid3 = false;
        public bool Solenoid4 = false;
        public bool Solenoid5 = false;
        public bool VacJetEnabled = false;
        public bool VacJetHotColdMode = false;
        public bool VacuumPump = false;

        public int ColdTempSetpoint = 0;
        public int HotTempSetpoint = 0;
        public int VacJetDutyCycle = 0;
        public int VacJetPeriod = 0;
        public int VacuumPumpPercentage = 0;

        #endregion

        public static string DumpHeader()
        {
            return string.Format("{27,12} {0,4} {1,4} {2,4} {3,4} {4,4} {5,4} {6,4} {7,4} {8,4} {9,4} {10,4} {11,4} {12,4} {13,4} {14,4} {15,4} {16,4} {17,4} {18,4} {19,4} {20,4} {21,4} {22,4} {23,4} {24,4} {25,4}{26}",
                "J11", "VJ", "VJHC",
                "HC", "H",
                "J15", "J16", "J17", "J18",
                "BL1", "PPWR", "RL1", "RL2",
                "12V", "5V",
                "S1", "S2", "S3", "S4", "S5",
                "LRF",
                "SPC", "SPH", "CYCL", "PER", "VACP",
                Environment.NewLine,
                "Time");
        }

        public string Dump()
        {
            return string.Format("{27:hh:mm:ss.fff} {0,4} {1,4} {2,4} {3,4} {4,4} {5,4} {6,4} {7,4} {8,4} {9,4} {10,4} {11,4} {12,4} {13,4} {14,4} {15,4} {16,4} {17,4} {18,4} {19,4} {20,4} {21,4} {22,4} {23,4} {24,4} {25,4}{26}",
                VacuumPump.X(), VacJetEnabled.X(), VacJetHotColdMode.X(),
                HotColdEnabled.X(), HotEnabled.X(),
                Exmass1.X(), Exmass2.X(), Exmass3.X(), Exmass4.X(),
                BlueLed1.X(), PrinterPower.X(), RedLed1.X(), RedLed2.X(),
                Sbc12V.X(), Sbc_5V.X(),
                Solenoid1.X(), Solenoid2.X(), Solenoid3.X(), Solenoid4.X(), Solenoid5.X(),
                BlueRfidLeds.X(),
                ColdTempSetpoint, HotTempSetpoint, VacJetDutyCycle, VacJetPeriod, VacuumPumpPercentage,
                Environment.NewLine,
                DateTime.Now);
        }

        public void ResetAll()
        {
            BlueLed1 = true;  // set charger on 002633 0020-05

            PrinterPower = false;
            RedLed1 = false;
            RedLed2 = false;
            Sbc_5V = true; //Requires M command
            Sbc12V = true; //Requires M command
            VacuumPump = false;
            VacuumPumpPercentage = 0;
            Exmass1 = false;
            Exmass2 = false;
            Exmass3 = false; //Requires M Command
            Exmass4 = false;
            Solenoid1 = false;
            Solenoid2 = false;
            Solenoid3 = false;
            Solenoid4 = false;
            Solenoid5 = false;
            BlueRfidLeds = false;
            VacJetEnabled = false;
            VacJetHotColdMode = false;
            HotColdEnabled = false;
            HotEnabled = false;
            VacJetPeriod = 0;
            VacJetDutyCycle = 0;
            HotTempSetpoint = 110;
            ColdTempSetpoint = 40;
        }

        private int Int(bool b)
        {
            return b ? 1 : 0;
        }

        private string BoolsToHexChar(bool bit0, bool bit1, bool bit2, bool bit3)
        {
            //string ret = OFF;

            //string binaryString = (bit3) ? ON : OFF;
            //binaryString += (bit2) ? ON : OFF;
            //binaryString += (bit1) ? ON : OFF;
            //binaryString += (bit0) ? ON : OFF;

            //ret = Convert.ToByte(binaryString, 2).ToString();

            //return ret;

            var number = Int(bit0) + Int(bit1)*2 + Int(bit2)*4 + Int(bit3)*8;
            return string.Format("{0:X}", number);
        }

        public string GetRequestSetAllOutputs()
        {
            return GetBaseSetIOString(true);
        }

        /// <summary>
        /// Returns the ~M Command
        /// </summary>
        /// <returns></returns>
        public string GetRequestSetIOStringManual()
        {
            return GetBaseSetIOString(false);
        }

        //  Command =>S
        private string GetBaseSetIOString(bool ackOnlyRequested)
        {
            var commandConstructor = new StringBuilder();

            commandConstructor.Append(ackOnlyRequested ? RequestSetAllNoAck : RequestSetIOManual);

            //Offset 2: LED Boolean Array to string
            commandConstructor.Append(BoolsToHexChar(BlueLed1, PrinterPower, RedLed1, RedLed2));

            // J11
            //Offset 3: Vacuum Pump Portion, this is the automatic one so there is no need to specify power on
            //commandConstructor.Append(BoolsToHexChar(Sbc_5V, Sbc12V, VacuumPump, false));
            commandConstructor.Append(BoolsToHexChar(Sbc_5V, Sbc12V, VacuumPump, DC_Motor)); // 2014 07/14 Modified by sww



            //Offset 4: Exmass  ,j15,j16,j17,j18
            commandConstructor.Append(BoolsToHexChar(Exmass1, Exmass2, Exmass3, Exmass4));

            commandConstructor.Append(BoolsToHexChar(VacJetEnabled, VacJetHotColdMode, HotColdEnabled, HotEnabled));

            //Offset 5, 6, 7: Volume - ask rich, does 100, just sent 100 ?  75 send 075? same with BVCON
            //TODO
            commandConstructor.Append("100");

            //Offset 8: Solenoid 1 to 4
            commandConstructor.Append(BoolsToHexChar(Solenoid1, Solenoid2, Solenoid3, Solenoid4));

            //Offset 9: Solenoid 5 + Blue LEDs (TODO, ask Rich isn't this a repeat of 2? or different LED)
            commandConstructor.Append(BoolsToHexChar(Solenoid5, BlueRfidLeds, false, false));

            //Offset 10-12 BVCON 
            //TODO
            commandConstructor.Append(VacuumPumpPercentage.ToString("000"));

            commandConstructor.Append(VacJetPeriod.ToString("000")); //VAC Jet Period
            commandConstructor.Append(VacJetDutyCycle.ToString("000")); //VAC Jet Duty Cycle
            commandConstructor.Append(HotTempSetpoint.ToString("000")); //Hot Temp Setpoint  
            commandConstructor.Append(ColdTempSetpoint.ToString("000")); //Cold Temp Setpoint 

            //2014 08/08 disabled by sww 
            //Offset 13, 14, 15, 16 CRC (not used currently)
            //commandConstructor.Append("1111"); //TBD: Modified by Bob

            var ret = commandConstructor.ToString();

            return ret;
        }
    }
}