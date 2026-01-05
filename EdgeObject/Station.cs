using System;
using System.Configuration;
using System.IO;
using System.Linq;

using System.Text;

namespace Edge.EdgeObject
{
    public class Station
    {
        public const string NoBottleUid = "0000000000000000";

        public bool IsTagValid = false;
        public string Uid = string.Empty;
        public string mFullBottleData = string.Empty;
        private bool mPendingBottleVerification;
        private bool logCommunication = false;

        public Station()
        {
            IsTagValid = false;
            mPendingBottleVerification = false;
            Uid = NoBottleUid;

            var logOutputsString = ConfigurationManager.AppSettings["LogCommunication"];
            logCommunication = logOutputsString != null && Boolean.Parse(logOutputsString);
        }

        public Station(bool pIsTagValid, string pStationUID)
        {
            IsTagValid = pIsTagValid;
            mPendingBottleVerification = true;
            Uid = (pStationUID != "2020202020202020") ? pStationUID : NoBottleUid;
        }

        //public Bottle mBottle;

        public string FillCode
        {
            get { return DataIsValid() ? mFullBottleData.Substring(0, 5) : null; }
        }

        public string BatchFillNumber
        {
            get { return DataIsValid() ? mFullBottleData.Substring(5, 1) : null; }
        }

        public string PartNumber
        {
            get { return DataIsValid() ? mFullBottleData.Substring(6, 5) : null; }
        }

        public string PartNumberRevisionLevel
        {
            get { return DataIsValid() ? mFullBottleData.Substring(11, 1) : null; }
        }

        public string SerialNumber
        {
            get { return DataIsValid() ? mFullBottleData.Substring(12, 7) : null; }
        }
      
        // 2014 10/29 add by sww 
        public string ProductName
        {
            //get { return DataIsValid() ? mFullBottleData.Substring(20, 20) : null; }
            get 
            {
                if (DataIsValid())
                    return TrimTheString(mFullBottleData.Substring(20, 20));
                else
                    return null;
            }
        }

        private string TrimTheString(string s )
        {
            if (s.IndexOf('^') > 0)
               return(s.Substring (0,s.IndexOf('^')));
            else
               return s;
        }

        public string ProductSize
        {
            //get { return DataIsValid() ? mFullBottleData.Substring(40, 10) : null; }
            get 
            {
                if (DataIsValid())
                    return TrimTheString(mFullBottleData.Substring(40, 15)) ; 
                else
                   return null;
            }
        }

        public string CheckCode
        {
            get { return DataIsValid() ? mFullBottleData.Substring(55, 8) : null; }
        }

        public bool RequireFullBottleData
        {
            get { return mPendingBottleVerification; }
        }

        /// <summary>
        /// Update Station with another Station Status
        /// </summary>
        /// <param name="newStatus"></param>
        public void UpdateStation(Station newStatus)
        {
            if (!string.Equals(Uid, newStatus.Uid))
            {
                //Station changed
                //Future - log change of bottles

                // update the status
                Uid = newStatus.Uid;
                mFullBottleData = "";
                mPendingBottleVerification = (Uid != NoBottleUid);
            }
        }

        public void UpdateOneStationFullData(string uid,string fullData)
        {
            //For now just accept a string
            var newData = fullData.Trim();
            newData = newData.Replace("\0", "").Trim();
            //bool isAllZeros = EdgeUtility.Utility.StringIsValid(newData, 1, 1000, EdgeUtility.Utility.REGEX_ALL_ZEROS);

            // Need verify code here
            // Verify_RFID_Tag()
            
            // 2014  11/23
            if (DataIsValid(newData) && Verify_RFID_Tag(uid, newData))
            {
                mFullBottleData = newData;
                mPendingBottleVerification = false;
            }
            else
            {
                if (logCommunication)
                {
                    AddLogMessage("Invalid Data received from RFID details.");  // 0103-07
                }

                mPendingBottleVerification = false;
            }
        }

        public bool DataIsValid(string fullData = null)
        {
            if (fullData == null)
                fullData = mFullBottleData;

            // For demo purposes, if it's not the letter combos, then it's not valid. 
 
            // Verify RFID CheckCode here
            // 2014 10/29
            return fullData.Length >= 19 ;
        }

        // 2014 11/25
        private bool Verify_RFID_Tag(string uid, string s)
        {
            String hash = String.Empty;
           
            string verifycode ="";
            string code ="";

            //return true;

            if (s.Length >= 63)            // 63
            {    
                code= s.Substring(0,55);
                verifycode = s.Substring(55,8);
            }
            else
                return false;

            byte[] fs = System.Text.Encoding.ASCII.GetBytes(uid+code);

            RFID_crc32 rfid_crc32 = new RFID_crc32();
            foreach (byte b in rfid_crc32.ComputeChecksumBytes(fs))
                hash += b.ToString("x2").ToLower();

            string data = hash.ToString();

            if (verifycode== data)
                return true;
            else
                return false;

        }

        private FileInfo f;
        private void AddLogMessage(string s) // 0103-07
        {
            if (!logCommunication)
                return;

            try
            {
                // 2014 11/04
                var filename = "Diagnostics.txt";

                f = new FileInfo(filename);
                long L = f.Length;
                if (L > 900000)                                                                 // if file size > 900k
                {
                    var lines = System.IO.File.ReadAllLines(filename, Encoding.ASCII);          // 0106-09
                    int cnt = lines.Count();

                    // Cut all lines in half 
                    File.WriteAllLines(filename, lines.Skip(cnt / 2).ToArray(), Encoding.ASCII);// 0106-09
                }

                File.AppendAllText(filename,
                        string.Format("{0}-{1}{2}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), s, Environment.NewLine),
                        Encoding.ASCII);   // 0106-09
            }

            catch (Exception ex)
            {

            }
        }
    }
}