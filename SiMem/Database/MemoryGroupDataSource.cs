using SiMem.database;
using SiMem.DataModel;
using System.Collections.Generic;
using System;

namespace SiMem.Database
{
    public class MemoryGroupDataSource : IDataSource<MemoryGroup>

    {
        private SQLite.SQLiteConnection conn;
        public MemoryGroupDataSource(IDBConnection db)
        {
            conn = db.getConnection();

        }

        public void Delete(MemoryGroup mem)
        {
            conn.Delete(mem);
        }

        public List<MemoryGroup> GetAll(int id)
        {
            List<MemoryGroup> lMemory = new List<MemoryGroup>();
            var query = conn.Table<MemoryGroup>();
            foreach (var item in query)
            {
                lMemory.Add(item);
            }
            return lMemory;
        }

        public MemoryGroup GetById(int id)
        {
            return conn.Table<MemoryGroup>().Where(mem => mem.Id.Equals(id)).FirstOrDefault();

        }

        public int Insert(MemoryGroup mem)
        {
            int result = -1;
            conn.RunInTransaction(() =>
            {
                result = conn.Insert(mem);
            });
            return result;
        }

        public void Update(MemoryGroup mem)
        {
            MemoryGroup existingMem = conn.Table<MemoryGroup>().Where(myMem => myMem.Id.Equals(mem.Id)).FirstOrDefault();
            if (existingMem != null)
            {
                existingMem.Id = mem.Id;
                existingMem.Title = mem.Title;
                conn.RunInTransaction(() =>
                {
                    conn.Update(existingMem);
                });
            }
        }
    }
}
