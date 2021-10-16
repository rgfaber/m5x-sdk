using NStack;
using Terminal.Gui;

namespace Robby.TUI
{
    public class MainView : View
    {
        private static Toplevel _top;


        private static readonly Label login = new("Login: ") {X = 3, Y = 2};

        private static readonly Label password = new("Password: ")
        {
            X = Pos.Left(login),
            Y = Pos.Top(login) + 1
        };

        private static readonly TextField loginText = new("")
        {
            X = Pos.Right(password),
            Y = Pos.Top(login),
            Width = 40
        };

        private readonly TextField passText = new("")
        {
            Secret = true,
            X = Pos.Left(loginText),
            Y = Pos.Top(password),
            Width = Dim.Width(loginText)
        };

        public MainView()
        {
        }

        public MainView(int x, int y, ustring text) : base(x, y, text)
        {
            Add(login,
                password,
                loginText,
                passText,
                // The ones laid out like an australopithecus, with Absolute positions:
                new CheckBox(3, 6, "Remember me"),
                new RadioGroup(3, 8, new ustring[] {"_Personal", "_Company"}),
                new Button(3, 14, "Ok"),
                new Button(10, 14, "Cancel"),
                new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar")
            );
        }

        public MainView(Rect rect, ustring text) : base(rect, text)
        {
        }

        public MainView(ustring text, TextDirection direction = TextDirection.LeftRight_TopBottom) : base(text,
            direction)
        {
        }


        public MainView(Rect frame) : base(frame)
        {
        }

        private static bool Quit()
        {
            var n = MessageBox.Query(50, 7,
                "Quit Demo",
                "Are you sure you want to quit this demo?",
                "Yes", "No");
            return n == 0;
        }
    }
}