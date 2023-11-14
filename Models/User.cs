using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
        //public bool IsFreezed { get; set; }
        //public DateTime UnfreezeTime { get; set; }       
        public virtual ICollection<Account> Accounts { get; set; }
       
    }
}
