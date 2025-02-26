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
using static log4net.Appender.FileAppender;

namespace FirmaPersistence2
{
    public class LocuriRepository : ILocuriRepository
    {
        private static readonly ILog MyLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UtilsDB _dbUtils;

        public LocuriRepository(UtilsDB db)
        {
            this._dbUtils = db;
        }

        public Optional<Loc> Delete(long id)
        {
            throw new NotImplementedException();
        }

        public List<Loc> FindAll()
        {
            MyLogger.Info($"Gasim toate Locurile");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Locuri";
                using (var reader = cmd.ExecuteReader())
                {
                    var listaLocuri = new List<Loc>();
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        int nrLoc = reader.GetInt32(1);
                        long rezervareId = reader.GetInt64(2);
                        RezervareRepository rezervareRepository = new RezervareRepository(_dbUtils);
                        Optional<Rezervare> rezervare = rezervareRepository.FindOne(rezervareId);
                        Loc loc = new Loc(nrLoc, rezervare.Value);
                        loc.Id = id;
                        listaLocuri.Add(loc);
                    }
                    connection.Close();
                    return listaLocuri;
                }
            }
        }

        public List<LocDTO> findByCursa(long id)
        {
            MyLogger.Info($"Cautare locuri dupa Cursa");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select l.id,l.loc,l.idRezervare, r.nume from Locuri l join Rezervari r on l.idRezervare=r.id where r.idCursa=@id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    var listaLocuri = new List<LocDTO>();
                    while (reader.Read())
                    {
                        long idLoc = reader.GetInt64(0);
                        int nrLoc = reader.GetInt32(1);
                        long rezervareId = reader.GetInt64(2);
                        string numeRezervare = reader.GetString(3);
                        LocDTO loc = new LocDTO( nrLoc, nrLoc, rezervareId);
                        loc.NumeClient = numeRezervare;
                        loc.Id = idLoc;
                        listaLocuri.Add(loc);
                    }
                    connection.Close();

                    //add more LocDTO untill 18 with name as -
                    for (int i = listaLocuri.Count+1; i <= 18; i++)
                    {
                        LocDTO loc = new LocDTO(0, i, 0);
                        loc.NumeClient = "-";
                        listaLocuri.Add(loc);
                    }

                    return listaLocuri;
                }
                
            }
        }

        public Optional<Loc> FindOne(long id)
        {
            MyLogger.Info($"Cautare loc dupa Id {id}");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Locuri where id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        long idLoc = reader.GetInt64(0);
                        int nrLoc = reader.GetInt32(1);
                        long rezervareId = reader.GetInt64(2);
                        connection.Close();
                        RezervareRepository rezervareRepository = new RezervareRepository(_dbUtils);
                        Optional<Rezervare> rezervare = rezervareRepository.FindOne(rezervareId);
                        Loc loc = new Loc(nrLoc, rezervare.Value);
                        loc.Id = idLoc;
                        
                        return Optional<Loc>.Of(loc);
                    }
                    else
                    {
                        MyLogger.Info($"Locul cu id-ul {id} nu a fost gasit");
                        
                        return Optional<Loc>.Empty();
                    }
                }
            }
        }

        public int findultimulLoc(long idCursa)
        {
            MyLogger.Info($"Cautare ultimul loc rezervat");
            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            int nrLoc = 0;
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select max(loc) as loc from Locuri l join Rezervari r on l.idRezervare=r.id where r.idCursa=@idCursa";
                cmd.Parameters.AddWithValue("@idCursa", idCursa);
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nrLoc = reader.GetInt32(0);
                        //connection.Close();
                        
                    }
                    
                }
            }
            return nrLoc;
        }

        public long lastId()
        {
            MyLogger.Info($"Returnare lastID");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            long id = 0;
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select max(id) as id from Locuri";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        id = reader.GetInt64(0);
                        connection.Close();
                    }
                }
            }
            return id;
        }

        public Optional<Loc> Save(Loc entitate)
        {
            MyLogger.Info($"Salvare loc {entitate}");
            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "insert into Locuri (loc, idRezervare) values (@nrLoc, @rezervareId)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@nrLoc", entitate.NrLoc);
                RezervareRepository rezervareRepository = new RezervareRepository(_dbUtils);
                entitate.Rezervare.Id = rezervareRepository.lastId();
                cmd.Parameters.AddWithValue("@rezervareId", entitate.Rezervare.Id);
                
                
                int rez = cmd.ExecuteNonQuery();
                if (rez == 1)
                {
                    MyLogger.Info($"Locul a fost salvat cu succes");
                    return Optional<Loc>.Of(new Loc(entitate.NrLoc, entitate.Rezervare));
                }
                else
                {
                    MyLogger.Info($"Locul nu a fost salvat");
                    return Optional<Loc>.Empty();
                }
            }
        }

        public Optional<Loc> Update(Loc entitate)
        {
            throw new NotImplementedException();
        }
    }
}
