using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    public class StockTransaction
    {
        public int Id { get; set; }
        public int StockId { get; set; }  // Foreign key to StockPrices table
        public int Quantity { get; set; }
        public decimal TransactionPrice { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CompanyName { get; set; }
        public string StockName { get; set; }
        public decimal PurchasePrice { get; set; }
        public StockPrice Stock { get; set; }
    }

    public enum TransactionType
    {
        Buy,
        Sell
    }
}
