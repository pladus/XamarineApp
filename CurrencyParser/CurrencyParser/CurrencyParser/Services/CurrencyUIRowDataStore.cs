using CurrencyParser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyParser.Services
{
    public class CurrencyUIRowDataStore : IDataStore<CurrencySnapshot>
    {
        private WebService _dataService;

        public CurrencyUIRowDataStore()
        {
            _dataService = WebService.GetServiceOrDefault(WebService.Instance.CentralBank);
        }

        public async Task<ObservableCollection<CurrencySnapshot>> GetAllAsync()
        {
            return await _dataService.GetUICurrencyRows(10);
        }

        public async Task<ObservableCollection<CurrencySnapshot>> GetDynamicsAsync(string id, DateTime startDate, DateTime endDate)
        {
            return await _dataService.GetDynamicItems(id, startDate, endDate, 10);
        }        

    }
}
