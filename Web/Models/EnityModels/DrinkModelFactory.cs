using System.Net.Http;
using System.Web.Http.Routing;
using Core.Entities;
using Web.Models.Interfaces;

namespace Web.Models.EnityModels
{
    public class DrinkModelFactory
    : IModelFactory<Drink, DrinkModel>
    {
        public DrinkModel Create(Drink unit)
        {
            var model = new DrinkModel()
            {
                Id = unit.Id,
                Name = unit.Name,
                Number = unit.Number,
                Cost = unit.Cost
            };
            return model;
        }

        public Drink Create(DrinkModel model)
        {
            return new Drink()
            {
                Id = model.Id,
                Name = model.Name,
                Number = model.Number
            };
        }
    }

    public class DrinkModel : IModel
    {
        public string Url { get; set; }
        /// <summary>
        /// Cretae Url for Post/Put return
        /// </summary>
        /// <param name="currentId">Id of current entity</param>
        /// <param name="requestMessage">Request info</param>
        public void CreateUrl(int currentId, HttpRequestMessage requestMessage)
        {
            Url = new UrlHelper(requestMessage).Link("Default", new { id = currentId });
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int Cost { get; set; }
    }
}