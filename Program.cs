namespace The_Bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to bank! \n");
            Console.WriteLine("Login ");
            Console.Write("Name: ");
            string userName = Console.ReadLine();
            Console.Write("PIN:");
            string pin = Console.ReadLine();

            if (userName == "admin")
            {
                if (pin != "1234") 
                {
                    Console.WriteLine("Wrong password");
                    return;
                }

                AdminFunctions.DoAdminTasks();
            }

            // Code here for user login ****
        }
    }
}