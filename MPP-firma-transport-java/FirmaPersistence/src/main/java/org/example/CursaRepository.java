package org.example;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.example.JdbcUtils;

import java.io.FileReader;
import java.io.IOException;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.Properties;

public class CursaRepository implements CursaRepoInterface {
    private static final Logger logger = LogManager.getLogger();
    private final JdbcUtils dbUtils;


    public CursaRepository(Properties props) {
        logger.info("Initializing CarsDBRepository with properties: {} ", props);
        dbUtils = new JdbcUtils(props);

    }

    @Override
    public List<Cursa> findAll() {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        List<Cursa> curse = new ArrayList<>();
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Curse");
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    String destinatie = result.getString("destinatie");
                    String data = result.getString("data");
                    DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

                    LocalDateTime dateTime = LocalDateTime.parse(data, formatter);
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    LocuriRepository locuriRepository = new LocuriRepository(props);
                    Cursa cursa = new Cursa(id,destinatie,dateTime);
                    cursa.setId(id);
                    cursa.setNrLocuri(18-locuriRepository.findUltimulLoc(id));
                    curse.add(cursa);
                }
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(curse);
        }
        return curse;
    }

    @Override
    public Cursa save(Cursa entity) {
        return null;
    }

    @Override
    public Cursa delete(Long aLong) {
        return null;
    }

    @Override
    public Cursa update(Cursa entity) {
        return null;
    }

    @Override
    public Optional<Cursa> findOne(Long aLong) {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        Cursa cursa = null;
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Curse where id=?");
            preStmt.setLong(1, aLong);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    String destinatie = result.getString("destinatie");
                    String data = result.getString("data");
                    DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

                    LocalDateTime dateTime = LocalDateTime.parse(data, formatter);
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    LocuriRepository locuriRepository = new LocuriRepository(props);
                    cursa = new Cursa(id,destinatie,dateTime);
                    cursa.setId(id);
                    cursa.setNrLocuri(18-locuriRepository.findUltimulLoc(id));
                }
                return Optional.ofNullable(cursa);
            }

        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(cursa);
            return Optional.ofNullable(cursa);
        }
    }






    @Override
    public Optional<Cursa> findByDestinatieData(String destinatie, String data) {
        logger.traceEntry();
        Connection con = dbUtils.getConnection();
        Cursa cursa = null;
        try {
            PreparedStatement preStmt = con.prepareStatement("select * from Curse where destinatie=? and data=?");
            preStmt.setString(1, destinatie);
            preStmt.setString(2, data);
            try (ResultSet result = preStmt.executeQuery()) {
                while (result.next()) {
                    long id = result.getLong("id");
                    String destinatie1 = result.getString("destinatie");
                    String data1 = result.getString("data");
                    DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

                    LocalDateTime dateTime = LocalDateTime.parse(data, formatter);
                    Properties props=new Properties();
                    try {
                        props.load(new FileReader("bd.config"));
                    } catch (IOException e) {
                        System.out.println("Cannot find bd.config "+e);
                    }
                    LocuriRepository locuriRepository = new LocuriRepository(props);
                    cursa = new Cursa(id,destinatie,dateTime);
                    cursa.setId(id);
                    cursa.setNrLocuri(18-locuriRepository.findUltimulLoc(id));
                }
                return Optional.ofNullable(cursa);
            }
        } catch (Exception e) {
            logger.error(e);
            System.err.println("Error DB " + e);
            logger.traceExit(cursa);
            return Optional.ofNullable(cursa);
        }
    }
}
