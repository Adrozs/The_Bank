using The_Bank.Data;
using System;

using The_Bank.Utilities;

namespace The_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Welcome phrase
            Console.WriteLine("Welcome to the bank! \n");

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

                        // BankContext Object
                        using (BankContext context = new BankContext())
                        {
                            // Validate customer details
                            bool isValidCustomer = IsCustomer(context, customerName, customerPin);

                            if (isValidCustomer)
                            {
                                // Go to UserMenu
                                UserFunctions.UserMenu(context, customerName);
                            }
                            break;
                            {
                                Console.WriteLine("Invalid customer credentials");
                            }
                        }
                        break;

                    case "2":
                        // Employee OR ADMIN login portal
                        Console.Write("Name: ");
                        string adminName = Console.ReadLine();
                        Console.Write("PIN: ");
                        string adminPin = Console.ReadLine();

                        // Check if they are an admin or an imposter (sus)
                        if (IsAdmin(adminName, adminPin))
                        {
                            
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
                // ADMIN LOGIN
                // Checks if username is admin and if pin is correct - if yes it calls the admin menu
                // TODO?: change so certain users are admin instead of a pre-determined admin? Discuss in group.
                if (userName == "admin")
                {
                    if (pin != "1234")
                    {
                        Console.WriteLine("Wrong admin password");
                    }
                    else
                        AdminFunctions.DoAdminTasks();
                }

                // USER LOGIN
                // Check if account is NOT freezed
                if (!AccountFreezed.IsFreezed(userName))
                {
                    // Check if user login is correct 
                    if (DbHelpers.VerifyLogin(userName, pin))
                    {
                        UserFunctions.UserMenu(userName);
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
                        AccountFreezed.FreezeUser(userName, UnFreezeTime);
                    }
                }
                // If account IS freezed
                else
                {
                    Console.WriteLine("Too many invalid attemtps. Please try again in a few minutes.");

                    // Checks if account can be unfrozen and unfreezes it if yes, does nothing if no
                    AccountFreezed.UnFreezeUser(userName);
                }
        private static bool IsCustomer(BankContext context, string userName, string pin)
        {
            // Has the user the correct credentials?
            return context.Users.Any(u => u.Name == userName && u.Pin == pin);
        }

                // Newline for text formatting
                Console.WriteLine();
            }
        private static bool IsAdmin(string adminName, string adminPin)
        {
            // ADMIN LOGIN
            return adminName == "admin" && adminPin == "1234";
        }
    }
}
