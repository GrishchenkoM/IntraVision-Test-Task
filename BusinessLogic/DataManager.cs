using BusinessLogic.Repositories.Implementations;

namespace BusinessLogic
{
    public interface IDataManager
    {
        CoinRepository Coins { get; }
        DrinkRepository Drinks { get; }
    }

    public class DataManager : IDataManager
    {
        public DataManager(
            CoinRepository coinRepository, 
            DrinkRepository drinkRepository, 
            DbDataContext context)
        {
            _coinRepository = coinRepository;
            _drinkRepository = drinkRepository;
            _context = context;
        }

        public CoinRepository Coins => _coinRepository ?? new CoinRepository(_context);
        public DrinkRepository Drinks => _drinkRepository ?? new DrinkRepository(_context);

        private readonly CoinRepository _coinRepository;
        private readonly DrinkRepository _drinkRepository;
        private readonly DbDataContext _context;
    }
}
