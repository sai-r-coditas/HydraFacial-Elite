namespace Edge.IOBoard
{
    public static class RequestGetStationTagData
    {
        //public const string STATION_1 = "~D10000";
        //public const string STATION_2 = "~D20000";
        //public const string STATION_3 = "~D30000";
        //public const string STATION_4 = "~D40000";
        //public const string STATION_5 = "~D50000";

        public static string Message(int stationIndex)
        {
            //2014 08/08 modified for crc
            return string.Format("~D{0}", stationIndex + 1);
        }
    }
}