using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirmaModel1;
using Utils;

namespace FirmaPersistence2
{
    public interface ICursaRepository:IRepository<long,Cursa>
    {
        Optional<Cursa> FindByDestinatieData(string destinatie, string data);
        
    }
}
