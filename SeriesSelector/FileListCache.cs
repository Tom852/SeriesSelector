using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSelector
{
    public class FileListCache
    {
        public readonly TimeSpan maxAge = TimeSpan.FromSeconds(30);

        public List<string> Filerinos { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> GetFiles(Func<List<string>> getFiles)
        {
            var age = DateTime.Now - CreatedAt;

            if (age > maxAge)
            {
                this.Filerinos = getFiles();
                CreatedAt = DateTime.Now;
            }

            return this.Filerinos.OrderBy(s => s).ToList();
        }
    }
}
