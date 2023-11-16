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

                        MenuFunctions.footer();

                        // Promts user to press enter key to go back to menu - doesn't accept any other input
                        MenuFunctions.PressEnter("\t\tPress [Enter] to go to main menu");
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
        public static void WithdrawMoney(BankContext context)
        {
            Console.WriteLine("From which account do you want to withdraw from?:");
            string accountChoice = Console.ReadLine();
            
            Console.Write("How much would you like to withdraw? ");
            if (double.TryParse(Console.ReadLine(), out double withdraw))
            {
                var account = context.Accounts
                 .Where(a => a.Name == accountChoice)
                 .SingleOrDefault();

                if (account != null)
                {

                    var balance = account.Balance;

                    double newBalance = balance - withdraw;

                    //Errorchecks
                    if (newBalance < 0)
                    {
                        Console.WriteLine($"Cannot withdraw more than {balance}");
                        return;
                    }
                    if (withdraw == 0)
                    {
                        Console.WriteLine("Cannot withdraw 0");
                        return;
                    }
                    
                    account.Balance = newBalance;
                    context.SaveChanges();
                    Console.WriteLine($"You new balance is {newBalance}");

                }
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
        }


        // Withdraw money from an account
        static void WithdrawMoney(BankContext context, string userName)
            {
                // get info from database
                User user = context.Users
                    .Include(u => u.Accounts)
                    .Single(u => u.Name == userName);

                // X accounts Y numbers
                Console.WriteLine("Select the account to withdraw money from:");
                foreach (var account in user.Accounts)
                {
                    Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance}");
                }

            // CHOOSE AN ACCOUNT
            Console.Write("Enter the account number: ");
            if (int.TryParse(Console.ReadLine(), out int selectedAccountId))
            {
                // FIND account
                Account selectedAccount = user.Accounts.SingleOrDefault(a => a.Id == selectedAccountId);

                    if (selectedAccount != null)
                    {
                        // HOW MUCH DO U WANT TO WITHDRAW
                        Console.Write("Enter the withdrawal amount: ");
                        if (double.TryParse(Console.ReadLine(), out double withdrawalAmount) && withdrawalAmount > 0)
                        {
                            // U got enough cash? or you broke
                            if (selectedAccount.Balance >= withdrawalAmount)
                            {
                                // Update account balance (or not if u broke
                                selectedAccount.Balance -= withdrawalAmount;

                            // SAVE IT
                            context.SaveChanges();

                                // Display balance
                                Console.WriteLine($"Withdrawal successful! New balance for {selectedAccount.Name}: {selectedAccount.Balance}");
                            }
                            else
                            {
                                Console.WriteLine("Insufficient funds in the account. Withdrawal canceled.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid withdrawal amount. Please enter a valid positive number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid account number. Please select a valid account.");

                    // New line for text formatting
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid account number.");
            }
        }

        // Deposit money to account
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
        }

        // Create a new account for logged in user
        static void OpenNewAccount(BankContext context, string userName)
        {
            // Clear window
            Console.Clear();

            // Declare new account variable outside of loop
            string newAccountName;

            while (true)
            {
                Console.WriteLine("Enter new account name: ");
                newAccountName = Console.ReadLine();

                // Checks if string is null or empty or if an account with that name already exist for current user
                if (string.IsNullOrEmpty(newAccountName))
                {
                    Console.WriteLine("Error! Name cannot be empty. \n");
                }
                else if (DbHelpers.AccountAlreadyExist(context, userName, newAccountName))
                {
                    Console.WriteLine("Error! You already have an account with that name. \n");
                }
                else
                    break;
            }

            // Set Swedish Krona (SEK) as the default currency
            string selectedCurrency = "SEK";


            // TODO make a menu option of yes and no like our other menus instead of a Y/N option

            // Ask if the user wants to create a foreign currency account
            Console.Write("Will this account be in a foreign currency? (Y/N): ");
            bool isForeignAccount = Console.ReadLine().Trim().ToUpper() == "Y";

            if (isForeignAccount)
            {
                while (true)
                {
                    Console.WriteLine("Available foreign currencies:");
                    Console.WriteLine("1. US Dollar (USD)");
                    Console.WriteLine("2. Euro (EUR)");
                    Console.WriteLine("3. UK Sterling (GBP)");
                    Console.WriteLine("4. Swiss Franc (CHF)");
                    Console.WriteLine("5. Canadian Dollar (CAD)");
                    Console.WriteLine("6. Zimbabwean Dollar (ZWD)");
                    // ADD MORE CURRENCIES HERE

                    // Create array of available currencies
                    string[] currencies = { "SEK", "USD", "EUR", "GBP", "CHF", "CAD", "ZWD" };

                    // Prompts user to choose currency and checks if input is a number
                    Console.Write("Select which currency: ");

                    // Takes input from user and checks if it's a number between 0 and the amount of currencies in the currencies array - this is scalable if we add more currencies in the future
                    if (int.TryParse(Console.ReadLine(), out int currencyChoice) && (currencyChoice > 0 && currencyChoice <= currencies.Length))
                    {
                        // Changes currency from the default SEK to the chosen one and breaks out of the loop to continue with the rest of the code
                        selectedCurrency = currencies[currencyChoice];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid currency number.");
                    }
                }
            }

            // Creates new user object of the user that's logged in
            User user = DbHelpers.GetUser(context, userName);

            // Creates a new account object on the logged in user with the chosen name and currency (SEK unless it's foreign account) with a starting balance of 0
            Account account = new Account()
            {
                UserId = user.Id,
                Name = newAccountName,
                Balance = 0,
                Currency = selectedCurrency
            };


            // TODO make a menu option of yes and no like our other menus instead of a Y/N option

            // Ask if the user wants to make an initial deposit
            Console.Write($"Do you wish make a deposit to {newAccountName}? (Y/N): ");
            if(Console.ReadLine().Trim().ToUpper() == "Y")
            {
                // Prompts user intil a correct value is entered
                while (true)
                {

                    // Asks user for deposit amount and checks if value is correct. If yes changes account balance to chosen amount and breaks out of the loop to continue with the rest of the code.
                    Console.Write($"Enter deposit amount in {selectedCurrency}: ");
                    if (double.TryParse(Console.ReadLine(), out double initialDeposit) && initialDeposit >= 0)
                    {
                        account.Balance = initialDeposit;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid deposit amount. Please enter a valid positive number.");
                    }
                }
            }

            // Attempts to add account to database. Returns true or false if successful or not
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