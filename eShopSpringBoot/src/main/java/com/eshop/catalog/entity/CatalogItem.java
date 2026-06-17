package com.eshop.catalog.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.SequenceGenerator;
import jakarta.persistence.Table;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;

import java.math.BigDecimal;

@Entity
@Table(name = "CatalogItem")
public class CatalogItem {

    public static final String DEFAULT_PICTURE_NAME = "dummy.png";

    @Id
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "catalog_hilo")
    @SequenceGenerator(name = "catalog_hilo", sequenceName = "catalog_hilo", allocationSize = 10)
    private Integer id;

    @NotNull
    @Column(name = "Name", nullable = false)
    private String name;

    @Column(name = "Description")
    private String description;

    @Column(name = "Price", precision = 18, scale = 2)
    private BigDecimal price;

    @Column(name = "PictureFileName")
    private String pictureFileName = DEFAULT_PICTURE_NAME;

    @Column(name = "PictureUri")
    private String pictureUri;

    @Column(name = "CatalogTypeId", insertable = false, updatable = false)
    private int catalogTypeId;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "CatalogTypeId")
    private CatalogType catalogType;

    @Column(name = "CatalogBrandId", insertable = false, updatable = false)
    private int catalogBrandId;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "CatalogBrandId")
    private CatalogBrand catalogBrand;

    @Min(0)
    @Max(10000000)
    @Column(name = "AvailableStock")
    private int availableStock;

    @Column(name = "RestockThreshold")
    private int restockThreshold;

    @Column(name = "MaxStockThreshold")
    private int maxStockThreshold;

    @Column(name = "OnReorder")
    private boolean onReorder;

    public CatalogItem() {
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public BigDecimal getPrice() {
        return price;
    }

    public void setPrice(BigDecimal price) {
        this.price = price;
    }

    public String getPictureFileName() {
        return pictureFileName;
    }

    public void setPictureFileName(String pictureFileName) {
        this.pictureFileName = pictureFileName;
    }

    public String getPictureUri() {
        return pictureUri;
    }

    public void setPictureUri(String pictureUri) {
        this.pictureUri = pictureUri;
    }

    public int getCatalogTypeId() {
        return catalogTypeId;
    }

    public void setCatalogTypeId(int catalogTypeId) {
        this.catalogTypeId = catalogTypeId;
    }

    public CatalogType getCatalogType() {
        return catalogType;
    }

    public void setCatalogType(CatalogType catalogType) {
        this.catalogType = catalogType;
    }

    public int getCatalogBrandId() {
        return catalogBrandId;
    }

    public void setCatalogBrandId(int catalogBrandId) {
        this.catalogBrandId = catalogBrandId;
    }

    public CatalogBrand getCatalogBrand() {
        return catalogBrand;
    }

    public void setCatalogBrand(CatalogBrand catalogBrand) {
        this.catalogBrand = catalogBrand;
    }

    public int getAvailableStock() {
        return availableStock;
    }

    public void setAvailableStock(int availableStock) {
        this.availableStock = availableStock;
    }

    public int getRestockThreshold() {
        return restockThreshold;
    }

    public void setRestockThreshold(int restockThreshold) {
        this.restockThreshold = restockThreshold;
    }

    public int getMaxStockThreshold() {
        return maxStockThreshold;
    }

    public void setMaxStockThreshold(int maxStockThreshold) {
        this.maxStockThreshold = maxStockThreshold;
    }

    public boolean isOnReorder() {
        return onReorder;
    }

    public void setOnReorder(boolean onReorder) {
        this.onReorder = onReorder;
    }
}
