using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Bank.Data;

namespace The_Bank.Utilities
{
    // Contains all methods regarding freezing and unfreezing accounts
    public static class AccountFreezed
    {

        // Checks if user account is frozen or not
        public static bool IsFreezed(string username)
        {
            using (BankContext context = new BankContext())
            {
                // Get user
                var user = DbHelpers.GetUser(context, username);

                // Returns if user us frozen or not
                //if (user.IsFreezed)
                //    return true;
                //else
                return false;
            }
        }

        // Freezes user for user for as long as the sent in time was
        public static void FreezeUser(string username, DateTime UnFreezeTime)
        {
            using (BankContext context = new BankContext())
            {
                // Get user
                var user = DbHelpers.GetUser(context, username);

                // Freeze user and add the time that they can be unfrozen
                //user.IsFreezed = true;
                //user.UnfreezeTime = UnFreezeTime;

                // Try to save changes to database
                try
                {
                    context.SaveChanges();
                }
                // If wasn't possible to save, print error and return
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding user: {ex}");
                }
            }
        }

        //Unfreezes user if the current time is past the freezed time
        public static void UnFreezeUser(string username)
        {
            using (BankContext context = new BankContext())
            {
                // Get user
                var user = DbHelpers.GetUser(context, username);

                // Check if the freeze time has passed and unfreeze or not depending on result
                if (user.UnfreezeTime < DateTime.Now)
                {
                    user.IsFreezed = false;
                }

                // Try to save changes to database
                try
                {
                    context.SaveChanges();
                }
                // If wasn't possible to save, print error and return
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding user: {ex}");
                }
            }
        }
    }
}
