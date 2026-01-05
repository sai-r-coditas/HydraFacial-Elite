using System;
using JetBrains.Annotations;

namespace Edge.IOBoard
{
    public class ResponseSetIO : Response
    {
        public bool PowerPressed;
        public int TemperatureAtoD;
        public int VacuumAtoD;

                             // Set IO
        public override void UpdateFromString([NotNull] string pResponseString)
        {
            try
            {
                var responseArray = pResponseString.ToCharArray();

                //2
                PowerPressed = (responseArray[2] == '1');

                //3,4,5,6 is BVCON
                VacuumAtoD = int.Parse(pResponseString.Substring(3, 4));

                //7,8,9,10 CRC, not currently used
                TemperatureAtoD = int.Parse(pResponseString.Substring(7, 4));

                ParseStatus = ParseStatusType.Valid;
            }
            catch (Exception)
            {
                ParseStatus = ParseStatusType.ParseError;
            }
        }
    }
}