using FirmaModel1;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPersistence;
using Utils;

namespace FirmaPersistence2
{
    public class RezervareRepository : IRezervareRepository
    {
        private static readonly ILog MyLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UtilsDB _dbUtils;

        public RezervareRepository(UtilsDB db)
        {
            this._dbUtils = db;
        }

        public Optional<Rezervare> Delete(long id)
        {
            throw new NotImplementedException();
        }

        public List<Rezervare> FindAll()
        {
            MyLogger.Info("Cautare toate rezervarile");

            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Rezervari";
                using (var reader = cmd.ExecuteReader())
                {
                    var listaRezervari = new List<Rezervare>();
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);

                        string numeClient = reader.GetString(1);
                        int nrLocuri = reader.GetInt32(2);
                        long cursaId = reader.GetInt64(3);

                        
                        CursaRepository cursaRepository = new CursaRepository(_dbUtils);
                        Optional<Cursa> cursa = cursaRepository.FindOne(cursaId);
                        if (cursa.HasValue)
                        {
                            Rezervare rezervare = new Rezervare( numeClient, nrLocuri, cursa.Value);
                            rezervare.Id = id;
                            listaRezervari.Add(rezervare);
                        }
                    }
                    connection.Close();
                    return listaRezervari;
                }
            }

        }

        public List<Rezervare> findByCursa(long id)
        {
            MyLogger.Info($"Find Rezervari dupa Cursa");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Rezervari where idCursa=@cursaId";
                cmd.Parameters.AddWithValue("@cursaId", id);
                using (var reader = cmd.ExecuteReader())
                {
                    var listaRezervari = new List<Rezervare>();
                    while (reader.Read())
                    {
                        long idRezervare = reader.GetInt64(0);
                        string numeClient = reader.GetString(1);
                        int nrLocuri = reader.GetInt32(2);
                        long cursaId = reader.GetInt64(3);

                        CursaRepository cursaRepository = new CursaRepository(_dbUtils);
                        Optional<Cursa> cursa = cursaRepository.FindOne(cursaId);
                        if (cursa.HasValue)
                        {
                            Rezervare rezervare = new Rezervare(numeClient, nrLocuri, cursa.Value);
                            rezervare.Id = idRezervare;
                            listaRezervari.Add(rezervare);
                        }
                    }
                    connection.Close();
                    return listaRezervari;
                }
            }


        }

        public Optional<Rezervare> FindOne(long id)
        {
            MyLogger.Info($"Cautare Rezervare dupa ID");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Rezervari where id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        long idRezervare = reader.GetInt64(0);
                        string numeClient = reader.GetString(1);
                        int nrLocuri = reader.GetInt32(2);
                        long cursaId = reader.GetInt64(3);
                        connection.Close();
                        CursaRepository cursaRepository = new CursaRepository(_dbUtils);
                        Optional<Cursa> cursa = cursaRepository.FindOne(cursaId);
                        if (cursa.HasValue)
                        {
                            Rezervare rezervare = new Rezervare(numeClient, nrLocuri, cursa.Value);
                            rezervare.Id = idRezervare;
                            
                            return Optional<Rezervare>.Of(rezervare);
                        }

                        else return Optional<Rezervare>.Empty();
                    }
                    else return Optional<Rezervare>.Empty();


                }
            }
        }

        public long lastId()
        {
            MyLogger.Info($"Gasire ultimul Id");
            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select max(id) from Rezervari";
                using (var reader = cmd.ExecuteReader())
                {
                    long id = 0;
                    while (reader.Read())
                    {
                        id = reader.GetInt64(0);
                    }
                    //connection.Close();
                    return id;
                }
            }
        }

        public Optional<Rezervare> Save(Rezervare entitate)
        {
            MyLogger.Info($"Salvare excursie {entitate}");
            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "insert into Rezervari (idCursa,nume,nrLocuri) values (@idCursa,@nume,@nrLocuri)";
                cmd.Prepare();
                long cursaId=entitate.Cursa.Id;

                cmd.Parameters.AddWithValue("@idCursa", cursaId);
                cmd.Parameters.AddWithValue("@nume", entitate.NumeClient);
                cmd.Parameters.AddWithValue("@nrLocuri", entitate.NrLocuri);
                cmd.ExecuteNonQuery();
                //save in the database Locuri for the Rezervare
                LocuriRepository locuriRepository = new LocuriRepository(_dbUtils);
                for(int i=1; i<=entitate.NrLocuri;i++)
                locuriRepository.Save(new Loc( locuriRepository.findultimulLoc(cursaId) + 1, entitate));
                connection.Close();
                return Optional<Rezervare>.Of(entitate);
            }
        }

        public Optional<Rezervare> Update(Rezervare entitate)
        {
            throw new NotImplementedException();
        }
    }
}
