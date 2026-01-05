using System;
using System.ComponentModel;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

using System.Reflection;   // 0102-38

namespace Edge.Tower2.UI.Printing
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class Printer : INotifyPropertyChanged
    {
        public Printer()
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

            PrintButton.Visibility = Visibility.Hidden;  // 0102-35

            DisableWPFTabletSupport(); // 0102-38  // 0103-06

            Utility.Lib.LoadImageNoLock(imgBG, "\\Skin\\Images\\"+ControlParams.Params.p_SecondLanguage +"\\DailyEssentials_mod3.png");  // 0102-39  0106-13
            Utility.Lib.LoadImageNoLock(imgKeyboard_bg, "\\Skin\\Images\\t2\\Daily-Essential-Form_mod1.png");  // 0102-39  

            lblCustomer.Visibility = Visibility.Hidden;
            lblOperator.Visibility = Visibility.Hidden;
        }

        [NotNull]
        public App App
        {
            get { return (App) Application.Current; }
        }

        private void OnEnter()
        {
            Outputs.LogHeader("Printer", "Enter");

            DataContext = new PrinterViewModel("selected_products.xml");  // 0101-06

            App.Outputs.PrinterPower = true;
            var pvm = (PrinterViewModel) DataContext;

            foreach (var product in pvm.Products)
            {
                product.Quantity = 1;  // TBD: Fix this 
                product.Quantity = 0;
            }

            ControlParams.Params.p_LoginUser = ControlParams.Params.p_LoginUser;  // 0102-02
            lblOperator.Content = ControlParams.Params.p_LoginUser; // 2014 07/21

            lblCustomer.Content = ""; 
          
            // Set default product image load
            if (pvm != null && pvm.Products.Count != 0)   // 0101-07 
               Utility.Lib.LoadImageFromAppDir(imgViewer, "/Products/" + pvm.Products[0].Photo.ToString());

        }

        private void OnLeave()
        {
            App.Outputs.PrinterPower = true;

            Outputs.LogHeader("Printer", "Exit");
        }

        public static void DisableWPFTabletSupport() // 0102-38 framework 4.5.2 not work
        {
            // Get a collection of the tablet devices for this window.  
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                                null, stylusLogic, new object[] { (uint)0 });
                    }
                }

            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Print
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
            
            //headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Qty")) { TextAlignment = TextAlignment.Right }));
            //headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Price")) { TextAlignment = TextAlignment.Right }));
            //headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Amount")) { TextAlignment = TextAlignment.Right }));

            var viewModel = (PrinterViewModel) DataContext;

            //foreach (var product in viewModel.Bundles)
            //{
            //    AddProduct(product, table);
            //}

            foreach (var product in viewModel.Products)
            {
                AddProduct(product, table);
            }

            //doc.Blocks.Add(new Paragraph(new Run(string.Format("SubTotal: {0,9:C2}", viewModel.SubTotal))) { TextAlignment = TextAlignment.Right });
            //doc.Blocks.Add(new Paragraph(new Run(string.Format("{1}% Tax: {0,9:C2}", viewModel.SubTotal * viewModel.SalesTaxRate/100, viewModel.SalesTaxRate))) { TextAlignment = TextAlignment.Right });
            //doc.Blocks.Add(new Paragraph(new Bold(new Run(string.Format("Total:  {0,9:C2}", viewModel.SubTotal * (1 + viewModel.SalesTaxRate/100))))) { TextAlignment = TextAlignment.Right });
            
            //sww add
            doc.Blocks.Add(new Paragraph(new Run(string.Format("Customer: " + lblCustomer.Content))) { TextAlignment = TextAlignment.Right });
            doc.Blocks.Add(new Paragraph(new Run(string.Format("Operator: " + lblOperator.Content))) { TextAlignment = TextAlignment.Right });
          
            IDocumentPaginatorSource paginator = doc;
            paginator.DocumentPaginator.PageSize = new Size(maxWidth, 9999);

            diag.PrintDocument(paginator.DocumentPaginator, "Print output");

            App.Go(Mode.Home);
        }
        #endregion

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

            return; // 0103-06

            var button = (Button) sender;
            var product = (Product) button.DataContext;

            // 2014 12/22
            if (button.Content == "")
            {
                button.Content = "√";
                product.Quantity = 1;
            }
            else
            {
                button.Content = "";
                product.Quantity = 0;
            }
        }

        private void ManipulationBoundaryFeedbackHandler(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

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
            // how much to scroll.
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

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            lblOperator.Content = txtOperator.Text;

            lblCustomer.Content = txtCustomer.Text;
            
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

        private void txtCustomer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UC_Keyboard._CurrentControl = txtCustomer;
        }
 
        private void txtOperator_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UC_Keyboard._CurrentControl = txtOperator;
        }

        private void tbk_Description_MouseDown(object sender, MouseButtonEventArgs e)
        {
          
            var tb = (TextBlock)sender;
            var product = (Product)tb.DataContext;
               
            Utility.Lib.LoadImageFromAppDir(imgViewer, "/Products/" + product.Photo.ToString());
        }

        private void ScrollViewerR_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown_R = false;
        }

        private void ScrollViewerR_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown_R = true;

            // Save starting point, used later when determining 
            // how much to scroll.
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            App.Go(Mode.wpfLogin);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // keyboard disappear
            cvsKeyBoard.Visibility = Visibility.Hidden;
        }

        private void lblOperator_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cvsKeyBoard.Visibility = Visibility.Visible;  // 2014 12/24

            UC_Keyboard.CapsLockFlag = true;
            txtOperator.Text = lblOperator.Content.ToString();

            UC_Keyboard._CurrentControl = txtOperator;
        }

        private void lblCustomer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cvsKeyBoard.Visibility = Visibility.Visible;  // 2014 12/24

            UC_Keyboard.CapsLockFlag = true;
            txtCustomer.Text = lblCustomer.Content.ToString();

            UC_Keyboard._CurrentControl = txtCustomer;
        }

        private void Me_Activated(object sender, EventArgs e)
        {
            NavBar.sldVolume.Value = ControlParams.Params.p_AudioVolume;
            
            // keyboard disappear 0101-07
            //cvsKeyBoard.Visibility = Visibility.Hidden;
        }

        private void Me_Deactivated(object sender, EventArgs e)
        {
            NavBar.cvsVolume.Visibility = Visibility.Hidden;
        }
     }
}