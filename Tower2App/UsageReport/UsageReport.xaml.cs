using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Edge.Tower2.UI.UsageReport.Model;
using Edge.Tower2.UI.UsageReport.ViewModel;
using JetBrains.Annotations;

// For add printing features
using System.Collections.Generic;
using System.Windows.Data;

//For DispatcherPriority
using System.Windows.Threading;
using System.IO;

using OfficeOpenXml;


namespace Edge.Tower2.UI.UsageReport
{
    public partial class UsageReport : INotifyPropertyChanged
    {
        UsageReportVm ViewModel { get; set; }

        public UsageReportModel Model { get { return ViewModel.Model; } }

        #region Initialize
        public UsageReport()
        {
            DataContext = ViewModel = new UsageReportVm();
            InitializeComponent();

            ViewModel.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "Counts":
                        Dispatcher.BeginInvoke((Action)UpdateGrid);
                        break;
                }
            };

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            // 2014 12/10
            Model.LoadBottleDataFile();
        }

        public void page_init()
        {
            Model.LoadProductUsages();
        }

        [NotNull]
        public App App
        {
            get { return (App)Application.Current; }
        }

        private void OnEnter()
        {
            Outputs.LogHeader("UsageReport", "Enter");
            NavBar.sldVolume.Value = ControlParams.Params.p_AudioVolume;
        }

        private void OnLeave()
        {
            Outputs.LogHeader("UsageReport", "Exit");
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }

        private int[] _MonTotal = new int[20];
        public int[] MonTotal
        {
            get { return _MonTotal; }
            set { _MonTotal = value; }
        }
        #endregion

        #region Create EXcel File
        private void CreateExcelFile_Epplus()  // 0102-26
        {
            string toExcelFile = Environment.CurrentDirectory + "\\..\\UsageLogs\\usagereport.xlsx";  // 0102-33

            ExcelPackage p = new ExcelPackage();
            try
            {
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Usage Report");

                CreateCell(ws, 1, 1, "ID", 1);
                CreateCell(ws, 1, 2, "Product Name", 1);
                var i = 0;
                for (i = 0; i < UsageReportVm.NumberOfMonths; i++)
                {
                    var cell = ws.Cells[1, i + 3];
                    cell.Value = ViewModel.Months[i];
                }

                CreateCell(ws, 1, i + 3, "Total", 1);

                int row = 2;
                int TotalCount = 0;
                int SumTotal = 0;

                #region month data
                Array.Clear(MonTotal, 0, MonTotal.Length); // For monthly total

                foreach (var product in ViewModel.Counts.Keys)
                {
                    CreateCell(ws, row, 1, product, 1);
                    CreateCell(ws, row, 2, Product.Name(product), 1);

                    int offset = 1;
                    TotalCount = 0;

                    foreach (var populatedMonthDate in ViewModel.Counts[product].Keys)
                    {
                        var nowMonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                        var monthsBeforeNow = (nowMonthDate.Year * 12 + nowMonthDate.Month) -
                                              (populatedMonthDate.Year * 12 + populatedMonthDate.Month);

                        var column = UsageReportVm.NumberOfMonths - monthsBeforeNow + offset;

                        MonTotal[column] += Convert.ToInt32(ViewModel.Counts[product][populatedMonthDate]);

                        if (column <= 1)
                            continue; // Old Data in the folder

                        CreateCell(ws, row, column + 1,
                            ViewModel.Counts[product][populatedMonthDate].ToString(CultureInfo.CurrentCulture), 1);

                        TotalCount += Convert.ToInt32(ViewModel.Counts[product][populatedMonthDate]);


                    } // End of For

                    SumTotal += TotalCount;

                    CreateCell(ws, row, 21, TotalCount.ToString(), 1);

                    row++;
                }

                ws.Column(2).Width = 30;
                CreateCell(ws, row, 2, "Monthly Total", 1);

                int col;
                for (col = 0; col < 18; col++) 
                {
                    if (MonTotal[col+2] != 0)
                        CreateCell(ws, row, col + 3, Convert.ToString(MonTotal[col+2]), 1);
                }

                CreateCell(ws, row, col + 3, Convert.ToString(SumTotal).ToString(), 1);
                #endregion

                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();

                string file = toExcelFile;

                if (File.Exists(toExcelFile))
                {
                    File.Delete(toExcelFile);
                }

                File.WriteAllBytes(file, bin);

                if (p != null) p.Dispose();

            }
            catch (Exception ex)
            {
                MessageBox.Show("File create failed!");
                if (p != null) p.Dispose();
            }
        }

        private void CreateCell(ExcelWorksheet ws,int row, int col, string data, int type)
        {
            var cell = ws.Cells[row, col];
            cell.Value = data;
        }

        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; // "Calibri"; //Default Font name for whole sheet

            return ws;
        }
        #endregion

        #region usage Grid table
        private void UpdateGrid()
        {
            //sww
            DoEvents();

            grdContent.RowDefinitions.Clear();
            grdContent.ColumnDefinitions.Clear();
            grdContent.Children.Clear();

            grdContent.Margin = new Thickness(30, 120, 30, 50);
            grdContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            ////=================================================
            // sww, add productID Header 
            //===================================================
            grdContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
            AddHeader("\nID", 20, 0);  // Name, Intensity ,Col

            ////=================================================
            // The "Product" Header 
            //===================================================
            grdContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            AddHeader("\nPRODUCT NAME", 10, 1);

            var i = 0;
            TextBlock textBlock;
            for (i = 0; i < UsageReportVm.NumberOfMonths; i++)
            {
                //// Column Stripe
                grdContent.ColumnDefinitions.Add(new ColumnDefinition());
                              
                //======================
                //// Month Header
                //======================
                textBlock = new TextBlock
                {
                    Text = ViewModel.Months[i],      // show month 2014 Jan......
                    //Text = (i + 1).ToString(),     // show number 1...18
                    FontSize = 14,
                    Height=50,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(0,10,0,0),
                    Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8")),
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffffff")),
                    Margin = new Thickness(1.0, 0.0, 0.0, 0.0)
                };

                grdContent.Children.Add(textBlock);
                Grid.SetColumn(textBlock, i + 2);
            }

            //====================================================================================
            //====================================================================================

            //// Total Column on end of right hand side
            grdContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            AddHeader("\nTOTAL", 10, i + 3);

            ////===============================================================
            // Add Content / Content /Content
            //=================================================================

            //// for each row =================================================
            int row = 1;
            int TotalCount = 0;
            int SumTotal = 0;

            try
            {
                Array.Clear(MonTotal, 0, MonTotal.Length); //For monthly total

                foreach (var product in ViewModel.Counts.Keys)
                {
                    ////=========================
                    //// Row Stripe
                    ////=========================
                    grdContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

                    //// H Strips
                    textBlock = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                       
                        Background = row % 2 == 1
                             ? new SolidColorBrush(new Color { A = 255, R = 216, G = 229, B = 245 })
                             : new SolidColorBrush(new Color { A = 255, R = 226, G = 243, B = 248 })
                    };
                    grdContent.Children.Add(textBlock);
                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, 0);
 
                    //// product Id, Name ============================================
                    AddColumn1(textBlock, product, 0, row);
                    AddColumn1(textBlock, Product.Name(product), 1, row);
                    //================================================================

                    // H Strips change color
                    textBlock = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c")), 
                        Background = row % 2 == 1
                             ? new SolidColorBrush(new Color { A = 255, R = 235, G = 234, B = 235 })
                             : new SolidColorBrush(new Color { A = 255, R = 240, G = 240, B = 240 })
                    };

                    grdContent.Children.Add(textBlock);
                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, 2);
                    Grid.SetColumnSpan(textBlock, UsageReportVm.NumberOfMonths + 1);

                    int offset = 1;
                    TotalCount = 0;
                    
                    foreach (var populatedMonthDate in ViewModel.Counts[product].Keys)
                    {
                        //index++; // for monthly total
                        //MonTotal[index] += index;

                        var nowMonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                        var monthsBeforeNow = (nowMonthDate.Year * 12 + nowMonthDate.Month) -
                                              (populatedMonthDate.Year * 12 + populatedMonthDate.Month);

                        var column = UsageReportVm.NumberOfMonths - monthsBeforeNow + offset;

                        MonTotal[column] += Convert.ToInt32(ViewModel.Counts[product][populatedMonthDate]);

                        if (column <= 1)
                            continue; // Old Data in the folder

                        AddColumn2(textBlock, ViewModel.Counts[product][populatedMonthDate].ToString(CultureInfo.CurrentCulture), column, row);
                        TotalCount += Convert.ToInt32(ViewModel.Counts[product][populatedMonthDate]);

                    } // End of For

                    SumTotal += TotalCount;

                    //// Show Total Monthly Usage Count
                    //// in Right side last column
                    textBlock = new TextBlock
                    {
                        Text = TotalCount.ToString(),
                        FontSize = 14,
                        FontWeight = FontWeight.FromOpenTypeWeight(600),
                        Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c")),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextAlignment = TextAlignment.Center
                        
                    };

                    grdContent.Children.Add(textBlock);
                    Grid.SetColumn(textBlock, 20);
                    Grid.SetRow(textBlock, row);

                    row++;
                }

                //=====================================================
                // sww, Add one row on bottom for total of each month
                //=====================================================
                //// H Strips, bottom of Product column
                grdContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

                textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = row % 2 == 1
                         ? new SolidColorBrush(new Color { A = 255, R = 235, G = 234, B = 235 })
                         : new SolidColorBrush(new Color { A = 255, R = 240, G = 240, B = 240 })
                };
                grdContent.Children.Add(textBlock);
                Grid.SetRow(textBlock, row);
                Grid.SetColumn(textBlock, 0);
                Grid.SetColumnSpan(textBlock, UsageReportVm.NumberOfMonths + 3);
 
                //// H Strips, bottom of Product column
                // First Column
                textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8")),
                    Margin = new Thickness(1.0, 0.0, 0.0, 0.0),
                    Background = row % 2 == 1
                           ? new SolidColorBrush(new Color { A = 255, R = 216, G = 229, B = 245 })
                           : new SolidColorBrush(new Color { A = 255, R = 226, G = 243, B = 248 })
                };
                grdContent.Children.Add(textBlock);
                Grid.SetRow(textBlock, row);
                Grid.SetColumn(textBlock, 0);

                //Grid.SetColumnSpan(textBlock, UsageReportVm.NumberOfMonths + 3); // ??
 
                // Second Column
                textBlock = new TextBlock
                {
                    Text = "Monthly Total",
                    FontSize = 14,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeight.FromOpenTypeWeight(600),
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8")),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(1.0, 0.0, 0.0, 0.0),
                    Background = row % 2 == 1
                           ? new SolidColorBrush(new Color { A = 255, R = 216, G = 229, B = 245 })
                           : new SolidColorBrush(new Color { A = 255, R = 226, G = 243, B = 248 })
                };
                grdContent.Children.Add(textBlock);
                Grid.SetRow(textBlock, row);
                Grid.SetColumn(textBlock, 1);

                TotalCount = 0;
                        
                int col;
                for (col = 2; col <= 19; col++)
                {
                    if (MonTotal[col] !=0)
                    AddColumn2(textBlock,Convert.ToString(MonTotal[col]), col, row);
                } 
                AddColumn2(textBlock, Convert.ToString(SumTotal), col, row);

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Usage Report >>>" + e.Message);
            }
            // ==================================================
        }
        #endregion

        #region Pring Report
        //// For printing
        private void PrintReport()
        {

            PrintDialog printDialog = new PrintDialog();

            GridView p_GridView = new GridView();

            //// sww, add productID Header ========================
            p_AddGridColumn(p_GridView, "ID", "\nProd. ID", 90);

            ////===================================================
            // The "Product" Header 
            //=====================================================
            p_AddGridColumn(p_GridView, "ProductName", "\nProduct Name", 180);

            var i = 0;
            TextBlock textBlock;
            for (i = 0; i < UsageReportVm.NumberOfMonths; i++)
            {
                //// For header printing
                p_AddGridColumn(p_GridView, "M" + i.ToString(), ViewModel.Months[i], 38);
            }

            //// Total Column
            p_AddGridColumn(p_GridView, "Total", "\nTotal", 60);
            //// for printing
            lstView.View = p_GridView;

            ////===============================================================
            // Add Content / Content /Content
            //=================================================================

            //// for each row =================================================
            int row = 0;
            int TotalCount = 0;
            int SumTotal = 0;
            int pg = 1;
            try
            {
                // For printing
                List<p_Report> p_items = new List<p_Report>();

                Array.Clear(MonTotal, 0, MonTotal.Length); //For monthly total

                foreach (var product in ViewModel.Counts.Keys) // Get total total products which have been used
                {
                    row++;
                   
                    int offset = 1;
                    TotalCount = 0;
                    
                    Array.Clear(_pVal, 0, _pVal.Length);

                    foreach (var populatedMonthDate in ViewModel.Counts[product].Keys)
                    {
                        var nowMonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                        var monthsBeforeNow = (nowMonthDate.Year * 12 + nowMonthDate.Month) -
                                                (populatedMonthDate.Year * 12 + populatedMonthDate.Month);

                        var column = UsageReportVm.NumberOfMonths - monthsBeforeNow + offset;
                        MonTotal[column] += Convert.ToInt32(ViewModel.Counts[product][populatedMonthDate]); // for monthly total  

                        if (column <= 1)
                            continue; //// Old Data in the folder

                        _pVal[column - 2] = ViewModel.Counts[product][populatedMonthDate].ToString(CultureInfo.CurrentCulture);

                        TotalCount += Convert.ToInt32(ViewModel.Counts[product][populatedMonthDate]);

                    } //// End of second Foreach

                    SumTotal += TotalCount;

                    AddToList(p_items, product,Product.Name(product),TotalCount.ToString());

                    //// 8 rows of data filled and now print the page
                    if (row >= 8)
                    {
                        //lblPrintPage.Content = "Page " + pg;
                        lblPrintPage.Content = DateTime.Now.ToString("MM/dd/yyyy") + "  Page " + pg;
                        
                        lstView.ItemsSource = p_items;
                        lstView.Items.Refresh();

                        ICollectionView view = CollectionViewSource.GetDefaultView(lstView.ItemsSource);
                        view.Refresh();
                        DoEvents();

                        printDialog.PrintVisual(grd_L2_g1, "Usage Report");

                        //// Start counting again for next page
                        pg++;
                        row = 0;
                        p_items.Clear();
                        lstView.ItemsSource = p_items;
                    }
                } //// End of first foreach

                Array.Clear(_pVal, 0, _pVal.Length);
                CopyToBuffer(ref _pVal,MonTotal);
                
                //// Last row to monthly total
                row++;
                
                //// Monthly Total
                AddToList (p_items,"","Total",SumTotal.ToString());

                //// Print the last page
                if (row >0)
                {
                    lblPrintPage.Content = DateTime.Now.ToString("MM/dd/yyyy") + "  Page " + pg;

                    lstView.ItemsSource = p_items;
                    lstView.Items.Refresh();
                    ICollectionView view = CollectionViewSource.GetDefaultView(lstView.ItemsSource);
                    view.Refresh();
                    DoEvents();
                    printDialog.PrintVisual(grd_L2_g1, "Usage Report");
                }

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Usage Report >>" + e.Message);
            }
        }
        #endregion

        private void AddToList(List<p_Report> L, string add_id , string add_productname, string sumtotal)
        { 
            L.Add(new p_Report()  ////For printing
            {
                ID = add_id,
                ProductName = add_productname,
                M0 = _pVal[0],
                M1 = _pVal[1],
                M2 = _pVal[2],
                M3 = _pVal[3],
                M4 = _pVal[4],
                M5 = _pVal[5],
                M6 = _pVal[6],
                M7 = _pVal[7],
                M8 = _pVal[8],
                M9 = _pVal[9],
                M10 = _pVal[10],
                M11 = _pVal[11],
                M12 = _pVal[12],
                M13 = _pVal[13],
                M14 = _pVal[14],
                M15 = _pVal[15],
                M16 = _pVal[16],
                M17 = _pVal[17],
                Total = sumtotal
            });
        }

        private void CopyToBuffer(ref string[] _pVal, int[] MonTotal)
        {
            for (int i = 0; i < 18; i++)
            {
                if (MonTotal[i + 2] != 0)
                    _pVal[i] = MonTotal[i + 2].ToString();
                else
                    _pVal[i] = "";
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void AddHeader( string HeaderName, byte Intensity, int Col)
        {
            var textBlock1 = new TextBlock
            {
                Text = HeaderName,
                FontSize = 14,
                Height=50,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Padding = new Thickness(0, 10, 0, 0),
                Margin = new Thickness(1.0, 0.0, 0.0, 0.0),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8")),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffffff")),
            };
            Grid.SetColumn(textBlock1, Col);
            grdContent.Children.Add(textBlock1);
        }

        private void AddColumn(TextBlock textBlock,string strText,int Col, int Row)
        {
            textBlock = new TextBlock
            {
                Text = strText,
                FontWeight = FontWeight.FromOpenTypeWeight(600),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Center
            };
            grdContent.Children.Add(textBlock);
            Grid.SetRow(textBlock, Row);
            Grid.SetColumn(textBlock, Col);
        }

        // also add border
        private void AddColumn1(TextBlock textBlock, string strText, int Col, int Row)
        {
            textBlock = new TextBlock
            {
                Text = strText,
                FontWeight = FontWeight.FromOpenTypeWeight(600),
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff08c5d8")),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(1.0, 0.0, 0.0, 0.0),
                Background = Row % 2 == 1
                           ? new SolidColorBrush(new Color { A = 255, R = 216, G = 229, B = 245 })
                           : new SolidColorBrush(new Color { A = 255, R = 226, G = 243, B = 248 })
            };
            grdContent.Children.Add(textBlock);
            Grid.SetRow(textBlock, Row);
            Grid.SetColumn(textBlock, Col);
        }

        // Change ForeGround
        private void AddColumn2(TextBlock textBlock, string strText, int Col, int Row)
        {
            textBlock = new TextBlock
            {
                Text = strText,
                FontWeight = FontWeight.FromOpenTypeWeight(600),
                FontSize = 14,
                Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff64676c")),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Center
            };
            grdContent.Children.Add(textBlock);
            Grid.SetRow(textBlock, Row);
            Grid.SetColumn(textBlock, Col);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private Size ControlSize(Control control, string content)
        {
            var formattedText = new FormattedText(
                content, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch),
                control.FontSize, Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void btnDeleteUsages_Click(object sender, RoutedEventArgs e)
        {
            Model.ClearAllUsages();
        }
      
        ////==================================================
        // Print Useage , Using Listview
        //====================================================

        private string[] _pVal = new string[20];
        public string[] pVal
        {
            get { return _pVal; }
            set { _pVal = value; }
        }

        public class p_Report
        {
            public string ID { get; set; }
            public string ProductName { get; set; }
            public string M0 { get; set; }
            public string M1 { get; set; }
            public string M2 { get; set; }
            public string M3 { get; set; }
            public string M4 { get; set; }
            public string M5 { get; set; }
            public string M6 { get; set; }
            public string M7 { get; set; }
            public string M8 { get; set; }
            public string M9 { get; set; }
            public string M10 { get; set; }
            public string M11 { get; set; }
            public string M12 { get; set; }
            public string M13 { get; set; }
            public string M14 { get; set; }
            public string M15 { get; set; }
            public string M16 { get; set; }
            public string M17 { get; set; }
            public string Total { get; set; }
        }
 
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintReport();
            return;
        }

        private void p_AddGridColumn(GridView gv, string ColumnName, string Name, int ColumnWidth)
        {
            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding(ColumnName);
            gvc1.Header = Name;
            gvc1.Width = ColumnWidth;
            gv.Columns.Add(gvc1);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }
 
        // 2014 11/03
        private void sumUsage(System.DateTime Date1, System.DateTime Date2)
        {


        }

        private void DBToExcel()
        {

        }

        private void Me_Activated(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            //CreateExcelFile();
            CreateExcelFile_Epplus();
        }

        private void Me_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}