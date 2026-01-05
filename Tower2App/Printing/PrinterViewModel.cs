using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using JetBrains.Annotations;

// for readXML
using System.IO;

using System.Xml.Serialization;

// sww add tax
using System.Configuration;

namespace Edge.Tower2.UI.Printing
{
    public class PrinterViewModel : INotifyPropertyChanged
    {
        private readonly List<Product> products = new List<Product>();
        public ReadOnlyCollection<Product> Products { get; private set; }

        private readonly List<Product> bundles = new List<Product>();
        private decimal _subTotal;
        private decimal _salesTaxRate;
        public ReadOnlyCollection<Product> Bundles { get; private set; }

        internal void RefreshGrandTotal()
        {
            SubTotal = 0;

            foreach (var product in Products)
            {
                SubTotal += product.SubTotal;
            }

            foreach (var bundle in Bundles)
            {
                SubTotal += bundle.SubTotal;
            }
        }

        public decimal SubTotal
        {
            get { return _subTotal; }
            private set
            {
                if (value == _subTotal) return;
                _subTotal = value;
                OnPropertyChanged("SubTotal");
                OnPropertyChanged("GrandTotal");
                OnPropertyChanged("Tax");
            }
        }

        public decimal Tax
        {
            get { return Decimal.Round(_subTotal * _salesTaxRate / 100, 2); }
        }

        public decimal GrandTotal
        {
            get { return _subTotal + Tax; }
        }

        public decimal SalesTaxRate
        {
            get { return _salesTaxRate; }
            set
            {
                _salesTaxRate = value;
                OnPropertyChanged("SalesTaxRate");
            }
        }

        public PrinterViewModel(string FileName)
        {
            Products = new ReadOnlyCollection<Product>(products);
            Bundles = new ReadOnlyCollection<Product>(bundles);
            
            productlist pdlist = null;

            // 2014/07/21 Add by sww  2014 12/19 disabled no in use
            //pdlist = ReadXML("products\\product_bundle.xml");
            //foreach (var pd in pdlist.item)
            //{
            //    bundles.Add(new Product { pvm = this, Description = pd.name, Price = System.Convert.ToDecimal(pd.price), Quantity = 0 });
            //}

            pdlist = null;
            pdlist = ReadXML("products\\" + FileName); // 0101-06
            foreach (var pd in pdlist.item)
            {
                // 2014 12/23
                products.Add(new Product { pvm = this, Description = pd.name, Price = System.Convert.ToDecimal(pd.price), Photo = pd.photo, Mark= pd.mark, Quantity = 0  });  // 0101-06
            }

            // Changed by sww
            SalesTaxRate = 10.00m;
            SalesTaxRate = decimal.Parse(ConfigurationManager.AppSettings["LocalTax"]);

            // 2014 12/19
            // RefreshGrandTotal();
        }

        // 2014 07/21 Add by sww
        public productlist ReadXML(string FileName)   // UTF-8 format
        {
            string path = FileName;
            XmlSerializer serializer = new XmlSerializer(typeof(productlist));
            StreamReader reader = new StreamReader(path);   // UTF-8 format
            productlist pList;
            pList = (productlist)serializer.Deserialize(reader);
            reader.Close();
            return pList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}