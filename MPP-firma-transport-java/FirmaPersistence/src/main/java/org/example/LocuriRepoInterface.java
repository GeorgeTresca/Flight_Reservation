package org.example;

public interface LocuriRepoInterface extends Repository<Long, Loc>{

    Integer findUltimulLoc(Long idCursa);
    Long lastId();

}
