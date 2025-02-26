using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirmaModel1;
using Utils;

namespace FirmaPersistence2
{
    public interface IOficiuRepository:IRepository<long,Oficiu>
    {
        Optional<Oficiu> FindByUserPassword(string user,string password);

    }
}
