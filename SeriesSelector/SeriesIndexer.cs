using SeriesSelector.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

// should rethink exception handling
namespace SeriesSelector
{


    public class SeriesIndexer
    {

        private readonly string[] wordlist = new[]
            {
                    "dd51",
                    "dd20",
                    "x264",
                    "h264",
                    "x265",
                    "h265",
                    "144",
                    "240",
                    "360",
                    "480",
                    "720",
                    "1024",
                    "7p",
                    "72p",
                    "4k",
                    "2k",
                    ".mp4"

                };

        public (bool success, int season, int episode, bool hasName, string episodeName) GetSeasonAndIndex(string filenameOrPath)
        {
            string workingString = filenameOrPath.Split('/', '\\').ToList().Last();
            workingString = workingString.ToLower();
            workingString = FilterCommonMisleadingNumbers(workingString);

            Regex r = new Regex(@"([0-3]?\d)[x\-_eE]?(\d{2})( - )?(.*)\.[mp4|mkv]?"); //goal: 112 s1e12 S01E12 1x12 1-12 etc; max 39 seasons to be more robust against stuff like "720p"
            Match m = r.Match(workingString);


 

            if (m.Success)
            {
                int.TryParse(m.Groups[1].Value, out int s);
                int.TryParse(m.Groups[2].Value, out int e);
                bool hasName = m.Groups[3].Value != string.Empty;
                string name = m.Groups[4].Value;
                name = HotFixName(name);
                return (true, s, e, hasName, name);
            }
            else
            {
                return (false, 0, 0, false, string.Empty);
            }
        }

        private string HotFixName(string name)
        {
            string FirstCharUpper(string word)
                => word.Length < 2 ? word : char.ToUpper(word[0]) + word.Substring(1);

            var words = name.Split(' ');
            string result = string.Empty;
            bool isFirst = true;
            foreach (var word in words)
            {
                if (isFirst)
                {
                    result += FirstCharUpper(word);
                    isFirst = false;
                }
                else if(word.Length > 3)
                {
                    result += ' ';
                    result += FirstCharUpper(word);
                } else
                {
                    result += ' ';
                    result += word;
                }
            }
            return result;
        }

        private string FilterCommonMisleadingNumbers(string original)
        {
            string result = original;
            foreach (var badThing in wordlist)
            {
                result = result.Replace(badThing, "");
            }

            return result;
        }
    }
}