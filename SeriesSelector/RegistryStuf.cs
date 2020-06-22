using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SeriesSelector
{
    public class RegistryStuf
    {

        public const string idKey = @"SOFTWARE\TomsSeriesShit";

 



     
        private readonly RegistryKey key = Registry.CurrentUser.OpenSubKey(idKey, true);


        public RegistryStuf()
        {
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(idKey);
                key.SetValue(Series.TNG.ToString(), 0);
                key.SetValue(Series.Men.ToString(), 0);
            }
            
        }

        public void Increase(Series s, int amount = 1)
        {
            int val = GetCurrentIndex(s);
            val += amount;
            key.SetValue(s.ToString(), val);
        }




        public void Decrease(Series s, int amount = 1)
        {
            int val = GetCurrentIndex(s);
            val -= amount;
            key.SetValue(s.ToString(), val);
        }

     

        public int GetCurrentIndex(Series s)
        {
            if (key != null)
            {
                return int.Parse(key.GetValue(s.ToString()).ToString());
            }
            throw new Exception("kein key");
        }

     

        ~RegistryStuf()
        {
            key.Close();
        }

    }
}
