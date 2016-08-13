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

    public enum NicoNicoFilterOperator
    {
        [LabeledEnum("gt")]
        Gt,
        [LabeledEnum("lt")]
        Lt,
        [LabeledEnum("gte")]
        Gte,
        [LabeledEnum("lte")]
        Lte,
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
        //内部的にこの順番に依存しているので属性の順番を変えないこと
        [LabeledEnum("video")]
        [LabeledEnum("contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime")]
        Video,
        [LabeledEnum("live")]
        [LabeledEnum("contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime,scoreTimeshiftReserved,liveStatus")]
        Live,
        [LabeledEnum("illust")]
        [LabeledEnum("contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime")]
        Illust,
        [LabeledEnum("manga")]
        [LabeledEnum("contentId,title,description,tags,categoryTags,viewCounter,mylistCounter,commentCounter,startTime")]
        Manga,
        [LabeledEnum("book")]
        [LabeledEnum("contentId,title,description,tags,viewCounter,mylistCounter,commentCounter,startTime")]
        Book,
        [LabeledEnum("channel")]
        [LabeledEnum("contentId,title,description,tags,startTime")]
        Channel,
        [LabeledEnum("channelarticle")]
        [LabeledEnum("contentId,title,description,tags,mylistCounter,commentCounter,startTime")]
        ChannelArticle,
        [LabeledEnum("news")]
        [LabeledEnum("contentId,title,description,tags,commentCounter,startTime")]
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

        public SearchBuilder Range(NicoNicoFilter field, NicoNicoFilterOperator ope, object value)
        {
            DateTime? isodate = value as DateTime?;
            if (isodate != null)
                return new SearchBuilder(this, string.Format("filters[{0}][{1}]={2}", field.GetLabel(), ope.GetLabel() , isodate.Value.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")));
            else
                return new SearchBuilder(this, string.Format("filters[{0}][{1}]={2}", field.GetLabel(), ope.GetLabel() , value));
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
            string sort_param = (order ? "%2b" : "-") + sort_field.GetLabel();  //+はエンコードしないとエラーになる
            return new SearchBuilder(string.Format(EndPoints.Search, type.GetLabel() , query, target.GetLabel(), sort_param, type.GetLabel(1)));
        }
    }
}
