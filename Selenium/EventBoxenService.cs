using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace JuLiMl.Selenium
{
    public interface IEventBoxenService
    {
        void FindeEventBoxen(Image<Rgba32> imageDerWebsite);
    }
    public class EventBoxenService : IEventBoxenService
    {
        private Image<Rgba32> _image;
        private readonly List<Rectangle> _weisseFlaechen = new List<Rectangle>(2000);
        private readonly List<Rectangle> _kombinierteWeisseFlaechen = new List<Rectangle>();

        public void FindeEventBoxen(Image<Rgba32> imageDerWebsite)
        {
            _image = imageDerWebsite;

            FindeWeissePixel();
            KombiniereWeisseRects();

            _image = null;
        }

        private void FindeWeissePixel()
        {
            for(var y = 0; y < _image.Height; y++)
            {
                FindeErstenWeissenPixelRow(y);
            }
        }

        private void FindeErstenWeissenPixelRow(int y)
        {
            if (y == 1212)
            {
                var t = 1;
            }
            var ersterWeisserXPixel = -1;
            var letzterWeisserXPixel = -1;
            for (var x = 0; x < _image.Width; x++)
            {
                var pixel = _image[x, y];
                if(IstWeisserPixel(pixel))
                {
                    ersterWeisserXPixel = x;
                    break;
                }
            }

            for (var x = _image.Width - 1; x >= 0; x--)
            {
                var pixel = _image[x, y];
                if(IstWeisserPixel(pixel))
                {
                    letzterWeisserXPixel = x;
                    break;
                }
            }

            if (ersterWeisserXPixel == -1 | letzterWeisserXPixel == -1) return;
            
            var breite = letzterWeisserXPixel - ersterWeisserXPixel;
            var rect = new Rectangle(ersterWeisserXPixel, y, breite, 1);
            _weisseFlaechen.Add(rect);
        }

        private static bool IstWeisserPixel(Rgba32 pixel)
        {
            const byte whiteThreshold = 255;
            return pixel.R >= whiteThreshold & pixel.G >= whiteThreshold & pixel.B >= whiteThreshold;
        }

        private void KombiniereWeisseRects()
        {
            Rectangle? letzteRawFlaeche = null;
            var zusammenhaengend = false;
            var yStart = -1;
            foreach (var curRawFlaeche in _weisseFlaechen)
            {
                if(!letzteRawFlaeche.HasValue)
                {
                    letzteRawFlaeche = curRawFlaeche;
                    continue;
                }

                if(curRawFlaeche.X == letzteRawFlaeche.Value.X & Math.Abs(curRawFlaeche.Y - letzteRawFlaeche.Value.Y) == 1)
                {
                    if (!zusammenhaengend) yStart = curRawFlaeche.Y;
                    zusammenhaengend = true;
                }
                else
                {
                    if (zusammenhaengend)
                    {
                        var hoehe = letzteRawFlaeche.Value.Y - yStart;
                        var rect = new Rectangle(letzteRawFlaeche.Value.X, yStart, letzteRawFlaeche.Value.Width, hoehe);
                        _kombinierteWeisseFlaechen.Add(rect);
                    }
                }

                letzteRawFlaeche = curRawFlaeche;
            }
        }
    }
}
