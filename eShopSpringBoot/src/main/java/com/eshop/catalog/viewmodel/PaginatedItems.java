package com.eshop.catalog.viewmodel;

import java.util.List;

public record PaginatedItems<T>(
        int pageIndex,
        int pageSize,
        long totalItems,
        int totalPages,
        List<T> data
) {
    public PaginatedItems(int pageIndex, int pageSize, long totalItems, List<T> data) {
        this(pageIndex, pageSize, totalItems,
                (int) Math.ceil((double) totalItems / pageSize), data);
    }
}
