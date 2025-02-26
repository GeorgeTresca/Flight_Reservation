
using FirmaNetworking.Utils;
using System;
using System.Net.Sockets;
using System.Threading;
using FirmaService;
using FirmaNetworking;

namespace FirmaNetworking.Utils
{
    public class RPCConcurrentServer : AbstractConcurrentServer
    {
        private IFirmaServices server;
        private FirmaClientRPCWorker worker;
        public RPCConcurrentServer(string host, int port, IFirmaServices server) : base(host, port)
        {
            this.server = server;
            Console.WriteLine("RPCConcurrentServer...");
        }
        protected override Thread CreateWorker(TcpClient client)
        {
            worker = new FirmaClientRPCWorker(server, client);
            return new Thread(new ThreadStart(worker.Run));
        }

    }
}
