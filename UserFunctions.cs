using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;
using The_Bank.Migrations;
using System.Security.Principal;
using System.Runtime.CompilerServices;

namespace The_Bank
{
    public class UserFunctions
    {


        public static void UserMenu(BankContext outerContext, string userName)
        {
            int menuSelection = 1; // Start from the first option

            using (BankContext context = new BankContext())
            {
                while (true)
                {
                    Console.Clear(); //clears the console
                    //Console.ForegroundColor = ConsoleColor.Green; //commenting this out since i like the defualt color

                    //Option menu
                    MenuFunctions.header();
                    Console.WriteLine("\t\tChoose one of the following options:");
                    MenuFunctions.divider();

                    for (int i = 1; i <= 7; i++) //Forloop to change color on the option the arrow "goes" to.
                    {
                        if (i == menuSelection) // so when menuSelection is for exemple "2" the second option will turn darkgrey
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray; // Change to your preferred color
                        }

                        Console.WriteLine($"\t\t{ColorOptionText(i)}{(menuSelection == i ? " <--" : "")}");

                        Console.ResetColor(); // Reset color to default
                    }

                    MenuFunctions.footer();
                    //"Listen" to keystrokes from the user
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    //Handles the arrow keys to move up and down the menu
                    if (key.Key == ConsoleKey.UpArrow && menuSelection > 1)
                    {
                        menuSelection--;
                    }
                    else if (key.Key == ConsoleKey.DownArrow && menuSelection < 7)
                    {
                        menuSelection++;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        // Method that calls a method to perform the action based on the selected option
                        HandleMenuSelection(context, menuSelection, userName);
                    }
                }
            }
        }

        static void HandleMenuSelection(BankContext context, int selection, string userName)
        {
            switch (selection)
            {
                case 1:
                    Console.Clear();
                    ViewAccountInfo(context, userName);
                    break;
                case 2:
                    TransferMoney(context, userName);
                    break;
                case 3:
                    WithdrawMoney(context, userName);
                    break;
                case 4:
                    DepositMoney(context, userName);
                    break;
                case 5:
                    OpenNewAccount(context, userName);
                    break;
                case 6:
                    ChangePin(context, userName);
                    break;
                case 7:
                    return;
                default:
                    Console.WriteLine("Error! Please try again.");
                    break;
            }
        }

        private static string ColorOptionText(int option)
        {
            switch (option)
            {
                case 1:
                    return "View all of your accounts and balance";
                case 2:
                    return "Transfer Balance";
                case 3:
                    return "Withdrawal";
                case 4:
                    return "Deposit";
                case 5:
                    return "Open a new account";
                case 6:
                    return "Change PIN";
                case 7:
                    return "Log out";
                default:
                    return "Unknown Option";
            }


            
        }
        public static void WithdrawMoney(BankContext context, string userName)
        {   
            Console.Write("\t\t\tPIN: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string customerPin = Console.ReadLine();
            Console.ResetColor();
            Console.WriteLine();
            if (DbHelpers.IsCustomer(context, userName, customerPin))
            {
                ViewAccountInfo(context, userName);
                Console.WriteLine("\t\tFrom which account do you want to withdraw from?:");
                Console.Write("\t\t");
                string accountChoice = Console.ReadLine();
                if (string.IsNullOrEmpty(accountChoice)) //Checks that user have written anything
                {
                    Console.WriteLine("\t\tYou have to put in a valid account name.");
                    Console.ReadKey();
                    return;
                }
                if (DbHelpers.AccountAlreadyExist(context, userName, accountChoice)) //Checks that the user input matches existing account.
                {
                    Console.Write("\t\tHow much would you like to withdraw? ");
                    if (double.TryParse(Console.ReadLine(), out double withdraw))
                    {
                        var account = context.Accounts
                         .Include(a => a.User)
                         .SingleOrDefault(a => a.Name == accountChoice && a.User.Name == userName);

                        if (account != null) 
                        {
                            double balance = account.Balance;
                            //Errorchecks
                            if (withdraw > balance)
                            {
                                Console.WriteLine($"\t\tCannot withdraw more than: {balance}");
                                Console.ReadKey(true);
                                return;
                            }
                            if (withdraw <= 0)
                            {
                                Console.WriteLine("\t\tCannot withdraw 0 or less");
                                Console.ReadKey(true);
                                return;
                            } 
                            // Counts the new balance after withdrawal then saves it in DB
                            double newBalance = balance - withdraw;
                            account.Balance = newBalance;
                            context.SaveChanges();
                            Console.WriteLine($"\t\tYou new balance is: {newBalance}");
                            Console.ReadKey(true);
                        }
                    }
                    else { Console.WriteLine("\t\tPlease write a valid number."); Console.ReadKey(true); return; }
                }
                else { Console.WriteLine("\t\tThis account name does not exist. Please write an existing account name."); Console.ReadKey(true); return;}
            }
            else
            {
                Console.WriteLine("\t\tInvalid PIN. Try again.");
                Console.ReadKey(true);
                return;
            }
        }

        //private static void DisplayAccountBalances(BankContext context, string userName)
        private static void ViewAccountInfo(BankContext context, string userName)
        {
            //Get info about user from database
            User user = context.Users
                .Include(u => u.Accounts)
                .Single(u => u.Name == userName);

            //Display user accounts and balance
            if (user != null)
            {

                MenuFunctions.header();
                Console.WriteLine($"\t\tUser: {user.Name}\n");
                Console.WriteLine("\t\tYour accounts and balance:");
                MenuFunctions.footer();

                    foreach (var account in user.Accounts)
                    {
                        Console.WriteLine($"\t\t{account.Name}: {account.Balance} {account.Currency}");
                    }
                }
                else
                {
                    Console.WriteLine("\t\tUser not found");
                }

            Console.WriteLine("\t\tPress [Enter] to go main menu");
            ConsoleKeyInfo key = Console.ReadKey(true);
        }

        // Transfer money between accounts
        // Transfer money between accounts
        static void TransferMoney(BankContext context, string userName)
        {
            // Get user info from Database
            User user = context.Users
                .Include(u => u.Accounts)
                .Single(u => u.Name == userName);

            // Display user accounts
            Console.WriteLine("Select the source account to transfer money from:");
            foreach (var account in user.Accounts)
            {
                Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance} {account.Currency}");
            }

            // Select source account number
            Console.Write("Enter the source account number: ");
            if (int.TryParse(Console.ReadLine(), out int sourceAccountId))
            {
                // SOURCE ACCOUNT
                Account sourceAccount = user.Accounts.SingleOrDefault(a => a.Id == sourceAccountId);

                if (sourceAccount != null)
                {
                    // Select destination account (only user's own accounts)
                    Console.WriteLine("Select the destination account to transfer money to:");
                    foreach (var account in user.Accounts.Where(a => a.Id != sourceAccountId))
                    {
                        Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance} {account.Currency}");
                    }

                    Console.Write("Enter the destination account number: ");
                    if (int.TryParse(Console.ReadLine(), out int destinationAccountId))
                    {
                        // Destination account
                        Account destinationAccount = user.Accounts.SingleOrDefault(a => a.Id == destinationAccountId);

                        if (destinationAccount != null)
                        {
                            // Transfer amount
                            Console.Write("Enter the transfer amount: ");
                            if (double.TryParse(Console.ReadLine(), out double transferAmount) && transferAmount > 0)
                            {
                                // Check if the source account has sufficient funds
                                if (sourceAccount.Balance >= transferAmount)
                                {
                                    // If source and destination accounts have different currencies, perform currency conversion
                                    if (sourceAccount.Currency != destinationAccount.Currency)
                                    {
                                        // Use the CurrencyConverter to convert the amount to the destination currency
                                        double convertedAmount = CurrencyConverter.Convert(sourceAccount.Currency, destinationAccount.Currency, transferAmount);

                                        // Update balances
                                        sourceAccount.Balance -= transferAmount;
                                        destinationAccount.Balance += convertedAmount;

                                        // Save changes
                                        context.SaveChanges();

                                        // Display updated balances
                                        Console.WriteLine($"Transfer successful! New balance for {sourceAccount.Name}: {sourceAccount.Balance} ({sourceAccount.Currency})");
                                        Console.WriteLine($"New balance for {destinationAccount.Name}: {destinationAccount.Balance} ({destinationAccount.Currency})");
                                    }
                                    else
                                    {
                                        // Same currency, no conversion needed
                                        sourceAccount.Balance -= transferAmount;
                                        destinationAccount.Balance += transferAmount;

                                        // Save changes
                                        context.SaveChanges();

                                        // Display updated balances
                                        Console.WriteLine($"Transfer successful! New balance for {sourceAccount.Name}: {sourceAccount.Balance} ({sourceAccount.Currency})");
                                        Console.WriteLine($"New balance for {destinationAccount.Name}: {destinationAccount.Balance} ({destinationAccount.Currency})");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Insufficient funds in the source account. Transfer canceled.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid transfer amount. Please enter a valid positive number.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid destination account number. Please select a valid account.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid destination account number.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid source account number. Please select a valid account.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid source account number.");
            }

            // Display message and wait for Enter key press to return to the main menu
            Console.WriteLine("Press [Enter] to return to the main menu");
            Console.ReadLine();
        }

        //// Deposit money to account
        private static void DepositMoney(BankContext context, string username)
        {
            ViewAccountInfo(context, username);

            Console.WriteLine("How much do you wish to deposit?");
            double depositAmount;

            if (double.TryParse(Console.ReadLine(), out depositAmount))
            {
                Console.WriteLine("Which account?");
                string accountChoice = Console.ReadLine();

                var account = context.Accounts
                    .FirstOrDefault(a => a.Name == accountChoice);

                if (account != null)
                {
                    Console.WriteLine($"Depositing {depositAmount} into {accountChoice}");

                    account.Balance += depositAmount;
                    context.SaveChanges();
                    Console.WriteLine($"New balance {account.Balance}");
                }
                else
                {
                    Console.WriteLine("Account not found. Do you want to open a new account? (Y/N)");

                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        OpenNewAccount(context, username);
                    }
                    else
                    {
                        Console.WriteLine("Deposit operation cancelled.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid deposit amount entered.");
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
        }

        // Create a new account
        static void OpenNewAccount(BankContext context, string userName)
        {
            // Declare new account variable outside of loop
            string newAccountName;

            while (true)
            {
                Console.WriteLine("Enter new account name: ");
                newAccountName = Console.ReadLine();

                if (string.IsNullOrEmpty(newAccountName))
                {
                    Console.WriteLine("Error! Name cannot be empty \n");
                }
                else
                {
                    break;
                }
            }

            // Creates new user object of the user that's logged in
            User user = DbHelpers.GetUser(context, userName);

            // Ask if the user wants to create a "Vacation" account
            Console.Write("Do you want to create a 'Vacation' account? (Y/N): ");
            bool isVacationAccount = Console.ReadLine().Trim().ToUpper() == "Y";

            Account account = new Account()
            {
                UserId = user.Id,
                Name = newAccountName,
                Balance = 0,
                Currency = "SEK", // Default currency is Swedish Krona (SEK)
            };

            if (isVacationAccount)
            {
                Console.WriteLine("Select the currency for the 'Vacation' account:");
                Console.WriteLine("1. US Dollar (USD)");
                Console.WriteLine("2. Euro (EUR)");
                Console.WriteLine("3. UK Sterling (GBP)");
                Console.WriteLine("4. Swiss Franc (CHF)");
                Console.WriteLine("5. Canadian Dollar (CAD)");
                Console.WriteLine("6. Zimbabwean Dollar (ZWD)");
                // ADD MORE CURRENCIES HERE

                Console.Write("Enter the currency number: ");
                if (int.TryParse(Console.ReadLine(), out int currencyChoice))
                {
                    string[] currencies = { "SEK", "USD", "EUR", "GBP", "CHF", "CAD", "ZWD" };
                    string selectedCurrency = currencies[currencyChoice];

                    Console.Write($"Enter the initial deposit in {selectedCurrency}: ");
                    if (double.TryParse(Console.ReadLine(), out double initialDeposit) && initialDeposit >= 0)
                    {
                        account.Currency = selectedCurrency;
                        account.Balance = initialDeposit;
                    }
                    else
                    {
                        Console.WriteLine("Invalid deposit amount. Please enter a valid positive number.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid currency number.");
                    return;
                }
            }

            bool success = DbHelpers.AddAccount(context, account);

            if (success)
            {
                Console.WriteLine($"Created new account '{newAccountName}' for user '{userName}'");
            }
            else
            {
                Console.WriteLine($"Failed to create account '{newAccountName}'");
                Console.WriteLine("Returning to menu");
                return;
            }

            Console.WriteLine("Press [Enter] to go to the main menu");
            Console.ReadLine(); // This waits for Enter Key

            Console.WriteLine(); // New line for text formatting
        }


        // Changes current pin to a new pin for a user
        static void ChangePin(BankContext context, string username)
            {
                User user = DbHelpers.GetUser(context, username);

            // Asks user for pin and checks if it matches login
            while (true)
            {
                Console.WriteLine("Enter current PIN: ");
                string currentPin = Console.ReadLine();

                // Re-promt user until string isn't empty
                while (string.IsNullOrEmpty(currentPin))
                {
                    Console.WriteLine("Error! PIN can't be empty");
                    currentPin = Console.ReadLine();
                }

                // If pin matches login info, break out of loop
                if (DbHelpers.IsCustomer(context, username, currentPin))
                    break;
                else
                    Console.WriteLine("Error! Wrong PIN. Try again. \n");
            }

            // Re-promt user for pins until 2 consecutive pins match
            while (true)
            {
                Console.WriteLine("Enter new PIN: ");
                string newPin = Console.ReadLine();

                Console.WriteLine("Confirm new PIN: ");
                string newPinConfirm = Console.ReadLine();

                // If pins match save them to database and break out of loop - else write error message
                if (newPin == newPinConfirm)
                {
                    bool success = DbHelpers.EditPin(context, user, newPin);
                    if (success)
                    {
                        Console.WriteLine($"Changed PIN to {newPin} for user {username}");
                    }
                    // If wasn't possible to save account to database, print error
                    else
                    {
                        Console.WriteLine($"Failed to update PIN to {newPin} for {username}");
                        Console.WriteLine("Returning to menu");
                    }
                    break;
                }
                else
                    Console.WriteLine("PIN codes doesn't match. Try again. \n");
            }
        }
    }
}