using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirmaModel1;

namespace FirmaPersistence2
{
    public interface IRezervareRepository: IRepository<long, Rezervare>
    {
        List<Rezervare> findByCursa(long id);
        long lastId();
    }
}
