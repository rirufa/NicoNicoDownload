using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NicoNicoDownloader
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

        public string ConvertTitle(string name)
        {
            foreach (string pattern in this.InputPattern)
            {
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(name))
                {
                    Logger.Current.WriteLine(string.Format("{0} matched {1}", name, pattern));
                    return Regex.Replace(name, pattern, (e) => {
                        return this.OutputParttern.Replace("%title%", e.Groups["title"].Value.Trim());
                    });
                }
            }
            return name;
        }

        public static TitileConverterInfo Build(string file_path)
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
            return info;
        }
    }
}
