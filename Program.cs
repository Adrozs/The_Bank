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
            // Removes so the blinking cursor isn't visible
            Console.CursorVisible = false;

            using (BankContext context = new BankContext())
            {

                // Check if an admin exists in the system upon first launch.
                // If none exists, create one. (We assume the first person to start the program is an admin).
                if (!DbHelpers.IsAdminCreated(context))
                {
                    AdminFunctions.CreateAdmin(context);
                }

                // Welcome screen
                MenuFunctions.header();
                MenuFunctions.main_header();
                MenuFunctions.footer();
                Console.WriteLine();
                Console.WriteLine();

                // Initalize counter to keep track of login attempts - 2 as default
                int loginAttempts = 2;

                // Declare date time variable to be used to keep track of when a user can be unfrozen if they've frozen their account
                DateTime UnFreezeTime;

                // Delcare username and user pin to be used for the login
                string customerName;
                string customerPin;

                // Re-promts the login screen until program is closed and while not in a method
                while (true)
                {

                    // Re-promts until login information is not null or empty
                    while (true)
                    {
                        // Login screen
                        MenuFunctions.header();

                        Console.WriteLine("\t\t\tLog in to your account:");
                        Console.Write("\t\t\tName: ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        customerName = MenuFunctions.CursorReadLine();
                        Console.ResetColor();

                        Console.Write("\t\t\tPIN: ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        customerPin = MenuFunctions.CursorReadLine();
                        Console.ResetColor();

                        MenuFunctions.footer();
                        Thread.Sleep(500);

                        // Checks if either name or pin is null or empty
                        if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(customerPin))
                        {
                            Console.WriteLine("\t\tName and/or PIN can't be empty");
                            MenuFunctions.PressEnter("\t\tPress [Enter] to try again");
                        }
                        else
                            break;
                    }


                    // Check if login is admin login  
                    if (DbHelpers.VerifyAdminLogin(context, customerName, customerPin))
                    {
                        // Goes to the admin menu
                        AdminFunctions.AdminMenu(context);
                    }
                    // If not admin continue with the rest of the code
                    else if (DbHelpers.DoesUserExist(context, customerName))
                    {
                        // Checks if account is NOT frozen
                        if (!AccountFreezed.IsFreezed(customerName))
                        {

                            // Checks if user login is correct
                            if (DbHelpers.VerifyUserLogin(context, customerName, customerPin))
                            {
                                // Reset login attempts and go to UserMenu
                                loginAttempts = 2;
                                UserFunctions.UserMenu(context, customerName);
                            }
                            // Checks if there are any login attempts left
                            else if (loginAttempts > 0)
                            {
                                Console.WriteLine($"\t\tUsername or pin is invalid. {loginAttempts} tries left.");
                                loginAttempts--;

                                // Waits to allow text to be shown before proceeding
                                MenuFunctions.PressEnter("\t\tPress [Enter] to try again");
                            }
                            // If too many invalid login attempts were made freeze account for x amount of time
                            else
                            {
                                // Waits to allow text to be shown before proceeding
                                Console.WriteLine("\t\tToo many invalid attemtps. \nPlease try again in a few minutes.");

                                MenuFunctions.PressEnter("\t\tPress [Enter] to try again");

                                // Freeze account and set time that account is frozen for
                                UnFreezeTime = DateTime.Now.AddMinutes(3);
                                AccountFreezed.FreezeUser(customerName, UnFreezeTime);
                            }
                        }
                        // If account IS frozen
                        else
                        {
                            Console.WriteLine("Too many invalid attemtps. Please try again in a few minutes.");

                            // Checks if account can be unfrozen and unfreezes it if yes, does nothing if no
                            AccountFreezed.UnFreezeUser(customerName);

                            MenuFunctions.PressEnter("\t\tPress [Enter] to try again");
                        }

                    }
                    // If account doesn't already exist
                    else
                    {
                        Console.WriteLine($"\t\tNo user with the name \"{customerName}\" exists.");
                        Console.WriteLine($"\t\tContact the admin to create a new user. ");

                        MenuFunctions.PressEnter("\t\tPress [Enter] to try again");
                    }

                    // Newline for text formatting
                    Console.WriteLine();
                }
                

            }
        }      
    }
}
