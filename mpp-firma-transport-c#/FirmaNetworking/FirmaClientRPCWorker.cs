using FirmaModel1;
using FirmaNetworking.DTO;
using FirmaNetworking;
using FirmaService;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Utils;

namespace FirmaNetworking
{
    public class FirmaClientRPCWorker : IFirmaObserver
    {
        private IFirmaServices server;
        private TcpClient connection;
        private NetworkStream stream;
        private IFormatter formatter;
        private volatile bool connected;

        public FirmaClientRPCWorker(IFirmaServices server, TcpClient connection)
        {
            this.server = server;
            this.connection = connection;
            try
            {
                stream = connection.GetStream();
                formatter = new BinaryFormatter();
                connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public virtual void Run()
        {
            while (connected)
            {
                try
                {
                    if (stream.CanRead && stream.DataAvailable)
                    {
                        object request = formatter.Deserialize(stream);
                        //object request = null;
                        object response = HandleRequest((Request)request);
                        if (response != null)
                        {
                            SendResponse((Response)response);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            try
            {
                stream.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e);
            }
        }

        private void SendResponse(Response response)
        {
            lock (stream)
            {
                try
                {
                    Console.WriteLine($"Send response ... {response}");
                    formatter.Serialize(stream, response);
                    stream.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private Response HandleRequest(Request request)
        {
            Response response = null;
            RequestType requestType = request.Type;
            switch (requestType)
            {
                case RequestType.LOGIN:
                    Console.WriteLine("Login request ...");

                    Oficiu utilizator = (Oficiu)request.Data;
                    try
                    {
                        var userOpt = server.login(utilizator.Username, utilizator.Password, this);
                        if (userOpt.HasValue)
                        {
                            return new Response.Builder().SetType(ResponseType.OK).SetData(userOpt.Value).Build();
                        }
                        else
                        {
                            connected = false;
                            return new Response.Builder().SetType(ResponseType.ERROR).SetData("Invalid username or password").Build();
                        }
                    }
                    catch (Exception e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }
                case RequestType.LOGOUT:
                    Console.WriteLine("Logout request ...");
                    string username = (string)request.Data;
                    try
                    {
                        server.logout(username);
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.OK).Build();
                    }
                    catch (AppException e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }
                case RequestType.GET_Curse:
                    Console.WriteLine("Get excursii request ...");
                    try
                    {
                        return new Response.Builder().SetType(ResponseType.GET_Curse).SetData(server.getAllCurse()).Build();
                    }
                    catch (AppException e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }
                case RequestType.GET_Curse_FILTERED:
                    Console.WriteLine("Get excursii filtered request ...");
                    var filter = (FilterDTO)request.Data;
                    try
                    {
                        return new Response.Builder().SetType(ResponseType.GET_Curse_FILTERED).SetData(server.getCursaByDestinatieData(filter.destinatie, filter.data)).Build();

                    }
                    catch (AppException e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }
                case RequestType.RESERVATION:
                    Console.WriteLine("Rezerva request ...");
                    RezervareDTO rezervareDTO = (RezervareDTO)request.Data;
                    try
                    {
                        server.makeReservation(rezervareDTO.IdCursa, rezervareDTO.NumeClient, rezervareDTO.NrLocuri);
                        return new Response.Builder().SetType(ResponseType.OK).Build();
                    }
                    catch (AppException e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }

                case RequestType.GET_Locuri:
                    Console.WriteLine("Get locuri request ...");
                    long idCursa = (long)request.Data;
                    try
                    {
                        return new Response.Builder().SetType(ResponseType.GET_Locuri).SetData(server.getLocuriDisponibile(idCursa)).Build();
                    }
                    catch (AppException e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }

                case RequestType.Get_Rezervare:
                    Console.WriteLine("Get rezervare request ...");
                    long idRezervare = (long)request.Data;
                    try
                    {
                        return new Response.Builder().SetType(ResponseType.Get_Rezervare).SetData(server.getRezervareById(idRezervare)).Build();
                    }
                    catch (AppException e)
                    {
                        connected = false;
                        return new Response.Builder().SetType(ResponseType.ERROR).SetData(e.Message).Build();
                    }
                default:
                    return new Response.Builder().SetType(ResponseType.ERROR).SetData("Invalid request type").Build();
            }
        }




        public void ReservationUpdate()
        {
            Console.WriteLine("Reservation update ...");
            Response response = new Response.Builder().SetType(ResponseType.UPDATE).SetData(null).Build();
            SendResponse(response);
        }
    }
}