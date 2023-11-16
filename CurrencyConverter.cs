using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank
{
    public static class CurrencyConverter
{
    public static decimal Convert(string sourceCurrency, string destinationCurrency, decimal amount)
    {

            Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
            {
                { "SEK", 1 },
                { "USD", 0.094M },  //M betyder att värdet är literal (Typ bokstavligen, utan detta blir det errors. THANKS GOOGLE
                { "EUR", 0.087M },
                { "GBP", 0.076M},
                { "CHF", 0.084M},
                { "CAD", 0.13M },
                { "ZWD", 225.18M},
            };

            decimal convertedAmount = amount * exchangeRates[destinationCurrency];
            return convertedAmount;

           
    }
}
}