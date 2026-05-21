package com.eshop.catalog.controller;

import com.eshop.catalog.entity.CatalogItem;
import com.eshop.catalog.service.CatalogService;
import com.eshop.catalog.viewmodel.PaginatedItems;
import jakarta.validation.Valid;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
@RequestMapping("/catalog")
public class CatalogController {

    private static final Logger log = LoggerFactory.getLogger(CatalogController.class);

    private final CatalogService catalogService;

    public CatalogController(CatalogService catalogService) {
        this.catalogService = catalogService;
    }

    @GetMapping
    public String index(@RequestParam(defaultValue = "10") int pageSize,
                        @RequestParam(defaultValue = "0") int pageIndex,
                        Model model) {
        log.info("Now loading... /catalog?pageSize={}&pageIndex={}", pageSize, pageIndex);
        PaginatedItems<CatalogItem> paginatedItems = catalogService.getCatalogItemsPaginated(pageSize, pageIndex);
        model.addAttribute("paginatedItems", paginatedItems);
        model.addAttribute("pageSize", pageSize);
        return "catalog/index";
    }

    @GetMapping("/{id}")
    public String details(@PathVariable int id, Model model) {
        log.info("Now loading... /catalog/{}  (details)", id);
        CatalogItem item = catalogService.findCatalogItem(id);
        if (item == null) {
            return "redirect:/catalog";
        }
        model.addAttribute("item", item);
        return "catalog/details";
    }

    @GetMapping("/create")
    public String createForm(Model model) {
        log.info("Now loading... /catalog/create");
        model.addAttribute("catalogItem", new CatalogItem());
        model.addAttribute("brands", catalogService.getCatalogBrands());
        model.addAttribute("types", catalogService.getCatalogTypes());
        return "catalog/create";
    }

    @PostMapping("/create")
    public String create(@Valid @ModelAttribute("catalogItem") CatalogItem catalogItem,
                         BindingResult result, Model model) {
        log.info("Now processing... /catalog/create?catalogItemName={}", catalogItem.getName());
        if (result.hasErrors()) {
            model.addAttribute("brands", catalogService.getCatalogBrands());
            model.addAttribute("types", catalogService.getCatalogTypes());
            return "catalog/create";
        }
        catalogService.createCatalogItem(catalogItem);
        return "redirect:/catalog";
    }

    @GetMapping("/{id}/edit")
    public String editForm(@PathVariable int id, Model model) {
        log.info("Now loading... /catalog/{}/edit", id);
        CatalogItem item = catalogService.findCatalogItem(id);
        if (item == null) {
            return "redirect:/catalog";
        }
        model.addAttribute("catalogItem", item);
        model.addAttribute("brands", catalogService.getCatalogBrands());
        model.addAttribute("types", catalogService.getCatalogTypes());
        return "catalog/edit";
    }

    @PostMapping("/{id}/edit")
    public String edit(@PathVariable int id,
                       @Valid @ModelAttribute("catalogItem") CatalogItem catalogItem,
                       BindingResult result, Model model) {
        log.info("Now processing... /catalog/{}/edit", id);
        if (result.hasErrors()) {
            model.addAttribute("brands", catalogService.getCatalogBrands());
            model.addAttribute("types", catalogService.getCatalogTypes());
            return "catalog/edit";
        }
        catalogItem.setId(id);
        catalogService.updateCatalogItem(catalogItem);
        return "redirect:/catalog";
    }

    @GetMapping("/{id}/delete")
    public String deleteForm(@PathVariable int id, Model model) {
        log.info("Now loading... /catalog/{}/delete", id);
        CatalogItem item = catalogService.findCatalogItem(id);
        if (item == null) {
            return "redirect:/catalog";
        }
        model.addAttribute("item", item);
        return "catalog/delete";
    }

    @PostMapping("/{id}/delete")
    public String deleteConfirmed(@PathVariable int id) {
        log.info("Now processing... /catalog/{}/delete confirmed", id);
        CatalogItem item = catalogService.findCatalogItem(id);
        if (item != null) {
            catalogService.removeCatalogItem(item);
        }
        return "redirect:/catalog";
    }
}
