using EShopApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EShopApi.Controllers
{
    
    public class CartController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Route("GetUser")]
        public IHttpActionResult GetUser()
        {
            return Ok(User.Identity.Name);
        }

        [Route("api/cart/products")]
        public IHttpActionResult GetProductsInCart()
        {
            List<UserProducts> products = context.UserProducts.Where(model => model.Customer.UserName == User.Identity.Name).ToList();
            List<CartDto> CartProducts = new List<CartDto>();
            foreach (var item in products)
            {
                CartProducts.Add(new CartDto
                {
                    Product_ID = item.Product_ID,
                    Product_Image = item.Product.Product_Image,
                    Product_Name = item.Product.Product_Name,
                    Product_Price = item.Product.Product_Price,
                    Product_Quantity = item.Product_Quantity,
                    Cart_ID = item.UserProducts_ID
                });
            };
            return Ok(CartProducts);
        }

        [HttpPost]
        [Route("api/addcart/{product_id}")]
        public IHttpActionResult PostAddToCart(int product_id)
        {
            try
            {
                UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>();
                UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(userStore);

                var user = manager.FindByEmail(User.Identity.Name);
                var product = context.Products.FirstOrDefault(model => model.Product_ID == product_id);

                UserProducts userProducts = new UserProducts
                {
                    Customer_ID = user.Id,
                    Product_ID = product.Product_ID,
                };

                context.UserProducts.Add(userProducts);
                context.SaveChanges();
                return Created(Url.Link("DefaultApi", null), userProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/Cart/{id}/{quantity}")]
        public IHttpActionResult SetQuantity(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("The Quantity should be one or more than one .");
            }
            UserProducts userproduct = context.UserProducts.FirstOrDefault(model => model.UserProducts_ID == id);
            userproduct.Product_Quantity = quantity;
            context.SaveChanges();
            CartDto cart = new CartDto()
            {
                Cart_ID = userproduct.UserProducts_ID,
                Product_Quantity = userproduct.Product_Quantity,
                Product_ID = userproduct.Product_ID,
                Product_Image = userproduct.Product.Product_Image,
                Product_Name = userproduct.Product.Product_Name,
                Product_Price = userproduct.Product.Product_Price,
            };
            return Ok(cart);
        }
    }
}
