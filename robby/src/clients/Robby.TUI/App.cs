using NStack;
using Terminal.Gui;

namespace Robby.TUI
{
    public class App : Window
    {
        public App(ustring title = null) : base(title)
        {
            var level = Application.Current;
            // MainView _mainView = new MainView(0, 1, title);
            // level.Add(_mainView);
        }

        public App()
        {
        }
    }
}