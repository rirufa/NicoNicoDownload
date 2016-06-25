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

            const string commnent_symbol = "#";

            //key = niconico video id
            //value = if download, value is true. 
            Dictionary<string, bool> state_list = new Dictionary<string, bool>();

            using (StreamReader sr = new StreamReader("list.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string id = sr.ReadLine();
                    if (id.IndexOf(commnent_symbol) != 0)    //id does not have comment symbol
                        state_list.Add(id, false);
                    else
                        Console.WriteLine(string.Format("{0} is skipped", id.Trim(commnent_symbol.ToCharArray())));
                }
            }

            foreach(string id in state_list.Keys.ToList())
            {
                Console.WriteLine(string.Format("{0}...", id));
                try
                {
                    await nico.GetMusicFile(id);
                    Console.WriteLine("complete");
                    state_list[id] = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("failed.please see log.message is {0}", ex.Message));
                }
            }

            using (StreamWriter sw = new StreamWriter("list.txt"))
            {
                foreach(KeyValuePair<string,bool> state in state_list)
                {
                    if (state.Value)
                        sw.WriteLine(commnent_symbol + state.Key);
                    else
                        sw.WriteLine(state.Key);
                }
            }

            PowerManagement.AllowMonitorPowerdown();

            Console.WriteLine("all task complete!");
        }
    }
}
