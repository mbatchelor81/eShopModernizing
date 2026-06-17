package com.eshop.catalog.controller.api;

import com.eshop.catalog.dto.BrandDTO;
import com.eshop.catalog.service.CatalogService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/files")
public class FilesRestController {

    private static final Logger log = LoggerFactory.getLogger(FilesRestController.class);

    private final CatalogService catalogService;

    public FilesRestController(CatalogService catalogService) {
        this.catalogService = catalogService;
    }

    @GetMapping
    public List<BrandDTO> get() {
        log.info("GET /api/files");
        return catalogService.getCatalogBrands().stream()
                .map(b -> new BrandDTO(b.getId(), b.getBrand()))
                .toList();
    }
}
