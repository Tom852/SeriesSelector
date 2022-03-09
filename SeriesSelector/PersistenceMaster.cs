using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SeriesSelector
{
    public class PersistenceMaster
    {
        private readonly string appDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TomsSeriesSelector/SeriesSelectorData.xml");
        private readonly string inFolderFileName = "_SeriesSelectorData.xml";

        private XmlSerializer collectionSerializer = new XmlSerializer(typeof(TrulyObservableCollection<Series>));
        private XmlSerializer singleSeriesSerializer = new XmlSerializer(typeof(Series));

        public void Persist(TrulyObservableCollection<Series> list)
        {
            var env = Properties.Settings.Default.UsageEnv;
            if (env == UsageEnvironments.Local.ToString())
            {
                SaveIndexesToAppdata(list);
            }
            else if (env == UsageEnvironments.Network.ToString())
            {
                SaveIndexesToAppdata(list); // index may be ignored then
                SaveEachIndexNetworkBased(list);
            }
            else
            {
                throw new ApplicationException("Unknown usage env");
            }

        }


        public TrulyObservableCollection<Series> Load()
        {
            var env = Properties.Settings.Default.UsageEnv;
            if (env == UsageEnvironments.Local.ToString())
            {
                return LoadFromAppData();
            }
            else if (env == UsageEnvironments.Network.ToString())
            {
                var appDataData = LoadFromAppData();
                var result = new TrulyObservableCollection<Series>();

                foreach (var item in appDataData)
                {
                    result.Add(LoadDataFromSeriesFolder(item)); // abusing series as path kinda
                }
                return result;
            }
            else
            {
                throw new ApplicationException("Unknown usage env");
            }
        }

        private Series LoadDataFromSeriesFolder(Series item)
        {
            try
            {

                string dataFile = Path.Combine(item.FolderPath, inFolderFileName);
                if (File.Exists(dataFile))
                {

                    string xml = File.ReadAllText(dataFile);
                    TextReader tr = new StringReader(xml);

                    var data = (Series)singleSeriesSerializer.Deserialize(tr);
                    return data;
                }
                else
                {
                    return item;
                }
            }
            catch
            {
                return item;
            }
        }

        private TrulyObservableCollection<Series> LoadFromAppData()
        {
            if (File.Exists(appDataFile))
            {
                try
                {
                    string xml = File.ReadAllText(appDataFile);
                    TextReader tr = new StringReader(xml);

                    var data = (TrulyObservableCollection<Series>)collectionSerializer.Deserialize(tr);
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


        private void SaveIndexesToAppdata(TrulyObservableCollection<Series> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            collectionSerializer.Serialize(sw, list);
            string xmlResult = sw.GetStringBuilder().ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(appDataFile));
            File.WriteAllText(appDataFile, xmlResult);
        }


        private void SaveEachIndexNetworkBased(TrulyObservableCollection<Series> list)
        {
            foreach (var item in list)
            {
                string dataFile = Path.Combine(item.FolderPath, inFolderFileName);

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                singleSeriesSerializer.Serialize(sw, item);
                string xmlResult = sw.GetStringBuilder().ToString();
                if (!File.Exists(dataFile))
                {
                    var fs = File.Create(dataFile);
                    fs.Close();
                }

                File.SetAttributes(dataFile, FileAttributes.Normal);
                File.WriteAllText(dataFile, xmlResult);
                File.SetAttributes(dataFile, FileAttributes.Hidden);
            }
        }
    }
}