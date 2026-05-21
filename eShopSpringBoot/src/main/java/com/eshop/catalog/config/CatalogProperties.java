package com.eshop.catalog.config;

import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties(prefix = "catalog")
public record CatalogProperties(
        boolean useMockData,
        boolean useCustomizationData
) {
}
