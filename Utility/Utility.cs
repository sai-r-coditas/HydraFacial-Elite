using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Edge.EdgeUtility
{
    public class Utility
    {
        public const string RegexNumericOnly = "^[0-9]+$"; //0 to 9 only
        public const string RegexAlphaNumericOnly = "^[a-zA-Z0-9]+$"; //a to z, A to Z, 0 to 9 only
        public const string RegexAllZ = "^[Z]+$"; //all Z's
        public const string RegexAllZeros = "^[0]+$";

        /// <summary>
        /// Checks if string is has the correct length and has only accepted characters
        /// </summary>
        /// <param name="pInputString"></param>
        /// <param name="minLength"></param>
        /// <param name="pMaxLength"></param>
        /// <param name="acceptedFormat">Regular expression</param>
        /// <returns></returns>
        public static bool StringIsValid(string pInputString, int minLength, int pMaxLength,
                                         string acceptedFormat = null)
        {
            var isValidString = false;

            if (!string.IsNullOrEmpty(pInputString) && pInputString.Length >= minLength &&
                pInputString.Length <= pMaxLength)
            {
                if (!string.IsNullOrEmpty(acceptedFormat))
                {
                    var regex = new Regex(acceptedFormat);
                    isValidString = regex.IsMatch(pInputString);
                }
            }

            return isValidString;
        }

        public static byte[] StringToByteArray(string data)
        {
            var byteConverter = new ASCIIEncoding();
            return byteConverter.GetBytes(data);
        }

        public static string ByteArrayToString(byte[] data)
        {
            var byteConverter = new ASCIIEncoding();
            return byteConverter.GetString(data);
        }

        /// <summary>
        /// Increments 0-9 and A-Z, if lower case letter is passed in, it is converted to upper and incremented
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool AlphaNumericIncrement(ref char a)
        {
            var aByte = (byte) char.ToUpper(a);
            var ret = AlphaNumericIncrement(ref aByte);
            a = (char) aByte;

            return ret;
        }

        /// <summary>
        /// Increments 0-9 and A-Z.  Lower case characters does NOT count
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool AlphaNumericIncrement(ref byte a)
        {
            var overFlow = false;

            if (a == 90) //Z
            {
                overFlow = true;
                a = 48; // the digit, zero 
            }
            else if (a == 57) //the digit 9
            {
                a = 65; //A
            }
            else
            {
                a++; //otherwise increment 
            }

            return overFlow;
        }

        public static string HexStringToASCIIString(string hexString)
        {
            var index = 0;
            var hexStringLength = hexString.Length;
            var b = new StringBuilder();
            while (index < hexStringLength)
            {
                b.Append((char) int.Parse(hexString.Substring(index, 2), NumberStyles.HexNumber));
                index += 2;
            }

            var ret = b.ToString();
           
            return ret;
        }
    }
}