using CurrencyParser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CurrencyParser.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public ObservableCollection<CurrencySnapshot> Rows { get; private set; } = new ObservableCollection<CurrencySnapshot>();

        public ObservableCollection<CurrencySnapshot> SourceRows { get; private set; } = new ObservableCollection<CurrencySnapshot>();

        public List<string> PinnedCurrencyCodes { get; private set; } = new List<string>();
        public Command LoadRowsCommand { get; private set; }

        public MainPageViewModel()
        {
            Title = "ВАЛЮТНЫЕ КОТИРОВКИ";
            LoadRowsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;



            try
            {              
                var items = await CurrencyUIRowDataStore.GetAllAsync();
                SourceRows.Clear();
                foreach (var item in items)
                {
                    SourceRows.Add(item);
                    Rows.Add(item);
                }
                SortRows();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                PopException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void SortRows()
        {
            PinCurrency(SourceRows, "EUR");
            PinCurrency(Rows, "EUR");
            PinCurrency(SourceRows, "USD");
            PinCurrency(Rows, "USD");
            var favorites = App.FavoriteCurrencyDatabase.GetItems().OrderBy(x => x.Id);
            foreach(var item in favorites)
            {
                PinnedCurrencyCodes.Add(item.Code);
                PinCurrency(SourceRows, item.Code);
                PinCurrency(Rows, item.Code);
            }
        }

        private void PinCurrency(ObservableCollection<CurrencySnapshot> rows, string code)
        {
            var euroRow = rows.FirstOrDefault(x => string.Equals(x.CurrencyCode, code, StringComparison.InvariantCultureIgnoreCase));
            if (euroRow == null) return;
            var oldIndex = rows.IndexOf(euroRow);
            rows.Move(oldIndex, 0);
        }
        public void FilterRows(object sender, TextChangedEventArgs e)
        {
            var input = e.NewTextValue.ToUpperInvariant();
            Rows.Clear();
            var newRows = SourceRows.Where(x => x.CurrencyName.ToUpperInvariant().Contains(input) || x.CurrencyCode.ToUpperInvariant().Contains(input)).ToList();
            foreach (var row in newRows)
            {
                Rows.Add(row);
            }
        }
    }
}
