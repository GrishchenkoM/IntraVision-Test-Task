using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using BusinessLogic;
using Web.Models;
using Web.Models.EnityModels;

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

        public JsonResult GetDrinksBySum(List<CoinModel> coinModel)
        {
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
        public JsonResult MakePurchase([FromBody]List<CoinModel> coinModel, int id)
        {
            var model = _modelFactory.Create(coinModel, id);

            return Json(model);
        }

        private readonly PurchaseModelFactory _modelFactory;
    }
}
