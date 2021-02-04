using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModels
{
	public class CartProductView
	{
	    public long Id {get; set;}

        public string Name {get; set;}

        public double Price {get; set;}

        public int Quantity {get; set;}

        public CartProductView() { }
        public CartProductView(long i, string n, double p, int q)
        {
            Id = i;
            Name = n;
            Price = p;
            Quantity = q;
        }
	}
}
