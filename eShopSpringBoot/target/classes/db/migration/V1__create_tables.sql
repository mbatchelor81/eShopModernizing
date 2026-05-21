CREATE TABLE CatalogBrand (
    Id INT NOT NULL PRIMARY KEY,
    Brand NVARCHAR(100) NULL
);

CREATE TABLE CatalogType (
    Id INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(100) NULL
);

CREATE TABLE CatalogItem (
    Id INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    PictureFileName NVARCHAR(255) NULL DEFAULT 'dummy.png',
    PictureUri NVARCHAR(1024) NULL,
    CatalogTypeId INT NOT NULL,
    CatalogBrandId INT NOT NULL,
    AvailableStock INT NOT NULL DEFAULT 0,
    RestockThreshold INT NOT NULL DEFAULT 0,
    MaxStockThreshold INT NOT NULL DEFAULT 0,
    OnReorder BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_CatalogItem_CatalogType FOREIGN KEY (CatalogTypeId) REFERENCES CatalogType(Id),
    CONSTRAINT FK_CatalogItem_CatalogBrand FOREIGN KEY (CatalogBrandId) REFERENCES CatalogBrand(Id)
);
