using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace NicoNicoDownloader.Model
{
    class VideoToAudioConveter
    {
        //一時フォルダー（最後は円記号で終わらせること）
        public const string tempDirectory = "download\\";
        const string tempFileName = "temp";

        public TitileConverterInfo TitleConverter
        {
            get;
            set;
        }

        public string GetVideoFileName(string namepart,string ext)
        {
            return string.Format(VideoToAudioConveter.tempDirectory + "{0}.{1}", namepart, ext);
        }

        public string GetAudioFileName(string title)
        {
            return string.Format(tempDirectory + "{0}.m4a", this.TitleConverter.ConvertTitle(this.ConvertFileName(title)));
        }

        private string ConvertFileName(string name)
        {
            string new_name = name.Replace('?', '？');
            new_name = new_name.Replace('"', '”');
            new_name = new_name.Replace('/', '／');
            new_name = new_name.Replace('\'', '’');
            new_name = new_name.Replace('\\', '￥');
            new_name = new_name.Replace('<', '＜');
            new_name = new_name.Replace('>', '＞');
            new_name = new_name.Replace('*', '＊');
            new_name = new_name.Replace('|', '｜');
            return new_name.Replace(':', '：');
        }

        public async Task GetVideoFile(string temp_file_name, Stream stream, CancellationTokenSource token = null)
        {
            using (var sr = new FileStream(temp_file_name, System.IO.FileMode.Create))
            {
                var count = 0;
                do
                {
                    byte[] data = new byte[64 * 1024];
                    count = await stream.ReadAsync(data, 0, data.Length);
                    await sr.WriteAsync(data, 0, count);
                    if (token != null && token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                } while (count != 0);
            }
        }

        public void GetAudioFile(string temp_file_name, string audio_file_name)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = "ffmpeg.exe";
            info.UseShellExecute = false;
            info.Arguments = string.Format("-i {0} -vn -acodec copy \"{1}\"", temp_file_name, audio_file_name);
            info.WorkingDirectory = Environment.CurrentDirectory;
            info.CreateNoWindow = true;
            var p = System.Diagnostics.Process.Start(info);
            p.WaitForExit();
        }
    }
}
