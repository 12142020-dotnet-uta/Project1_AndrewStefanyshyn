using ModelLayer.Models;
using ModelLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
	public class ModelConvertor
	{
		public static IEnumerable<OrderView> OrdersToOrderViews(IEnumerable<Order> list)
		{
			List<OrderView> rv = new List<OrderView>();
			foreach(Order x in list)
				rv.Add(OrderToOrderView(x));
			return rv;
		}

		public static OrderView OrderToOrderView(Order o) 
			=> new OrderView(o.Id, o.Store.Address, o.OrderTime, o.Items.Sum(x => x.Price));

		public static IEnumerable<OrderDetailView> OrderLinesToOrderDetailViews(IEnumerable<OrderLine> list)
		{
			List<OrderDetailView> rv = new List<OrderDetailView>();
			foreach(OrderLine x in list)
				rv.Add(OrderLineToOrderDetailView(x));
			return rv;
		}

		public static OrderDetailView OrderLineToOrderDetailView(OrderLine o) 
			=> new OrderDetailView(o.Quantity, o.Price, o.Product.Name);
	}
}
