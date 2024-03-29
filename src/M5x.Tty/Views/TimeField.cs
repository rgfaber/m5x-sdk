﻿//
// TimeField.cs: text entry for time
//
// Author: Jörg Preiß
//
// Licensed under the MIT license

using System;
using System.Globalization;
using System.Linq;
using M5x.Tty.Core;
using NStack;

namespace M5x.Tty.Views
{
    /// <summary>
    ///     Time editing <see cref="View" />
    /// </summary>
    /// <remarks>
    ///     The <see cref="TimeField" /> <see cref="View" /> provides time editing functionality with mouse support.
    /// </remarks>
    public class TimeField : TextField
    {
        private readonly int longFieldLen = 8;
        private readonly int shortFieldLen = 5;
        private bool isShort;
        private string longFormat;
        private string sepChar;
        private string shortFormat;
        private TimeSpan time;

        /// <summary>
        ///     Initializes a new instance of <see cref="TimeField" /> using <see cref="LayoutStyle.Absolute" /> positioning.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="time">Initial time.</param>
        /// <param name="isShort">If true, the seconds are hidden. Sets the <see cref="IsShortFormat" /> property.</param>
        public TimeField(int x, int y, TimeSpan time, bool isShort = false) : base(x, y, isShort ? 7 : 10, "")
        {
            this.isShort = isShort;
            Initialize(time);
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="TimeField" /> using <see cref="LayoutStyle.Computed" /> positioning.
        /// </summary>
        /// <param name="time">Initial time</param>
        public TimeField(TimeSpan time) : base(string.Empty)
        {
            isShort = true;
            Width = FieldLen + 2;
            Initialize(time);
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="TimeField" /> using <see cref="LayoutStyle.Computed" /> positioning.
        /// </summary>
        public TimeField() : this(TimeSpan.MinValue)
        {
        }

        private int FieldLen => isShort ? shortFieldLen : longFieldLen;
        private string Format => isShort ? shortFormat : longFormat;

        /// <summary>
        ///     Gets or sets the time of the <see cref="TimeField" />.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public TimeSpan Time
        {
            get => time;
            set
            {
                if (ReadOnly)
                    return;

                var oldTime = time;
                time = value;
                Text = " " + value.ToString(Format.Trim());
                var args = new DateTimeEventArgs<TimeSpan>(oldTime, value, Format);
                if (oldTime != value) OnTimeChanged(args);
            }
        }

        /// <summary>
        ///     Get or sets whether <see cref="TimeField" /> uses the short or long time format.
        /// </summary>
        public bool IsShortFormat
        {
            get => isShort;
            set
            {
                isShort = value;
                if (isShort)
                    Width = 7;
                else
                    Width = 10;
                var ro = ReadOnly;
                if (ro)
                    ReadOnly = false;
                SetText(Text);
                ReadOnly = ro;
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     TimeChanged event, raised when the Date has changed.
        /// </summary>
        /// <remarks>
        ///     This event is raised when the <see cref="Time" /> changes.
        /// </remarks>
        /// <remarks>
        ///     The passed <see cref="EventArgs" /> is a <see cref="DateTimeEventArgs{T}" /> containing the old value, new value,
        ///     and format string.
        /// </remarks>
        public event Action<DateTimeEventArgs<TimeSpan>> TimeChanged;

        private void Initialize(TimeSpan time)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            sepChar = cultureInfo.DateTimeFormat.TimeSeparator;
            longFormat = $" hh\\{sepChar}mm\\{sepChar}ss";
            shortFormat = $" hh\\{sepChar}mm";
            CursorPosition = 1;
            Time = time;
            TextChanged += TextField_TextChanged;
        }

        private void TextField_TextChanged(ustring e)
        {
            try
            {
                if (!TimeSpan.TryParseExact(Text.ToString().Trim(), Format.Trim(), CultureInfo.CurrentCulture,
                    TimeSpanStyles.None, out var result))
                    Text = e;
            }
            catch (Exception)
            {
                Text = e;
            }
        }

        private bool SetText(Rune key)
        {
            var text = TextModel.ToRunes(Text);
            var newText = text.GetRange(0, CursorPosition);
            newText.Add(key);
            if (CursorPosition < FieldLen)
                newText = newText.Concat(text.GetRange(CursorPosition + 1, text.Count - (CursorPosition + 1))).ToList();
            return SetText(ustring.Make(newText));
        }

        private bool SetText(ustring text)
        {
            if (text.IsEmpty) return false;

            var vals = text.Split(ustring.Make(sepChar));
            var isValidTime = true;
            var hour = int.Parse(vals[0].ToString());
            var minute = int.Parse(vals[1].ToString());
            var second = isShort ? 0 : vals.Length > 2 ? int.Parse(vals[2].ToString()) : 0;
            if (hour < 0)
            {
                isValidTime = false;
                hour = 0;
                vals[0] = "0";
            }
            else if (hour > 23)
            {
                isValidTime = false;
                hour = 23;
                vals[0] = "23";
            }

            if (minute < 0)
            {
                isValidTime = false;
                minute = 0;
                vals[1] = "0";
            }
            else if (minute > 59)
            {
                isValidTime = false;
                minute = 59;
                vals[1] = "59";
            }

            if (second < 0)
            {
                isValidTime = false;
                second = 0;
                vals[2] = "0";
            }
            else if (second > 59)
            {
                isValidTime = false;
                second = 59;
                vals[2] = "59";
            }

            var t = isShort
                ? $" {hour,2:00}{sepChar}{minute,2:00}"
                : $" {hour,2:00}{sepChar}{minute,2:00}{sepChar}{second,2:00}";

            if (!TimeSpan.TryParseExact(t.Trim(), Format.Trim(), CultureInfo.CurrentCulture, TimeSpanStyles.None,
                    out var result) ||
                !isValidTime)
                return false;
            Time = result;
            return true;
        }

        private void IncCursorPosition()
        {
            if (CursorPosition == FieldLen)
                return;
            if (Text[++CursorPosition] == sepChar.ToCharArray()[0])
                CursorPosition++;
        }

        private void DecCursorPosition()
        {
            if (CursorPosition == 1)
                return;
            if (Text[--CursorPosition] == sepChar.ToCharArray()[0])
                CursorPosition--;
        }

        private void AdjCursorPosition()
        {
            if (Text[CursorPosition] == sepChar.ToCharArray()[0])
                CursorPosition++;
        }

        /// <inheritdoc />
        public override bool ProcessKey(KeyEvent kb)
        {
            switch (kb.Key)
            {
                case Key.DeleteChar:
                case Key.D | Key.CtrlMask:
                    if (ReadOnly)
                        return true;

                    SetText('0');
                    break;

                case Key.Delete:
                case Key.Backspace:
                    if (ReadOnly)
                        return true;

                    SetText('0');
                    DecCursorPosition();
                    break;

                // Home, C-A
                case Key.Home:
                case Key.A | Key.CtrlMask:
                    CursorPosition = 1;
                    break;

                case Key.CursorLeft:
                case Key.B | Key.CtrlMask:
                    DecCursorPosition();
                    break;

                case Key.End:
                case Key.E | Key.CtrlMask: // End
                    CursorPosition = FieldLen;
                    break;

                case Key.CursorRight:
                case Key.F | Key.CtrlMask:
                    IncCursorPosition();
                    break;

                default:
                    // Ignore non-numeric characters.
                    if (kb.Key < (Key) (int) Key.D0 || kb.Key > (Key) (int) Key.D9)
                        return false;

                    if (ReadOnly)
                        return true;

                    if (SetText(TextModel.ToRunes(ustring.Make((uint) kb.Key)).First()))
                        IncCursorPosition();
                    return true;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool MouseEvent(MouseEvent ev)
        {
            if (!ev.Flags.HasFlag(MouseFlags.Button1Clicked))
                return false;
            if (!HasFocus)
                SetFocus();

            var point = ev.X;
            if (point > FieldLen)
                point = FieldLen;
            if (point < 1)
                point = 1;
            CursorPosition = point;
            AdjCursorPosition();
            return true;
        }

        /// <summary>
        ///     Event firing method that invokes the <see cref="TimeChanged" /> event.
        /// </summary>
        /// <param name="args">The event arguments</param>
        public virtual void OnTimeChanged(DateTimeEventArgs<TimeSpan> args)
        {
            TimeChanged?.Invoke(args);
        }
    }
}