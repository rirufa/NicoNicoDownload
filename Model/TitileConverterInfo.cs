using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NicoNicoDownloader.Model
{
    public class TitileConverterInfo
    {
        public TitileConverterInfo()
        {
        }

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

        public string ConvertTitle(string name,string description = "")
        {
            string band_name = this.GetBandName(name, description);
            foreach (string pattern in this.InputPattern)
            {
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(name))
                {
                    Logger.Current.WriteLine(string.Format("{0} matched {1}", name, pattern));
                    return Regex.Replace(name, pattern, (e) => {
                        string output = this.OutputParttern;

                        if (band_name != null)
                            output = output.Replace("%known_album_artist%", band_name);

                        return output
                            .Replace("%title%", e.Groups["title"].Value.Trim())
                            .Replace("%album_artist%", e.Groups["album_artist"].Value.Trim());
                    });
                }
            }
            return name;
        }

        private string GetBandName(string name, string description)
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
                    band_name = m.Groups[1].Value.Trim();
                    break;
                }
            }
            return band_name;
        }

        public static TitileConverterInfo Build(string file_path, string known_band_file_path = null)
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
            return info;
        }
    }
}
