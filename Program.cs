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
            Console.CursorVisible = false;
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

            using (BankContext context = new BankContext())
            {
                while (true)
                {

                    MenuFunctions.header();
                    Console.WriteLine("\t\t\tLog in to your account:");
                    Console.Write("\t\t\tName: ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    string customerName = MenuFunctions.CursorReadLine();
                    Console.ResetColor();
                    Console.Write("\t\t\tPIN: ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    string customerPin = MenuFunctions.CursorReadLine();
                    Console.ResetColor();
                    MenuFunctions.footer();


                    // Check if login is admin login  
                    if (DbHelpers.IsAdmin(customerName, customerPin))
                    {
                        // Goes to the admin menu
                        AdminFunctions.DoAdminTasks(context);


                    }

                    // If not admin continue with the rest of the code


                    // Checks if account is NOT frozen
                    // TODO OBS! Doesn't handle if an non existing username is put in - crashes 
                    if (!AccountFreezed.IsFreezed(customerName))
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

                            // Waits to allow text to be shown before proceeding
                            Thread.Sleep(2000);
                        }
                        // If too many invalid login attempts were made freeze account for x amount of time
                        else
                        {
                            // Waits to allow text to be shown before proceeding
                            Console.WriteLine("Too many invalid attemtps. Please try again in a few minutes.");

                            Thread.Sleep(2000);

                            // Freeze account and set time that account is frozen for
                            UnFreezeTime = DateTime.Now.AddMinutes(3);
                            AccountFreezed.FreezeUser(customerName, UnFreezeTime);
                        }

                    }
                    // If account IS frozen
                    else
                    {
                        Console.WriteLine("Too many invalid attemtps. Please try again in a few minutes.");

                        // Waits to allow text to be shown before proceeding
                        Thread.Sleep(2000);

                        // Checks if account can be unfrozen and unfreezes it if yes, does nothing if no
                        //AccountFreezed.UnFreezeUser(customerName);
                    }


                    // Newline for text formatting
                    Console.WriteLine();
                }
            }

        }          
    }
}
