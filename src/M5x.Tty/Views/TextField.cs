﻿//
// TextField.cs: single-line text editor with Emacs keybindings
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//

using System;
using System.Collections.Generic;
using System.Linq;
using M5x.Tty.Core;
using M5x.Tty.Types;
using NStack;
using Attribute = M5x.Tty.Core.Attribute;

namespace M5x.Tty.Views
{
    /// <summary>
    ///     Single-line text entry <see cref="View" />
    /// </summary>
    /// <remarks>
    ///     The <see cref="TextField" /> <see cref="View" /> provides editing functionality and mouse support.
    /// </remarks>
    public class TextField : View
    {
        private int first, point;

        private List<ustring> historyText;
        private int idxhistoryText;
        private bool isButtonPressed;
        private bool isButtonReleased = true;
        private bool isFromHistory;

        private int start, length;
        private List<Rune> text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextField" /> class using <see cref="LayoutStyle.Computed" />
        ///     positioning.
        /// </summary>
        /// <param name="text">Initial text contents.</param>
        public TextField(string text) : this(ustring.Make(text))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextField" /> class using <see cref="LayoutStyle.Computed" />
        ///     positioning.
        /// </summary>
        public TextField() : this(string.Empty)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextField" /> class using <see cref="LayoutStyle.Computed" />
        ///     positioning.
        /// </summary>
        /// <param name="text">Initial text contents.</param>
        public TextField(ustring text) : base(text)
        {
            Initialize(text, text.RuneCount + 1);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextField" /> class using <see cref="LayoutStyle.Absolute" />
        ///     positioning.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="w">The width.</param>
        /// <param name="text">Initial text contents.</param>
        public TextField(int x, int y, int w, ustring text) : base(new Rect(x, y, w, 1))
        {
            Initialize(text, w);
        }

        /// <summary>
        ///     Tracks whether the text field should be considered "used", that is, that the user has moved in the entry, so new
        ///     input should be appended at the cursor position, rather than clearing the entry
        /// </summary>
        public bool Used { get; set; }

        /// <summary>
        ///     If set to true its not allow any changes in the text.
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <inheritdoc />
        public override Rect Frame
        {
            get => base.Frame;
            set
            {
                base.Frame = value;
                var w = base.Frame.Width;
                first = point > w ? point - w : 0;
                Adjust();
            }
        }

        /// <summary>
        ///     Sets or gets the text held by the view.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public new ustring Text
        {
            get => ustring.Make(text);

            set
            {
                var oldText = ustring.Make(text);

                if (oldText == value)
                    return;

                var newText = OnTextChanging(value);
                if (newText.Cancel) return;
                text = TextModel.ToRunes(newText.NewText);
                if (!Secret && !isFromHistory)
                {
                    if (historyText == null)
                        historyText = new List<ustring> {oldText};
                    if (idxhistoryText > 0 && idxhistoryText + 1 < historyText.Count)
                        historyText.RemoveRange(idxhistoryText + 1, historyText.Count - idxhistoryText - 1);
                    historyText.Add(ustring.Make(text));
                    idxhistoryText++;
                }

                TextChanged?.Invoke(oldText);

                if (point > text.Count) point = Math.Max(DisplaySize(text, 0).size - 1, 0);

                Adjust();
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     Sets the secret property.
        /// </summary>
        /// <remarks>
        ///     This makes the text entry suitable for entering passwords.
        /// </remarks>
        public bool Secret { get; set; }

        /// <summary>
        ///     Sets or gets the current cursor position.
        /// </summary>
        public int CursorPosition
        {
            get => point;
            set
            {
                point = value;
                Adjust();
                SetNeedsDisplay();
            }
        }

        /// <inheritdoc />
        public override bool CanFocus
        {
            get => base.CanFocus;
            set => base.CanFocus = value;
        }

        /// <summary>
        ///     Start position of the selected text.
        /// </summary>
        public int SelectedStart { get; set; } = -1;

        /// <summary>
        ///     Length of the selected text.
        /// </summary>
        public int SelectedLength { get; set; }

        /// <summary>
        ///     The selected text.
        /// </summary>
        public ustring SelectedText { get; set; }

        /// <summary>
        ///     Changing event, raised before the <see cref="Text" /> changes and can be canceled or changing the new text.
        /// </summary>
        public event Action<TextChangingEventArgs> TextChanging;

        /// <summary>
        ///     Changed event, raised when the text has changed.
        /// </summary>
        /// <remarks>
        ///     This event is raised when the <see cref="Text" /> changes.
        /// </remarks>
        /// <remarks>
        ///     The passed <see cref="EventArgs" /> is a <see cref="ustring" /> containing the old value.
        /// </remarks>
        public event Action<ustring> TextChanged;

        private void Initialize(ustring text, int w)
        {
            Initialize();

            if (text == null)
                text = "";

            this.text = TextModel.ToRunes(text);
            point = text.RuneCount;
            first = point > w ? point - w : 0;
            CanFocus = true;
            Used = true;
            WantMousePositionReports = true;
        }

        private void Initialize()
        {
            Height = 1;
        }

        /// <inheritdoc />
        public override bool OnLeave(View view)
        {
            if (Application.mouseGrabView != null && Application.mouseGrabView == this)
                Application.UngrabMouse();
            if (SelectedLength != 0 && !(Application.mouseGrabView is MenuBar))
                ClearAllSelection();

            return base.OnLeave(view);
        }

        /// <summary>
        ///     Sets the cursor position.
        /// </summary>
        public override void PositionCursor()
        {
            var col = 0;
            for (var idx = first < 0 ? 0 : first; idx < text.Count; idx++)
            {
                if (idx == point)
                    break;
                var cols = Rune.ColumnWidth(text[idx]);
                col = SetCol(col, Frame.Width - 1, cols);
            }

            Move(col, 0);
        }

        /// <inheritdoc />
        public override void Redraw(Rect bounds)
        {
            var color = Colors.Menu;
            SetSelectedStartSelectedLength();

            Driver.SetAttribute(ColorScheme.Focus);
            Move(0, 0);

            var p = first;
            var col = 0;
            var width = Frame.Width;
            var tcount = text.Count;
            var roc = new Attribute(Color.DarkGray, Color.Gray);
            for (var idx = p; idx < tcount; idx++)
            {
                var rune = text[idx];
                var cols = Rune.ColumnWidth(rune);
                if (idx == point && HasFocus && !Used && SelectedLength == 0 && !ReadOnly)
                    Driver.SetAttribute(Colors.Menu.HotFocus);
                else if (ReadOnly)
                    Driver.SetAttribute(idx >= start && length > 0 && idx < start + length ? color.Focus : roc);
                else
                    Driver.SetAttribute(idx >= start && length > 0 && idx < start + length
                        ? color.Focus
                        : ColorScheme.Focus);
                if (col + cols <= width) Driver.AddRune(Secret ? '*' : rune);
                col = SetCol(col, width, cols);
                if (idx + 1 < tcount && col + Rune.ColumnWidth(text[idx + 1]) > width) break;
            }

            Driver.SetAttribute(ColorScheme.Focus);
            for (var i = col; i < width; i++) Driver.AddRune(' ');

            PositionCursor();
        }

        private static int SetCol(int col, int width, int cols)
        {
            if (col + cols <= width) col += cols;

            return col;
        }

        // Returns the size and length in a range of the string.
        private (int size, int length) DisplaySize(List<Rune> t, int start = -1, int end = -1,
            bool checkNextRune = true)
        {
            if (t == null || t.Count == 0) return (0, 0);
            var size = 0;
            var len = 0;
            var tcount = end == -1 ? t.Count : end > t.Count ? t.Count : end;
            var i = start == -1 ? 0 : start;
            for (; i < tcount; i++)
            {
                var rune = t[i];
                size += Rune.ColumnWidth(rune);
                len += Rune.RuneLen(rune);
                if (checkNextRune && i == tcount - 1 && t.Count > tcount && Rune.ColumnWidth(t[i + 1]) > 1)
                {
                    size += Rune.ColumnWidth(t[i + 1]);
                    len += Rune.RuneLen(t[i + 1]);
                }
            }

            return (size, len);
        }

        private void Adjust()
        {
            var offB = OffSetBackground();
            if (point < first)
                first = point;
            else if (first + point - (Frame.Width + offB) == 0 ||
                     DisplaySize(text, first, point).size >= Frame.Width + offB)
                first = Math.Max(CalculateFirst(text, first, point, Frame.Width - 1 + offB), 0);
            SetNeedsDisplay();
        }

        private int OffSetBackground()
        {
            var offB = 0;
            if (SuperView?.Frame.Right - Frame.Right < 0) offB = SuperView.Frame.Right - Frame.Right - 1;

            return offB;
        }

        private int CalculateFirst(List<Rune> t, int start, int end, int width)
        {
            if (t == null) return 0;
            var (dSize, _) = DisplaySize(t, start, end);
            if (dSize < width) return start;
            var size = 0;
            var tcount = end > t.Count - 1 ? t.Count - 1 : end;
            var col = 0;
            for (var i = tcount; i > start; i--)
            {
                var rune = t[i];
                var s = Rune.ColumnWidth(rune);
                size += s;
                if (size >= dSize - width)
                {
                    col = tcount - i + start;
                    if (start == 0 || col == start || point == t.Count && point - col > width) col++;
                    break;
                }
            }

            return col;
        }

        private void SetText(List<Rune> newText)
        {
            Text = ustring.Make(newText);
        }

        private void SetText(IEnumerable<Rune> newText)
        {
            SetText(newText.ToList());
        }

        private void SetClipboard(IEnumerable<Rune> text)
        {
            if (!Secret)
                Clipboard.Contents = ustring.Make(text.ToList());
        }

        /// <summary>
        ///     Processes key presses for the <see cref="TextField" />.
        /// </summary>
        /// <param name="kb"></param>
        /// <returns></returns>
        /// <remarks>
        ///     The <see cref="TextField" /> control responds to the following keys:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Keys</term>
        ///             <description>Function</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="Key.Delete" />, <see cref="Key.Backspace" /></term>
        ///             <description>Deletes the character before cursor.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public override bool ProcessKey(KeyEvent kb)
        {
            // remember current cursor position
            // because the new calculated cursor position is needed to be set BEFORE the change event is triggest
            // Needed for the Elmish Wrapper issue https://github.com/DieselMeister/Terminal.Gui.Elmish/issues/2
            var oldCursorPos = point;

            switch (ShortcutHelper.GetModifiersKey(kb))
            {
                case Key.Delete:
                case Key.DeleteChar:
                case Key.D | Key.CtrlMask:
                    if (ReadOnly)
                        return true;

                    if (SelectedLength == 0)
                    {
                        if (text.Count == 0 || text.Count == point)
                            return true;

                        SetText(text.GetRange(0, point).Concat(text.GetRange(point + 1, text.Count - (point + 1))));
                        Adjust();
                    }
                    else
                    {
                        DeleteSelectedText();
                    }

                    break;

                case Key.Backspace:
                    if (ReadOnly)
                        return true;

                    if (SelectedLength == 0)
                    {
                        if (point == 0)
                            return true;

                        point--;
                        SetText(text.GetRange(0, oldCursorPos - 1)
                            .Concat(text.GetRange(oldCursorPos, text.Count - oldCursorPos)));
                        Adjust();
                    }
                    else
                    {
                        DeleteSelectedText();
                    }

                    break;

                case Key.Home | Key.ShiftMask:
                case Key.Home | Key.ShiftMask | Key.CtrlMask:
                case Key.A | Key.ShiftMask | Key.CtrlMask:
                    if (point > 0)
                    {
                        var x = point;
                        point = 0;
                        PrepareSelection(x, point - x);
                    }

                    break;

                case Key.End | Key.ShiftMask:
                case Key.End | Key.ShiftMask | Key.CtrlMask:
                case Key.E | Key.ShiftMask | Key.CtrlMask:
                    if (point <= text.Count)
                    {
                        var x = point;
                        point = text.Count;
                        PrepareSelection(x, point - x);
                    }

                    break;

                // Home, C-A
                case Key.Home:
                case Key.Home | Key.CtrlMask:
                case Key.A | Key.CtrlMask:
                    ClearAllSelection();
                    point = 0;
                    Adjust();
                    break;

                case Key.CursorLeft | Key.ShiftMask:
                case Key.CursorUp | Key.ShiftMask:
                    if (point > 0) PrepareSelection(point--, -1);
                    break;

                case Key.CursorRight | Key.ShiftMask:
                case Key.CursorDown | Key.ShiftMask:
                    if (point < text.Count) PrepareSelection(point++, 1);
                    break;

                case Key.CursorLeft | Key.ShiftMask | Key.CtrlMask:
                case Key.CursorUp | Key.ShiftMask | Key.CtrlMask:
                    if (point > 0)
                    {
                        var x = start > -1 && start > point ? start : point;
                        if (x > 0)
                        {
                            var sbw = WordBackward(x);
                            if (sbw != -1)
                                point = sbw;
                            PrepareSelection(x, sbw - x);
                        }
                    }

                    break;

                case Key.CursorRight | Key.ShiftMask | Key.CtrlMask:
                case Key.CursorDown | Key.ShiftMask | Key.CtrlMask:
                    if (point < text.Count)
                    {
                        var x = start > -1 && start > point ? start : point;
                        var sfw = WordForward(x);
                        if (sfw != -1)
                            point = sfw;
                        PrepareSelection(x, sfw - x);
                    }

                    break;

                case Key.CursorLeft:
                case Key.B | Key.CtrlMask:
                    ClearAllSelection();
                    if (point > 0)
                    {
                        point--;
                        Adjust();
                    }

                    break;

                case Key.End:
                case Key.End | Key.CtrlMask:
                case Key.E | Key.CtrlMask: // End
                    ClearAllSelection();
                    point = text.Count;
                    Adjust();
                    break;

                case Key.CursorRight:
                case Key.F | Key.CtrlMask:
                    ClearAllSelection();
                    if (point == text.Count)
                        break;
                    point++;
                    Adjust();
                    break;

                case Key.K | Key.CtrlMask: // kill-to-end
                    if (ReadOnly)
                        return true;

                    ClearAllSelection();
                    if (point >= text.Count)
                        return true;
                    SetClipboard(text.GetRange(point, text.Count - point));
                    SetText(text.GetRange(0, point));
                    Adjust();
                    break;

                // Undo
                case Key.Z | Key.CtrlMask:
                    if (ReadOnly)
                        return true;

                    if (historyText != null && historyText.Count > 0)
                    {
                        isFromHistory = true;
                        if (idxhistoryText > 0)
                            idxhistoryText--;
                        if (idxhistoryText > -1)
                            Text = historyText[idxhistoryText];
                        point = text.Count;
                        isFromHistory = false;
                    }

                    break;

                //Redo
                case Key.Y | Key.CtrlMask: // Control-y, yank
                    if (ReadOnly)
                        return true;

                    if (historyText != null && historyText.Count > 0)
                    {
                        isFromHistory = true;
                        if (idxhistoryText < historyText.Count - 1)
                        {
                            idxhistoryText++;
                            if (idxhistoryText < historyText.Count)
                                Text = historyText[idxhistoryText];
                            else if (idxhistoryText == historyText.Count - 1) Text = historyText[historyText.Count - 1];
                            point = text.Count;
                        }

                        isFromHistory = false;
                    }

                    //if (Clipboard.Contents == null)
                    //	return true;
                    //var clip = TextModel.ToRunes (Clipboard.Contents);
                    //if (clip == null)
                    //	return true;

                    //if (point == text.Count) {
                    //	point = text.Count;
                    //	SetText(text.Concat(clip).ToList());
                    //} else {
                    //	point += clip.Count;
                    //	SetText(text.GetRange(0, oldCursorPos).Concat(clip).Concat(text.GetRange(oldCursorPos, text.Count - oldCursorPos)));
                    //}
                    //Adjust ();

                    break;

                case Key.CursorLeft | Key.CtrlMask:
                case Key.CursorUp | Key.CtrlMask:
                case 'b' + Key.AltMask:
                    ClearAllSelection();
                    var bw = WordBackward(point);
                    if (bw != -1)
                        point = bw;
                    Adjust();
                    break;

                case Key.CursorRight | Key.CtrlMask:
                case Key.CursorDown | Key.CtrlMask:
                case 'f' + Key.AltMask:
                    ClearAllSelection();
                    var fw = WordForward(point);
                    if (fw != -1)
                        point = fw;
                    Adjust();
                    break;

                case Key.InsertChar:
                    Used = !Used;
                    SetNeedsDisplay();
                    break;

                case Key.C | Key.CtrlMask:
                    Copy();
                    break;

                case Key.X | Key.CtrlMask:
                    if (ReadOnly)
                        return true;

                    Cut();
                    break;

                case Key.V | Key.CtrlMask:
                    Paste();
                    break;

                // MISSING:
                // Alt-D, Alt-backspace
                // Alt-Y
                // Delete adding to kill buffer

                default:
                    // Ignore other control characters.
                    if (kb.Key < Key.Space || kb.Key > Key.CharMask)
                        return false;

                    if (ReadOnly)
                        return true;

                    if (SelectedLength != 0)
                    {
                        DeleteSelectedText();
                        oldCursorPos = point;
                    }

                    var kbstr = TextModel.ToRunes(ustring.Make((uint) kb.Key));
                    if (Used)
                    {
                        point++;
                        if (point == text.Count + 1)
                        {
                            SetText(text.Concat(kbstr).ToList());
                        }
                        else
                        {
                            if (oldCursorPos > text.Count) oldCursorPos = text.Count;
                            SetText(text.GetRange(0, oldCursorPos).Concat(kbstr).Concat(text.GetRange(oldCursorPos,
                                Math.Min(text.Count - oldCursorPos, text.Count))));
                        }
                    }
                    else
                    {
                        SetText(text.GetRange(0, oldCursorPos).Concat(kbstr).Concat(
                            text.GetRange(Math.Min(oldCursorPos + 1, text.Count),
                                Math.Max(text.Count - oldCursorPos - 1, 0))));
                        point++;
                    }

                    Adjust();
                    return true;
            }

            return true;
        }

        private int WordForward(int p)
        {
            if (p >= text.Count)
                return -1;

            var i = p;
            if (Rune.IsPunctuation(text[p]) || Rune.IsWhiteSpace(text[p]))
            {
                for (; i < text.Count; i++)
                {
                    var r = text[i];
                    if (Rune.IsLetterOrDigit(r))
                        break;
                }

                for (; i < text.Count; i++)
                {
                    var r = text[i];
                    if (!Rune.IsLetterOrDigit(r))
                        break;
                }
            }
            else
            {
                for (; i < text.Count; i++)
                {
                    var r = text[i];
                    if (!Rune.IsLetterOrDigit(r))
                        break;
                }
            }

            if (i != p)
                return i;
            return -1;
        }

        private int WordBackward(int p)
        {
            if (p == 0)
                return -1;

            var i = p - 1;
            if (i == 0)
                return 0;

            var ti = text[i];
            if (Rune.IsPunctuation(ti) || Rune.IsSymbol(ti) || Rune.IsWhiteSpace(ti))
            {
                for (; i >= 0; i--)
                    if (Rune.IsLetterOrDigit(text[i]))
                        break;
                for (; i >= 0; i--)
                    if (!Rune.IsLetterOrDigit(text[i]))
                        break;
            }
            else
            {
                for (; i >= 0; i--)
                    if (!Rune.IsLetterOrDigit(text[i]))
                        break;
            }

            i++;

            if (i != p)
                return i;

            return -1;
        }

        /// <inheritdoc />
        public override bool MouseEvent(MouseEvent ev)
        {
            if (!ev.Flags.HasFlag(MouseFlags.Button1Pressed) && !ev.Flags.HasFlag(MouseFlags.ReportMousePosition) &&
                !ev.Flags.HasFlag(MouseFlags.Button1Released) && !ev.Flags.HasFlag(MouseFlags.Button1DoubleClicked) &&
                !ev.Flags.HasFlag(MouseFlags.Button1TripleClicked))
                return false;

            if (ev.Flags == MouseFlags.Button1Pressed)
            {
                if (!CanFocus) return true;
                if (!HasFocus) SetFocus();
                PositionCursor(ev);
                if (isButtonReleased) ClearAllSelection();
                isButtonReleased = true;
                isButtonPressed = true;
            }
            else if (ev.Flags == (MouseFlags.Button1Pressed | MouseFlags.ReportMousePosition) && isButtonPressed)
            {
                var x = PositionCursor(ev);
                isButtonReleased = false;
                PrepareSelection(x);
                if (Application.mouseGrabView == null) Application.GrabMouse(this);
            }
            else if (ev.Flags == MouseFlags.Button1Released)
            {
                isButtonReleased = true;
                isButtonPressed = false;
                Application.UngrabMouse();
            }
            else if (ev.Flags == MouseFlags.Button1DoubleClicked)
            {
                var x = PositionCursor(ev);
                var sbw = x;
                if (x > 0 && (char) Text[x - 1] != ' ') sbw = WordBackward(x);
                if (sbw != -1)
                {
                    x = sbw;
                    PositionCursor(x);
                }

                var sfw = WordForward(x);
                ClearAllSelection();
                PrepareSelection(sbw, sfw - sbw);
            }
            else if (ev.Flags == MouseFlags.Button1TripleClicked)
            {
                PositionCursor(0);
                ClearAllSelection();
                PrepareSelection(0, text.Count);
            }

            SetNeedsDisplay();
            return true;
        }

        private int PositionCursor(MouseEvent ev)
        {
            // We could also set the cursor position.
            int x;
            var pX = GetPointFromX(text, first, ev.X);
            if (text.Count == 0)
                x = pX - ev.OfX;
            else
                x = pX;
            return PositionCursor(x, false);
        }

        private int PositionCursor(int x, bool getX = true)
        {
            var pX = x;
            if (getX) pX = GetPointFromX(text, first, x);
            if (first + pX > text.Count)
                point = text.Count;
            else if (first + pX < first)
                point = 0;
            else
                point = first + pX;

            return point;
        }

        private int GetPointFromX(List<Rune> t, int start, int x)
        {
            if (x < 0) return x;
            var size = start;
            var pX = x + start;
            for (var i = start; i < t.Count; i++)
            {
                var r = t[i];
                size += Rune.ColumnWidth(r);
                if (i == pX || size > pX) return i - start;
            }

            return t.Count - start;
        }

        private void PrepareSelection(int x, int direction = 0)
        {
            x = x + first < 0 ? 0 : x;
            SelectedStart = SelectedStart == -1 && text.Count > 0 && x >= 0 && x <= text.Count ? x : SelectedStart;
            if (SelectedStart > -1)
            {
                SelectedLength = x + direction <= text.Count
                    ? x + direction - SelectedStart
                    : text.Count - SelectedStart;
                SetSelectedStartSelectedLength();
                if (start > -1)
                {
                    SelectedText = length > 0
                        ? ustring.Make(text).ToString().Substring(
                            start < 0 ? 0 : start, length > text.Count ? text.Count : length)
                        : "";
                    if (first > start) first = start;
                }
                else
                {
                    ClearAllSelection();
                }
            }

            Adjust();
        }

        /// <summary>
        ///     Clear the selected text.
        /// </summary>
        public void ClearAllSelection()
        {
            if (SelectedStart == -1 && SelectedLength == 0)
                return;
            SelectedStart = -1;
            SelectedLength = 0;
            SelectedText = "";
            start = 0;
            length = 0;
            SetNeedsDisplay();
        }

        private void SetSelectedStartSelectedLength()
        {
            if (SelectedLength < 0)
            {
                start = SelectedLength + SelectedStart;
                length = Math.Abs(SelectedLength);
            }
            else
            {
                start = SelectedStart;
                length = SelectedLength;
            }
        }

        /// <summary>
        ///     Copy the selected text to the clipboard.
        /// </summary>
        public virtual void Copy()
        {
            if (Secret)
                return;

            if (SelectedLength != 0) Clipboard.Contents = SelectedText;
        }

        /// <summary>
        ///     Cut the selected text to the clipboard.
        /// </summary>
        public virtual void Cut()
        {
            if (SelectedLength != 0)
            {
                Clipboard.Contents = SelectedText;
                DeleteSelectedText();
            }
        }

        private void DeleteSelectedText()
        {
            var actualText = Text;
            var selStart = SelectedLength < 0 ? SelectedLength + SelectedStart : SelectedStart;
            var selLength = Math.Abs(SelectedLength);
            var (_, len) = DisplaySize(text, 0, selStart, false);
            var (_, len2) = DisplaySize(text, selStart, selStart + selLength, false);
            var (_, len3) = DisplaySize(text, selStart + selLength, actualText.RuneCount, false);
            Text = actualText[0, len] +
                   actualText[len + len2, len + len2 + len3];
            ClearAllSelection();
            point = selStart >= Text.RuneCount ? Text.RuneCount : selStart;
            Adjust();
        }

        /// <summary>
        ///     Paste the selected text from the clipboard.
        /// </summary>
        public virtual void Paste()
        {
            if (ReadOnly) return;

            SetSelectedStartSelectedLength();
            var selStart = start == -1 ? CursorPosition : start;
            var actualText = Text;
            var (_, len) = DisplaySize(text, 0, selStart, false);
            var (_, len2) = DisplaySize(text, selStart, selStart + length, false);
            var (_, len3) = DisplaySize(text, selStart + length, actualText.RuneCount, false);
            var cbTxt = Clipboard.Contents ?? "";
            Text = actualText[0, len] +
                   cbTxt +
                   actualText[len + len2, len + len2 + len3];
            point = selStart + cbTxt.RuneCount;
            ClearAllSelection();
            SetNeedsDisplay();
        }

        /// <summary>
        ///     Virtual method that invoke the <see cref="TextChanging" /> event if it's defined.
        /// </summary>
        /// <param name="newText">The new text to be replaced.</param>
        /// <returns>Returns the <see cref="TextChangingEventArgs" /></returns>
        public virtual TextChangingEventArgs OnTextChanging(ustring newText)
        {
            var ev = new TextChangingEventArgs(newText);
            TextChanging?.Invoke(ev);
            return ev;
        }
    }

    /// <summary>
    ///     An <see cref="EventArgs" /> which allows passing a cancelable new text value event.
    /// </summary>
    public class TextChangingEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="TextChangingEventArgs" />
        /// </summary>
        /// <param name="newText">The new <see cref="TextField.Text" /> to be replaced.</param>
        public TextChangingEventArgs(ustring newText)
        {
            NewText = newText;
        }

        /// <summary>
        ///     The new text to be replaced.
        /// </summary>
        public ustring NewText { get; set; }

        /// <summary>
        ///     Flag which allows to cancel the new text value.
        /// </summary>
        public bool Cancel { get; set; }
    }
}