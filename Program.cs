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


            // Initalize counter to keep track of login attempts - 2 as default
            int loginAttempts = 2;

            // Declare date time variable to be used to keep track of when a user can be unfrozen if they've frozen their account
            DateTime UnFreezeTime;

            string[] menuOptions = { "1. Customer", "2. Employee", "3. Exit" };
            int selectedOption = 0;

            // Loop until user chooses to exit program
            while (true)
            {
                Console.Clear();

                // User Type Selection
                MenuFunctions.header();
                Console.WriteLine("\t\t\t  Are you a:");

                for (int i = 0; i < menuOptions.Length; i++) //Forloop to change color on the option the arrow "goes" to.
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray; // Set the color for the selected option
                        Console.Write($"\t\t\t  {menuOptions[i]} <--\n");
                        Console.ResetColor(); // Reset color to default
                    }
                    else
                    {
                        Console.WriteLine($"\t\t\t  {menuOptions[i]}");//so when menuOptions is for exemple "2" the second option will turn darkgrey
                    }
                }

                MenuFunctions.footer();
                //"Listen" to keystrokes from the user
                ConsoleKeyInfo key = Console.ReadKey(true);

                //Handles the arrow keys to move up and down the menu
                if (key.Key == ConsoleKey.UpArrow && selectedOption > 0)
                {
                    selectedOption--;
                }
                else if (key.Key == ConsoleKey.DownArrow && selectedOption < menuOptions.Length - 1)
                {
                    selectedOption++;
                }
                else if (key.Key == ConsoleKey.Enter)
                {

                    string userTypeChoice = (selectedOption + 1).ToString(); // Updates userTypeChoice based on the selected option


                    // Perform action based on selected option
                    switch (userTypeChoice)
                    {
                        case "1":
                            // Customer Login
                            MenuFunctions.header();
                            Console.WriteLine("\t\t\tLog in to your account:");                           
                            Console.Write("\t\t\tName: ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            string customerName = Console.ReadLine();
                            Console.ResetColor();
                            Console.Write("\t\t\tPIN: ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            string customerPin = Console.ReadLine();
                            Console.ResetColor();
                            MenuFunctions.footer();


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
                            MenuFunctions.header();
                            Console.WriteLine("\t\t\tLog in as admin:");
                            Console.Write("\t\t\tName: ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            string adminName = Console.ReadLine();
                            Console.ResetColor();
                            Console.Write("\t\t\tPIN: ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            string adminPin = Console.ReadLine();
                            Console.ResetColor();
                            MenuFunctions.footer();
                            
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
                }

                   // Newline for text formatting
                    Console.WriteLine();

            }
        }          
    }
}
