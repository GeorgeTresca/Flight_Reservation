using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmaNetworking
{
    [Serializable]
    public enum ResponseType
    {
        OK,
        ERROR,
        GET_Curse,
        GET_Curse_FILTERED,
        GET_Locuri,
        Get_Rezervare,
        UPDATE,
    }
}
