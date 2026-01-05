using System.Collections.Generic;

namespace Edge.Tower2.UI.Client.ViewModel
{
    public class ClientViewModel
    {
        readonly Model.ClientModel _model = new Model.ClientModel();

        public IList<Model.Client> Clients
        {
            get { return _model.Clients; }

        }

    }
}