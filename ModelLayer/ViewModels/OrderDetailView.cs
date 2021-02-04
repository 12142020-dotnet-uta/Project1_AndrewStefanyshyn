using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModels
{
	public class OrderDetailView
	{
        public int Quantity{get; set;}

        public double Price{get; set;}

        public string ProductName{get; set;}

        
        public OrderDetailView(){}

        public OrderDetailView(int q, double p, string n)
        {
            Quantity = q;
            Price = p;
            ProductName = n;
        }
	}
}
