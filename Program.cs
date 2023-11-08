namespace The_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Welcome phrase
            Console.WriteLine("Welcome to bank! \n");
            
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
                // TODO: change so certain users are admin instead of a pre-determined admin? Discuss in group.
                if (userName == "admin")
                {
                    if (pin != "1234")
                    {
                        Console.WriteLine("Wrong password");
                    }
                    else
                        AdminFunctions.DoAdminTasks();
                }
                // TODO: Check if user exists in db 
                else if (userName == "user") // Need to change to check if username is in database and pin matches that users pin
                {
                    UserFunctions.UserMenu(userName);
                }
                // TODO: If pin is invalid 3 times shut down the program
                else
                    Console.WriteLine("Username or pin is invalid");

                // Newline
                Console.WriteLine();
            }
                





        }
    }
}