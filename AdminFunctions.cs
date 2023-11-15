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
                MenuFunctions.header();
                Console.WriteLine("\t\tCurrent users in the system:");
                List<User> users = DbHelpers.GetAllUsers(context);

                foreach (User user in users)
                {
                    Console.WriteLine($"\t\t\t{user.Name}");
                }

                Console.WriteLine($"\t\t\tTotal number of users {users.Count()}");
                MenuFunctions.footer();
                Console.WriteLine("\t\t\t[C]: Create new user");
                Console.WriteLine("\t\t\t[D]: Delete an existing user");
                //Console.WriteLine("[U]: User Menu");
                Console.WriteLine("\t\t\t[X]: Exit");

                while (true)
                {
                    Console.Write("\t\t\tEnter command: ");
                    string command = Console.ReadLine();
                    MenuFunctions.footer();

                    switch (command.ToLower())
                    {
                        case "c":
                            CreateUser(context);
                            break;
                        //case "u":
                        //    // Ask for the username to pass to UserMenu
                        //    Console.Write("Enter username: ");
                        //    string username = Console.ReadLine();
                        //    UserFunctions.UserMenu(context, username);
                        //    break;
                        case "d":
                            DeleteUser(context);
                            return;
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
            Console.WriteLine("\t\t\tCreate user:");
            Console.Write("\t\t\tEnter username: ");
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
                Console.WriteLine($"\t\t\tCreated a new user {username} with pin {pin}");
            }
            else
            {
                Console.WriteLine($"\t\t\tFailed to create a user {username}");
            }
        }
        private static void DeleteUser(BankContext context)
        {
            Console.WriteLine("\t\t\tDelete user:");
            Console.Write("\t\t\tEnter username to delete: ");
            string usernameToDelete = Console.ReadLine();

            // Get the user from the database
            User userToDelete = DbHelpers.GetUserByUsername(context, usernameToDelete);

            if (userToDelete != null)
            {
                // Delete the user
                bool success = DbHelpers.DeleteUser(context, userToDelete);
                if (success)
                {
                    Console.WriteLine($"\t\t\tUser {usernameToDelete} has been deleted.");
                }
                else
                {
                    Console.WriteLine($"\t\t\tFailed to delete user {usernameToDelete}.");
                }
            }
            else
            {
                Console.WriteLine($"\t\t\tUser {usernameToDelete} not found.");
            }
        }
    }
}
