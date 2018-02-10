using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NicoNicoDownloader.Model;

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
                pass = ConsoleHelper.ReadLineWithMask();
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
                Console.WriteLine(string.Format("#{0}", search.Title));
                Console.WriteLine(string.Format("{0}",search.ContentId,search.Title,search.StartTime));
            }
        }
    }
}
