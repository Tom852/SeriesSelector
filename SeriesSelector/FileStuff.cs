using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSelector
{
    public class FileStuff
    {
        public const string starTrekFolder = @"F:\New Files\_series\Star Trek xD\TNG";
        public const string menfolder = @"E:\Serien\Two And A Half Men";

        public string GetFilePath(Series s)
        {
            if (s == Series.TNG)
            {
                return starTrekFolder;
            }
            if (s == Series.Men)
            {
                return menfolder;
            }
            throw new Exception("Unkown Series");
        }

        public string[] GetFileList(Series s)
        {
            return Directory.GetFiles(GetFilePath(s));
        }

        public string GetFullFilePath(Series s, int index)
        {
            var allFiles = GetFileList(s);
            var targetFile = allFiles[index];
            return targetFile;
        }

        public string GetJustFileName(Series s, int index)
        {
            var full = GetFullFilePath(s, index);
            var justName = Path.GetFileName(full);
            return justName;
        }

        public void Open(Series s, int index)
        {
            
            System.Diagnostics.Process.Start(GetFullFilePath(s, index));
        }


        public bool CanIncrease(Series s, int current, int amount)
        {
            var maxIndex = GetFileList(s).Length;
            return current + amount < maxIndex;
        }

        public bool CanDecrease(Series s, int current, int amount)
        {
            return current - amount >= 0;
        }
    }
}
