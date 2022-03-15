using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SeriesSelector
{
    public class PersistenceMaster
    {
        private readonly string appDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TomsSeriesSelector/SeriesSelectorData.xml");
        private readonly string inFolderFileName = "Ξ-SeriesSelectorData.data";

        private XmlSerializer collectionSerializer = new XmlSerializer(typeof(TrulyObservableCollection<Series>));

        public void Persist(TrulyObservableCollection<Series> list)
        {
            var env = Properties.Settings.Default.UsageEnv;
            if (env == UsageEnvironments.Local.ToString())
            {
                SaveAllDataToAppData(list);
            }
            else if (env == UsageEnvironments.Nas.ToString())
            {
                SaveAllDataToAppData(list);
                SaveCurrentFileToEachFolder(list);
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
            else if (env == UsageEnvironments.Nas.ToString())
            {
                var appDataData = LoadFromAppData();
                var result = new TrulyObservableCollection<Series>();

                foreach (var itemFromAppData in appDataData)
                {
                    var currentFileFromInFolder = LoadCurrentFileNameFromSeriesFolder(itemFromAppData.FolderPath);
                    var folder = itemFromAppData.FolderPath;
                    var file = currentFileFromInFolder ?? itemFromAppData.CurrentFileName;
                    var series = new Series() { FolderPath = folder, CurrentFileName = file };
                    result.Add(series);
                }
                return result;
            }
            else
            {
                Properties.Settings.Default.UsageEnv = UsageEnvironments.Local.ToString();
                Properties.Settings.Default.Save();
                throw new ApplicationException("Unknown usage env - has been reset");
            }
        }

        public string LoadCurrentFileNameFromSeriesFolder(string fodlerPath)
        {
            try
            {

                string dataFile = Path.Combine(fodlerPath, inFolderFileName);
                if (File.Exists(dataFile))
                {

                    var text = File.ReadAllLines(dataFile, Encoding.UTF8).First();
                    var candidateFiel = Path.Combine(fodlerPath, text);
                    if (File.Exists(candidateFiel))
                    {
                        return text;
                    }
                }
                return null;
            }
            catch
            {
                return null;
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


        private void SaveAllDataToAppData(TrulyObservableCollection<Series> list)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            collectionSerializer.Serialize(sw, list);
            string xmlResult = sw.GetStringBuilder().ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(appDataFile));
            File.WriteAllText(appDataFile, xmlResult);
        }


        private void SaveCurrentFileToEachFolder(TrulyObservableCollection<Series> list)
        {
            foreach (var item in list)
            {
                string dataFile = Path.Combine(item.FolderPath, inFolderFileName);

                if (File.Exists(dataFile))
                {
                    File.SetAttributes(dataFile, FileAttributes.Normal);

                }
                File.WriteAllText(dataFile, item.CurrentFileName, Encoding.UTF8);
                File.SetAttributes(dataFile, FileAttributes.Hidden);
            }
        }
    }
}