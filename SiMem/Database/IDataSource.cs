using System.Collections.Generic;

namespace SiMem.DataModel
{
    /// <summary>
    /// Interface für alle wichtigen Datenbankabfragen
    /// </summary>
    /// <typeparam name="T">Tabelle der Datenbank</typeparam>
    public interface IDataSource<T>
    {
        T GetById(int id);
        List<T> GetAll(int id);
        void Update(T mem);
        void Delete(T mem);
        int Insert(T mem);
        int GetMax();
    }
}
