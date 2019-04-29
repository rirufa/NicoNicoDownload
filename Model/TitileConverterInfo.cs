using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace NicoNicoDownloader.Model
{
    public class TitileConverterInfo
    {
        public TitileConverterInfo()
        {
        }

        public string Title = null;

        public string Description = null;

        public string[] InputPattern
        {
            get;
            set;
        }

        public string OutputParttern
        {
            get;
            set;
        }

        public string[] KnownBandList
        {
            get;
            set;
        }

        public string[] IgnoreTokenList
        {
            get;
            set;
        }

        public string GetTitle(string description)
        {
            string title = null;
            foreach (string pattern in this.InputPattern)
            {
                Regex regex = new Regex(pattern);
                Match m = regex.Match(description);
                if (m.Success)
                {
                    title = m.Groups["title"].Value.Trim();
                    return title;
                }
            }
            return title;
        }

        public string[] SplitToken(string s,char[] tokens)
        {
            var result = s.Split(tokens,StringSplitOptions.RemoveEmptyEntries);
            return result;
        }

        public string ParseOutputPartten(string band_name,string album_artist, string title)
        {
            string output = this.OutputParttern;

            if (band_name != null)
                output = output.Replace("%known_album_artist%", band_name);

            return output
                .Replace("%title%", title)
                .Replace("%album_artist%", album_artist);
        }

        public string ParseDescription(string description)
        {
            var parsed = Regex.Replace(description, "<br />", "\n");
            return Regex.Replace(parsed, "<br>", "\n");
        }

        public bool IsBandName(string name,string description)
        {
            string band_name = GetBandName(name, description);
            return band_name != null;
        }

        public string GetBandName(string name, string description)
        {
            if (this.KnownBandList == null)
                return null;

            string band_name = null;
            foreach (string pattern in this.KnownBandList)
            {
                Regex regex = new Regex(pattern);
                var m = regex.Match(name);
                if (m.Success)
                {
                    band_name = m.Groups[0].Value.Trim();
                    break;
                }
                m = regex.Match(description);
                if (m.Success)
                {
                    //グループ指定がされない場合は最初の要素以外に値が放り込まれない
                    band_name = (m.Groups[1].Value == string.Empty ? m.Groups[0].Value : m.Groups[1].Value).Trim();
                    break;
                }
            }
            return band_name;
        }

        public bool IsIgonoreToken(string name)
        {
            if (this.IgnoreTokenList == null)
                return false;

            foreach (string pattern in this.IgnoreTokenList)
            {
                Regex regex = new Regex(pattern);
                var m = regex.Match(name);
                if (m.Success)
                    return true;
            }
            return false;
        }

        public static TitileConverterInfo Build(string file_path, string known_band_file_path = null,string ignore_token_file_path = null)
        {
            TitileConverterInfo info = new TitileConverterInfo();
            using (StreamReader sr = new StreamReader(file_path,Encoding.Default))
            {
                info.OutputParttern = sr.ReadLine();
                List<string> input_list = new List<string>();
                while(!sr.EndOfStream)
                {
                    input_list.Add(sr.ReadLine());
                }
                info.InputPattern = input_list.ToArray();
            }
            if(known_band_file_path != null)
            {
                using (StreamReader sr = new StreamReader(known_band_file_path, Encoding.Default))
                {
                    List<string> known_band = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        known_band.Add(sr.ReadLine());
                    }
                    info.KnownBandList = known_band.ToArray();
                }
            }
            if (ignore_token_file_path != null)
            {
                using (StreamReader sr = new StreamReader(ignore_token_file_path, Encoding.Default))
                {
                    List<string> ignore_tokens = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        ignore_tokens.Add(sr.ReadLine());
                    }
                    info.IgnoreTokenList = ignore_tokens.ToArray();
                }
            }
            return info;
        }
    }
}
