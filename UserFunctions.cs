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
                    DepositMoney();
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
            if (decimal.TryParse(Console.ReadLine(), out decimal withdraw))
            {
                var account = context.Accounts
                 .Where(a => a.Name == accountChoice)
                 .SingleOrDefault();

                if (account != null)
                {

                    var balance = account.Balance;

                    decimal newBalance = (decimal)balance - withdraw;

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
                    
                    account.Balance = (double)newBalance;
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
                        Console.WriteLine($"\t\t{account.Name}: {account.Balance:C}");
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
                    Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance:C}");
                }

                // select source account number
                Console.Write("Enter the source account number: ");
                if (int.TryParse(Console.ReadLine(), out int sourceAccountId))
                {
                    // SOURCE ACCOUNT
                    Account sourceAccount = user.Accounts.SingleOrDefault(a => a.Id == sourceAccountId);

                    if (sourceAccount != null)
                    {
                        // select destination account (no money laundering pls)
                        Console.WriteLine("Select the destination account to transfer money to:");
                        foreach (var account in user.Accounts.Where(a => a.Id != sourceAccountId))
                        {
                            Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance:C}");
                        }

                        Console.Write("Enter the destination account number: ");
                        if (int.TryParse(Console.ReadLine(), out int destinationAccountId))
                        {
                            // destination account find it
                            Account destinationAccount = user.Accounts.SingleOrDefault(a => a.Id == destinationAccountId);

                            if (destinationAccount != null)
                            {
                                // HOW MUCH
                                Console.Write("Enter the transfer amount: ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount) && transferAmount > 0)

                                {
                                    // you got the cash or u broke?
                                    if (sourceAccount.Balance >= transferAmount)
                                    {
                                        // update balance on both ends
                                        sourceAccount.Balance -= transferAmount;
                                        destinationAccount.Balance += transferAmount;

                                        // SAVE
                                        context.SaveChanges();

                                        // DISPLAY UPDATED BALANCE PLS WORK
                                        Console.WriteLine($"Transfer successful! New balance for {sourceAccount.Name}: {sourceAccount.Balance:C}");
                                        Console.WriteLine($"New balance for {destinationAccount.Name}: {destinationAccount.Balance:C}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Insufficient funds in the source account. Transfer canceled.");
                                    }
                                }//errorcode heaven
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
                    Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance:C}");
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
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawalAmount) && withdrawalAmount > 0)
                        {
                            // U got enough cash? or you broke
                            if (selectedAccount.Balance >= withdrawalAmount)
                            {
                                // Update account balance (or not if u broke
                                selectedAccount.Balance -= withdrawalAmount;

                                // SAVE IT
                                context.SaveChanges();

                                // Display balance
                                Console.WriteLine($"Withdrawal successful! New balance for {selectedAccount.Name}: {selectedAccount.Balance:C}");
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
        private static void DepositMoney()
        {
            Console.WriteLine("How much do you wish to deposit?");
            decimal depositAmount;

            if (decimal.TryParse(Console.ReadLine(), out depositAmount))
            {
                Console.WriteLine("Which account?");
                string accountChoice = Console.ReadLine();

                using (BankContext context = new BankContext()) 
                {
                         var user = context.Users
                        .Include(a => a.Accounts)
                        .FirstOrDefault(a => a.Name == accountChoice);

                    if (user != null)
                    {
                        Console.WriteLine($"Depositing {depositAmount} into {accountChoice}");

                        var account = user.Accounts.FirstOrDefault();

                        if (account != null)
                        {
                            account.Balance += depositAmount;
                            context.SaveChanges();
                            Console.WriteLine($"New balance {account.Balance}");
                        }
                        else
                        {
                            Console.WriteLine("User doesn't have any accounts.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"User with the name {accountChoice} not found.");
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

                // Loop until not true
                while (true)
                {
                    // Enter account name
                    Console.WriteLine("Enter new account name: ");
                    newAccountName = Console.ReadLine();

                    if (string.IsNullOrEmpty(newAccountName))
                    {
                        Console.WriteLine("Error! Name cannot be empty \n");
                    }
                    else
                        break;
                }

                // Creates new user object of the user that's logged in
                User user = DbHelpers.GetUser(context, userName);

                // Create new account type with id and Name of current user and starting balance of 0
                Account account = new Account()
                {
                    UserId = user.Id,
                    Name = newAccountName,
                    Balance = 0,
                };

                // Save account to database
                bool success = DbHelpers.AddAccount(context, account);
                if (success)
                {
                    Console.WriteLine($"Created new account {newAccountName} for user {userName}");
                }
                // If wasn't possible to save account to database, print error
                else
                {
                    Console.WriteLine($"Failed to create account {newAccountName}");
                    Console.WriteLine("Returning to menu");
                }

                // Waits for user to press enter to continue
                Console.WriteLine("Press [Enter] to go main menu");
                ConsoleKeyInfo key = Console.ReadKey(true); // True means it doesn't output the pressed key - looks better

                // Loops until user presses Enter
                while (key.Key != ConsoleKey.Enter)
                    key = Console.ReadKey(true); // True means it doesn't output the pressed key - looks better

                // New line for text formatting
                Console.WriteLine();
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