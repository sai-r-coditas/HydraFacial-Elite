using System;
using Edge.EdgeUtility;
using JetBrains.Annotations;

namespace Edge.IOBoard
{
    public class ResponseGetTagData : Response
    {
        public string RfidData;
        public int StationId;
                             //Get Tag   
        public override void UpdateFromString([NotNull] string pResponseString)
        {
            try
            {
                //2	'1' .. '5',  ASCII hex	Station number: '1','2','3','4','5'
                StationId = int.Parse(pResponseString.Substring(2, 1));

                //3	'0' .. 'F',  ASCII hex	Byte 1 of 100 data bytes, High Nibble
                //4	'0' .. 'F',  ASCII hex	Byte 1 of 100 data bytes, Low Nibble
                //5..202	'0' .. 'F',  ASCII hex	Bytes 2 through 100
                //Parse tag data
                //For now there's only 100 characters maximum available
                //And for now parsing it into readable string...
                RfidData = Utility.HexStringToASCIIString(pResponseString.Substring(3, 200));
                ParseStatus = ParseStatusType.Valid;
            }
            catch (Exception)
            {
                ParseStatus = ParseStatusType.ParseError;
            }
        }
    }
}