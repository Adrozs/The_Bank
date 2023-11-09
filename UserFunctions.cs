using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using The_Bank.Data;
using The_Bank.Models;

namespace The_Bank
{
    internal class UserFunctions
    {
        internal static void UserMenu()
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
                            //ViewAccountInfo();
                            break;
                        case "2":
                            //TransferMoney();
                            break;
                        case "3":
                           // WithdrawMoney();
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


            
        }
            public static void WithdrawMoney(BankContext context)
        {
            var balance = context.Accounts
                .Where(b => b.Id == b.User.Id)
                .Select(b => b.Balance).Single();
               
            Console.Write("How much would you like to withdraw? ");
            double withdraw = Convert.ToDouble(Console.ReadLine());

            double newBalance = (balance - withdraw);
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

                
            Console.WriteLine($"You new balance is {newBalance}");
            balance = newBalance;
            context.SaveChanges();

        }




    }
}
