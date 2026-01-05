using System.Configuration;
using System;
using System.Windows;

namespace Edge.Tower2.UI
{
    public static class Settings
    {
        public static int HotColdPulseLowPercentage { get; private set; }
        public static int HotColdPulseMediumPercentage { get; private set; }
        public static int HotColdPulseHighPercentage { get; private set; }
        public static int VacJetPulseLowPercentage { get; private set; }
        public static int VacJetPulseMediumPercentage { get; private set; }
        public static int VacJetPulseHighPercentage { get; private set; }

        public static int HotColdPulseLowDuration { get; private set; }
        public static int HotColdPulseMediumDuration { get; private set; }
        public static int HotColdPulseHighDuration { get; private set; }
        public static int VacJetPulseLowDuration { get; private set; }
        public static int VacJetPulseMediumDuration { get; private set; }
        public static int VacJetPulseHighDuration { get; private set; }

        public static int HotColdTemperatureWarm { get; private set; }
        public static int HotColdTemperatureWarmer { get; private set; }
        public static int HotColdTemperatureWarmest { get; private set; }

        public static int HotColdTemperatureCool { get; private set; }
        public static int HotColdTemperatureCooler { get; private set; }
        public static int HotColdTemperatureCoolest { get; private set; }

        //****************************

        public static int HydraFacialDefaultVacuum { get; private set; }

        public static int HotColdLowVacuum { get; private set; }
        public static int HotColdMediumVacuum { get; private set; }
        public static int HotColdHighVacuum { get; private set; }

        public static int VacJetLowVacuum { get; private set; }
        public static int VacJetMediumVacuum { get; private set; }
        public static int VacJetHighVacuum { get; private set; }

        public static int BodyLymphLowVacuum { get; private set; }
        public static int BodyLymphMediumVacuum { get; private set; }
        public static int BodyLymphHighVacuum { get; private set; }

        public static int FaceLymphLowVacuum { get; private set; }
        public static int FaceLymphMediumVacuum { get; private set; }
        public static int FaceLymphHighVacuum { get; private set; }

        public static int BodyLymphPulseLowPercentage { get; private set; }
        public static int BodyLymphPulseMediumPercentage { get; private set; }
        public static int BodyLymphPulseHighPercentage { get; private set; }

        public static int BodyLymphPulseLowDuration { get; private set; }
        public static int BodyLymphPulseMediumDuration { get; private set; }
        public static int BodyLymphPulseHighDuration { get; private set; }

        public static int FaceLymphPulseLowPercentage { get; private set; }
        public static int FaceLymphPulseMediumPercentage { get; private set; }
        public static int FaceLymphPulseHighPercentage { get; private set; }

        public static int FaceLymphPulseLowDuration { get; private set; }
        public static int FaceLymphPulseMediumDuration { get; private set; }
        public static int FaceLymphPulseHighDuration { get; private set; }

        public static string CameraDeviceName { get; private set; }

        public static int MaxBottleInsertions { get; private set; }
        public static int MaxRinseawayInsertions { get; private set; }      // 0020-05

        // Add for clean up vacuum control ver 0020-02
        public static int CleanupVacuum_Vortex { get; private set; }
        public static int CleanupVacuum_PulseFusion { get; private set; }

        // Add for clean up control 2014 10/16
        public static int CleanupSeconds_Vortex { get; private set; }
        public static int CleanupSeconds_PulseFusion { get; private set; }
        
        public static string ConfigFileVersion { get; private set; }

        public static int CreditCode_WarningDays { get; private set; }

        public static int LED_One_Light_Minutes { get; private set; }
        public static int LED_Two_Lights_Minutes { get; private set; }
        
        public static int LED_One_Light_Seconds { get; private set; }       // 0101-07
        public static int LED_Two_Lights_Seconds { get; private set; }      // 0101-07

        public static bool DisplayCursorOn { get; private set; }
        private static string displaycurson_string { get; set; }

        public static string Default_Hot_Cold_None { get; private set; }    // 0020-09

        public static string Web_Download { get; private set; }             // 0102-12
        
        public static bool SystemRestart { get; private set; }              // 0102-12
        private static string SystemRestart_string { get; set; }            // 0102-12

        public static string InternetConnection { get; private set; }       // 0102-12
        public static int InternetTimeout { get; private set; }             // 0102-12

        public static string MailServer { get; private set; }               // 0102-27
        public static string MailServerAccount { get; private set; }        // 0102-27
        public static string MailServerPass { get; private set; }           // 0102-27
        public static string MailFrom { get; private set; }                 // 0102-27
        public static string MailTo { get; private set; }                   // 0102-27

        public static string MD_SPA_Mode{ get; private set; }               // 0106-06

        public static string Operation_Mode { get; private set; }           // 0106-06

        public static string DB_Driver { get; private set; }                // 0106-06

        public static int VortexMax_S { get; private set; }                 // 0106-15
        public static int LymphMax_S { get; private set; }                  // 0106-15

        public static int VortexMax_M { get; private set; }                 // 0106-15
        public static int LymphMax_M { get; private set; }                  // 0106-15

        static Settings()
        {
            try
            {

                HotColdPulseLowPercentage = GetParameter("HotColdPulseLowPercentage", 25);
                HotColdPulseMediumPercentage = GetParameter("HotColdPulseMediumPercentage", 25);
                HotColdPulseHighPercentage = GetParameter("HotColdPulseHighPercentage", 25);

                HotColdPulseLowDuration = GetParameter("HotColdPulseLowDuration", 200);
                HotColdPulseMediumDuration = GetParameter("HotColdPulseMediumDuration", 100);
                HotColdPulseHighDuration = GetParameter("HotColdPulseHighDuration", 67);

                //****************************

                VacJetPulseLowPercentage = GetParameter("VacJetPulseLowPercentage", 65);
                VacJetPulseMediumPercentage = GetParameter("VacJetPulseMediumPercentage", 65);
                VacJetPulseHighPercentage = GetParameter("VacJetPulseHighPercentage", 65);

                VacJetPulseLowDuration = GetParameter("VacJetPulseLowDuration", 200);
                VacJetPulseMediumDuration = GetParameter("VacJetPulseMediumDuration", 100);
                VacJetPulseHighDuration = GetParameter("VacJetPulseHighDuration", 67);

                //****************************

                HotColdTemperatureWarm = GetParameter("HotColdTemperatureWarm", 100);
                HotColdTemperatureWarmer = GetParameter("HotColdTemperatureWarmer", 105);
                HotColdTemperatureWarmest = GetParameter("HotColdTemperatureWarmest", 110);

                HotColdTemperatureCool = GetParameter("HotColdTemperatureCool", 50);
                HotColdTemperatureCooler = GetParameter("HotColdTemperatureCooler", 45);
                HotColdTemperatureCoolest = GetParameter("HotColdTemperatureCoolest", 40);

                //****************************

                HydraFacialDefaultVacuum = GetParameter("HydraFacialDefaultVacuum", 18);

                HotColdLowVacuum = GetParameter("HotColdDefaultVacuum", 3);
                HotColdMediumVacuum = GetParameter("HotColdMediumVacuum", 5);
                HotColdHighVacuum = GetParameter("HotColdHighVacuum", 7);

                VacJetLowVacuum = GetParameter("VacJetLowVacuum", 3);
                VacJetMediumVacuum = GetParameter("VacJetMediumVacuum", 3);
                VacJetHighVacuum = GetParameter("VacJetHighVacuum", 3);

                BodyLymphLowVacuum = GetParameter("BodyLymphLowVacuum", 10);
                BodyLymphMediumVacuum = GetParameter("BodyLymphMediumVacuum", 10);
                BodyLymphHighVacuum = GetParameter("BodyLymphHighVacuum", 10);

                FaceLymphLowVacuum = GetParameter("FaceLymphLowVacuum", 3);
                FaceLymphMediumVacuum = GetParameter("FaceLymphMediumVacuum", 3);
                FaceLymphHighVacuum = GetParameter("FaceLymphHighVacuum", 3);

                //****************************

                BodyLymphPulseLowPercentage = GetParameter("BodyLymphPulseLowPercentage", 65);
                BodyLymphPulseMediumPercentage = GetParameter("BodyLymphPulseMediumPercentage", 65);
                BodyLymphPulseHighPercentage = GetParameter("BodyLymphPulseHighPercentage", 65);

                BodyLymphPulseLowDuration = GetParameter("BodyLymphPulseLowDuration", 200);
                BodyLymphPulseMediumDuration = GetParameter("BodyLymphPulseMediumDuration", 100);
                BodyLymphPulseHighDuration = GetParameter("BodyLymphPulseHighDuration", 67);

                //****************************

                FaceLymphPulseLowPercentage = GetParameter("FaceLymphPulseLowPercentage", 65);
                FaceLymphPulseMediumPercentage = GetParameter("FaceLymphPulseMediumPercentage", 65);
                FaceLymphPulseHighPercentage = GetParameter("FaceLymphPulseHighPercentage", 65);

                FaceLymphPulseLowDuration = GetParameter("FaceLymphPulseLowDuration", 200);
                FaceLymphPulseMediumDuration = GetParameter("FaceLymphPulseMediumDuration", 100);
                FaceLymphPulseHighDuration = GetParameter("FaceLymphPulseHighDuration", 67);

                // default is USB2760, but will be overwriten by config file.
                // CameraDeviceName = GetParameter("CameraDeviceName", "USB 2760 Camera");
                CameraDeviceName = GetParameter("CameraDeviceName", "Logitech HD Pro Webcam C920");

                MaxBottleInsertions = GetParameter("MaxBottleInsertions", 5);
                MaxRinseawayInsertions = GetParameter("MaxRinseawayInsertions", 5);

                CleanupVacuum_Vortex = GetParameter("CleanupVacuum_Vortex", 50);                // 0020-02
                CleanupVacuum_PulseFusion = GetParameter("CleanupVacuum_PulseFusion", 50);      // 0020-02

                // New layout for clean up process 2014 10/16
                CleanupSeconds_Vortex = GetParameter("CleanupSeconds_Vortex", 100);
                CleanupSeconds_PulseFusion = GetParameter("CleanupSeconds_PulseFusion", 15);
                ConfigFileVersion = GetParameter("ConfigFileVersion", "0.00");
                CreditCode_WarningDays = GetParameter("CreditCode_WarningDays", 5);

                LED_One_Light_Minutes = GetParameter("LED_One_Light_Minutes", 4);
                LED_Two_Lights_Minutes = GetParameter("LED_Two_Lights_Minutes", 4);

                LED_One_Light_Seconds = GetParameter("LED_One_Light_Seconds", 0);               // 0101-07
                LED_Two_Lights_Seconds = GetParameter("LED_Two_Lights_Seconds", 0);             // 0101-07

                displaycurson_string = ConfigurationManager.AppSettings["DisplayCursorOn"];
                DisplayCursorOn = displaycurson_string != null && Boolean.Parse(displaycurson_string);

                Default_Hot_Cold_None = ConfigurationManager.AppSettings["Default_Hot_Cold_None"]; // 0020-09

                Web_Download = ConfigurationManager.AppSettings["Web_Download"];                // 0102-12

                SystemRestart_string = ConfigurationManager.AppSettings["SystemRestart"];       // 0102-12
                SystemRestart = SystemRestart_string != null && Boolean.Parse(SystemRestart_string); // 0102-12

                InternetConnection = ConfigurationManager.AppSettings["InternetConnection"];    // 0102-23
                InternetTimeout = GetParameter("InternetTimeout", 300);                         // 0102-23

                MailServer = ConfigurationManager.AppSettings["MailServer"];                    // 0102-27
                MailServerAccount = ConfigurationManager.AppSettings["MailServerAccount"];      // 0102-27
                MailServerPass = ConfigurationManager.AppSettings["MailServerPass"];            // 0102-27
                MailFrom = ConfigurationManager.AppSettings["MailFrom"];                        // 0102-27
                MailTo = ConfigurationManager.AppSettings["MailTo"];                            // 0102-27

                MD_SPA_Mode = GetParameter("MD_SPA", "MD");                                     // 0106-06

                Operation_Mode = GetParameter("Operation", "operation");                        // 0106-06

                DB_Driver = ConfigurationManager.AppSettings["DB_Driver"];

                VortexMax_S = CheckMaxvalue(GetParameter("VortexMax_S", 35), 50);               // 0106-15
                LymphMax_S = CheckMaxvalue(GetParameter("LymphMAX_S", 16), 50);                 // 0106-15

                VortexMax_M = CheckMaxvalue(GetParameter("VortexMax_M", 50), 50);               // 0106-15
                LymphMax_M = CheckMaxvalue(GetParameter("LymphMAX_M", 16), 50);                 // 0106-15
            }
            catch (Exception ex)                                                                // 0106-18
            {
                Utility.Lib.SaveErrorLog(ex.ToString());                                        // 0106-18
                MessageBox.Show("Unable to read config file!");                                 // 0106-18
            }
        }   

        private static int GetParameter(string name, int defaultValue)
        {
            var parameterString = ConfigurationManager.AppSettings[name];
            return parameterString == null ? defaultValue : int.Parse(parameterString);
        }

        private static string GetParameter(string name, string defaultValue)
        {
            var parameterString = ConfigurationManager.AppSettings[name];
            return parameterString ?? defaultValue;
        }

        private static int CheckMaxvalue(int originalvalue, int maxvalue)                       // 0106-15
        {
            return originalvalue <= maxvalue ? originalvalue : maxvalue;
        }
    }

    public static class Layout_Params
    {
        public static string DefaultColor { get; private set; }  

        static Layout_Params()
        {
            DefaultColor = ConfigurationManager.AppSettings["DefaultColor"]; 
        }

        private static string GetParameter(string name, string defaultValue)
        {
            var parameterString = ConfigurationManager.AppSettings[name];
            return parameterString ?? defaultValue;
        }
    }
}