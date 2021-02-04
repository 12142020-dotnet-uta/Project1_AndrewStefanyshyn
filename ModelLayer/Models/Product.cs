using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ModelLayer.Models
{
    [Table("Product")]
    [TypeConverter(typeof(ProductConverter))]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public string Description { get; set; }


        public Product() { }
        public Product(string n,double p,string d)
        {
            Name = n;
            Price = p;
            Description = d;
        }

        public override string ToString()
        {
            return "Id: " + Id + ", Name: " + Name + ", Price: " + Price + ", Description: " + Description;
        }

		public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Product)) return false;

            Product p = (Product) obj;
            return Id == p.Id;
        }

		public override int GetHashCode() => Id.GetHashCode();
	}

    // I made this class because originally Customer.Cart was a dictionary of Products, and Json had trouble
    //      deserializing it from the session.
    //   But I was unable to solve an issue with EF tracking these products and was thus not able to submit an order,
    //      so now Cart holds the ids and we query the DB for the Product every time we want to use it.
    //   So for now this class is not used
    public class ProductConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(string))   return true;
            else                               return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(value is string @string)
            {
                string s = @string;
                s = s.Substring(3); //Id
                Product rv = new Product();
                rv.Id = long.Parse(s.Substring(0, s.IndexOf(",")).Trim());
                s = s.Substring(s.IndexOf(","));
                s = s.Substring(7); //Name
                rv.Name = s.Substring(0, s.IndexOf(",")).Trim();
                s = s.Substring(s.IndexOf(","));
                s = s.Substring(8); //Price
                rv.Price = double.Parse(s.Substring(0, s.IndexOf(",")).Trim());
                s = s.Substring(s.IndexOf(","));
                s = s.Substring(14); //Description
                rv.Description = s.Trim();
                return rv;
            }
            else
                return base.ConvertFrom(context, culture, value);
        } 
    }
}