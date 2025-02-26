using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    internal class RezervareDTO
    {
        public long Id { get; set; }
        public string NumeClient { get; set; }
        public int NrLocuri { get; set; }
        public long IdCursa { get; set; }

        public RezervareDTO(long id, string numeClient, int nrLocuri, long idCursa)
        {
            Id = id;
            this.NumeClient = numeClient;
            this.NrLocuri = nrLocuri;
            this.IdCursa = idCursa;
        }
    }
}
