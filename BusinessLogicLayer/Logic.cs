using ModelLayer.Models;
using ModelLayer.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer
{
    public class Logic
    {
        protected readonly P1_Repo _repo;
        protected readonly IHttpContextAccessor _accessor;

        public Logic(P1_Repo repo, IHttpContextAccessor accessor)
        {
            _repo = repo;
            _accessor = accessor;
        }
        
        /// <summary>Clears all keys set in the session cache.</summary>
        public void ResetCache()
        {
        	_accessor.HttpContext.Session.SetString("_Store", "");
			_accessor.HttpContext.Session.SetString("_CachedProducts", "");
			_accessor.HttpContext.Session.SetString("_User", "");
        }

        /// <summary>Clears the "_Store", "_CacehedProducts" keys, and empties the user's cart</summary>
        public void ResetStoreCache()
		{
			_accessor.HttpContext.Session.SetString("_Store", "");
			_accessor.HttpContext.Session.SetString("_CachedProducts", "");
			_accessor.HttpContext.Session.SetString("_User", JsonConvert.SerializeObject(_repo.Login(JsonConvert.DeserializeObject<Customer>(_accessor.HttpContext.Session.GetString("_User")).Number)));
		}
    }
}
