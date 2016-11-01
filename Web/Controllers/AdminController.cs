using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Filters;

namespace Web.Controllers
{
    [SimpleAuthorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CoinView(int? index)
        {
            if (index != null)
                return View("_ManageCoin", index);

            return View("_ManageCoin", -1);
        }

        public ActionResult DrinkView(int? index)
        {
            if (index != null)
                return View("_ManageDrink", index);

            return View("_ManageDrink", -1);
        }
    }
}