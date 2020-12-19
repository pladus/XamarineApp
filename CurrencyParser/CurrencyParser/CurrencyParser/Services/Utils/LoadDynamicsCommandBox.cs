using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyParser.Services
{
    class LoadDynamicsCommandBox
    {
        public string Id;
        public DateTime startDate;
        public DateTime endDate;

        public LoadDynamicsCommandBox(string id, DateTime startDate, DateTime endDate)
        {
            Id = id;
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
