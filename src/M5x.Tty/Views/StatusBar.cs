//
// StatusBar.cs: a statusbar for an application
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// TODO:
//   Add mouse support

using System;
using M5x.Tty.Core;
using M5x.Tty.Types;
using NStack;
using Attribute = M5x.Tty.Core.Attribute;

namespace M5x.Tty.Views
{
    /// <summary>
    ///     <see cref="StatusItem" /> objects are contained by <see cref="StatusBar" /> <see cref="View" />s.
    ///     Each <see cref="StatusItem" /> has a title, a shortcut (hotkey), and an <see cref="Action" /> that will be invoked
    ///     when the
    ///     <see cref="StatusItem.Shortcut" /> is pressed.
    ///     The <see cref="StatusItem.Shortcut" /> will be a global hotkey for the application in the current context of the
    ///     screen.
    ///     The colour of the <see cref="StatusItem.Title" /> will be changed after each ~.
    ///     A <see cref="StatusItem.Title" /> set to `~F1~ Help` will render as *F1* using <see cref="ColorScheme.HotNormal" />
    ///     and
    ///     *Help* as <see cref="ColorScheme.HotNormal" />.
    /// </summary>
    public class StatusItem
    {
        /// <summary>
        ///     Initializes a new <see cref="StatusItem" />.
        /// </summary>
        /// <param name="shortcut">Shortcut to activate the <see cref="StatusItem" />.</param>
        /// <param name="title">Title for the <see cref="StatusItem" />.</param>
        /// <param name="action">Action to invoke when the <see cref="StatusItem" /> is activated.</param>
        public StatusItem(Key shortcut, ustring title, Action action)
        {
            Title = title ?? "";
            Shortcut = shortcut;
            Action = action;
        }

        /// <summary>
        ///     Gets the global shortcut to invoke the action on the menu.
        /// </summary>
        public Key Shortcut { get; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>
        ///     The colour of the <see cref="StatusItem.Title" /> will be changed after each ~.
        ///     A <see cref="StatusItem.Title" /> set to `~F1~ Help` will render as *F1* using <see cref="ColorScheme.HotNormal" />
        ///     and
        ///     *Help* as <see cref="ColorScheme.HotNormal" />.
        /// </remarks>
        public ustring Title { get; set; }

        /// <summary>
        ///     Gets or sets the action to be invoked when the statusbar item is triggered
        /// </summary>
        /// <value>Action to invoke.</value>
        public Action Action { get; }
    }

    /// <summary>
    ///     A status bar is a <see cref="View" /> that snaps to the bottom of a <see cref="Toplevel" /> displaying set of
    ///     <see cref="StatusItem" />s.
    ///     The <see cref="StatusBar" /> should be context sensitive. This means, if the main menu and an open text editor are
    ///     visible, the items probably shown will
    ///     be ~F1~ Help ~F2~ Save ~F3~ Load. While a dialog to ask a file to load is executed, the remaining commands will
    ///     probably be ~F1~ Help.
    ///     So for each context must be a new instance of a statusbar.
    /// </summary>
    public class StatusBar : View
    {
        private bool disposedValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatusBar" /> class.
        /// </summary>
        public StatusBar() : this(new StatusItem[] { })
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatusBar" /> class with the specified set of
        ///     <see cref="StatusItem" />s.
        ///     The <see cref="StatusBar" /> will be drawn on the lowest line of the terminal or <see cref="View.SuperView" /> (if
        ///     not null).
        /// </summary>
        /// <param name="items">A list of statusbar items.</param>
        public StatusBar(StatusItem[] items)
        {
            Width = Dim.Fill();
            Height = 1;
            Items = items;
            CanFocus = false;
            ColorScheme = Colors.Menu;
            X = 0;
            Width = Dim.Fill();
            Height = 1;

            Initialized += StatusBar_Initialized;
            Application.Resized += Application_Resized();
        }

        /// <summary>
        ///     The items that compose the <see cref="StatusBar" />
        /// </summary>
        public StatusItem[] Items { get; set; }

        private void StatusBar_Initialized(object sender, EventArgs e)
        {
            Y = SuperView.Frame.Height - 1;
        }

        private Action<Application.ResizedEventArgs> Application_Resized()
        {
            return delegate
            {
                X = 0;
                Height = 1;
                if (SuperView != null || SuperView is Toplevel) Y = SuperView.Frame.Height - (Visible ? 1 : 0);
            };
        }

        private Attribute ToggleScheme(Attribute scheme)
        {
            var result = scheme == ColorScheme.Normal ? ColorScheme.HotNormal : ColorScheme.Normal;
            Driver.SetAttribute(result);
            return result;
        }

        /// <inheritdoc />
        public override void Redraw(Rect bounds)
        {
            //if (Frame.Y != Driver.Rows - 1) {
            //	Frame = new Rect (Frame.X, Driver.Rows - 1, Frame.Width, Frame.Height);
            //	Y = Driver.Rows - 1;
            //	SetNeedsDisplay ();
            //}

            Move(0, 0);
            Driver.SetAttribute(Colors.Menu.Normal);
            for (var i = 0; i < Frame.Width; i++)
                Driver.AddRune(' ');

            Move(1, 0);
            var scheme = ColorScheme.Normal;
            Driver.SetAttribute(scheme);
            for (var i = 0; i < Items.Length; i++)
            {
                var title = Items[i].Title.ToString();
                for (var n = 0; n < Items[i].Title.RuneCount; n++)
                {
                    if (title[n] == '~')
                    {
                        scheme = ToggleScheme(scheme);
                        continue;
                    }

                    Driver.AddRune(title[n]);
                }

                if (i + 1 < Items.Length)
                {
                    Driver.AddRune(' ');
                    Driver.AddRune(Driver.VLine);
                    Driver.AddRune(' ');
                }
            }
        }

        /// <inheritdoc />
        public override bool ProcessHotKey(KeyEvent kb)
        {
            foreach (var item in Items)
                if (kb.Key == item.Shortcut)
                {
                    item.Action?.Invoke();
                    return true;
                }

            return false;
        }

        /// <inheritdoc />
        public override bool MouseEvent(MouseEvent me)
        {
            if (me.Flags != MouseFlags.Button1Clicked)
                return false;

            var pos = 1;
            for (var i = 0; i < Items.Length; i++)
            {
                if (me.X >= pos && me.X < pos + GetItemTitleLength(Items[i].Title)) Run(Items[i].Action);
                pos += GetItemTitleLength(Items[i].Title) + 3;
            }

            return true;
        }

        private int GetItemTitleLength(ustring title)
        {
            var len = 0;
            foreach (var ch in title)
            {
                if (ch == '~')
                    continue;
                len++;
            }

            return len;
        }

        private void Run(Action action)
        {
            if (action == null)
                return;

            Application.MainLoop.AddIdle(() =>
            {
                action();
                return false;
            });
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) Application.Resized -= Application_Resized();
                disposedValue = true;
            }
        }
    }
}