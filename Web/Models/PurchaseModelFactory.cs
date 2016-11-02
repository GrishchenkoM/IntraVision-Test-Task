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
            List<int> coinsValueList;
            List<CoinModel> newCoinsList;
            int sum;

            CoinsServiceSetUp(list, out newCoinsList, out coinsValueList, out sum);

            var drinks = _dataManager.Drinks.Get()
                    .Where(d => d.Cost <= sum && d.Number > 0).ToList();

            var drinkModel = Create(drinks);

            DefinePresenceOfChange(drinkModel, sum, coinsValueList, newCoinsList);

            return drinkModel;
        }
        public List<CoinModel> Create(List<CoinModel> list, int id)
        {
            List<int> coinsValueList;
            List<CoinModel> tempCoinsList;
            int sum;

            CoinsServiceSetUp(list, out tempCoinsList, out coinsValueList, out sum);
            
            var drink = _dataManager.Drinks.Get(id);
            drink.Number--;
            _dataManager.Drinks.Update(drink);

            var drinkModel = Create(drink);

            var numberOfCoinsForChange = DefineChange(sum, coinsValueList, tempCoinsList, drinkModel);

            UpdateTempCoinsList(tempCoinsList, numberOfCoinsForChange);

            UpdateCoinsDatabase(tempCoinsList);

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
            List<CoinModel> list, out List<CoinModel> newCoinsList,
            out List<int> coinsValueList, out int sum)
        {
            var coins = _dataManager.Coins.Get().ToList();
            coinsValueList = CreateCoinsValueList(coins);
            sum = Sum(coins, list);
            newCoinsList = CreateNewCoinsList(coins, list);
        }

        /// <summary>
        /// Update Coins in Database
        /// </summary>
        /// <param name="tempCoinsList">Temporary list od coins</param>
        protected void UpdateCoinsDatabase(List<CoinModel> tempCoinsList)
        {
            foreach (var newCoin in tempCoinsList)
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
        /// <param name="tempCoinsList">Temporary list od coins</param>
        /// <param name="numberOfCoinsForChange">List of change</param>
        private static void UpdateTempCoinsList(List<CoinModel> tempCoinsList, Dictionary<int, int> numberOfCoinsForChange)
        {
            foreach (var newCoin in tempCoinsList)
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
        private static Dictionary<int, int> DefineChange(int sum, List<int> coinsValueList, List<CoinModel> newCoinsList, DrinkModel drinkModel)
        {
            var numberOfCoinsForChange = new Dictionary<int, int>(); //Dictionary<coinId, coinNumber>

            DefineNumberOfCoinsForChange(
                ref numberOfCoinsForChange, sum, coinsValueList, newCoinsList, drinkModel);
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
            List<DrinkModel> model, int sum, List<int> coinsValueList, List<CoinModel> newCoinsList)
        {
            foreach (var drink in model)
            {
                var coinsForChangeNumberDictionary = new Dictionary<int, int>(); //Dictionary<coinId, coinNumber>

                var isHaveChange = DefineNumberOfCoinsForChange(
                    ref coinsForChangeNumberDictionary, sum, coinsValueList, newCoinsList, drink);

                if (!isHaveChange)
                {
                    drink.ErrorMessage = "No change! Available only without change!";
                    //drink.ErrorMessage = "No change!";
                }
            }
        }

        /// <summary>
        /// Remove coins from dictionary if coinsList hasn't them 
        /// </summary>
        /// <param name="coinsForChangeNumberDictionary"></param>
        /// <param name="sum"></param>
        /// <param name="coinsValueList"></param>
        /// <param name="newCoinsList"></param>
        /// <param name="drinks"></param>
        /// <returns></returns>
        private static bool DefineNumberOfCoinsForChange(
            ref Dictionary<int, int> coinsForChangeNumberDictionary, int sum, List<int> coinsValueList, List<CoinModel> newCoinsList, DrinkModel drinks)
        {
            var isHaveChange = false;
            var change = sum - drinks.Cost;

            FillDictionaryOfCoinsForChange(
                coinsValueList, change, coinsForChangeNumberDictionary);

            if (coinsForChangeNumberDictionary.Count == 0)
                return true;

            for (var i = 0; i < coinsValueList.Count; ++i)
            {
                if (coinsForChangeNumberDictionary.ContainsKey(coinsValueList[i]))
                {
                    var coin = newCoinsList.FirstOrDefault(c =>
                        c.Name.Equals(coinsValueList[i].ToString()) && c.Number > 0);

                    var neededCoinsForChange = coinsForChangeNumberDictionary[coinsValueList[i]];

                    if (coin != null)
                    {
                        var neededCoinsNumber = neededCoinsForChange - coin.Number;
                        if (neededCoinsNumber > 0)
                        {
                            coinsForChangeNumberDictionary[coinsValueList[i]] = coin.Number;

                            FindOtherCoins(ref coinsForChangeNumberDictionary, ref isHaveChange,
                                        coinsValueList, coinsValueList[i], neededCoinsNumber);
                        }
                        else
                            isHaveChange = true;
                    }
                    else
                    {
                        var neededCoinsNumber = neededCoinsForChange;

                        coinsForChangeNumberDictionary.Remove(coinsValueList[i]);

                        FindOtherCoins(ref coinsForChangeNumberDictionary, ref isHaveChange,
                                        coinsValueList, coinsValueList[i], neededCoinsNumber);
                    }
                }
            }
            if (coinsForChangeNumberDictionary.Count == 0)
                isHaveChange = false;

            return isHaveChange;
        }

        private static void FindOtherCoins(
            ref Dictionary<int, int> coinsForChangeNumberDictionary, ref bool isHaveChange, 
            List<int> coinsValueList, int coinValue, int neededCoinsNumber)
        {
            var newValue = coinValue / 2;
            if(newValue == 0)
                return;
            
            if (coinsValueList.Contains(newValue) && coinValue != 3)
            {
                if (coinsForChangeNumberDictionary.ContainsKey(newValue))
                    coinsForChangeNumberDictionary[newValue] += 2 * neededCoinsNumber;
                else
                    coinsForChangeNumberDictionary.Add(newValue, 2 * neededCoinsNumber);

                isHaveChange = true;
            }
            else
            {
                var newCoinsValueList = coinsValueList.Where(x => x < coinValue).OrderByDescending(c => c).ToList();

                for (var j = 0; j < newCoinsValueList.Count; ++j)
                {
                    var nextCoin = newCoinsValueList[j];
                    if (newValue / nextCoin > 0)
                    {
                        if (coinsForChangeNumberDictionary.ContainsKey(nextCoin))
                        {
                            coinsForChangeNumberDictionary[nextCoin] += neededCoinsNumber;
                            newValue = newValue - nextCoin;

                            if (coinsValueList.Contains(newValue))
                                if (coinsForChangeNumberDictionary.ContainsKey(newValue))
                                {
                                    coinsForChangeNumberDictionary[newValue] += neededCoinsNumber;
                                    isHaveChange = true;
                                }
                                else
                                {
                                    FindOtherCoins(ref coinsForChangeNumberDictionary, ref isHaveChange, 
                                        coinsValueList, newValue, neededCoinsNumber);
                                }
                        }
                    }

                }
            }
        }
        
        /// <summary>
        /// Create dictionary of needed coins for change
        /// </summary>
        /// <param name="coinsValueList">List of coins value sorted in descending</param>
        /// <param name="change">Change value for Client</param>
        /// <param name="coinsForChangeNumberDictionary">Empty dictionary of needed coins for change</param>
        private static void FillDictionaryOfCoinsForChange(List<int> coinsValueList, int change,
            IDictionary<int, int> coinsForChangeNumberDictionary)
        {
            foreach (var coin in coinsValueList)
            {
                var rest = change / coin;
                if (rest > 0)
                {
                    coinsForChangeNumberDictionary.Add(coin, rest);
                    // записываем необходимое кол-во определенных монет для сдачи
                    change -= rest * coin;
                }
            }
        }

        /// <summary>
        /// Create list of coins value sorted in descending
        /// </summary>
        /// <param name="coins">List of coins from DB</param>
        /// <returns>List of coins value sorted in descending</returns>
        private static List<int> CreateCoinsValueList(List<Coin> coins)
        {
            var coinsValueList = new List<int>(coins.Count);
            coinsValueList.AddRange(coins.Select(coin => Convert.ToInt32(coin.Name)));
            coinsValueList.Sort((x, y) => y - x);
            return coinsValueList;
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