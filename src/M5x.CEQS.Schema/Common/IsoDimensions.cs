namespace M5x.CEQS.Schema.Common
{
    public static class IsoDimensions
    {
        public static Measurement Time()
        {
            return new(decimal.Zero, 0, IsoUnits.Second);
        }

        public static Measurement Length()
        {
            return Measurement.CreateNew(decimal.Zero, 0, IsoUnits.Meter);
        }

        public static Measurement Mass()
        {
            return Measurement.CreateNew(decimal.Zero, 0, IsoUnits.KiloGram);
        }

        public static Measurement Displacement()
        {
            return Measurement.CreateNew(decimal.Zero, 0, IsoUnits.MetricTon);
        }
    }
}