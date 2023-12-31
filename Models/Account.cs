﻿using System;
using System.Collections.Generic;


namespace The_Bank.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }        
        public virtual User User { get; set; }
        public string Currency { get; set; }

    }
}