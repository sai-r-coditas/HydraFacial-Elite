using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using System;

namespace Edge.Tower2.UI.PhotoCapture
{
    public partial class ClientSelector : INotifyPropertyChanged
    {
        private ClientSelectorModel _vm;
        private Client _selectedClient;

        public ClientSelector()
        {
            InitializeComponent();
            DataContext = _vm = new ClientSelectorModel(this);
        }

        private void ClientsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClient = (Client)ClientsListView.SelectedItem;
            OnPropertyChanged("SelectedClient");
        }

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                if (Equals(value, _selectedClient)) return;
                _selectedClient = value;

                ClientsListView.SelectedItems.Clear();
                ClientsListView.SelectedItems.Add(_selectedClient);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
