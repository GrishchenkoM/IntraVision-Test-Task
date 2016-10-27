using System.Net.Http;
using System.Web.Http.Routing;
using Core.Entities;
using Web.Models.Interfaces;

namespace Web.Models.EnityModels
{
    public class CoinModelFactory
        : IModelFactory<Coin, CoinModel>
    {
        public CoinModel Create(Coin unit)
        {
            var model = new CoinModel()
            {
                Id = unit.Id,
                Name = unit.Name,
                Number = unit.Number,
                IsAvailable = unit.IsAvailable
            };
            return model;
        }

        public Coin Create(CoinModel model)
        {
            return new Coin()
            {
                Id = model.Id,
                Name = model.Name,
                Number = model.Number,
                IsAvailable = model.IsAvailable
            };
        }
    }

    public class CoinModel : IModel
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
        public bool IsAvailable { get; set; }
    }
}