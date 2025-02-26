using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaNetworking
{
    [Serializable]
    public enum RequestType
    {
        LOGIN,
        LOGOUT,
        REGISTER,
        GET_Curse,
        GET_Curse_FILTERED,
        GET_Locuri,
        Get_Rezervare,
        RESERVATION
    }
}
