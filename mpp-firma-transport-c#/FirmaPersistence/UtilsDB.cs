
using System.Data.SQLite;
using log4net;

namespace TripPersistence
{
    public class UtilsDB
    {
        // private static Logger logger = LogManager.getLogger();
        private string dbUrl;
        private SQLiteConnection instance = null;

        public UtilsDB(string dbUrl)
        {
            this.dbUrl = dbUrl;
        }

        private SQLiteConnection GetNewConnection()
        {
            instance = new SQLiteConnection(dbUrl);
            return instance;
        }

        public SQLiteConnection GetConnection()
        {
            if (instance == null || instance.State == System.Data.ConnectionState.Closed)
            {
                instance = GetNewConnection();
            }
            return instance;
        }

    }
}
