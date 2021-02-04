using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModels
{
	public class OrderView
	{
        public long OrderId{get; set;}
        public string Address{get; set;}
        public string OrderTime{get; set;}
        [Display(Name = "Sum Total")]
        public double Price{get; set;}


        public OrderView(){}

        public OrderView(long id, string a, string o, double p)
        {
            OrderId = id;
            Address = a;
            OrderTime = o;
            Price = p;
        }
	}
}
