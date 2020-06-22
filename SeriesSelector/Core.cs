using System.Text.RegularExpressions;

namespace SeriesSelector
{
    public class Core
    {
        readonly RegistryStuf reg = new RegistryStuf();
        readonly FileStuff fileStuff = new FileStuff();

        public void PlayNext(Series s)
        {
            int index = reg.GetCurrentIndex(s);
            fileStuff.Open(s, index);
            Increase(s);
        }

        public void Increase(Series s, int amount = 1)
        {
            if (fileStuff.CanIncrease(s, reg.GetCurrentIndex(s), amount))
            {
                reg.Increase(s, amount);
            }
        }

        public void Decrease(Series s, int amount = 1)
        {
            if (fileStuff.CanDecrease(s, reg.GetCurrentIndex(s), amount))
            {
                reg.Decrease(s, amount);
            }
        }

        public string GetPrintableName(Series s)
        {
            int index = reg.GetCurrentIndex(s);
            string filename = fileStuff.GetJustFileName(s, index);
            Regex r = new Regex(@"(.* - )(\d{2}x\d{2}.*)");
            Match m = r.Match(filename);
            return m.Groups[2].Value;
        }
    }
}
