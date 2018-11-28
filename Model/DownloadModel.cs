using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NicoNicoDownloader.Model
{
    enum BatchDownloadProgressState
    {
        Begin,
        Complete,
        Failed
    }

    [XmlRootAttribute("DownloadMusic", IsNullable = false)]
    public class DownloadMusic
    {
        public string Version;
        [XmlArrayAttribute]
        public DownloadMusicItem[] items;
        public DownloadMusic()
        {
            this.Version = "1.0";
        }
    }

    public class DownloadMusicItem
    {
        public string id;
        public bool IsFinished;
        public string music_name;
        public DownloadMusicItem()
        {
            this.IsFinished = false;
        }
        public DownloadMusicItem(string id,string name) : this()
        {
            this.id = id;
            this.music_name = name;
        }
    }

    class BatchDownloadModel
    {
        private DownloadMusic downloadMusic;

        private NicoNicoDownload nico;

        private CancellationTokenSource cancelToken = new CancellationTokenSource();

        private BatchDownloadModel(NicoNicoDownload nico)
        {
            this.nico = nico;
        }

        public async static Task<BatchDownloadModel> LoginAsync(string email, string pass)
        {
            NicoNicoDownload nico = new NicoNicoDownload();
            nico.TitleConverter = TitileConverterInfo.Build("format.txt", "bands.txt");
            await nico.Login(email, pass);

            BatchDownloadModel _model = new BatchDownloadModel(nico);
            return _model;
        }

        public void LoadListFromFile(string filepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DownloadMusic));
            using (StreamReader sr = new StreamReader(filepath, Encoding.UTF8))
            {
                this.downloadMusic = (DownloadMusic)xmlSerializer.Deserialize(sr);
            }
        }

        public void SaveListToFile(string filepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DownloadMusic));
            using (StreamWriter sw = new StreamWriter(filepath,false,Encoding.UTF8))
            {
                xmlSerializer.Serialize(sw, this.downloadMusic);
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
            foreach (var item in this.downloadMusic.items)
            {
                Progress(item.id, BatchDownloadProgressState.Begin,null);
                try
                {
                    await nico.GetMusicFile(item, cancelToken);
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    else
                    {
                        Progress(item.id, BatchDownloadProgressState.Complete, null);
                        Thread.Sleep(1000 * 10);
                    }
                    item.IsFinished = true;
                }
                catch (Exception ex)
                {
                    Progress(item.id, BatchDownloadProgressState.Failed,ex.Message);
                }
            }
        }

        public void Cancel()
        {
            cancelToken.Cancel();
        }

    }
}
