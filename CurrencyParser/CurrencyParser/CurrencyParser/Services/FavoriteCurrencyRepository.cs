using System.Collections.Generic;
using System.Linq;
using CurrencyParser.Models;
using SQLite;
using Xamarin.Forms;
 
namespace CurrencyParser.Services
{
    public class FavoriteCurrencyRepository
    {
        SQLiteConnection Database;
        public FavoriteCurrencyRepository(string filename)
        {
            string databasePath = DependencyService.Get<ISQLitePathFinder>().GetDatabasePath(filename);
            Database = new SQLiteConnection(databasePath);
            Database.CreateTable<FavoriteCurrency>();
        }
        public IEnumerable<FavoriteCurrency> GetItems()
        {
            return Database.Table<FavoriteCurrency>().ToList();

        }
        public FavoriteCurrency GetItem(int id)
        {
            return Database.Get<FavoriteCurrency>(id);
        }
        public void DeleteItem(int id)
        {
            Database.Delete<FavoriteCurrency>(id);
        }

        public void DeleteItem(string code)
        {
            var matchedItems = Database.Table<FavoriteCurrency>().Where(x => x.Code == code).ToList();
            foreach (var item in matchedItems)
            {
                Database.Delete<FavoriteCurrency>(item.Id);
            }
        }
        public int SaveItem(FavoriteCurrency item)
        {
            if (item.Id != 0)
            {
                Database.Update(item);
                return item.Id;
            }
            else
            {
                return Database.Insert(item);
            }
        }
    }
}