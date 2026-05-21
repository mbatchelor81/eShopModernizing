package com.eshop.catalog.service;

import com.eshop.catalog.entity.CatalogBrand;
import com.eshop.catalog.entity.CatalogItem;
import com.eshop.catalog.entity.CatalogType;
import com.eshop.catalog.viewmodel.PaginatedItems;

import java.util.List;

public interface CatalogService {

    CatalogItem findCatalogItem(int id);

    List<CatalogBrand> getCatalogBrands();

    PaginatedItems<CatalogItem> getCatalogItemsPaginated(int pageSize, int pageIndex);

    List<CatalogType> getCatalogTypes();

    void createCatalogItem(CatalogItem catalogItem);

    void updateCatalogItem(CatalogItem catalogItem);

    void removeCatalogItem(CatalogItem catalogItem);
}
