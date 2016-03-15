using SiMem.database;
using SiMem.Data;
using System.Collections.Generic;
using SiMem.DataModel;
using System.Diagnostics;
using System.Linq;

namespace SiMem.Database
{
    /// <summary>
    /// Klasse für Kommunikation mit der Memory-Tabelle der Datenbank. Implementier IDataSource, die alle wichtigen Funktionen bereitstellt.
    /// </summary>
    public class MemoryDataSource : IDataSource<Memory>
    {
        /// <summary>
        /// Datenbank-Connetion
        /// </summary>
        private SQLite.SQLiteConnection conn;
        /// <summary>
        /// Standardkonstruktur
        /// </summary>
        /// <param name="db">Interface, um die Datenbankverbindung zu gewährleisten</param>
        public MemoryDataSource(IDBConnection db)
        {
            conn = db.getConnection();
        }

        /// <summary>
        /// Löscht eine Memory aus der Datenbank
        /// </summary>
        /// <param name="mem">Zu löschende Memory</param>
        public void Delete(Memory mem)
        {
            conn.Delete(mem);
        }

        /// <summary>
        /// Holt alle Memoryobjekte aus der Datenbank, die zu einer Gruppe gehören
        /// </summary>
        /// <param name="id">Die Id der gemeinsamen Gruppe</param>
        /// <returns>Liste von Memoryobjekte</returns>
        public List<Memory> GetAll(int id)
        {
            List<Memory> lMemory = new List<Memory>();
            var query = conn.Table<Memory>().Where(mem => mem.GroupId.Equals(id));
            foreach (var item in query)
            {
                lMemory.Add(item);
            }
            return lMemory;
        }

        /// <summary>
        /// Sucht ein Memory Objekt aus der Datenbank anhand der Id
        /// </summary>
        /// <param name="id">Id des Memory-Objekts</param>
        /// <returns>Das Objekt aus der Datenbank</returns>
        public Memory GetById(int id)
        {
            return conn.Table<Memory>().Where(mem => mem.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Fügt ein neues Objekt in die Datenbank ein
        /// </summary>
        /// <param name="mem">Ein neues Memory</param>
        /// <returns>Die Anzahl der hinzugefügten Zeilen</returns>
        public int Insert(Memory mem)
        {
            int result = -1;
            conn.RunInTransaction(() =>
            {
                result = conn.Insert(mem);
            });
            return result;
        }

        /// <summary>
        /// Aktualisiert das übergeben Memory Objekt in der Datenbank. Dies muss eine ID haben
        /// </summary>
        /// <param name="mem">Zu aktualisieren Objekt</param>
        public void Update(Memory mem)
        {
            Memory existingMem = conn.Table<Memory>().Where(myMem => myMem.Id.Equals(mem.Id)).FirstOrDefault();
            if (existingMem != null)
            {
                existingMem.Datum = mem.Datum;
                existingMem.Id= mem.Id;
                existingMem.Text = mem.Text;
                existingMem.Title = mem.Title;
                conn.RunInTransaction(() =>
                {
                    conn.Update(existingMem);
                });
            }
        }
        /// <summary>
        /// Sucht nacht der Maximal ID in der Memory Tabelle
        /// </summary>
        /// <returns>Die höchste Memory-ID</returns>
        public int GetMax()
        {
            //Tritt der Error InvalidOperationException auf, sind keine Einträge vorhanden und es wir 0 zurückgegeben.
            try
            {
                return conn.Table<Memory>().Max(x => x.Id);
            }
            catch (System.InvalidOperationException e)
            {
                return 0;
            }
            
        }
    }
}
