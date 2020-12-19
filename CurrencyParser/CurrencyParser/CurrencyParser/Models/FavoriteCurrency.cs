using SQLite;

namespace CurrencyParser.Models
{
    [Table("FavoriteCurrencies")]
    public class FavoriteCurrency
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }        
        public string Code { get; set; }
    }
}
