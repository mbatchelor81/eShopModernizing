package com.eshop.catalog.repository;

import com.eshop.catalog.entity.CatalogItem;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface CatalogItemRepository extends JpaRepository<CatalogItem, Integer> {

    @EntityGraph(attributePaths = {"catalogBrand", "catalogType"})
    @Query("SELECT c FROM CatalogItem c ORDER BY c.id")
    Page<CatalogItem> findAllWithBrandAndType(Pageable pageable);

    @EntityGraph(attributePaths = {"catalogBrand", "catalogType"})
    Optional<CatalogItem> findById(Integer id);
}
