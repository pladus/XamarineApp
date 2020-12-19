using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyParser.Models
{
    public class CurrencySnapshot
    {
        //[Obsolete("Using for ORM-frameworks only!")]
        //public UICurrencyRow()
        //{

        //}
        public CurrencySnapshot(string id, string currencyCode, string currencyName, decimal nominal, decimal toRouble, DateTime snapshotDate)
        {
            Id = id;
            CurrencyCode = currencyCode;
            CurrencyName = currencyName;
            Nominal = nominal;
            ToRouble = toRouble;
            SnapshotDate = snapshotDate;
        }

        public string Id { get; private set; }
        public string CurrencyCode { get; private set; }

        public DateTime SnapshotDate { get; private set; }
        public string CurrencyName { get; private set; }

        public decimal Nominal { get; private set; }
        public decimal ToRouble { get; private set; }
    }
}
