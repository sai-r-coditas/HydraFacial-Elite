using System;
using System.Xml.Serialization;

namespace Edge.Tower2.UI
{
    #region Democode
    public class N_crc16
    {
        const ushort polynomial = 0xA001;
        //const ushort polynomial = 0x0001;
        ushort[] table = new ushort[256];

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public N_crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
    #endregion

    #region Class c_DemoCode
    class c_DemoCode
    {
        private Int32 devicePin = 0;
        public void init(int s)
        {
            devicePin = s;
        }

        public ParsedCode parseCode(String code)
        {

            /*Code format:

             *  codes are either 13 digits or 6 digits in length
             *  
             *      (1) 12 digits have format   DDDIIICUUUNNK
             *          DDD - unencrypted # of days for Distributors to recognize the code
             *              - Range 001-998  (days)
             *              - 999 puts the device back into continuous runtime mode
             *              
             *          III - unencrypted code sequence (unique credid code identifier) for out-of-sequence support
             *              - Range 000-999
             *          
             * 
             *          The Encrypted portion CUUUNNK is a 7-digit long feed-forward OTP (One Time Pad produced by the III'th rand number sequence from pre-shared secret pin)         
             *          C   - encrypted control digit
             *              - 0-6:AddDaysAny
             *              - 7:ContinuousRuntime
             *              - 8,9:reserved for future use
             *              
             *          UUU - encrypted number of days to credit
             *              - Range 001-998
             *              - 999 for continuous runtime
             *              - Once decrypted it must match with DDD, else code is invalid
             *              
             *          NNK - Encrypted anti-tamper block consists of:
             *              - NN is a random nonce
             *              - K is a 1-digit checksum of the fully decrypted code
             *          
             *          
             * 
             *      (2) 6 digits has format PPPPPP, where PPPPPP is the pre-shared secret pin.  This is a master unlock code
             *          
             */

            if (code == null)  // 002639-08
                return new ParsedCode(ParsedCode.CodeStatus.IncorrectCodeLength, 0, code, string.Empty);

            switch (code.Length)
            {
                case 6:
                    {
                        try
                        {
                            int enteredPin = Int32.Parse(code);

                            if (enteredPin == devicePin)
                                return new ParsedCode(ParsedCode.CodeStatus.ValidCodeMasterUnlock, 0, code, string.Empty);
                            else
                                return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, string.Empty);
                        }
                        catch (Exception e)
                        {
                            return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, string.Empty);
                        }
                    }

                case 13:
                    {
                        String DDD = "", III = "", CUUUNNK_e = "", CUUUNNK_d = "";
                        Int32 ddd, iii, c, uuu, nn, k;

                        try
                        {
                            DDD = code.Substring(0, 3);
                            III = code.Substring(3, 3);
                            CUUUNNK_e = code.Substring(6);
                            CUUUNNK_d = "";
                            ddd = Int32.Parse(DDD);
                            iii = Int32.Parse(III);
                        }
                        catch (Exception e)
                        {
                            return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, string.Empty);
                        }

                        //Start with new generator
                        Random rGen = new Random(devicePin);
                        Int32 decryptCode = 0;
                        for (int i = 0; i <= iii; i++)
                        {
                            decryptCode = rGen.Next((Int32)9999999);
                        }

                        CUUUNNK_d = DecryptOTPData(CUUUNNK_e, decryptCode.ToString().PadLeft(7, '0'));
                        c = Int32.Parse(CUUUNNK_d.Substring(0, 1));
                        uuu = Int32.Parse(CUUUNNK_d.Substring(1, 3));
                        nn = Int32.Parse(CUUUNNK_d.Substring(4, 2));
                        k = Int32.Parse(CUUUNNK_d.Substring(6, 1));

                        if (CUUUNNK_d.Substring(4, 2).CompareTo(decryptCode.ToString().PadLeft(7, '0').Substring(4, 2)) != 0)
                            return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, III);

                        int checkSum = (ddd + iii + c + uuu + nn) % 10;
                        if (k != checkSum)
                            return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, III);

                        if (uuu != ddd)
                            return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, III);

                        switch (c)
                        {
                            case 1:     //AddDaysAny
                            case 2:     //AddDaysAny
                            case 3:     //AddDaysAny
                            case 4:     //AddDaysAny
                            case 5:     //AddDaysAny
                            case 6:     //AddDaysAny
                                {
                                    return new ParsedCode(ParsedCode.CodeStatus.ValidCodeDays, uuu, code, III);
                                }

                            case 7:     //ContinuousRuntime
                                {
                                    if (uuu != 999)
                                        return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, III);
                                    else
                                        return new ParsedCode(ParsedCode.CodeStatus.ValidCodeContinuousMode, uuu, code, III);
                                }

                            default:
                                {
                                    return new ParsedCode(ParsedCode.CodeStatus.InvalidCode, 0, code, III);
                                }
                        }
                    }

                default:
                    return new ParsedCode(ParsedCode.CodeStatus.IncorrectCodeLength, 0, code, string.Empty);
            }
        }

        private String DecryptOTPData(String encryptedData_s, String oneTimePad_s)
        {
            Int32 decryptedDigit, encryptedDigit, otpDigit;
            String encryptedDigit_s, otpDigit_s, decryptedData_s = "";
            Int32 feedForward = 0;

            while (encryptedData_s.Length > 0)
            {
                encryptedDigit_s = encryptedData_s.Substring(0, 1);
                otpDigit_s = oneTimePad_s.Substring(0, 1);

                encryptedDigit = Int32.Parse(encryptedDigit_s);
                otpDigit = Int32.Parse(otpDigit_s);

                decryptedDigit = (encryptedDigit - otpDigit) - feedForward;
                while (decryptedDigit < 0)
                    decryptedDigit += 10;
                decryptedDigit = decryptedDigit % 10;

                feedForward = encryptedDigit;

                decryptedData_s += decryptedDigit.ToString();

                encryptedData_s = encryptedData_s.Substring(1);
                oneTimePad_s = oneTimePad_s.Substring(1);
            }

            return decryptedData_s;
        }
    }
    #endregion

    #region Class declare for XML  // 0102-05
    [Serializable()]
    public class Items
    {
        [System.Xml.Serialization.XmlElement("Code")]
        public string Code { get; set; }

        [System.Xml.Serialization.XmlElement("Date")]
        public string Date { get; set; }

    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("Demo")]   // Name
    public class c_XmlDemoCode
    {
        [XmlArray("Group")]
        [XmlArrayItem("SubGroup", typeof(Items))]
        public Items[] items { get; set; }
    }
    #endregion
}
