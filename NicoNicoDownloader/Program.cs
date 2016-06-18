using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NicoNicoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var email = "";
            var pass = "";
            if(args.Length != 2)
            {
                Console.WriteLine("Enter Email: ");
                email = Console.ReadLine();
                Console.WriteLine("Enter Password: ");

                // Mask Password
                pass = "";
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        pass += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key != ConsoleKey.Backspace || pass.Length <= 0) continue;
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
                while (key.Key != ConsoleKey.Enter);
                Console.WriteLine();
            }
            else
            {
                email = args[0];
                pass = args[1];
            }

            NicoNicoDownload nico = new NicoNicoDownload();
            nico.TitleConverter = TitileConverterInfo.Build("format.txt");
            await nico.Login(email, pass);

            PowerManagement.PreventSleep();

            using (StreamReader sr = new StreamReader("list.txt"))
            {
                while(!sr.EndOfStream)
                {
                    string id = sr.ReadLine();
                    Console.WriteLine(string.Format("{0}...",id));
                    try
                    {
                        await nico.GetMusicFile(id);
                        Console.WriteLine("complete");
                    }catch(Exception ex)
                    {
                        Console.WriteLine(string.Format("failed.please see log.message is {0}",ex.Message));
                    }
                }
            }

            PowerManagement.AllowMonitorPowerdown();

            Console.WriteLine("all task complete!");
        }
    }
}
