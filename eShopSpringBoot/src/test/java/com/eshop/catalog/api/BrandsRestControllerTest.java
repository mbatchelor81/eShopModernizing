package com.eshop.catalog.api;

import com.eshop.catalog.entity.CatalogBrand;
import com.eshop.catalog.service.CatalogService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.test.web.servlet.MockMvc;

import static org.hamcrest.Matchers.greaterThan;
import static org.hamcrest.Matchers.hasSize;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
@ActiveProfiles("mock")
class BrandsRestControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Test
    void getAllBrands_returnsOkWithBrands() throws Exception {
        mockMvc.perform(get("/api/brands"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$", hasSize(greaterThan(0))));
    }

    @Test
    void getBrandById_existingId_returnsOk() throws Exception {
        mockMvc.perform(get("/api/brands/1"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.id").value(1))
                .andExpect(jsonPath("$.brand").exists());
    }

    @Test
    void getBrandById_nonExistingId_returnsNotFound() throws Exception {
        mockMvc.perform(get("/api/brands/99999"))
                .andExpect(status().isNotFound());
    }

    @Test
    void deleteBrand_existingId_returnsOk() throws Exception {
        mockMvc.perform(delete("/api/brands/1"))
                .andExpect(status().isOk());
    }

    @Test
    void deleteBrand_nonExistingId_returnsNotFound() throws Exception {
        mockMvc.perform(delete("/api/brands/99999"))
                .andExpect(status().isNotFound());
    }
}
