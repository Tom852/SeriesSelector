using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SeriesSelector
{
    public class PersistenceMaster
    {
        private readonly string persistenceFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TomsSeriesSelector/SeriesPicker.data");

        private XmlSerializer s = new XmlSerializer(typeof(ObservableCollection<Series>));

        public void Persist(ObservableCollection<Series> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            s.Serialize(sw, list);
            string xmlResult = sw.GetStringBuilder().ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(persistenceFile));
            File.WriteAllText(persistenceFile, xmlResult);
        }

        public ObservableCollection<Series> Load()
        {
            if (File.Exists(persistenceFile))
            {
                try
                {
                    string xml = File.ReadAllText(persistenceFile);
                    TextReader tr = new StringReader(xml);

                    var data = (ObservableCollection<Series>)s.Deserialize(tr);
                    return data;
                }
                catch
                {
                    return new ObservableCollection<Series>();
                }
            }
            else
            {
                return new ObservableCollection<Series>();
            }
        }
    }
}