using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    internal class Loc: Entity<long>
    {
        public int NrLoc { get; set; }
        public Rezervare Rezervare { get; set; }

        public Loc(int loc, Rezervare rezervare)
        {
            this.NrLoc = loc;
            this.Rezervare = rezervare;
        }

        public override string ToString()
        {
            return "Loc{" +
                   "id=" + Id +
                   ", loc=" + NrLoc +
                   ", rezervare=" + Rezervare +
                   '}';
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Loc loc = (Loc)obj;
            return Id == loc.Id;
        }
    }
}
