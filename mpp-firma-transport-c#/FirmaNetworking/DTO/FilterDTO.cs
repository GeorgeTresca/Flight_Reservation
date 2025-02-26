using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaNetworking.DTO
{
    [Serializable]
    public class FilterDTO
    {
        public string destinatie { get; set; }
        public string data { get; set; }

        public FilterDTO(string destinatie, string data)
        {
            this.destinatie = destinatie;
            this.data = data;
        }
    }
}
