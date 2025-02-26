using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaModel
{
    [Serializable]
    public class Oficiu : Entity<long>
    {
        public string Nume { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public Oficiu(string nume, string username, string password)
        {
            Nume = nume;
            this.Username = username;
            this.Password = password;
        }

        public override string ToString()
        {
            return "FirmaTransport{" +
                   "id=" + Id +
                   ", nume='" + Nume + '\'' +
                   ", username='" + Username + '\'' +
                   ", password='" + Password + '\''+
                   '}';
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Oficiu firmaTransport = (Oficiu)obj;
            return Id == firmaTransport.Id;
        }
    }
}
