using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using NicoNico.Net.Entities.Video;

namespace NicoNicoDownloader.Model
{
    static class VideoFlvExtend
    {
        const string ua = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; Touch; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; Tablet PC 2.0; rv:11.0) like Gecko";
        public static async Task<Stream> GetVideoAsync(this VideoFlv video, string nico_id, CookieContainer cookieContainer)
        {
            
            string nicohistory = "";
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                //ユーザーエージェントを追加しないと突然接続が落とされることがある
                client.DefaultRequestHeaders.UserAgent.ParseAdd(ua);
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
    }
}
