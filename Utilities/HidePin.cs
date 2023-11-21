using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Utilities
{
    public class HidePin
    {
        public static string EnterPin()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                // If user doesn't press enter, backspace or esc -> PIN gets entered in console with '*'
                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Escape)
                {
                    password += key.KeyChar; // Every character in PIN gets hidden with help of '*'
                    Console.Write("*");
                }

                // If user clicks backspace, it erases a character in password
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b"); 
                }

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}
