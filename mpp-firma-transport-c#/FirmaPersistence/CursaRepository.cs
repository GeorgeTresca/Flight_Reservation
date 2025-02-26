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
    public class CursaRepository : ICursaRepository
    {
        private static readonly ILog MyLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UtilsDB _dbUtils;


        public CursaRepository(UtilsDB db)
        {
            this._dbUtils = db;

        }
        public Optional<Cursa> Delete(long id)
        {
            throw new NotImplementedException();
        }

        public List<Cursa> FindAll()
        {
            MyLogger.Info("Cautare toate cursele");

            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Curse ";
                using (var reader = cmd.ExecuteReader())
                {
                    var listaCurse = new List<Cursa>();
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        string destinatie = reader.GetString(1);
                        string data = reader.GetString(2);
                        DateTime dateTime = DateTime.ParseExact(data, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        Cursa cursa = new Cursa(destinatie, dateTime);
                        cursa.Id = id;
                        //create an instance of locuriRepository
                        LocuriRepository locuriRepository = new LocuriRepository(_dbUtils);
                        //set the nrLocuri of the cursa object as 18-locuriRepository.findUltimulLocuri(cursa.getId())
                        //connection.Close();
                        cursa.NrLocuri = 18 - locuriRepository.findultimulLoc(cursa.Id);
                        listaCurse.Add(cursa);
                    }
                    connection.Close();
                    return listaCurse;
                }
            }
        }

        public Optional<Cursa> FindByDestinatieData(string destinatie, string data)
        {
            MyLogger.Info(
                $"Cautare excursii cu dedstinatia {destinatie} si data {data}");

            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText =
                    "select * from Curse where destinatie=@destinatie and data=@data";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@destinatie", destinatie);
                cmd.Parameters.AddWithValue("@data", data);
                using (var reader = cmd.ExecuteReader())
                {
                    var listaCurse = new List<Cursa>();
                    if (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        string destinatie1 = reader.GetString(1);
                        string data1 = reader.GetString(2);
                        //connection.Close();
                        DateTime dateTime = DateTime.ParseExact(data1, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        Cursa cursa = new Cursa( destinatie1, dateTime);
                        cursa.Id = id;
                        LocuriRepository locuriRepository = new LocuriRepository(_dbUtils);
                        //set the nrLocuri of the cursa object as 18-locuriRepository.findUltimulLocuri(cursa.getId())
                        cursa.NrLocuri = 18 - locuriRepository.findultimulLoc(cursa.Id);
                        cursa.NrLocuri = 18 - locuriRepository.findultimulLoc(cursa.Id);
                        connection.Close();
                        return Optional<Cursa>.Of(cursa);
                    }

                    else  { connection.Close(); return Optional<Cursa>.Empty(); }

                    
                }
            }
        }


        public Optional<Cursa> FindOne(long id)
        {
            MyLogger.Info("Cautare o cursa");

            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Curse where id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        //var idFirmaTransport = reader.GetInt32(5);
                        long id1 = reader.GetInt64(0);
                        string destinatie = reader.GetString(1);
                        string data = reader.GetString(2);
                        //connection.Close();
                        DateTime dateTime = DateTime.ParseExact(data, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        Cursa cursa = new Cursa(destinatie, dateTime);
                        cursa.Id = id1;
                        return Optional<Cursa>.Of(cursa);

                    }
                    else
                    {
                        MyLogger.Info($"CUrsa cu id-ul {id} nu a fost gasita");
                        return Optional<Cursa>.Empty();
                    }
                }
            }
        }

    


        public Optional<Cursa> Save(Cursa entitate)
        {
            throw new NotImplementedException();
        }

        public Optional<Cursa> Update(Cursa entitate)
        {
            throw new NotImplementedException();
        }
    }
}
