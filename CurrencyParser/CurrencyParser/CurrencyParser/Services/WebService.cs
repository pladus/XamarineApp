using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using CurrencyParser.Models;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.Globalization;

namespace CurrencyParser.Services
{
    public class WebService
    {
        private string TemplateForGetAllValues;

        private string TemplateForGetDynamics;

        private Func<Stream, ObservableCollection<CurrencySnapshot>> ParseAllValues;

        private Func<Stream, ObservableCollection<CurrencySnapshot>> ParseDymamicsValues;

        private HttpClient Client;
        private WebService()
        {

        }

        public static WebService GetServiceOrDefault(Instance type)
        {
            var fabrics = new Dictionary<Instance, Func<WebService>>()
            {
                { Instance.CentralBank, () => GetCBWebService() }
            };

            Func<WebService> fabric = null;

            fabrics.TryGetValue(type, out fabric);

            return fabric?.Invoke();
        }
        public enum Instance
        {
            CentralBank
        }

        private static HttpClient GetWebClientForCB()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.MaxAutomaticRedirections = 300;
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri("http://www.cbr.ru");
            return client;

        }
        private static WebService GetCBWebService()
        {
            var service = new WebService();
            service.Client = GetWebClientForCB();

            service.TemplateForGetAllValues = "/scripts/XML_daily.asp?date_req={0}/{1}/{2}";
            service.TemplateForGetDynamics = "/scripts/XML_dynamic.asp?date_req1={0}/{1}/{2}&date_req2={3}/{4}/{5}&VAL_NM_RQ={6}";

            service.ParseAllValues = (x) =>
            {
                var xml = XDocument.Load(x);
                var list = from xe in xml.Element("ValCurs").Elements("Valute")
                           select new CurrencySnapshot
                           (
                               id: xe.Attribute("ID").Value,
                               currencyCode: xe.Element("CharCode").Value,
                               currencyName: xe.Element("Name").Value,
                               nominal: decimal.Parse(xe.Element("Nominal").Value, new CultureInfo("ru-RU")),
                               toRouble: Decimal.Parse(xe.Element("Value").Value, new CultureInfo("ru-RU")),
                               snapshotDate: DateTime.Now.Date
                           );
                return new ObservableCollection<CurrencySnapshot>(list);
            };

            service.ParseDymamicsValues = (x) =>
            {
                var xml = XDocument.Load(x);
                var list = from xe in xml.Element("ValCurs").Elements("Record")
                           select new CurrencySnapshot
                           (
                               id: xe.Attribute("Id").Value,
                               currencyCode: string.Empty,
                               currencyName: string.Empty,
                               snapshotDate: DateTime.Parse(xe.Attribute("Date").Value, new CultureInfo("ru-RU")),
                               nominal: Decimal.Parse(xe.Element("Nominal").Value, new CultureInfo("ru-RU")),
                               toRouble: Decimal.Parse(xe.Element("Value").Value, new CultureInfo("ru-RU"))
                           );
                return new ObservableCollection<CurrencySnapshot>(list);
            };

            return service;
        }

        public async Task<ObservableCollection<CurrencySnapshot>> GetUICurrencyRows(int tryCount = 1)
        {
            var now = DateTime.Now;
            var request = string.Format(TemplateForGetAllValues,
                    now.ToString("dd"),
                    now.ToString("MM"),
                    now.ToString("yyyy"));
            Stream data = null;
            var did = 1;
            var success = false;
            Exception exception = null;
            while (did <= tryCount && success == false)
            {
                try
                {
                    Client.Timeout = TimeSpan.FromSeconds(10);
                    var result = await Client.GetAsync(request);                    
                    data = await result.Content.ReadAsStreamAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                did++;
            }
            if (success != true) throw exception ?? new Exception("Unknown error in web transaction.");

            return ParseAllValues(data);
        }

        public async Task<ObservableCollection<CurrencySnapshot>> GetDynamicItems(string currencyId, DateTime startDate, DateTime endDate, int tryCount = 1)
        {
            var request = string.Format(TemplateForGetDynamics,
                startDate.ToString("dd"),
                startDate.ToString("MM"),
                startDate.ToString("yyyy"),
                endDate.ToString("dd"),
                endDate.ToString("MM"),
                endDate.ToString("yyyy"),
                currencyId
                );
            Stream data = null;
            var did = 1;
            var success = false;
            Exception exception = null;
            while (did <= tryCount && success == false)
            {
                try
                {
                    var result = await Client.GetAsync(request);
                    Client.Timeout = TimeSpan.FromSeconds(10);
                    data = await result.Content.ReadAsStreamAsync();
                    success = true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                did++;
            }
            if (success != true) throw exception ?? new Exception("Unknown error in web transaction.");

            return ParseDymamicsValues(data);
        }
    }
}

