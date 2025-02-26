using FirmaNetworking.DTO;
using FirmaNetworking;
using FirmaService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using FirmaModel1;
using FirmaNetworking.DTO;
using FirmaService;
using Utils;

namespace FirmaNetworking
{
    public class FirmaServicesRPCProxy : IFirmaServices
    {
        private string host;
        private int port;
        private IFirmaObserver client;
        private TcpClient connection;
        private NetworkStream stream;
        private IFormatter formatter;
        private volatile bool finished;
        private BlockingCollection<Response> queueResponses;

        public FirmaServicesRPCProxy(string host, int port)
        {
            this.host = host;
            this.port = port;
            // formatter = new BinaryFormatter();
            queueResponses = new BlockingCollection<Response>();
        }

        private void InitializeConnection()
        {
            try
            {
                connection = new TcpClient(host, port);
                stream = connection.GetStream();
                formatter = new BinaryFormatter();
                finished = false;
                StartReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void StartReader()
        {
            Thread tw = new Thread(Run);
            tw.Start();
        }

        public virtual void Run()
        {
            while (!finished)
            {
                try
                {
                    if (stream.CanRead && stream.DataAvailable)
                    {
                        // Console.WriteLine(stream);
                        // object raspuns = formatter.Deserialize(stream);
                        // stream.Flush();
                        // stream.Position = 0L;
                        Response response = (Response)formatter.Deserialize(stream);
                        Console.WriteLine("Response received " + response);
                        if (IsUpdate(response))
                        {
                            HandleUpdate(response);
                        }
                        else
                        {
                            queueResponses.Add(response);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    CloseConnection();
                    finished = true;
                }
            }
        }

        private void CloseConnection()
        {
            finished = true;
            try
            {
                stream.Close();
                connection.Close();
                client = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        public Optional<Oficiu> login(string username, string password, IFirmaObserver client)
        {
            InitializeConnection();
            Oficiu utilizator = new Oficiu("", username, password);
            Request request = new Request.Builder().SetType(RequestType.LOGIN).SetData(utilizator).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.OK)
            {
                this.client = client;
                return new Optional<Oficiu>((Oficiu)response.Data);
            }
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                CloseConnection();
                throw new AppException(message);
            }
            return Optional<Oficiu>.Empty();
        }

        private Response ReadResponse()
        {
            Response response = null;
            // int i = 10;
            try
            {
                response = queueResponses.Take();
                // incearca:
                // if (queueResponses.TryDequeue(out Response topElement))
                // {
                //     response = topElement;
                //     Console.WriteLine("Element removed: {0}", topElement);
                //     return response;
                // }
                //
                // if (i >= 0)
                // {
                //
                //     i--;
                //     Thread.Sleep(100);
                //     goto incearca;
                // }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return response;
        }

        private void SendRequest(Request request)
        {
            lock (stream)
            {
                try
                {
                    formatter.Serialize(stream, request);
                    stream.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public void logout(string username)
        {
            Request request = new Request.Builder().SetType(RequestType.LOGOUT).SetData(username).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                throw new AppException(message);
            }
            // this.CloseConnection();
        }

        public List<CursaDTO> getAllCurse()
        {
            Request request = new Request.Builder().SetType(RequestType.GET_Curse).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                throw new AppException(message);
            }
            return (List<CursaDTO>)response.Data;
        }

        public List<CursaDTO> getCursaByDestinatieData(string destinatie, string data)
        {
            Request request = new Request.Builder().SetType(RequestType.GET_Curse_FILTERED).SetData(new FilterDTO(destinatie, data)).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                throw new AppException(message);
            }
            return (List<CursaDTO>)response.Data;
        }

        public void makeReservation(long idCursa, String numeClient, int nrLocuri)
        {
            RezervareDTO rezervare = new RezervareDTO(1L, numeClient, nrLocuri, idCursa);
            Request request = new Request.Builder().SetType(RequestType.RESERVATION).SetData(rezervare).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                throw new AppException(message);
            }
        }

        public List<LocDTO> getLocuriDisponibile(long idCursa)
        {

            Request request = new Request.Builder().SetType(RequestType.GET_Locuri).SetData(idCursa).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                throw new AppException(message);
            }
            return (List<LocDTO>)response.Data;
        }

        public RezervareDTO getRezervareById(long id)
        {
            Request request = new Request.Builder().SetType(RequestType.Get_Rezervare).SetData(id).Build();
            SendRequest(request);
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR)
            {
                string message = (string)response.Data;
                throw new AppException(message);
            }
            return (RezervareDTO)response.Data;
        }

        private bool IsUpdate(Response response)
        {
            return response.Type == ResponseType.UPDATE;
        }

        private void HandleUpdate(Response response)
        {
            if (response.Type == ResponseType.UPDATE)
            {
                client.ReservationUpdate();
            }
        }
    }
}