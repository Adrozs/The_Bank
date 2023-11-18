using Microsoft.EntityFrameworkCore;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    public class UserFunctions
    {
        public static void UserMenu(BankContext outerContext, string userName)
        {
            int menuSelection = 1; // Start from the first option

            bool isLoggedIn = true; // When user chooses "Log Out" this turns to false and exits loop to then go back to login screen

            using (BankContext context = new BankContext())
            {
                while (isLoggedIn)
                {
                    Console.Clear(); //clears the console
                    //Console.ForegroundColor = ConsoleColor.Green; //commenting this out since i like the defualt color

                    //Option menu
                    MenuFunctions.header();
                    Console.WriteLine("\t\t\t  ::Main Menu::");
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
                        // Returns false when user chooses option "Log out" this exits the menu loop and goes back to the login screen
                        isLoggedIn = HandleMenuSelection(context, menuSelection, userName);

                        MenuFunctions.footer();

                        // Promts user to press enter key to go back to menu - doesn't accept any other input
                        MenuFunctions.PressEnter("\t\tPress [Enter] to go to main menu");
                    }
                }
            }
        }

        public static bool HandleMenuSelection(BankContext context, int selection, string userName)
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
                    MenuFunctions.PrintFast("\t\tYou are now logging out.");
                    Thread.Sleep(2000);
                    return false;
                default:
                    Console.WriteLine("Error! Please try again.");
                    break;
            }
            return true;
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
            // Prompt the user for their PIN
            Console.Write("\t\t\tPIN: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string customerPin = MenuFunctions.CursorReadLine();
            Console.ResetColor();
            Console.WriteLine();

            // Check if the provided PIN is valid for the given user
            if (DbHelpers.VerifyUserLogin(context, userName, customerPin))
            {
                // Retrieve the user information, including accounts
                User user = context.Users
                    .Include(u => u.Accounts)
                    .SingleOrDefault(u => u.Name == userName);

                // Check if the user exists
                if (user == null)
                {
                    Console.WriteLine("\t\tUser not found. Withdrawal canceled.");
                    Console.ReadKey(true);
                    return;
                }

                // Allows the user to select an account using arrow keys and highlight the selection
                int chosenAccountPosition = MenuFunctions.OptionsNavigation(user.Accounts.Select(a => $"\t\t{a.Name}: {a.Balance} {a.Currency}").ToArray(), "\t\tChoose account to withdraw:");
                // Check if the selected account position is valid
                if (chosenAccountPosition < 0 || chosenAccountPosition >= user.Accounts.Count)
                {
                    Console.WriteLine("\t\tInvalid account selection. Withdrawal canceled.");
                    Console.ReadKey(true);
                    return;
                }
                //Retrieves the selected account
                Account selectedAccount = user.Accounts.ElementAt(chosenAccountPosition);

                // Prompt the user for the withdrawal amount
                MenuFunctions.footer();
                Console.Write("\t\tHow much would you like to withdraw? ");
                Console.CursorVisible = true;

                // Validates and processes the withdrawal amount
                if (double.TryParse(Console.ReadLine(), out double withdraw))
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    MenuFunctions.ClearCurrentConsoleLine();

                    double balance = selectedAccount.Balance;

                    // Check if the withdrawal amount is valid
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

                    // Update the account balance after the withdrawal and save changes to the database                 
                    double newBalance = balance - withdraw;
                    selectedAccount.Balance = newBalance;
                    context.SaveChanges();
                    // Display the new account balance to the user
                    Console.WriteLine($"\t\tYour new balance is: {newBalance}");
                    Console.ReadKey(true);
                }
                else
                {
                    Console.WriteLine("\t\tPlease write a valid number.");
                    Console.ReadKey(true);
                    return;
                }
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
        }

        // Transfer money between accounts
        static void TransferMoney(BankContext context, string userName)
        {
            // Retrieve the user information, including accounts
            User user = context.Users
             .Include(u => u.Accounts)
             .Single(u => u.Name == userName);

            //Checks if user exist
            if (user == null)
            {
                Console.WriteLine("\t\tUser not found. Transfer canceled.");
                Console.WriteLine("\t\tPress [Enter] to return to the main menu");
                MenuFunctions.CursorReadLine();
                return;
            }

            //Allows the user to select an account using arrow keys and highlight the selection
            MenuFunctions.header();
            int sourceAccountPosition = MenuFunctions.OptionsNavigation(user.Accounts.Select(a => $"\t\t{a.Name}: {Math.Round(a.Balance,2)} {a.Currency}").ToArray(),
            "\t\tSelect the source account to transfer money from:");


            // Check if the selected account position is valid
            if (sourceAccountPosition < 0 || sourceAccountPosition >= user.Accounts.Count)
            {
                Console.WriteLine("\t\tInvalid source account selection. Transfer canceled.");
                Console.WriteLine("\t\tPress [Enter] to return to the main menu");
                MenuFunctions.CursorReadLine();
                return;
            }
            //Makes a list of User Accounts
            List<Account> userAccountsList = user.Accounts.ToList();

            Account sourceAccount = userAccountsList[sourceAccountPosition];
            //Getting the user account info from database and turns it to a list
            List<string> destinationOptions = userAccountsList
               .Where(a => a.Id != sourceAccount.Id)
               .Select(a => $"\t\t{a.Name}: {Math.Round(a.Balance,2)} {a.Currency}")
               .ToList();

            //Allows the user to select an account using arrow keys and highlight the selection
            int destinationAccountPosition = MenuFunctions.OptionsNavigation(destinationOptions.ToArray(),
            "\t\tSelect the destination account to transfer money to:");
            //Promts the user to select an account to transfer money to

            //Checks if the chosen destination account is valid
            if (destinationAccountPosition < 0 || destinationAccountPosition >= user.Accounts.Count)
            {
                Console.WriteLine("\t\tInvalid destination account selection. Transfer canceled.");
                Console.WriteLine("\t\tPress [Enter] to return to the main menu");
                MenuFunctions.CursorReadLine();
                return;
            }

            Account destinationAccount = userAccountsList[destinationAccountPosition];

            //promts the user to enter the transfer amount
            Console.Write("\t\tEnter the transfer amount: ");
            if (double.TryParse(Console.ReadLine(), out double transferAmount) && transferAmount > 0)
            {
                if (sourceAccount.Balance >= transferAmount)
                {
                    if (sourceAccount.Currency != destinationAccount.Currency)
                    {
                        double convertedAmount = CurrencyConverter.Convert(sourceAccount.Currency, destinationAccount.Currency, transferAmount);
                        sourceAccount.Balance -= transferAmount;
                        destinationAccount.Balance += convertedAmount;
                        context.SaveChanges();
                        DisplayBalances(sourceAccount, destinationAccount);
                    }
                    else
                    {
                        sourceAccount.Balance -= transferAmount;
                        destinationAccount.Balance += transferAmount;
                        context.SaveChanges();
                        DisplayBalances(sourceAccount, destinationAccount);
                    }
                }
                else
                {
                    Console.WriteLine("\t\tInsufficient funds in the source account. Transfer canceled.");
                }
            }
            else
            {
                Console.WriteLine("\t\tInvalid transfer amount. Please enter a valid positive number.");
            }
            //method to print out new balances
            void DisplayBalances(Account source, Account destination)
            {
                MenuFunctions.footer();
                Console.WriteLine($"\t\tTransfer successful!");
                Console.WriteLine($"\t\tNew balance for {source.Name}: {Math.Round(source.Balance,2)} ({source.Currency})");
                Console.WriteLine($"\t\tNew balance for {destination.Name}: {Math.Round(destination.Balance,2)} ({destination.Currency})");
            }

        }


        // Deposit money to account
        private static void DepositMoney(BankContext context, string username)
        {
            User user = context.Users
            .Include(u => u.Accounts)
            .SingleOrDefault(u => u.Name == username);

            if (user == null)
            {
                Console.WriteLine("\t\tUser not found. Deposit canceled.");
                Console.ReadKey(true);
                return;
            }

            
            string[] accountOptions = user.Accounts.Select(a => $"\t\t{a.Name}: {a.Balance} {a.Currency}").ToArray();
            int chosenAccountPosition = MenuFunctions.OptionsNavigation(accountOptions, "\t\tChoose an account to deposit into:");

            if (chosenAccountPosition < 0 || chosenAccountPosition >= user.Accounts.Count)
            {
                Console.WriteLine("\t\tInvalid account selection. Deposit canceled.");
                Console.ReadKey(true);
                return;
            }

            Account selectedAccount = user.Accounts.ElementAt(chosenAccountPosition);

            Console.Write("\t\tHow much do you wish to deposit?");
            if (double.TryParse(Console.ReadLine(), out double depositAmount))
            {
                MenuFunctions.divider();
                Console.WriteLine($"\t\tDepositing {depositAmount} into {selectedAccount.Name}");

                selectedAccount.Balance += depositAmount;
                context.SaveChanges();
                Console.WriteLine($"\t\tNew balance: {selectedAccount.Balance}");
            }
            else
            {
                Console.WriteLine("\t\tInvalid deposit amount entered.");
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
                MenuFunctions.header();
                Console.Write("\t\tEnter new account name: ");
                newAccountName = MenuFunctions.CursorReadLine();

                // Checks if string is null or empty or if an account with that name already exist for current user
                if (string.IsNullOrEmpty(newAccountName))
                {
                    Console.WriteLine("\t\tError! Name cannot be empty. \n");
                }
                else if (DbHelpers.AccountAlreadyExist(context, userName, newAccountName))
                {
                    Console.WriteLine("\t\tError! You already have an account with that name. \n");
                }
                else
                    break;
            }

            // Set Swedish Krona (SEK) as the default currency
            string selectedCurrency = "SEK";


            // Declare array navOptions as yes or no
            string[] navOptions = { "\t\tYes", "\t\tNo" };

            // Passes along the options yes and no as well as the phrase to the nav method
            // Checks if it returns 0 (which translates to yes in this case) and runs the code, otherwise skips.
            if (MenuFunctions.OptionsNavigation(navOptions, "\t\tWill this account be in a foreign currency?") == 0)
            {
                // Create array of all the currenies to choose from
                string[] currencyOptions = {
                "\t\tUS Dollar (USD)", "\t\tEuro (EUR)", "\t\tUK Sterling (GBP)", "\t\tSwiss Franc (CHF)",
                "\t\tCanadian Dollar (CAD)", "\t\tZimbabwean Dollar (ZWD)" 
                // ADD MORE CURRENCIES HERE
                };

                // Create array of available currency shortname
                string[] currencies = { "USD", "EUR", "GBP", "CHF", "CAD", "ZWD" };


                // Prompts user to choose one of the foreign 
                int chosenOption = MenuFunctions.OptionsNavigation(currencyOptions, "\t\tAvailable foreign currencies: ");

                // Sets selectedCurrency to the chopsen one from the aboce OptionsNavigation 
                selectedCurrency = currencies[chosenOption];
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

            // Passes along the options yes and no as well as the phrase to the nav method
            // Checks if it returns 0 (which translates to yes in this case) and runs the code, otherwise skips.
            if (MenuFunctions.OptionsNavigation(navOptions, $"\t\tDo you wish make a deposit to {newAccountName}?") == 0)
            {
                // Re-promts user until correct input is made
                while (true)
                {
                    Console.Clear();
                    MenuFunctions.header();
                    // Asks user for deposit amount and checks if value is correct. If yes changes account balance to chosen amount and breaks out of the loop to continue with the rest of the code.
                    Console.Write($"\t\tEnter deposit amount in {selectedCurrency}: ");
                    if (double.TryParse(Console.ReadLine(), out double initialDeposit) && initialDeposit >= 0)
                    {
                        account.Balance = initialDeposit;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\t\tInvalid deposit amount. Please enter a valid positive number.");
                    }
                }
            }

            // Attempts to add account to database. Returns true or false if successful or not
            bool success = DbHelpers.AddAccount(context, account);

            if (success)
            {
                Console.WriteLine($"\t\tCreated new account '{newAccountName}' for user '{userName}'");
            }
            else
            {
                Console.WriteLine($"\t\tFailed to create account '{newAccountName}'");
                Console.WriteLine("\t\tReturning to menu");
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
                // Clear screen and re-print header
                Console.Clear();
                MenuFunctions.header();

                Console.Write("\t\tEnter current PIN: ");
                string currentPin = MenuFunctions.CursorReadLine();

                // Re-promt user until string isn't empty
                while (string.IsNullOrEmpty(currentPin))
                {
                    Console.Write("\t\tError! PIN can't be empty");

                    Thread.Sleep(1000);

                    // Clear screen and re-print header
                    Console.Clear();
                    MenuFunctions.header();

                    Console.Write("\t\tEnter current PIN: ");
                    currentPin = MenuFunctions.CursorReadLine();
                }

                // If pin matches login info, break out of loop
                if (DbHelpers.VerifyUserLogin(context, username, currentPin))
                    break;
                else
                    Console.WriteLine("\t\tError! Wrong PIN. Try again. \n");

                Thread.Sleep(1000);
            }

            // Re-promt user for pins until 2 consecutive pins match
            while (true)
            {
                // Clear screen and re-print header
                Console.Clear();
                MenuFunctions.header();

                Console.Write("\t\tEnter new 4 digit PIN: ");
                string newPin = MenuFunctions.CursorReadLine();

                Console.Write("\t\tConfirm new PIN: ");
                string newPinConfirm = MenuFunctions.CursorReadLine();
                
                // Checks so either pin is not null or empty
                if (!string.IsNullOrEmpty(newPin) || !string.IsNullOrEmpty(newPinConfirm))
                {
                    // Chekcs if both pins are exactly 4 digits long
                    if (newPin.Length == 4 && newPinConfirm.Length == 4)
                    {
                        // If pins match save them to database and break out of loop - else write error message
                        if (newPin == newPinConfirm)
                        {
                            bool success = DbHelpers.EditPin(context, user, newPin);
                            if (success)
                            {
                                Console.WriteLine($"\t\tChanged PIN to {newPin} for user {username}");
                                Thread.Sleep(1500);
                            }
                            // If wasn't possible to save account to database, print error
                            else
                            {
                                Console.WriteLine($"\t\tFailed to update PIN to {newPin} for {username}");
                                Console.WriteLine("\t\tReturning to menu");

                                Thread.Sleep(1500);
                            }
                            break;
                        }
                        else
                            Console.WriteLine("\t\tPIN codes doesn't match. Try again.");
                    }
                    else
                        Console.WriteLine("\t\tError! PIN must be exactly 4 digits. Try again.");
                }
                else
                    Console.WriteLine("\t\tError! PIN can't be empty. Try again.");

                Thread.Sleep(1000);
            }
        }

    }
}