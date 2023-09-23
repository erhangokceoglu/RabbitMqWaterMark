using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMqWaterMark.Web.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(100)]
        public string? ImageName { get; set; }

        [Range(1, 100000)]
        public int Stock { get; set; }
    }
}
