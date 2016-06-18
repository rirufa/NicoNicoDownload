using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoNicoScraper
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
            var query = "";
            if (args.Length != 3)
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

                Console.WriteLine("query: ");
                query = Console.ReadLine();
            }
            else
            {
                email = args[0];
                pass = args[1];
                query = args[2];
            }

            Scraper scraper = new Scraper();
            await scraper.Login(email, pass);
            foreach(var search in scraper.Scrape(query))
            {
                Console.WriteLine(string.Format("{0}:{1},{2}",search.ContentId,search.Title,search.StartTime));
            }
        }
    }
}
