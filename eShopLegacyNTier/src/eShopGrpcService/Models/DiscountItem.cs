using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopGrpcService.Models;

public class DiscountItem
{
    [Key]
    public int Id { get; set; }

    public double Size { get; set; }

    [Column(TypeName = "date")]
    public DateTime Start { get; set; }

    [Column(TypeName = "date")]
    public DateTime End { get; set; }
}
