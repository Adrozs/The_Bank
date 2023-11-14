using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    public class CryptoTransaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
