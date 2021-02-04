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
	public class LocationLogic : Logic
	{
		public LocationLogic(P1_Repo repo, IHttpContextAccessor accessor) : base(repo, accessor){}

		/// <summary>Fetches the list of locations from the database</summary><returns>A list of locations</returns>
		public IEnumerable<Location> FetchLocations() => _repo.FetchLocations();

		/// <summary>Sets the "_CachedProducts" cookie</summary><param name="locationId">The location id</param><returns>A list of productviewmodels from the location</returns>
		public IEnumerable<ProductViewModel> SetSessionStoreCache(long locationId)
		{
			Location store = _repo.SelectStore(locationId);
			List<ProductViewModel> temp = (List<ProductViewModel>)_repo.FetchProductViews(store);
			_accessor.HttpContext.Session.SetString("_CachedProducts", JsonConvert.SerializeObject(temp));
			return temp;
		}

		/// <summary>Adds the selected products to the session's user's cart</summary><param name="productId">The id of the product to add</param><param name="quantity">The ammount of the product to add</param><returns>True if successful, False if there was an error</returns>
		//private bool Add(Product prod, int quantity)
		public bool Add(long productId, int quantity)
        {
            //Serializing and deserializing the product list every time is probably not the best way to do this,
            //  but I'm not sure how else to dynamically update the product stock.
			List<ProductViewModel> products = null;
			try
			{
				products = JsonConvert.DeserializeObject<List<ProductViewModel>>(_accessor.HttpContext.Session.GetString("_CachedProducts"));
			}
			catch(ArgumentNullException)
			{
				try
				{
					Location store = _repo.SelectStore(long.Parse(_accessor.HttpContext.Session.GetString("_Store")));
					products = (List<ProductViewModel>)_repo.FetchProductViews(store);
					_accessor.HttpContext.Session.SetString("_CachedProducts", JsonConvert.SerializeObject(products));
				}
				catch(Exception){ return false; }
			}
			Product prod = _repo.SelectProduct(productId);

            bool rv = false;
            foreach(ProductViewModel x in products)
                if(x.Name == prod.Name)	//Should be refactored to Id
				{
                    Customer user = JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User"));
                    if(x.Stock >= quantity)    
                    {
                        x.Stock -= quantity;
                        //user.AddToCart(prod, quantity);
                        user.AddToCart(productId, quantity);
                        rv = true;

                        //Write Back to Session
                        _accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(user));
                        _accessor.HttpContext.Session.SetString("_CachedProducts", JsonConvert.SerializeObject(products));
                    }
					break;
				}
            return rv;
        }

		/// <summary>Gets the session's store</summary><returns>The store currently stored in the HttpSession</returns>
		public Location GetSessionStore() => _repo.SelectStore(long.Parse(_accessor.HttpContext.Session.GetString("_Store")));
	}
}
