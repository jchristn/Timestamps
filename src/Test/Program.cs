using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GetSomeInput;
using Timestamps;

namespace Test
{
    public static class Program
    {
        private static bool _RunForever = true;
        private static Timestamp _Timestamp = new();

        public static void Main()
        {

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
                        Console.WriteLine(_Timestamp.TotalMs + "ms");
                        break;
                    case "end":
                        _Timestamp.End = DateTime.UtcNow;
                        break;
                    case "clear":
                        _Timestamp.End = null;
                        break;
                    case "add":
                        AddMessage();
                        break;
                    case "msg":
                        ShowMessages();
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
            Console.WriteLine("  add      Add a message");
            Console.WriteLine("  msg      Show messages");
            Console.WriteLine("");
        }

        private static void AddMessage()
        {
            string msg = Inputty.GetString("Message:", null, true);
            if (String.IsNullOrEmpty(msg)) return;
            _Timestamp.AddMessage(msg);
        }

        private static void ShowMessages()
        {
            Dictionary<DateTime, string> messages = _Timestamp.Messages;

            Console.WriteLine("");
            Console.WriteLine(messages.Count + " messages");
            foreach (KeyValuePair<DateTime, string> msg in messages)
            {
                Console.WriteLine("| " + msg.Key.ToString("yyyy-MM-dd HH:mm:ss.ffffffZ") + ": " + msg.Value);
            }
            Console.WriteLine("");
        }
    }
}