using EscapeDBUsage.Confirmations;
using EscapeDBUsage.Events;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.InteractionRequests
{
    public class ConnectRequest : InteractionRequest<ConnectConfirmation>
    {
        public ConnectRequest(IEventAggregator evAgg)
        {
            FinishInteraction = new Action(() => {
                if ((Notification as ConnectConfirmation).DatabaseConnection.IsConnected)
                {
                    evAgg.GetEvent<ConnectEvent>().Publish((Notification as ConnectConfirmation).DatabaseConnection);
                }
            });
        }

        public Action FinishInteraction { get; private set; }
        public INotification Notification { get; set; }
    }
}
