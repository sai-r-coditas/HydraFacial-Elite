using System;
using System.Globalization;
using Edge.EdgeObject;
using JetBrains.Annotations;

namespace Edge.IOBoard
{
    // =>i
    public class ResponseGetAllInputs : Response
    {
        public Station[] DeviceStations = new Station[Constants.NUMBER_OF_STATIONS];
        public bool PowerPressed;
        public bool PowerLatched;
        public int HotTemperatureAtoD;
        public int ColdTemperatureAtoD;
        public int VacuumAtoD;

        public bool[] LedIsCharging = new bool[Constants.NUMBER_OF_LED_STATIONS];
        public int[] LedBatteryLevel = new int[Constants.NUMBER_OF_LED_STATIONS];

        //public int[] LedBatteryLevelCount = new int[Constants.NUMBER_OF_LED_STATIONS];
        //public int[] LedBatteryLevelTotal = new int[Constants.NUMBER_OF_LED_STATIONS];
       
        //Get All Input =>i
        public override void UpdateFromString([NotNull] string pResponseString)
        {
            try
            {
                var responseArray = pResponseString.ToCharArray();

                //2
                PowerPressed = (responseArray[2] == '1' || responseArray[2] == '3');
                PowerLatched = (responseArray[2] == '2' || responseArray[2] == '3');

                //3,4,5,6 is BVCON
                VacuumAtoD = int.Parse(pResponseString.Substring(3, 4));

                //7,8,9
                HotTemperatureAtoD = int.Parse(pResponseString.Substring(7, 3));
                
                //10,11,12
                ColdTemperatureAtoD = int.Parse(pResponseString.Substring(10, 3));

                var chargingHexChar = pResponseString.Substring(13, 1);
                var bits2 = int.Parse(chargingHexChar, NumberStyles.HexNumber);

                // 2015 06/17 0101-08
                #region for new firmware
                setBatteryRange(0,int.Parse(pResponseString.Substring(14, 3)));
                setBatteryRange(1,int.Parse(pResponseString.Substring(17, 3)));
                setBatteryRange(2,int.Parse(pResponseString.Substring(20, 3)));
                setBatteryRange(3,int.Parse(pResponseString.Substring(23, 3)));
                #endregion

                var stationsValidHexChar = pResponseString.Substring(27, 1);
                var bits = int.Parse(stationsValidHexChar, NumberStyles.HexNumber);

                DeviceStations[0] = new Station((bits & 1) == 1, pResponseString.Substring(28, 16));
                DeviceStations[1] = new Station((bits & 2) == 2, pResponseString.Substring(44, 16));
                DeviceStations[2] = new Station((bits & 4) == 4, pResponseString.Substring(60, 16));
                DeviceStations[3] = new Station((bits & 8) == 8, pResponseString.Substring(76, 16));
                DeviceStations[4] = new Station(pResponseString[26] == '1', pResponseString.Substring(92, 16));

                // 2014 10/22 filter out invalid UID
                if (DeviceStations[0].Uid == "0101010101010101")
                    ParseStatus = ParseStatusType.UnknownError;
                else
                    ParseStatus = ParseStatusType.Valid;
            }
            catch (Exception)
            {
                ParseStatus = ParseStatusType.ParseError;
            }
        }

        #region setBattery Range
        //private void setBatteryRange_old(int i, int L) // 0101-08
        //{
        //    if (L > 10)
        //    {
        //        LedIsCharging[i] = true;
        //        BoardManager.LedBatteryLevel_buf[i] = 0;
        //        BoardManager.LedBatteryLevelCount[i] = 0;
        //    }

        //    if (L > 750 && L <= 999)
        //        LedBatteryLevel[i] = 25;
        //    else if (L > 500 && L <= 750)
        //        LedBatteryLevel[i] = 50;
        //    else if (L > 250 && L <= 500)
        //        LedBatteryLevel[i] = 75;
        //    else if (L > 10 && L <= 250)
        //        LedBatteryLevel[i] = 90;
        //    else if (L >= 0 && L <= 10)
        //    {
        //        BoardManager.LedBatteryLevelCount[i]++;

        //        if (BoardManager.LedBatteryLevelCount[i] > 3)
        //            BoardManager.LedBatteryLevelCount[i] = 1;

        //        if (BoardManager.LedBatteryLevelCount[i] == 3)
        //        // Count 3 times then check if the charge station has no hand piece
        //        {
        //            if (BoardManager.LedBatteryLevelTotal[i] > 0)
        //            {
        //                LedBatteryLevel[i] = 100;
        //                BoardManager.LedBatteryLevel_buf[i] = 100;
        //                LedIsCharging[i] = true;
        //            }
        //            else
        //            {
        //                LedBatteryLevel[i] = 0;
        //                BoardManager.LedBatteryLevel_buf[i] = 0;
        //                LedIsCharging[i] = false;
        //            }

        //            BoardManager.LedBatteryLevelTotal[i] = 0; // reset Total
        //        }
        //        else
        //        {
        //            BoardManager.LedBatteryLevelTotal[i] = BoardManager.LedBatteryLevelTotal[i] + L;
        //            LedBatteryLevel[i] = BoardManager.LedBatteryLevel_buf[i];
        //        }
        //    }
        //}

        // ==================================================
        // Use 2 parameters to check battery
        // ==================================================
        //private void setBatteryRange(int i, int L) // 0101-09
        //{
        //    if (L > 10)
        //    {
        //        LedIsCharging[i] = true;
        //        BoardManager.LedBatteryLevel_buf[i] = 0;
        //        BoardManager.LedBatteryLevelCount[i] = 0;

        //        BoardManager.LedBatteryData[i, 0] = 0;
        //        BoardManager.LedBatteryData[i, 1] = 0;
        //    }

        //    if (L > 750 && L <= 999)
        //        LedBatteryLevel[i] = 25;
        //    else if (L > 500 && L <= 750)
        //        LedBatteryLevel[i] = 50;
        //    else if (L > 250 && L <= 500)
        //        LedBatteryLevel[i] = 75;
        //    else if (L > 10 && L <= 250)
        //        LedBatteryLevel[i] = 90;
        //    else if (L >= 0 && L <= 10)
        //    {
               
        //        if (BoardManager.LedBatteryData[i, 0] + BoardManager.LedBatteryData[i, 1] +L > 2)
        //            {
        //                LedBatteryLevel[i] = 100;
        //                BoardManager.LedBatteryLevel_buf[i] = 100;
        //                LedIsCharging[i] = true;
        //            }
        //            else
        //            {
        //                LedBatteryLevel[i] = 0;
        //                BoardManager.LedBatteryLevel_buf[i] = 0;
        //                LedIsCharging[i] = false;
        //            }

        //            BoardManager.LedBatteryData[i, 0] = BoardManager.LedBatteryData[i, 1]; // reset Total
        //            BoardManager.LedBatteryData[i,1] = L; // reset Total
              
        //    }
        //}

        // ==================================================
        // Use 3 parameters to check battery
        // ==================================================
        private void setBatteryRange(int i, int L) // 0101-09
        {
            if (L > 10)
            {
                LedIsCharging[i] = true;
                BoardManager.LedBatteryLevel_buf[i] = 0;
                BoardManager.LedBatteryLevelCount[i] = 0;

                BoardManager.LedBatteryData[i, 0] = 0;
                BoardManager.LedBatteryData[i, 1] = 0;
                BoardManager.LedBatteryData[i, 2] = 0;
            }

            if (L > 750 && L <= 999)
                LedBatteryLevel[i] = 25;
            else if (L > 500 && L <= 750)
                LedBatteryLevel[i] = 50;
            else if (L > 250 && L <= 500)
                LedBatteryLevel[i] = 75;
            else if (L > 10 && L <= 250)
                LedBatteryLevel[i] = 90;
            else if (L >= 0 && L <= 10)
            {

                if (BoardManager.LedBatteryData[i, 0] + BoardManager.LedBatteryData[i, 1] + BoardManager.LedBatteryData[i, 2] + L > 3)
                {
                    LedBatteryLevel[i] = 100;
                    BoardManager.LedBatteryLevel_buf[i] = 100;
                    LedIsCharging[i] = true;
                }
                else
                {
                    LedBatteryLevel[i] = 0;
                    BoardManager.LedBatteryLevel_buf[i] = 0;
                    LedIsCharging[i] = false;
                }

                BoardManager.LedBatteryData[i, 0] = BoardManager.LedBatteryData[i, 1]; // reset Total
                BoardManager.LedBatteryData[i, 1] = BoardManager.LedBatteryData[i, 2]; // reset Total
                BoardManager.LedBatteryData[i, 2] = L; // reset Total
            }
        }

        #endregion

        private int CheckFullCharge()
        {
            return 100;
        }

        private int UpdateBatteryLevel(bool devicePresent,int BatteryLevel)
        {
            if (!devicePresent)
                return 0;
            else
                return BatteryLevel;
        }

    }
}