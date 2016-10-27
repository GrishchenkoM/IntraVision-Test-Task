using System.Data.Entity;
using Core.Entities;

namespace BusinessLogic
{
    public class DbDataContext : DbContext
    {
        public DbDataContext() : base("DefaultConnection")
        { }

        public virtual DbSet<Coin> Coins { get; set; }
        public virtual DbSet<Drink> Drinks { get; set; }
    }
}
