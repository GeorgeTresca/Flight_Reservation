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

public class LocuriRepository implements LocuriRepoInterface {
    private static final Logger logger = LogManager.getLogger();
    private final JdbcUtils dbUtils;

    //private RezervareRepository rezervareRepository;

    public LocuriRepository(Properties props) {
        logger.info("Initializing CarsDBRepository with properties: {} ", props);
        dbUtils = new JdbcUtils(props);


    }
    @Override
    public List<Loc> findAll() {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        List<Loc> locuri = new ArrayList<>();
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Locuri");
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    int loc= result.getInt("loc");
                    long rezervare = result.getLong("idRezervare");
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    RezervareRepository rezervareRepository = new RezervareRepository(props);
                    Optional<Rezervare> rezervare1 = rezervareRepository.findOne(rezervare);

                    locuri.add(new Loc(id,loc,rezervare1.get()));
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(locuri);
        }
        return locuri;
    }

    @Override
    public Loc save(Loc entity) {
        logger.traceEntry("saving loc {}",entity);
        Connection con=dbUtils.getConnection();
        try(PreparedStatement preStmt=con.prepareStatement("insert into Locuri(loc,idRezervare) values (?,?)")){
            preStmt.setInt(1,entity.getLoc());
            Properties props=new Properties();
            try {
                props.load(new FileReader("bd.config"));
            } catch (IOException e) {
                System.out.println("Cannot find bd.config "+e);
            }
            RezervareRepository rezervareRepository = new RezervareRepository(props);
            preStmt.setLong(2,rezervareRepository.lastId());
            preStmt.executeUpdate();
        }catch (SQLException ex){
            logger.error(ex);
            System.err.println("Error DB "+ex);
        }
        logger.traceExit(entity);
        return entity;
    }

    @Override
    public Loc delete(Long aLong) {
        return null;
    }

    @Override
    public Loc update(Loc entity) {
        return null;
    }

    @Override
    public Optional<Loc> findOne(Long aLong) {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        Loc loc = null;
        try (PreparedStatement preStmt = con.prepareStatement("select * from Locuri where id=?")) {
            preStmt.setLong(1, aLong);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    int loc1 = result.getInt("loc");
                    long rezervare = result.getLong("idRezervare");
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    RezervareRepository rezervareRepository = new RezervareRepository(props);
                    Optional<Rezervare> rezervare1 = rezervareRepository.findOne(rezervare);
                    loc = new Loc(id,loc1,rezervare1.get());
                    return Optional.ofNullable(loc);
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
        }
        return Optional.ofNullable(loc);
    }

    public List<Loc> findByRezervare(Long idRezervare){
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        List<Loc> locuri = new ArrayList<>();
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Locuri where idRezervare=?");
            preStmt.setLong(1, idRezervare);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    int loc= result.getInt("loc");
                    long rezervare = result.getLong("idRezervare");
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    RezervareRepository rezervareRepository = new RezervareRepository(props);
                    Optional<Rezervare> rezervare1 = rezervareRepository.findOne(rezervare);

                    locuri.add(new Loc(id,loc,rezervare1.get()));
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(locuri);
        }
        return locuri;
    }

    public List<LocDTO> findByCursa(Long idCursa){
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        List<LocDTO> locuri = new ArrayList<>();
        try {
            PreparedStatement preStmt = con.prepareStatement("select l.id,l.loc,l.idRezervare, r.nume from Locuri l join Rezervari r on l.idRezervare=r.id where r.idCursa=?");
            preStmt.setLong(1, idCursa);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    int loc= result.getInt("loc");
                    long rezervare = result.getLong("idRezervare");
                    String nume = result.getString("nume");
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    RezervareRepository rezervareRepository = new RezervareRepository(props);
                    Optional<Rezervare> rezervare1 = rezervareRepository.findOne(rezervare);
                    var locdto = new LocDTO(id,loc,rezervare1.get().getId());
                    locdto.setNumeClient(nume);
                    locuri.add(locdto);
                }
                //add new locuri that have not been reserved that have "-" in the name untill the number of locuri is 18
                int lastLoc = locuri.get(locuri.size()-1).getNrLoc();
                for(int i=lastLoc+1;i<=18;i++){
                    var locnou= new LocDTO((long) i,i,(long) i);
                    locnou.setNumeClient("-");
                    locuri.add(locnou);
                }

            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(locuri);
        }
        return locuri;
    }

    @Override
    public Integer findUltimulLoc(Long idCursa) {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        int loc = 0;
        try {
            PreparedStatement preStmt = con.prepareStatement("select max(loc) as loc from Locuri l join Rezervari r on l.idRezervare=r.id where r.idCursa=?");
            preStmt.setLong(1, idCursa);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    loc = result.getInt("loc");
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(loc);
        }
        return loc;
    }

    public Long lastId(){
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        long id = 0;
        try {
            PreparedStatement preStmt = con.prepareStatement("select max(id) as id from Locuri");
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    id = result.getLong("id");
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
