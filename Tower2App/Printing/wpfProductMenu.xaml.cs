using System;
using System.ComponentModel;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

using System.Xml.Linq;  // for XML save

namespace Edge.Tower2.UI.Printing
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class wpfProductMenu : INotifyPropertyChanged
    {
        public wpfProductMenu()
        {
            InitializeComponent();

            // 2014 07/21 disabled by sww
            //DataContext = new PrinterViewModel();

            IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    OnEnter();
                else
                    OnLeave();
            };

            btnPrint.Visibility = Visibility.Hidden; // 0101-06

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\DailyEssentials_config.png");  // 0102-39  0106-13
            Utility.Lib.LoadImageNoLock(imgKeyboard_bg, "\\Skin\\Images\\t2\\Daily-Essential-Form_mod1.png");  // 0102-39  
        }

        [NotNull]
        public App App
        {
            get { return (App) Application.Current; }
        }

        private void OnEnter()
        {
            Outputs.LogHeader("Printer", "Enter");

            DataContext = new PrinterViewModel("products.xml"); //0101-06

            App.Outputs.PrinterPower = true;
            var pvm = (PrinterViewModel) DataContext;

            foreach (var product in pvm.Products)
            {
                product.Quantity = 1;  // TBD: Fix this 
                product.Quantity = 0;
            }

            ControlParams.Params.p_LoginUser = "Admin";

            // var product = pvm.Products[0].;

            if (pvm != null)
               Utility.Lib.LoadImageFromAppDir(imgViewer, "/Products/" + pvm.Products[0].Photo.ToString());
        }

        private void OnLeave()
        {
            App.Outputs.PrinterPower = true;

            Outputs.LogHeader("Printer", "Exit");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Printing
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

            // Create the document, passing a new paragraph and new run using text
            var doc = new FlowDocument {PagePadding = new Thickness(40), LineHeight = 3, FontFamily = new FontFamily("Tahoma"), FontSize = 11};

            // Prinit company information 
            var image = new Image { Source = (ImageSource)App.Resources["st_Logo"], Width = 75, Height = 75, Stretch = Stretch.UniformToFill };  // 0102-09
            doc.Blocks.Add(new BlockUIContainer(image));
            doc.Blocks.Add(new Paragraph(new Bold(new Run("Edge Systems Corporation"))) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Bold(new Run("Signal Hill, CA 90755 US"))) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Bold(new Run("(800) 603-4996"))) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph());
            doc.Blocks.Add(new Paragraph(new Bold(new Run(string.Format("{0:g}", DateTime.Now)))) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph());

            var diag = new PrintDialog();

            
            var maxWidth = 300;  //diag.PrintableAreaWidth;

            var table = new Table { Margin = new Thickness(0), Padding = new Thickness(0), CellSpacing = 0.0 };

            table.Columns.Add(new TableColumn { Width = new GridLength(60) });
            table.Columns.Add(new TableColumn { Width = new GridLength(30) });
            table.Columns.Add(new TableColumn { Width = new GridLength(60) });
            table.Columns.Add(new TableColumn { Width = new GridLength(60) });
            table.RowGroups.Add(new TableRowGroup());

            doc.Blocks.Add(table);

            var headerRow = new TableRow();
            table.RowGroups[0].Rows.Add(headerRow);

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Item")) { TextAlignment = TextAlignment.Left }));
 
            var viewModel = (PrinterViewModel) DataContext;

            foreach (var product in viewModel.Products)
            {
                AddProduct(product, table);
            }
                 
            IDocumentPaginatorSource paginator = doc;
            paginator.DocumentPaginator.PageSize = new Size(maxWidth, 9999);

            diag.PrintDocument(paginator.DocumentPaginator, "Print output");

            App.Go(Mode.Home);
        }
        #endregion

        #region for Prining
        private static void AddProduct(Product product, Table table)
        {
            if (product.Quantity <= 0)
                return;

            table.RowGroups[0].Rows.Add(BlankRow());

            var productRow = new TableRow();
            
            table.RowGroups[0].Rows.Add(productRow);

            productRow.Cells.Add(
                new TableCell(new Paragraph(new Bold(new Run(product.Description))) {TextAlignment = TextAlignment.Left}));
            productRow.Cells[0].ColumnSpan = 4;

            productRow = new TableRow();
            table.RowGroups[0].Rows.Add(productRow);
        }
        #endregion

        private static TableRow BlankRow()
        {
            var blankRow = new TableRow();
            blankRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            return blankRow;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.Home);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
            e.Handled = true;
        }

        private void FasterButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Product)button.DataContext;

            product.Quantity++;
        }

        private void SlowerButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var product = (Product) button.DataContext;

            // 2014 12/22
            if (button.Content == "")
            {
                button.Content = "√";
                product.Quantity = 1;
                product.Mark = "√";  // 0101-06
            }
            else
            {
                button.Content = "";
                product.Quantity = 0;
                product.Mark = "";  // 0101-06
            }
        }

        private void ManipulationBoundaryFeedbackHandler(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        #region Scroll Control
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private bool mouseDown;

        private Point scrollStartPoint_R ;
        private Point scrollStartOffset_R;
        private bool mouseDown_R;

        private void ScrollViewer2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;

            // Save starting point, used later when determining 
            //how much to scroll.
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

        private void ScrollViewer2_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void ScrollViewerR_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown_R = false;
        }

        private void ScrollViewerR_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown_R = true;

            // Save starting point, used later when determining 
            //how much to scroll.
            scrollStartPoint_R = e.GetPosition(this);
            scrollStartOffset_R.X = ScrollViewerR.HorizontalOffset;
            scrollStartOffset_R.Y = ScrollViewerR.VerticalOffset;

            // Update the cursor if can scroll or not.
            this.Cursor = (ScrollViewerR.ExtentWidth > ScrollViewerR.ViewportWidth) ||
                (ScrollViewerR.ExtentHeight > ScrollViewerR.ViewportHeight) ?
               System.Windows.Input.Cursors.Arrow : System.Windows.Input.Cursors.Arrow;
        }

        private void ScrollViewerR_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown_R = false;
        }

        private void ScrollViewerR_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the new scroll position.
            Point point = e.GetPosition(this);

            if (mouseDown_R)
            {
                // Determine the new amount to scroll.
                Point delta = new Point((point.X > this.scrollStartPoint_R.X) ?
                                 -(point.X - this.scrollStartPoint_R.X) :
                                    (this.scrollStartPoint_R.X - point.X),

                                (point.Y > this.scrollStartPoint_R.Y) ?
                                -(point.Y - this.scrollStartPoint_R.Y) :
                                    (this.scrollStartPoint_R.Y - point.Y));

                // Scroll to the newer location
                ScrollViewerR.ScrollToHorizontalOffset(this.scrollStartOffset_R.X + delta.X);
                ScrollViewerR.ScrollToVerticalOffset(this.scrollStartOffset_R.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }
        #endregion

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            // keyboard disappear
            cvsKeyBoard.Visibility = Visibility.Hidden;
        }

        private void getScrollbarvalue()
        {
            //FieldInfo fi = documentViewer1.GetType().GetField("hScrollBar", BindingFlags.Instance | BindingFlags.NonPublic);
            //int scrollPosition = (fi.GetValue(documentViewer1) as DevExpress.XtraEditors.HScrollBar).Value;
        }

        private void btn_ScrollDown_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer2.LineDown();
        }

        private void btn_ScrollUp_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer2.LineUp();
        }
 
        private void tbk_Description_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var tb = (TextBlock)sender;
            var product = (Product)tb.DataContext;
               
            Utility.Lib.LoadImageFromAppDir(imgViewer, "/Products/" + product.Photo.ToString());
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // keyboard disappear
            cvsKeyBoard.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 0101-06
            SaveToSelectedProduct();
            SaveToDefaultProduct();
            App.Go(Mode.wpfSettings);
        }

        public void SaveToSelectedProduct()
        {
            XDocument ndoc = new XDocument();

            XElement nRoot = new XElement("productlist");
            XElement nSubRoot = new XElement("products");

            var pvm = (PrinterViewModel)DataContext;
            foreach (var product in pvm.Products)
            {
                if (product.Mark == "√")
                {
                    XElement xRow = new XElement("product");

                    xRow.Add(new XElement("name", product.Description));
                    xRow.Add(new XElement("price", product.Price));
                    xRow.Add(new XElement("photo", product.Photo));

                    nSubRoot.Add(xRow);
                }
            }

            nRoot.Add(nSubRoot);
            ndoc.Add(nRoot);

            ndoc.Save(Environment.CurrentDirectory + "\\Products\\Selected_Products.xml");  // 0101-06
        }

        public void SaveToDefaultProduct()   // update the default product list file
        {
            XDocument ndoc = new XDocument();

            XElement nRoot = new XElement("productlist");
            XElement nSubRoot = new XElement("products");

            var pvm = (PrinterViewModel)DataContext;
            foreach (var product in pvm.Products)
            {
                XElement xRow = new XElement("product");

                xRow.Add(new XElement("name", product.Description));
                xRow.Add(new XElement("price", product.Price));
                xRow.Add(new XElement("photo", product.Photo));
                xRow.Add(new XElement("mark", product.Mark));

                nSubRoot.Add(xRow);
            }

            nRoot.Add(nSubRoot);
            ndoc.Add(nRoot);

            ndoc.Save(Environment.CurrentDirectory + "\\Products\\Products.xml");  // 0101-06
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            NavBar.sldVolume.Value = ControlParams.Params.p_AudioVolume;
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }
     }
}