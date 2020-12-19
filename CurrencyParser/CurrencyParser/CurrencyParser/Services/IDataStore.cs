using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyParser.Services
{
    public interface IDataStore<T>
    {
        Task<ObservableCollection<T>> GetAllAsync();

        Task<ObservableCollection<T>> GetDynamicsAsync(string id, DateTime startDate, DateTime endDate);

    }
}
