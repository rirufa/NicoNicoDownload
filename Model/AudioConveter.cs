using System;
using System.Linq;
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

        public string GetMusicTitle(string title, string description)
        {
            var original_titile = this.ConvertFileName(title);
            return this.TitleConverter.ConvertTitle(this.ConvertFileName(title), description);
        }

        public string GetAudioFileName(string file_name_part, string audio_format)
        {
            try
            {
                //同じファイル名（拡張子を除く）が存在するかどうか
                var isexist = Directory.EnumerateFiles(tempDirectory, file_name_part + ".*").Count() > 0;
                if (isexist)
                    return string.Format(tempDirectory + "dup_{0}.{1}", file_name_part, audio_format);
                else
                    return string.Format(tempDirectory + "{0}.{1}", file_name_part, audio_format);
            }
            catch(Exception ex)
            {
                Logger.Current.WriteLine(string.Format("happen something wrong in GetAudioFileName (file_name_part:{0})", file_name_part));
                throw ex;
            }
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
            var download_file = temp_file_name + ".part";
            using (var sr = new FileStream(download_file, System.IO.FileMode.Create))
            {
                var count = 0;
                do
                {
                    byte[] data = new byte[256 * 1024];
                    count = await stream.ReadAsync(data, 0, data.Length);
                    await sr.WriteAsync(data, 0, count);
                    if (token != null && token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                } while (count != 0);
            }
            File.Move(download_file, temp_file_name);
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
