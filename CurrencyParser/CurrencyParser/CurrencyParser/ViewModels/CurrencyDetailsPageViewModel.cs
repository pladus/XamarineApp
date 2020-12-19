using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using CurrencyParser.Models;
using CurrencyParser.Services;

namespace CurrencyParser.ViewModels
{
    public class CurrencyDetailsPageViewModel : BaseViewModel
    {
        public CurrencySnapshot Item { get; private set; }

        public ObservableCollection<CurrencySnapshot> DynamicsItems { get; private set; } = new ObservableCollection<CurrencySnapshot>();

        public Command LoadDynamicsCommand { get; private set; }

        public event EventHandler LoadDynamicsCommandIsOver;

        public CurrencyDetailsPageViewModel(CurrencySnapshot item)
        {
            LoadDynamicsCommand = item == null ? null: new Command(async data => await ExecuteLoadDynamicsCommand(data));

            Item = item;

            Title = Item.CurrencyCode;
        }
        async Task ExecuteLoadDynamicsCommand(object data)
        {
            var parameters = (LoadDynamicsCommandBox)data;

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                DynamicsItems.Clear();
                var items = await CurrencyUIRowDataStore.GetDynamicsAsync(parameters.Id, parameters.startDate, parameters.endDate);
                foreach (var item in items)
                {
                    DynamicsItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                PopException(ex);
            }
            finally
            {
                LoadDynamicsCommandIsOver?.Invoke(this, null);
                IsBusy = false;
            }
        }
    }
}
