package com.eshop.catalog.config;

import com.eshop.catalog.entity.CatalogBrand;
import com.eshop.catalog.entity.CatalogItem;
import com.eshop.catalog.entity.CatalogType;
import com.eshop.catalog.repository.CatalogBrandRepository;
import com.eshop.catalog.repository.CatalogItemRepository;
import com.eshop.catalog.repository.CatalogTypeRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.CommandLineRunner;
import org.springframework.context.annotation.Profile;
import org.springframework.core.io.ClassPathResource;
import org.springframework.stereotype.Component;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.math.BigDecimal;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

@Component
@Profile("!mock")
public class DataInitializer implements CommandLineRunner {

    private static final Logger log = LoggerFactory.getLogger(DataInitializer.class);

    private final CatalogBrandRepository brandRepository;
    private final CatalogTypeRepository typeRepository;
    private final CatalogItemRepository itemRepository;
    private final CatalogProperties properties;

    public DataInitializer(CatalogBrandRepository brandRepository,
                           CatalogTypeRepository typeRepository,
                           CatalogItemRepository itemRepository,
                           CatalogProperties properties) {
        this.brandRepository = brandRepository;
        this.typeRepository = typeRepository;
        this.itemRepository = itemRepository;
        this.properties = properties;
    }

    @Override
    public void run(String... args) {
        if (itemRepository.count() > 0) {
            log.info("Database already seeded, skipping initialization");
            return;
        }

        if (!properties.useCustomizationData()) {
            log.info("Customization data loading disabled, skipping CSV import");
            return;
        }

        log.info("Seeding catalog database from CSV files...");
        seedBrands();
        seedTypes();
        seedItems();
        log.info("Database seeding completed");
    }

    private void seedBrands() {
        List<String> lines = readCsvLines("setup/CatalogBrands.csv");
        for (String line : lines) {
            String brand = line.trim().replace("\"", "");
            if (!brand.isEmpty()) {
                CatalogBrand entity = new CatalogBrand();
                entity.setBrand(brand);
                brandRepository.save(entity);
            }
        }
        log.info("Seeded {} catalog brands", brandRepository.count());
    }

    private void seedTypes() {
        List<String> lines = readCsvLines("setup/CatalogTypes.csv");
        for (String line : lines) {
            String type = line.trim().replace("\"", "");
            if (!type.isEmpty()) {
                CatalogType entity = new CatalogType();
                entity.setType(type);
                typeRepository.save(entity);
            }
        }
        log.info("Seeded {} catalog types", typeRepository.count());
    }

    private void seedItems() {
        List<String> lines = readCsvLines("setup/CatalogItems.csv");
        List<CatalogBrand> brands = brandRepository.findAll();
        List<CatalogType> types = typeRepository.findAll();

        for (String line : lines) {
            String[] fields = parseCsvLine(line);
            if (fields.length < 8) continue;

            String typeName = fields[0].trim();
            String brandName = fields[1].trim();
            String description = fields[2].trim();
            String name = fields[3].trim();
            BigDecimal price = new BigDecimal(fields[4].trim());
            String pictureFileName = fields[5].trim();
            int availableStock = Integer.parseInt(fields[6].trim());
            boolean onReorder = Boolean.parseBoolean(fields[7].trim());

            CatalogType type = types.stream()
                    .filter(t -> t.getType().equalsIgnoreCase(typeName))
                    .findFirst().orElse(null);
            CatalogBrand brand = brands.stream()
                    .filter(b -> b.getBrand().equalsIgnoreCase(brandName))
                    .findFirst().orElse(null);

            if (type == null || brand == null) {
                log.warn("Skipping item '{}': type or brand not found", name);
                continue;
            }

            CatalogItem item = new CatalogItem();
            item.setName(name);
            item.setDescription(description);
            item.setPrice(price);
            item.setPictureFileName(pictureFileName);
            item.setCatalogType(type);
            item.setCatalogBrand(brand);
            item.setAvailableStock(availableStock);
            item.setOnReorder(onReorder);
            itemRepository.save(item);
        }
        log.info("Seeded {} catalog items", itemRepository.count());
    }

    private List<String> readCsvLines(String resourcePath) {
        List<String> lines = new ArrayList<>();
        try {
            ClassPathResource resource = new ClassPathResource(resourcePath);
            try (BufferedReader reader = new BufferedReader(
                    new InputStreamReader(resource.getInputStream(), StandardCharsets.UTF_8))) {
                String header = reader.readLine(); // skip header
                String line;
                while ((line = reader.readLine()) != null) {
                    if (!line.isBlank()) {
                        lines.add(line);
                    }
                }
            }
        } catch (Exception e) {
            log.warn("Could not read CSV file {}: {}", resourcePath, e.getMessage());
        }
        return lines;
    }

    private String[] parseCsvLine(String line) {
        List<String> fields = new ArrayList<>();
        StringBuilder current = new StringBuilder();
        boolean inQuotes = false;
        for (char c : line.toCharArray()) {
            if (c == '"') {
                inQuotes = !inQuotes;
            } else if (c == ',' && !inQuotes) {
                fields.add(current.toString());
                current = new StringBuilder();
            } else {
                current.append(c);
            }
        }
        fields.add(current.toString());
        return fields.toArray(new String[0]);
    }
}
