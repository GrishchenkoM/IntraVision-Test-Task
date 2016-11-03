using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic;
using Core.Entities;
using Web.Models.EnityModels;
using Web.Models.Interfaces;

namespace Web.Models
{
    public class PurchaseModelFactory 
        : IModelFactory, ICreateModelList<DrinkModel, CoinModel, Drink>
    {
        public PurchaseModelFactory(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public List<DrinkModel> Create(List<CoinModel> list)
        {
            List<CoinModel> newCoinsList;
            int sum;

            CoinsServiceSetUp(list, out newCoinsList, out sum);

            var drinks = _dataManager.Drinks.Get()
                    .Where(d => d.Cost <= sum && d.Number > 0).ToList();

            var drinkModel = Create(drinks);

            DefinePresenceOfChange(drinkModel, sum, newCoinsList);

            return drinkModel;
        }
        public List<CoinModel> Create(List<CoinModel> list, int id)
        {
            List<CoinModel> newCoinsList;
            int sum;

            CoinsServiceSetUp(list, out newCoinsList, out sum);
            
            var drink = _dataManager.Drinks.Get(id);
            drink.Number--;
            _dataManager.Drinks.Update(drink);

            var drinkModel = Create(drink);

            var numberOfCoinsForChange = DefineChange(sum, newCoinsList, drinkModel);

            UpdateTempCoinsList(newCoinsList, numberOfCoinsForChange);

            UpdateCoinsDatabase(newCoinsList);

            return CreateModelOfChange(numberOfCoinsForChange); 
        }
        public List<DrinkModel> Create(List<Drink> list)
        {
            return list.Select(drink => new DrinkModel(drink)).ToList();
        }
        public DrinkModel Create(Drink entity)
        {
            return new DrinkModel(entity);
        }
        public List<CoinModel> Create(int[] list)
        {
            var dict = new Dictionary<int, int>();
            foreach (var coin in list)
            {
                try
                {
                    dict[coin]++;
                }
                catch (Exception)
                {
                    dict.Add(coin, 1);
                }
            }
            var coinModel = new List<CoinModel>();
            foreach (var record in dict)
            {
                var coin = _dataManager.Coins.Get().FirstOrDefault(x => x.Name.Equals(record.Key.ToString()));
                if (coin != null)
                    coinModel.Add(new CoinModel()
                    {
                        Id = coin.Id,
                        Name = coin.Name,
                        Number = record.Value
                    });
            }
            return coinModel;
        }

        /// <summary>
        /// Define some needed info
        /// </summary>
        /// <param name="list">List of coins from client</param>
        /// <param name="newCoinsList">Temporary list of coins</param>
        /// <param name="coinsValueList">List of values of coins</param>
        /// <param name="sum">Amount of client's money</param>
        protected void CoinsServiceSetUp(
            List<CoinModel> list, out List<CoinModel> newCoinsList, out int sum)
        {
            var coins = _dataManager.Coins.Get().ToList();

            sum = Sum(coins, list);

            newCoinsList = CreateNewCoinsList(coins, list);
            newCoinsList.Reverse();
        }

        /// <summary>
        /// Update Coins in Database
        /// </summary>
        /// <param name="newCoinsList">Temporary list od coins</param>
        protected void UpdateCoinsDatabase(List<CoinModel> newCoinsList)
        {
            foreach (var newCoin in newCoinsList)
            {
                var coin = _dataManager.Coins.Get(newCoin.Id);
                coin.Number = newCoin.Number;
                _dataManager.Coins.Update(coin);
            }
        }
        
        #region Private Methods

        /// <summary>
        /// Change for client
        /// </summary>
        /// <param name="numberOfCoinsForChange">List of change</param>
        /// <returns>List of coins</returns>
        private static List<CoinModel> CreateModelOfChange(Dictionary<int, int> numberOfCoinsForChange)
        {
            var model = numberOfCoinsForChange.Select(c =>
                new CoinModel {Id = c.Key, Name = c.Key.ToString(), Number = c.Value}).ToList();
            return model;
        }
        
        /// <summary>
        /// Subtract the change from temporary list of coins.
        /// </summary>
        /// <param name="newCoinsList">Temporary list od coins</param>
        /// <param name="numberOfCoinsForChange">List of change</param>
        private static void UpdateTempCoinsList(List<CoinModel> newCoinsList, Dictionary<int, int> numberOfCoinsForChange)
        {
            foreach (var newCoin in newCoinsList)
            {
                var key = Convert.ToInt32(newCoin.Name);

                if (numberOfCoinsForChange.ContainsKey(key))
                    newCoin.Number -= numberOfCoinsForChange[key];
            }
        }

        /// <summary>
        /// Count the change
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="coinsValueList"></param>
        /// <param name="newCoinsList"></param>
        /// <param name="drinkModel"></param>
        /// <returns></returns>
        private static Dictionary<int, int> DefineChange(int sum, List<CoinModel> newCoinsList, DrinkModel drinkModel)
        {
            var numberOfCoinsForChange = new Dictionary<int, int>(); //Dictionary<coinId, coinNumber>
            var change = sum - drinkModel.Cost;

            var stackListOfChange = new Stack<CoinModel>();

            FillStackOfChange(stackListOfChange, newCoinsList, 0, 0, change);

            foreach (var coin in stackListOfChange)
            {
                var key = Convert.ToInt32(coin.Name);

                if (numberOfCoinsForChange.ContainsKey(key))
                    numberOfCoinsForChange[key] += 1;
                else
                    numberOfCoinsForChange.Add(key, 1);
            }

            return numberOfCoinsForChange;
        }
        
        /// <summary>
        /// Define the change for drinks
        /// </summary>
        /// <param name="model">Model of drinks</param>
        /// <param name="sum">Sum of received money from Client</param>
        /// <param name="coinsValueList">List of coins value sorted in descending</param>
        /// <param name="newCoinsList">List of coins with coins from Client</param>
        private static void DefinePresenceOfChange(
            List<DrinkModel> model, int sum, List<CoinModel> newCoinsList)
        {
            foreach (var drink in model)
            {
                var change = sum - drink.Cost;

                if(change == 0)
                    continue;
                
                var stackOfChange = new Stack<CoinModel>();
                
                var result = FillStackOfChange(stackOfChange, newCoinsList, 0, 0, change);

                if(result) continue;
                
                if (change != 0)
                    drink.ErrorMessage = "No change! Available only without change!";
            }
        }

        private static bool FillStackOfChange(
            Stack<CoinModel> stackOfChange, List<CoinModel> newCoinList, int coinIndex, int number, int change)
        {
            var result = false;

            if (coinIndex >= newCoinList.Count || newCoinList[coinIndex].Number == 0)
                return false;

            var temp = stackOfChange.Sum(s => Convert.ToInt32(s.Name));

            if (number >= newCoinList[coinIndex].Number)
            {
                number = 0;
                ++coinIndex;

                if (coinIndex >= newCoinList.Count)
                    return result;
            }

            for (int i = number; i < newCoinList[coinIndex].Number; ++i)
            {
                temp += Convert.ToInt32(newCoinList[coinIndex].Name);

                if(temp < change)
                {
                    stackOfChange.Push(newCoinList[coinIndex]);
                    result = FillStackOfChange(stackOfChange, newCoinList, coinIndex, i + 1, change);
                    if (!result)
                    {
                        stackOfChange.Pop();

                        coinIndex++;
                        if (coinIndex >= newCoinList.Count)
                            break;

                        i = -1;
                        temp = stackOfChange.Sum(s => Convert.ToInt32(s.Name));
                        continue;
                    }
                    break;
                }
                if (temp == change)
                {
                    stackOfChange.Push(newCoinList[coinIndex]);
                    return true;
                }

                if (temp > change)
                {
                    if (++coinIndex >= newCoinList.Count)
                        break;

                    while (coinIndex < newCoinList.Count && newCoinList[coinIndex].Number == 0)
                    {
                        ++coinIndex;
                    }

                    if (coinIndex >= newCoinList.Count)
                        break;

                    temp = stackOfChange.Sum(s => Convert.ToInt32(s.Name));
                    i = -1;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Calculate the sum of received money from Client
        /// </summary>
        /// <param name="coins">List of coins from DB</param>
        /// <param name="coinModel">Model of coins from Client</param>
        /// <returns>Sum of received coins from Client</returns>
        private static int Sum(List<Coin> coins, List<CoinModel> coinModel)
        {
            var sum = coinModel.Sum(cn =>
            {
                var firstOrDefault = coins.FirstOrDefault(x => x.Id == cn.Id);
                return firstOrDefault != null ? cn.Number * Convert.ToInt32(firstOrDefault.Name) : 0;
            });
            return sum;
        }

        /// <summary>
        /// Create list of coins from Client
        /// </summary>
        /// <param name="coins">List of coins from DB</param>
        /// <param name="coinModel">Model of coins from Client</param>
        /// <returns>List of coins from Client</returns>
        private static List<CoinModel> CreateNewCoinsList(
            List<Coin> coins, List<CoinModel> coinModel)
        {
            var newCoinsList = new List<CoinModel>();
            foreach (var c in coins)
            {
                var firstOrDefault = coinModel.FirstOrDefault(x => x.Id == c.Id);
                if (firstOrDefault != null)
                    newCoinsList.Add(new CoinModel()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Number = c.Number + firstOrDefault.Number,
                        IsAvailable = c.IsAvailable
                    });
                else
                    newCoinsList.Add(new CoinModel()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Number = c.Number,
                        IsAvailable = c.IsAvailable
                    });
            }
            return newCoinsList;
        }

        #endregion
        

        private readonly DataManager _dataManager;
    }
}