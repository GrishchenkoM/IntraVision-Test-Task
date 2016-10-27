using System.Collections.Generic;
using Core.Entities;

namespace BusinessLogic.Repositories.Interfaces
{
    public interface IRepository<T> where T : EntityBase
    {
        T Get(int id);
        List<T> Get();
        T Create(T obj);
        T Update(T obj);
        int Delete(T obj);
    }
}
