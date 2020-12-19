using CurrencyParser.Models;
using CurrencyParser.Services;
using CurrencyParser.ViewModels;
using CurrencyParser.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CurrencyParser
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        internal MainPageViewModel ViewModel;

        public ObservableCollection<CurrencySnapshot> Rows { get; internal set; } = new ObservableCollection<CurrencySnapshot>();
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = ViewModel = viewModel;
            ToolbarItems.Add(
                new ToolbarItem(
                    "О программе",
                    null,
                    async () => { await Navigation.PushAsync(new AboutPage()); },
                    ToolbarItemOrder.Primary)
                { IconImageSource = new FileImageSource {File = "about_app.png" } });
            ToolbarItems.Add(
                new ToolbarItem(
                    "О разработчике",
                    null,
                    async () => { await Navigation.PushAsync(new AboutCompanyPage()); },
                    ToolbarItemOrder.Primary)
                { IconImageSource = new FileImageSource { File = "about_org.png" } });
            MainPageSearchBar.TextChanged += ViewModel.FilterRows;
        }

        private async void UIRows_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as CurrencySnapshot;
            if (item == null) return;
            var vm = new CurrencyDetailsPageViewModel(item);
            var nextPage = new CurrencyDetailsPage(vm);
            nextPage.PinChanged += PinChangedHandle;
            await Navigation.PushAsync(nextPage);
        }

        private void PinChangedHandle(object sender, EventArgs e)
        {
            ViewModel.SortRows();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel.Rows.Count == 0 || ViewModel.Rows.First().SnapshotDate.Date < DateTime.Now.Date)
            {
                ViewModel.LoadRowsCommand.Execute(null);
            }  
        }
    }
}
