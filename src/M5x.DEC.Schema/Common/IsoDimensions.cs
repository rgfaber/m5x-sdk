namespace M5x.DEC.Schema.Common
{
    public static class IsoDimensions
    {
        public static Measurement Time()
        {
            return new Measurement(decimal.Zero, 0, IsoUnits.Second);
        }

        public static Measurement Length()
        {
            return Measurement.New(decimal.Zero, 0, IsoUnits.Meter);
        }

        public static Measurement Mass()
        {
            return Measurement.New(decimal.Zero, 0, IsoUnits.KiloGram);
        }

        public static Measurement Displacement()
        {
            return Measurement.New(decimal.Zero, 0, IsoUnits.MetricTon);
        }
    }
}