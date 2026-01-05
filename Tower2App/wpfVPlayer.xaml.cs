using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;

// For DispatcherPriority
using System.Windows.Threading;

// Add for config file
using System.Configuration;

using JetBrains.Annotations;

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for VPlayer.xaml
    /// </summary>
    public partial class VPlayer : Window
    {
        //- For Scroll Bar
        ObservableCollection<EquipmentItem> m_selectedEquipmentHorizontal = new ObservableCollection<EquipmentItem>();
        ObservableCollection<EquipmentItem> m_selectedEquipmentVertical = new ObservableCollection<EquipmentItem>();

        public string CurrentFolder;
        public string CurrentSubFolder;
        private string Video_Extension;

        #region Loading
        public VPlayer()
        {
            InitializeComponent();

            dispatchertimer.Tick += new EventHandler(Timer_Tick);                               // 0101-03

            //- Check Directory Existing  0106-15
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Videos\\" + ControlParams.Params.p_SecondLanguage + "\\Promo\\") ||
                !Directory.Exists(Environment.CurrentDirectory + "\\Videos\\" + ControlParams.Params.p_SecondLanguage + "\\Training\\"))
            {
                System.Windows.MessageBox.Show("Warming! Video folders are not exist!");
                return;
            }

            imgPlayStop.Tag = "";                                                               // 0101-03

            Video_Extension = ConfigurationManager.AppSettings["Video_Extension_Name"];         // 0102-37

            CreateButton();                                                                     // 0102-37

            currentpage = 0;                                                                    // 0102-38

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\WindowBackground_v3.png");      // 0102-39

            Utility.Lib.LoadImageNoLock(imgPrevious, "\\Skin\\Images\\arrow_l.png");            // 0106-03

            Utility.Lib.LoadImageNoLock(imgNext, "\\Skin\\Images\\arrow_r.png");                // 0106-03

            Utility.Lib.LoadImageNoLock(imgReplay, "\\Skin\\Images\\Video_restart.png");        // 0106-05
          
            Utility.Lib.LoadImageNoLock(imgPlayStop, "\\Skin\\Images\\n_Play.png");             // 0106-05

            tbkBack.Text = App.getTextMessages("Back");                                         // 0103-09

            tbk_Promo.Text = App.getTextMessages("Promo");                                      // 0106-01

            tbk_Training.Text = App.getTextMessages("Training");                                // 0106-01

            tbkNext.Text = App.getTextMessages("Next");

            tbkPrevious.Text = App.getTextMessages("Previous");
         }

        int count;
        Button[] btn = new Button[10];
        private void CreateButton()
        {
            btn1.Visibility = Visibility.Hidden;
            btn2.Visibility = Visibility.Hidden;
            btn3.Visibility = Visibility.Hidden;
            btn4.Visibility = Visibility.Hidden;
            btn5.Visibility = Visibility.Hidden;
            btn6.Visibility = Visibility.Hidden;
            btn7.Visibility = Visibility.Hidden;
            btn8.Visibility = Visibility.Hidden;
            btn9.Visibility = Visibility.Hidden;

            for (count = 0; count < 9; count++)
            {
                Button btnn = new Button();

                btnn.Width = btn1.Width;
                btnn.Height = btn1.Height;

                if (count == 0)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn1));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn1));
                }
                else if (count == 1)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn2));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn2));
                }
                else if (count == 2)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn3));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn3));
                }
                else if (count == 3)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn4));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn4));
                }
                else if (count == 4)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn5));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn5));
                }
                else if (count == 5)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn6));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn6));
                }
                else if (count == 6)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn7));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn7));
                }
                else if (count == 7)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn8));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn8));
                }
                else if (count == 8)
                {
                    Canvas.SetLeft(btnn, Canvas.GetLeft(btn9));
                    Canvas.SetTop(btnn, Canvas.GetTop(btn9));
                }

                btnn.Name = "aa" + count.ToString();

                btn[count] = btnn;
                btn[count].FontSize = 24;
               
                btn[count].Content = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap
                };
      
                btn[count].Style = (Style)FindResource("st_MenuButton");

                btn[count].Click += new System.Windows.RoutedEventHandler(ClickHandler);
                this.cvsButtons.Children.Add(btn[count]);
           
            }
        }
        #endregion

        #region Init
        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        private void Page_Init()                                                                // 0102-37
        {
            btn_Back.Visibility = Visibility.Collapsed;
            pg_Video.Visibility = Visibility.Collapsed;
            pg_Selection.Visibility = Visibility.Collapsed;
            pg_Promo_Training.Visibility = Visibility.Collapsed;
            cvsButtons.Visibility = Visibility.Collapsed;

            Page_Display("Promo_Training", false);                                              // 0102-37

            currentpage = 0;                                                                    // 0103-03

            ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.Promo_Training;
            pSelected_Folder = "";
            pSelected_SubFolder = "";
          
        }
        #endregion

        #region For Scroll Bar ======================================================
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ObservableCollection<EquipmentItem> equipmentList1 = new ObservableCollection<EquipmentItem>();
            this.horizontalListBox.ItemsSource = equipmentList1;
            
            //- Set Default Folder to Training
            CreateEquipments_Folders(equipmentList1, "Training");
           
        }
    
        string pSelected_Folder;
        string pSelected_SubFolder;
        private ObservableCollection<EquipmentItem> CreateEquipments_Folders(ObservableCollection<EquipmentItem> equipmentList, string newFolder)
        {
            string VideoDirectory = Environment.CurrentDirectory + "\\Videos\\" + ControlParams.Params.p_SecondLanguage + "\\"+ newFolder +"\\";        // 0106-15
            foreach (string folder in System.IO.Directory.GetDirectories(VideoDirectory))
            {
                equipmentList.Add(new EquipmentItem() { Name = System.IO.Path.GetFileName(folder) });
            }
            return equipmentList;
        }

        private ObservableCollection<EquipmentItem> CreateEquipments_Files(ObservableCollection<EquipmentItem> equipmentList, string newDir)
        {
            //- For directory
            string VideoDirectory = "";
            if (newDir != "")
            {
                VideoDirectory = Environment.CurrentDirectory + "\\Videos\\" + ControlParams.Params.p_SecondLanguage + "\\" + newDir + "\\";   // 0106-15
            }
            
            // T2
            //foreach (string s in Directory.GetDirectories(VideoDirectory))
            //{
            //    //System.Windows.MessageBox.Show(s.Remove(0, VideoDirectory.Length));
            //    equipmentList.Add(new EquipmentItem() { Name = s.Remove(0, VideoDirectory.Length) });
            //}
           
            string s1;
            foreach (string s in System.IO.Directory.GetFiles(VideoDirectory))
            {
                s1 = s.Remove(0, VideoDirectory.Length);
                s1 = s1.Substring(0, s1.Length - 4);
                equipmentList.Add(new EquipmentItem() { Name = s1 });
            }
            return equipmentList;
        }

        private void horizontalListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // T2
            //if (horizontalListBox.SelectedItem != null)
            //{
            //    if (horizontalListBox.SelectedItem.ToString().Length != 0)
            //    {

            //        EquipmentItem li = (EquipmentItem)horizontalListBox.SelectedItem;
            //        string s = li.Name;
                   
            //        if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.dirSelect)
            //        {
            //            //- get sub folder names
            //            RefreshScrollBar_Files(pSelected_Folder+"\\"+s);
            //            pSelected_SubFolder = s;
            //        }
            //        else
            //        {   // Video File Selected 
            //            ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.fileSelect;
            //            Page_Display("Selection", false);
                        
            //            //ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.playVideo;
            //            //Page_Display("Video", true);
                       
            //            //PlayVideoFile("hydrafacial_training.wmv");
            //            PlayVideoFile(pSelected_Folder +"\\" + pSelected_SubFolder + "\\" + s +"." + Video_Extension);
            //        }
            //    }
            //}
        }

        private void horizontalListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (horizontalListBox.SelectedItem != null)
            {
                if (horizontalListBox.SelectedItem.ToString().Length != 0)
                {

                    EquipmentItem li = (EquipmentItem)horizontalListBox.SelectedItem;
                    string s = li.Name;

                    if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.dirSelect)
                    {
                        //- get sub folder names
                        RefreshScrollBar_Files(pSelected_Folder + "\\" + s);
                        pSelected_SubFolder = s;
                    }
                    else
                    {   
                        // Video File Selected 
                        ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.playVideo;
                        Page_Display("Video", true);
                       
                        PlayVideoFile(pSelected_Folder + "\\" + pSelected_SubFolder + "\\" + s + "." + Video_Extension);
                    }
                }
            }
        }

        //- For Folder Promo ,Training or their children 
        private void RefreshScrollBar_Folders(string selectedFolder)
        {
            //- Set to File Selection Mode
            ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.dirSelect;

            ObservableCollection<EquipmentItem> equipmentList1 = new ObservableCollection<EquipmentItem>();
            this.horizontalListBox.ItemsSource = equipmentList1;

            CreateEquipments_Folders(equipmentList1, selectedFolder);
        }

        private void RefreshScrollBar_Files(string selectedDir)
        {
            //- Set to File Selection Mode
            ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.fileSelect;

            ObservableCollection<EquipmentItem> equipmentList1 = new ObservableCollection<EquipmentItem>();
            this.horizontalListBox.ItemsSource = equipmentList1;

            CreateEquipments_Files(equipmentList1, selectedDir);
        }
        #endregion ====================================================

        #region Demo Training Page Selection
        public void Page_Display(string pagename,bool changemode)
        {
            btn_Back.Visibility = Visibility.Collapsed;
            pg_Promo_Training.Visibility = Visibility.Collapsed;
            pg_Selection.Visibility = Visibility.Collapsed;
            pg_Video.Visibility = Visibility.Collapsed;
            cvsButtons.Visibility = Visibility.Collapsed;                                       // 0102-37

            if (pagename == "Promo_Training")
            {
                pg_Promo_Training.Visibility = Visibility.Visible;
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.Promo_Training;
            }
            else if (pagename == "Selection")                                                   // not use in this version
            {
                btn_Back.Visibility = Visibility.Visible;
                pg_Selection.Visibility = Visibility.Visible;
                
                if ( changemode == true)
                   ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.dirSelect;

            }
            else if (pagename == "Video")
            {
                btn_Back.Visibility = Visibility.Visible;
                pg_Video.Visibility = Visibility.Visible;
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.playVideo;
            }
            else if (pagename == "File_Buttons")                                                // 0102-37  9 File buttons
            {
                cvsButtons.Visibility = Visibility.Visible;
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.fileButtons;
            }
        }

        private void btnPromo_Click(object sender, RoutedEventArgs e)
        {
            //Page_Display("Selection", true);
            //ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.dirSelect;
            //pSelected_Folder = "Promo";
            //RefreshScrollBar_Folders("Promo");

            currentpage = 0;                    // 0103-03

            // for tower 1.5                    // 0102-38
            Page_Display("File_Buttons", true); 
            pSelected_Folder = "Promo";
            RefreshScrollBar_Folders("Promo");

            LoadVideoFiles();                   // 0102-38
            LoadFileToButton(currentpage);      // 0102-38
        }

        private void btnTraining_Click(object sender, RoutedEventArgs e)
        {
            //Page_Display("Selection", true);
            //ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.dirSelect;
            //pSelected_Folder = "Training";
            //RefreshScrollBar_Folders("Training");

            currentpage = 0;                    // 0103-03

            // for tower 1.5                    // 0102-38
            Page_Display("File_Buttons", true);
            pSelected_Folder = "Training";
            RefreshScrollBar_Folders("Training");

            LoadVideoFiles();                   // 0102-38
            LoadFileToButton(currentpage);      // 0102-38
        }
        #endregion

        System.Windows.Threading.DispatcherTimer dispatchertimer = new System.Windows.Threading.DispatcherTimer();
        private void PlayVideoFile(String FileName)
        {
            var FileLocation = Environment.CurrentDirectory + "\\Videos\\"  + ControlParams.Params.p_SecondLanguage + "\\"+ pSelected_Folder +"\\"+ FileName;       // 0106-15
            if (!File.Exists(FileLocation))
            {
                System.Windows.MessageBox.Show(App.getTextMessages("Video File Not Found!"));   // 0103-09
                return;
            }

            try
            {
                MediaElement1.Source = new Uri(FileLocation);
            }
            catch { new NullReferenceException("Error"); }

            VolumeSlider.Value = ControlParams.Params.p_AudioVolume;
            MediaElement1.Volume = VolumeSlider.Value;
         
            dispatchertimer.Interval = new TimeSpan(0, 0, 1);
            dispatchertimer.Start();

            if (MediaElement1.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds); // default
                pbrVideo.Maximum = ts.TotalSeconds;
            }

            SetvideoOn(true);
        }
        
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Pause();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Play();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Stop();
        }

        // 0101-03
        private void Timer_Tick(object sender, EventArgs e)
        {
            pbrVideo.Value = MediaElement1.Position.TotalSeconds;                               // default

            TimeSpan ts = TimeSpan.FromSeconds(MediaElement1.Position.TotalSeconds);
            string str = ts.ToString(@"mm\:ss");
            lblProgressValue.Content = str;
        }

        private void MediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaElement1.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds); // default
              
                pbrVideo.Maximum = ts.TotalSeconds;

                // 0101-03
                string str = ts.ToString(@"mm\:ss");
                lblProgressValue.Content = "0:00";
                lblProgressValue1.Content = " / " + str;
            }
        }
 
        //- Back button on selection page
        private void btnBack_Click(object sender, RoutedEventArgs e)                            // 0102-37
        {
            MediaElement1.Stop();
            MediaElement1.Source = null;

            if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.playVideo)
            {
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.fileSelect;
                Page_Display("Selection", false);

            }
            //- It has 2 modes on selection page , folder and file
            else if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.fileSelect)
            {
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.dirSelect;
                RefreshScrollBar_Folders(pSelected_Folder);

            }
            else if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.dirSelect)
            {
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.Promo_Training;
                Page_Display("Promo_Training", true);
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)                           // 0102-37 button from video player page
        {
            MediaElement1.Stop();
            MediaElement1.Source = null;
            
            Page_Display("File_Buttons", false);                                                // 0102-37
        }

        private void btn_BackfromFile_Click(object sender, RoutedEventArgs e)                   // 0102-37 button on file page
        {
            if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.fileSelect)
            {
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.dirSelect;
                RefreshScrollBar_Folders(pSelected_Folder);

            }
            else if (ControlParams.Params.CurrentVideoMode == ControlParams.e_Video.dirSelect)
            {
                ControlParams.Params.CurrentVideoMode = ControlParams.e_Video.Promo_Training;
                Page_Display("Promo_Training", true);
            }
        }

        // 0101-03
        private void imgPlayStop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (imgPlayStop.Tag == "")
            {
                SetvideoOn(true);
            }
            else
            {
                SetvideoOn(false);
            }
        }

        // 0101-03
        private void imgReplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MediaElement1.Stop();
            DoEvents();
            SetvideoOn(true);
        }

        private void SetvideoOn(bool On)                                                        // 0101-03
        {
            if (On)
            {
                dispatchertimer.Start(); 

                MediaElement1.Play();
                imgPlayStop.Tag = "Stop";
                Utility.Lib.LoadImageFromAppDir(imgPlayStop, "/Skin/Images/n_Stop.png");
            }
            else
            {
                MediaElement1.Pause();
                imgPlayStop.Tag = "";
                Utility.Lib.LoadImageFromAppDir(imgPlayStop, "/Skin/Images/n_Play.png");

                dispatchertimer.Stop();                                                         // 0020-03
            }
        }

        public void setVolume(double V)                                                         // 0101-03
        {
            VolumeSlider.Value = V;
        }

        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement1.Volume = VolumeSlider.Value;
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            Page_Init();

            NavBar.setVolume(ControlParams.Params.p_AudioVolume);                               // 0101-03
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            MediaElement1.Stop();

            MediaElement1.Source = null;

            Page_Display("File_Buttons", true);                                                 // 0102-37

            NavBar.cvsVolume.Visibility = Visibility.Hidden;                                    // 0101-03
        }

        private void Me_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatchertimer.Tick -= new EventHandler(Timer_Tick);                               // 0101-03
        }

        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private bool mouseDown;

        private void ScrollViewer2_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void ScrollViewer2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            
            // Save starting point, used later when determining 
            // how many to scroll.
            scrollStartPoint = e.GetPosition(this);
            scrollStartOffset.X = ScrollViewer2.HorizontalOffset;
            scrollStartOffset.Y = ScrollViewer2.VerticalOffset;

            // Update the cursor if can scroll or not.
            this.Cursor = (ScrollViewer2.ExtentWidth > ScrollViewer2.ViewportWidth) ||
                (ScrollViewer2.ExtentHeight > ScrollViewer2.ViewportHeight) ?
               System.Windows.Input.Cursors.Arrow : System.Windows.Input.Cursors.Arrow;
        }

        private void ScrollViewer2_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the new scroll position.
            Point point = e.GetPosition(this);

            if (mouseDown)
            {
                // Determine the new amount to scroll.
                Point delta = new Point((point.X > this.scrollStartPoint.X) ?
                                 -(point.X - this.scrollStartPoint.X) :
                                    (this.scrollStartPoint.X - point.X),

                                (point.Y > this.scrollStartPoint.Y) ?
                                -(point.Y - this.scrollStartPoint.Y) :
                                    (this.scrollStartPoint.Y - point.Y));

                // Scroll to the newer location
                ScrollViewer2.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                ScrollViewer2.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }

        private void ScrollViewer2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }
        
        // 9 button of click events 
        private void ClickHandler(object sender, System.EventArgs e)                            // 0102-37
        {
            Button button = (Button)sender;
           
            Page_Display("Video", true);
             
            PlayVideoFile(button.Tag + "." + Video_Extension);
        }

        private string[] filePaths { set; get; }
        private int totalvideos { set; get; }
        private int totalpage { set; get; } 
        private int totalremain { set; get; }
        private int currentpage { set; get; }
        private void LoadVideoFiles()                                                           // 0102-37
        {
            filePaths = null;
            filePaths = Directory.GetFiles(Environment.CurrentDirectory + "\\Videos\\"  + ControlParams.Params.p_SecondLanguage + "\\"+ pSelected_Folder+"\\", "*." + Video_Extension);  // 0106-15
           
            totalvideos = filePaths.Length;

            if (totalvideos == null)
                totalpage = -1;
            else
            {
                totalpage = totalvideos / 9;
                totalremain = totalvideos % 9;
            }
        }

        #region Load vidoe File to button 
        private void LoadFileToButton(int page)                                                 // 0102-37
        {
            if (page > totalpage || page < 0)
                return;

            int j = 0; int i = 0;
            for (j = page *9; j < (page+1) * 9; j++)
            {
                if (j < totalvideos)
                {
                  
                    btn[i].Tag = System.IO.Path.GetFileNameWithoutExtension(filePaths[j]);
                 
                    btn[i].Content = new TextBlock
                    {
                        Text=System.IO.Path.GetFileNameWithoutExtension(filePaths[j]) ,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center,
                        Width=150
                    };
                    btn[i].Visibility = Visibility.Visible;
                }
                else
                {
                    btn[i].Content = "";
                    btn[i].Visibility = Visibility.Hidden;
                }
                i++;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) // 0102-37
        {
            if (currentpage + 1 <= totalpage)
            {
                currentpage = currentpage + 1;
                LoadFileToButton(currentpage);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e) // 0102-37
        {
            if (currentpage - 1 >= 0)
            {
                currentpage = currentpage - 1;
                LoadFileToButton(currentpage);
            }
            else
            {
                Page_Display("Promo_Training", false);  // 0102-38
            }
        }
        #endregion

        private void imgPrevious_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentpage - 1 >= 0)
            {
                currentpage = currentpage - 1;
                LoadFileToButton(currentpage);
            }
            else
            {
                Page_Display("Promo_Training", false);                                          // 0102-38
            }

            imgNext.Visibility = (currentpage == totalpage) ? Visibility.Hidden : Visibility.Visible;   // 0106-18
        }

        private void imgNext_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentpage + 1 <= totalpage)
            {
                currentpage = currentpage + 1;
                LoadFileToButton(currentpage);
            }

            imgNext.Visibility = (currentpage == totalpage) ? Visibility.Hidden : Visibility.Visible;   // 0106-18
        }

    }
}
