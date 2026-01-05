using System;
using System.Windows;
using JetBrains.Annotations;
using System.IO;

using System.Text;   // 0106-09

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfProto.xaml
    /// </summary>
    public partial class wpfModalities: Window
    {
        public int l_Submenu { get; set; }                                                      // 0020-12
        public int l_MainButton { get; set; }                                                   // 0020-12
        public int l_SelectPage { get; set; }                                                   // 0020-12

        public bool l_isBusy { get; set; }                                                      // 0102-36

        #region Init
        public wpfModalities()
        {
            InitializeComponent();

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            l_Submenu = 0;                                                                      // 0020-12
            l_MainButton = 0;                                                                   // 0020-12
            
            // 0102-39
            Utility.Lib.LoadImageNoLock(imgModalities, @"/Skin/images/" + ControlParams.Params.p_SecondLanguage+ @"/"+ App.getTextMessages("modalities_main_menu_noled") + ".png"); // 0106-06  0106-13

            Utility.Lib.LoadImageNoLock(imgPreClean,  @"/Skin/images/"+ControlParams.Params.p_SecondLanguage + @"/SignaturePreClean.png");   // 0106-13

            cvsPreClean.Visibility = Visibility.Hidden;

            l_SelectPage = 0;
        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void OnEnter()
        {
           
        }

        private void OnLeave()
        {
          
        }
 
        public void setMenuPage(Menu mode)                                                      // 0102-38
        {
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Modalities_Page;      // 0106-06
            ControlParams.Params.p_hydrafacialLoaded = false;

            if (mode == Menu.Main)                                                              // Main is default page , second is for sub menu 1
            {
                Utility.Lib.LoadImageNoLock(imgModalities, "\\Skin\\images\\" + ControlParams.Params.p_SecondLanguage + "\\" + ControlParams.Params.p_Mainfolder[14, 0] + ".png"); // 00250  00115a1-0001

                imgModalities.Visibility = Visibility.Visible;
                imgSubMenu1.Visibility = Visibility.Hidden;

                l_SelectPage = 0;                                                               // 0102-39
            }
            else if (mode == Menu.Second)
            {
                imgModalities.Visibility = Visibility.Hidden;
                imgSubMenu1.Visibility = Visibility.Visible;
           }
        }

        private int stateID { set; get; }
        public void setDefaultSubMenu(int id)                                                   // 0102-38 keep for T2 use
        {
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page;
            ControlParams.Params.p_hydrafacialLoaded = false;

            l_SelectPage =0;
           
            setMenuPage(Menu.Main);
            FindTheState(id); 
        }
        #endregion

        #region Load files
        public void LoadOperationMode(string filename)                                          // 0102-39
        {
            LoadOperationKey(filename);                                                         // 0020-12

            if (!File.Exists(Environment.CurrentDirectory + "\\"+Settings.Operation_Mode+"\\" + ControlParams.Params.p_SelectDir  +filename))  // 0106-07
            {
                MessageBox.Show(filename +" Operation file not found!");                        // 0102-36

                return;
            }

            string line, line1;
            using (var sr = new StreamReader(Environment.CurrentDirectory + "\\" + Settings.Operation_Mode + "\\" + ControlParams.Params.p_SelectDir + filename, Encoding.ASCII))  // 0106-09
            {
                int iCountLine = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        break;
                   
                    if (line.IndexOf(";") > 0)
                        line1 = line.Substring(0, line.IndexOf(";"));                           // 0102-36
                    else
                        line1 = line;

                    var answers = line1.Split(',');

                    for (int iCountAnswer = 0; iCountAnswer < 15; iCountAnswer++)
                    {
                        ControlParams.Params.IntegrateMode[iCountLine, iCountAnswer] = int.Parse(answers[iCountAnswer]);
                    }

                    iCountLine++;
                }
            }
        }

        private void LoadOperationKey(string filename)                                          // 0020-12
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\"+Settings.Operation_Mode+"\\"+ ControlParams.Params.p_SelectDir +"Key\\" + filename))  // 0106-07
            {
                MessageBox.Show(filename+" Operation key file not found!");                     // 0102-36
       
                return;
            }

            string line;
            using (var sr = new StreamReader(Environment.CurrentDirectory + "\\" + Settings.Operation_Mode + "\\" + ControlParams.Params.p_SelectDir + "key\\" + filename, Encoding.ASCII))  // 0106-07  0106-09
            {
                int iCountLine = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;

                    string str,str1;
                    if (line.IndexOf(";") > 0)
                        str1 = line.Substring(0,line.IndexOf(";") );
                    else
                        str1 = line;

                    str1 = str1.Replace("\t", "");
                    str = str1.Replace(" ", "");

                    var answers = str.Split(',');
                    for (int iCountAnswer = 0; iCountAnswer < 15; iCountAnswer++)
                    {
                        ControlParams.Params.IntegrateKey[iCountLine, iCountAnswer] = answers[iCountAnswer];
                    }

                    iCountLine++;
                }
            }
        }
        #endregion

        #region 9 button Clicks
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(1);                                                                    // 0020-12
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(2);                                                                    // 0020-12
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(3);                                                                    // 0020-12
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(4);                                                                    // 0020-12
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(5);                                                                    // 0020-12
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(6);                                                                    // 0020-12
        }
        
        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(7);                                                                    // 0020-12
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(8);                                                                    // 0020-12
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            FindTheState(9);                                                                    // 0020-12
        }
        #endregion

        #region 9 button call function
        public void FindTheState(int id)                                                        // 0102-36
        {
            if (l_isBusy) return;
 
            l_isBusy = true;

            if (l_SelectPage != 0) 
            {
                // go to sub menu
                l_Submenu = id;
                SelectSubmenuMode(id);                                                        // 6 is total modalities

                l_isBusy = false;
                return;
            }
            else 
            {
                l_MainButton = id;
                l_SelectPage = 1;
            }

            ControlParams.Params.p_ProtoSelect= id;

            if (ControlParams.Params.p_Mainfolder[id,0] == "0")                                 // 0106-01   , if it is 0, no where to go
            {
                l_isBusy = false;
                l_SelectPage = 0;
                return;
            }

            if (ControlParams.Params.p_Subfolder[id, 10] != "0")                                // 0106-06       0106-17 , if not 0, it should have a subfolder
                Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/images/" + ControlParams.Params.p_SecondLanguage + "/" + ControlParams.Params.p_Subfolder[id, 10] + ".png");  // 0106-13   0107   0106-17
            else
            {
                LoadOperationMode(ControlParams.Params.p_Mainfolder[id, 0] + ".txt");
                ProtoPage_init();
            }
 
            // ========================================================

            // For T2
            //if (id == 1)
            //{
            //    Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/images/submenu_9.png"); // 0020-11
            //}
            //else if (id == 2)  // Hot
            //{
            //    Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/images/submenu_7.png"); // 0020-11
            //}
            //else if (id == 3)  // Cold
            //{
            //    Utility.Lib.LoadImageFromAppDir(imgSubMenu1, "/Skin/images/submenu_8.png"); // 0020-11
            //}

            // ========================================================

            imgSubMenu1.Visibility = Visibility.Visible;
            imgModalities.Visibility = Visibility.Hidden;

            l_isBusy = false; // 0102-36
        }
        #endregion
 
        #region submenu call function
        private void SelectSubmenuMode(int id)
        {
            if (Int32.Parse(ControlParams.Params.p_Subfolder[l_MainButton, 0]) >= id)           // Check if id is available, and should not over total id
            {
                LoadOperationMode(ControlParams.Params.p_Mainfolder[l_MainButton,0] + "_" + ControlParams.Params.p_Subfolder[l_MainButton, id] + ".txt");
                ProtoPage_init();
            }
         }
        #endregion

        #region Others
       private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.HydraFacial);                                                           // 0102-38

            cvsRoot.Visibility = Visibility.Visible;
            cvsPreClean.Visibility = Visibility.Hidden;

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).protoMode_init(false);
        }

        private void ProtoPage_init()
        {
            ControlParams.Params.p_ProtoOpt = 0;                                                // read first array of InttegrateMode[]

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).l_Signature = 1;
            ControlParams.Params.p_TreatmentSteps = ControlParams.Params.IntegrateMode[0, 0];
            ControlParams.Params.p_control_mode = ControlParams.Params.IntegrateMode[0, 1];

            btnContinue_Click(null, null);                                                      // will go to hydrafacial window
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            ControlParams.Params.p_Protopage_selected = 0;                                      // 0106-18

            NavBar.setVolume(ControlParams.Params.p_AudioVolume);                               // 0101-03
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;                                    // 0101-03
        }

        private void Me_ContentRendered(object sender, EventArgs e)
        {

        }
        #endregion

        private string LoadModalitiesMenu()                                                     // 0106-06
        {
            try
            {
                string line;
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\operation\\modalities_main_menu.txt", Encoding.ASCII))  // 0102-33  0106-09
                {
                    var i = 0;
                    if ((line = file.ReadLine()) == null)
                    {
                        line = "";
                    }
                }

                return line;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read modalities main menu file failed!");
                return "";
            }
        }
    }
}
