using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using NicoNico.Net.Entities.Video;

namespace NicoNicoDownloader
{
    static class VideoFlvExtend
    {
        public static async Task<Stream> GetVideoAsync(this VideoFlv video, string nico_id, CookieContainer cookieContainer)
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
    }
}
