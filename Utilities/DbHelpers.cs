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

        public static bool DoesUserExist(BankContext context, string username)
        {
            return context.Users.Any(u => u.Name == username);
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

        // Checks if user login is correct
        public static bool VerifyUserLogin(BankContext context, string userName, string pin)
        {
            // Has the user the correct credentials? Returns true if yes - false if not
            return context.Users.Any(u => u.Name == userName && u.Pin == pin);
        }

        // Checks if admin login is correct according 
        public static bool VerifyAdminLogin(BankContext context, string adminName, string adminPin)
        {
            return context.Admin.Any(a => a.Name == adminName && a.Pin == adminPin);
        }

        // Checks if there is any admin in the database
        public static bool IsAdminCreated(BankContext context)
        {
            return context.Admin.Any();
        }


        // Creates an admin 
        public static void CreateAdmin(BankContext context)
        {
            Console.WriteLine("Welcome to the Bank of Dreams setup wizard!");
            Console.Write("Please select your Admin username: ");
            string adminName = MenuFunctions.CursorReadLine();

            // Re-promt user until string isn't empty
            while (string.IsNullOrEmpty(adminName))
            {
                Console.Clear();
                Console.WriteLine("Error! Name can't be empty.");
                adminName = MenuFunctions.CursorReadLine();
            }

            // Declare admin pin string outside the loop
            string adminPin;

            // Re-promt user for pins until 2 consecutive pins match
            while (true)
            {
                // Ensure correct input
                while (true)
                {
                    Console.Clear();

                    Console.Write("Please enter a 4-digit PIN of your choosing: ");
                    adminPin = MenuFunctions.CursorReadLine();
                    
                    // Re-promt user until string isn't empty
                    if (string.IsNullOrEmpty(adminPin))
                    {
                        Console.WriteLine("Error! PIN can't be empty.");
                        Thread.Sleep(1000);
                    }
                    else if (adminPin.Length != 4)
                    {
                        Console.WriteLine("Error! PIN must be exactly 4 digits.");
                        Thread.Sleep(1000);
                    }
                    else
                        break;
                }

                // Ensure user types the same PIN again
                while (true)
                {
                    Console.Clear();
                    Console.Write("Confirm PIN: ");
                    string adminPinConfirm = MenuFunctions.CursorReadLine();

                    // If pins match save them to database and break out of loop - else write error message
                    if (adminPin == adminPinConfirm)
                    {
                        Admin admin = new Admin()
                        {
                            Name = adminName,
                            Pin = adminPin,
                        };

                        // Returns true if save to the database was successful
                        bool success = AddAdmin(context, admin);

                        if (success)
                        {
                            Console.WriteLine($"Created Admin account \"{adminName}\" with PIN \"{adminPin}\"");
                            Thread.Sleep(2000);
                        }
                        // If wasn't possible to save account to database, print error
                        else
                        {
                            Console.WriteLine($"Failed to create Admin account");
                            Console.WriteLine("Returning to menu");
                            Thread.Sleep(2000);
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("PIN codes doesn't match. Try again. \n");
                        Thread.Sleep(1000);
                        break;
                    }
                }
            }

        }

        // Returns true or false depending on if saving admin to the database was successful or not
        private static bool AddAdmin(BankContext context, Admin admin)
        {
            try
            {
                context.Admin.Add(admin);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false;
            }
        }
    }
}