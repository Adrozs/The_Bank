using The_Bank.Data;
using System;

using The_Bank.Utilities;

using The_Bank.Models;

namespace The_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Welcome phrase
            MenuFunctions.header();
            MenuFunctions.main_header();
            MenuFunctions.footer();
            Console.WriteLine();
            Console.WriteLine();


            // Initalize counter to keep track of login attempts
            int loginAttempts = 2;

            // Declare date time variable to be used to keep track of when a user can be unfrozen if they've frozen their account
            DateTime UnFreezeTime;


            // Loop until user chooses to exit program
            // TODO: Add command to exit program
            while (true)
            {
                // User Type Selection
                Console.WriteLine("Are you a:");
                Console.WriteLine("1. Customer");
                Console.WriteLine("2. Employee");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice (1, 2, or 3): ");
                
                string userTypeChoice = Console.ReadLine();

                switch (userTypeChoice)
                {
                    case "1":
                        // Customer Login
                        Console.Write("Name: ");
                        string customerName = Console.ReadLine();
                        Console.Write("PIN: ");
                        string customerPin = Console.ReadLine();

                        // Checks if account is NOT frozed 
                        // TODO OBS! Doesn't handle if an non existing username is put in - crashes 
                        if (!AccountFreezed.IsFreezed(customerName))
                        {
                            // BankContext Object
                            using (BankContext context = new BankContext())
                            {
                                // Checks if user login is correct
                                if (DbHelpers.IsCustomer(context, customerName, customerPin))
                                {
                                    // Reset login attempts and go to UserMenu
                                    loginAttempts = 2; 
                                    UserFunctions.UserMenu(context, customerName);
                                }
                                else if (loginAttempts > 0)
                                {
                                    Console.WriteLine($"Username or pin is invalid. {loginAttempts} tries left.");
                                    loginAttempts--;
                                }
                                // If too many invalid login attempts were made freeze account for x amount of time
                                else
                                {
                                    Console.WriteLine("Too many invalid attemtps. Please try again in a few minutes.");

                                    // Freeze account and set time that account is frozen for
                                    UnFreezeTime = DateTime.Now.AddMinutes(3);
                                    AccountFreezed.FreezeUser(customerName, UnFreezeTime);
                                }
                            }
                        }
                        // If account IS freezed
                        else
                        {
                            Console.WriteLine("Too many invalid attemtps. Please try again in a few minutes.");

                            // Checks if account can be unfrozen and unfreezes it if yes, does nothing if no
                            AccountFreezed.UnFreezeUser(customerName);
                        }
                        break;
                    case "2":
                        // Employee OR ADMIN login portal
                        Console.Write("Name: ");
                        string adminName = Console.ReadLine();
                        Console.Write("PIN: ");
                        string adminPin = Console.ReadLine();

                        // Check if they are an admin or an imposter (sus)
                        if (DbHelpers.IsAdmin(adminName, adminPin))
                        {
                            // Goes to the admin menu
                            using (BankContext context = new BankContext())
                            {
                                AdminFunctions.DoAdminTasks();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid admin credentials");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }

                // Newline for text formatting
                Console.WriteLine();
            }
        }          
    }
}
