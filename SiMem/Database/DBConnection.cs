using SiMem.Data;
using SQLite;
using System;
using System.Threading.Tasks;

namespace SiMem.Database
{

    public interface IDBConnection
    {
        SQLiteConnection getConnection();
        bool onAppStart();
    }
    public class DBConnection : IDBConnection
    {
        private SQLiteConnection conn;


        /// <summary>
        /// Establish a connection to the database.
        /// </summary>
        /// <returns>SQLiteConnection (Singleton)</returns>
        public SQLiteConnection getConnection()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(DBStatics.DB_NAME);
            }

            return conn;
        }

        /// <summary>
        /// Connects to the database and creates tables if they do not exist
        /// </summary>
        /// <returns>True for okay and false for Error</returns>
        public bool onAppStart()
        {
            try
            {
                if (!CheckFileExists().Result)
                {
                    SQLiteConnection db = getConnection();
                    db.CreateTable<Memory>();
                }
                getConnection();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the database already exists
        /// </summary>
        /// <returns>True for file exists and false for database non existent</returns>
        private async Task<bool> CheckFileExists()
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(DBStatics.DB_NAME);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
