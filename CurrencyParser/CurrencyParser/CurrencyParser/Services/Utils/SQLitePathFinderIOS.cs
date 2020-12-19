using System;
using Xamarin.Forms;
using System.IO;
using CurrencyParser.Services;

[assembly: Dependency(typeof(SQLitePathFinderIOS))]
namespace CurrencyParser.Services
{
    public class SQLitePathFinderIOS : ISQLitePathFinder
    {
        public SQLitePathFinderIOS() { }
        public string GetDatabasePath(string sqliteFilename)
        {
            // определяем путь к бд
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // папка библиотеки
            var path = Path.Combine(libraryPath, sqliteFilename);

            return path;
        }
    }
}