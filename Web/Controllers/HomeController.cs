using System;
using System.Web.Http;
using System.Web.Mvc;
using BusinessLogic;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(DataManager dataManager)
        {
            _modelFactory = new PurchaseModelFactory(dataManager);
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public JsonResult GetDrinksBySum([FromBody]int[] coins)
        {
            var coinModel = _modelFactory.Create(coins);
            try
            {
                var model = _modelFactory.Create(coinModel);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Inrternal Server Error: " + ex.Message);
            }
        }
        public ActionResult MakePurchase([FromBody]int[] coins, int id)
        {
            var coinModel = _modelFactory.Create(coins);
            var model = _modelFactory.Create(coinModel, id);

            return View("_Change", model);
        }
        
        private readonly PurchaseModelFactory _modelFactory;
    }
}