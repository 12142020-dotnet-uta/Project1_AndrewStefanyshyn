using BusinessLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using ModelLayer.ViewModels;
using Newtonsoft.Json;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Controllers
{
	public class LocationController : Controller
	{
		private readonly LocationLogic _logic;
		private readonly ILogger _logger;

        public LocationController(LocationLogic logic, ILogger<LocationController> logger)
		{
			_logic = logic;
			_logger = logger;
		}

		/// <summary>Resets the cache and logs out</summary><returns>A view of the home page</returns>
		[Route("/Location/Cancel")]
		public IActionResult Cancel()
		{
			//_logic.ResetStoreCache();
			_logic.ResetCache();
			ViewBag.Message = "Order failed. Try Again.";
			return View("../Home/Index");
		}

		/// <summary>Displays the list of locations page</summary> <returns>A view of the locations</returns>
		[Route("/Location/Display")]
		public IActionResult Display()
		{
			return View("StoreSelect", _logic.FetchLocations());
		}

		/// <summary>Takes the user to a store menu</summary><param name="locationId">the id of the location</param><returns>A view of the store where the user will be shopping</returns>
		[Route("/Location/Select/{locationId}")]
		public IActionResult Select(long locationId)
		{
			HttpContext.Session.SetString("_Store", "" + locationId);

			if (string.IsNullOrEmpty(HttpContext.Session.GetString("_CachedProducts")))
				return View("Shop", _logic.SetSessionStoreCache(locationId));
			else
			{
				try
				{
					List<ProductViewModel> temp = JsonConvert.DeserializeObject<List<ProductViewModel>>(HttpContext.Session.GetString("_CachedProducts"));
					return View("Shop", temp);
				}
				catch(ArgumentNullException e) 
                { 
                    _logger.LogError(e.ToString());
					return View("Cancel", "Location"); 
				}
			}
		}

		/// <summary>Adds the products to the user's cart</summary><param name="productId">The id of the product to add</param><param name="quantity">The amount of the product to add</param><returns>A view of the updated store</returns>
		[Route("/Location/Add/{productId}")]
		[Route("/Location/Add/{productId}/{quantity}")]
		public IActionResult Add(long productId, int quantity = 1)
		{
			try
			{
				if(!_logic.Add(productId, quantity))
					return RedirectToAction("Cancel", "Order");
				else
				{
					Location temp = _logic.GetSessionStore(); 
					return RedirectToAction("Select", "Location", temp);
				}
			}
			catch(ArgumentNullException e) 
            { 
                _logger.LogError(e.ToString());
				return RedirectToAction("Cancel", "Location"); 
			}
		}
    }
}
