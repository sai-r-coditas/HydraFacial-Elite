using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Edge.Tower2.UI.PhotoCapture
{
    public class ClientSelectorModel : INotifyPropertyChanged
    {
        FileSystemWatcher fsw;

        private readonly ObservableCollection<Client> _clients = new ObservableCollection<Client>();
        public ReadOnlyObservableCollection<Client> Clients { get; private set; }

        private Dispatcher _dispatcher;
        private ClientSelector _clientSelector;

        public ClientSelectorModel(ClientSelector clientCapture)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Photos"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Photos");

            fsw = new FileSystemWatcher(Environment.CurrentDirectory + "\\Photos");

            _clientSelector = clientCapture;
            _dispatcher = Dispatcher.CurrentDispatcher;

            Clients = new ReadOnlyObservableCollection<Client>(_clients);

            foreach (var file in Directory.GetDirectories(Environment.CurrentDirectory + "\\Photos"))
            {
                var fi = new FileInfo(file);
                
                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    _clients.Add(new Client { Path = file });
                }
            }

            fsw.Created += (sender, args) =>
            {
                Thread.Sleep(300);
                _dispatcher.Invoke((Action)(() =>
                {
                    var client = new Client { Path = args.FullPath };
                    _clients.Add(client);;
                    _clientSelector.ClientsListView.ScrollIntoView(client);
                    _clientSelector.ClientsListView.UnselectAll();
                }));
            };

            fsw.Deleted += (sender, args) => _dispatcher.Invoke((Action)(() => _clients.Remove(_clients.First(p => p.Path == args.FullPath))));

            fsw.EnableRaisingEvents = true;
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