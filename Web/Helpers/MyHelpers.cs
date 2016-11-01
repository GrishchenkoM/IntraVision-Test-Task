using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Web.Models.EnityModels;

namespace Web.Helpers
{
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

                    var li = new TagBuilder("li") { InnerHtml = btn.ToString() };
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