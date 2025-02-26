package org.example;

import java.io.Serializable;
import java.util.List;
import java.util.Optional;

public interface Repository<ID extends Serializable,E extends Entity<ID>> {
    List<E> findAll();
    E save(E entity);
    E delete(ID id);
    E update(E entity);
    Optional<E> findOne(ID id);

}
