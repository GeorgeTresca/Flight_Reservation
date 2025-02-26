package org.example;

import java.util.List;
import java.util.Optional;

public interface IFirmaServices {

    Optional<Oficiu> login(String username, String password, IFirmaObserver client) throws AppException;
    void logout(String username) throws AppException;
    List<CursaDTO> getAllCurse() throws AppException;

    List<CursaDTO> getCursaByDestinatieData(String destinatie, String data) throws AppException;

    List<LocDTO> getLocuriDisponibile(Long idCursa) throws AppException;
    void makeReservation(Long idCursa,String numeClient, Integer nrLocuri) throws AppException;

    RezervareDTO getRezervareById(Long rezervare) throws AppException;
}
