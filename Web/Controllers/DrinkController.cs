using System;
using System.Linq;
using System.Web.Http;
using BusinessLogic;
using Web.Models.EnityModels;

namespace Web.Controllers
{
    public class DrinkController
        : BaseApiController<DrinkModelFactory>
    {
        public DrinkController(IDataManager dataManager) : base(dataManager)
        { }

        public IHttpActionResult Get()
        {
            var model = DataManager.Drinks.Get().Where(d => d.Number > 0);
            if (model.Any())
                return Ok(model);

            return NotFound();
        }
        public IHttpActionResult Get(int id)
        {
            var model = DataManager.Drinks.Get(id);
            if (model != null)
                return Ok(model);

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] DrinkModel model)
        {
            try
            {
                var entity = Modelfactory.Create(model);
                var createdEntity = DataManager.Drinks.Create(entity);

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

        public IHttpActionResult Put([FromBody] DrinkModel model)
        {
            try
            {
                var entity = Modelfactory.Create(model);
                var updatedEntity = DataManager.Drinks.Update(entity);

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
                var entity = DataManager.Drinks.Get(id);
                if (entity != null)
                {
                    DataManager.Drinks.Delete(entity);
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
