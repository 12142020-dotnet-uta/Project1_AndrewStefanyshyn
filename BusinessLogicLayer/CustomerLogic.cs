using Microsoft.AspNetCore.Http;
using ModelLayer.Models;
using ModelLayer.ViewModels;
using Newtonsoft.Json;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
	public class CustomerLogic : Logic
	{
        public CustomerLogic(P1_Repo repo, IHttpContextAccessor accessor) : base(repo, accessor){}

        /// <summary>Attempts to register the user</summary><param name="c">The customer to register</param><returns>True if successful, false else</returns>
        public bool Register(Customer c)
        {
            c = _repo.Register(c);
            if(c == null)
                return false;
            else
            {
                _accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(c));
                return true;
            }
        }

        /// <summary>Attempts to login the user</summary><param name="number">The phone number</param><returns>True if successful, false else</returns>
        public bool Login(string number)
        {
            Customer user = _repo.Login(number);
            if(user == null)
                return false;
            else
            {
			    _accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(user));
                return true;
            }
        }

        /// <summary>Fetches the registered customers from the database</summary><returns>The list of customers</returns>
        public IEnumerable<Customer> FetchCustomers() => _repo.FetchCustomers();
            
        /// <summary>Deletes the specified item from the cart</summary><param name="productId">The id of the product to delete</param>
        public void DeleteFromCart(long productId)
        {
            Customer user = JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User"));
            user.DeleteFromCart(productId);
            _accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(user));
        }

        /// <summary>Cancel's the session user's order</summary>
        public void CancelOrder(Customer user = null)
        {
            if(user == null)
                user = JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User"));
            user.DeleteFromCart();
            _accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(user));
            _accessor.HttpContext.Session.SetString("_CachedProducts", "");
        }

        /// <summary>Gets information about the products in the user's cart</summary><returns>The list of cartproductviews</returns>
        public IEnumerable<CartProductView> DisplayCart(Customer user = null)
        {
            if(user == null)
                user = JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User"));
            List<CartProductView> rv = new List<CartProductView>();
            //foreach(KeyValuePair<Product, int> x in user.Cart)
            //    rv.Add(new CartProductView(x.Key.Id, x.Key.Name, x.Key.Price * x.Value, x.Value));
            
            foreach(KeyValuePair<long, int> x in user.Cart)
            {
                Product prod = _repo.SelectProduct(x.Key);
                rv.Add(new CartProductView(prod.Id, prod.Name, prod.Price * x.Value, x.Value));
            }
            
            return rv;
        }

        /// <summary>Submits the session user's order.</summary><returns>The session's customer</returns>
        public Customer Checkout()
        {
            Location store = _repo.SelectStore(long.Parse(_accessor.HttpContext.Session.GetString("_Store")));
            Customer user1 = JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User"));
            Customer user2 = _repo.Login(user1.Number);

            foreach(KeyValuePair<long, int> x in user1.Cart)
                user2.AddToCart(x.Key, x.Value);
            _repo.SubmitOrder(store, user2);

            //Set user cart to empty in case he wants to do more shopping
            user1.DeleteFromCart(); 
            _accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(user1));
            _accessor.HttpContext.Session.SetString("_CachedProducts",  "");
            return user1;
        }
	}
}
