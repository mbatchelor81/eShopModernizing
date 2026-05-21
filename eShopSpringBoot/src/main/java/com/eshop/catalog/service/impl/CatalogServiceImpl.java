package com.eshop.catalog.service.impl;

import com.eshop.catalog.entity.CatalogBrand;
import com.eshop.catalog.entity.CatalogItem;
import com.eshop.catalog.entity.CatalogType;
import com.eshop.catalog.repository.CatalogBrandRepository;
import com.eshop.catalog.repository.CatalogItemRepository;
import com.eshop.catalog.repository.CatalogTypeRepository;
import com.eshop.catalog.service.CatalogService;
import com.eshop.catalog.viewmodel.PaginatedItems;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Profile;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
@Profile("!mock")
public class CatalogServiceImpl implements CatalogService {

    private static final Logger log = LoggerFactory.getLogger(CatalogServiceImpl.class);

    private final CatalogItemRepository catalogItemRepository;
    private final CatalogBrandRepository catalogBrandRepository;
    private final CatalogTypeRepository catalogTypeRepository;

    public CatalogServiceImpl(CatalogItemRepository catalogItemRepository,
                              CatalogBrandRepository catalogBrandRepository,
                              CatalogTypeRepository catalogTypeRepository) {
        this.catalogItemRepository = catalogItemRepository;
        this.catalogBrandRepository = catalogBrandRepository;
        this.catalogTypeRepository = catalogTypeRepository;
    }

    @Override
    public CatalogItem findCatalogItem(int id) {
        return catalogItemRepository.findById(id).orElse(null);
    }

    @Override
    public List<CatalogBrand> getCatalogBrands() {
        return catalogBrandRepository.findAll();
    }

    @Override
    @Transactional(readOnly = true)
    public PaginatedItems<CatalogItem> getCatalogItemsPaginated(int pageSize, int pageIndex) {
        Page<CatalogItem> page = catalogItemRepository.findAllWithBrandAndType(
                PageRequest.of(pageIndex, pageSize));

        return new PaginatedItems<>(
                pageIndex,
                pageSize,
                page.getTotalElements(),
                page.getContent()
        );
    }

    @Override
    public List<CatalogType> getCatalogTypes() {
        return catalogTypeRepository.findAll();
    }

    @Override
    @Transactional
    public void createCatalogItem(CatalogItem catalogItem) {
        resolveRelationships(catalogItem);
        catalogItemRepository.save(catalogItem);
        log.info("Created catalog item: {}", catalogItem.getName());
    }

    @Override
    @Transactional
    public void updateCatalogItem(CatalogItem catalogItem) {
        resolveRelationships(catalogItem);
        catalogItemRepository.save(catalogItem);
        log.info("Updated catalog item: {}", catalogItem.getId());
    }

    private void resolveRelationships(CatalogItem catalogItem) {
        catalogItem.setCatalogType(
                catalogTypeRepository.findById(catalogItem.getCatalogTypeId())
                        .orElseThrow(() -> new IllegalArgumentException("Catalog type not found: " + catalogItem.getCatalogTypeId())));
        catalogItem.setCatalogBrand(
                catalogBrandRepository.findById(catalogItem.getCatalogBrandId())
                        .orElseThrow(() -> new IllegalArgumentException("Catalog brand not found: " + catalogItem.getCatalogBrandId())));
    }

    @Override
    @Transactional
    public void removeCatalogItem(CatalogItem catalogItem) {
        catalogItemRepository.delete(catalogItem);
        log.info("Removed catalog item: {}", catalogItem.getId());
    }
}
