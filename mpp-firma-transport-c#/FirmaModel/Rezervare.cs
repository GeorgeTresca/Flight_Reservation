using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    internal class Rezervare: Entity<long>
    {

        public Cursa Cursa { get; set; }
        public string NumeClient { get; set; }
        public int NrLocuri { get; set; }

        public Rezervare( string numeClient, int nrLocuri, Cursa idCursa)
        {
            this.Cursa = idCursa;
            this.NumeClient = numeClient;
            this.NrLocuri = nrLocuri;
        }

        public override string ToString()
        {
            return "Rezervare{" +
                   "id=" + Id +
                   ", idCursa=" + Cursa +
                   ", numeClient='" + NumeClient + '\'' +
                   ", nrLocuri=" + NrLocuri +
                   '}';
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Rezervare rezervare = (Rezervare)obj;
            return Id == rezervare.Id;
        }
    }
}
