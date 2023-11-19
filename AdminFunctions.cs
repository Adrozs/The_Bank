using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    internal static class AdminFunctions
    {
        // The admin menu
        internal static void DoAdminTasks(BankContext context)
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
            Console.WriteLine("\t\t\t[X]: Exit");

            while (true)
            {
                Console.Write("\t\t\tEnter command: ");
                string command = MenuFunctions.CursorReadLine();
                MenuFunctions.footer();

                switch (command.ToLower())
                {
                    case "c":
                        CreateUser(context);
                        break;
                    case "d":
                        DeleteUser(context);
                        break;
                    case "x":
                        return;
                    default:
                        Console.WriteLine($"Unknown command: {command} ");
                        break;
                }
            }
        }


        // Creates a new user with a chosen name and a random pin
        private static void CreateUser(BankContext context)
        {
            Console.WriteLine("\t\t\tCreate user:");
            Console.Write("\t\t\tEnter username: ");
            string username = MenuFunctions.CursorReadLine();

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
            string usernameToDelete = MenuFunctions.CursorReadLine();

            // Get the user from the database
            User userToDelete = DbHelpers.GetUser(context, usernameToDelete);

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

        // Creates an admin 
        public static void CreateAdmin(BankContext context)
        {
            MenuFunctions.header();

            // Prints welcome setup wizard text
            MenuFunctions.PrintSuperFast("\t\tDear Dreamer, welcome to *Bank of Dreams*!");
            MenuFunctions.PrintSuperFast("\t\tGet ready to turn your financial dreams \n\t\tinto reality as we guide you through the \n\t\tenchanted setup process.");
            Console.WriteLine();
            MenuFunctions.PrintSuperFast("\t\tTo start your journey, craft a magical \n\t\tAdmin username.");

            MenuFunctions.divider();

            // Take username input
            Console.WriteLine("\t\tPlease select your Admin username.");
            Console.Write("\t\t Username: ");
            string adminName = MenuFunctions.CursorReadLine();

            // Re-promt user until string isn't empty and only contains letters
            while (string.IsNullOrEmpty(adminName) || !adminName.All(char.IsLetter))
            {
                Console.WriteLine("\t\tError! Username can't be empty or contain a number.");
                Thread.Sleep(1000);

                MenuFunctions.header();

                Console.WriteLine("\t\tPlease select your Admin username.");
                Console.Write("\t\tUsername: ");
                adminName = MenuFunctions.CursorReadLine();
            }

            MenuFunctions.divider();

            // Continue with setup wizard text 
            MenuFunctions.PrintSuperFast("\t\tFantastic! Now it's time to dream up PIN.");
            MenuFunctions.PrintSuperFast("\t\tThis is the key to unlocking the \n\t\tdoor to your dreams.");
            MenuFunctions.PrintSuperFast("\t\tChoose wisely, for your account's \n\t\tsafety rests in the realms of your imagination.");

            Thread.Sleep(1000);

            // Declare admin pin string outside the loop
            string adminPin;

            // Re-promt user to enter pin until both pin and confrim pin match to ensure no mistake was made
            while (true)
            {
                // Ensure correct input
                while (true)
                {
                    MenuFunctions.header();

                    Console.Write("\t\tPlease enter a 4-digit PIN: ");
                    adminPin = MenuFunctions.CursorReadLine();

                    // Re-promt user until conditions are met
                    // Checks so string isn't empty
                    if (string.IsNullOrEmpty(adminPin))
                    {
                        Console.WriteLine("\t\tError! PIN can't be empty.");
                        Thread.Sleep(1000);
                    }
                    else if (!adminPin.All(char.IsDigit))
                    {
                        Console.WriteLine("\t\tError! PIN can only contain digits.");
                        Thread.Sleep(1000);
                    }
                    // Checks so pin i exactly 4 digits
                    else if (adminPin.Length != 4)
                    {
                        Console.WriteLine("\t\tError! PIN must be exactly 4 digits.");
                        Thread.Sleep(1000);
                    }
                    // Checks so pin is only digits
                    else
                        break;
                }

                // Ensure user types the same PIN again
                while (true)
                {
                    MenuFunctions.header();

                    Console.Write("\t\tConfirm PIN: ");
                    string adminPinConfirm = MenuFunctions.CursorReadLine();

                    // If pins match save them to database and break out of loop - else write error message
                    if (adminPin == adminPinConfirm)
                    {
                        Admin admin = new Admin()
                        {
                            Name = adminName,
                            Pin = adminPin,
                        };

                        // Returns true if save to the database was successful
                        bool success = DbHelpers.AddAdmin(context, admin);

                        if (success)
                        {
                            Console.WriteLine($"\t\tCreated Admin account \"{adminName}\" with PIN \"{adminPin}\"");
                            Thread.Sleep(2000);
                        }
                        // If wasn't possible to save account to database, print error
                        else
                        {
                            Console.WriteLine($"\t\tFailed to create Admin account");
                            Console.WriteLine("\t\tReturning to menu");
                            Thread.Sleep(2000);
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("\t\tPIN codes doesn't match. Try again. \n");
                        Thread.Sleep(1000);
                        break;
                    }
                }
            }

        }
    }
}
