using System;
using M5x.Tty.Core;
using M5x.Tty.Types;

namespace Example
{
    public class Box10x : View
    {
        private readonly int h = 50;
        private readonly int w = 40;

        public Box10x(int x, int y) : base(new Rect(x, y, 20, 10))
        {
        }

        public bool WantCursorPosition { get; set; } = false;

        public Size GetContentSize()
        {
            return new(w, h);
        }

        public void SetCursorPosition(Point pos)
        {
            throw new NotImplementedException();
        }

        public override void Redraw(Rect bounds)
        {
            //Point pos = new Point (region.X, region.Y);
            Driver.SetAttribute(ColorScheme.Focus);

            for (var y = 0; y < h; y++)
            {
                Move(0, y);
                Driver.AddStr(y.ToString());
                for (var x = 0; x < w - y.ToString().Length; x++) //Driver.AddRune ((Rune)('0' + (x + y) % 10));
                    if (y.ToString().Length < w)
                        Driver.AddStr(" ");
            }
            //Move (pos.X, pos.Y);
        }
    }
}