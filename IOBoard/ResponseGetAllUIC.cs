using System;
using Edge.EdgeObject;
using JetBrains.Annotations;

namespace Edge.IOBoard
{
    public class ResponseGetAllUic : Response
    {
        public Station[] DeviceStations = new Station[Constants.NUMBER_OF_STATIONS];

                             // Get Uic
        public override void UpdateFromString([NotNull] string pResponseString)
        {
            try
            {
                var responseArray = pResponseString.Substring(2, 4).ToCharArray();

                var stationStatus_5 = Convert.ToString((byte) responseArray[0], 2);
                var stationStatus_4_To_1 = Convert.ToString((byte) responseArray[1], 2).ToCharArray();

                var stationChange_5 = Convert.ToString((byte) responseArray[2], 2);
                var stationChange_4_To_1 = Convert.ToString((byte) responseArray[3], 2).ToCharArray();

                DeviceStations[0] = new Station(stationStatus_4_To_1[3] == '1', pResponseString.Substring(6, 16));
                DeviceStations[1] = new Station(stationStatus_4_To_1[2] == '1', pResponseString.Substring(22, 16));
                DeviceStations[2] = new Station(stationStatus_4_To_1[1] == '1', pResponseString.Substring(38, 16));
                DeviceStations[3] = new Station(stationStatus_4_To_1[0] == '1', pResponseString.Substring(54, 16));
                DeviceStations[4] = new Station(string.Equals(stationStatus_5, "1"), pResponseString.Substring(70, 16));

                //86	'0' .. 'F',  ASCII hex	CRC 16, High Nibble
                //87	'0' .. 'F',  ASCII hex	CRC 16 
                //88	'0' .. 'F',  ASCII hex	CRC 16 
                //89	'0' .. 'F',  ASCII hex	CRC 16, Low Nibble

                ParseStatus = ParseStatusType.Valid;
            }
            catch (Exception)
            {
                ParseStatus = ParseStatusType.ParseError;
            }
        }
    }
}