﻿using System;
using System.Linq;
using M5x.Tty.Core;
using M5x.Tty.Types;
using NStack;
using Attribute = M5x.Tty.Core.Attribute;

namespace M5x.Tty.ConsoleDrivers.FakeDriver
{
    /// <summary>
    ///     Implements a mock ConsoleDriver for unit testing
    /// </summary>
    public class FakeDriver : ConsoleDriver
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override int Cols { get; }

        public override int Rows { get; }

        public override int Top => 0;

        // The format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
        private int[,,] contents;
        private bool[] dirtyLine;

        private void UpdateOffscreen()
        {
            var cols = Cols;
            var rows = Rows;

            contents = new int [rows, cols, 3];
            for (var r = 0; r < rows; r++)
            for (var c = 0; c < cols; c++)
            {
                contents[r, c, 0] = ' ';
                contents[r, c, 1] = MakeColor(ConsoleColor.Gray, ConsoleColor.Black);
                contents[r, c, 2] = 0;
            }

            dirtyLine = new bool [rows];
            for (var row = 0; row < rows; row++)
                dirtyLine[row] = true;
        }

        private static readonly bool sync = false;

        public FakeDriver()
        {
            Cols = FakeConsole.WindowWidth;
            Rows = FakeConsole.WindowHeight; // - 1;
            UpdateOffscreen();
        }

        private bool needMove;

        // Current row, and current col, tracked by Move/AddCh only
        private int ccol, crow;

        public override void Move(int col, int row)
        {
            ccol = col;
            crow = row;

            if (Clip.Contains(col, row))
            {
                FakeConsole.CursorTop = row;
                FakeConsole.CursorLeft = col;
                needMove = false;
            }
            else
            {
                FakeConsole.CursorTop = Clip.Y;
                FakeConsole.CursorLeft = Clip.X;
                needMove = true;
            }
        }

        public override void AddRune(Rune rune)
        {
            rune = MakePrintable(rune);
            if (Clip.Contains(ccol, crow))
            {
                if (needMove) //MockConsole.CursorLeft = ccol;
                    //MockConsole.CursorTop = crow;
                    needMove = false;
                contents[crow, ccol, 0] = (int) (uint) rune;
                contents[crow, ccol, 1] = currentAttribute;
                contents[crow, ccol, 2] = 1;
                dirtyLine[crow] = true;
            }
            else
            {
                needMove = true;
            }

            ccol++;
            //if (ccol == Cols) {
            //	ccol = 0;
            //	if (crow + 1 < Rows)
            //		crow++;
            //}
            if (sync)
                UpdateScreen();
        }

        public override void AddStr(ustring str)
        {
            foreach (var rune in str)
                AddRune(rune);
        }

        public override void End()
        {
            FakeConsole.ResetColor();
            FakeConsole.Clear();
        }

        private static Attribute MakeColor(ConsoleColor f, ConsoleColor b)
        {
            // Encode the colors into the int value.
            return new() {value = (((int) f & 0xffff) << 16) | ((int) b & 0xffff)};
        }

        public override void Init(Action terminalResized)
        {
            Colors.TopLevel = new ColorScheme();
            Colors.Base = new ColorScheme();
            Colors.Dialog = new ColorScheme();
            Colors.Menu = new ColorScheme();
            Colors.Error = new ColorScheme();
            Clip = new Rect(0, 0, Cols, Rows);

            Colors.TopLevel.Normal = MakeColor(ConsoleColor.Green, ConsoleColor.Black);
            Colors.TopLevel.Focus = MakeColor(ConsoleColor.White, ConsoleColor.DarkCyan);
            Colors.TopLevel.HotNormal = MakeColor(ConsoleColor.DarkYellow, ConsoleColor.Black);
            Colors.TopLevel.HotFocus = MakeColor(ConsoleColor.DarkBlue, ConsoleColor.DarkCyan);

            Colors.Base.Normal = MakeColor(ConsoleColor.White, ConsoleColor.Blue);
            Colors.Base.Focus = MakeColor(ConsoleColor.Black, ConsoleColor.Cyan);
            Colors.Base.HotNormal = MakeColor(ConsoleColor.Yellow, ConsoleColor.Blue);
            Colors.Base.HotFocus = MakeColor(ConsoleColor.Yellow, ConsoleColor.Cyan);

            // Focused,
            //    Selected, Hot: Yellow on Black
            //    Selected, text: white on black
            //    Unselected, hot: yellow on cyan
            //    unselected, text: same as unfocused
            Colors.Menu.HotFocus = MakeColor(ConsoleColor.Yellow, ConsoleColor.Black);
            Colors.Menu.Focus = MakeColor(ConsoleColor.White, ConsoleColor.Black);
            Colors.Menu.HotNormal = MakeColor(ConsoleColor.Yellow, ConsoleColor.Cyan);
            Colors.Menu.Normal = MakeColor(ConsoleColor.White, ConsoleColor.Cyan);
            Colors.Menu.Disabled = MakeColor(ConsoleColor.DarkGray, ConsoleColor.Cyan);

            Colors.Dialog.Normal = MakeColor(ConsoleColor.Black, ConsoleColor.Gray);
            Colors.Dialog.Focus = MakeColor(ConsoleColor.Black, ConsoleColor.Cyan);
            Colors.Dialog.HotNormal = MakeColor(ConsoleColor.Blue, ConsoleColor.Gray);
            Colors.Dialog.HotFocus = MakeColor(ConsoleColor.Blue, ConsoleColor.Cyan);

            Colors.Error.Normal = MakeColor(ConsoleColor.White, ConsoleColor.Red);
            Colors.Error.Focus = MakeColor(ConsoleColor.Black, ConsoleColor.Gray);
            Colors.Error.HotNormal = MakeColor(ConsoleColor.Yellow, ConsoleColor.Red);
            Colors.Error.HotFocus = Colors.Error.HotNormal;

            //MockConsole.Clear ();
        }

        public override Attribute MakeAttribute(Color fore, Color back)
        {
            return MakeColor((ConsoleColor) fore, (ConsoleColor) back);
        }

        private int redrawColor = -1;

        private void SetColor(int color)
        {
            redrawColor = color;
            var values = Enum.GetValues(typeof(ConsoleColor))
                .OfType<ConsoleColor>()
                .Select(s => (int) s);
            if (values.Contains(color & 0xffff)) FakeConsole.BackgroundColor = (ConsoleColor) (color & 0xffff);
            if (values.Contains((color >> 16) & 0xffff))
                FakeConsole.ForegroundColor = (ConsoleColor) ((color >> 16) & 0xffff);
        }

        public override void UpdateScreen()
        {
            var rows = Rows;
            var cols = Cols;

            FakeConsole.CursorTop = 0;
            FakeConsole.CursorLeft = 0;
            for (var row = 0; row < rows; row++)
            {
                dirtyLine[row] = false;
                for (var col = 0; col < cols; col++)
                {
                    contents[row, col, 2] = 0;
                    var color = contents[row, col, 1];
                    if (color != redrawColor)
                        SetColor(color);
                    FakeConsole.Write((char) contents[row, col, 0]);
                }
            }
        }

        public override void Refresh()
        {
            var rows = Rows;
            var cols = Cols;

            var savedRow = FakeConsole.CursorTop;
            var savedCol = FakeConsole.CursorLeft;
            for (var row = 0; row < rows; row++)
            {
                if (!dirtyLine[row])
                    continue;
                dirtyLine[row] = false;
                for (var col = 0; col < cols; col++)
                {
                    if (contents[row, col, 2] != 1)
                        continue;

                    FakeConsole.CursorTop = row;
                    FakeConsole.CursorLeft = col;
                    for (; col < cols && contents[row, col, 2] == 1; col++)
                    {
                        var color = contents[row, col, 1];
                        if (color != redrawColor)
                            SetColor(color);

                        FakeConsole.Write((char) contents[row, col, 0]);
                        contents[row, col, 2] = 0;
                    }
                }
            }

            FakeConsole.CursorTop = savedRow;
            FakeConsole.CursorLeft = savedCol;
        }

        public override void UpdateCursor()
        {
            //
        }

        public override void StartReportingMouseMoves()
        {
        }

        public override void StopReportingMouseMoves()
        {
        }

        public override void Suspend()
        {
        }

        private int currentAttribute;

        public override void SetAttribute(Attribute c)
        {
            currentAttribute = c.value;
        }

        private Key MapKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    return MapKeyModifiers(keyInfo, Key.Esc);
                case ConsoleKey.Tab:
                    return keyInfo.Modifiers == ConsoleModifiers.Shift ? Key.BackTab : Key.Tab;
                case ConsoleKey.Home:
                    return MapKeyModifiers(keyInfo, Key.Home);
                case ConsoleKey.End:
                    return MapKeyModifiers(keyInfo, Key.End);
                case ConsoleKey.LeftArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorLeft);
                case ConsoleKey.RightArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorRight);
                case ConsoleKey.UpArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorUp);
                case ConsoleKey.DownArrow:
                    return MapKeyModifiers(keyInfo, Key.CursorDown);
                case ConsoleKey.PageUp:
                    return MapKeyModifiers(keyInfo, Key.PageUp);
                case ConsoleKey.PageDown:
                    return MapKeyModifiers(keyInfo, Key.PageDown);
                case ConsoleKey.Enter:
                    return MapKeyModifiers(keyInfo, Key.Enter);
                case ConsoleKey.Spacebar:
                    return MapKeyModifiers(keyInfo, Key.Space);
                case ConsoleKey.Backspace:
                    return MapKeyModifiers(keyInfo, Key.Backspace);
                case ConsoleKey.Delete:
                    return MapKeyModifiers(keyInfo, Key.DeleteChar);
                case ConsoleKey.Insert:
                    return MapKeyModifiers(keyInfo, Key.InsertChar);

                case ConsoleKey.Oem1:
                case ConsoleKey.Oem2:
                case ConsoleKey.Oem3:
                case ConsoleKey.Oem4:
                case ConsoleKey.Oem5:
                case ConsoleKey.Oem6:
                case ConsoleKey.Oem7:
                case ConsoleKey.Oem8:
                case ConsoleKey.Oem102:
                case ConsoleKey.OemPeriod:
                case ConsoleKey.OemComma:
                case ConsoleKey.OemPlus:
                case ConsoleKey.OemMinus:
                    if (keyInfo.KeyChar == 0)
                        return Key.Unknown;

                    return (Key) keyInfo.KeyChar;
            }

            var key = keyInfo.Key;
            if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
            {
                var delta = key - ConsoleKey.A;
                if (keyInfo.Modifiers == ConsoleModifiers.Control)
                    return (Key) ((uint) Key.CtrlMask | ((uint) Key.A + delta));
                if (keyInfo.Modifiers == ConsoleModifiers.Alt)
                    return (Key) ((uint) Key.AltMask | ((uint) Key.A + delta));
                if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                {
                    if (keyInfo.KeyChar == 0)
                        return (Key) ((uint) Key.AltMask | (uint) Key.CtrlMask | ((uint) Key.A + delta));
                    return (Key) keyInfo.KeyChar;
                }

                return (Key) keyInfo.KeyChar;
            }

            if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
            {
                var delta = key - ConsoleKey.D0;
                if (keyInfo.Modifiers == ConsoleModifiers.Alt)
                    return (Key) ((uint) Key.AltMask | ((uint) Key.D0 + delta));
                if (keyInfo.Modifiers == ConsoleModifiers.Control)
                    return (Key) ((uint) Key.CtrlMask | ((uint) Key.D0 + delta));
                if (keyInfo.KeyChar == 0 || keyInfo.KeyChar == 30)
                    return MapKeyModifiers(keyInfo, (Key) ((uint) Key.D0 + delta));
                return (Key) keyInfo.KeyChar;
            }

            if (key >= ConsoleKey.F1 && key <= ConsoleKey.F12)
            {
                var delta = key - ConsoleKey.F1;
                if ((keyInfo.Modifiers & (ConsoleModifiers.Shift | ConsoleModifiers.Alt | ConsoleModifiers.Control)) !=
                    0) return MapKeyModifiers(keyInfo, (Key) ((uint) Key.F1 + delta));

                return (Key) ((uint) Key.F1 + delta);
            }

            return (Key) 0xffffffff;
        }

        private KeyModifiers keyModifiers;

        private Key MapKeyModifiers(ConsoleKeyInfo keyInfo, Key key)
        {
            var keyMod = new Key();
            if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
                keyMod = Key.ShiftMask;
            if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                keyMod |= Key.CtrlMask;
            if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
                keyMod |= Key.AltMask;

            return keyMod != Key.Null ? keyMod | key : key;
        }

        public override void PrepareToRun(MainLoop mainLoop, Action<KeyEvent> keyHandler,
            Action<KeyEvent> keyDownHandler, Action<KeyEvent> keyUpHandler, Action<MouseEvent> mouseHandler)
        {
            // Note: Net doesn't support keydown/up events and thus any passed keyDown/UpHandlers will never be called
            (mainLoop.Driver as FakeMainLoop).KeyPressed = delegate(ConsoleKeyInfo consoleKey)
            {
                var map = MapKey(consoleKey);
                if (map == (Key) 0xffffffff)
                    return;

                if (keyModifiers == null)
                    keyModifiers = new KeyModifiers();
                switch (consoleKey.Modifiers)
                {
                    case ConsoleModifiers.Alt:
                        keyModifiers.Alt = true;
                        break;
                    case ConsoleModifiers.Shift:
                        keyModifiers.Shift = true;
                        break;
                    case ConsoleModifiers.Control:
                        keyModifiers.Ctrl = true;
                        break;
                }

                keyHandler(new KeyEvent(map, keyModifiers));
                keyUpHandler(new KeyEvent(map, keyModifiers));
            };
        }

        public override void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            throw new NotImplementedException();
        }

        public override void SetColors(short foregroundColorId, short backgroundColorId)
        {
            throw new NotImplementedException();
        }

        public override void CookMouse()
        {
        }

        public override void UncookMouse()
        {
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}