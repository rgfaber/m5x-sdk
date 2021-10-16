using System;

namespace M5x.Utils
{
    public static class ConsoleUtils
    {
        private static Random RandCol => new();

        public static ConsoleColor GetRandomConsoleColor()
        {
            var consoleColors = Enum.GetValues(typeof(ConsoleColor));
            var col = (ConsoleColor)consoleColors.GetValue(RandCol.Next(consoleColors.Length));
            if (col == ConsoleColor.Black)
                col = ConsoleColor.Green;
            if (col == ConsoleColor.Blue)
                col = ConsoleColor.Cyan;
            if (col == ConsoleColor.DarkBlue)
                col = ConsoleColor.Cyan;
            if (col == ConsoleColor.DarkGray)
                col = ConsoleColor.White;
            if (col == ConsoleColor.DarkMagenta)
                col = ConsoleColor.White;
            return col;
        }

        public static void WriteError(Exception e)
        {
            if (e == null) return;
            var cf = Console.ForegroundColor;
            var cb = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine();
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"ERROR => {e.Message}");
            Console.WriteLine("-----------------------------");
            Console.ForegroundColor = cf;
            Console.BackgroundColor = cb;
            Console.WriteLine();
        }
    }
}