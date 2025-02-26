package org.example.Validator;

import org.example.Exception.ValidationException;

import org.example.LocuriRepository;
import org.example.Rezervare;

import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;


public class ValidatorRezervare {
    public void validate(Rezervare entity) throws ValidationException {
        Properties props=new Properties();
        try {
            props.load(new FileReader("bd.config"));
        } catch (IOException e) {
            System.out.println("Cannot find bd.config "+e);
        }
        LocuriRepository locuriRepository = new LocuriRepository(props);
        String msg = "";
        if (entity.getNumeClient().equals("")) {
            msg += "Numele clientului nu poate fi vid!";
        }
        if (entity.getNrLocuri() <= 0) {
            msg += "Numarul de locuri trebuie sa fie pozitiv!";
        }
        if(entity.getCursa() == null)
        {
            msg += "Cursa nu poate fi vida!";
        }
        //write a condition to check if the number of seats is greater than the number of seats available, maximum 18 seats
        //including the seats already reserved
        if(locuriRepository.findUltimulLoc(entity.getCursa().getId()) + entity.getNrLocuri() > 18)
        {
            msg += "Numarul de locuri este mai mare decat locurile disponibile!";
        }



        if (msg.length() > 0) {
            throw new ValidationException(msg);
        }
    }
}
