
using System;
using CurrencyParser.Services;
using System.IO;
using Xamarin.Forms;
 
[assembly: Dependency(typeof(SQLitePathFinderAndroid))]
namespace CurrencyParser.Services
{
    public class SQLitePathFinderAndroid : ISQLitePathFinder
    {
        public SQLitePathFinderAndroid() { }
        public string GetDatabasePath(string sqliteFilename)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFilename);
            return path;
        }
    }
}