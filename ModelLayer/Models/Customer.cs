using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;  

namespace ModelLayer.Models
{
    [Table("Customer")]
    public class Customer : IShoppingCart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id{get; set;}

        [Required]
        [DisplayName("First Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Must be 2-20 characters long")]
        public string FName{get; set;}

        [Required]
        [DisplayName("Last Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Must be 2-20 characters long")]
        public string LName{get; set;}

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Must be 2-100 characters long")]
        public string Address{get; set;}

        [Required]
        [DisplayName("Phone Number")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "Must 9-20 digits long")]
        public string Number{get; set;}

        
        [NotMapped]
        public Dictionary<long, int> Cart{get; private set;} = new Dictionary<long, int>();

        /// <summary>Adds an item to cart</summary><param name="item">The item id</param><param name="quantity">The amount that will be purchased</param>
        public void AddToCart(long item, int quantity)
        {
            try{Cart.Add(item, quantity);}
            catch(System.ArgumentException){Cart[item] += quantity;}
        }

        /// <summary>Empties all items from cart</summary>
        public void DeleteFromCart() => Cart = new Dictionary<long, int>();

        /// <summary>Empties a specific item from the cart</summary><param name="productId">The id of the product to delete</param>
        public void DeleteFromCart(long productId) => Cart.Remove(productId);
        
        
        /*
        [NotMapped]
        public Dictionary<Product, int> Cart{get; private set;} = new Dictionary<Product, int>();
        //TRY STORING PRODUCTID INSTEAD OF PRODUCT
        
        /// <summary>Displays the Customer's shopping cart</summary>
        public void DisplayCart()
        {
            if(Cart.Count <= 0)
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            double total = 0, price;
            foreach(KeyValuePair<Product, int> x in Cart)
            {
                price = x.Value * x.Key.Price;
                total += price;
                Console.WriteLine(x.Value + "x " + x.Key.Name + "  = " + price);
            }
            Console.WriteLine("-----------------------\n          Total: " + total.ToString("F2"));
        }

        /// <summary>Adds an item to cart</summary><param name="item">The item</param><param name="quantity">The amount that will be purchased</param>
        public void AddToCart(Product item, int quantity)
        {
            try{Cart.Add(item, quantity);}
            catch(System.ArgumentException){Cart[item] += quantity;}
        }

        /// <summary>Empties all items from cart</summary>
        public void DeleteFromCart()
        {
            Cart = new Dictionary<Product, int>();
        }
        */


        public Customer(){}

        public Customer(string f, string l, string a, string n)
        {
            FName = f;
            LName = l;
            Address = a;
            Number = n;
        }
    }
}