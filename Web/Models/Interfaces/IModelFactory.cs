using System.Net.Http;

namespace Web.Models.Interfaces
{
    public interface IModelFactory<TEntity, TModel> 
        : IModelFactory where TModel : IModel
    {
        TModel Create(TEntity unit);
        TEntity Create(TModel model);
    }

    public interface IModelFactory
    { }

    public interface IModel
    {
        string Url { get; set; }
        void CreateUrl(int currentId, HttpRequestMessage requestMessage);
    }
}
