using System;
using System.Collections.Generic;
using System.Text;
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

    public static class MyHelpers
    {
        public static MvcHtmlString DisplayCoins(this HtmlHelper helper, List<CoinModel> list)
        {
            var ul = new TagBuilder("ul");
            ul.AddCssClass("coin");

            var ulBuilder = new StringBuilder();
            foreach (var cm in list)
            {
                for (var i = 0; i < cm.Number; ++i)
                {
                    var btn = new TagBuilder("input");
                    btn.Attributes.Add("type", "button");
                    btn.AddCssClass("coin_btn");
                    btn.AddCssClass("product col-lg-12 col-md-12 col-sm-12 col-xs-12");
                    btn.Attributes.Add("value", cm.Name);
                    btn.Attributes.Add("disabled", "disabled");

                    var li = new TagBuilder("li") {InnerHtml = btn.ToString()};
                    ulBuilder.Append(li);
                }
            }
            ul.InnerHtml = ulBuilder.ToString();

            var sb = new StringBuilder();
            sb.Append(ul);
            return MvcHtmlString.Create(sb.ToString());
        }
    }

}
/*
 var ul = document.createElement("ul");
        ul.setAttribute("class", "coin");

        for (var i = 0; i < data.length; ++i) {
            var li = document.createElement("li");
            var btn = document.createElement("input");
            btn.setAttribute("type", "button");
            btn.setAttribute("class", "coin_btn");
            btn.setAttribute("value", data[i].Name);
            btn.setAttribute("disabled", "disabled");
            li.appendChild(btn);
            ul.appendChild(li);
        }

        $("#coinsForChange").append(ul);

        $("#closeChange_btn").bind("click", UpdateIndexContent);
        */
