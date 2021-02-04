using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using ModelLayer.ViewModels;

namespace RepositoryLayer
{
    public class P1_Repo
    {
        private readonly P1_DbContext _context = new P1_DbContext();

        public P1_Repo() => Init();

        public P1_Repo(P1_DbContext c)
        {
            _context = c;
            Init();
        }

        /// <summary>Initializes Dependencies from DB</summary>
        public void Init()
        {
            _context.Locations.ToList().ForEach(x => x.Items = _context.LocationLines.Where(y => y.LocationId == x.Id).ToList());
            _context.Orders.ToList().ForEach(x => x.Items = _context.OrderLines.Where(y => y.OrderId == x.Id).ToList());
            _context.LocationLines.ToList().ForEach(x => x.Item = _context.Products.Where(y => y.Id == x.ProductId).First());
            _context.OrderLines.ToList().ForEach(x => x.Product = _context.Products.Where(y => y.Id == x.ProductId).First());
        }

        /// <summary>If the customer does not exist, adds him to the database and returns the instance</summary><param name="fName">First Name</param><param name="lName">Last Name</param><param name="address">Address</param><param name="phone">Phone Number</param><returns>The newly created Customer, or null if he already exists</returns>
        public Customer Register(string fName, string lName, string address, string phone)
        {
            if(fName.Length < 2 || lName.Length < 2 || address.Length < 2 || phone.Length < 9)  return null;
            
            //Ideally we would use a credit card to identify the customer
            Customer c = _context.Customers.Where(x => x.Number == phone).FirstOrDefault();
            if (c == null)
            {
                c = new Customer(fName, lName, address, phone);
                _context.Customers.Add(c);
                _context.SaveChanges();
                return c;
            }
            else return null;
        }

        /// <summary>If the customer does not exist, adds him to the database and returns the instance</summary><param name="customer">The customer object to register</param>><returns>The newly created Customer, or null if he already exists</returns>
        public Customer Register(Customer customer)
        {
            Customer c = _context.Customers.Where(x => x.Number == customer.Number).FirstOrDefault();
            if (c == null)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return customer;
            }
            else return null;
        }

        /// <summary>Searches for a Customer based on his phone number</summary><param name="phone">Phone Number</param><returns>Returns the Customer found in the database, or null if none exists</returns>
        public Customer Login(string phone) => _context.Customers.Where(x => x.Number == phone).FirstOrDefault();

        /// <summary>Orders the Customer's cart from the chosen Location</summary><param name="store">The Location where the user is shopping</param><param name="user">The user, whose cart will be ordered</param>
        public void SubmitOrder(Location store, Customer user)
        {
            Order o = new Order(store, user);
            //foreach(KeyValuePair<Product, int> x in user.Cart)
            foreach(KeyValuePair<long, int> x in user.Cart)
            {
                /*
                Purchase(store, x.Key, x.Value);
                OrderLine ol = new OrderLine(x.Key, x.Value, x.Value * x.Key.Price);
                o.Items.Add(ol);
                */
                
                Product prod = SelectProduct(x.Key);
                Purchase(store, prod, x.Value);
                OrderLine ol = new OrderLine(prod, x.Value, x.Value * prod.Price);
                o.Items.Add(ol);
                
            }
            _context.Orders.Add(o);
            _context.SaveChanges();
        }

        /// <summary>Decreases item stock</summary><param name="store">The Location at where shopping is occuring</param><param name="item">The Product that is being purchased</param><param name="quantity">The quantity that is being purchased</param>
        private void Purchase(Location store, Product item, int quantity)
        {
            LocationLine l = store.Items.Where(x => x.Item.Id == item.Id).FirstOrDefault();
            if(l == null)   throw new Exception("ERROR!!!! SHOULD NOT HAPPEN!");
            l.Stock -= quantity;
            _context.SaveChanges();
        }

        /// <summary>Returns a Location based on id</summary><param name="id">The id of the Location</param><returns>The Location found in the database, or null if none found</returns>
        public Location SelectStore(long id) => _context.Locations.Where(x => x.Id == id).FirstOrDefault();

        /// <summary>]Selects the specified product</summary><param name="id">The id of the product</param>
        public Product SelectProduct(long id) => _context.Products.Where(x => x.Id == id).FirstOrDefault();

        /// <summary>Selects the specified location</summary><param name="id">The id of the location</param>
        public Location SelectLocation(long id) => _context.Locations.Where(x => x.Id == id).FirstOrDefault();

		/// <summary>Fetches all Customers</summary>
		public IEnumerable<Customer> FetchCustomers() => _context.Customers.ToList();

        /// <summary>Fetches all Locations</summary>
        public IEnumerable<Location> FetchLocations() => _context.Locations.ToList();
        
        /// <summary>Fetches all Products at a Location, converting to productviewmodel</summary><param name="store">The Location</param>
        public IEnumerable<ProductViewModel> FetchProductViews(Location store)
        {
            List<ProductViewModel> rv = new List<ProductViewModel>();
            foreach(LocationLine x in store.Items)  
                rv.Add(new ProductViewModel(x.Item.Id, x.Item.Name, x.Item.Price, x.Item.Description, x.Stock));
            return rv;
        }

        /// <summary> Fetches all Orders from corresponding Location, sorted as specified.</summary><param name="store">The Location</param><param name="sort">Specifies how to sort: 1 by Earliest, 2 by Latest, 3 by Cheapest, 4 by Priceiest</param>
        public IEnumerable<Order> FetchOrders(Location store, byte sort = 2)
        {
            IEnumerable<Order> rv = null;
            try
            {
                if(sort == 1)
                    rv = _context.Orders.Where(x => x.Store.Id == store.Id).OrderBy(x => x.OrderTime).ToList();
                else if(sort == 2)
                    rv = _context.Orders.Where(x => x.Store.Id == store.Id).OrderByDescending(x => x.OrderTime).ToList();
                else if(sort == 3)
                    rv = _context.Orders.Where(x => x.Store.Id == store.Id).OrderBy(y => y.Items.Sum(z => z.Quantity * z.Price)).ToList();
                else
                    rv = _context.Orders.Where(x => x.Store.Id == store.Id).OrderByDescending(y => y.Items.Sum(z => z.Quantity * z.Price)).ToList();
            }
            catch(Exception)
            {
                Init();
                return FetchOrders(store, sort);
            }
            return rv;
        }

        /// <summary>Displays all Orders from corresponding Location, sorted as specified.</summary><param name="store">The Location</param><param name="sort">Specifies how to sort: 1 by Earliest, 2 by Latest, 3 by Cheapest, 4 by Priceiest</param>
        public IEnumerable<Order> FetchOrders(Customer c, byte sort = 2)
        {
            IEnumerable<Order> rv = null;
            try
            {
                if(sort == 1)
                    rv = _context.Orders.Where(x => x.Customer.Id == c.Id).OrderBy(x => x.OrderTime).ToList();
                else if(sort == 2)
                    rv = _context.Orders.Where(x => x.Customer.Id == c.Id).OrderByDescending(x => x.OrderTime).ToList();
                else if(sort == 3)
                    rv = _context.Orders.Where(x => x.Customer.Id == c.Id).OrderBy(y => y.Items.Sum(z => z.Quantity * z.Price)).ToList();
                else
                    rv = _context.Orders.Where(x => x.Customer.Id == c.Id).OrderByDescending(y => y.Items.Sum(z => z.Quantity * z.Price)).ToList();
            }
            catch(Exception)
            {
                Init();
                return FetchOrders(c, sort);
            }
            return rv;
        }

        /// <summary>Fetches all orderlines from the corresoponding order, sorted as specified</summary><param name="orderId">The id of the order</param><param name="sort">Describes how to sort the data. 1 for cheapest, 2 for priciest</param><returns>A list of all orderlines in the order</returns>
        public IEnumerable<OrderLine> FetchOrderDetails(long orderId, byte sort = 1)
        {
            IEnumerable<OrderLine> rv = null;
            if(sort == 1)
                rv = _context.OrderLines.Where(x => x.O.Id == orderId).OrderBy(x => x.Price).ToList();
            else
                rv = _context.OrderLines.Where(x => x.O.Id == orderId).OrderByDescending(x => x.Price).ToList();
            return rv;
        }
	}
}