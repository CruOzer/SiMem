using SiMem.database;
using SiMem.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiMem.Database
{
    public class MemoryDataSource : IDataSource<Memory>
    {
        private SQLite.SQLiteConnection conn;
        public MemoryDataSource(IDBConnection db)
        {
            conn = db.getConnection();
        }

        
        public void Delete(Memory mem)
        {
            conn.Delete(mem);
        }

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

        public Memory GetById(int id)
        {
            return conn.Table<Memory>().Where(mem => mem.Id.Equals(id)).FirstOrDefault();
        }

        public int Insert(Memory mem)
        {
            int result = -1;
            conn.RunInTransaction(() =>
            {
                result = conn.Insert(mem);
            });
            return result;
        }

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
    }
}
