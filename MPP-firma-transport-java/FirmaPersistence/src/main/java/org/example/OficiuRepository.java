package org.example;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.example.OficiuRepoInterface;
import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.query.Query;

import org.example.Oficiu;

import java.util.List;
import java.util.Optional;

public class OficiuRepository implements OficiuRepoInterface {

    private static final Logger logger = LogManager.getLogger();
    private final SessionFactory sessionFactory;

    public OficiuRepository(SessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
    }

    @Override
    public Optional<Oficiu> findByUsernameAndPassword(String username, String password) {
        logger.traceEntry();
        try (Session session = sessionFactory.openSession()) {
            Query<Oficiu> query = session.createQuery("FROM Oficiu WHERE username = :username AND parola = :password", Oficiu.class);
            query.setParameter("username", username);
            query.setParameter("password", password);
            return query.uniqueResultOptional();
        } catch (Exception e) {
            logger.error("Error retrieving entity by username and password: {}", e.getMessage());
            return Optional.empty();
        }
    }


    public Optional<Oficiu> findByUsername(String username) {
        logger.traceEntry();
        try (Session session = sessionFactory.openSession()) {
            Query<Oficiu> query = session.createQuery("FROM Oficiu WHERE username = :username", Oficiu.class);
            query.setParameter("username", username);
            return query.uniqueResultOptional();
        } catch (Exception e) {
            logger.error("Error retrieving entity by username: {}", e.getMessage());
            return Optional.empty();
        }
    }

    @Override
    public List<Oficiu> findAll() {
        return null;
    }

    @Override
    public Oficiu save(Oficiu entity) {
        return null;
    }

    @Override
    public Oficiu delete(Long aLong) {
        return null;
    }

    @Override
    public Oficiu update(Oficiu entity) {
        return null;
    }

    @Override
    public Optional<Oficiu> findOne(Long aLong) {
        return Optional.empty();
    }

    // Implement other methods as needed

}