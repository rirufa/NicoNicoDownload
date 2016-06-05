using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using NicoNico.Net.Managers;
using System.IO;

namespace NicoNicoDownloader
{
    class NicoNicoDownload
    {
        //一時フォルダー（最後は円記号で終わらせること）
        const string tempDirectory = "download\\";
        const string tempFileName = "temp";

        NicoNico.Net.Entities.User.UserSession session;
        System.Net.CookieContainer cookieContainer;

        public NicoNicoDownload()
        {
        }

        public async Task Login(string email,string pass)
        {
            var authManager = new AuthenticationManager();
            var userLoginSession = await authManager.LoginUserThroughV1ApiAsync(email, pass);
            this.cookieContainer = authManager.CreateLoginCookieContainer(userLoginSession);

            // Reset the authentication manager with the proper cookie container.
            authManager = new AuthenticationManager(cookieContainer);
            this.session = await authManager.StartUserSessionAsync();

            // In order to test "extending" the users session, we need to reset the authManager again,
            // this time with the session key.
            authManager = new AuthenticationManager(cookieContainer, session.Session);

            var test = await authManager.ExtendUserSessionAsync();

            Logger.Current.WriteLine("login success");
        }

        public async Task<Stream> GetVideoAsync(NicoNico.Net.Entities.Video.VideoFlv video, string nico_id)
        {
            string nicohistory = "";
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                Uri video_uri = new Uri(string.Format("http://www.nicovideo.jp/watch/{0}", nico_id));
                await client.GetAsync(video_uri);
                nicohistory = handler.CookieContainer.GetCookies(video_uri)["nicohistory"].Value;
            }

            cookieContainer.Add(new System.Net.Cookie("nicohistory", nicohistory, "/", "nicovideo.jp"));
            var reqLog = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(video.Url);
            reqLog.CookieContainer = cookieContainer;
            var resLog = reqLog.GetResponse();
            return resLog.GetResponseStream();
        }

        public async Task GetMusicFile(string nico_id)
        {
            var videoManager = new VideoManager(cookieContainer, session.Session);
            var video = await videoManager.GetVideoFlvAsync(nico_id);
            string temp_file_name = string.Format(tempDirectory + "{0}.{1}", nico_id, this.GetCodecExt(video.Url));
            using (var stream = await this.GetVideoAsync(video,nico_id))
            using (var sr = new FileStream(temp_file_name, System.IO.FileMode.Create))
            {
                var count = 0;
                do
                {
                    byte[] data = new byte[1024 * 1024];
                    count = await stream.ReadAsync(data, 0, data.Length);
                    await sr.WriteAsync(data, 0, count);
                } while (count != 0);
            }
            Logger.Current.WriteLine(string.Format("get video from {0} and saved to {1}", temp_file_name, temp_file_name));

            var thumbManager = new ThumbManager(cookieContainer, session.Session);
            var thumb = await thumbManager.GetThumbInfoAsync(nico_id);
            string new_file_name = string.Format(tempDirectory + "{0}.m4a", this.TitleConverter.ConvertTitle(this.ConvertFileName(thumb.Title.Trim())));
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = "ffmpeg.exe";
            info.UseShellExecute = false;
            info.Arguments = string.Format("-i {0} -vn -acodec copy \"{1}\"", temp_file_name, new_file_name);
            info.WorkingDirectory = Environment.CurrentDirectory;
            info.CreateNoWindow = true;
            var p = System.Diagnostics.Process.Start(info);
            p.WaitForExit();

            Logger.Current.WriteLine(string.Format("get audio track from {0} and saved to {1}", temp_file_name, new_file_name));
        }

        public TitileConverterInfo TitleConverter
        {
            get;
            set;
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

        private string GetCodecExt(string url)
        {
            //こんな感じの文字列が来るので、そこからコーデックを判定する
            //"http://smile-fnl10.nicovideo.jp/smile?m=28943856.66275"
            Regex regex = new Regex("http://(.+)\\.nicovideo\\.jp/smile\\?(.+?)=.*");
            var match = regex.Match(url);
            if(!match.Success)
            {
                Logger.Current.WriteLine(string.Format("{0} is unknown codec", url));
                return "flv";   //とりあえずflvを返す
            }
            switch (match.Groups[2].Value)
            {
                case "m":
                    return "mp4";
                case "s":
                    return "swf";
                default:
                    return "flv";
            }
        }
    }
}
