using System;
using M5x.Config;
using Terminal.Gui;

namespace Robby.TUI
{
    internal class Program
    {
        private static readonly App win = new("Robby")
        {
            X = 0,
            Y = 1, // Leave one row for the toplevel menu

            // By using Dim.Fill(), it will automatically resize without manual intervention
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        private static readonly MenuBar menu = new(new[]
        {
            new MenuBarItem("_File", new[]
            {
                new("_New", "Creates new file", null),
                new MenuItem("_Close", "", null),
                new MenuItem("_Quit", "", () =>
                {
                    if (Quit())
                        Application.Top.Running = false;
                })
            }),
            new MenuBarItem("_Edit", new[]
            {
                new MenuItem("_Copy", "", null),
                new MenuItem("C_ut", "", null),
                new MenuItem("_Paste", "", null)
            })
        });


        private static bool Quit()
        {
            var n = MessageBox.Query(50, 7,
                "Quit Demo",
                "Are you sure you want to quit this demo?",
                "Yes", "No");
            return n == 0;
        }

        private static void Main(string[] args)
        {
            DotEnv.FromEmbedded();

            Application.Init();

            var mainApp = Application.Top;

// Creates the top-level window to show


            mainApp.Add(win, menu);

            Application.Run<App>(exception =>
            {
                Console.WriteLine(exception);
                return true;
            });
        }
    }
}