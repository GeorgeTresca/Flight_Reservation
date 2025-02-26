using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FirmaModel1;
using FirmaPersistence3;
using FirmaService;
using Utils;

namespace FirmaServer
{
    public class FirmaServices : IFirmaServices
    {
        private IOficiuRepository oficiuRepo;
        private ILocuriRepository locuriRepo;
        private ICursaRepository cursaRepo;
        private IRezervareRepository rezervareRepo;
        private Dictionary<string, IFirmaObserver> _loggedClients;
        private readonly int defaultThreads = 5;
        private readonly object _lock = new object();

        public FirmaServices(IOficiuRepository oficiuRepo, ILocuriRepository locuriRepo, ICursaRepository cursaRepo, IRezervareRepository rezervareRepo)
        {
            this.oficiuRepo = oficiuRepo;
            this.locuriRepo = locuriRepo;
            this.cursaRepo = cursaRepo;
            this.rezervareRepo = rezervareRepo;
            _loggedClients = new Dictionary<string, IFirmaObserver>();
        }



        public Optional<Oficiu> login(string username, string password, IFirmaObserver client)
        {
            Console.WriteLine("Login request");
            lock (_lock)
            {
                var utilizatorOpt = oficiuRepo.FindByUserPassword(username, password);
                if (utilizatorOpt.HasValue)
                {
                    var utilizator = utilizatorOpt.Value;
                    var parolaDB = utilizator.Password;

                    if (_loggedClients.ContainsKey(username))
                    {
                        throw new AppException("Utilizator logat deja");
                    }
                    else
                    {
                        _loggedClients[username] = client;
                        return Optional<Oficiu>.Of(utilizator);
                    }
                }

                return Optional<Oficiu>.Empty();
            }
        }

        public void logout(string username)
        {
            lock (_lock)
            {
                _loggedClients.Remove(username);
            }
        }

        public List<CursaDTO> getAllCurse()
        {
            try
            {
                // Get all curse from cursaRepo
                var allCurse = cursaRepo.FindAll();

                // Transform curse into DTOs
                var cursaDTOs = allCurse.Select(cursa =>
                {
                    var locuriDisponibile = 18 - locuriRepo.findultimulLoc(cursa.Id);
                    return new CursaDTO(cursa.Id, cursa.Destinatie, cursa.Data, locuriDisponibile);
                }).ToList();

                return cursaDTOs;
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                // You might log the exception or throw a custom exception
                throw new AppException("Error occurred while getting all curse.");
            }
        }

        public List<CursaDTO> getCursaByDestinatieData(string destinatie, string data)
        {
            try
            {
                // Find Cursa by destinatie and data
                Optional<Cursa> optionalCursa = cursaRepo.FindByDestinatieData(destinatie, data);

                if (optionalCursa.HasValue)
                {
                    Cursa cursa = optionalCursa.Value;
                    int locuriDisponibile = 18 - locuriRepo.findultimulLoc(cursa.Id);
                    CursaDTO cursaDTO = new CursaDTO(cursa.Id, cursa.Destinatie, cursa.Data, locuriDisponibile);
                    List<CursaDTO> cursaDTOs = new List<CursaDTO> ();
                    cursaDTOs.Add(cursaDTO);
                    return cursaDTOs; // Return a list containing the DTO
                }
                else
                {
                    return new List<CursaDTO>(); // Return an empty list if no matching record was found
                }
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                // You might log the exception or throw a custom exception
                throw new AppException("Error occurred while getting cursa by destinatie and data.");
            }
        }

        public List<LocDTO> getLocuriDisponibile(long idCursa)
        {
            return locuriRepo.findByCursa(idCursa);


        }

        public RezervareDTO getRezervareById(long rezervareId)
        {
            Rezervare rezervare = rezervareRepo.FindOne(rezervareId).Value;

            if (rezervare != null)
            {
                return new RezervareDTO(rezervare.Id, rezervare.NumeClient, rezervare.NrLocuri, rezervare.Cursa.Id);
            }
            else
            {
                return null;
            }
        }

        public void makeReservation(long idCursa, String numeClient, int nrLocuri)
        {
            try
            {
                int locuriDisponibile = 18 - locuriRepo.findultimulLoc(idCursa);

                if (nrLocuri > locuriDisponibile)
                {
                    throw new AppException("Nu sunt suficiente locuri disponibile");
                }

                // Create a new reservation
                Optional<Cursa> cursa = cursaRepo.FindOne(idCursa);
                if (cursa.Value == null)
                {
                    throw new AppException("Cursa not found");
                }

                Rezervare rezervare = new Rezervare(numeClient, nrLocuri, cursa.Value);
                //rezervare.Id = rezervareRepo.FindAll().Last().Id + 1;



                // Save reservation
                Optional<Rezervare> rezervare1 = rezervareRepo.Save(rezervare);

                NotifyAllClients();
            }
            catch (ValidationException ex)
            {
                throw new AppException(ex.Message);
            }
            catch (AppException ex)
            {
                Console.WriteLine(ex.Message);
                // Handle other exceptions if needed
                throw new AppException("Error occurred while making reservation");
            }
        }

        private void NotifyAllClients()
        {
            Console.WriteLine("Notifying all clients ...");
            foreach (var client in _loggedClients.Values)
            {
                // client.ReservationUpdate();
                Task.Run(() => client.ReservationUpdate());
            }
        }


    }
}