using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace FirmaNetworking.Utils
{
    public abstract class AbstractConcurrentServer : AbstractServer
    {

        public AbstractConcurrentServer(string host, int port) : base(host, port)
        { }

        public override void ProcessRequest(TcpClient client)
        {

            Thread t = CreateWorker(client);
            t.Start();

        }

        protected abstract Thread CreateWorker(TcpClient client);

    }
}
