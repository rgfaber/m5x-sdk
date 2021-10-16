using System;
using M5x.Tty.Core;
using M5x.Tty.Types;

namespace Example
{
    public class Filler : View
    {
        private int h = 50;
        private int w = 40;

        public Filler(Rect rect) : base(rect)
        {
            w = rect.Width;
            h = rect.Height;
        }

        public Size GetContentSize()
        {
            return new(w, h);
        }

        public override void Redraw(Rect bounds)
        {
            Driver.SetAttribute(ColorScheme.Focus);
            var f = Frame;
            w = 0;
            h = 0;

            for (var y = 0; y < f.Width; y++)
            {
                Move(0, y);
                var nw = 0;
                for (var x = 0; x < f.Height; x++)
                {
                    Rune r;
                    switch (x % 3)
                    {
                        case 0:
                            var er = y.ToString().ToCharArray(0, 1)[0];
                            nw += er.ToString().Length;
                            Driver.AddRune(er);
                            if (y > 9)
                            {
                                er = y.ToString().ToCharArray(1, 1)[0];
                                nw += er.ToString().Length;
                                Driver.AddRune(er);
                            }

                            r = '.';
                            break;
                        case 1:
                            r = 'o';
                            break;
                        default:
                            r = 'O';
                            break;
                    }

                    Driver.AddRune(r);
                    nw += Rune.RuneLen(r);
                }

                if (nw > w)
                    w = nw;
                h = y + 1;
            }
        }
    }
}