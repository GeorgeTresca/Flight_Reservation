using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    internal class LocDTO
    {
        public long Id { get; set; }
        public int NrLoc { get; set; }
        public long IdRezervare { get; set; }

        public string NumeClient{ get; set; }

        public LocDTO(long id, int loc, long idRezervare)
        {
            Id = id;
            this.NrLoc = loc;
            this.IdRezervare = idRezervare;
        }
    }
}
