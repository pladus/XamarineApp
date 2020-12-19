using CurrencyParser.Models;
using CurrencyParser.Services;
using CurrencyParser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views;
using System.Text;
using System.Threading.Tasks;
using Microcharts.Forms;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections;
using SkiaSharp.Views.Forms;

namespace CurrencyParser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrencyDetailsPage : ContentPage
    {
        private readonly decimal MaxLabelsOnChart = 9;

        private Dictionary<int, string> LabelDateLegend = new Dictionary<int, string>();
        readonly CurrencyDetailsPageViewModel ViewModel;

        private readonly ToolbarItem PinButton;

        public event EventHandler PinChanged;

        private readonly ToolbarItem UnPinButton;

        private bool FromRubleConvertorDirect = true;

        public CurrencyDetailsPage(CurrencyDetailsPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = ViewModel = viewModel;
            ViewModel.LoadDynamicsCommandIsOver += SetUpChart;
            //FillPeriodStrategies();
            StartDatePicker.DateSelected += RebuildChart;
            EndDatePicker.DateSelected += RebuildChart;
            PinButton = new ToolbarItem(
                        "ЗАКРЕПИТЬ",
                        null,
                        Pin(),
                        ToolbarItemOrder.Primary)
            { IconImageSource = new FileImageSource { File = "unpinned.png" } };

            UnPinButton = new ToolbarItem(
                        "ОТКРЕПИТЬ",
                        null,
                        UnPin(),
                        ToolbarItemOrder.Primary)
            { IconImageSource = new FileImageSource { File = "pinned.png" } };

            if (App.FavoriteCurrencyDatabase.GetItems().Any(x => x.Code == ViewModel.Item.CurrencyCode))
            {
                ToolbarItems.Add(UnPinButton);
            }
            else
            {
                ToolbarItems.Add(PinButton);
            }

        }

        private void RebuildChart(object sender, DateChangedEventArgs e)
        {
            if (EndDatePicker.Date > DateTime.Now.Date) EndDatePicker.Date = DateTime.Now.Date;
            if (StartDatePicker.Date > EndDatePicker.Date) StartDatePicker.Date = EndDatePicker.Date;
            ViewModel.LoadDynamicsCommand.Execute(new LoadDynamicsCommandBox(ViewModel.Item.Id, StartDatePicker.Date, EndDatePicker.Date));
        }
        private void SetUpChart(object sender, EventArgs e)
        {
            List<Microcharts.Entry> entries;
            decimal max, min;
            GetEntries(out entries, out max, out min);
            var chart = new LineChart() { PointSize = 0.5F, LabelTextSize=15F, LineMode=LineMode.Straight, LineSize=0.5F, MaxValue = (float)(max), MinValue = (float)(min), Entries = entries  , LineAreaAlpha = 60, BackgroundColor=SKColor.Parse("#fffafa") };
            chartView.Chart = chart;
        }

        private void GetEntries(out List<Microcharts.Entry> entries, out decimal max, out decimal min)
        {
            entries = new List<Microcharts.Entry>();
            max = 0M;
            min = decimal.MaxValue;
            var labelFrequency = Math.Floor(ViewModel.DynamicsItems.Count / MaxLabelsOnChart);
            if (labelFrequency == 0) labelFrequency = 1;
            BuildPoints(entries, ref max, ref min, labelFrequency, ViewModel.DynamicsItems);                
        }

        private void BuildPoints(List<Microcharts.Entry> entries, ref decimal max, ref decimal min, decimal labelFrequency, IEnumerable<CurrencySnapshot> currencies)
        {
            LabelDateLegend = new Dictionary<int, string>();
            var i = 0;
            var count = 1;
            foreach (var item in currencies)
            {
                max = item.ToRouble > max ? item.ToRouble : max;
                min = item.ToRouble < min ? item.ToRouble : min;
                if (i % labelFrequency == 0)
                {
                    entries.Add(new Microcharts.Entry((float)item.ToRouble) { ValueLabel = item.ToRouble.ToString() });
                    LabelDateLegend.Add(count, item.SnapshotDate.ToLongTimeString());
                    count++;
                }
                else
                {
                    entries.Add(new Microcharts.Entry((float)item.ToRouble));
                }

                i++;
            }
        }
        private Action Pin()
        {
            return async () =>
            {
                if (!await DisplayAlert("ЗАКРЕПИТЬ", "Это действие поднимет валюту вверх по списку на главной странице. Продолжить?", "ОК", "ОТМЕНА")) return;
                App.FavoriteCurrencyDatabase.SaveItem(new FavoriteCurrency() { Code = ViewModel.Item.CurrencyCode });
                ToolbarItems.Remove(PinButton);
                ToolbarItems.Add(UnPinButton);
                PinChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        private Action UnPin()
        {
            return async () =>
            {
                if (!await DisplayAlert("ОТКРЕПИТЬ", "Это действие вернет валюту в списке на главной странице на стандартное место. Продолжить?", "ОК", "ОТМЕНА")) return;
                App.FavoriteCurrencyDatabase.DeleteItem(ViewModel.Item.CurrencyCode);
                ToolbarItems.Remove(UnPinButton);
                ToolbarItems.Add(PinButton);
                PinChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel.DynamicsItems.Count == 0)
            {
                    var startDate = DateTime.Now - TimeSpan.FromDays(31);
                    StartDatePicker.Date = startDate;
                    ViewModel.LoadDynamicsCommand.Execute(new LoadDynamicsCommandBox(ViewModel.Item.Id, startDate, DateTime.Now));
            }               

        }

        private void ConvertorInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                ConvertorInput.Text = "0,00";
                return;
            }
            var input = 0M;
            if (!Decimal.TryParse(e.NewTextValue.Replace('.', ','), out input))
            {
                ConvertorInput.Text = e.OldTextValue;
                return;
            }
            UpdateConvertorResult(input);
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            FromRubleConvertorDirect = !FromRubleConvertorDirect;
            ConvertDirectionLabel.Text = FromRubleConvertorDirect ? "КОНВЕРТАЦИЯ ИЗ РУБЛЕЙ" : "КОНВЕРТАЦИЯ В РУБЛИ";
            var input = 0M;
            if (!Decimal.TryParse(ConvertorInput.Text.Replace('.', ','), out input)) return;
            UpdateConvertorResult(input);
        }

        private void UpdateConvertorResult(decimal input)
        {
            if (ViewModel == null) return;
            ConvertorResult.Text = Math.Round((FromRubleConvertorDirect ?
                 input /  (ViewModel.Item.ToRouble / ViewModel.Item.Nominal) :
                 input * (ViewModel.Item.ToRouble / ViewModel.Item.Nominal)), 2).ToString() + (!FromRubleConvertorDirect ? " РУБЛЕЙ" : " " + ViewModel.Item.CurrencyCode);
        }
    }
}