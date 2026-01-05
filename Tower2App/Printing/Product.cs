using System.ComponentModel;
using JetBrains.Annotations;

namespace Edge.Tower2.UI.Printing
{
    public class Product : INotifyPropertyChanged
    {
        private uint _quantity;
        private decimal _price;
        private decimal _subTotal;

        internal PrinterViewModel pvm;

        public uint Quantity
        {
            get { return _quantity; }
            set
            {
                if (value == _quantity) return;
                _quantity = value;
                OnPropertyChanged("Quantity");
                SubTotal = Price * Quantity;
            }
        }

        public string Description { get; set; }

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value == _price) return;
                _price = value;
                OnPropertyChanged("Price");
                SubTotal = Price * Quantity;
            }
        }

        // 2014 12/23
        public string Photo { get; set; }
        
        // 0101-06
        public string Mark { get; set; }

        public decimal SubTotal
        {
            get { return _subTotal; }
            private set
            {
                if (value == _subTotal) return;
                _subTotal = value;
                OnPropertyChanged("SubTotal");
                pvm.RefreshGrandTotal();
            }
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