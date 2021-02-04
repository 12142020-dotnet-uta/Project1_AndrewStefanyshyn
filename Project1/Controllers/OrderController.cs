using BusinessLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer;
using ModelLayer.Models;
using Newtonsoft.Json;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Controllers
{
	public class OrderController : Controller
	{
	    private readonly P1_Repo _repo;
		private readonly OrderLogic _logic;
		private readonly ILogger _logger;

		public OrderController(P1_Repo repo, OrderLogic logic, ILogger<OrderController> logger)
		{
			_repo = repo;
			_logic = logic;
			_logger = logger;
		}

		/// <summary>Displays a view of the logged in user's orders</summary><param name="sort">How to sort the Orders. 1 and 2 by date, 3 and 4 by price</param><returns>A view of the orders</returns>
		[Route("Order/ViewCustomer")]
		public IActionResult ViewOrders(byte sort = 2)
		{
			try
			{ 
				IEnumerable<Order> temp = _logic.FetchCustomerOrders(sort); 
				ViewBag.Summary = _logic.GetCustomerOrderSummary(temp);
				return View("ViewCustomerOrders", ModelConvertor.OrdersToOrderViews(temp));
			}
			catch(ArgumentNullException e) 
            { 
                _logger.LogError(e.ToString());
                ViewBag.Message = "We encountered an error. Please retry.";
                _logic.ResetCache();
                return View("Index");
            }
		}

		/// <summary>Displays a view of either the specified store's orders, or the selected store if none specified </summary> <param name="sort">How to sort the Orders. 1 and 2 by date, 3 and 4 by price</param><returns>A view of the orders</returns>
		[Route("Order/ViewLocation")]
		[Route("Order/ViewLocation/{locationId}")]
		public IActionResult ViewOrders(long locationId = 0, byte sort = 2)
		{
			IEnumerable<Order> temp = new List<Order>();
			if(locationId <= 0)
			{
				if(!long.TryParse(HttpContext.Session.GetString("_Store"), out locationId))
					ViewBag.Message = "Location not found";
			}
			else
			{
				temp = _logic.FetchLocationOrders(locationId, sort);
				if(!temp.Any())	ViewBag.Message = "Location not found";
				else            ViewBag.Summary = _logic.GetLocationOrderSummary(temp);
			}
			return View("ViewLocationOrders", ModelConvertor.OrdersToOrderViews(temp));
		}

		/// <summary>Displays the detail of the selected order</summary><param name="orderId">The order Id</param><param name="sort">Describes how to sort the items in the order. 1 by cheapest, 2 by costliest</param><returns>A view of the items in the order</returns>
		[Route("Order/Details/{orderId}")]
		public IActionResult OrderDetails(long orderId, byte sort = 1)
		{
			IEnumerable<OrderLine> temp = _logic.FetchOrderDetails(orderId, sort);
			if(!temp.Any())	ViewBag.Message = "Order not found";
			return View("ViewOrderDetails", ModelConvertor.OrderLinesToOrderDetailViews(temp));
		}
	}
}
