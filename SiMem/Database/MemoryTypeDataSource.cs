using SiMem.Data;
using SiMem.DataModel;
using SiMem.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiMem.Database
{
    class MemoryTypeDataSource : IDataSource<MemoryType>
    {
        /// <summary>
        /// Standardkonstruktur
        /// </summary>
        /// <param name="db">Interface, um die Datenbankverbindung zu gewährleisten</param>
        /// <param name="memDS"></param>
        public MemoryTypeDataSource(IDataSource<Memory> memDS, IDBConnection db)
        {
            conn = db.getConnection();
            this.memDS = memDS;
        }
        /// <summary>
        /// MemoryDataSource, um auch die Datenbanktabelle zu benutzen
        /// </summary>
        private IDataSource<Memory> memDS;

        /// <summary>
        /// Datenbank-Connetion
        /// </summary>
        private SQLite.SQLiteConnection conn;


        /// <summary>
        /// Löscht eine MemoryType und alle Memories aus der Datenbank
        /// </summary>
        /// <param name="mem">Zu löschende Memory</param>
        public void Delete(MemoryType memType)
        {
            conn.Delete(memType);
        }
        /// <summary>
        /// Holt alle Memorytypeobjekte aus der Datenbank, die zu einer Gruppe gehören
        /// </summary>
        /// <returns>Liste von Memoryobjekte</returns>
        public List<MemoryType> GetAll()
        {
            List<MemoryType> lMemory = new List<MemoryType>();
            var query = conn.Table<MemoryType>();
            foreach (var item in query)
            {
                lMemory.Add(item);
            }
            return lMemory;
        }
        /// <summary>
        /// Sucht ein MemoryType aus der Datenbank anhand der Id
        /// </summary>
        /// <param name="id">Id des MemoryType-Objekts</param>
        /// <returns>Das Objekt aus der Datenbank</returns>
        public MemoryType GetById(int id)
        {
            return conn.Table<MemoryType>().Where(memType => memType.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Get all memories of one type
        /// </summary>
        /// <param name="type">MemoryType</param>
        /// <returns>List of memories</returns>
        public List<Memory> GetByType(int type)
        {
            return memDS.GetByType(type);
        }
        /// <summary>
        /// Sucht nacht der Maximal ID in der Memorytype Tabelle
        /// </summary>
        /// <returns>Die höchste Memory-ID</returns>
        public int GetMax()
        {
            //Tritt der Error InvalidOperationException auf, sind keine Einträge vorhanden und es wir 0 zurückgegeben.
            try
            {
                return conn.Table<MemoryType>().Max(x => x.Id);
            }
            catch (System.InvalidOperationException)
            {
                return 0;
            }
        }


        /// <summary>
        /// Fügt ein neues Objekt in die Datenbank ein
        /// </summary>
        /// <param name="mem">Ein neues Memory</param>
        /// <returns>Die Anzahl der hinzugefügten Zeilen</returns>
        public int Insert(MemoryType memType)
        {
            int result = -1;
            conn.RunInTransaction(() =>
            {
                result = conn.Insert(memType);
            });
            return result;
        }

        /// <summary>
        /// Aktualisiert das übergeben Memory Objekt in der Datenbank. Dies muss eine ID haben
        /// </summary>
        /// <param name="mem">Zu aktualisieren Objekt</param>

        public void Update(MemoryType memType)
        {

            MemoryType existingMemType = conn.Table<MemoryType>().Where(myMemType => myMemType.Id.Equals(memType.Id)).FirstOrDefault();
            if (existingMemType != null)
            {
                //Kopieren
                existingMemType = new MemoryType(memType);
                conn.RunInTransaction(() =>
                {
                    conn.Update(existingMemType);
                });
            }
        }

        List<MemoryType> IDataSource<MemoryType>.GetByType(int type)
        {
            throw new NotImplementedException();
        }

        public List<MemoryType> GetRecent(int count)
        {
            throw new NotImplementedException();
        }
    }
}
