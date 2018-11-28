using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NicoNicoDownloader.Model;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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
            List<DownloadMusicItem> item = new List<DownloadMusicItem>();
            foreach(var search in scraper.Scrape(query))
            {
                item.Add(search);
            }
            DownloadMusic music = new DownloadMusic();
            music.items = item.ToArray();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DownloadMusic));
            using (StreamWriter sw = new StreamWriter("list.xml", false, Encoding.UTF8))
            {
                xmlSerializer.Serialize(sw, music);
            }
        }
    }
}
