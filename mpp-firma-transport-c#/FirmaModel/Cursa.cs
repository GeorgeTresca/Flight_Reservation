using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    internal class Cursa: Entity<long>
    {
        public string Destinatie { get; set; }
        public DateTime Data { get; set; }
        public int NrLocuri { get; set; }

        public Cursa(string destinatie, DateTime data)
        {
            this.Destinatie = destinatie;
            this.Data = data;
            
        }

        public override string ToString()
        {
            return "Cursa{" +
                   "id=" + Id +
                   ", destinatie='" + Destinatie + '\'' +
                   ", data=" + Data +
                   ", nrLocuri=" + NrLocuri +
                   '}';
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Cursa cursa = (Cursa)obj;
            return Id == cursa.Id;
        }
    }
}
