package org.example;

import java.util.Optional;


public interface OficiuRepoInterface extends Repository<Long, Oficiu> {


    //Optional<Oficiu> findByUsername(String username);
    Optional<Oficiu> findByUsernameAndPassword(String username, String password);

}
