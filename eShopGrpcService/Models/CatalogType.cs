using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopGrpcService.Models;

public class CatalogType
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [StringLength(50)]
    public string Type { get; set; } = string.Empty;
}
