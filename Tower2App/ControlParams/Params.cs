
namespace Edge.Tower2.UI.ControlParams
{
    public class Params
    {
        public static e_Content ContentMode { get; set; }
        public static e_DB PhotoCapture_State { get; set; }
        public static e_Session PhotoCapture_Session { get; set; }
        public static e_Video CurrentVideoMode { get; set; }

        public static string p_SessionName{ get; set; }
        public static bool p_NewCustomer{ get; set; }

        public static bool p_SystemLoading { get; set; }                                        // for system IO loading
        public static bool p_SystemStartUp { get; set; }                                        // For bottle insertion control

        public static string p_Date_Org { get; set; }
        public static string p_Date_V1 { get; set; }
        public static string p_Date_V2 { get; set; }
        public static string p_SelectedGroup{ get; set; }                                       // 0103-06

        public static string p_LastVisit { get; set; }
        public static string p_NCaptureDate { get; set; }

        public static string p_Org_Visit { get; set; }
        public static string p_Mid_Visit { get; set; }
        public static string p_Last_Visit { get; set; }

        public static bool p_PhotoCaptureSelected { get; set; }                                 // to keep info from search page or photo capture over all (info) page 

        public static string p_LoginUser { get; set; }
        public static string p_LoginRole{ get; set; }

        public static int p_control_mode { get; set; }                                          // 002639-08-05 Vortexfusion, PulseFusion, LymphaticBody ....

        public static bool p_hydrafacialLoaded { get; set; }                                    // for Cleaning Process, to avoid clean up message on the first time load
        public static int p_last_QS_Mode { get; set; }                                          // last operate quick start mode  002639-08-05

        public static int p_CleanupMode { get; set; }                                           // keep clean up param from script file
        public static int p_RinseawayStation { get; set; }

        public static double p_AudioVolume { get; set; }

        public static string p_last_PulseSelect { get; set; }

        private static bool[] _valid_insertedbottle = new bool[20];
        public static bool[] valid_insertedbottle
        {
            get { return _valid_insertedbottle; }
            set { _valid_insertedbottle = value; }
        }

        public static string p_HotColdSelected { get; set; }
        public static string p_LastHotTempSelected { get; set; }
        public static string p_LastColdTempSelected { get; set; }

        // 002639-08
        public static bool p_Splash_On { get; set; }

        public static bool p_ProtoMode { get; set; }                                            // control Right side panel, because "portocol" and "quick start" are different

        public static int p_ProtoSelect { get; set; }                                           // 0020-12
      
        // 002639-01
        public static int p_TreatmentSteps { get; set; }                                        // keep total treatment steps from script file
        public static int p_ProtoOpt { get; set; }
 
        public static bool p_BottleCountOn { get; set; }                                        // 002639-08

        public static string p_SelectDir { get; set; }                                          // 0020-12

        public static int[,] IntegrateMode = new int[10, 15];                                   // for basic.txt, deluxe.txt //  0020-12

        // Example
        //8, 3, 1, 1, 1, 4, 1, 1, 7, 0 ..              // 8 is total number of mode, 3 is mode 3 , 1 is mode 1...      ( 1 is voxtex, 2 pulse fusion, 3 hot/cold, 4 LED, 5 Facial, 6 Body )
        //0, 0, 0, 0, 0, 1, 0, 0, 0, 0 ..              // require clean up set to 1
        //0, 0,16,16,22, 0,16,16, 0, 0 ..              // pump value
        //0, 2, 0, 0, 0, 1, 0, 0, 0, 0 ..              // Pulse select  1 is low, 2 is Mid, 3 is high
        //0, 1, 0, 0, 0, 0, 0, 0, 0, 0 ..              // 1 is hot, 2 is cold
        //0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ..              // Reserved, Not in use

        public static string[,] IntegrateKey = new string[10, 15];                              // for file name on different mode // 0020-12

        public static string[,] p_Mainfolder = new string[15, 3];                               // 0020-12 for protocol main folder, 0106-06, 0106-17
 
        public static string[,] p_Subfolder = new string[11, 15];                               // 0020-12 0106-17 for protocol sub folder

        // 002639-08-03
        public static bool p_dongle_Presented { get; set; }                                     // If dongle is in and valid
        public static string p_dongle_ID { get; set; }
        public static string p_dongle_SN { get; set; }
        public static string p_dongle_Expired { get; set; }
        public static string p_dongle_Created { get; set; }

        public static bool p_DemoMode { get; set; }                                             // 0101-08 If the DemoMode is set on the settings page

        public static string p_DemoCodeStart { get; set; }                                      // 0020-12

        public static bool p_Downloading { get; set; }                                          // 0102-26

        public static string Photos_Default { get; set; }                                       // 0102-38

        public static string Photos_Backup { get; set; }                                        // 0102-38

        public static bool p_CleanupRequired { get; set; }                                      // 0103-10

        public static bool p_InCleaningPage { get; set; }                                       // 0103-10 in 5 steps of cleaning group, not need to clean up 
                                                                                                //         popup window, no power off between each steps

        public static string p_SecondLanguage { get; set; }                                     // 0106-09

        public static int p_MainfolderMax { get; set; }                                         // 0106-17  range start from 1 

        public static int p_Protopage_selected { get; set; }                                    // 0106-18
    }

    /// <summary>
    /// In the photo capture session, it needs to have notes editing to keep state for each session 
    /// such as profile, photoL, photoR.... 
    /// </summary>
    public enum e_Session
    {
        Search,
        Create,
        Edit,
        Profile,
        PhotoL,
        PhotoR,
        CloseUp1,
        CloseUp2,
        CloseUp3
    }

    public enum e_DB
    {
        Search,
        Create,
        InfoSave,
        InfoEdit,
    }

    public enum e_Video
    {
        Promo_Training,
        dirSelect,
        fileSelect,
        playVideo,
        fileButtons
    }

    public enum e_Content
    {
        FullVideo,
        FullDoc,
        Split
    }

    public class DBInfo
    {
        public static string ID { get; set; }
        public static string Name { get; set; }
        public static string Date_Created { get; set; }
        public static string Birthday { get; set; }
        public static string TelNo { get; set; }

        public static string PhotoL { get; set; }
        public static string PhotoR { get; set; }
        public static string Profile { get; set; }
        public static string PhotoDescL { get; set; }
        public static string PhotoDescC { get; set; }
        public static string PhotoDescR { get; set; }
        
        public static string CloseUp1 { get; set; }
        public static string CloseUp2 { get; set; }
        public static string CloseUp3 { get; set; }
        public static string CloseUpDesc1 { get; set; }
        public static string CloseUpDesc2 { get; set; }
        public static string CloseUpDesc3 { get; set; }
       
        public static string Operator { get; set; }
        public static string PhoneType { get; set; }
       
    }

    public static class e_InfoState
    {
        public const string Profile = "1";
        public const string Left = "2";
        public const string Right = "3";
        public const string CloseUp1 = "4";
        public const string CloseUp2 = "5";
        public const string CloseUp3 = "6";
    }

    // 002639-08
    public static class e_Mode
    {
        public const int None = 0;
        public const int VortexFusion = 1;
        public const int PulseFusion = 2;
        public const int ThermalTherapy = 3;
        public const int LightTherapy = 4;
        public const int LymphaticFacial = 5;
        public const int LymphaticBody = 6;
        public const int Purchase = 7;
        public const int To_Home_Page = 8;
        public const int To_Proto_Page = 9;
        public const int UsageReport = 10;
        public const int Printing = 11;
        public const int Camera = 12;
        public const int LightBodyTherapy = 13;  // 0100
        public const int Settings = 14;  // 0102-08
        public const int To_Modalities_Page = 15;  // 0106-06

    }

    public static class e_Proto
    {
        public const int Option = 1;
        public const int PlatinumOpt = 2;
    }

}
