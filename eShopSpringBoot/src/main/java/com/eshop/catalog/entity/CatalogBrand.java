package com.eshop.catalog.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.SequenceGenerator;
import jakarta.persistence.Table;

@Entity
@Table(name = "CatalogBrand")
public class CatalogBrand {

    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "catalog_brand_hilo")
    @SequenceGenerator(name = "catalog_brand_hilo", sequenceName = "catalog_brand_hilo", allocationSize = 10)
    private Integer id;

    @Column(name = "Brand")
    private String brand;

    public CatalogBrand() {
    }

    public CatalogBrand(Integer id, String brand) {
        this.id = id;
        this.brand = brand;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getBrand() {
        return brand;
    }

    public void setBrand(String brand) {
        this.brand = brand;
    }
}
