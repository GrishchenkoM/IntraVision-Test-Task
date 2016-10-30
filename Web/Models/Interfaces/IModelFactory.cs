using System.Collections.Generic;
using System.Net.Http;
using Core.Entities;

namespace Web.Models.Interfaces
{
    public interface IModelFactory<TEntity, TModel>
        : IModelFactory, ICreate<TEntity, TModel> where TModel : IModel
    { }

    public interface IModelFactory
    { }

    public interface IModel
    {
        string Url { get; set; }
        void CreateUrl(int currentId, HttpRequestMessage requestMessage);
    }

    public interface ICreate<TEntity, TModel> where TModel : IModel
    {
        TModel Create(TEntity unit);
        TEntity Create(TModel model);
    }

    public interface ICreateModelList<TModel1, TModel2, TEntity>
        where TModel1 : IModel 
        where TModel2 : IModel 
        where TEntity : EntityBase
    {
        List<TModel1> Create(List<TModel2> list);
        List<TModel2> Create(List<TModel2> list, int id);

        List<TModel1> Create(List<TEntity> list);
        TModel1 Create(TEntity entity);
    }
}
