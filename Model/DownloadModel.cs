using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NicoNicoDownloader.Model
{
    enum BatchDownloadProgressState
    {
        Begin,
        Complete,
        Failed
    }

    class BatchDownloadModel
    {
        const string commnent_symbol = "#";

        //key = niconico video id
        //value = if download, value is true. 
        private Dictionary<string, bool> state_list = new Dictionary<string, bool>();

        private NicoNicoDownload nico;

        private CancellationTokenSource cancelToken = new CancellationTokenSource();

        private BatchDownloadModel(NicoNicoDownload nico)
        {
            this.nico = nico;
        }

        public async static Task<BatchDownloadModel> LoginAsync(string email, string pass, string title_path)
        {
            NicoNicoDownload nico = new NicoNicoDownload();
            nico.TitleConverter = TitileConverterInfo.Build(title_path);
            await nico.Login(email, pass);

            BatchDownloadModel _model = new BatchDownloadModel(nico);
            return _model;
        }

        public void LoadListFromFile(string filepath)
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                while (!sr.EndOfStream)
                {
                    string id = sr.ReadLine();
                    if (id.IndexOf(commnent_symbol) != 0 && !state_list.ContainsKey(id))    //id does not have comment symbol
                        state_list.Add(id, false);
                }
            }
        }

        public void SaveListToFile(string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (KeyValuePair<string, bool> state in state_list)
                {
                    if (state.Value)
                        sw.WriteLine(commnent_symbol + state.Key);
                    else
                        sw.WriteLine(state.Key);
                }
            }
        }

        public bool IsAborted
        {
            get
            {
                return cancelToken.IsCancellationRequested;
            }
        }

        //id,state,message
        public Action<string,BatchDownloadProgressState,string> Progress;

        public async Task DownloadAsync()
        {
            foreach (string id in state_list.Keys.ToList())
            {
                Progress(id, BatchDownloadProgressState.Begin,null);
                try
                {
                    await nico.GetMusicFile(id, cancelToken);
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    else
                    {
                        Progress(id, BatchDownloadProgressState.Complete, null);
                    }
                    state_list[id] = true;
                }
                catch (Exception ex)
                {
                    Progress(id, BatchDownloadProgressState.Failed,ex.Message);
                }
            }
        }

        public void Cancel()
        {
            cancelToken.Cancel();
        }

    }
}
