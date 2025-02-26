using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirmaModel1;
using Utils;



namespace FirmaService
{
    public interface IFirmaServices
    {
        Optional<Oficiu> login(String username, String password, IFirmaObserver client) ;
        void logout(String username) ;
        List<CursaDTO> getAllCurse();

        List<CursaDTO> getCursaByDestinatieData(String destinatie, String data) ;

        List<LocDTO> getLocuriDisponibile(long idCursa) ;
        void makeReservation(long idCursa, String numeClient, int nrLocuri) ;

        RezervareDTO getRezervareById(long rezervare) ;


    }
}
