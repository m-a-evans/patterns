﻿using Patterns.Account;
using Patterns.Account.Model;

namespace Utility
{
    internal class Program
    {
        /// <summary>
        /// Bootstraps an admin user to make the rest of the Patternz application usable
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            const string PathToStore = @"..\..\..\..\data\";
            const string UserStore = "users.dat";

            if (!Directory.Exists(PathToStore)) 
            {
                Directory.CreateDirectory(PathToStore);
            }

            Console.WriteLine($"Creating user account at {Path.GetFullPath(PathToStore + UserStore)}");

            if (File.Exists(PathToStore + UserStore)) 
            {
                Console.WriteLine("Store already exists! Aborting...");
                return;
            }
            PatternzUserAccount acct = new();

            string password;

            do
            {
                Console.Write("Enter password for admin user: ");
                password = Console.ReadLine() ?? string.Empty;
            } while (string.IsNullOrWhiteSpace(password));

            IPatternzUser admin = acct.CreateUser("admin", password, "Administrator", @"/Resources/Images/Admin.png", 
                Permission.WriteAccess | Permission.ReadAccess | Permission.UpdateUser | Permission.AddUser | Permission.RemoveUser);

            acct.UpdateUser(admin);

            Task<bool> writeTask = acct.TryWriteUsersToStoreAsync(PathToStore + UserStore);
            writeTask.Wait();

            if (writeTask.IsFaulted || !writeTask.Result) 
            {
                Console.WriteLine($"An error occurred: {writeTask.Exception}");
            }
            else
            {
                Console.WriteLine($"User written to {Path.GetFullPath(PathToStore + UserStore)}!");
            }

            Console.WriteLine("Done");
        }
    }
}