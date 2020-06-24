using Microsoft.Win32;
using System;

namespace SeriesSelector
{
    public class RegistryStuf
    {
        public const string idKey = @"SOFTWARE\TomsSeriesShit";
        public const string keyWidth = "width";
        public const string keyHeight = "height";
        public const string keyXPos = "xPos";
        public const string keyYPos = "yPos";

        private readonly RegistryKey key = Registry.CurrentUser.OpenSubKey(idKey, true);

        public RegistryStuf()
        {
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(idKey);
            }

            if (key == null)
            {
                throw new InvalidOperationException("Error Creating Registry Key");
            }
        }

        public void Write(string k, int val)
        {
            key.SetValue(k, val);
        }

        public int Read(string k)
        {
            var r = key.GetValue(k);
            return int.Parse(r.ToString());
        }


        ~RegistryStuf()
        {
            key?.Dispose();
        }
    }
}