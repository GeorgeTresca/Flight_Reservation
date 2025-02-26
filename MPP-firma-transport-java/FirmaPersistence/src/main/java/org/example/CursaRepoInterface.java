package org.example;

import java.util.Optional;

public interface CursaRepoInterface extends Repository<Long, Cursa>{

    Optional<Cursa> findByDestinatieData(String destinatie, String data);


}
