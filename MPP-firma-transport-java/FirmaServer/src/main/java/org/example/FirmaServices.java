package org.example;

import org.example.Exception.ValidationException;
import org.example.Validator.ValidatorRezervare;

import javax.crypto.Cipher;
import javax.crypto.spec.SecretKeySpec;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class FirmaServices implements IFirmaServices{

    private OficiuRepository oficiuRepo;
    private LocuriRepository locuriRepo;
    private CursaRepository cursaRepo;
    private RezervareRepository rezervareRepo;
    private Map<String,IFirmaObserver> loggedClients;

    private final int defaultThreadsNo=5;

    public FirmaServices(OficiuRepository oficiuRepo, LocuriRepository locuriRepo, CursaRepository cursaRepo, RezervareRepository rezervareRepo) {
        this.oficiuRepo = oficiuRepo;
        this.locuriRepo = locuriRepo;
        this.cursaRepo = cursaRepo;
        this.rezervareRepo = rezervareRepo;
        this.loggedClients = new ConcurrentHashMap<>();
    }

    @Override
    public synchronized Optional<Oficiu> login(String username, String password, IFirmaObserver client) throws AppException {
        //get oficiuDTO by username and password
        Optional<Oficiu> utilizatorOpt = oficiuRepo.findByUsernameAndPassword(username, password);



        if (utilizatorOpt.isPresent()) {
            //print the oficiu
            System.out.println(utilizatorOpt.get());
            Oficiu utilizator = utilizatorOpt.get();


                if (loggedClients.get(username) != null)
                    throw new AppException("User already logged in");

                loggedClients.put(username, client);
                return Optional.of(utilizator);

        }
        return Optional.empty();
    }

    @Override
    public synchronized void logout(String username) throws AppException {
        IFirmaObserver localClient=loggedClients.remove(username);
        if (localClient==null)
            throw new AppException("User "+username+" is not logged in.");
    }

    private String encrypt(String strToEncrypt, String secret) throws AppException {
        SecretKeySpec secretKey = new SecretKeySpec(secret.getBytes(), "AES");
        try {
            Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5Padding");
            cipher.init(Cipher.ENCRYPT_MODE, secretKey);
            byte[] encryptedBytes = cipher.doFinal(strToEncrypt.getBytes());
            return Base64.getEncoder().encodeToString(encryptedBytes);
        }
        catch (Exception e){
            throw new AppException("Invalid password");
        }
    }

    private String decrypt(String strToDecrypt, String secret) throws AppException {
        SecretKeySpec secretKey = new SecretKeySpec(secret.getBytes(), "AES");
        try {
            Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5Padding");
            cipher.init(Cipher.DECRYPT_MODE, secretKey);
            byte[] decryptedBytes = cipher.doFinal(Base64.getDecoder().decode(strToDecrypt));
            return new String(decryptedBytes);
        }
        catch (Exception e){
            throw new AppException("Invalid password");
        }
    }

    @Override
    public List<CursaDTO> getAllCurse() throws AppException {
        //get all curse and transform them into DTOs
        return cursaRepo.findAll().stream()
                .map(cursa -> {
                    var locuriDisponibile = 18-locuriRepo.findUltimulLoc(cursa.getId());
                    return new CursaDTO(cursa.getId(), cursa.getDestinatie(), cursa.getData(), locuriDisponibile);
                })
                .toList();
    }

    @Override
    public List<CursaDTO> getCursaByDestinatieData(String destinatie,String data) throws AppException {
        Optional<Cursa> optionalCursa = cursaRepo.findByDestinatieData(destinatie, data);

        if (optionalCursa.isPresent()) {
            Cursa cursa = optionalCursa.get();
            int locuriDisponibile = 18 - locuriRepo.findUltimulLoc(cursa.getId());
            CursaDTO cursaDTO = new CursaDTO(cursa.getId(), cursa.getDestinatie(), cursa.getData(), locuriDisponibile);
            return List.of(cursaDTO); // Return a list containing the DTO
        } else {
            return Collections.emptyList(); // Return an empty list if no matching record was found
        }
    }

    @Override
    public List<LocDTO> getLocuriDisponibile(Long idCursa) throws AppException {
        return locuriRepo.findByCursa(idCursa);
    }

    public RezervareDTO getRezervareById(Long rezervare) {
        return rezervareRepo.findOne(rezervare).map(rezervare1 -> new RezervareDTO(rezervare1.getId(), rezervare1.getNumeClient(), rezervare1.getNrLocuri(), rezervare1.getCursa().getId())).orElse(null);
    }


    @Override
    public void makeReservation(Long idCursa, String numeClient, Integer nrLocuri) throws AppException {

        int locuriDisponibile = 18 - locuriRepo.findUltimulLoc(idCursa);
        if (nrLocuri > locuriDisponibile) {
            throw new AppException("Nu sunt suficiente locuri disponibile");
        }

        // Create a new reservation
        Rezervare rezervare = new Rezervare(null, numeClient, nrLocuri, cursaRepo.findOne(idCursa).get());
        ValidatorRezervare validatorRezervare = new ValidatorRezervare();
        try {
            validatorRezervare.validate(rezervare);
            notifyClients();

        } catch (ValidationException e) {
            throw new AppException(e.getMessage());
        }

        rezervare = rezervareRepo.save(rezervare);

        notifyClients();
    }

    private void notifyClients(){
        ExecutorService executor = Executors.newFixedThreadPool(defaultThreadsNo);
        for(var client : loggedClients.values()){
            if(client == null)
                continue;
            executor.execute(client::reservationUpdate);


        }
    }
}
