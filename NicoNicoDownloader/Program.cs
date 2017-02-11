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
        static BatchDownloadModel model;
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }
        static async Task MainAsync(string[] args)
        {
            Console.CancelKeyPress += (s, e) =>
            {
                model.Cancel();
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

            model = await BatchDownloadModel.LoginAsync(email, pass, "format.txt");

            PowerManagement.PreventSleep();

            model.LoadListFromFile("list.txt");

            model.Progress = (id, state,msg) =>
            {
                switch(state)
                {
                    case BatchDownloadProgressState.Begin:
                        Console.WriteLine(string.Format("{0}...", id));
                        break;
                    case BatchDownloadProgressState.Complete:
                        Console.WriteLine("complete");
                        break;
                    case BatchDownloadProgressState.Failed:
                        Console.WriteLine(string.Format("failed.please see log.message is {0}", msg));
                        break;
                }
            };

            await model.DownloadAsync();

            model.SaveListToFile("list.txt");

            PowerManagement.AllowMonitorPowerdown();

            if (model.IsAborted)
                Console.WriteLine("all task complete!");
            else
                Console.WriteLine("aborted!");
        }
    }
}
