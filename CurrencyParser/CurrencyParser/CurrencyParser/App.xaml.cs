using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CurrencyParser.Services;
using CurrencyParser.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CurrencyParser.ViewModels;

namespace CurrencyParser
{
    public partial class App : Application
    {
        private static readonly string FAV_CURRENCY_DB_NAME = "currency.db";

        private static FavoriteCurrencyRepository _favoriteCurrencyDatabase;
        public static FavoriteCurrencyRepository FavoriteCurrencyDatabase
        {
            get
            {
                if (_favoriteCurrencyDatabase == null)
                {
                    _favoriteCurrencyDatabase = new FavoriteCurrencyRepository(FAV_CURRENCY_DB_NAME);
                }
                return _favoriteCurrencyDatabase;
            }
        }

        public App()
        {
            InitializeComponent();
            DependencyService.Register<CurrencyUIRowDataStore>();
            BaseViewModel.ModelBuildFailed += ShowErrorPage;
            RefreshPage();
        }

        private void ShowErrorPage(Exception ex)
        {
            MainPage = new ErrorPage(ex.Message + ". Call Stack: " + ex.StackTrace);
        }

        private void RefreshPage()
        {
            var vm = new MainPageViewModel();
            var mainPage = new MainPage(vm);
            MainPage = new NavigationPage(mainPage);
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            
        }

        protected override void OnResume()
        {
           
        }
    }
}
