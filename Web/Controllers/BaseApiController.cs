using System.Web.Http;
using BusinessLogic;
using Web.Models.Interfaces;

namespace Web.Controllers
{
    public class BaseApiController<TModelfactory> 
        : ApiController where TModelfactory : IModelFactory, new()
    {
        public BaseApiController(IDataManager dataManager)
        {
            DataManager = dataManager;
            Modelfactory = new TModelfactory();
        }

        public IDataManager DataManager { get; set; }
        public TModelfactory Modelfactory { get; set; }
    }
}
