using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Text;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;
using The_Bank.CurrencyTypeEnum;
using The_Bank.Migrations;

namespace The_Bank
{
    internal class UserFunctions
    {
        private static DateTime currentDateTime = DateTime.Now;
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
                    Console.WriteLine("7. Invest your Money");
                    Console.WriteLine("8. Exit");
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
                            WiseInvestments(outerContext, userName);
                            break;
                        case "8":
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
            try
            {
                User user = context.Users
                    .Where(u => u.Name == userName)
                    .Include(u => u.Accounts)
                    .SingleOrDefault();

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
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
        private static void DepositMoney()
        {
            using (BankContext context = new BankContext())
            {
                Console.WriteLine("How much do you wish to deposit?");
                decimal deposit = decimal.Parse(Console.ReadLine());

                Console.WriteLine("Which bank?");
                string bankChoice = Console.ReadLine();

                if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                {
                    var account = context.Accounts
                     .Where(a => a.Name == bankChoice)
                     .FirstOrDefault();

                    if (account != null)
                    {
                        account.Balance += depositAmount;
                        context.SaveChanges();

                        Console.WriteLine($"Deposit successful. New balance: {account.Balance} in account: {bankChoice}");
                    }
                }

                else
                {
                    Console.WriteLine("Invalid choice");
                }
            }

            Console.WriteLine("Press enter to continue");
            ConsoleKeyInfo key = Console.ReadKey(true);
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
                {
                    break;
                }
            }

            // Creates a new user object for the user that's logged in
            User user = DbHelpers.GetUser(context, username);

            // Create a new account type with ID and Name of the current user and starting balance of 0
            Account account = new Account()
            {
                UserId = user.Id,
                Name = newAccountName,
                Balance = 0,
            };

            // Save account to the database
            bool success = DbHelpers.AddAccount(context, account);

            if (success)
            {
                Console.WriteLine($"Created new account {newAccountName} for user {username}");

                // Ask if it's a vacation account
                Console.WriteLine("Is this a Vacation Account? (Yes/No): ");
                string isVacationAccountInput = Console.ReadLine();

                bool isVacationAccount = isVacationAccountInput.Equals("Yes", StringComparison.OrdinalIgnoreCase);

                // If it's a vacation account, ask for the currency
                if (isVacationAccount)
                {
                    Console.WriteLine("Select the currency for the Vacation Account:");
                    DisplayCurrencyOptions();
                    Console.Write("Enter the currency number: ");

                    if (int.TryParse(Console.ReadLine(), out int currencyChoice) && currencyChoice >= 1 && currencyChoice <= 10)
                    {
                        CurrencyType selectedCurrency = (CurrencyType)currencyChoice;
                        account.Currency = selectedCurrency;

                        // Ask for the initial deposit amount
                        Console.WriteLine("Enter the initial deposit amount: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal initialDeposit))
                        {
                            // Set the initial balance in the selected currency
                            account.Balance = initialDeposit;

                            Console.WriteLine($"Vacation Account {newAccountName} is created with currency: {selectedCurrency}");
                            Console.WriteLine($"Initial deposit: {initialDeposit} {selectedCurrency}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid initial deposit amount. Account created with 0 balance.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid currency selection. Defaulting to Swedish Krona.");
                        account.Currency = CurrencyType.SwedishKrona;
                    }
                }
                else
                {
                    Console.WriteLine($"Normal Account {newAccountName} is created with currency: Swedish Krona");
                    account.Currency = CurrencyType.SwedishKrona;
                }

                // Save changes to the database
                context.SaveChanges();

                // If it wasn't possible to save the account to the database, print error
                if (!success)
                {
                    Console.WriteLine($"Failed to create account {newAccountName}");
                    Console.WriteLine("Returning to the menu");
                }

                // Waits for the user to press enter to continue
                Console.WriteLine("Press [Enter] to go to the main menu");
            ConsoleKeyInfo key = Console.ReadKey(true);

            // Loops until the user presses Enter
            while (key.Key != ConsoleKey.Enter)
                key = Console.ReadKey(true);

            // New line for text formatting
            Console.WriteLine();

            // Save changes to the database
            context.SaveChanges();

        }
            }
        

        public static class CurrencyConverter
        {
            // Conversion rates
            private static readonly Dictionary<CurrencyType, decimal> ConversionRates = new Dictionary<CurrencyType, decimal>
        {
            { CurrencyType.US_Dollar, 10.83m },
            { CurrencyType.Euro, 11.59m },
            { CurrencyType.BritishPound, 13.30m },
            { CurrencyType.SwissFranc, 12.02m },
            { CurrencyType.TurkishLira, 0.38m },
            { CurrencyType.RussianRouble, 0.12m },
            { CurrencyType.ChineseYuan, 2.87m },
            { CurrencyType.BrazilianReal, 2.21m },
            { CurrencyType.ZimbabweanDollar, 0.033m },
            { CurrencyType.CanadianDollar, 7.85m },
        };

            public static decimal ConvertCurrency(decimal amount, CurrencyType fromCurrency, CurrencyType toCurrency)
            {
                // Check if conversion rates are available for both currencies
                if (ConversionRates.TryGetValue(fromCurrency, out decimal fromRate) && ConversionRates.TryGetValue(toCurrency, out decimal toRate))
                {
                    // Convert amount to SEK first, then to the target currency
                    decimal amountInSEK = amount / fromRate;
                    return amountInSEK * toRate;
                }

                // If conversion rates are not available, return the original amount
                return amount;
            }
        }

        private static void DisplayCurrencyOptions()
        {
            Console.WriteLine("Select the currency for the Vacation Account:");
            Console.WriteLine("1. Euro");
            Console.WriteLine("2. US Dollar");
            Console.WriteLine("3. British Pound");
            Console.WriteLine("4. Swiss Franc");
            Console.WriteLine("5. Turkish Lira");
            Console.WriteLine("6. Russian Rouble");
            Console.WriteLine("7. Chinese Yuan");
            Console.WriteLine("8. Brazilian Real");
            Console.WriteLine("9. Zimbabwean Dollar");
            Console.WriteLine("10. Canadian Dollar");
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
        public static void InvestInStockExchange(BankContext context, string userName)
        {
            while (true)
            {
                Console.WriteLine("\nStock Exchange Menu:");
                Console.WriteLine("1. View Stock Portfolio");
                Console.WriteLine("2. Show Trending Stocks");
                Console.WriteLine("3. Show Most Advanced Stocks");
                Console.WriteLine("4. Show Most Declined Stocks");
                Console.WriteLine("5. Search Stocks by Name");
                Console.WriteLine("6. Simulate Time");
                Console.WriteLine("7. Exit Stock Exchange");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewStockPortfolio(context, userName);
                        break;
                    case "2":
                        ShowTrendingStocks(context);
                        break;
                    case "3":
                        ShowMostAdvancedStocks(context);
                        break;
                    case "4":
                        ShowMostDeclinedStocks(context);
                        break;
                    case "5":
                        SearchStocksByName(context);
                        break;
                    case "6":
                        SimulateTime();
                        break;
                    case "7":
                        return; // Exit Stock Exchange menu
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                        break;
                }
            }
        }

            private static void ViewStockPortfolio(BankContext context, string userName)
            {
                // Get user info from Database
                User user = context.Users
                    .Include(u => u.Accounts)
                    .ThenInclude(a => a.StockPortfolio)
                    .Single(u => u.Name == userName);

                Console.WriteLine($"Stock Portfolio for {userName}:");

                foreach (var account in user.Accounts)
                {
                    Console.WriteLine($"Account: {account.Name}");

                    if (account.StockPortfolio != null && account.StockPortfolio.Any())
                    {
                        Console.WriteLine("Stocks:");

                        foreach (var stock in account.StockPortfolio)
                        {
                            Console.WriteLine($"Company: {stock.CompanyName}, Stock: {stock.StockName}, Quantity: {stock.Quantity}, Purchase Price: {stock.PurchasePrice:C}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No stocks in the portfolio.");
                    }

                    Console.WriteLine(); // Add a newline for better formatting
                }
            }

        private static void ShowTrendingStocks(BankContext context)
        {
            Console.WriteLine("\nTrending Stocks:");

            var trendingStocks = context.StockPrices
                .OrderByDescending(stock => stock.CurrentPrice)
                .Take(10)
                .ToList();

            if (trendingStocks.Any())
            {
                DisplayStocks(trendingStocks);
            }
            else
            {
                Console.WriteLine("No trending stocks found.");
            }

            // Display current date and time
            Console.WriteLine($"Current Date and Time: {DateTime.Now}");
        }

        private static void ShowMostAdvancedStocks(BankContext context)
        {
            // Fetch and display the most advanced stocks
            Console.WriteLine("\nMost Advanced Stocks:");

            var mostAdvancedStocks = context.StockPrices
                .OrderByDescending(stock => stock.CurrentPrice)
                .Take(10)
                .ToList();

            DisplayStocks(mostAdvancedStocks);

            // Display current date and time
            Console.WriteLine($"Current Date and Time: {currentDateTime}");
        }

        private static void ShowMostDeclinedStocks(BankContext context)
        {
            // Fetch and display the most declined stocks
            Console.WriteLine("\nMost Declined Stocks:");

            var mostDeclinedStocks = context.StockPrices
                .OrderBy(stock => stock.CurrentPrice)
                .Take(10)
                .ToList();

            DisplayStocks(mostDeclinedStocks);

            // Display current date and time
            Console.WriteLine($"Current Date and Time: {currentDateTime}");
        }

        private static void DisplayStocks(List<StockPrice> stocks)
        {
            foreach (var stock in stocks)
            {
                Console.WriteLine($"Company: {stock.CompanyName}, Stock: {stock.StockName}, Current Price: {stock.CurrentPrice:C}");
            }
        }

        private static void SearchStocksByName(BankContext context)
        {
            Console.Write("Enter the name of the Company or Stock to search: ");
            string searchTerm = Console.ReadLine();

            // Search for stocks in the "StockPrices" database based on CompanyName or StockName
            var matchingStocks = context.StockPrices
                .Where(stock => stock.CompanyName.Contains(searchTerm) || stock.StockName.Contains(searchTerm))
                .ToList();

            // Display search results
            if (matchingStocks.Any())
            {
                Console.WriteLine("Search Results:");
                foreach (var stock in matchingStocks)
                {
                    Console.WriteLine($"Company: {stock.CompanyName}, Stock: {stock.StockName}, Current Price: {stock.CurrentPrice:C}");
                }
            }
            else
            {
                Console.WriteLine("No matching stocks found.");
            }

            // Display current date and time
            Console.WriteLine($"Current Date and Time: {DateTime.Now}");
        }

        private static void SimulateTime()
        {
            Console.WriteLine("\nSimulate Time Menu:");
            Console.WriteLine("d. Simulate 24 hours ahead");
            Console.WriteLine("w. Simulate 1 week ahead");
            Console.WriteLine("m. Simulate 1 month ahead");
            Console.WriteLine("y. Simulate 1 year ahead");
            Console.WriteLine("r. Return to Real Time");

            Console.Write("Enter your choice: ");
            string timeChoice = Console.ReadLine();

            switch (timeChoice.ToLower())
            {
                case "d":
                    currentDateTime = currentDateTime.AddHours(24);
                    break;
                case "w":
                    currentDateTime = currentDateTime.AddDays(7);
                    break;
                case "m":
                    currentDateTime = currentDateTime.AddMonths(1);
                    break;
                case "y":
                    currentDateTime = currentDateTime.AddYears(1);
                    break;
                case "r":
                    currentDateTime = DateTime.Now;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Time simulation canceled.");
                    break;
            }

            // Display updated current date and time
            Console.WriteLine($"Current Date and Time: {currentDateTime}");
        }


        private static void ViewCryptoInvestments(BankContext context, string userName)
        {
            // Get user info from Database
            User user = context.Users
                .Include(u => u.CryptoInvestments)
                .Single(u => u.Name == userName);

            Console.WriteLine($"Crypto Investments for {userName}:");

            if (user.CryptoInvestments != null && user.CryptoInvestments.Any())
            {
                foreach (var crypto in user.CryptoInvestments)
                {
                    Console.WriteLine($"Currency: {crypto.CurrencyName} ({crypto.CurrencyCode}), Quantity: {crypto.Quantity}, Purchase Price: {crypto.PurchasePrice:C}, Purchase Date: {crypto.PurchaseDate}");
                }
            }
            else
            {
                Console.WriteLine("No crypto investments found.");
            }

            Console.WriteLine(); // Add a newline for better formatting
        }

        private static void InvestInCryptoExchange(BankContext context, string userName)
        {
            while (true)
            {
                Console.WriteLine("\nCrypto Exchange Menu:");
                Console.WriteLine("1. View Crypto Investments");
                Console.WriteLine("2. Buy Crypto");
                Console.WriteLine("3. Sell Crypto");
                Console.WriteLine("4. Exit Crypto Exchange");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewCryptoInvestments(context, userName);
                        break;
                    case "2":
                        BuyCrypto(context, userName);
                        break;
                    case "3":
                        SellCrypto(context, userName);
                        break;
                    case "4":
                        return; // Exit Crypto Exchange menu
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                        break;
                }
            }
        }

        private static void BuyCrypto(BankContext context, string userName)
        {
            // Get user info from Database
            User user = context.Users
                .Include(u => u.Accounts)
                .Include(u => u.CryptoInvestments)
                .Single(u => u.Name == userName);

            Console.WriteLine("Available Cryptocurrencies:");
            Console.WriteLine("1. Bitcoin (BTC)");
            Console.WriteLine("2. Ethereum (ETH)");
            // Add more cryptocurrencies as needed

            Console.Write("Enter the number of the cryptocurrency to buy: ");
            if (int.TryParse(Console.ReadLine(), out int cryptoChoice) && cryptoChoice >= 1 && cryptoChoice <= 2) // Adjust the range based on the number of cryptocurrencies
            {
                // Mapping cryptoChoice to cryptocurrency details
                string[] cryptoNames = { "Bitcoin", "Ethereum" }; // Add more names as needed
                string[] cryptoCodes = { "BTC", "ETH" }; // Add more codes as needed
                string selectedCryptoName = cryptoNames[cryptoChoice - 1];
                string selectedCryptoCode = cryptoCodes[cryptoChoice - 1];

                // Ask for quantity
                Console.Write($"Enter the quantity of {selectedCryptoName} to buy: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal quantity) && quantity > 0)
                {
                    // Assume a static price for demonstration purposes, replace with actual pricing logic
                    decimal currentPrice = GetCryptoCurrentPrice(selectedCryptoCode); // Replace with your actual pricing logic

                    // Calculate total purchase price
                    decimal purchasePrice = currentPrice * quantity;

                    // Check if the user has enough balance in any account
                    if (user.Accounts.Any(a => a.Balance >= purchasePrice))
                    {
                        // Deduct the purchase amount from the user's balance in the first account with sufficient funds
                        Account sourceAccount = user.Accounts.First(a => a.Balance >= purchasePrice);
                        sourceAccount.Balance -= purchasePrice;

                        // Add the crypto investment to the user's portfolio
                        CryptoInvestment cryptoInvestment = new CryptoInvestment
                        {
                            UserId = user.Id,
                            CurrencyName = selectedCryptoName,
                            CurrencyCode = selectedCryptoCode,
                            Quantity = quantity,
                            PurchasePrice = currentPrice,
                            PurchaseDate = DateTime.Now,
                        };

                        user.CryptoInvestments.Add(cryptoInvestment);

                        Console.WriteLine($"Successfully bought {quantity} {selectedCryptoName} ({selectedCryptoCode}) for {purchasePrice:C}. Remaining balance: {sourceAccount.Balance:C}");
                        Console.WriteLine("TO THE MOON!");
                    }
                    else
                    {
                        Console.WriteLine("Insufficient funds in any account to buy the selected cryptocurrency.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid quantity. Please enter a valid positive number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a valid number.");
            }
        }


        private static void SellCrypto(BankContext context, string userName)
        {
            // Get user info from Database
            User user = context.Users
                .Include(u => u.Accounts)
                .Include(u => u.CryptoInvestments)
                .Single(u => u.Name == userName);

            Console.WriteLine("Your Crypto Investments:");

            if (user.CryptoInvestments != null && user.CryptoInvestments.Any())
            {
                int index = 1;
                foreach (var crypto in user.CryptoInvestments)
                {
                    Console.WriteLine($"{index}. {crypto.CurrencyName} ({crypto.CurrencyCode}) - Quantity: {crypto.Quantity}");
                    index++;
                }

                Console.Write("Enter the number of the cryptocurrency to sell: ");
                if (int.TryParse(Console.ReadLine(), out int sellChoice) && sellChoice >= 1 && sellChoice <= user.CryptoInvestments.Count)
                {
                    // Get the selected crypto investment
                    CryptoInvestment selectedCrypto = user.CryptoInvestments[sellChoice - 1];

                    // Assume a static price for demonstration purposes, replace with actual pricing logic
                    decimal currentPrice = GetCryptoCurrentPrice(selectedCrypto.CurrencyCode); // Replace with your actual pricing logic

                    // Calculate total selling price
                    decimal sellingPrice = currentPrice * selectedCrypto.Quantity;

                    // Add the selling amount to the user's balance in the first account
                    Account destinationAccount = user.Accounts.First();
                    destinationAccount.Balance += sellingPrice;

                    // Remove the crypto investment from the user's portfolio
                    user.CryptoInvestments.Remove(selectedCrypto);

                    Console.WriteLine($"Successfully sold {selectedCrypto.Quantity} {selectedCrypto.CurrencyName} ({selectedCrypto.CurrencyCode}) for {sellingPrice:C}. New balance: {destinationAccount.Balance:C}");
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter a valid number.");
                }
            }
            else
            {
                Console.WriteLine("No crypto investments found.");
            }

            Console.WriteLine(); // Add a newline for better formatting
        }


        private static decimal GetCryptoCurrentPrice(string cryptoCode)
        {
            // Implement your logic to get the current price of the specified cryptocurrency
            // Replace this with actual pricing logic or API calls to fetch real-time prices
            // For simplicity, return a static price in this example
            if (cryptoCode == "BTC")
                return 399237.96m; // Replace with actual BTC price
            else if (cryptoCode == "ETH")
                return 22710.98m; // Replace with actual ETH price
            else
                return 0m; // Handle other cryptocurrencies as needed
        }
        public static void WiseInvestments(BankContext context, string userName)
        {
            {
                while (true)
                {
                    Console.WriteLine("\nWise Investments Menu:");
                    Console.WriteLine("1. Stock Exchange");
                    Console.WriteLine("2. Crypto Exchange");
                    Console.WriteLine("3. Exit Wise Investments");

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            InvestInStockExchange(context, userName);
                            break;
                        case "2":
                            InvestInCryptoExchange(context, userName);
                            break;
                        case "3":
                            return; // Exit Wise Investments menu
                        default:
                            Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                            break;
                    }
                }
            }
        }
    }
}


