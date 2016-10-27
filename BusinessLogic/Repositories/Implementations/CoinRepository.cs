using Core.Entities;

namespace BusinessLogic.Repositories.Implementations
{
    public class CoinRepository : Repository<Coin>
    {
        public CoinRepository(DbDataContext context) : base(context)
        { }
    }
}
