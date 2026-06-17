package com.eshop.catalog.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.SequenceGenerator;
import jakarta.persistence.Table;

@Entity
@Table(name = "CatalogType")
public class CatalogType {

    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "catalog_type_hilo")
    @SequenceGenerator(name = "catalog_type_hilo", sequenceName = "catalog_type_hilo", allocationSize = 10)
    private Integer id;

    @Column(name = "Type")
    private String type;

    public CatalogType() {
    }

    public CatalogType(Integer id, String type) {
        this.id = id;
        this.type = type;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }
}
