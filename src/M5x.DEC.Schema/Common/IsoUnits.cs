namespace M5x.DEC.Schema.Common;

public static class IsoUnits
{
    public static Unit Meter = Unit.New("meter", "m");
    public static Unit Gram = Unit.New("gram", "g");
    public static Unit Second = Unit.New("second", "s");
    public static Unit MetricTon = Unit.New("tonne", "t", Gram, 6);
    public static Unit KiloGram = Unit.New("kilogram", "kg", Gram, 3);
    public static Unit Liter = Unit.New("liter", "l");
}