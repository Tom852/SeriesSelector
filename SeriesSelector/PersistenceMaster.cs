using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SeriesSelector
{
    public class PersistenceMaster
    {
        private readonly string persistenceFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TomsSeriesSelector/SeriesPickerData.xml");

        private XmlSerializer s = new XmlSerializer(typeof(TrulyObservableCollection<Series>));

        public void Persist(TrulyObservableCollection<Series> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            s.Serialize(sw, list);
            string xmlResult = sw.GetStringBuilder().ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(persistenceFile));
            File.WriteAllText(persistenceFile, xmlResult);
        }

        public TrulyObservableCollection<Series> Load()
        {
            if (File.Exists(persistenceFile))
            {
                try
                {
                    string xml = File.ReadAllText(persistenceFile);
                    TextReader tr = new StringReader(xml);

                    var data = (TrulyObservableCollection<Series>)s.Deserialize(tr);
                    return data;
                }
                catch
                {
                    return new TrulyObservableCollection<Series>();
                }
            }
            else
            {
                return new TrulyObservableCollection<Series>();
            }
        }
    }
}