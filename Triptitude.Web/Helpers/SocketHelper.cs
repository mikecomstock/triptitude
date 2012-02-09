using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;

namespace Triptitude.Web.Helpers
{
    public class SocketHelper
    {
        public static MyConnection connection;

        static SocketHelper()
        {
            IConnectionManager connectonManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            connection = (MyConnection)connectonManager.GetConnection<Triptitude.Web.MyConnection>();
        }

        public static void Send(string s)
        {
            connection.Connection.Broadcast(s);
        }
    }
}