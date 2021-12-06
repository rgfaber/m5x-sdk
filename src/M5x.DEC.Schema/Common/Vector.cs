using System;

namespace M5x.DEC.Schema.Common
{
    public record Vector : IPayload
    {
        public Vector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector()
        {
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public override string ToString()
        {
            return $"Postion: ({X},{Y},{Z})";
        }

        public static Vector New(int x, int y, int z)
        {
            return new Vector(x, y, z);
        }

        public static Vector Random(int x, int y, int z)
        {
            return New(
                new Random().Next(0, x), 
                new Random().Next(0, y), 
                new Random().Next(0, z));
        }
    }
}