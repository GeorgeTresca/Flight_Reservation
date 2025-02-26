package org.example;

import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.control.PasswordField;
import javafx.scene.control.TextField;
import javafx.scene.layout.AnchorPane;
import javafx.stage.Modality;
import javafx.stage.Stage;


import java.io.IOException;

public class LoginController  {


    private IFirmaServices server;
    @FXML
    TextField textField;
    @FXML
    PasswordField passwordField;

    Stage stage;

    public void setController(IFirmaServices server,Stage stage) {
        this.server = server;
        this.stage = stage;
    }

    public void login_handle() throws IOException {
        try {
            UserController userController = new UserController();
            var selected = server.login(textField.getText(), passwordField.getText(), userController);
            if (selected.isPresent()) {
                FXMLLoader fxmlLoader = new FXMLLoader();
                fxmlLoader.setLocation(getClass().getResource("views/view2.fxml"));
                AnchorPane root = fxmlLoader.load();
                Stage stage = new Stage();
                stage.initModality(Modality.WINDOW_MODAL);
                Scene scene = new Scene(root);
                stage.setScene(scene);
                 userController = fxmlLoader.getController();
                userController.Setcontroller(server);


                this.stage.close();

                stage.show();

            }

        }

        //cathc any error and print it
        catch (Exception e) {
            Alert alert = new Alert(Alert.AlertType.ERROR);
            alert.setTitle("Error");
            alert.setHeaderText("Login error");
            alert.setContentText(e.getMessage());
            alert.showAndWait();
        }

    }
}





