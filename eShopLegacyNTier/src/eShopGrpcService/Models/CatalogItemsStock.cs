using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopGrpcService.Models;

[Table("CatalogItemsStock")]
public class CatalogItemsStock
{
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    public int CatalogItemId { get; set; }

    public int AvailableStock { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int StockId { get; set; }
}
