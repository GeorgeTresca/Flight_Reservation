package org.example;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.example.JdbcUtils;

import java.io.FileReader;
import java.io.IOException;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.Properties;

public class RezervareRepository implements RezervareRepoInterface {
    private static final Logger logger = LogManager.getLogger();
    private final JdbcUtils dbUtils;



    public RezervareRepository(Properties props) {
        logger.info("Initializing CarsDBRepository with properties: {} ", props);
        dbUtils = new JdbcUtils(props);

    }
    @Override
    public List<Rezervare> findAll() {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        List<Rezervare> rezervari = new ArrayList<>();
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Rezervari");
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    long cursa = result.getLong("idCursa");
                    String numeClient = result.getString("nume");
                    int nrLocuri = result.getInt("nrLocuri");

                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    CursaRepository cursaRepository = new CursaRepository(props);
                    Optional<Cursa> cursa1 = cursaRepository.findOne(cursa);
                    Rezervare rezervare = new Rezervare(id,numeClient,nrLocuri,cursa1.get());
                    rezervare.setId(id);
                    rezervari.add(rezervare);
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(rezervari);
        }
        return rezervari;
    }

    @Override
    public Rezervare save(Rezervare entity) {
        logger.traceEntry("saving rezervare {}",entity);
        Connection con=dbUtils.getConnection();
        try(PreparedStatement preStmt=con.prepareStatement("insert into Rezervari (idCursa,nume,nrLocuri) values (?,?,?)")){
            preStmt.setLong(1,entity.getCursa().getId());
            preStmt.setString(2,entity.getNumeClient());
            preStmt.setInt(3,entity.getNrLocuri());
            int result=preStmt.executeUpdate();
            Properties props=new Properties();
            try {
                props.load(new FileReader("bd.config"));
            } catch (IOException e) {
                System.out.println("Cannot find bd.config "+e);
            }
            LocuriRepository locuriRepository = new LocuriRepository(props);
            for(int i=1;i<=entity.getNrLocuri();i++){
                locuriRepository.save(new Loc(locuriRepository.lastId()+1,locuriRepository.findUltimulLoc(entity.getCursa().getId())+1,entity));
            }
            logger.trace("Saved {} instances",result);
            System.out.println("Saved "+result+" instances");
        }catch (SQLException ex){
            logger.error(ex);
            System.out.println("Error DB "+ex);
        }
        logger.traceExit(entity);
        return entity;
    }

    @Override
    public Rezervare delete(Long aLong) {
        return null;
    }

    @Override
    public Rezervare update(Rezervare entity) {
        return null;
    }

    @Override
    public Optional<Rezervare> findOne(Long aLong) {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        Rezervare rezervare = null;
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Rezervari where id=?");
            preStmt.setLong(1, aLong);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    long cursa = result.getLong("idCursa");
                    String numeClient = result.getString("nume");
                    int nrLocuri = result.getInt("nrLocuri");
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    CursaRepository cursaRepository = new CursaRepository(props);
                    Optional<Cursa> cursa1 = cursaRepository.findOne(cursa);
                    rezervare = new Rezervare(id,numeClient,nrLocuri,cursa1.get());
                    rezervare.setId(id);
                }
                return Optional.ofNullable(rezervare);
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(rezervare);
            return Optional.ofNullable(rezervare);
        }
    }

    @Override
    public List<Rezervare> findByCursa(Long id) {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        List<Rezervare> rezervari = new ArrayList<>();
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Rezervari where idCursa=?");
            preStmt.setLong(1, id);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id1 = result.getLong("id");
                    long cursa = result.getLong("idCursa");
                    String numeClient = result.getString("nume");
                    int nrLocuri = result.getInt("nrLocuri");
                    Properties props=new Properties();
                    CursaRepository cursaRepository = new CursaRepository(props);
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    Optional<Cursa> cursa1 = cursaRepository.findOne(cursa);
                    Rezervare rezervare = new Rezervare(id1,numeClient,nrLocuri,cursa1.get());
                    rezervare.setId(id1);
                    rezervari.add(rezervare);
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(rezervari);
        }
        return rezervari;
    }

    public Long lastId(){
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        Long id = 0L;
        try {
            PreparedStatement preStmt = con.prepareStatement("select max(id) from Rezervari");
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    id = result.getLong("max(id)");
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(id);
        }
        return id;
    }
}
