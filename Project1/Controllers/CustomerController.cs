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

/*
 * Ask Mark:
 *  1. asp-route-id
 *  2. Serialization problem (in test)
 *  3. Tests not working all at once
 */

namespace Project1.Controllers
{
    [Route("/")]
    public class CustomerController : Controller
    {
        private readonly CustomerLogic _logic;
        private readonly ILogger _logger;

        public CustomerController(CustomerLogic logic, ILogger<CustomerController> logger)
        {
            _logic = logic;
            _logger = logger;
        }

        /// <summary>Shows the home page</summary><returns>A view of the home page</returns>
        [Route("/")]
        [Route("/Home")]
        [Route("/Customer/Home")]
        public IActionResult Index()
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("_User")))
                return View("../Home/Index");
            else
            {
                try
                { 
                    Customer temp = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("_User"));
                    return View("UserHome", temp); 
                }
                catch(ArgumentNullException e) 
                { 
                    _logger.LogError(e.ToString());
                    _logic.ResetCache();
                    return View("UserHome"); 
                }
            }
        }

        /// <summary>Logs out the user, resetting all cookies</summary><returns>A view of the homepage</returns>
        [Route("/Customer/Logout")]
        public IActionResult Logout()
        {
            _logic.ResetCache();
            return View("../Home/Index");
        }

        /// <summary>Displays the login page</summary><returns>A view of the login page</returns>
        [Route("/Customer/Login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        /// <summary>Attempts to login the user with the provided credentials</summary><param name="number">The user's phone number</param><returns>A view of the home page if successful, or alerts the user if failed</returns>
        [HttpPost]
        [Route("/Customer/Login")]
        public IActionResult Login(string number)
        {
            if(_logic.Login(number))
                return RedirectToAction("Index", "Customer");
			else
			{
                _logger.LogError("Login Failed");
                ViewBag.Message = "Login Failed";
                return View("Login", "Customer");
			}
        }

        /// <summary>Displays the registration page</summary><returns>A view of the registration page</returns>
        [Route("/Customer/Registration")]
        public IActionResult Register()
        {
            return View("Registration");
        }

        /// <summary>Attempts to register a new customer</summary><param name="c">The posted customer</param><returns>A view to the home page if registration was successful, alerts the user if not</returns>
        [HttpPost]
        [Route("/Customer/Registration")]
        public IActionResult SaveCustomer(Customer c)
        {
            if (!ModelState.IsValid)    return View("Registration");

            if(!_logic.Register(c)) 
            {
                ViewBag.Message = "You are already registered!";
                return View("Registration");
            }
            else return RedirectToAction("Index", "Customer");
        }

        /// <summary>Displays all registered users</summary><returns>A view of all registered users</returns>
        [Route("/Customer/ViewUsers")]
        public IActionResult ViewUsers()
        {
            return View("ViewUsers", _logic.FetchCustomers());
        }

        /// <summary>Displays the active session's user's shopping cart</summary><returns>A view of the shopping cart</returns>
        [Route("/Customer/Cart")]
        public IActionResult DisplayCart()
        {
            try
            {
                return View("ShoppingCart", _logic.DisplayCart());
            }
            catch(ArgumentNullException e) 
            { 
                _logger.LogError(e.ToString());
                ViewBag.Message = "We encountered an error. Please retry.";
                _logic.ResetCache();
                return View("../Home/Index");
            }
        }

        /// <summary>Deletes the specified item from the session's shopping cart</summary><param name="productId">The id of the item to delete</param><returns>A view of the new shopping cart</returns>
        [Route("/Customer/DeleteFromCart/{productId}")]
        public IActionResult DeleteFromCart(long productId)
        {
            try
            {
                _logic.DeleteFromCart(productId);
            }
            catch(ArgumentNullException e) 
            { 
                _logger.LogError(e.ToString());
                ViewBag.Message = "We encountered an error. Please retry.";
                _logic.ResetCache();
                return View("../Home/Index");
            }
            return RedirectToAction("Cart", "Customer");
        }
        
        /// <summary>Cancel's the session's users's order</summary><returns>A view of the home page</returns>
        [Route("/Customer/CancelOrder")]
        public IActionResult CancelOrder()
        {
            try
            {
                _logic.CancelOrder();
            }
            catch(ArgumentNullException e) 
            { 
                _logger.LogError(e.ToString());
                ViewBag.Message = "We encountered an error. Please retry.";
                _logic.ResetCache();
                return View("Index");
            }
            return RedirectToAction("Index", "Customer");
        }
        
        /// <summary>Finalizes the session's user's order</summary><returns>A view of the home page</returns>
        [Route("/Customer/Checkout")]
        public IActionResult Checkout()
        {
            try
            {
                return View("UserHome", _logic.Checkout());
            }
            catch(ArgumentNullException e) 
            { 
                _logger.LogError(e.ToString());
                ViewBag.Message = "We encountered an error. Please retry.";
                _logic.ResetCache();
                return View("../Home/Index");
            }
        }

        /// <summary>Used to test the Database </summary><returns>A string if successful</returns>
        [Route("/Test")]
        public string Test()
        {
            /*
            Customer user = _repo.Login("1111111111");
            //Serialize + Deserialize user
            _repo.FetchLocations();
            //Location store = _context.Locations.Where(x => x.Address == "Brooklyn").First();
            Location store = _repo.SelectStore(2);
            List<ProductViewModel> temp = (List<ProductViewModel>) _repo.FetchProductViews(store);
            //Serialize + Deserialize List
            //Product item = _context.Products.Where(x => x.Name == "Saw").First();
            Product item = _repo.SelectProduct(temp[0].Id);
            //user.AddToCart(item, 1);
            //Serialize + Deserialize Cart
            HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(user));
            Customer user2 = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("_User"));
            Location store2 = _repo.SelectStore(2);
            _repo.SubmitOrder(store2, user2);
            */
            return "Done";
        }
    }
}
