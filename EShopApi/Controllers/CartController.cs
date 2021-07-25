using EShopApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EShopApi.Controllers
{
    public class CartController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();
        
        public IHttpActionResult GetProductsInCart()
        {
            //if (User.Identity.IsAuthenticated == false)
            //{
            //    return StatusCode(HttpStatusCode.Forbidden);
            //}
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

        public IHttpActionResult PostAddToCart(int product_id)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            try
            {
                var user = context.Users.FirstOrDefault(model => model.UserName == User.Identity.Name);
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
