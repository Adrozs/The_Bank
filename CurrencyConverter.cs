using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank
{
    public static class CurrencyConverter
{
    public static double Convert(string sourceCurrency, string destinationCurrency, double amount)
    {

            Dictionary<string, double> exchangeRates = new Dictionary<string, double>
            {
                { "SEK", 1 },
                { "USD", 0.094 },  //M betyder att värdet är literal (Typ bokstavligen, utan detta blir det errors. THANKS GOOGLE
                { "EUR", 0.087 },
                { "GBP", 0.076},
                { "CHF", 0.084},
                { "CAD", 0.13 },
                { "ZWD", 225.18},
            };

            double convertedAmount = amount * exchangeRates[destinationCurrency];
            return convertedAmount;

           
    }
}
}