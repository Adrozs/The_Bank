using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Bank.Data;
using The_Bank.Models;
using The_Bank.Utilities;

namespace The_Bank
{
    internal static class AdminFunctions
    {
        public static void DoAdminTasks()
        {
            using (BankContext context = new BankContext())
            {
                Console.WriteLine("Current users in system:");
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

        private static void CreateUser(BankContext context)
        {
            Console.WriteLine("Create user:");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Random rnd = new Random();
            string pin = rnd.Next(1000, 10000).ToString();

            User newUser = new User()
            {
                Name = username,
                Pin = pin,
            };


            bool success = DbHelpers.AddUser(context, newUser);
            if (success)
            {
                Console.WriteLine($"Created new user {username} with pin {pin}");
            }
            else 
            {
                Console.WriteLine($"Failed to create user {username}");
            }

        }
    }
}
