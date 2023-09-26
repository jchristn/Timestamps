using System;
using GetSomeInput;
using Timestamps;

namespace Test
{
    public static class Program
    {
        private static bool _RunForever = true;

        public static void Main()
        {
            Timestamp ts = new();

            while (_RunForever)
            {
                string userInput = Inputty.GetString("Command [?/help]:", null, false);

                switch (userInput)
                {
                    case "q":
                        _RunForever = false;
                        break;
                    case "c":
                    case "cls":
                        Console.Clear();
                        break;
                    case "?":
                        Menu();
                        break;
                    case "ms":
                        Console.WriteLine(ts.TotalMs + "ms");
                        break;
                    case "end":
                        ts.End = DateTime.UtcNow;
                        break;
                    case "clear":
                        ts.End = null;
                        break;
                }
            }
        }

        private static void Menu()
        {
            Console.WriteLine("");
            Console.WriteLine("Available commands:");
            Console.WriteLine("  q        Quit the program");
            Console.WriteLine("  cls      Clear the screen");
            Console.WriteLine("  ?        Help, this menu");
            Console.WriteLine("  ms       Total milliseconds from now (or defined end)");
            Console.WriteLine("  end      Set end to UTC now");
            Console.WriteLine("  clear    Clear end time");
            Console.WriteLine("");
        }
    }
}