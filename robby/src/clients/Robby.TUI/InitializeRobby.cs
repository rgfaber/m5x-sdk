using Terminal.Gui;

namespace Robby.TUI
{
    public static class InitializeRobby
    {
        public class Dialog : Terminal.Gui.Dialog
        {
            public Dialog()
            {
                Title = "Initialize a new simulation";
                Width = Dim.Percent(50);
            }
        }
    }
}