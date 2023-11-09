using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Bank.Data;
using The_Bank.Models;

namespace The_Bank.Utilities
{
    internal class DbHelpers
    {
        // Creates a list of all users in the database
        public static List<User> GetAllUsers(BankContext context)
        {
            List<User> users = context.Users.ToList();
            return users;   
        }

        // Adds and saves user to database
        public static bool AddUser(BankContext context, User user)
        {
            // Tries to save changes to database
            context.Users.Add(user);
            try
            {
                context.SaveChanges();
            }
            // If wasn't possible to save, print error and return
            catch (Exception ex) 
            {
                Console.WriteLine($"Error adding user: {ex}");
                return false;
            }
            return true;
        }

        // Adds and saves account to database
        public static bool AddAccount(BankContext context, Account account)
        {
            // Tries to save changes to database
            context.Accounts.Add(account);
            try
            {
                context.SaveChanges();
            }
            // If wasn't possible to save, print error and return
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding account: {ex}");
            }
            return true;
        }

        // Checks if specified user exists in the database
        public static bool VerifyLogin(string username, string pin)
        {
            using (BankContext context = new BankContext())
            {
                // Checks if any user with the specified combination of username and pin exists in the db
                // Returns true if yes and false if no
                return context.Users
                    .Any(u => u.Name == username && u.Pin == pin);
            };
        }
    }
}
