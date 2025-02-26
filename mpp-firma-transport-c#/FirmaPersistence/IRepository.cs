
using System.Collections.Generic;
using FirmaModel1;
using Utils;

namespace FirmaPersistence2
{
    public interface IRepository<ID, E> where E : Entity<ID>
    {
        Optional<E> Save(E entitate);
        Optional<E> Update(E entitate);
        Optional<E> FindOne(ID id);
        Optional<E> Delete(ID id);
        List<E> FindAll();
    }
}
