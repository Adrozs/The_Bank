using Microsoft.Extensions.Options;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    internal static class AdminFunctions
    {
        // The admin menu
        internal static void AdminMenu(BankContext context)
        {
            // Create array of all menu options
            string[] options = { "\t\tView all users", "\t\tCreate new user", "\t\tDelete an existing user", "\t\tLog out" };

            // Loop until user chooses to return to menu
            while (true)
            {
                int menuChoice = MenuFunctions.OptionsNavigation(options, "\t\tChoose one of the following options:" + "\n\t\t-----------------------------------");

                switch (menuChoice)
                {
                    case 0:
                        ViewAllUsers(context);
                        break;
                    case 1:
                        CreateUser(context);
                        break;
                    case 2:
                        DeleteUser(context);
                        break;
                    case 3:
                        MenuFunctions.PrintSuperFastNoNewLine("\t\tYou are now logging out.");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(320);
                        Console.Write(".");
                        Thread.Sleep(700);
                        return;
                    default:
                        Console.WriteLine($"Unknown choice: {menuChoice} ");
                        break;
                }
                MenuFunctions.PressEnter("\t\tPress [Enter] to return to the menu");
                Sound.PlaySound("enterSound.mp3");

            }
        }

        // Prints all users in the system
        private static void ViewAllUsers(BankContext context)
        {
            MenuFunctions.header();
            Console.WriteLine("\t\tCurrent users in the system:");
            MenuFunctions.divider();
            List<User> users = DbHelpers.GetAllUsers(context);

            foreach (User user in users)
            {
                Console.WriteLine($"\t\t{user.Name}");
            }

            MenuFunctions.divider();
            Console.WriteLine($"\t\tTotal number of users: {users.Count()}");
            MenuFunctions.footer();
        }

        // Creates a new user with a chosen name and a random pin
        private static void CreateUser(BankContext context)
        {
            string username;

            // Ensures username is create correctly
            while (true)
            {
                // Prints menu text 
                MenuFunctions.header();
                Console.WriteLine("\t\tCreate a new user");
                MenuFunctions.divider();

                Console.Write("\t\tEnter username: ");
                username = MenuFunctions.CursorReadLine();

                // Checks so string isn't empty and only contains letters
                if (string.IsNullOrEmpty(username) || !username.All(char.IsLetter))
                {
                    Console.WriteLine("\t\tUsername can't be empty and can only consist of letters.");
                    Thread.Sleep(1000);
                }
                // Checks if chosen username is already taken or not and re-prompts user.
                else if (DbHelpers.DoesUserExist(context, username))
                {
                    Console.WriteLine($"\t\t Username {username} is already taken.");
                    Thread.Sleep(1000);
                }
                // If username is unique break out of the loop and continue 
                else
                    break;

            }

            // Generates a random pin between 1000-9999 and converts it to a string
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
                Console.WriteLine($"\t\tCreated a new user {username} with pin {pin}");
            }
            else
            {
                Console.WriteLine($"\t\tFailed to create a user {username}");
            }
        }
        
        // Lets user choose a user and delete them from the database
        private static void DeleteUser(BankContext context)
        {

            // Get a list of all users in the database
            List<User> usersList = DbHelpers.GetAllUsers(context);

            // Convert list to array of only the usernames
            string[] userOptions = usersList.Select(u => u.Name).ToArray();


            // NOTE! 
            // I know there's a method for the menu choice but as the array is made from the list I couldn't figure out a way to get the \t\t before each choice when using MenuFunctions.OptionsNavigation
            // So ultimately to get it too look coherent and like every other menu I decided to just put the same code here but slightly modified so it looks as it should

            // Initialize deleteChoice outside outside the loop
            int deleteChoice = 0;

            // Loops until user presses enter on a choice
            while (true)
            {
                // Clears window and re-prints the phrase on each loop to get the menu choice effect
                MenuFunctions.header();
                Console.WriteLine("\t\tSelect which user to delete");
                MenuFunctions.divider();

                // For loop to print all the options 
                for (int i = 0; i < userOptions.Length; i++)
                {
                    // Changes color of the option we've currently selected so when menuSelection is for example "2" the second option will turn Dark Grey
                    if (i == deleteChoice)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    // Prints all the options in the array along with the pointer arrow if on the current selection
                    Console.Write($"\t\t{userOptions[i]}{(deleteChoice == i ? " <--" : "")}\n");

                    // Reset color to default
                    Console.ResetColor();
                }

                // "Listen" to keystrokes from the user
                ConsoleKeyInfo key = Console.ReadKey(true);

                //Handles the arrow keys to move up and down the menu
                if (key.Key == ConsoleKey.UpArrow && deleteChoice > 0)
                {
                    deleteChoice--;
                    Sound.PlaySound("navSound.mp3");
                }
                else if (key.Key == ConsoleKey.DownArrow && deleteChoice < userOptions.Length - 1)
                {
                    deleteChoice++;
                    Sound.PlaySound("navSound.mp3");

                }
                // If user presses enter they choose the currently selected option and we leave the loop and continue with the code
                else if (key.Key == ConsoleKey.Enter)
                {
                    Sound.PlaySound("enterSound.mp3");
                    break;
                }
            }

            MenuFunctions.footer();

            // Select which user to delete from the list
            User userToDelete = usersList.Where(u => u.Name == userOptions[deleteChoice]).Single();

            if (userToDelete != null)
            {
                // Delete the user
                bool success = DbHelpers.DeleteUser(context, userToDelete);
                if (success)
                {
                    Console.WriteLine($"\t\tUser {userToDelete.Name} has been deleted.");
                }
                else
                {
                    Console.WriteLine($"\t\tFailed to delete user {userToDelete.Name}.");
                }
            }
            else
            {
                Console.WriteLine($"\t\tUser {userToDelete.Name} was not found.");
            }

            MenuFunctions.footer();
        }

        // Creates a new admin user
        public static void CreateAdmin(BankContext context)
        {
            MenuFunctions.header();

            // Prints welcome setup wizard text
            MenuFunctions.PrintSuperFast("\t\tDear Dreamer, welcome to *Bank of Dreams*!");
            MenuFunctions.PrintSuperFast("\t\tGet ready to turn your financial dreams \n\t\tinto reality as we guide you through the \n\t\tenchanted setup process.");
            Console.WriteLine();
            MenuFunctions.PrintSuperFast("\t\tTo start your journey, let's craft a \n\t\tmagical Admin username.");

            // Wait then clear screen and re-print header
            Thread.Sleep(1200);
            MenuFunctions.header();

            // Take username input
            Console.WriteLine("\t\tPlease select your Admin username.");
            Console.Write("\t\tUsername: ");
            string adminName = MenuFunctions.CursorReadLine();

            // Ensures username is create correctly
            while (true)
            {
                MenuFunctions.header();

                Console.WriteLine("\t\tPlease select your Admin username.");
                Console.Write("\t\tUsername: ");
                adminName = MenuFunctions.CursorReadLine();

                // Re-promt user until string isn't empty and only contains letters
                if (string.IsNullOrEmpty(adminName) || !adminName.All(char.IsLetter))
                {
                    Console.WriteLine("\t\tError! Username can't be empty and can only consist of letters.");
                    Thread.Sleep(1000);

                    MenuFunctions.header();
                }
                // Checks if chosen username is already taken or not and re-prompts user.
                else if (DbHelpers.DoesUserExist(context, adminName))
                {
                    Console.WriteLine($"\t\t Username {adminName} is already taken.");
                    Thread.Sleep(1000);
                }
                // If username is unique and isn't empty and is only letters break out of the loop and continue 
                else
                    break;
            }

            MenuFunctions.divider();

            // Continue with setup wizard text 
            MenuFunctions.PrintSuperFast($"\t\tWow, {adminName} is a fantastic choice! \n\t\tNow it's time to dream up a PIN-code.");
            MenuFunctions.PrintSuperFast("\n\t\tThis is the key to unlocking the \n\t\tdoor to your dreams.\n");
            MenuFunctions.PrintSuperFast("\t\tChoose wisely, for your account's \n\t\tsafety rests in the realms of \n\t\tyour imagination.");

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

                    MenuFunctions.divider();

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
                            Console.WriteLine($"\t\tSuccessfully created Admin account \n\t\t\"{adminName}\" with PIN \"{adminPin}\"");

                            MenuFunctions.footer();
                            MenuFunctions.PrintSuperFast("\t\tCongratulations, Dream Explorer! \n\t\tYou've successfully unlocked the \n\t\tgates to Bank of Dreams. \n\n\t\tYour admin account is now infused with  \n\t\tthe magic needed for fantastical transactions.");
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
