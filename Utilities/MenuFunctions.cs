using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Bank.Data;
using The_Bank.Utilities;

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

        // An options menu navigation method that takes in an array of strings as options that you can cycle
        // between and choose one of the said options. 
        // Will return an int depending on which options you chose so if you have 10 options (10 things in the array)
        // And pressed [Enter] on option 4 it returns 4. So you can use an if or a switch to check which option was chosen.
        // Takes in a phrase that will be written at the top of the code.
        public static int OptionsNavigation(string[] options, string phrase)
        {
            int menuSelection = 0;

            // Loops until user presses enter on a choice
            while (true)
            {
                // Clears window and re-prints the sent in phrase on each loop
                header();
                Console.WriteLine(phrase);

                // Forloop to print all the options 
                for (int i = 0; i < options.Length; i++)
                {
                    // Changes color of the option we've currently selected so when menuSelection is for exemple "2" the second option will turn darkgrey
                    if (i == menuSelection)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    // Prints all the options in the array along with the pointer arrow if on the current selection
                    Console.Write($"{options[i]}{(menuSelection == i ? " <--" : "")}\n");

                    // Reset color to default
                    Console.ResetColor();
                }

                //"Listen" to keystrokes from the user
                ConsoleKeyInfo key = Console.ReadKey(true);

                //Handles the arrow keys to move up and down the menu
                if (key.Key == ConsoleKey.UpArrow && menuSelection > 0)
                {
                    menuSelection--;
                    Sound.PlaySound("navSound.mp3");
                }
                else if (key.Key == ConsoleKey.DownArrow && menuSelection < options.Length - 1)
                {
                    menuSelection++;
                    Sound.PlaySound("navSound.mp3");

                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Sound.PlaySound("enterSound.mp3");
                    return menuSelection;
                }
            }
        }

        // Promts user to press enter key doesn't accept any other input
        public static void PressEnter()
        {
            ConsoleKeyInfo keyPressed = Console.ReadKey(true);
            while (keyPressed.Key != ConsoleKey.Enter)
            {
                keyPressed = Console.ReadKey(true);
            }
            Sound.PlaySound("enterSound.mp3");

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
            Sound.PlaySound("enterSound.mp3");
        }
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static string CursorReadLine()
        {
            string input;
            
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            input = Console.ReadLine();
            Console.CursorVisible = false;
            Console.ResetColor();

            Sound.PlaySound("enterSound.mp3");

            return input;

        }
        public static void PrintSlow(string text, int speed = 700)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(speed);
            }
            Console.WriteLine();
        }
        public static void PrintFast(string text, int speed = 70)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(speed);
            }
            Console.WriteLine();
        }

        // Prints text one character at a time with a speed of 1 char per 0,04 seconds
        public static void PrintSuperFast(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(40);
            }
            Console.WriteLine();
        }
    
        // Prints text one character at a time with a speed of 1 char per 0,04 seconds without new line at the end
        public static void PrintSuperFastNoNewLine(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(40);
            }
        }
    }
}

