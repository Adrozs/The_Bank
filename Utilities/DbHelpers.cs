using Microsoft.EntityFrameworkCore;
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

        public static User GetUserByUsername(BankContext context, string username)
        {
            // Gets the user in the database that matches the username
            User user = context.Users.FirstOrDefault(u => u.Name == username);

            return user;
        }

        // Returns true or false depending on if a user already has an account of the existing name
        public static bool AccountAlreadyExist(BankContext context, string username, string accountName)
        {
            return context.Accounts.Any(a => a.Name == accountName && a.User.Name == username);  
        }


        //public static List<User> GetUserAccounts(BankContext context, string userName)
        //{
        //    var user = context.Users
        //       .Where(u => u.Name == userName)
        //       .Include(u => u.Accounts)
        //       .ToList();

        //    return user;
        //}

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

        // Adds and saves account to the database
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
        }

        // Saves a new user pin to the database
        public static bool EditPin(BankContext context, User user, string pin)
        {
            try
            {
                user.Pin = pin;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing pin: {ex.Message}");
                return false;
            }
        }

        // Deletes a user from the database
        public static bool DeleteUser(BankContext context, User user)
        {
            try
            {
                context.Users.Remove(user);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        // Checks if specified user exists in the database
        public static bool IsCustomer(BankContext context, string userName, string pin)
        {
            // Has the user the correct credentials? Returns true if yes - false if not
            return context.Users.Any(u => u.Name == userName && u.Pin == pin);
        }

        // Checks if admin login is correct according to db
        public static bool IsAdmin(string adminName, string adminPin)
        {
            // ADMIN LOGIN
            return adminName == "admin" && adminPin == "1234";
        }
    }
}