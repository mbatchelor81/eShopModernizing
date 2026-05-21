package com.eshop.catalog.controller.api;

import com.eshop.catalog.entity.CatalogBrand;
import com.eshop.catalog.service.CatalogService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/brands")
public class BrandsRestController {

    private static final Logger log = LoggerFactory.getLogger(BrandsRestController.class);

    private final CatalogService catalogService;

    public BrandsRestController(CatalogService catalogService) {
        this.catalogService = catalogService;
    }

    @GetMapping
    public List<CatalogBrand> getAll() {
        log.info("GET /api/brands");
        return catalogService.getCatalogBrands();
    }

    @GetMapping("/{id}")
    public ResponseEntity<CatalogBrand> getById(@PathVariable int id) {
        log.info("GET /api/brands/{}", id);
        return catalogService.getCatalogBrands().stream()
                .filter(b -> b.getId() == id)
                .findFirst()
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable int id) {
        log.info("DELETE /api/brands/{}", id);
        boolean exists = catalogService.getCatalogBrands().stream()
                .anyMatch(b -> b.getId() == id);
        if (!exists) {
            return ResponseEntity.notFound().build();
        }
        // demo only - don't actually delete
        return ResponseEntity.ok().build();
    }
}
