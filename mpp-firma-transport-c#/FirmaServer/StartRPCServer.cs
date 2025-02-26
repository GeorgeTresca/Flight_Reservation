using FirmaNetworking.Utils;
using System;
using System.IO;
using FirmaNetworking.Utils;

using FirmaService;
using FirmaPersistence3;
using FirmaModel1;

namespace FirmaServer
{
    public class StartRPCServer
    {

        
        private static int defaultPort = 8080;
        private static string defaultServer = "127.0.0.1";

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("BinaryFormatter.EnableUnsafeBinaryFormatterSerialization", true);
            string dbUrl;
            using (StreamReader reader = new StreamReader("C:/GEORGE/mpp-proiect-chsarp-GeorgeTresca/firma_client_server/FirmaServer/db.properties"))
            {
                dbUrl = reader.ReadLine();
            }

            int portCitit = defaultPort;
            string serverCitit = defaultServer;
            using (StreamReader reader = new StreamReader("C:/GEORGE/mpp-proiect-chsarp-GeorgeTresca/firma_client_server/FirmaServer/server.properties"))
            {
                portCitit = int.Parse(reader.ReadLine());
                serverCitit = reader.ReadLine();
                // portCitit = reader.ReadLine()
            }

            dbUrl = "Data Source=" + dbUrl;
            var db = new UtilsDB(dbUrl);
            IOficiuRepository oficiuRepo = new OficiuRepository(db);
            ICursaRepository cursaRepo = new CursaRepository(db);
            IRezervareRepository rezervareRepo = new RezervareRepository(db );
            ILocuriRepository locuriRepo = new LocuriRepository(db);
            IFirmaServices serverServices = new FirmaServices (oficiuRepo, locuriRepo, cursaRepo, rezervareRepo);
            AbstractServer server = new RPCConcurrentServer(defaultServer, defaultPort, serverServices);
            Console.WriteLine("Server started ... on port: " + portCitit);
            //print all cursa
            //print all curse using serverServices
            Console.WriteLine("All curse using serverServices");
            var curse = serverServices.getAllCurse();
            foreach (var cursa in curse)
            {
                Console.WriteLine(cursa);
            }
            //print all Curse
             /*var curse = cursaRepo.FindAll();
            foreach (var cursa in curse)
            {
                Console.WriteLine(cursa);
            }

            serverServices.login("oficiu1", "of1", null);
            var cursa1=serverServices.getCursaByDestinatieData("Cluj", "2024-03-24 12:30:00");
            Console.WriteLine(cursa1[0].LocuriDisponibile);
            
            serverServices.makeReservation(1L,"George", 1);
            curse= cursaRepo.FindAll();
            foreach (var cursa in curse)
            {
                Console.WriteLine(cursa);
            }*/
            try
            {
                server.Start();
                Console.WriteLine("Server stopped");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // finally
            // {
            //     server.Stop()
            // }
            // ServiceUtilizator serviceUtilizator = new ServiceUtilizator(repositoryUtilizator);
            // ServiceApplication serviceApplication = new ServiceApplication(repositoryFirmaTransport, repositoryExcursie, repositoryRezervare);
        }
    }
}