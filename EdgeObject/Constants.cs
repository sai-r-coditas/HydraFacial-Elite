using System;

namespace Edge
{
    public enum StationNumber
    {
        Station1,
        Station2,
        Station3,
        Station4,
        Station5
    }

    public static class Constants
    {
        public const int NUMBER_OF_LED_STATIONS = 4;
        public const int NUMBER_OF_STATIONS = 5;
        public static readonly TimeSpan MAX_RESPONSE_TIME;

        static Constants()
        {
            MAX_RESPONSE_TIME = TimeSpan.FromMilliseconds(999000);
        }
    }
}