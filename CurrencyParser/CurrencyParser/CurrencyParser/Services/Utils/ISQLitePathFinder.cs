namespace CurrencyParser.Services
{
    public interface ISQLitePathFinder
    {
        string GetDatabasePath(string filename);
    }
}