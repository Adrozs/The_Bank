using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    public class StockPrice
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string StockName { get; set; }
        public decimal CurrentPrice { get; set; }


        public virtual ICollection<Account> Accounts { get; set; }
        public virtual User User { get; set; }
    }
}
