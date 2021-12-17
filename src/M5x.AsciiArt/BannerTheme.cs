using System;

namespace M5x.AsciiArt;

public interface IBannerTheme
{
    string DisplayName { get; set; }
    ConsoleColor Foreground { get; set; }
    ConsoleColor BackGround { get; set; }
}

public class BannerTheme : IBannerTheme
{
    public BannerTheme(string name, ConsoleColor foreGround, ConsoleColor backGround)
    {
        DisplayName = name;
        Foreground = foreGround;
        BackGround = backGround;
    }

    public static IBannerTheme Default { get; } =
        new BannerTheme("Default", ConsoleColor.Yellow, ConsoleColor.Black);

    public string DisplayName { get; set; }
    public ConsoleColor Foreground { get; set; }
    public ConsoleColor BackGround { get; set; }
}