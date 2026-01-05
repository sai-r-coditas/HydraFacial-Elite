using System;
using System.IO;
using System.Windows;

using JetBrains.Annotations;

using System.Windows.Forms;

using System.Text;              // 0106-09

using System.Diagnostics;

using System.Windows.Threading;

using Application = System.Windows.Application;

using System.Xml;               // 0106-14

using System.Reflection;        // 0106-14

using System.Windows.Markup;    // 0106-14

using System.Windows.Media;     // 0106-14

using System.ComponentModel;    // 0106-15

namespace Edge.Tower2.UI
{
    /// <summary>
    /// Interaction logic for wpfLanguage.xaml
    /// </summary>
    public partial class wpfLanguage : Window
    {
        public wpfLanguage()
        {
            InitializeComponent();

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            init();                                                                             // 0106-14

            lblMessage.Content = "";                                                            // 0106-12

            App.cs_Events_Language.PropertyChanged += OnPropertyChanged;                        // 0106-15
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)  // 0106-15
        {
            if (propertyChangedEventArgs.PropertyName == "NewLanguage")
            {
                init();
            }
        }

        public void init()                                                                      // 0106-14
        {
            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\" + ControlParams.Params.p_SecondLanguage + "\\WindowBackground_Lng.png");   // 0106-09  0106-13
        }

        private void OnEnter()
        {
            ListAllLanguages();
        }

        private void OnLeave()
        {

        }

        [NotNull]
        private App App
        {
            get { return (App)System.Windows.Application.Current; }
        }

        private void ListAllLanguages()                                                         // 0106-12  0106-13  0106-16
        {
            lstLanguage.Items.Clear();

            string langname;
            foreach (InputLanguage  lang  in InputLanguage.InstalledInputLanguages)
            {
                langname = lang.Culture.ToString();

                if (langname != "" &&
                    File.Exists(Environment.CurrentDirectory + "\\Skin\\Images\\KB2-" + langname + ".png") &&
                    File.Exists(Environment.CurrentDirectory + "\\Skin\\Languages\\rsLanguage-" + langname + ".xaml") &&
                    Directory.Exists(Environment.CurrentDirectory + "\\Skin\\Images\\" + langname) &&
                    Directory.Exists(Environment.CurrentDirectory + "\\Marketing\\" + langname))
                
                    lstLanguage.Items.Add(lang.Culture +" "+lang.Culture.NativeName );
            }
        }
      
        private void btnSet_Click(object sender, RoutedEventArgs e)                     // 0106-12
        {
            if (lstLanguage.SelectedItem != null)
            {
                string lang = lstLanguage.SelectedValue.ToString();

                if (lang.Length < 5)
                    return;

                string str = Environment.CurrentDirectory + "//Skin//Images//KB2-" +
                             lstLanguage.SelectedValue.ToString() + ".png";

                App.wpfyesno.WarningMessage(App.getTextMessages("Change language,") + " " +
                                            App.getTextMessages("are you sure?"));

                App.wpfyesno.ShowDialog();
                var data = App.wpfyesno.ans;

                if (data != "yes")
                    return;

                ControlParams.Params.p_SecondLanguage = lang.Substring(0, 5);

                App.cs_Events_Language.Language = ControlParams.Params.p_SecondLanguage;        // 0106-15

                SaveLanguage();

                // 0106-14
                UpdateLanguageXaml("Skin\\Languages\\rsLanguage-" + ControlParams.Params.p_SecondLanguage + ".xaml");

                ((Photo_Customer_Search) App._mainWindows[Mode.Photo_Customer_Search]).ucKeyboard.LoadKeyBoardImage();

                ShowMessage("*System shutting down...", false);

                DoEvents();

                System.Threading.Thread.Sleep(5000);

                if (Settings.SystemRestart)
                    RestartComputer();
            }
            else
            {
                ShowMessage("Please select the language!", true);
                
                DoEvents();
                System.Threading.Thread.Sleep(3000);
                lblMessage.Content = "";
            }
        }

        private void ShowMessage(string msg, bool ColorRed)                                     // 0106-14
        {
            lblMessage.Content = App.getTextMessages(msg);      
           
            if (ColorRed)
                lblMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDC1212"));  // Red
            else
                lblMessage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00a5e2"));  // Blue  // was #FF247CD4

            DoEvents();
        }

        private static void UpdateLanguageXaml(string filename)                                 // 0106-14
        {
            var dictionaries = App.Current.Resources.MergedDictionaries;
            var filepath = Path.Combine(Environment.CurrentDirectory, filename);

            if (!File.Exists(filepath))
            {
                System.Windows.MessageBox.Show("File not found!");
            }

            try
            {
                var reader = XmlReader.Create(filepath);
                var resourceDictionary = (ResourceDictionary)XamlReader.Load(reader);

                PropertyInfo prop = typeof(ResourceDictionary).GetProperty("Keys");
                object[] keys = prop.GetValue(resourceDictionary, null) as object[];
                foreach (object o in keys)
                {
                    Application.Current.Resources[o.ToString()] = resourceDictionary[o.ToString()].ToString();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(string.Format("The file: {0} could not be loaded: \n{1}", Path.GetFileName(filepath),
                    e.Message));
            }
        }

        private void RestartComputer()
        {
            var psi = new ProcessStartInfo("shutdown", "/r /t 0");                              // restart
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        private void SaveLanguage()                                                             // 0106-09
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + "\\language.dat",false,Encoding.ASCII))  // 0106-18
                {
                    file.WriteLine(ControlParams.Params.p_SecondLanguage);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            App.cs_Events_Language.PropertyChanged -= OnPropertyChanged;                        // 0106-18
        }
    }
}
