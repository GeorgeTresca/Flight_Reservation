package org.example;

import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.input.MouseEvent;


import java.time.format.DateTimeFormatter;

public class UserController implements IFirmaObserver {



    @FXML
    private ObservableList<CursaDTO> curseModel;

    @FXML
    private ObservableList<LocDTO> locuriModel;

    //add the view2 elements
    @FXML
    private TableView<CursaDTO> CurseTableView;

    @FXML
    private TableColumn<CursaDTO, String> tableColumnDest;

    @FXML
    private TableColumn<CursaDTO, String> tableColumnData;

    @FXML
    private TableColumn<CursaDTO, Integer> tableColumnLocuri;

    @FXML
    private Button buttonSearch;

    @FXML
    private TextField textFieldDest;

    @FXML
    private Button buttonReserve;

    @FXML
    private TextField textFieldData;

    @FXML
    private TextField textFieldNume;

    @FXML
    private TextField textFieldNrLocuri;

    @FXML
    private TableView<LocDTO> locuriTableView;

    @FXML
    private TableColumn<LocDTO, String> tableColumnNume;

    @FXML
    private TableColumn<LocDTO, Integer> tableColumnLoc;

    private IFirmaServices server;



    public void Setcontroller(IFirmaServices server) {
        this.server = server;
        try {
            curseModel.setAll(server.getAllCurse());
        } catch (AppException e) {
            Action.showMessage(null, Alert.AlertType.WARNING, "Error", e.getMessage());

        }


    }
@FXML
    public void initialize() {
        curseModel= FXCollections.observableArrayList();
        tableColumnDest.setCellValueFactory(new PropertyValueFactory<>("destinatie"));
        tableColumnData.setCellValueFactory(new PropertyValueFactory<>("data"));
        tableColumnLocuri.setCellValueFactory(new PropertyValueFactory<>("locuriDisponibile"));
        CurseTableView.setItems(curseModel);

        initLocuriTable();

    }

    private void initLocuriTable() {
        locuriModel=FXCollections.observableArrayList();
        tableColumnNume.setCellValueFactory(new PropertyValueFactory<>("numeClient"));
//                (cellData -> {
//            LocDTO loc = cellData.getValue();
//            if (loc != null && loc.getRezervare() != null) {
//                //rerturn the name of the client knowing that loc.getRezervare() is a long
//                try{
//                    RezervareDTO rezervare = server.getRezervareById(loc.getRezervare());
//                    return new SimpleStringProperty(rezervare.getNumeClient());
//                }
//                catch (Exception e){
//                    return new SimpleStringProperty("-");
//                }
//
//
//
//            } else {
//                return new SimpleStringProperty("-");
//            }
//        });

        tableColumnLoc.setCellValueFactory(new PropertyValueFactory<>("nrLoc"));

        locuriTableView.setItems(locuriModel);
    }

    @Override
    public void reservationUpdate() {
    /*    curseModel.clear();
        try {
            curseModel.setAll(server.getAllCurse());
        } catch (AppException e) {
            Action.showMessage(null, Alert.AlertType.WARNING, "Error", e.getMessage());
        }*/
        //server = new FirmaServicesRPCProxy("localhost", 55555);
        Platform.runLater(() -> {
            try {
                curseModel.setAll(server.getAllCurse());
            } catch (AppException e) {
                Action.showMessage(null, Alert.AlertType.WARNING, "Error", e.getMessage());
            }
        });
    }

    public void seeoffers(ActionEvent actionEvent) {

        String destinatie=textFieldDest.getText();
        String data=textFieldData.getText();
        try{

        curseModel.setAll(server.getCursaByDestinatieData(destinatie,data).stream().toList());
        locuriModel.setAll(server.getLocuriDisponibile(curseModel.get(0).getId()));

        textFieldDest.setText("");
        textFieldData.setText("");
        }
        catch (Exception e){
            Action.showMessage(null, Alert.AlertType.WARNING,"Cautare esuata!","Nu exitsa nicio cursa cu destinatia si data introduse!");
        }
    }

    public void handle_save(MouseEvent mouseEvent) {
        try {
            String nume=textFieldNume.getText();
            Integer nrLocuri=Integer.parseInt(textFieldNrLocuri.getText());
            CursaDTO cursa=CurseTableView.getSelectionModel().getSelectedItem();
            //transform cursa dto in cursa
            Cursa cursa1=new Cursa(cursa.getId(),cursa.getDestinatie(),cursa.getData());

                server.makeReservation(cursa1.getId(),nume,nrLocuri);
          curseModel.setAll(server.getAllCurse());
          locuriModel.setAll(server.getLocuriDisponibile(cursa.getId()));

            textFieldNume.setText("");
            textFieldNrLocuri.setText("");
        }
        catch (Exception e){
            Action.showMessage(null, Alert.AlertType.WARNING,"Rezervare esuata!",e.getMessage());
        }
    }

    public void handle_select(MouseEvent mouseEvent) {
        CursaDTO cursa=CurseTableView.getSelectionModel().getSelectedItem();
        try {
            locuriModel.setAll(server.getLocuriDisponibile(cursa.getId()));
        } catch (AppException e) {
            Action.showMessage(null, Alert.AlertType.WARNING, "Error", e.getMessage());
        }


        textFieldDest.setText(cursa.getDestinatie());
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        String formattedDateTime = cursa.getData().format(formatter);

        textFieldData.setText(formattedDateTime);

    }

    public void logout(ActionEvent actionEvent) {
        //close the current window
        buttonSearch.getScene().getWindow().hide();
    }


}
