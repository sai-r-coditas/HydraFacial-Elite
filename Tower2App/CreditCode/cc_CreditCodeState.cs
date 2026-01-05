using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace Edge.Tower2.UI
{
    public enum State
    {
        PowerUp,
        CheckClock,
        CheckCredit,
        InitialInstructions,
        InitialAccountNum,
        InitialAccountPIN,
        InitialConfirm,
        NoCreditRemaining,
        CodeEntry,
        CodeAccepted,
        CodeNotAccepted,
        LockWarning,
        LockOut,
        ClockYear,
        ClockMonth,
        ClockDay,
        ClockHour,
        ClockMinute,
        MainScreen
    }

    //this should purely store state, advance state and handle logic related things
    //keep out code that "displays" stuff, 
    [Serializable]
    [XmlRoot("Everest")]
    public class CreditCodeState
    {
        private static string FILEPATH_CREDIT_CODE_STATE = @"c:\Disk\state.bin"; // 0102-33
        private static string LOCK_OUT_RESCUE_FILE_PATH = @"masterUnlock";     // 0102-05
        private static string MASTER_RESET_RESCUE_FILE_PATH = @"masterReset";  // 0102-05
        private static string MASTER_RESET_RESCUE_CODE = @"AAAAB3NzaC1yc2EAAAABJQAAAIEA5bcMw0uyogdiG+ufFHArQSlgSe98wclZdOqVita2OqZ3WxAizJUH8EFLm4EShJw1IS+ng3PAn9DhicksjrMupf8mxZduDIlnMmpNHkZy0zYRT//qI5AmKqQtdbuxefmrvl3mGWVUoMsVL5gleMjnDxH9diazRmkBPfcJVIHcCs0=";

        private static string FOLDERPATH_CREDIT_CODE_STATE = @"c:\Disk"; // 0102-35

        public static string DEFAULT_ACCOUNT_PREFIX = @"HFE-";

        //keeps the internal "credit" state of the machine
        #region Internal State Tracking
        [XmlIgnore]
        private State mInit;
        [XmlIgnore]
        private State mPrevious;
        [XmlIgnore]
        private State mCurrent;
        #endregion

        #region Temporary Variables

        //For temporary use
        [XmlIgnore]
        private string clockYear;
        [XmlIgnore]
        private string clockMonth;
        [XmlIgnore]
        private string clockDay;
        [XmlIgnore]
        private string clockHour;
        [XmlIgnore]
        private string clockMinute;
        [XmlIgnore]
        private string creditCode;
        [XmlIgnore]
        private int justAddedDays;
        #endregion

        #region Persisted Credit Code Information

        //Information to persist, TODO, obscure
        public string Kilimanjaro
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }
        private string accountNumber;

        public string VinsonMassif
        {
            get { return accountPIN; }
            set { accountPIN = value; }
        }
        private string accountPIN;

        public DateTime Kosciusko
        {
            get { return creditEmptyTime; }
            set { creditEmptyTime = value; }
        }
        private DateTime creditEmptyTime;

        public DateTime Elbrus
        {
            get { return lastStateUpdateTime; }
            set { lastStateUpdateTime = value; }
        }
        private DateTime lastStateUpdateTime;

        public DateTime MontBlanc
        {
            get { return lockOutExpireTime; }
            set { lockOutExpireTime = value; }
        }
        private DateTime lockOutExpireTime;

        public int PuncakJaya
        {
            get { return mCodeAttemptCount; }
            set { mCodeAttemptCount = value; }
        }
        private int mCodeAttemptCount;

        public List<string> TheAndes
        {
            get { return listSequenceNumbersUsed; }
            set { listSequenceNumbersUsed = value; }
        }
        
        // ver 00262
        private List<string> listSequenceNumbersUsed;
       
        #endregion

        #region Init / Parameters
        public CreditCodeState()
        {
            mInit = State.InitialInstructions;
        }

        /// <summary>
        /// What started the credit code state
        /// </summary>
        public State Initial
        {
            get
            {
                return mInit;
            }
        }

        public State Current
        {
            get
            {
                return mCurrent;
            }
        }

        [XmlIgnore]
        public string StateParam
        {
            get
            {
                string ret = string.Empty;

                switch (Current)
                {
                    case State.InitialAccountNum:
                        ret = String.IsNullOrEmpty(accountNumber) ? DEFAULT_ACCOUNT_PREFIX : accountNumber;
                        break;
                    case State.InitialAccountPIN:
                        ret = accountPIN;
                        break;
                    case State.InitialConfirm:
                        ret = accountNumber + "   " + "PIN: " + accountPIN;
                        break;
                    case State.CodeAccepted:
                        if (justAddedDays == 999)
                        {
                            ret = "Unlimited mode.";
                        }
                        else
                        {
                            int totalDays = (int)Decimal.Round((Decimal)(creditEmptyTime.Subtract(DateTime.Now).TotalDays), 0);
                            ret = "+ " + justAddedDays.ToString() + "   " + "= " + totalDays.ToString();
                        }
                        break;
                    case State.CodeEntry:
                        ret = creditCode;
                        break;
                    case State.ClockYear:
                        ret = clockYear;
                        break;
                    case State.ClockMonth:
                        ret = clockMonth;
                        break;
                    case State.ClockDay:
                        ret = clockDay;
                        break;
                    case State.ClockHour:
                        ret = clockHour;
                        break;
                    case State.ClockMinute:
                        ret = clockMinute;
                        break;
                    default:
                        break;
                }

                return ret;
            }
        }
        #endregion

        #region State Next/Back
        /// <summary>
        /// Go to the next state
        /// </summary>
        /// <returns></returns>
        public State Next(string param)
        {
            mPrevious = mCurrent;

            Utility.Lib.CreditLog("Next-(enter state)" + mCurrent.ToString()); // 0103-10

            switch (mCurrent)
            {
                case State.PowerUp:
                    mCurrent = (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(accountPIN)) ? State.InitialInstructions : State.CheckClock;
                    break;
                case State.CheckClock:
                    mCurrent = VerifyClock() ? State.CheckCredit : State.ClockYear;
                    break;
                case State.CheckCredit:
                    if (IsLockedOut())
                    {
                        mCurrent = State.LockOut;
                    }
                    else
                    {
                        mCurrent = HasCredit() ? State.MainScreen : State.CodeEntry;
                    }
                    SaveState();
                    break;
                case State.InitialInstructions:
                    mCurrent = State.InitialAccountNum;
                    break;
                case State.InitialAccountNum:
                    accountNumber = param;
                    mCurrent = State.InitialAccountPIN;
                    break;
                case State.InitialAccountPIN:
                    accountPIN = param;
                    mCurrent = State.InitialConfirm;
                    break;
                case State.InitialConfirm:
                    mInit = State.NoCreditRemaining;
                    mCurrent = State.NoCreditRemaining;
                    break;
                case State.NoCreditRemaining:
                    mCurrent = State.CodeEntry;
                    SaveState();
                    break;
                case State.CodeEntry:
                    creditCode = param;
                    mCurrent = VerifyCode(param);
                    break;
                case State.CodeAccepted:
                    creditCode = string.Empty;
                    param = string.Empty;
                    mCurrent = State.CheckCredit;
                    SaveState();
                    break;
                case State.CodeNotAccepted:
                    mCodeAttemptCount++;
                    if (mCodeAttemptCount == 3)
                    {
                        mCurrent = State.LockWarning;
                    }
                    else if (mCodeAttemptCount >= 5)     // 0102-31
                    {
                        mCurrent = State.LockOut;
                    }
                    else
                    {
                        mCurrent = State.CodeEntry;
                    }
                     
                    if (mCurrent == State.LockOut)
                    {
                        lockOutExpireTime = DateTime.Now.AddHours(1.0);
                    }
                    SaveState();
                    break;
                case State.LockOut:
                    
                    //stays in lock out until lock out time expires and the user reboots, no action
                    //unless the usb drive contains a file with the master unlock code
                    if (LockedOutDeviceRescue())
                    {
                        mCurrent = State.CodeEntry;
                    }
                    else if (MasterResetRescue())
                    {
                        if (System.Windows.Forms.MessageBox.Show(App.getTextMessages("Master Reset"),   // 0103-09
                                                                App.getTextMessages("Continue master reset?"),
                                                                System.Windows.Forms.MessageBoxButtons.OKCancel,
                                                                System.Windows.Forms.MessageBoxIcon.Exclamation,
                                                                System.Windows.Forms.MessageBoxDefaultButton.Button2)
                            == System.Windows.Forms.DialogResult.OK)
                        {
                            MasterUnlockReset();  // clear account number
                        }
                    }
                    SaveState();
                    break;
                case State.LockWarning:
                    mCurrent = State.CodeEntry;
                    break;
                case State.ClockYear:
                    clockYear = param;
                    mCurrent = State.ClockMonth;
                    break;
                case State.ClockMonth:
                    clockMonth = param;
                    mCurrent = State.ClockDay;
                    break;
                case State.ClockDay:
                    clockDay = param;
                    mCurrent = State.ClockHour;
                    break;
                case State.ClockHour:
                    clockHour = param;
                    mCurrent = State.ClockMinute;
                    break;
                case State.ClockMinute:
                    clockMinute = param;
                    SetTime();
                    mCurrent = VerifyClock() ? State.CheckCredit : State.ClockYear;

                    if (mCurrent == State.ClockYear) // 0106-06
                    {
                        updateremaindays();
                        SaveState();
                        mCurrent = State.CheckCredit;
                        break;
                    }

                    SaveState();
                    break;
                case State.MainScreen:
                    creditCode = string.Empty;
                    SaveState();
                    mCurrent = State.CodeEntry;
                    break;
                default:
                    break;
            }

            Utility.Lib.CreditLog("Next-(result)"+ mCurrent.ToString()); // 0103-10-2

            return mCurrent;
        }

        private void updateremaindays_notinused()  // 0106-06
        {
            DateTime dt = new DateTime(Int32.Parse(clockYear), Int32.Parse(clockMonth), Int32.Parse(clockDay), Int32.Parse(clockHour), Int32.Parse(clockMinute), 0);

            DateTime dt_expired = this.Kosciusko;

            double totalminutes = dt_expired.Subtract(lastStateUpdateTime).TotalMinutes;

            dt = dt.AddMinutes(totalminutes);

            this.Kosciusko = dt;
        }

        private void updateremaindays()  // 0106-06
        {
            DateTime dt_expired = this.Kosciusko;

            double totalminutes = dt_expired.Subtract(lastStateUpdateTime).TotalMinutes;

            //this.Elbrus
            DateTime dt = DateTime.Now;

            // expired date
            dt = dt.AddMinutes(totalminutes);

            this.Kosciusko = dt;
        }
        /// <summary>
        /// Go to the previous state
        /// </summary>
        /// <returns></returns>
        public State Back(string param)
        {
            mPrevious = mCurrent;

            switch (mCurrent)
            {
                case State.PowerUp:
                    //internal state, no moving back
                    break;
                case State.CheckClock:
                    //internal state, no moving back
                    break;
                case State.CheckCredit:
                    //internal state, no moving back
                    break;
                case State.InitialInstructions:
                    //internal state, no moving back
                    break;
                case State.InitialAccountNum:
                    mCurrent = State.InitialInstructions;
                    break;
                case State.InitialAccountPIN:
                    mCurrent = State.InitialAccountNum;
                    break;
                case State.InitialConfirm:
                    //this jumps back to account num so they have a chance to review both without erasing the PIN
                    mCurrent = State.InitialAccountNum;
                    break;
                case State.NoCreditRemaining:
                    //no moving back
                    break;
                case State.CodeEntry:
                    mCurrent = State.CheckCredit;
                    break;
                case State.CodeAccepted:
                    //no moving back
                    break;
                case State.CodeNotAccepted:
                    //no moving back
                    break;
                case State.LockOut:
                    //no moving back
                    break;
                case State.LockWarning:
                    mCurrent = State.CodeEntry;
                    break;
                case State.ClockYear:
                    //no moving back
                    break;
                case State.ClockMonth:
                    mCurrent = State.ClockYear;
                    break;
                case State.ClockDay:
                    mCurrent = State.ClockMonth;
                    break;
                case State.ClockHour:
                    mCurrent = State.ClockDay;
                    break;
                case State.ClockMinute:
                    mCurrent = State.ClockHour;
                    break;
                case State.MainScreen:
                    //no moving back
                    break;
                default:
                    break;
            }

            return mCurrent;
        }
        #endregion

        // ver 0002638
        #region Verify Code
        private MachineAlgorithm a;
        private ParsedCode c;
        private State ret;
        private State VerifyCode(string code)
        {
            a = new MachineAlgorithm();
            a.init(int.Parse(accountPIN));
            c = a.parseCode(code);

            Utility.Lib.CreditLog("VerifyCode ->"+c.getParsedCodeStatus().ToString());          // 0103-10-2

            switch (c.getParsedCodeStatus())
            {
                case ParsedCode.CodeStatus.InvalidCode:
                case ParsedCode.CodeStatus.IncorrectCodeLength:
                    ret = State.CodeNotAccepted;
                    break;
                case ParsedCode.CodeStatus.ValidCodeMasterUnlock:
                    
                    //master unlock resets back to account number entry
                    MasterUnlockReset();
                    ret = State.InitialAccountNum;
                    break;
                case ParsedCode.CodeStatus.ValidCodeContinuousMode:
                    creditEmptyTime = DateTime.MaxValue;
                    ret = State.CodeAccepted;

                    this.lockOutExpireTime = DateTime.MinValue;                                 // sww 0102-31
                    mCodeAttemptCount = 0;                                                      // sww 0102-31

                    break;
                case ParsedCode.CodeStatus.ValidCodeDays: //???
                    
                    if (listSequenceNumbersUsed.Contains(c.getSequence()))
                    {
                        ret = State.CodeNotAccepted;
                    }
                    else
                    {
                        listSequenceNumbersUsed.Add(c.getSequence());
                        if (creditEmptyTime < DateTime.Now)
                        {
                            creditEmptyTime = DateTime.Now;
                        }
                        
                        DateTime _dt = Convert.ToDateTime("1/1/9999");                          //sww
                        if (creditEmptyTime < _dt)                                              // If code is unlimit, then do nothing
                        {
                            creditEmptyTime = creditEmptyTime.AddDays(c.getDaysUnlocked());     // ??? 
                            justAddedDays = c.getDaysUnlocked();
                        }
                        else
                            justAddedDays = 0;   // Add no days

                        ret = State.CodeAccepted;

                        this.lockOutExpireTime = DateTime.MinValue;                             // sww 0102-31 Reset
                        mCodeAttemptCount = 0;                                                  // sww 0102-31
                    }
                    break;
                default:
                    ret = State.CodeNotAccepted;
                    break;
            }

            return ret;
        }
        #endregion

        #region Lockout/reset/master reset
        private bool VerifyClock()
        {
            return (DateTime.Compare(DateTime.Now, lastStateUpdateTime) > 0);
        }

        private bool IsLockedOut() 
        {
            //Empty lock out time or min value means not in lockout mode
            if (lockOutExpireTime == null || lockOutExpireTime == DateTime.MinValue)
            {
                return false;
            }

            //checks lock out time and compares to current machine time.  
            //if past the lock out time, then reset the lockout expire time
            bool lockOutExpired = (DateTime.Compare(lockOutExpireTime, DateTime.Now) <= 0);
            if (lockOutExpired)
            {
                lockOutExpireTime = DateTime.MinValue;
                mCodeAttemptCount = 0;
            }

            return !lockOutExpired;
        }

        private bool HasCredit()
        {
            return (DateTime.Compare(creditEmptyTime, DateTime.Now) > 0);
        }

        private void MasterUnlockReset()
        {
            this.accountNumber = "";
            this.accountPIN = "";
            this.creditCode = "";
            this.creditEmptyTime = DateTime.MinValue;
            this.lastStateUpdateTime = DateTime.MinValue;
            this.lockOutExpireTime = DateTime.MinValue;
            this.mCodeAttemptCount = 0;
            this.listSequenceNumbersUsed = new List<string>();
        }

        private bool LockedOutDeviceRescue()
        {
            bool ret = false;
            
            // 2015 01/19
            StreamReader r =null;
           
            if (File.Exists(GetUSBLetter()+LOCK_OUT_RESCUE_FILE_PATH))  // 0102-31
            {
                try
                {
                    //if the file contains pin, then allow setting to 
                    //r = new StreamReader(Environment.CurrentDirectory+LOCK_OUT_RESCUE_FILE_PATH);
                    r = new StreamReader(GetUSBLetter() + LOCK_OUT_RESCUE_FILE_PATH, Encoding.ASCII);   // 0106-09
                    if (r.ReadLine().Trim().CompareTo(this.accountPIN) == 0)
                    {
                        ret = true;
                    }
                    r.Close();
                }
                catch { }
            }

            return ret;
        }

        private bool MasterResetRescue()
        {
            bool ret = false;

            // 2015 01/19
            StreamReader r =null;

            if (File.Exists(GetUSBLetter() + MASTER_RESET_RESCUE_FILE_PATH))  // 0102-31
            {
                try
                {
                    // if the file contains pin, then allow setting to 
                    r = new StreamReader(GetUSBLetter() + MASTER_RESET_RESCUE_FILE_PATH, Encoding.ASCII); // 0102-31   0106-09
                    if (r.ReadLine().Trim().CompareTo(MASTER_RESET_RESCUE_CODE) == 0)
                    {
                        ret = true;
                    }
                    r.Close();
                }
                catch { }
            }

            return ret;
        }
        #endregion

        #region 2 Loadstate function
        public static CreditCodeState LoadState()
        {
            CreditCodeState state;

            //No credit code state file, initial boot up for credit code mode
            if (!File.Exists(FILEPATH_CREDIT_CODE_STATE))  //sww 0102-33
            {
                //Create the initial setup
                state = new CreditCodeState();
                state.mCurrent = State.InitialInstructions;
                state.mInit = State.InitialInstructions;
            }
            else
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(CreditCodeState));
                TextReader reader = new StreamReader(FILEPATH_CREDIT_CODE_STATE, Encoding.ASCII);  //sww 0102-33 0106-09
                state = (CreditCodeState)deserializer.Deserialize(reader);
                reader.Close();

                state.mCurrent = State.PowerUp;
                state.mInit = State.PowerUp;
            }

            return state;
        }

        // 2015 01/17
        public static CreditCodeState LoadState(bool CodeEntryMode)
        {
            CreditCodeState state;

            //No credit code state file, initial boot up for credit code mode
            if (!File.Exists(FILEPATH_CREDIT_CODE_STATE))  //sww 0102-33
            {
                //Create the initial setup
                state = new CreditCodeState();
                state.mCurrent = State.InitialInstructions;
                state.mInit = State.InitialInstructions;
            }
            else
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(CreditCodeState));
                TextReader reader = null;

                try
                {
                    reader = new StreamReader(FILEPATH_CREDIT_CODE_STATE,Encoding.ASCII); //sww 0102-33   0106-09
                    state = (CreditCodeState)deserializer.Deserialize(reader);
                    reader.Close();

                    // 2014 11/07

                    if (CodeEntryMode)
                    {
                        state.mCurrent = State.CodeEntry;
                        state.mInit = State.CodeEntry;
                    }
                    else
                    {
                        state.mCurrent = State.PowerUp;
                        state.mInit = State.PowerUp;
                    }

                }
                catch (Exception)
                {
                    // Invalid credit Code
                    if (reader != null)
                        reader.Close();

                    state = new CreditCodeState();
                    state.mCurrent = State.InitialInstructions;
                    state.mInit = State.InitialInstructions;

                    return state;
                }
            }

            return state;
        }
        #endregion

        #region Save State / Save create code information
        //Customers should always try to turn off the device with the power button 
        //to prevent the state being in the middle and corrupting the file.

        public void SaveState()
        {
            try
            {
                if (!Directory.Exists(FOLDERPATH_CREDIT_CODE_STATE))   // 0102-35
                    System.IO.Directory.CreateDirectory(FOLDERPATH_CREDIT_CODE_STATE); // 0102-35

                lastStateUpdateTime = DateTime.Now;

                //CreditCodeState.SaveState(this);
                //SaveState(this);
                //CreditCode.cc_Info.SaveState(this);

                SaveState_new1(this);  //0102-31
                //SaveState(this);
            }
            catch (Exception ex)
            {

            }
        }

        public static void SaveState_org(CreditCodeState codeState)
        {
            if (codeState.listSequenceNumbersUsed == null)
            {
                codeState.listSequenceNumbersUsed = new List<string>();
            }
            
            XmlSerializer serializer = new XmlSerializer(typeof(CreditCodeState));
            TextWriter writer = new StreamWriter(FILEPATH_CREDIT_CODE_STATE, false, Encoding.ASCII);  //sww 0102-33  0106-09
            serializer.Serialize(writer, codeState);
            writer.Close();
        }

        public static void SaveState(CreditCodeState codeState)
        {
            if (codeState.listSequenceNumbersUsed == null)
            {
                codeState.listSequenceNumbersUsed = new List<string>();
            }
           
            XmlSerializer serializer = new XmlSerializer(typeof(CreditCodeState));
            TextWriter writer = new StreamWriter(FILEPATH_CREDIT_CODE_STATE, false, Encoding.ASCII);  //sww 0102-33  0106-09
            serializer.Serialize(writer, codeState);
            writer.Close();
        }

        public void SaveState_new1(CreditCodeState codeState)  
        {
            // Ver 00262 0020-03 needed otherwise will have error message here
            if (codeState.listSequenceNumbersUsed == null)
            {
                 codeState.listSequenceNumbersUsed = new List<string>();
            }

            CreditCodeState state;
       
            XmlSerializer serializer = new XmlSerializer(typeof(CreditCodeState));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, codeState);

            if (ms == null)
                return;
         
            // =========================
            // Prevent file corruption 
            // =========================
            var data = "001";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\disk\\tmp.bin", false, Encoding.ASCII))  // sww 0102-33   0106-09
            {
                file.WriteLine(data);
            }

            data = "";
            using (System.IO.StreamReader file = new System.IO.StreamReader("c:\\disk\\tmp.bin", Encoding.ASCII))   // sww 0102-33  0106-09
            {
                data = file.ReadLine();
            }

            if (data != "001") return;
            //=======================
 
            File.Delete("c:\\disk\\state_buf.bin");  //sww 0102-33
            using (FileStream fsFileStream = new FileStream("c:\\disk\\state_buf.bin", FileMode.CreateNew, FileAccess.Write, FileShare.None, 1024, FileOptions.WriteThrough))   //sww 0102-33
            {
                ms.WriteTo(fsFileStream);
                ms.Dispose();
            }
            
            File.Delete( FILEPATH_CREDIT_CODE_STATE); //sww 0102-33
            File.Move("c:\\disk\\state_buf.bin",  FILEPATH_CREDIT_CODE_STATE); //sww 0102-33v
            File.Delete("c:\\disk\\state_buf.bin"); //sww 0102-33
        }
        #endregion

        #region GetUSBLetter
        public static string GetUSBLetter()  // sww 0102-31
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    return drive.Name;
                }
            }
            return "";
        }
        #endregion

        #region Set System Time

        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        private void SetTime()
        {
            SYSTEMTIME systime = new SYSTEMTIME();
            systime.wYear = ushort.Parse(clockYear);
            systime.wMonth = ushort.Parse(clockMonth);
            systime.wDay = ushort.Parse(clockDay);
            systime.wHour = ushort.Parse(clockHour);
            systime.wMinute = ushort.Parse(clockMinute);
            systime.wSecond = 0;
            systime.wMilliseconds = 0;

            try
            {
                DateTime dt = new DateTime(systime.wYear, systime.wMonth, systime.wDay, systime.wHour, systime.wMinute, 0);
                if (DateTime.Compare(dt, lastStateUpdateTime) > 0)
                {
                    SetSystemTime(ref systime);
                }
            }
            catch { }
        }

        #endregion
    }
}
