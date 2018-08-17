using System;
using Server.Accounting;

namespace Server.Misc
{
    public class AccountPrompt
    {
        public static void Initialize()
        {
            if (Accounts.Count == 0 && !Core.Service)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine(new String('_', Console.BufferWidth));
                Console.WriteLine("                            Account Administration:");
                Utility.PushColor(ConsoleColor.White);
                Console.Write("                Do you want to create the owner account now? ");
                Utility.PushColor(ConsoleColor.Cyan);
                Console.Write("(y/n)");
                Utility.PopColor();

                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    Console.WriteLine();

                    Console.Write("                             Username: ");
                    string username = Console.ReadLine();

                    Console.Write("                             Password: ");
                    string password = Console.ReadLine();

                    Account a = new Account(username, password);
                    a.AccessLevel = AccessLevel.Owner;

                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.Write("Shard: ");
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.Write("Owner Account");
                    Utility.PushColor(ConsoleColor.DarkGray);
                    Console.Write("..................................................");
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("[Created]");
                    Utility.PopColor();
                }
                else
                {
                    Console.WriteLine();
                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.Write("Shard: ");
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.Write("Owner Account");
                    Utility.PushColor(ConsoleColor.DarkGray);
                    Console.Write("..................................................");
                    Utility.PushColor(ConsoleColor.DarkRed);
                    Console.WriteLine("[Skipped]");
                    Utility.PopColor();
                }

                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine(new String('_', Console.BufferWidth));
                Utility.PopColor();
            }
        }
    }
}