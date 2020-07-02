using System;
using System.Windows;

namespace SeriesSelector
{
    public class PositionMaster
    {
        private readonly Window window;

        public PositionMaster(Window fe)
        {
            window = fe;
        }

        public void StorePosition()
        {
            var reg = new RegistryStuf();
            int height = (int)window.Height;
            int width = (int)window.Width;
            int xPos = (int)window.Left;
            int yPos = (int)window.Top;
            reg.Write("height", height);
            reg.Write("width", width);
            reg.Write("xPos", xPos);
            reg.Write("yPos", yPos);
        }

        public void LoadPosition()
        {
            try
            {
                var reg = new RegistryStuf();
                int h = reg.Read("height");
                int w = reg.Read("width");
                int x = reg.Read("xPos");
                int y = reg.Read("yPos");
                window.Height = h;
                window.Width = w;
                window.Left = x;
                window.Top = y;
            }
            catch (NullReferenceException)
            {
                //using defaults
            }
        }
    }
}