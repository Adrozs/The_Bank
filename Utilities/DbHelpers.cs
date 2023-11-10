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

        // Get specific user
        public static User GetUser(BankContext context, string username)
        {
            // Gets the user in the database that matches username
            User user = context.Users.Where(u => u.Name == username).Single();

            return user;
        }

        // Adds and saves user to database
        public static bool AddUser(BankContext context, User user)
        {
            try
            {
                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false;
            }
        }
        public static bool AddAccount(BankContext context, Account account)
        {
            try
            {
                context.Accounts.Add(account);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding account: {ex.Message}");
                return false;
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