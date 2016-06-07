using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NicoNico.Net.Tools;

namespace NicoNico.Net.Entities.Search
{
    public enum NicoNicoSort
    {
        [LabeledEnum("viewCounter")]
        ViewCounter,
        [LabeledEnum("mylistCounter")]
        MylistCounter,
        [LabeledEnum("commentCounter")]
        CommentCounter,
        [LabeledEnum("startTime")]
        StartTime,
        [LabeledEnum("thumbnailUrl")]
        ThumbnailUrl,
        [LabeledEnum("scoreTimeshiftReserved")]
        ScoreTimeshiftReserved,
    }

    public enum NicoNicoFilter
    {
        [LabeledEnum("contentId")]
        ContentId,
        [LabeledEnum("tags")]
        Tags,
        [LabeledEnum("categoryTags")]
        CategoryTags,
        [LabeledEnum("viewCounter")]
        ViewCounter,
        [LabeledEnum("mylistCounter")]
        MylistCounter,
        [LabeledEnum("startTime")]
        StartTime,
        [LabeledEnum("commentCounter")]
        CommentCounter,
        [LabeledEnum("liveStatus")]
        LiveStatus,
    }

    public enum NicoNicoTarget
    {
        [LabeledEnum("title,description,tags")]
        Keyword,
        [LabeledEnum("tagsExact")]
        Tag,
    }

    public enum SearchType
    {
        [LabeledEnum("video")]
        Video,
        [LabeledEnum("live")]
        Live,
        [LabeledEnum("illust")]
        Illust,
        [LabeledEnum("manga")]
        Manga,
        [LabeledEnum("book")]
        Book,
        [LabeledEnum("channel")]
        Channel,
        [LabeledEnum("channelarticle")]
        ChannelArticle,
        [LabeledEnum("news")]
        News,
    }

    public class SearchEntitiy
    {
        [JsonProperty("data")]
        public Search[] Data;
    }

    public class Search
    {
        [JsonProperty("contentId")]
        public string ContentId;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("tags")]
        public string Tags;
        [JsonProperty("categoryTags")]
        public string CategoryTags;
        [JsonProperty("viewCounter")]
        public string ViewCounter;
        [JsonProperty("mylistCounter")]
        public string MylistCounter;
        [JsonProperty("commentCounter")]
        public string CommentCounter;
        [JsonProperty("startTime")]
        public DateTime StartTime;
        [JsonProperty("communityIcon")]
        public string CommunityIcon;
        [JsonProperty("liveStatus")]
        public string LiveStatus;
    }

    public class SearchBuilder
    {
        public string Query
        {
            get;
            set;
        }

        private SearchBuilder(string query) : this(null,query)
        {

        }

        private SearchBuilder(SearchBuilder s,string query)
        {
            if (s != null)
                this.Query = s.Query  + "&" + query;
            else
                this.Query = query;
        }

        public SearchBuilder RangeFrom(NicoNicoFilter field,object value)
        {

            return new SearchBuilder(this, string.Format("filters[{0}][gte]={1}", field.GetLabel(), value));
        }

        public SearchBuilder RangeTo(NicoNicoFilter field, object value)
        {
            return new SearchBuilder(this, string.Format("filters[{0}][lt]={1}", field.GetLabel(), value));
        }

        public SearchBuilder RangeEqual(NicoNicoFilter field, object value)
        {
            return new SearchBuilder(this, string.Format("filters[{0}][0]={1}", field.GetLabel(), value));
        }

        public SearchBuilder Offset(int o)
        {
            return new SearchBuilder(this, string.Format("_offset={0}",o));
        }

        public SearchBuilder Limit(int l)
        {
            return new SearchBuilder(this, string.Format("_limit={0}", l));
        }

        public static SearchBuilder Build(SearchType type,string query,NicoNicoTarget target,NicoNicoSort sort_field,bool order)
        {
            string field = "";
            if (type == SearchType.Live)
                field = "contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime,scoreTimeshiftReserved,liveStatus";
            else
                field = "contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime";
            string sort_param = (order ? "%2b" : "-") + sort_field.GetLabel();  //+はエンコードしないとエラーになる
            return new SearchBuilder(string.Format(EndPoints.Search, type.GetLabel() , query, target.GetLabel(), sort_param, field));
        }
    }
}
