using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using NicoNicoDownloader.Model;

namespace NicoNicoDownloader
{
    class Program
    {
        static CancellationTokenSource cancelToken = new CancellationTokenSource();
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }
        static async Task MainAsync(string[] args)
        {
            Console.CancelKeyPress += (s, e) =>
            {
                cancelToken.Cancel();
                e.Cancel = true;
                Console.WriteLine("abort soon...");
            };

            var email = "";
            var pass = "";
            if(args.Length != 2)
            {
                Console.WriteLine("Enter Email: ");
                email = Console.ReadLine();
                Console.WriteLine("Enter Password: ");
                pass = ConsoleHelper.ReadLineWithMask();
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
                    if (id.IndexOf(commnent_symbol) != 0 && !state_list.ContainsKey(id))    //id does not have comment symbol
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
                    await nico.GetMusicFile(id,cancelToken);
                    if (cancelToken.IsCancellationRequested)
                        break;
                    else
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

            if (!cancelToken.IsCancellationRequested)
                Console.WriteLine("all task complete!");
            else
                Console.WriteLine("aborted!");
        }
    }
}
