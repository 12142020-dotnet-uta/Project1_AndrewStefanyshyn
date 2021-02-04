using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.ViewModels
{
    [Table("Product")]
    public class ProductViewModel
    {
        public long Id{get; set;}

        public string Name {get; set;}

        public double Price {get; set;}

        public string Description {get; set;}

        public int Stock {get; set;}

        public ProductViewModel() { }
        public ProductViewModel(long i, string n, double p, string d, int s)
        {
            Id = i;
            Name = n;
            Price = p;
            Description = d;
            Stock = s;
        }
    }
}