using M5x.DEC.Persistence;
using M5x.Schemas;

namespace M5x.Store.Tests
{
    public enum VehicleColor
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet
    }

    public enum VehicleModel
    {
        Toyota,
        Chrysler,
        Volkswagen,
        Audi,
        Mercedes,
        Ford,
        BMW,
        Saab
    }

    public enum VehicleKind
    {
        Sedan,
        Break,
        Coupe,
        SUV
    }

    public enum VehicleFuel
    {
        Diesel,
        Petrol,
        Hydrogen,
        Electric
    }

    [Common.DbName("vehicle")]
    public record Vehicle : IReadEntity
    {
        public VehicleColor Color { get; set; }
        public VehicleKind Kind { get; set; }
        public VehicleModel Model { get; set; }
        public VehicleFuel Fuel { get; set; }
        public string Id { get; set; }
        public string Prev { get; set; }
    }
}