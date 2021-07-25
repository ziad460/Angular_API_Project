using EShopApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EShopApi.Controllers
{
    public class OrderController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

    }
}
