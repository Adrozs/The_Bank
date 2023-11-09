using The_Bank.Utilities;

namespace The_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Welcome phrase
            Console.WriteLine("Welcome to bank! \n");

            // Counter to keep track of login attempts - initalize at value and decrement on each wrong attempt
            int loginAttempts = 2;

            // Loop until user chooses to exit program
            // TODO: Add command to exit program
            while (true)
            {
                // Login screen
                Console.WriteLine("Login ");
                Console.Write("Name: ");
                string userName = Console.ReadLine();
                Console.Write("PIN: ");
                string pin = Console.ReadLine();

                // Checks if username is admin and if pin is correct - if yes it calls the admin menu
                // TODO?: change so certain users are admin instead of a pre-determined admin? Discuss in group.
                if (userName == "admin")
                {
                    if (pin != "1234")
                    {
                        Console.WriteLine("Wrong admin password");
                    }
                    else
                        AdminFunctions.DoAdminTasks();
                }

                // Check if user login is correct 
                if (DbHelpers.VerifyLogin(userName, pin))
                {
                    UserFunctions.UserMenu(userName);
                }
                else if (loginAttempts > 0)
                {
                    Console.WriteLine($"Username or pin is invalid. {loginAttempts} tries left.");
                    loginAttempts--;
                }
                // If too many invalid login attempts were made exit program
                else
                {
                    Console.WriteLine("Too many invalid attempts.");
                    Thread.Sleep(500);
                    Console.WriteLine("Shutting down in...");
                    Thread.Sleep(1000);
                    Console.WriteLine("3");
                    Thread.Sleep(1000);
                    Console.WriteLine("2");
                    Thread.Sleep(1000);
                    Console.WriteLine("1");
                    Thread.Sleep(1000);
                    return;
                }

                // Newline for text formatting
                Console.WriteLine();
            }
                





        }
    }
}