using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    internal class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public virtual User User { get; set; }

    }
}
