using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NicoNico.Net.Entities.Search;
using NicoNico.Net.Managers;

namespace NicoNicoDownloader.Model
{
    class Scraper
    {
        const string config_file_name = "last_scraped";
        const string date_format = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        NicoNico.Net.Entities.User.UserSession session;
        System.Net.CookieContainer cookieContainer;
        DateTime lastDateTime;

        public Scraper()
        {
            if (File.Exists(config_file_name))
                this.lastDateTime = DateTime.Parse(File.ReadAllText(config_file_name),null,System.Globalization.DateTimeStyles.RoundtripKind);
            else
                this.lastDateTime = new DateTime(2006,12,12);   //ニコニコ動画の開始日以降を指定する

        }

        public async Task Login(string email, string pass)
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

        }

        public IEnumerable<DownloadMusicItem> Scrape(string query)
        {
            const int max_records = 100;
            SearchManager search_manager = new SearchManager(cookieContainer, session.Session);
            for(int i = 0; i < 16; i++)
            {
                var search_task = search_manager.Search(
                    SearchBuilder.Build(
                        SearchType.Video, query, NicoNicoTarget.Keyword, NicoNicoSort.StartTime, false)
                        .Offset(i * max_records)
                        .Limit(max_records)
                        .Range(NicoNicoFilter.StartTime,NicoNicoFilterOperator.Gte,this.lastDateTime)
                    );
                var videoToAudioConveter = new VideoToAudioConveter();
                videoToAudioConveter.TitleConverter = TitileConverterInfo.Build("format.txt", "bands.txt");
                var search_result = search_task.Result;
                foreach (var search in search_result.Data)
                {
                    Logger.Current.WriteLine(string.Format("getting titile from {0}", search.ContentId));
                    Logger.Current.WriteLine(string.Format("title:{0} description:{1}", search.Title, search.Description));
                    string title = videoToAudioConveter.GetMusicTitle(search.Title, search.Description);
                    yield return new DownloadMusicItem(search.ContentId, title);
                }
            }

            using (StreamWriter sw = new StreamWriter(config_file_name))
            {
                sw.WriteLine(DateTime.Now.ToString(date_format));
            }
        }
    }
}
