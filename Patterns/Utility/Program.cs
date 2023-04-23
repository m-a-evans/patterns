using Patterns.Account;
using System.Windows.Controls;

namespace Utility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string PathToStore = @".\data\users.dat";

            Console.WriteLine($"Creating user account at {Path.GetFullPath(PathToStore)}");

            if (File.Exists(PathToStore)) 
            {
                Console.WriteLine("Store already exists! Aborting...");
                return;
            }
            PatternzUserAccount acct = new();

            string password = string.Empty;

            do
            {
                Console.Write("Enter password for admin user: ");
                password = Console.ReadLine() ?? string.Empty;
            } while (string.IsNullOrWhiteSpace(password));

            PatternzUser admin = acct.CreateUser("admin", password);
            admin.Permissions = Permission.WriteAccess | Permission.ReadAccess | Permission.UpdateUser | Permission.AddUser | Permission.RemoveUser;
            admin.PictureUrl = @"/Resources/Images/Admin.png";
            admin.DisplayName = "Administrator";

            acct.UpdateUser(admin);

            Task<bool> writeTask = acct.TryWriteUsersToStoreAsync(PathToStore);
            writeTask.Wait();

            if (writeTask.IsFaulted || !writeTask.Result) 
            {
                Console.WriteLine($"An error occurred: {writeTask.Exception}");
            }
            else
            {
                Console.WriteLine("user written!");
            }

            Console.WriteLine("Done");
        }
    }
}