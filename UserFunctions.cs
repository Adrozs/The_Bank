using The_Bank.Data;

namespace The_Bank
{
    internal class UserFunctions
    {
        internal static void UserMenu()
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
                        //ViewAccountInfo();
                        break;
                    case "2":
                        //TransferMoney();
                        break;
                    case "3":
                        //WithdrawMoney();
                        break;
                    case "4":
                        //DepositMoney();
                        break;
                    case "5":
                        //OpenNewAccount();
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

        private static void DepositMoney()
        {
            using (BankContext context = new BankContext())
            {
                Console.WriteLine("How much do you wish to deposit?");
                double deposit = int.Parse(Console.ReadLine());

                var depositUser = context.Accounts
                .Where(a => a.Balance > 0)
                .Select(a => a.Name)
                .ToList();

                if (deposit > 0)
                {
                    Console.WriteLine($"Current balance: {depositUser.Balance}");
                }

                else
                {
                    Console.WriteLine("Invalid choice");
                }
            }
        }
    }
}
