using Core.Entities;

namespace BusinessLogic.Repositories.Implementations
{
    public class DrinkRepository : Repository<Drink>
    {
        public DrinkRepository(DbDataContext context) : base(context)
        { }
    }
}
