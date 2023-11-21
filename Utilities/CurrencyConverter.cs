using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Utilities
{
    public static class CurrencyConverter
    {
        public static double Convert(string sourceCurrency, string destinationCurrency, double amount)
        {

            Dictionary<string, double> exchangeRates = new Dictionary<string, double>
            {
                { "SEK", 1 },
                { "USD", 0.094 },
                { "EUR", 0.087 },
                { "GBP", 0.076},
                { "CHF", 0.084},
                { "CAD", 0.13 },
                { "ZWD", 225.18},
            };

            // Check if source and destination currencies are the same
            if (sourceCurrency == destinationCurrency)
            {
                return amount; // No conversion needed
            }

            // Check if the exchange rates are available for both source and destination currencies
            if (exchangeRates.ContainsKey(sourceCurrency) && exchangeRates.ContainsKey(destinationCurrency))
            {
                double sourceRate = exchangeRates[sourceCurrency];
                double destinationRate = exchangeRates[destinationCurrency];

                if (sourceCurrency == "SEK")
                {
                    // Convert from SEK to any other currency
                    return amount * destinationRate;
                }
                else if (destinationCurrency == "SEK")
                {
                    // Convert from any other currency to SEK
                    return amount / sourceRate;
                }
                else
                {
                    // Convert from any other currency to any other currency (excluding SEK)
                    return amount * (destinationRate / sourceRate);
                }
            }

            // Invalid conversion
            throw new ArgumentException("Invalid source or destination currency");
        }
    }
}