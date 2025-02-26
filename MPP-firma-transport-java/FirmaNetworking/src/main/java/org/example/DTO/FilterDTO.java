package org.example.DTO;
import java.io.Serializable;
import java.time.LocalDateTime;

public class FilterDTO implements Serializable {
    private String destinatie;
    private String data;

    public FilterDTO(String destinatie, String data) {
        this.destinatie = destinatie;
        this.data = data;
    }

    public String getDestinatie() {
        return destinatie;
    }

    public String getData() {
        return data;
    }

    public void setDestinatie(String destinatie) {
        this.destinatie = destinatie;
    }

    public void setData(String data) {
        this.data = data;
    }

    @Override
    public String toString() {
        return "FilterDTO{" +
                "destinatie='" + destinatie + '\'' +
                ", data='" + data + '\'' +
                '}';
    }
}
