using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    public class Currency
    {
        public string Code { get; set; }
        public double RateToEUR { get; set; }
        public double RateToUSD { get; set; }
        public double PercentageChangeEUR { get; set; }
        public double PercentageChangeUSD { get; set; }
    }
}