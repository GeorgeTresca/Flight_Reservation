
package org.example;

import org.example.Utils.AbstractServer;
import org.example.Utils.RPCConcurrentServer;
import org.hibernate.SessionFactory;

import java.io.FileReader;
import java.util.Properties;
import org.hibernate.SessionFactory;
import org.hibernate.boot.MetadataSources;
import org.hibernate.boot.registry.StandardServiceRegistry;
import org.hibernate.boot.registry.StandardServiceRegistryBuilder;

public class StartRPCServer {
    private static int defaultPort = 55555;
    private static SessionFactory sessionFactory;

    private static void setUp() {
        final StandardServiceRegistry registry = new StandardServiceRegistryBuilder()
                .configure()
                .build();
        try {
            sessionFactory = new MetadataSources(registry)
                    .buildMetadata()
                    .buildSessionFactory();
        } catch (Exception e) {
            e.printStackTrace();
            StandardServiceRegistryBuilder.destroy(registry);
        }
    }

    private static void tearDown() {
        if (sessionFactory != null) {
            sessionFactory.close();
        }
    }

    public static void main(String[] args) {





        Properties properties = new Properties();
        try {
            properties.load(new FileReader("bd.config"));
        } catch (Exception e) {
            throw new RuntimeException("Cannot find bd.config " + e);
        }
        Properties propertiesServer = new Properties();
        try {
            propertiesServer.load(new FileReader("server.properties"));
        } catch (Exception e) {
            throw new RuntimeException("Cannot find server.properties " + e);
        }

        setUp();

        OficiuRepository oficiuRepo=new OficiuRepository(sessionFactory);

        LocuriRepository locuriRepo=new LocuriRepository(properties);
        CursaRepository cursaRepo=new CursaRepository(properties);
        RezervareRepository rezervareRepo=new RezervareRepository(properties);
        IFirmaServices tripServices = new FirmaServices(oficiuRepo, locuriRepo, cursaRepo, rezervareRepo);

        String pass=propertiesServer.getProperty("Port");
        var intPass = Integer.parseInt(pass);
        System.out.println("Starting server on port " + intPass);
        AbstractServer server = new RPCConcurrentServer(intPass, tripServices);
        System.out.println("Oficii:");
        oficiuRepo.findByUsernameAndPassword("oficiu1","of1").ifPresent(System.out::println);
        try{
            server.start();
        } catch (Exception e) {
            System.err.println("Error starting the server " + e.getMessage());
        }

        finally {
            System.out.println("Stopping server ..." );
            System.out.println("-------------------------");
            tearDown();
        }

        //print all oficii
        //print a message

    }
}
