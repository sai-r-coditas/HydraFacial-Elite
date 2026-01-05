using System;
using System.Windows;
using JetBrains.Annotations;
using System.IO;

using System.Text;  // 0106-09

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfProto.xaml
    /// </summary>
    public partial class wpfProto : Window
    {
        public int l_Submenu { get; set; }                                                      // 0020-12
        public int l_MainButton { get; set; }                                                   // 0020-12
        public int l_ProtoPage { get; set; }                                                    // 0106-17

        #region Loading
        public wpfProto()
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
            l_ProtoPage = 0;                                                                    // 0106-17
  
            Utility.Lib.LoadImageNoLock(imgMenu1,  "\\Skin\\images\\t2\\protocol_signature_options.png");                                   // 0106-16                          
            
            Utility.Lib.LoadImageNoLock(imgMenu2,  "\\Skin\\images\\t2\\protocol-signature-platinum-opt.png");

            Utility.Lib.LoadImageNoLock(imgPreClean, "\\Skin\\images\\" +ControlParams.Params.p_SecondLanguage+"\\SignaturePreClean.png");  // 0106-13

            Utility.Lib.LoadImageNoLock(imgTopMenu, "\\Skin\\Images\\" + App.getTextMessages("top_menu_bg_dim") + ".png");                  // 0103-05  0106-06

            Utility.Lib.LoadImageNoLock(imgPrevious, "\\Skin\\Images\\arrow_l.png");            // 0106-17

            Utility.Lib.LoadImageNoLock(imgNext, "\\Skin\\Images\\arrow_r.png");                // 0106-17

            cvsPreClean.Visibility = Visibility.Hidden;
        }

        private void OnEnter()
        {
            // Ver 002633
            ControlParams.Params.p_control_mode = ControlParams.e_Mode.To_Proto_Page; // "Proto";
            ControlParams.Params.p_hydrafacialLoaded = false;
            setMenuPage(ControlParams.e_Proto.Option);
        }

        private void OnLeave()
        {
          
        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }
        #endregion

        #region Init
        private void ProtoPage_init()
        {
            cvsRoot.Visibility = Visibility.Hidden;
            cvsMenu2.Visibility = Visibility.Hidden;
            cvsPreClean.Visibility = Visibility.Visible;

            ControlParams.Params.p_ProtoOpt = 0;
            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).l_Signature = 1;
            ControlParams.Params.p_TreatmentSteps = ControlParams.Params.IntegrateMode[0, 0];
            ControlParams.Params.p_control_mode = ControlParams.Params.IntegrateMode[0, 1];
        
        }

        public void setMenuPage(int mode)
        {
            if (mode == 1)                                                                      // Mode 1 is default proto page , mode 2 is for sub menu 1
            {
                l_ProtoPage = 0;                                                                // 0106-17

                Utility.Lib.LoadImageNoLock(imgProto, "\\Skin\\images\\" + ControlParams.Params.p_SecondLanguage + "\\"+ ControlParams.Params.p_Mainfolder[14, l_ProtoPage]+".png"); // 0106-13  0106-17

                cvsMenu2.Visibility = Visibility.Hidden;                                        // 0020-10
                cvsProto.Visibility = Visibility.Visible;
                cvsMenu1.Visibility = Visibility.Hidden;                                        // disable 3 option for platinum

                if (ControlParams.Params.p_MainfolderMax <= 1)                                  // 0106-18
                {
                    imgNext.Visibility = Visibility.Hidden;
                    imgPrevious.Visibility = Visibility.Hidden;
                }
                else
                {
                    imgNext.Visibility = Visibility.Visible;
                    imgPrevious.Visibility = Visibility.Hidden;
                }
            }
            else if (mode == 2)
            {
                cvsProto.Visibility = Visibility.Hidden;
                cvsMenu1.Visibility = Visibility.Visible;                                       // show 3 option of sub menu
            }
        }
        #endregion

        #region Button Control
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.HydraFacial);                                                           // 0102-38

            cvsRoot.Visibility = Visibility.Visible;
            cvsPreClean.Visibility = Visibility.Hidden;

            ((HydraFacial)App._mainWindows[Mode.HydraFacial]).protoMode_init(false);            // 002639-05
        }
        #endregion

        #region Load files
        private void LoadOperationMode(string filename)
        {
            LoadOperationKey(filename); // 0020-12

            if (!File.Exists(Environment.CurrentDirectory + "\\"+Settings.Operation_Mode+"\\" + ControlParams.Params.p_SelectDir + filename)) // 0106-07
            {
                MessageBox.Show(filename +" Operation file not found!");
                return;
            }

            string line, line1;
            using (var sr = new StreamReader(Environment.CurrentDirectory + "\\" + Settings.Operation_Mode + "\\" + ControlParams.Params.p_SelectDir + filename, Encoding.ASCII))  // 0106-07
            {
                int iCountLine = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        break;
                   
                    if (line.IndexOf(";") > 0)
                        line1 = line.Substring(0, line.IndexOf(";"));
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

        private void LoadOperationKey(string filename)  // 0020-12
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\"+Settings.Operation_Mode+"\\" + ControlParams.Params.p_SelectDir + "Key\\" + filename))  // 0106-06
            {
                MessageBox.Show(filename+" Operation key file not found!");
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
        // at all protocol selection page
        private void btnSignature_Click(object sender, RoutedEventArgs e)
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
        private void FindTheState(int id)
        {
            l_MainButton = id;                                                                  // 0020-12
            ControlParams.Params.p_ProtoSelect = id;

            if (ControlParams.Params.p_Subfolder[l_MainButton, 0] == "0")                       // 0102-38  0106-17 , first row is reserved should be 0
            {
                if (ControlParams.Params.p_Mainfolder[id, l_ProtoPage] != "0")                  // 0106-17 check if it is valid
                {
                    LoadOperationMode(ControlParams.Params.p_Mainfolder[id, l_ProtoPage] + ".txt"); // 0106-17
                    ProtoPage_init();
                }
            }
         }
        #endregion

        private void Me_Activated(object sender, EventArgs e)
        {
            ControlParams.Params.p_Protopage_selected = 0;                                      // 0106-18

            NavBar.setVolume(ControlParams.Params.p_AudioVolume);                               // 0101-03
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;                                    // 0101-03
        }

        private void imgNext_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)       // 0106-17
        {
            if (l_ProtoPage + 1 < ControlParams.Params.p_MainfolderMax)
            {
                l_ProtoPage += 1;
                Utility.Lib.LoadImageNoLock(imgProto, "\\Skin\\images\\" + ControlParams.Params.p_SecondLanguage + "\\" + ControlParams.Params.p_Mainfolder[14, l_ProtoPage] + ".png"); // 0106-13  0106-17
            }
            else
            {
                l_ProtoPage = ControlParams.Params.p_MainfolderMax - 1;                         // l_ProtoPage =>> start from 0 to p_MainfolderMax -1
            }

            imgNext.Visibility = (l_ProtoPage == ControlParams.Params.p_MainfolderMax - 1) ? Visibility.Hidden : Visibility.Visible;
            imgPrevious.Visibility = (l_ProtoPage == 0) ? Visibility.Hidden : Visibility.Visible;

            ControlParams.Params.p_Protopage_selected  = l_ProtoPage;                           // 0106-18
        }

        private void imgPrevious_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)   // 0106-17
        {
            if (l_ProtoPage - 1 >= 0)
            {
                l_ProtoPage -= 1;
                Utility.Lib.LoadImageNoLock(imgProto, "\\Skin\\images\\" + ControlParams.Params.p_SecondLanguage + "\\" + ControlParams.Params.p_Mainfolder[14, l_ProtoPage] + ".png"); // 0106-13  0106-17
            }
            else
            {
                l_ProtoPage =  0;
            }

            imgNext.Visibility = (l_ProtoPage == ControlParams.Params.p_MainfolderMax - 1) ? Visibility.Hidden : Visibility.Visible;
            imgPrevious.Visibility = (l_ProtoPage == 0) ? Visibility.Hidden : Visibility.Visible;

            ControlParams.Params.p_Protopage_selected = l_ProtoPage;                            // 0106-18
        }

        /// <summary>
        /// Visibility control --works but not in use
        /// </summary>
        /// <param name="currentpage"></param>
        private void NextPrevControl(int currentpage)                                           // 0106-17
        {
            if (ControlParams.Params.p_MainfolderMax == 1)
            {
                imgPrevious.Visibility = Visibility.Hidden;
                imgNext.Visibility = Visibility.Hidden;
            }
            else if (currentpage == 0)
            {
                imgPrevious.Visibility = Visibility.Hidden;
                imgNext.Visibility = Visibility.Visible;
            }
            else if (currentpage == ControlParams.Params.p_MainfolderMax - 1)
            {
                imgPrevious.Visibility = Visibility.Visible;
                imgNext.Visibility = Visibility.Hidden;
            }
            else
            {
                imgPrevious.Visibility = Visibility.Visible;
                imgNext.Visibility = Visibility.Visible;
            }
        }
     
    }
}
