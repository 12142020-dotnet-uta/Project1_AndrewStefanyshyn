using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace ModelLayer.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id{get; set;}

        [Required]
        public Location Store{get; set;}

        [Required]
        public Customer Customer{get; set;}

        public string OrderTime{get; set;}
        public List<OrderLine> Items{get; set;} = new List<OrderLine>();


        public Order(){}

        public Order(Location l, Customer c)
        {
            Store = l;
            Customer = c;
            OrderTime = DateTime.Now.ToString();
        }
    }
}