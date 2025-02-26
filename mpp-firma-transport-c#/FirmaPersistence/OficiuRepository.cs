using System.Collections.Generic;
using System.Data.SQLite;
using FirmaModel1;
using log4net;
using TripPersistence;
using Utils;

namespace FirmaPersistence2
{
    public class OficiuRepository : IOficiuRepository
    {

        private UtilsDB _dbUtils;
        private static readonly ILog MyLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public OficiuRepository(UtilsDB db)
        {
            this._dbUtils = db;
        }
        public Optional<Oficiu> Delete(long id)
        {
            return null;
        }

        public List<Oficiu> FindAll()
        {

            MyLogger.Info("Find all oficii");
            SQLiteConnection connection = _dbUtils.GetConnection();
            connection.Open();
            List<Oficiu> utilizatori = new List<Oficiu>();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Oficii FROM utilizatori";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new Oficiu(reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        user.Id = reader.GetInt32(0);
                        utilizatori.Add(user);
                    }
                }
            }
            connection.Close();
            return utilizatori;

        }

        public Optional<Oficiu> FindByUserPassword(string user, string password)
        {
            MyLogger.Info($"Cautare utilizator cu username {user}");
            SQLiteConnection connection = _dbUtils.GetConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
            connection.Open();
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "select * from Oficii where username = @username and parola=@password";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", user);
                cmd.Parameters.AddWithValue("@password", password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MyLogger.Info($"Utilizatorul cu username {user} a fost gasit");
                        var user1 = new Oficiu(reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        user1.Id = reader.GetInt32(0);
                        connection.Close();
                        return Optional<Oficiu>.Of(user1);
                    }
                    else
                    {
                        MyLogger.Info($"Utilizatorul cu username {user} nu a fost gasit");
                        connection.Close();
                        return Optional<Oficiu>.Empty();
                    }
                }

            }
        }

        public Optional<Oficiu> FindOne(long id)
        {
            throw new NotImplementedException();
        }

        public Optional<Oficiu> Save(Oficiu entitate)
        {
            throw new NotImplementedException();
        }

        public Optional<Oficiu> Update(Oficiu entitate)
        {
            throw new NotImplementedException();
        }
    }
}
