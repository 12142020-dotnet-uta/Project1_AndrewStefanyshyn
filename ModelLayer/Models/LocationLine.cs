using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("LocationLine")]
    public class LocationLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id{get; set;}

        [Required]
        public int Stock{get; set;}


        [ForeignKey("Product")]
        public long ProductId{get; set;}

        public virtual Product Item{get; set;}

        [ForeignKey("Location")]
        public long LocationId{get; set;}

        public virtual Location L{get; set;}


        public LocationLine(){}
        public LocationLine(int s, long p, long l)
        {
            Stock = s;
            ProductId = p;
            LocationId = l;
        }
    }
}