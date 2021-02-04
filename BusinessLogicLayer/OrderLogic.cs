using Microsoft.AspNetCore.Http;
using ModelLayer.Models;
using Newtonsoft.Json;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
	public class OrderLogic : Logic
	{
		public OrderLogic(P1_Repo repo, IHttpContextAccessor accessor): base(repo, accessor){}

		/// <summary>Gets all the orders for the logged in user (The user in the session)</summary><param name="sort">How to sort the orders. 1 and 2 by date, 3 and 4 by price</param><returns>A list of the orders</returns>
		public IEnumerable<Order> FetchCustomerOrders(byte sort = 2, Customer user = null)
		{
			if(user == null)	user = JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User"));
			return _repo.FetchOrders(user, sort);
		}

		/// <summary>Gets all the orders for the specified location</summary><param name="locationId">The id of the location</param><param name="sort">How to sort the orders. 1 and 2 by date, 3 and 4 by price</param><returns>A list of the orders</returns>
        public IEnumerable<Order> FetchLocationOrders(long locationId, byte sort = 2)
		{
			Location temp = _repo.SelectLocation(locationId);
			return _repo.FetchOrders(temp, sort);
		}

		/// <summary>Gets all the orderlines of the specified order</summary><param name="orderId">The id of the order</param><param name="sort">How to sort the orders. 1 by cheapest, 2 by costliest</param><returns>A list of the orderlines<returns>
		public IEnumerable<OrderLine> FetchOrderDetails(long orderId, byte sort = 1) => _repo.FetchOrderDetails(orderId, sort);

		/// <summary>Gets information of the customer's orders</summary><param name="orders">A list of orders to summarize</param><returns>A string containing formatted information</returns>
		public string GetCustomerOrderSummary(IEnumerable<Order> orders)
        {
            return "Total Orders Places: " + orders.Count()// + Environment.NewLine //Environment.NewLine doesn't seem to display in the JS alert
                                + "  Total Products Ordered: " + orders.Select(x => x.Items.Sum(y => y.Quantity)).ToList().Sum()// + Environment.NewLine
                                + "  Total Amount Spent: $" + orders.Select(x => x.Items.Sum(y => y.Price)).ToList().Sum();
        }
		
		/// <summary>Gets information of the location's orders</summary><param name="orders">A list of orders to summarize</param><returns>A string containing formatted information</returns>
        public string GetLocationOrderSummary(IEnumerable<Order> orders)
        {
            return "Total Orders Places: " + orders.Count()// + Environment.NewLine //Environment.NewLine doesn't seem to display in the JS alert
                                + "  Total Products Ordered: " + orders.Select(x => x.Items.Sum(y => y.Quantity)).ToList().Sum()// + Environment.NewLine
                                + "  Total Revenue: $" + orders.Select(x => x.Items.Sum(y => y.Price)).ToList().Sum();
        }
	}
}
