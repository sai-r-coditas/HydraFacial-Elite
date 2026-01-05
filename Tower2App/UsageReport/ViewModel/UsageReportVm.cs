#define TESTING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Edge.Tower2.UI.UsageReport.Model;
using JetBrains.Annotations;

using System.Windows;
using System.Windows.Threading;

namespace Edge.Tower2.UI.UsageReport.ViewModel
{
    public sealed class UsageReportVm : INotifyPropertyChanged
    {
        public const int NumberOfMonths = 18;

        private readonly UsageReportModel _model = new UsageReportModel();
        
        public UsageReportModel Model { get { return _model; } set { } }

        public string[] Months { get; private set; }

        public SortedDictionary<string, Dictionary<DateTime, int>> Counts { get; private set; } 

        public UsageReportVm()
        {
            Months = new string[NumberOfMonths];

            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            for (var i = NumberOfMonths-1; i >= 0; i--)
            {
                var startOfMonth = new DateTime(year, month, 1);
                Months[i] = startOfMonth.ToString("MMM\nyyyy");

                month--;
                if (month == 0)
                {
                    month = 12;
                    year--;
                }

                _model.PropertyChanged += (sender, args) =>
                {
                    switch (args.PropertyName)
                    {
                        case "ProductUsages":
                            if (_model.IsLoaded)
                                UpdateCounts();
                            break;
                    }
                };
            }

            OnPropertyChanged("Months");

            Counts = new SortedDictionary<string, Dictionary<DateTime, int>>();

            UpdateCounts();
        }

        //sww modified change product to pid 
        private void UpdateCounts()
        {

            return;  // 0103-02

            lock (_model.ProductUsages)
            {
                Counts.Clear();
                               
                // carefully check here ??
 
                foreach (var productUsage in _model.ProductUsages)
                //foreach (var productUsage in PD_Usage)
                {
                    var usageMonthDate = new DateTime(productUsage.DateUsed.Year, productUsage.DateUsed.Month, 1);

                    lock (Counts)
                    {
                        if (!Counts.ContainsKey(productUsage.PID))
                        {
                            var singleProductUsages = new Dictionary<DateTime, int> {{usageMonthDate, 1}};
                            Counts.Add(productUsage.PID, singleProductUsages);
                        }
                        else
                        {
                            var singleProductUsages = Counts[productUsage.PID];

                            if (!singleProductUsages.ContainsKey(usageMonthDate))
                            {
                                singleProductUsages.Add(usageMonthDate, 1);
                            }
                            else
                            {
                                singleProductUsages[usageMonthDate] += 1;
                            }
                        }
                    }
                }

            }

            Model.IsLoaded = true;  //Add by sww 2014 10/15
            
            OnPropertyChanged("Counts");
        }

      
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    
        private void CopyToBuffer()
        {
            List<PDUsage> PD_Usage = new List<PDUsage>();
            foreach (var productUsage in _model.ProductUsages)
            {
                // Add content to the list.
                PD_Usage.Add(new PDUsage() { Product = productUsage.Product , DateUsed = productUsage.DateUsed , PID=productUsage.PID });
            }
    
        }

        public void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
    
    }
     
}


