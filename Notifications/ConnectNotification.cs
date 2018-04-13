using EscapeDBUsage.Classes;
using Prism.Interactivity.InteractionRequest;

namespace EscapeDBUsage.Notifications
{
    public class ConnectNotification: Confirmation
    {
        public DbConnection DbConnection { get; set; }
    }
}
