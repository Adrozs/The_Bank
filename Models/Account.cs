using System;
using System.Collections.Generic;
using The_Bank.CurrencyTypeEnum;

namespace The_Bank.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
        public List<StockTransaction> StockPortfolio { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<StockPrice> StockPrices { get; set; }
    }
}