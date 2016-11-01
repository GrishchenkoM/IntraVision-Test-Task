using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using BusinessLogic;
using Core.Entities;
using Web.Models.EnityModels;

namespace Web.Controllers
{
    public class UploadController : BaseApiController<DrinkModelFactory>
    {
        public UploadController(IDataManager dataManager) : base(dataManager)
        { }

        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }
            var provider = new MultipartMemoryStreamProvider();

            string root = HttpContext.Current.Server.MapPath("~/Files/");
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');

                var ext = filename.Substring(filename.LastIndexOf('.') + 1);

                if (!ext.Equals("xml"))
                    return BadRequest("It is not XML file!");

                byte[] fileArray = await file.ReadAsByteArrayAsync();

                using (FileStream fs = new FileStream(root + filename, FileMode.Create))
                {
                    await fs.WriteAsync(fileArray, 0, fileArray.Length);
                }
            }
            return Ok("Done!");
        }

        [HttpGet]
        public IHttpActionResult ReadImportFile()
        {
            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Files"));

            var drinkTag = new List<string>() { "drink", "напиток" };
            var nameTag = new List<string>() { "name", "наименование", "название" };
            var numberTag = new List<string>() { "number", "количество" };
            var costTag = new List<string>() { "cost", "стоимость" };

            var drinksList = new List<Drink>();
            Drink drink = null;
            var name = "name";
            var number = "number";
            var cost = "cost";
            string marker = null;

            ReadXml(files, drinkTag, drink, nameTag, marker, name,
                numberTag, number, costTag, cost, drinksList);

            if (drinksList.Any())
                foreach (var d in drinksList)
                    DataManager.Drinks.Create(d);

            return Ok("");
        }

        private void ReadXml(string[] files, List<string> drinkTag, Drink drink, List<string> nameTag, string marker, string name,
            List<string> numberTag, string number, List<string> costTag, string cost, List<Drink> drinksList)
        {
            using (var reader = new XmlTextReader(files[0]))
            {
                try
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (drinkTag.Any(tag => tag.Equals(reader.Name.ToLower())))
                                    drink = new Drink();

                                else if (nameTag.Any(tag => tag.Equals(reader.Name.ToLower())))
                                    marker = name;

                                else if (numberTag.Any(tag => tag.Equals(reader.Name.ToLower())))
                                    marker = number;

                                else if (costTag.Any(tag => tag.Equals(reader.Name.ToLower())))
                                    marker = cost;

                                break;
                            case XmlNodeType.Text:
                                if (marker == name)
                                    drink.Name = Сlean(reader.Value);

                                else if (marker == number)
                                    drink.Number = Convert.ToInt32(reader.Value);

                                else if (marker == cost)
                                    drink.Cost = Convert.ToInt32(reader.Value);

                                break;
                            case XmlNodeType.EndElement:
                                if (drinkTag.Any(tag => tag.Equals(reader.Name.ToLower())))

                                    drinksList.Add(drink);

                                break;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private string Сlean(string input)
        {
            return Regex.Replace(input, "[\r\t\n]+", "");
        }
    }
}
