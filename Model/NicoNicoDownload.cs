using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using NicoNico.Net.Managers;
using System.IO;

namespace NicoNicoDownloader.Model
{
    class NicoNicoDownload
    {

        NicoNico.Net.Entities.User.UserSession session;
        System.Net.CookieContainer cookieContainer;

        public NicoNicoDownload()
        {
            this.VideoToAudioConveter = new VideoToAudioConveter();
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


        public async Task GetMusicFile(DownloadMusicItem item, CancellationTokenSource token = null)
        {
            string nico_id = item.id;
            try
            {
                var videoManager = new VideoManager(cookieContainer, session.Session);
                var video_info = await videoManager.GetVideoInfoAsync(nico_id);
                var video = await videoManager.GetVideoFlvAsync(nico_id);
                var video_codec = this.GetCodecExt(video.Url);
                string video_file_name = this.VideoToAudioConveter.GetVideoFileName(nico_id, video_codec);
                using (var stream = await video.GetVideoAsync(nico_id, cookieContainer))
                {
                    await this.VideoToAudioConveter.GetVideoFile(video_file_name, stream, token);
                    Logger.Current.WriteLine(string.Format("get video from {0} and saved to {1}", nico_id, video_file_name));
                }
                var audio_codec = this.GetAudioCodec(video_codec);
                string new_file_name = this.VideoToAudioConveter.GetAudioFileName(item.music_name, audio_codec);
                this.VideoToAudioConveter.GetAudioFile(video_file_name, new_file_name);

                File.Delete(video_file_name);

                Logger.Current.WriteLine(string.Format("get audio track from {0} and saved to {1}", video_file_name, new_file_name));
            }
            catch (TaskCanceledException)
            {
                Logger.Current.WriteLine(string.Format("canceled get video track from {0}", nico_id));
                return;
            }
            catch (Exception ex)
            {
                Logger.Current.WriteLine(string.Format("failed get audio track from {0}", nico_id));
                Logger.Current.WriteLine(ex.ToString());
                throw ex;
            }
        }

        public VideoToAudioConveter VideoToAudioConveter
        {
            get;
            set;
        }

        public TitileConverterInfo TitleConverter
        {
            get
            {
                return this.VideoToAudioConveter.TitleConverter;
            }
            set
            {
                this.VideoToAudioConveter.TitleConverter = value;
            }
        }

        private void GetAudioFile(string temp_file_name,string new_file_name)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = "ffmpeg.exe";
            info.UseShellExecute = false;
            info.Arguments = string.Format("-i {0} -vn -acodec copy \"{1}\"", temp_file_name, new_file_name);
            info.WorkingDirectory = Environment.CurrentDirectory;
            info.CreateNoWindow = true;
            var p = System.Diagnostics.Process.Start(info);
            p.WaitForExit();
        }

        private string GetAudioCodec(string video_type)
        {
            switch (video_type)
            {
                case "flv":
                    return "mp3";
                case "swf":
                    return "mp3";
                default:
                    return "m4a";
            }
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
