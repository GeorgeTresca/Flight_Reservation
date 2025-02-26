package org.example;

import java.util.List;

public interface RezervareRepoInterface extends Repository<Long, Rezervare> {

    public List<Rezervare> findByCursa(Long id);
    public Long lastId();
}
