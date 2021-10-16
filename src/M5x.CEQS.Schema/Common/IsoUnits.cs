namespace M5x.CEQS.Schema.Common
{
    public static class IsoUnits
    {
        public static Unit Meter = Unit.CreateNew("meter", "m");
        public static Unit Gram = Unit.CreateNew("gram", "g");
        public static Unit Second = Unit.CreateNew("second", "s");
        public static Unit MetricTon = Unit.CreateNew("tonne", "t", Gram, 6);
        public static Unit KiloGram = Unit.CreateNew("kilogram", "kg", Gram, 3);
        public static Unit Liter = Unit.CreateNew("liter", "l");
    }
}