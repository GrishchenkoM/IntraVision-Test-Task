using System.Data.Entity.Migrations;
using Core.Entities;

namespace BusinessLogic.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DbDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DbDataContext context)
        {
            context.Drinks.AddOrUpdate(
                new Drink{ Name = "Coca Cola 0.3", Cost = 23, Number = 10 },
                new Drink{ Name = "Бёрн 0.5", Cost = 65, Number = 10 },
                new Drink{ Name = "Bon Aqua", Cost = 29, Number = 10 },
                new Drink{ Name = "Lipton", Cost = 41, Number = 10 },
                new Drink{ Name = "Pepsi 0.6", Cost = 21, Number = 10 }
                );

            context.Coins.AddOrUpdate(
                new Coin{ Name = "1", Number = 100, IsAvailable = true },
                new Coin{ Name = "2", Number = 100, IsAvailable = true },
                new Coin{ Name = "5", Number = 100, IsAvailable = true },
                new Coin{ Name = "10", Number = 100, IsAvailable = true }
                );
        }
    }
}
