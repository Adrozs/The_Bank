using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    internal class MenuFunctions
    {
        //if you take \t\t, the text will end up on the left (This is better to use when you add more then one console.WriteLine)
        //If you instead \t\t\t, the text will end up in the middle. (This is better to use if you only add one Console.WriteLine)
        //But you will probably have to experiment a few times to get it to work

        public static void header()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n\n\t\t====================================");
            Console.WriteLine("\t\t\t  Bank of Dreams");
            Console.WriteLine("\t\t====================================");
        }

        public static void headerNoClear() 
        {
            Console.WriteLine("\n\n\n\n\t\t====================================");
            Console.WriteLine("\t\t\t  Bank of Dreams");
            Console.WriteLine("\t\t====================================");
        }

        public static void main_header()
        {
            Console.Clear();
            Console.WriteLine("\n\n\t\t\t   Welcome to \n\t\t==================================");
            Console.WriteLine("\t\t\t  Bank of Dreams");
            Console.WriteLine("\t\t==================================");
            footer();
            Console.Write("\t\t  Press <any> key to continue:");
            Console.ReadKey();


        }

        public static void footer()
        {
            Console.WriteLine("\t\t===================================");
        }

        public static void divider() 
        {
            Console.WriteLine("\t\t-----------------------------------");
        }


        // Promts user to press enter key doesn't accept any other input
        public static void PressEnter()
        {
            ConsoleKeyInfo keyPressed = Console.ReadKey(true);
            while (keyPressed.Key != ConsoleKey.Enter)
            {
                keyPressed = Console.ReadKey(true);
            }
        }

        // Promts user to press enter key doesn't accept any other input
        // Same as PressEnter() however if you send in a string it'll print that before
        public static void PressEnter(string phrase)
        {
            Console.Write(phrase);
            ConsoleKeyInfo keyPressed = Console.ReadKey(true);
            while (keyPressed.Key != ConsoleKey.Enter)
            {
                keyPressed = Console.ReadKey(true);
            }
        }



    }
}

