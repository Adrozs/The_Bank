using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    internal static class AdminFunctions
    {
        // The admin menu
        internal static void DoAdminTasks()
        {
            using (BankContext context = new BankContext())
            {
                Console.WriteLine("Current users in the system:");
                List<User> users = DbHelpers.GetAllUsers(context);

                foreach (User user in users)
                {
                    Console.WriteLine($"{user.Name}");
                }

                Console.WriteLine($"Total number of users {users.Count()}");
                Console.WriteLine("[C]: Create new user");
                Console.WriteLine("[X]: Exit");

                while (true)
                {
                    Console.Write("Enter command: ");
                    string command = Console.ReadLine();

                    switch (command.ToLower())
                    {
                        case "c":
                            CreateUser(context);
                            break;
                        case "x":
                            return;
                        default:
                            Console.WriteLine($"Unknown command: {command} ");
                            break;
                    }
                }
            }
        }

        // Creates a new user with a chosen name and a random pin
        private static void CreateUser(BankContext context)
        {
            Console.WriteLine("Create user:");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            // TODO?: Should we let the user choose their own pin? Discuss in a group.
            Random rnd = new Random();
            string pin = rnd.Next(1000, 10000).ToString();

            // Creates a new user object with the chosen name and a random pin-code
            User newUser = new User()
            {
                Name = username,
                Pin = pin,
            };

            // Adds and saves the user to the db - if successful prints out a confirmation message, if not prints out a fail message
            bool success = DbHelpers.AddUser(context, newUser);
            if (success)
            {
                Console.WriteLine($"Created a new user {username} with pin {pin}");
            }
            else
            {
                Console.WriteLine($"Failed to create a user {username}");
            }
        }
    }
}
