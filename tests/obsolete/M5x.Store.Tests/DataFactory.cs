using System;
using System.Collections.Generic;
using M5x.Utils;

namespace M5x.Store.Tests
{
    public static class DataFactory
    {
        private static readonly TypeBase[] _types =
        {
            new AnimalType(), new PlantType(), new VehicleType()
        };

        private static readonly string[] _mood =
        {
            "Happy", "Sad", "Exited", "Depressed", "Sincere", "Sinister", "Sensual", "Opportunistic", "Selfish",
            "Timid", "Intripid", "Lonely", "Scared"
        };

        private static readonly string[] _color =
        {
            "Blue", "Red", "Yellow", "Orange", "Purple", "Green", "Black", "White", "Brown", "Magenta", "Cyan"
        };

        private static readonly string[] _animal =
        {
            "Human", "Monkey", "Buffalo", "Kanguroo", "Hippo", "Lion", "Zebra", "Antilope", "Starfish", "Dog", "Cat"
        };


        private static readonly string[] _flower =
        {
            "Daisy", "Rose", "Tulip", "Lilly"
        };


        private static readonly string[] _vehicle =
        {
            "Bicycle", "Motor", "Bus", "Car", "Boat"
        };


        public static IEnumerable<ItemBase> Items = GenerateDemoItems();


        public static ItemBase Unknown => new() {Id = GuidUtils.NullGuid, DisplayName = "Unknown"};


        private static string RandomDisplayName(int ti)
        {
            var im = new Random().Next(0, _mood.Length - 1);
            var ic = new Random().Next(0, _color.Length - 1);
            switch (ti)
            {
                case 0:
                {
                    var ia = new Random().Next(0, _animal.Length - 1);
                    return $"{_mood[im]} {_color[ic]} {_animal[ia]}";
                }

                case 1:
                {
                    var ia = new Random().Next(0, _flower.Length - 1);
                    return $"{_mood[im]} {_color[ic]} {_flower[ia]}";
                }

                case 2:
                {
                    var ia = new Random().Next(0, _vehicle.Length - 1);
                    return $"{_mood[im]} {_color[ic]} {_vehicle[ia]}";
                }
            }

            return default;
        }

        public static IEnumerable<ItemBase> GenerateDemoItems()
        {
            var res = new List<ItemBase>();
            var max = new Random().Next(1, 10);
            for (var j = 0; j < max; j++)
            {
                var it = GenerateDemoItem();
                res.Add(it);
            }

            return res;
        }

        public static ItemBase GenerateDemoItem()
        {
            var ti = new Random().Next(0, 2);
            return new ItemBase
            {
                Id = Convert.ToString(new Random().Next(1, 2048)),
                Category = RandomType(ti),
                DisplayName = RandomDisplayName(ti)
            };
        }

        private static TypeBase RandomType(int i)
        {
            return _types[i];
        }

        public static IEnumerable<ItemBase> GenerateDefaults()
        {
            return new[] {Unknown};
        }

        public static IEnumerable<TypeBase> GenerateDemoTypes()
        {
            return _types;
        }
    }

    internal class AnimalType : TypeBase
    {
        public AnimalType()
        {
            Id = "TYPE_ANIMAL";
            DisplayName = "Animal";
        }
    }


    internal class PlantType : TypeBase
    {
        public PlantType()
        {
            Id = "TYPE_PLANT";
            DisplayName = "Plant";
        }
    }


    internal class VehicleType : TypeBase
    {
        public VehicleType()
        {
            Id = "TYPE_VEHICLE";
            DisplayName = "Vehicle";
        }
    }
}