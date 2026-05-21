package com.eshop.catalog.service.impl;

import com.eshop.catalog.entity.CatalogBrand;
import com.eshop.catalog.entity.CatalogItem;
import com.eshop.catalog.entity.CatalogType;
import com.eshop.catalog.service.CatalogService;
import com.eshop.catalog.viewmodel.PaginatedItems;
import org.springframework.context.annotation.Profile;
import org.springframework.stereotype.Service;

import java.math.BigDecimal;
import java.util.Comparator;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.concurrent.atomic.AtomicInteger;

@Service
@Profile("mock")
public class CatalogServiceMock implements CatalogService {

    private final List<CatalogItem> catalogItems = new CopyOnWriteArrayList<>();
    private final List<CatalogBrand> brands;
    private final List<CatalogType> types;
    private final AtomicInteger idSequence = new AtomicInteger(20);

    public CatalogServiceMock() {
        brands = List.of(
                new CatalogBrand(1, "Azure"),
                new CatalogBrand(2, ".NET"),
                new CatalogBrand(3, "Visual Studio"),
                new CatalogBrand(4, "SQL Server"),
                new CatalogBrand(5, "Other")
        );
        types = List.of(
                new CatalogType(1, "Mug"),
                new CatalogType(2, "T-Shirt"),
                new CatalogType(3, "Sheet"),
                new CatalogType(4, "USB Memory Stick")
        );
        initItems();
    }

    private void initItems() {
        catalogItems.add(createItem(1, ".NET Bot Black Hoodie", 2, 2, new BigDecimal("19.50"), "1.png", 100));
        catalogItems.add(createItem(2, ".NET Black & White Mug", 1, 2, new BigDecimal("8.50"), "2.png", 89));
        catalogItems.add(createItem(3, "Prism White T-Shirt", 2, 5, new BigDecimal("12.00"), "3.png", 56));
        catalogItems.add(createItem(4, ".NET Foundation T-shirt", 2, 2, new BigDecimal("12.00"), "4.png", 120));
        catalogItems.add(createItem(5, "Roslyn Red Sheet", 3, 5, new BigDecimal("8.50"), "5.png", 55));
        catalogItems.add(createItem(6, ".NET Blue Hoodie", 2, 2, new BigDecimal("12.00"), "6.png", 17));
        catalogItems.add(createItem(7, "Roslyn Red T-Shirt", 2, 5, new BigDecimal("12.00"), "7.png", 8));
        catalogItems.add(createItem(8, "Kudu Purple Hoodie", 2, 2, new BigDecimal("8.50"), "8.png", 34));
        catalogItems.add(createItem(9, "Cup<T> White Mug", 1, 5, new BigDecimal("12.00"), "9.png", 76));
        catalogItems.add(createItem(10, ".NET Foundation Sheet", 3, 2, new BigDecimal("12.00"), "10.png", 11));
        catalogItems.add(createItem(11, "Cup<T> Sheet", 3, 2, new BigDecimal("8.50"), "11.png", 3));
        catalogItems.add(createItem(12, "Prism White TShirt", 2, 5, new BigDecimal("12.00"), "12.png", 0));
    }

    private CatalogItem createItem(int id, String name, int typeId, int brandId,
                                   BigDecimal price, String pic, int stock) {
        CatalogItem item = new CatalogItem();
        item.setId(id);
        item.setName(name);
        item.setCatalogTypeId(typeId);
        item.setCatalogBrandId(brandId);
        item.setPrice(price);
        item.setPictureFileName(pic);
        item.setAvailableStock(stock);
        item.setCatalogBrand(brands.stream().filter(b -> b.getId() == brandId).findFirst().orElse(null));
        item.setCatalogType(types.stream().filter(t -> t.getId() == typeId).findFirst().orElse(null));
        return item;
    }

    @Override
    public CatalogItem findCatalogItem(int id) {
        return catalogItems.stream().filter(i -> i.getId() == id).findFirst().orElse(null);
    }

    @Override
    public List<CatalogBrand> getCatalogBrands() {
        return brands;
    }

    @Override
    public PaginatedItems<CatalogItem> getCatalogItemsPaginated(int pageSize, int pageIndex) {
        List<CatalogItem> sorted = catalogItems.stream()
                .sorted(Comparator.comparingInt(CatalogItem::getId))
                .toList();

        List<CatalogItem> page = sorted.stream()
                .skip((long) pageSize * pageIndex)
                .limit(pageSize)
                .toList();

        return new PaginatedItems<>(pageIndex, pageSize, sorted.size(), page);
    }

    @Override
    public List<CatalogType> getCatalogTypes() {
        return types;
    }

    @Override
    public void createCatalogItem(CatalogItem catalogItem) {
        catalogItem.setId(idSequence.getAndIncrement());
        catalogItems.add(catalogItem);
    }

    @Override
    public void updateCatalogItem(CatalogItem catalogItem) {
        for (int i = 0; i < catalogItems.size(); i++) {
            if (catalogItems.get(i).getId().equals(catalogItem.getId())) {
                catalogItems.set(i, catalogItem);
                return;
            }
        }
    }

    @Override
    public void removeCatalogItem(CatalogItem catalogItem) {
        catalogItems.removeIf(i -> i.getId().equals(catalogItem.getId()));
    }
}
