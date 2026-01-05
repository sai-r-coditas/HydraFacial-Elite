using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using System.Threading.Tasks;
using System.Windows;

using Edge.EdgeObject;
using JetBrains.Annotations;
using System.Text; // 016-09

namespace Edge.Tower2.UI.UsageReport.Model
{
    public class UsageReportModel : INotifyPropertyChanged
    {
        static FileSystemWatcher _fsw;

        static string _usagefolder;  //sww 0102-33
        public static string UsageFolder
        {
            get
            {
                return _usagefolder;
            }
            set
            {
                _usagefolder = value;
            }
        }

        public UsageReportModel()
        {

            UsageFolder = Environment.CurrentDirectory + "\\..\\UsageLogs";  //sww 00102-33

            if (!Directory.Exists(UsageFolder))
                Directory.CreateDirectory(UsageFolder);

            _fsw = new FileSystemWatcher(UsageFolder);
            //_fsw.Created += (sender, args) => LoadProductUsages();
            //_fsw.Deleted += (sender, args) => LoadProductUsages();
            //_fsw.Changed += (sender, args) => LoadProductUsages();

            _fsw.EnableRaisingEvents = true;

            ProductUsages = new ObservableCollection<ProductUsage>();

            LoadProductUsages();
        }

        public void ClearAllUsages()
        {
            if (!Directory.Exists(UsageFolder))
                return;

            _fsw.EnableRaisingEvents = false;

            foreach (var file in Directory.GetFiles(UsageFolder, "*.usages"))
                 DeleteFile(file);
            
            _fsw.EnableRaisingEvents = true;

            IsLoaded = false;
            
            // 2014 11/05
            LoadProductUsages();
        }

        private void DeleteFile(string FileName)
        {
            try
            {
                File.Delete(FileName);
            }
            catch (IOException e)
            {
                MessageBox.Show("Fail remove the file ->" + FileName);
            }
        }

        public void LogUsage(Station station, int stationIndex, bool startup)
        {
            // 2014 10/15
            // Pop up a Message 
            //NotifyIfAlreadyUsed(station, stationIndex);

            if (!_bottleInsertions.ContainsKey(station.Uid))  // if it is a new uid
            {
                 _bottleInsertions[station.Uid] = 1;

                lock (ProductUsages)
                {
                    ProductUsages.Add(new ProductUsage { DateUsed = DateTime.Now, Product = station.PartNumber });
                }
            }
            else
            {
                if (startup)
                {
                    // 2301 12/10
                    // it need take care last save uid when power up
                    // (if bottle has been changed before power up
                    // count 1)
                    if (station.Uid != bottleUID[stationIndex])
                        _bottleInsertions[station.Uid] += 1;
                    else
                        return;
                       
                }
                else
                {
                    //// Count the bottle been used once
                    _bottleInsertions[station.Uid] += 1;
                }
            }
            
            //=========================================
            // Check count and Pop up a Message
            //========================================= 
            NotifyIfAlreadyUsed(station, stationIndex);

            Task.Run(() =>
            {
                //lock (UsageFileLock)
                //{

                if (!Directory.Exists(UsageFolder))
                {
                    Directory.CreateDirectory(UsageFolder);
                }

                var currentLogFile = UsageFolder+"\\" +
                                     DateTime.Now.Year.ToString("D4") + " " + DateTime.Now.Month.ToString("D2") +
                                     ".usages";

                _fsw.EnableRaisingEvents = false;

                // 2014 11/5
                if (!File.Exists(currentLogFile))
                {
                    using (File.Create(currentLogFile)) { }
                }
                

                var logEntry =
                    string.Format("{0},{1},{2},{3},{4},{5},{6}{7}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        station.Uid,
                        station.PartNumber,
                        station.PartNumberRevisionLevel, station.SerialNumber, station.FillCode,
                        station.BatchFillNumber,
                        Environment.NewLine);

                lock (UsageFileLock)
                {
                    File.AppendAllText(currentLogFile, logEntry, Encoding.ASCII);  // 0106-09
                }

                _fsw.EnableRaisingEvents = true;
                //}
            });
        }

        // 2014 12/10 
        private string[] _bottleUID = new string[5];
        private string[] bottleUID { set { _bottleUID = value; } get { return _bottleUID; } }
        public void LoadBottleDataFile()
        {
            try
            {
                using (StreamReader file = new StreamReader(Environment.CurrentDirectory + "\\..\\UsageLogs\\bottles.dat", Encoding.ASCII))  // 0102-33  0106-09
                {
                    string line;
                    var i = 0;
                    while ((line = file.ReadLine()) != null && i < 5)    // 0106-09 total 5 bottles
                    {
                        bottleUID[i] = line; 
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load bottle information failed!");
            }
        }

        public bool ValidUIDInsertion(string UID, int maxInsertions)  // 2014 10/14 ,0020-05
        {
            if (_bottleInsertions.ContainsKey(UID))
            {
                if (_bottleInsertions[UID] > maxInsertions)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        //public void LogUsage(Station station, int stationIndex)
        //{
        //    // Pop up a Message
        //    NotifyIfAlreadyUsed(station, stationIndex);

        //    if (!_bottleInsertions.ContainsKey(station.Uid))
        //    {
        //        _bottleInsertions[station.Uid] = 1;
                
        //        lock (ProductUsages)
        //        {
        //            ProductUsages.Add(new ProductUsage {DateUsed = DateTime.Now, Product = station.PartNumber});
        //        }
        //    }
        //    else
        //    {
        //        _bottleInsertions[station.Uid] += 1;
        //    }

        //    Task.Run(() =>
        //    {
        //        //lock (UsageFileLock)
        //        //{
                
        //        if (!Directory.Exists(Environment.CurrentDirectory + "\\UsageLogs"))
        //        {
        //            Directory.CreateDirectory(Environment.CurrentDirectory + "\\UsageLogs");
        //        }

        //        var currentLogFile = Environment.CurrentDirectory + "\\UsageLogs\\" +
        //                             DateTime.Now.Year.ToString("D4") + " " + DateTime.Now.Month.ToString("D2") +
        //                             ".usages";

        //        _fsw.EnableRaisingEvents = false;

        //        if (!File.Exists(currentLogFile))
        //        {
        //            var f = File.Create(currentLogFile);
        //            f.Flush(true);
        //            f.Dispose();
        //        }
                
        //        var logEntry =
        //            string.Format("{0},{1},{2},{3},{4},{5},{6}{7}",
        //                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //                station.Uid,
        //                station.PartNumber,
        //                station.PartNumberRevisionLevel, station.SerialNumber, station.FillCode,
        //                station.BatchFillNumber,
        //                Environment.NewLine);

        //        lock (UsageFileLock)
        //        {
        //            File.AppendAllText(currentLogFile, logEntry);
        //        }

        //        _fsw.EnableRaisingEvents = true;
        //        //}
        //    });
        //}

        private static readonly object UsageFileLock = new object();

        private void NotifyIfAlreadyUsed(Station station, int stationIndex)
        {
            if (station.PartNumber == "70140")
            {
                if (_bottleInsertions.ContainsKey(station.Uid) &&
                    _bottleInsertions[station.Uid] >= Settings.MaxRinseawayInsertions)  // 0020-06
                {
                    App.Current.Dispatcher.BeginInvoke((Action) (() =>
                    {
                        //MessageBox.Show(string.Format("The inserted bottle of {0} in position {1} has been used: {2} times.",
                        //    Product.Name(station.PartNumber),
                        //    stationIndex + 1,
                        //    _bottleInsertions[station.Uid].ToString(CultureInfo.CurrentCulture)

                        //    ));

                        WarningMessage_Rinseaway(Product.Name(station.PartNumber), stationIndex,
                            _bottleInsertions[station.Uid].ToString(CultureInfo.CurrentCulture));
                    }));
                }
            }
            else
            {
                if (_bottleInsertions.ContainsKey(station.Uid) &&
                       _bottleInsertions[station.Uid] >= Settings.MaxBottleInsertions)  // 0020-06
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        //MessageBox.Show(string.Format("The inserted bottle of {0} in position {1} has been used: {2} times.",
                        //    Product.Name(station.PartNumber),
                        //    stationIndex + 1,
                        //    _bottleInsertions[station.Uid].ToString(CultureInfo.CurrentCulture)

                        //    ));

                        WarningMessage(Product.Name(station.PartNumber), stationIndex,
                            _bottleInsertions[station.Uid].ToString(CultureInfo.CurrentCulture));
                    }));
                }   
            }
        }

        private void WarningMessage(string partnumber, int stationindex, string counter)
        {
            // 2014 09/15
            if (Convert.ToInt32(counter) > Settings.MaxBottleInsertions)
                ControlParams.Params.valid_insertedbottle[stationindex] = false;
            else
                ControlParams.Params.valid_insertedbottle[stationindex] = true;

            // 2014 10/15
            // Only show once
            if (Convert.ToInt32(counter) == Settings.MaxBottleInsertions )   // 2014 10/14  0020-06
            {
                MessageBox.Show(string.Format("The inserted bottle of {0} in position {1} has been used: {2} times.",
                    partnumber,
                    stationindex + 1,
                    counter
                    ));
            }
        }

        private void WarningMessage_Rinseaway(string partnumber, int stationindex, string counter)
        {
            // 2014 09/15
            if (Convert.ToInt32(counter) > Settings.MaxRinseawayInsertions)
                ControlParams.Params.valid_insertedbottle[stationindex] = false;
            else
                ControlParams.Params.valid_insertedbottle[stationindex] = true;

            // 2014 10/15
            // Only show once
            if (Convert.ToInt32(counter) == Settings.MaxRinseawayInsertions )   // 2014 10/14  0020-06
            {
                MessageBox.Show(string.Format("The inserted bottle of {0} in position {1} has been used: {2} times.",
                    partnumber,
                    stationindex + 1,
                    counter
                    ));
            }
        }
      
        public readonly Dictionary<string, int> _bottleInsertions = new Dictionary<string, int>();
       
        // This will show Loading... Please wait message
        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (value.Equals(_isLoaded)) return;
                _isLoaded = value;
                OnPropertyChanged("IsLoaded");
            }
        }

        // sww changed to public ??
        public void LoadProductUsages()
        {
            Task.Run(() =>
            {
                //lock (UsageFileLock)
                //{

                ProductUsages.Clear();
                _bottleInsertions.Clear();
                
                // 2014 10/02 Add
                ControlParams.Params.valid_insertedbottle[0] = true;
                ControlParams.Params.valid_insertedbottle[1] = true;
                ControlParams.Params.valid_insertedbottle[2] = true;
                ControlParams.Params.valid_insertedbottle[3] = true;
                ControlParams.Params.valid_insertedbottle[4] = true;

                //try { 
                Parallel.ForEach(Directory.GetFiles(UsageFolder, "*.usages"), file =>
                    //foreach (var file in Directory.GetFiles(Environment.CurrentDirectory + "\\UsageLogs", "*.usages"))
                    {
                        var fi = new FileInfo(file);

                        if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            using (var sr = new StreamReader(file,Encoding.ASCII))  // 0106-09
                            {
                                string usageLine;
                                while ((usageLine = sr.ReadLine()) != null)
                                {
                                    var timeString = usageLine.Substring(0, 19);
                                    var uid = usageLine.Substring(20, 16);

                                    //sww modified
                                    var pid = usageLine.Substring(37, 5);

                                    var product = Product.Name(pid);

                                    DateTime usageTime;
                                    if (!DateTime.TryParse(timeString, out usageTime))
                                        continue;

                                    lock (_bottleInsertions)
                                    {
                                        if (!_bottleInsertions.ContainsKey(uid))
                                        {
                                            _bottleInsertions[uid] = 1;
                                            
                                            //sww modified
                                            //ProductUsages.Add(new ProductUsage { DateUsed = usageTime, Product = product });
                                            ProductUsages.Add(new ProductUsage { DateUsed = usageTime, Product = product, PID =pid });
                                        }
                                        else
                                        {
                                            _bottleInsertions[uid] += 1;
                                        }
                                    } //lock
                                } //while
                            }
                        } //if
                        //var stationState = App.BoardManager.mStationStates[0];
                     }

                 );

                IsLoaded = true;

                OnPropertyChanged("ProductUsages");
            });
        }

        public ObservableCollection<ProductUsage> ProductUsages { get; private set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

//=====================================
//
//   Declare Class
//
//=====================================
public class ProductUsage
{
    public string Product { get; internal set; }
    public DateTime DateUsed { get; internal set; }
    //Add by sww
    public string PID { get; internal set; }
}

//sww
public class PDUsage //: IEquatable<PDUsage>
{
    public string Product { get; internal set; }
    public DateTime DateUsed { get; internal set; }
    public string PID { get; internal set; }

    //public override string ToString()
    //{
    //    return "ID: " + PartId + "   Name: " + PartName;
    //}
    //public override bool Equals(object obj)
    //{
    //    if (obj == null) return false;
    //    Part objAsPart = obj as Part;
    //    if (objAsPart == null) return false;
    //    else return Equals(objAsPart);
    //}
    //public override int GetHashCode()
    //{
    //    return PartId;
    //}
    //public bool Equals(Part other)
    //{
    //    if (other == null) return false;
    //    return (this.PartId.Equals(other.PartId));
    //}
    // Should also override == and != operators.

}


//#if (TESTING)
//            _allproductusages.Add(new ProductUsage
//            {
//                Product = "BetaMax HD",
//                MonthlyUsages = new MonthlyUsages
//                {
//                    {DateTime.Parse("11/1/2013"), 5},
//                    {DateTime.Parse("12/1/2013"), 4},
//                    {DateTime.Parse("1/1/2014"), 6},
//                }
//            });

//            _allproductusages.Add(new ProductUsage
//            {
//                Product = "Deep Cleaner XL",
//                MonthlyUsages = new MonthlyUsages
//                {
//                    {DateTime.Parse("11/1/2013"), 0},
//                    {DateTime.Parse("12/1/2013"), 0},
//                    {DateTime.Parse("1/1/2014"), 6},
//                }
//            });
//#endif
//        }

