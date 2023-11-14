using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Models
{
    internal class MenuFunctions
    {
        public static void header()
        {
            Console.Clear();
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
            Console.WriteLine("\t\t==================================");
        }
    }
}

