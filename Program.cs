using The_Bank.Data;
using System;

namespace The_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Welcome phrase
            Console.WriteLine("Welcome to the bank! \n");

            // Loop until the user chooses to exit the program
            // TODO: Add a command to exit the program
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

                // New line
                Console.WriteLine();
            }
        }

        private static bool IsCustomer(BankContext context, string userName, string pin)
        {
            // Has the user the correct credentials?
            return context.Users.Any(u => u.Name == userName && u.Pin == pin);
        }

        private static bool IsAdmin(string adminName, string adminPin)
        {
            // ADMIN LOGIN
            return adminName == "admin" && adminPin == "1234";
        }
    }
}
