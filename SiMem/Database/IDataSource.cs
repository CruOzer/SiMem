using System.Collections.Generic;

namespace SiMem.DataModel
{
    public interface IDataSource<T>
    {
        T GetById(int id);
        List<T> GetAll(int id);
        void Update(T mem);
        void Delete(T mem);
        int Insert(T mem);
    }
}
