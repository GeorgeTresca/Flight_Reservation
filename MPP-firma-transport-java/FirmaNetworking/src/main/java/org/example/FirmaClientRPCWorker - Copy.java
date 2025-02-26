
package org.example;

import org.example.DTO.FilterDTO;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;

public class FirmaClientRPCWorker implements Runnable, IFirmaObserver {
    private IFirmaServices server;
    private Socket connection;
    private ObjectInputStream input;
    private ObjectOutputStream output;
    private volatile boolean connected;

    public FirmaClientRPCWorker(IFirmaServices server, Socket connection) {
        this.server = server;
        this.connection = connection;
        try {
            output = new ObjectOutputStream(connection.getOutputStream());
            output.flush();
            input = new ObjectInputStream(connection.getInputStream());
            connected = true;
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void run() {
        while (connected) {
            try {
                Object request = input.readObject();
                Response response = handleRequest((Request) request);
                if (response != null) {
                    sendResponse(response);
                }
            } catch (IOException | ClassNotFoundException e) {
                e.printStackTrace();
//                connected = false;
            }
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        try {
            input.close();
            output.close();
            connection.close();
        } catch (
                IOException e) {
            System.out.println("Error " + e);
        }
    }

    private static Response okResponse = new Response.Builder().type(ResponseType.OK).build();

    private Response handleRequest(Request request) {
        Response response = null;
        if (request.getType() == RequestType.LOGIN) {
            System.out.println("Login request ...");
            Oficiu userDTO = (Oficiu) request.getData();
            try {
                var optional = server.login(userDTO.getUsername(), userDTO.getParola(), this);
                if(optional.isPresent()) {
                    return new Response.Builder().type(ResponseType.OK).data(optional.get()).build();
                }
                else{
                    connected = false;
                    return new Response.Builder().type(ResponseType.ERROR).data("Username or password invalid!").build();
                }
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }
        }
        if (request.getType() == RequestType.LOGOUT) {
            System.out.println("Logout request ...");
            String username = (String) request.getData();
            try {
                server.logout(username);
                return okResponse;
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }
        }
        if (request.getType() == RequestType.REGISTER) {
            System.out.println("Register request ...");
            return null;
//            Utilizator userDTO = (Utilizator) request.getData();
//            try {
//                server.(userDTO.getUsername(), userDTO.getPassword());
//                return new Response.Builder().type(ResponseType.OK).build();
//            } catch (AppException e) {
//                connected = false;
//                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
//            }
        }
        if (request.getType() == RequestType.GET_Curse) {
            System.out.println("GetCurse request ...");
            try {
                return new Response.Builder().type(ResponseType.GET_Curse).data(server.getAllCurse()).build();
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }

        }
        if (request.getType() == RequestType.GET_Curse_FILTERED) {
            System.out.println("GetCurseFiltered request ...");
            var filter = (FilterDTO) request.getData();
            try{
                return new Response.Builder().type(ResponseType.GET_Curse_FILTERED).data(server.getCursaByDestinatieData(filter.getDestinatie(), filter.getData())).build();
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }
        }
        if (request.getType() == RequestType.RESERVATION) {
            System.out.println("Reservation request ...");
            RezervareDTO rezervare = (RezervareDTO) request.getData();
            try {
                server.makeReservation(rezervare.getCursaID(),rezervare.getNumeClient(), rezervare.getNrLocuri());
                return okResponse;
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }
        }

        if (request.getType() == RequestType.GET_LOCURI) {
            System.out.println("GetLocuri request ...");
            Long idCursa = (Long) request.getData();
            try {
                return new Response.Builder().type(ResponseType.GET_LOCURI).data(server.getLocuriDisponibile(idCursa)).build();
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }
        }
        if (request.getType()==RequestType.GET_REZERVARE){
            System.out.println("GetRezervare request ...");
            Long idRezervare = (Long) request.getData();
            try {
                return new Response.Builder().type(ResponseType.GET_REZERVARE).data(server.getRezervareById(idRezervare)).build();
            } catch (AppException e) {
                connected = false;
                return new Response.Builder().type(ResponseType.ERROR).data(e.getMessage()).build();
            }
        }


        return response;
    }

    private void sendResponse(Response response) {
        try {
            output.writeObject(response);
            output.flush();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
    @Override
    public void reservationUpdate() {
        System.out.println("o intrat aici");
        Response response = new Response.Builder().type(ResponseType.UPDATE).data(null).build();
        sendResponse(response);
    }
}
