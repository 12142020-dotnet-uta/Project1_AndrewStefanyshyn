using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ModelLayer.Models
{
    [Table("OrderLine")]
    public class OrderLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id{get; set;}

        [Required]
        public int Quantity{get; set;}

        [Required]
        public double Price{get; set;}

        [ForeignKey("Product")]
        public long ProductId{get; set;}

        public virtual Product Product{get; set;}

        [ForeignKey("Order")]
        public long OrderId{get; set;}

        public virtual Order O{get; set;}

        
        public OrderLine(){}

        public OrderLine(Product pp, int q, double pd)
        {
            Product = pp;
            Quantity = q;
            Price = pd;
        }

        public OrderLine(long pId, int q, double pd)
        {
            ProductId = pId;
            Quantity = q;
            Price = pd;
        }
    }
}