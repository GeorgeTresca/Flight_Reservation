using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    internal class CursaDTO
    {
        public long Id { get; set; }
        public string Destinatie { get; set; }
        public DateTime Data { get; set; }

        public int LocuriDisponibile { get; set; }

        public CursaDTO(long id, string destinatie, DateTime data, int locuriDisponibile)
        {
            Id = id;
            this.Destinatie = destinatie;
            this.Data = data;
            this.LocuriDisponibile = locuriDisponibile;
        }

    }
}
