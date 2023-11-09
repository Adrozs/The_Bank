using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    internal class UserFunctions
    {
        internal static void UserMenu(string userName)
        {
            using (BankContext context = new BankContext())
            {            
                while (true)
                {
                    Console.WriteLine("Choose one of the following options:");
                    Console.WriteLine("1. View all of your acounts and balance");
                    Console.WriteLine("2. Transfer Balance");
                    Console.WriteLine("3. Withdrawal");
                    Console.WriteLine("4. Deposit");
                    Console.WriteLine("5. Open a new account");
                    //Console.WriteLine("6. Currency Converter"); Do later if have extra time
                    Console.WriteLine("7. Log out.");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            DisplayAccountBalances(context, userName);
                            break;
                        case "2":
                            TransferMoney(context, userName);
                            break;
                        case "3":
                            WithdrawMoney(context, userName);
                            break;
                        case "4":
                            DepositMoney(context, userName);
                            break;
                        case "5":
                            OpenNewAccount(context, userName);
                            break;
                        // If we have time over we can fix currency conversion
                        //case "6":
                        //    currentUser = null;
                        //    return;
                        case "7":
                            return;
                            break;
                        default:
                            Console.WriteLine("Error! Please try again.");
                            break;
                    }
                }
            }  
        }
        private static void DisplayAccountBalances(BankContext context, string userName)
        {
            Console.WriteLine("Your Accounts and Balances:");

            // Retrieve user information and their accounts from the database
            User user = context.Users
                .Include(u => u.Accounts)
                .Single(u => u.Name == userName);

            // Display each account and its balance
            foreach (var account in user.Accounts)
            {
                Console.WriteLine($"{account.Name}: {account.Balance:C}");
            }

            // Add a newline for better formatting
            Console.WriteLine();
        }


        private static void TransferMoney(BankContext context, string userName)
        {
            // Your transfer logic goes here
            // Prompt user for source account, destination account, and amount

            // Check if source account has enough balance
            // If not, inform the user and return

            // Update balances of source and destination accounts

            // Display updated balances
            Console.WriteLine($"Transfer successful! Updated balances:");
            // Display balances of source and destination accounts
        }

        private static void WithdrawMoney(BankContext context, string userName)
        {
            // withdraw
            // Which account and how much?

            // CONFIRM W/ PIN

            // Balance - Amount = NewBalance (not the shoes)

            // Show new balance
            Console.WriteLine("You have successfully completed your withdrawal! New balance:");
            // 
        }

        private static void DepositMoney(BankContext context, string userName)
        {
            // Deposit
            // which account and how much?

            // Display the updated balance
            Console.WriteLine("You have successfully completed your deposit! New balance:");
            // Display the balance of the selected account
        }

        // Create a new account
        private static void OpenNewAccount(BankContext context, string username)
        {
            // Declare new account variable outside of loop
            string newAccountName;

            // loop until not true
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
            User user = context.Users
                .Where(u => u.Name == username)
                .Single(); // TODO: Change to SingleOrDefault() and add exeption handling later

            // Create new account type with UserId and Name of current user and starting balance of 0
            Account account = new Account()
            {
                UserId = user.Id,
                Name = user.Name,
                Balance = 0,
            };

            // Save account to database
            bool success = DbHelpers.AddAccount(context, account);
            if (success)
            {
                Console.WriteLine($"Created new account {newAccountName} for user {username}");
            }
            // If wasn't possible to save account to database, print error
            else
            {
                Console.WriteLine($"Failed to create account {newAccountName}");
                Console.WriteLine("Returning to menu");
            }
        }
    }
}
