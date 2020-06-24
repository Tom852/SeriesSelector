using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeriesSelector
{
    public class PersistenceMaster
    {
        private readonly string persistenceFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TomsSeriesShit/SeriesPicker.data");

        public void Persist(SeriesViewModel model)
        {
            var s = new XmlSerializer(typeof(SeriesViewModel));
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            s.Serialize(sw, model);
            string xmlResult = sw.GetStringBuilder().ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(persistenceFile));
            File.WriteAllText(persistenceFile, xmlResult);
        }

        public SeriesViewModel Load()
        {
            if (File.Exists(persistenceFile))
            {
                string xml = File.ReadAllText(persistenceFile);
                TextReader tr = new StringReader(xml);

                var s = new XmlSerializer(typeof(SeriesViewModel));
                var data = (SeriesViewModel)s.Deserialize(tr);
                return data;
            }
            else
            {
                return new SeriesViewModel();
            }
        }
    }
}
