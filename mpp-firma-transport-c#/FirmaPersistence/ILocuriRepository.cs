using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirmaModel1;

namespace FirmaPersistence2
{
    public interface ILocuriRepository: IRepository<long, Loc>
    {
        int findultimulLoc(long id);
        List<LocDTO> findByCursa(long id);
        long lastId();
    }
}
