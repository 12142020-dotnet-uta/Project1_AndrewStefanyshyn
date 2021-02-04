using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Models;
using RepositoryLayer;
using Xunit;
using System;
using System.Linq;
using BusinessLogicLayer;
using Microsoft.AspNetCore.Http;

namespace Tester
{
	[TestClass]
	public class UnitTest1
	{
        //Static Data used for multiple tests
        private static Product product_A;// = new Product("Wrench", 20, "Red");
        private static Product product_B;// = new Product("Hammer", 15, "Black");
        private static Location location_A;// = new Location("Queens");
        private static LocationLine locationLine_A;// = new LocationLine(100, _productA.Id, _locationA.Id);
        private static LocationLine locationLine_B;// = new LocationLine(50, _productB.Id, _locationA.Id);
        private static Customer customer_A;// = new Customer("Fernando", "Alonso", "Spain", "1111111111");

        private static void DefaultAddItemsToContext(P1_DbContext context)
        {
            product_A = new Product("Wrench", 20, "Red");
            product_B = new Product("Hammer", 15, "Black");
            location_A = new Location("Queens");
            locationLine_A = new LocationLine(100, product_A.Id, location_A.Id);
            locationLine_B = new LocationLine(50, product_B.Id, location_A.Id);
            customer_A = new Customer("Fernando", "Alonso", "Spain", "1111111111");
            customer_A = new Customer("Fernando", "Alonso", "Spain", "1111111111");
            context.Products.Add(product_A);
            context.Products.Add(product_B);
            context.Locations.Add(location_A);
            context.LocationLines.Add(locationLine_A);
            context.LocationLines.Add(locationLine_B);
            context.Customers.Add(customer_A);
        }

        private static void DefaultInitContext(P1_DbContext context)
        {
            locationLine_A.Item = product_A;
            locationLine_B.Item = product_B;
            location_A.Items.Add(locationLine_A);
            location_A.Items.Add(locationLine_B);
            customer_A.AddToCart(product_A.Id, 3);
            context.SaveChanges();
        }

		[TestMethod]
        public void Add_Writes_To_Db()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                repo.Register("AAA", "BBB", "CCC", "098890098890");
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.AreEqual("098890098890", repo.Login("098890098890").Number);
            }
        }

        [TestMethod]
        public void Register_Does_Not_Overwrite()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                repo.Register("AAA", "BBB", "CCC", "098890098890");
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Customer c = repo.Register("XXX", "YYY", "ZZZ", "098890098890");
                Assert.IsNull(c);
            }
        }

        [TestMethod]
        public void Register_Same_Name()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                repo.Register("AAA", "BBB", "CCC", "098890098890");
                repo.Register("AAA", "BBB", "CCC", "1111111111");
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Customer c1 = repo.Login("098890098890");
                Customer c2 = repo.Login("1111111111");
                Assert.AreEqual(c1.FName, c2.FName);
            }
        }

        [TestMethod]
        public void Register_Invalid_Data()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                repo.Register("AAA", "BBB", "CCC", "890");
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Customer c1 = repo.Login("890");
                Assert.IsNull(c1);
            }
        }

        [TestMethod]
        public void Order_Adds_To_Database()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                repo.SubmitOrder(location_A, customer_A);
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.AreEqual(1, context.Orders.Count());
            }
        }

        [TestMethod]
        public void OrderLines_Adds_Stock()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                repo.SubmitOrder(location_A, customer_A);
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.AreEqual(3, context.OrderLines.Where(x => x.Product.Name == "Wrench").FirstOrDefault().Quantity);
            }
        }

        [TestMethod]
        public void LocaionLines_Updates_Stock()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                repo.SubmitOrder(location_A, customer_A);
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.AreEqual(97, context.LocationLines.Where(x => x.Item.Name == "Wrench").FirstOrDefault().Stock);
            }
        }

        [TestMethod]
        public void Purchase_Throws_Error_If_No_Product()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                locationLine_A.Item = product_A;
                customer_A.AddToCart(product_B.Id, 3);
                context.SaveChanges();
                Assert.ThrowsException<Exception>(() => repo.SubmitOrder(location_A, customer_A));
            }
        }

        [TestMethod]
        public void Select_Store_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                context.Locations.Add(location_A);
                context.SaveChanges();
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.AreEqual("Queens", repo.SelectStore(location_A.Id).Address);
            }
        }

        [TestMethod]
        public void Select_Product_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                context.Products.Add(product_A);
                context.Products.Add(product_B);
                context.SaveChanges();
            }
            
            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.AreEqual("Wrench", repo.SelectProduct(product_A.Id).Name);
                Assert.AreEqual("Hammer", repo.SelectProduct(product_B.Id).Name);
            }
        }

        [TestMethod]
        public void Multiple_Orders_Saved_In_Customer()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                repo.SubmitOrder(location_A, customer_A);
                repo.SubmitOrder(location_A, customer_A);
                context.SaveChanges();
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                OrderLogic logic = new OrderLogic(repo, new HttpContextAccessor());
                Assert.AreEqual(2, logic.FetchCustomerOrders(1, customer_A).Count());
            }
        }

        [TestMethod]
        public void Multiple_Orders_Saved_In_Location()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                repo.SubmitOrder(location_A, customer_A);
                repo.SubmitOrder(location_A, customer_A);
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                OrderLogic logic = new OrderLogic(repo, new HttpContextAccessor());
                Assert.AreEqual(2, logic.FetchLocationOrders(1).Count());
            }
        }

        [TestMethod]
        public void Fetch_Locations_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                Location a = new Location("Queens");
                Location b = new Location("Brooklyn");
                context.Locations.Add(a);
                context.Locations.Add(b);
                context.SaveChanges();
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                LocationLogic logic = new LocationLogic(repo, new HttpContextAccessor());
                Assert.AreEqual(2, logic.FetchLocations().Count());
            }
        }

        [TestMethod]
        public void Repo_Init_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                locationLine_A.Item = product_A;
                locationLine_B.Item = product_B;
                location_A.Items.Add(locationLine_A);
                location_A.Items.Add(locationLine_B);
                context.SaveChanges();
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                repo.Init();
                Assert.AreEqual(2, context.Locations.First().Items.Count);
            }
        }

        [TestMethod]
        public void Same_Orders_Saved()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                repo.SubmitOrder(location_A, customer_A);
                Assert.AreEqual(repo.FetchOrders(customer_A).Count(), repo.FetchOrders(location_A).Count());
            }
        }

        [TestMethod]
        public void Invalid_Login_Returns_Null()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                repo.Register("AAA", "BBB", "CCC", "098890098890");
                context.SaveChanges();
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                Assert.IsNull(repo.Login("09889009889"));
            }
        }

        [TestMethod]
        public void Fetch_Customers_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                repo.Register("AAA", "BBB", "CCC", "098890098890");
                repo.Register("AAA", "BBB", "CCC", "098778900987");
                context.SaveChanges();
            }

            using(P1_DbContext context = new P1_DbContext(options))
            {
                P1_Repo repo = new P1_Repo(context);
                CustomerLogic logic = new CustomerLogic(repo, new HttpContextAccessor());
                Assert.AreEqual(2, logic.FetchCustomers().Count());
            }
        }

        [TestMethod]
        public void Delete_From_Customer_Cart_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                repo.Init();
                customer_A.DeleteFromCart();
                Assert.AreEqual(0, context.OrderLines.Count());
            }
        }

        [TestMethod]
        public void Display_Cart_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                CustomerLogic logic = new CustomerLogic(repo, new HttpContextAccessor());
                DefaultAddItemsToContext(context);
                DefaultInitContext(context);
                Assert.AreEqual(1, logic.DisplayCart(customer_A).Count());
            }
        }
        
        [TestMethod]
        public void Register_Validation_Works()
        {
            DbContextOptions<P1_DbContext> options = new DbContextOptionsBuilder<P1_DbContext>()
                                                            .UseInMemoryDatabase(databaseName: "Add_Writes_To_Db").Options;

            using(P1_DbContext context = new P1_DbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                P1_Repo repo = new P1_Repo(context);
                Assert.IsNull(repo.Register("A", "B", "C", "098890098890"));
            }
        }
	}
}
