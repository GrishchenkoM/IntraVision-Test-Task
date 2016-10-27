using System;
using System.Web.Http;
using BusinessLogic;
using Web.Models.EnityModels;

namespace Web.Controllers
{
    public class CoinController
        : BaseApiController<CoinModelFactory>
    {
        public CoinController(IDataManager dataManager) : base(dataManager)
        { }

        public IHttpActionResult Get()
        {
            var model = DataManager.Coins.Get();
            if (model != null)
                return Ok(model);

            return NotFound();
        }
        public IHttpActionResult Get(int id)
        {
            var model = DataManager.Coins.Get(id);
            if (model != null)
                return Ok(model);

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CoinModel model)
        {
            try
            {
                var entity = Modelfactory.Create(model);
                var createdEntity = DataManager.Coins.Create(entity);

                if (createdEntity != null)
                {
                    var newModel = Modelfactory.Create(createdEntity);
                    newModel.CreateUrl(createdEntity.Id, Request);

                    return Created(newModel.Url, newModel);
                }

                return InternalServerError(new Exception("Not created"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Put([FromBody] CoinModel model)
        {
            try
            {
                var entity = Modelfactory.Create(model);
                var updatedEntity = DataManager.Coins.Update(entity);

                if (updatedEntity != null)
                {
                    var newModel = Modelfactory.Create(updatedEntity);
                    newModel.CreateUrl(updatedEntity.Id, Request);

                    return Ok(newModel);
                }

                return InternalServerError(new Exception("Not Updated"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Delete(int id)
        {
            try
            {
                var entity = DataManager.Coins.Get(id);
                if (entity != null)
                {
                    DataManager.Coins.Delete(entity);
                    return Ok();
                }

                return NotFound();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
