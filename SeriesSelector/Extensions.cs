using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSelector
{
    public static class Extensions
    {

        public static byte[] ToRGB(this string name)
        {
            var list = name.ToLower().Reverse().ToList();
            char c1 = list[0];
            char c2 = list[1];
            char c3 = list[2];

            return new byte[]
            {
                ScaledByteFromChar(c1) , ScaledByteFromChar(c2) , ScaledByteFromChar(c3)
            };
        }

        private static byte ScaledByteFromChar(char x)
        {
            int startFrom0 = x - 'a';
            int rangedFrom0To250 = startFrom0 * 10;
            int rangedFrom5To255 = 5 + rangedFrom0To250;
            return (byte)rangedFrom5To255; //if over / underflow happens cause of spaces or numbers etc it does not matter, is just to get a color per series name.
        }
    }
}
