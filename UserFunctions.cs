using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using System.Text;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    internal class UserFunctions
    {
        internal static void UserMenu(BankContext outerContext, string userName)
        {
            using (BankContext context = new BankContext())
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("\nChoose one of the following options:");
                    Console.WriteLine("1. Display Account Balances");
                    Console.WriteLine("2. Transfer Money");
                    Console.WriteLine("3. Withdraw Money");
                    Console.WriteLine("4. Deposit Money");
                    Console.WriteLine("5. Open New Account");
                    Console.WriteLine("6. Change PIN");
                    Console.WriteLine("7. Exit");
                    Console.ResetColor();
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ViewAccountInfo(outerContext, userName);
                            break;
                        case "2":
                            TransferMoney(outerContext, userName);
                            break;
                        case "3":
                            WithdrawMoney(outerContext, userName);
                            break;
                        case "4":
                            DepositMoney(outerContext, userName);
                            break;
                        case "5":
                            OpenNewAccount(outerContext, userName);
                            break;
                        case "6":
                            ChangePin(outerContext, userName);
                                break;
                        case "7":
                            return;
                        default:
                            Console.WriteLine("Error! Please try again.");
                            break;
                    }
                }
            }
        }

        private static void ViewAccountInfo(BankContext context, string userName)
        {

            User user = context.Users
               .Where(u => u.Name == userName)
               .Include(u => u.Accounts)
               .Single();


            if (user != null)
            {
                Console.WriteLine($"User: {user.Name}\n");
                Console.WriteLine("Your accounts and balance:");

                foreach (var account in user.Accounts)
                {
                    Console.WriteLine($"{account.Name}: {account.Balance:C}");
                }
            }
            else
            {
                Console.WriteLine("User not found");
            }
        }

        // Transfer money between accounts
        private static void TransferMoney(BankContext context, string userName)
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
        private static void WithdrawMoney(BankContext context, string userName)
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
        private static void DepositMoney(BankContext context, string userName)
        {
            // Get info from database
            User user = context.Users
                .Include(u => u.Accounts)
                .Single(u => u.Name == userName);

            // Display all accounts
            Console.WriteLine("Select the account to deposit money into:");
            foreach (var account in user.Accounts)
            {
                Console.WriteLine($"{account.Id}. {account.Name}: {account.Balance:C}");
            }

            // CHOOSE IT NOW
            Console.Write("Enter the account number: ");
            if (int.TryParse(Console.ReadLine(), out int selectedAccountId))
            {
                // Find selected account
                Account selectedAccount = user.Accounts.SingleOrDefault(a => a.Id == selectedAccountId);

                if (selectedAccount != null)
                {
                    // How much do you want to deposit?
                    Console.Write("Enter the deposit amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount) && depositAmount > 0)
                    {
                        // Update the balance
                        selectedAccount.Balance += depositAmount;

                        // save it
                        context.SaveChanges();

                        // Display new balance (sadly not the shoes)
                        Console.WriteLine($"Deposit successful! New balance for {selectedAccount.Name}: {selectedAccount.Balance:C}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid deposit amount. Please enter a valid positive number.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid account number. Please select a valid account.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid account number.");
            }
        }


        // Create a new account
        private static void OpenNewAccount(BankContext context, string username)
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
            User user = DbHelpers.GetUser(context, username);

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
                Console.WriteLine($"Created new account {newAccountName} for user {username}");
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
        private static void ChangePin(BankContext context, string username)
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
            while(true)
            {
                Console.WriteLine("Enter new PIN: ");
                string newPin = Console.ReadLine();
        private static void DepositMoney()
        {
            using (BankContext context = new BankContext())
            {
                Console.WriteLine("How much do you wish to deposit?");
                double deposit = double.Parse(Console.ReadLine());

                Console.WriteLine("Confirm new PIN: ");
                string newPinConfirm = Console.ReadLine();
                if (double.TryParse(Console.ReadLine(), out double depositAmount))
                { 
                    var account = context.Accounts
                     .Where(a => a.Balance > 0)
                     .FirstOrDefault();

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
                    if (account != null)
                    {
                        account.Balance += depositAmount;
                        context.SaveChanges();

                        Console.WriteLine($"Deposit successful. New balance: {account.Balance}");
                    }
                }

                else
                {
                    Console.WriteLine("Invalid choice");
                }
            }
        }
    }
}
