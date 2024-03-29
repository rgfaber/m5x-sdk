﻿//
// ConsoleDriver.cs: Definition for the Console Driver API
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Define this to enable diagnostics drawing for Window Frames

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using M5x.Tty.ConsoleDrivers;
using M5x.Tty.ConsoleDrivers.CursesDriver;
using M5x.Tty.Types;
using M5x.Tty.Views;
using NStack;

namespace M5x.Tty.Core
{
    /// <summary>
    ///     Basic colors that can be used to set the foreground and background colors in console applications.
    /// </summary>
    public enum Color
    {
        /// <summary>
        ///     The black color.
        /// </summary>
        Black,

        /// <summary>
        ///     The blue color.
        /// </summary>
        Blue,

        /// <summary>
        ///     The green color.
        /// </summary>
        Green,

        /// <summary>
        ///     The cyan color.
        /// </summary>
        Cyan,

        /// <summary>
        ///     The red color.
        /// </summary>
        Red,

        /// <summary>
        ///     The magenta color.
        /// </summary>
        Magenta,

        /// <summary>
        ///     The brown color.
        /// </summary>
        Brown,

        /// <summary>
        ///     The gray color.
        /// </summary>
        Gray,

        /// <summary>
        ///     The dark gray color.
        /// </summary>
        DarkGray,

        /// <summary>
        ///     The bright bBlue color.
        /// </summary>
        BrightBlue,

        /// <summary>
        ///     The bright green color.
        /// </summary>
        BrightGreen,

        /// <summary>
        ///     The brigh cyan color.
        /// </summary>
        BrighCyan,

        /// <summary>
        ///     The bright red color.
        /// </summary>
        BrightRed,

        /// <summary>
        ///     The bright magenta color.
        /// </summary>
        BrightMagenta,

        /// <summary>
        ///     The bright yellow color.
        /// </summary>
        BrightYellow,

        /// <summary>
        ///     The White color.
        /// </summary>
        White
    }

    /// <summary>
    ///     Attributes are used as elements that contain both a foreground and a background or platform specific features
    /// </summary>
    /// <remarks>
    ///     <see cref="Attribute" />s are needed to map colors to terminal capabilities that might lack colors, on color
    ///     scenarios, they encode both the foreground and the background color and are used in the <see cref="ColorScheme" />
    ///     class to define color schemes that can be used in your application.
    /// </remarks>
    public struct Attribute
    {
        internal int value;
        internal Color foreground;
        internal Color background;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Attribute" /> struct.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="foreground">Foreground</param>
        /// <param name="background">Background</param>
        public Attribute(int value, Color foreground = new(), Color background = new())
        {
            this.value = value;
            this.foreground = foreground;
            this.background = background;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Attribute" /> struct.
        /// </summary>
        /// <param name="foreground">Foreground</param>
        /// <param name="background">Background</param>
        public Attribute(Color foreground = new(), Color background = new())
        {
            value = value = (int) foreground | ((int) background << 4);
            this.foreground = foreground;
            this.background = background;
        }

        /// <summary>
        ///     Implicit conversion from an <see cref="Attribute" /> to the underlying Int32 representation
        /// </summary>
        /// <returns>The integer value stored in the attribute.</returns>
        /// <param name="c">The attribute to convert</param>
        public static implicit operator int(Attribute c)
        {
            return c.value;
        }

        /// <summary>
        ///     Implicitly convert an integer value into an <see cref="Attribute" />
        /// </summary>
        /// <returns>An attribute with the specified integer value.</returns>
        /// <param name="v">value</param>
        public static implicit operator Attribute(int v)
        {
            return new(v);
        }

        /// <summary>
        ///     Creates an <see cref="Attribute" /> from the specified foreground and background.
        /// </summary>
        /// <returns>The make.</returns>
        /// <param name="foreground">Foreground color to use.</param>
        /// <param name="background">Background color to use.</param>
        public static Attribute Make(Color foreground, Color background)
        {
            if (Application.Driver == null)
                throw new InvalidOperationException("The Application has not been initialized");
            return Application.Driver.MakeAttribute(foreground, background);
        }
    }

    /// <summary>
    ///     Color scheme definitions, they cover some common scenarios and are used
    ///     typically in containers such as <see cref="Window" /> and <see cref="FrameView" /> to set the scheme that is used
    ///     by all the
    ///     views contained inside.
    /// </summary>
    public class ColorScheme : IEquatable<ColorScheme>
    {
        private Attribute _disabled;
        private Attribute _focus;
        private Attribute _hotFocus;
        private Attribute _hotNormal;
        private Attribute _normal;
        internal string caller = "";

        private bool preparingScheme;

        /// <summary>
        ///     The default color for text, when the view is not focused.
        /// </summary>
        public Attribute Normal
        {
            get => _normal;
            set => _normal = SetAttribute(value);
        }

        /// <summary>
        ///     The color for text when the view has the focus.
        /// </summary>
        public Attribute Focus
        {
            get => _focus;
            set => _focus = SetAttribute(value);
        }

        /// <summary>
        ///     The color for the hotkey when a view is not focused
        /// </summary>
        public Attribute HotNormal
        {
            get => _hotNormal;
            set => _hotNormal = SetAttribute(value);
        }

        /// <summary>
        ///     The color for the hotkey when the view is focused.
        /// </summary>
        public Attribute HotFocus
        {
            get => _hotFocus;
            set => _hotFocus = SetAttribute(value);
        }

        /// <summary>
        ///     The default color for text, when the view is disabled.
        /// </summary>
        public Attribute Disabled
        {
            get => _disabled;
            set => _disabled = SetAttribute(value);
        }

        /// <summary>
        ///     Compares two <see cref="ColorScheme" /> objects for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if the two objects are equal</returns>
        public bool Equals(ColorScheme other)
        {
            return other != null &&
                   EqualityComparer<Attribute>.Default.Equals(_normal, other._normal) &&
                   EqualityComparer<Attribute>.Default.Equals(_focus, other._focus) &&
                   EqualityComparer<Attribute>.Default.Equals(_hotNormal, other._hotNormal) &&
                   EqualityComparer<Attribute>.Default.Equals(_hotFocus, other._hotFocus) &&
                   EqualityComparer<Attribute>.Default.Equals(_disabled, other._disabled);
        }

        private Attribute SetAttribute(Attribute attribute, [CallerMemberName] string callerMemberName = null)
        {
            if (!Application._initialized && !preparingScheme)
                return attribute;

            if (preparingScheme)
                return attribute;

            preparingScheme = true;
            switch (caller)
            {
                case "TopLevel":
                    switch (callerMemberName)
                    {
                        case "Normal":
                            HotNormal = Application.Driver.MakeAttribute(HotNormal.foreground, attribute.background);
                            break;
                        case "Focus":
                            HotFocus = Application.Driver.MakeAttribute(HotFocus.foreground, attribute.background);
                            break;
                        case "HotNormal":
                            HotFocus = Application.Driver.MakeAttribute(attribute.foreground, HotFocus.background);
                            break;
                        case "HotFocus":
                            HotNormal = Application.Driver.MakeAttribute(attribute.foreground, HotNormal.background);
                            if (Focus.foreground != attribute.background)
                                Focus = Application.Driver.MakeAttribute(Focus.foreground, attribute.background);
                            break;
                    }

                    break;

                case "Base":
                    switch (callerMemberName)
                    {
                        case "Normal":
                            HotNormal = Application.Driver.MakeAttribute(HotNormal.foreground, attribute.background);
                            break;
                        case "Focus":
                            HotFocus = Application.Driver.MakeAttribute(HotFocus.foreground, attribute.background);
                            break;
                        case "HotNormal":
                            HotFocus = Application.Driver.MakeAttribute(attribute.foreground, HotFocus.background);
                            Normal = Application.Driver.MakeAttribute(Normal.foreground, attribute.background);
                            break;
                        case "HotFocus":
                            HotNormal = Application.Driver.MakeAttribute(attribute.foreground, HotNormal.background);
                            if (Focus.foreground != attribute.background)
                                Focus = Application.Driver.MakeAttribute(Focus.foreground, attribute.background);
                            break;
                    }

                    break;

                case "Menu":
                    switch (callerMemberName)
                    {
                        case "Normal":
                            if (Focus.background != attribute.background)
                                Focus = Application.Driver.MakeAttribute(attribute.foreground, Focus.background);
                            HotNormal = Application.Driver.MakeAttribute(HotNormal.foreground, attribute.background);
                            Disabled = Application.Driver.MakeAttribute(Disabled.foreground, attribute.background);
                            break;
                        case "Focus":
                            Normal = Application.Driver.MakeAttribute(attribute.foreground, Normal.background);
                            HotFocus = Application.Driver.MakeAttribute(HotFocus.foreground, attribute.background);
                            break;
                        case "HotNormal":
                            if (Focus.background != attribute.background)
                                HotFocus = Application.Driver.MakeAttribute(attribute.foreground, HotFocus.background);
                            Normal = Application.Driver.MakeAttribute(Normal.foreground, attribute.background);
                            Disabled = Application.Driver.MakeAttribute(Disabled.foreground, attribute.background);
                            break;
                        case "HotFocus":
                            HotNormal = Application.Driver.MakeAttribute(attribute.foreground, HotNormal.background);
                            if (Focus.foreground != attribute.background)
                                Focus = Application.Driver.MakeAttribute(Focus.foreground, attribute.background);
                            break;
                        case "Disabled":
                            if (Focus.background != attribute.background)
                                HotFocus = Application.Driver.MakeAttribute(attribute.foreground, HotFocus.background);
                            Normal = Application.Driver.MakeAttribute(Normal.foreground, attribute.background);
                            HotNormal = Application.Driver.MakeAttribute(HotNormal.foreground, attribute.background);
                            break;
                    }

                    break;

                case "Dialog":
                    switch (callerMemberName)
                    {
                        case "Normal":
                            if (Focus.background != attribute.background)
                                Focus = Application.Driver.MakeAttribute(attribute.foreground, Focus.background);
                            HotNormal = Application.Driver.MakeAttribute(HotNormal.foreground, attribute.background);
                            break;
                        case "Focus":
                            Normal = Application.Driver.MakeAttribute(attribute.foreground, Normal.background);
                            HotFocus = Application.Driver.MakeAttribute(HotFocus.foreground, attribute.background);
                            break;
                        case "HotNormal":
                            if (Focus.background != attribute.background)
                                HotFocus = Application.Driver.MakeAttribute(attribute.foreground, HotFocus.background);
                            if (Normal.foreground != attribute.background)
                                Normal = Application.Driver.MakeAttribute(Normal.foreground, attribute.background);
                            break;
                        case "HotFocus":
                            HotNormal = Application.Driver.MakeAttribute(attribute.foreground, HotNormal.background);
                            if (Focus.foreground != attribute.background)
                                Focus = Application.Driver.MakeAttribute(Focus.foreground, attribute.background);
                            break;
                    }

                    break;

                case "Error":
                    switch (callerMemberName)
                    {
                        case "Normal":
                            HotNormal = Application.Driver.MakeAttribute(HotNormal.foreground, attribute.background);
                            HotFocus = Application.Driver.MakeAttribute(HotFocus.foreground, attribute.background);
                            break;
                        case "HotNormal":
                        case "HotFocus":
                            HotFocus = Application.Driver.MakeAttribute(attribute.foreground, attribute.background);
                            Normal = Application.Driver.MakeAttribute(Normal.foreground, attribute.background);
                            break;
                    }

                    break;
            }

            preparingScheme = false;
            return attribute;
        }

        /// <summary>
        ///     Compares two <see cref="ColorScheme" /> objects for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the two objects are equal</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ColorScheme);
        }

        /// <summary>
        ///     Returns a hashcode for this instance.
        /// </summary>
        /// <returns>hashcode for this instance</returns>
        public override int GetHashCode()
        {
            var hashCode = -1242460230;
            hashCode = hashCode * -1521134295 + _normal.GetHashCode();
            hashCode = hashCode * -1521134295 + _focus.GetHashCode();
            hashCode = hashCode * -1521134295 + _hotNormal.GetHashCode();
            hashCode = hashCode * -1521134295 + _hotFocus.GetHashCode();
            hashCode = hashCode * -1521134295 + _disabled.GetHashCode();
            return hashCode;
        }

        /// <summary>
        ///     Compares two <see cref="ColorScheme" /> objects for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns><c>true</c> if the two objects are equivalent</returns>
        public static bool operator ==(ColorScheme left, ColorScheme right)
        {
            return EqualityComparer<ColorScheme>.Default.Equals(left, right);
        }

        /// <summary>
        ///     Compares two <see cref="ColorScheme" /> objects for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns><c>true</c> if the two objects are not equivalent</returns>
        public static bool operator !=(ColorScheme left, ColorScheme right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    ///     The default <see cref="ColorScheme" />s for the application.
    /// </summary>
    public static class Colors
    {
        static Colors()
        {
            // Use reflection to dynamically create the default set of ColorSchemes from the list defiined 
            // by the class. 
            ColorSchemes = typeof(Colors).GetProperties()
                .Where(p => p.PropertyType == typeof(ColorScheme))
                .Select(p =>
                    new KeyValuePair<string, ColorScheme>(p.Name, new ColorScheme())) // (ColorScheme)p.GetValue (p)))
                .ToDictionary(t => t.Key, t => t.Value);
        }

        /// <summary>
        ///     The application toplevel color scheme, for the default toplevel views.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes" /> instead (e.g.
        ///         <c>edit.ColorScheme = Colors.ColorSchemes["TopLevel"];</c>
        ///     </para>
        /// </remarks>
        public static ColorScheme TopLevel
        {
            get => GetColorScheme();
            set => SetColorScheme(value);
        }

        /// <summary>
        ///     The base color scheme, for the default toplevel views.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes" /> instead (e.g.
        ///         <c>edit.ColorScheme = Colors.ColorSchemes["Base"];</c>
        ///     </para>
        /// </remarks>
        public static ColorScheme Base
        {
            get => GetColorScheme();
            set => SetColorScheme(value);
        }

        /// <summary>
        ///     The dialog color scheme, for standard popup dialog boxes
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes" /> instead (e.g.
        ///         <c>edit.ColorScheme = Colors.ColorSchemes["Dialog"];</c>
        ///     </para>
        /// </remarks>
        public static ColorScheme Dialog
        {
            get => GetColorScheme();
            set => SetColorScheme(value);
        }

        /// <summary>
        ///     The menu bar color
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes" /> instead (e.g.
        ///         <c>edit.ColorScheme = Colors.ColorSchemes["Menu"];</c>
        ///     </para>
        /// </remarks>
        public static ColorScheme Menu
        {
            get => GetColorScheme();
            set => SetColorScheme(value);
        }

        /// <summary>
        ///     The color scheme for showing errors.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This API will be deprecated in the future. Use <see cref="Colors.ColorSchemes" /> instead (e.g.
        ///         <c>edit.ColorScheme = Colors.ColorSchemes["Error"];</c>
        ///     </para>
        /// </remarks>
        public static ColorScheme Error
        {
            get => GetColorScheme();
            set => SetColorScheme(value);
        }

        /// <summary>
        ///     Provides the defined <see cref="ColorScheme" />s.
        /// </summary>
        public static Dictionary<string, ColorScheme> ColorSchemes { get; }

        private static ColorScheme GetColorScheme([CallerMemberName] string callerMemberName = null)
        {
            return ColorSchemes[callerMemberName];
        }

        private static void SetColorScheme(ColorScheme colorScheme, [CallerMemberName] string callerMemberName = null)
        {
            ColorSchemes[callerMemberName] = colorScheme;
            colorScheme.caller = callerMemberName;
        }
    }

    ///// <summary>
    ///// Special characters that can be drawn with 
    ///// </summary>
    //public enum SpecialChar {
    //	/// <summary>
    //	/// Horizontal line character.
    //	/// </summary>
    //	HLine,

    //	/// <summary>
    //	/// Vertical line character.
    //	/// </summary>
    //	VLine,

    //	/// <summary>
    //	/// Stipple pattern
    //	/// </summary>
    //	Stipple,

    //	/// <summary>
    //	/// Diamond character
    //	/// </summary>
    //	Diamond,

    //	/// <summary>
    //	/// Upper left corner
    //	/// </summary>
    //	ULCorner,

    //	/// <summary>
    //	/// Lower left corner
    //	/// </summary>
    //	LLCorner,

    //	/// <summary>
    //	/// Upper right corner
    //	/// </summary>
    //	URCorner,

    //	/// <summary>
    //	/// Lower right corner
    //	/// </summary>
    //	LRCorner,

    //	/// <summary>
    //	/// Left tee
    //	/// </summary>
    //	LeftTee,

    //	/// <summary>
    //	/// Right tee
    //	/// </summary>
    //	RightTee,

    //	/// <summary>
    //	/// Top tee
    //	/// </summary>
    //	TopTee,

    //	/// <summary>
    //	/// The bottom tee.
    //	/// </summary>
    //	BottomTee,
    //}

    /// <summary>
    ///     ConsoleDriver is an abstract class that defines the requirements for a console driver.
    ///     There are currently three implementations: <see cref="CursesDriver" /> (for Unix and Mac),
    ///     <see cref="WindowsDriver" />, and <see cref="NetDriver" /> that uses the .NET Console API.
    /// </summary>
    public abstract class ConsoleDriver
    {
        /// <summary>
        ///     Enables diagnostic funcions
        /// </summary>
        [Flags]
        public enum DiagnosticFlags : uint
        {
            /// <summary>
            ///     All diagnostics off
            /// </summary>
            Off = 0b_0000_0000,

            /// <summary>
            ///     When enabled, <see cref="DrawWindowFrame(Rect, int, int, int, int, bool, bool)" /> will draw a
            ///     ruler in the frame for any side with a padding value greater than 0.
            /// </summary>
            FrameRuler = 0b_0000_0001,

            /// <summary>
            ///     When Enabled, <see cref="DrawWindowFrame(Rect, int, int, int, int, bool, bool)" /> will use
            ///     'L', 'R', 'T', and 'B' for padding instead of ' '.
            /// </summary>
            FramePadding = 0b_0000_0010
        }

        /// <summary>
        ///     The bottom tee.
        /// </summary>
        public Rune BottomTee = '\u2534';

        /// <summary>
        ///     Checkmark.
        /// </summary>
        public Rune Checked = '\u221a';

        /// <summary>
        ///     Diamond character
        /// </summary>
        public Rune Diamond = '\u25ca';

        /// <summary>
        ///     Down Arrow.
        /// </summary>
        public Rune DownArrow = '\u25bc';

        /// <summary>
        ///     Horizontal line character.
        /// </summary>
        public Rune HLine = '\u2500';

        /// <summary>
        ///     Left Arrow.
        /// </summary>
        public Rune LeftArrow = '\u25c4';

        /// <summary>
        ///     Left frame/bracket (e.g. '[' for <see cref="Button" />).
        /// </summary>
        public Rune LeftBracket = '[';

        /// <summary>
        ///     Left indicator for default action (e.g. for <see cref="Button" />).
        /// </summary>
        public Rune LeftDefaultIndicator = '\u25e6';

        /// <summary>
        ///     Left tee
        /// </summary>
        public Rune LeftTee = '\u251c';

        /// <summary>
        ///     Lower left corner
        /// </summary>
        public Rune LLCorner = '\u2514';

        /// <summary>
        ///     Lower right corner
        /// </summary>
        public Rune LRCorner = '\u2518';

        /// <summary>
        ///     Off Segment indicator for meter views (e.g. <see cref="ProgressBar" />.
        /// </summary>
        public Rune OffMeterSegement = ' ';

        /// <summary>
        ///     On Segment indicator for meter views (e.g. <see cref="ProgressBar" />.
        /// </summary>
        public Rune OnMeterSegment = '\u258c';

        /// <summary>
        ///     Right Arrow.
        /// </summary>
        public Rune RightArrow = '\u25ba';

        /// <summary>
        ///     Right frame/bracket (e.g. ']' for <see cref="Button" />).
        /// </summary>
        public Rune RightBracket = ']';

        /// <summary>
        ///     Right indicator for default action (e.g. for <see cref="Button" />).
        /// </summary>
        public Rune RightDefaultIndicator = '\u25e6';

        /// <summary>
        ///     Right tee
        /// </summary>
        public Rune RightTee = '\u2524';

        /// <summary>
        ///     Selected mark.
        /// </summary>
        public Rune Selected = '\u25cf';

        /// <summary>
        ///     Stipple pattern
        /// </summary>
        public Rune Stipple = '\u2591';

        /// <summary>
        ///     The handler fired when the terminal is resized.
        /// </summary>
        protected Action TerminalResized;

        /// <summary>
        ///     Top tee
        /// </summary>
        public Rune TopTee = '\u252c';

        /// <summary>
        ///     Upper left corner
        /// </summary>
        public Rune ULCorner = '\u250C';

        /// <summary>
        ///     Un-checked checkmark.
        /// </summary>
        public Rune UnChecked = '\u2574';

        /// <summary>
        ///     Un-selected selected mark.
        /// </summary>
        public Rune UnSelected = '\u25cc';

        /// <summary>
        ///     Up Arrow.
        /// </summary>
        public Rune UpArrow = '\u25b2';

        /// <summary>
        ///     Upper right corner
        /// </summary>
        public Rune URCorner = '\u2510';

        /// <summary>
        ///     Vertical line character.
        /// </summary>
        public Rune VLine = '\u2502';

        /// <summary>
        ///     The current number of columns in the terminal.
        /// </summary>
        public abstract int Cols { get; }

        /// <summary>
        ///     The current number of rows in the terminal.
        /// </summary>
        public abstract int Rows { get; }

        /// <summary>
        ///     The current top in the terminal.
        /// </summary>
        public abstract int Top { get; }

        /// <summary>
        ///     Set flags to enable/disable <see cref="ConsoleDriver" /> diagnostics.
        /// </summary>
        public static DiagnosticFlags Diagnostics { get; set; }

        /// <summary>
        ///     Controls the current clipping region that AddRune/AddStr is subject to.
        /// </summary>
        /// <value>The clip.</value>
        public Rect Clip { get; set; }

        /// <summary>
        ///     Initializes the driver
        /// </summary>
        /// <param name="terminalResized">Method to invoke when the terminal is resized.</param>
        public abstract void Init(Action terminalResized);

        /// <summary>
        ///     Moves the cursor to the specified column and row.
        /// </summary>
        /// <param name="col">Column to move the cursor to.</param>
        /// <param name="row">Row to move the cursor to.</param>
        public abstract void Move(int col, int row);

        /// <summary>
        ///     Adds the specified rune to the display at the current cursor position
        /// </summary>
        /// <param name="rune">Rune to add.</param>
        public abstract void AddRune(Rune rune);

        /// <summary>
        ///     Ensures a Rune is not a control character and can be displayed by translating characters below 0x20
        ///     to equivalent, printable, Unicode chars.
        /// </summary>
        /// <param name="c">Rune to translate</param>
        /// <returns></returns>
        public static Rune MakePrintable(Rune c)
        {
            if (c <= 0x1F) // ASCII (C0) control characters. 
                return new Rune(c + 0x2400);
            if (
                c >= 0x80 &&
                c <= 0x9F) // C1 control characters (https://www.aivosto.com/articles/control-characters.html#c1)
                return new Rune(0x25a1); // U+25A1, WHITE SQUARE, □: 
            return c;
        }

        /// <summary>
        ///     Adds the specified
        /// </summary>
        /// <param name="str">String.</param>
        public abstract void AddStr(ustring str);

        /// <summary>
        ///     Prepare the driver and set the key and mouse events handlers.
        /// </summary>
        /// <param name="mainLoop">The main loop.</param>
        /// <param name="keyHandler">The handler for ProcessKey</param>
        /// <param name="keyDownHandler">The handler for key down events</param>
        /// <param name="keyUpHandler">The handler for key up events</param>
        /// <param name="mouseHandler">The handler for mouse events</param>
        public abstract void PrepareToRun(MainLoop mainLoop, Action<KeyEvent> keyHandler,
            Action<KeyEvent> keyDownHandler, Action<KeyEvent> keyUpHandler, Action<MouseEvent> mouseHandler);

        /// <summary>
        ///     Updates the screen to reflect all the changes that have been done to the display buffer
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        ///     Updates the location of the cursor position
        /// </summary>
        public abstract void UpdateCursor();

        /// <summary>
        ///     Ends the execution of the console driver.
        /// </summary>
        public abstract void End();

        /// <summary>
        ///     Redraws the physical screen with the contents that have been queued up via any of the printing commands.
        /// </summary>
        public abstract void UpdateScreen();

        /// <summary>
        ///     Selects the specified attribute as the attribute to use for future calls to AddRune, AddString.
        /// </summary>
        /// <param name="c">C.</param>
        public abstract void SetAttribute(Attribute c);

        /// <summary>
        ///     Set Colors from limit sets of colors.
        /// </summary>
        /// <param name="foreground">Foreground.</param>
        /// <param name="background">Background.</param>
        public abstract void SetColors(ConsoleColor foreground, ConsoleColor background);

        // Advanced uses - set colors to any pre-set pairs, you would need to init_color
        // that independently with the R, G, B values.
        /// <summary>
        ///     Advanced uses - set colors to any pre-set pairs, you would need to init_color
        ///     that independently with the R, G, B values.
        /// </summary>
        /// <param name="foregroundColorId">Foreground color identifier.</param>
        /// <param name="backgroundColorId">Background color identifier.</param>
        public abstract void SetColors(short foregroundColorId, short backgroundColorId);

        /// <summary>
        ///     Set the handler when the terminal is resized.
        /// </summary>
        /// <param name="terminalResized"></param>
        public void SetTerminalResized(Action terminalResized)
        {
            TerminalResized = terminalResized;
        }

        /// <summary>
        ///     Draws the title for a Window-style view incorporating padding.
        /// </summary>
        /// <param name="region">Screen relative region where the frame will be drawn.</param>
        /// <param name="title">
        ///     The title for the window. The title will only be drawn if <c>title</c> is not null or empty and
        ///     paddingTop is greater than 0.
        /// </param>
        /// <param name="paddingLeft">Number of columns to pad on the left (if 0 the border will not appear on the left).</param>
        /// <param name="paddingTop">Number of rows to pad on the top (if 0 the border and title will not appear on the top).</param>
        /// <param name="paddingRight">Number of columns to pad on the right (if 0 the border will not appear on the right).</param>
        /// <param name="paddingBottom">Number of rows to pad on the bottom (if 0 the border will not appear on the bottom).</param>
        /// <param name="textAlignment">Not yet immplemented.</param>
        /// <remarks></remarks>
        public virtual void DrawWindowTitle(Rect region, ustring title, int paddingLeft, int paddingTop,
            int paddingRight, int paddingBottom, TextAlignment textAlignment = TextAlignment.Left)
        {
            var width = region.Width - (paddingLeft + 2) * 2;
            if (!ustring.IsNullOrEmpty(title) && width > 4 && region.Y + paddingTop <= region.Y + paddingBottom)
            {
                Move(region.X + 1 + paddingLeft, region.Y + paddingTop);
                AddRune(' ');
                var str = title.RuneCount >= width ? title[0, width - 2] : title;
                AddStr(str);
                AddRune(' ');
            }
        }

        /// <summary>
        ///     Draws a frame for a window with padding and an optional visible border inside the padding.
        /// </summary>
        /// <param name="region">Screen relative region where the frame will be drawn.</param>
        /// <param name="paddingLeft">Number of columns to pad on the left (if 0 the border will not appear on the left).</param>
        /// <param name="paddingTop">Number of rows to pad on the top (if 0 the border and title will not appear on the top).</param>
        /// <param name="paddingRight">Number of columns to pad on the right (if 0 the border will not appear on the right).</param>
        /// <param name="paddingBottom">Number of rows to pad on the bottom (if 0 the border will not appear on the bottom).</param>
        /// <param name="border">If set to <c>true</c> and any padding dimension is > 0 the border will be drawn.</param>
        /// <param name="fill">
        ///     If set to <c>true</c> it will clear the content area (the area inside the padding) with the current
        ///     color, otherwise the content area will be left untouched.
        /// </param>
        public virtual void DrawWindowFrame(Rect region, int paddingLeft = 0, int paddingTop = 0, int paddingRight = 0,
            int paddingBottom = 0, bool border = true, bool fill = false)
        {
            var clearChar = ' ';
            var leftChar = clearChar;
            var rightChar = clearChar;
            var topChar = clearChar;
            var bottomChar = clearChar;

            if ((Diagnostics & DiagnosticFlags.FramePadding) == DiagnosticFlags.FramePadding)
            {
                leftChar = 'L';
                rightChar = 'R';
                topChar = 'T';
                bottomChar = 'B';
                clearChar = 'C';
            }

            void AddRuneAt(int col, int row, Rune ch)
            {
                Move(col, row);
                AddRune(ch);
            }

            // fwidth is count of hLine chars
            var fwidth = region.Width - (paddingRight + paddingLeft);

            // fheight is count of vLine chars
            var fheight = region.Height - (paddingBottom + paddingTop);

            // fleft is location of left frame line
            var fleft = region.X + paddingLeft - 1;

            // fright is location of right frame line
            var fright = fleft + fwidth + 1;

            // ftop is location of top frame line
            var ftop = region.Y + paddingTop - 1;

            // fbottom is locaiton of bottom frame line
            var fbottom = ftop + fheight + 1;

            var hLine = border ? HLine : clearChar;
            var vLine = border ? VLine : clearChar;
            var uRCorner = border ? URCorner : clearChar;
            var uLCorner = border ? ULCorner : clearChar;
            var lLCorner = border ? LLCorner : clearChar;
            var lRCorner = border ? LRCorner : clearChar;

            // Outside top
            if (paddingTop > 1)
                for (var r = region.Y; r < ftop; r++)
                for (var c = region.X; c < region.X + region.Width; c++)
                    AddRuneAt(c, r, topChar);

            // Outside top-left
            for (var c = region.X; c < fleft; c++) AddRuneAt(c, ftop, leftChar);

            // Frame top-left corner
            AddRuneAt(fleft, ftop, paddingTop >= 0 ? paddingLeft >= 0 ? uLCorner : hLine : leftChar);

            // Frame top
            for (var c = fleft + 1; c < fleft + 1 + fwidth; c++) AddRuneAt(c, ftop, paddingTop > 0 ? hLine : topChar);

            // Frame top-right corner
            if (fright > fleft)
                AddRuneAt(fright, ftop, paddingTop >= 0 ? paddingRight >= 0 ? uRCorner : hLine : rightChar);

            // Outside top-right corner
            for (var c = fright + 1; c < fright + paddingRight; c++) AddRuneAt(c, ftop, rightChar);

            // Left, Fill, Right
            if (fbottom > ftop)
            {
                for (var r = ftop + 1; r < fbottom; r++)
                {
                    // Outside left
                    for (var c = region.X; c < fleft; c++) AddRuneAt(c, r, leftChar);

                    // Frame left
                    AddRuneAt(fleft, r, paddingLeft > 0 ? vLine : leftChar);

                    // Fill
                    if (fill)
                        for (var x = fleft + 1; x < fright; x++)
                            AddRuneAt(x, r, clearChar);

                    // Frame right
                    if (fright > fleft)
                    {
                        var v = vLine;
                        if ((Diagnostics & DiagnosticFlags.FrameRuler) == DiagnosticFlags.FrameRuler)
                            v = (char) ('0' + (r - ftop) % 10); // vLine;
                        AddRuneAt(fright, r, paddingRight > 0 ? v : rightChar);
                    }

                    // Outside right
                    for (var c = fright + 1; c < fright + paddingRight; c++) AddRuneAt(c, r, rightChar);
                }

                // Outside Bottom
                for (var c = region.X; c < region.X + region.Width; c++) AddRuneAt(c, fbottom, leftChar);

                // Frame bottom-left
                AddRuneAt(fleft, fbottom, paddingLeft > 0 ? lLCorner : leftChar);

                if (fright > fleft)
                {
                    // Frame bottom
                    for (var c = fleft + 1; c < fright; c++)
                    {
                        var h = hLine;
                        if ((Diagnostics & DiagnosticFlags.FrameRuler) == DiagnosticFlags.FrameRuler)
                            h = (char) ('0' + (c - fleft) % 10); // hLine;
                        AddRuneAt(c, fbottom, paddingBottom > 0 ? h : bottomChar);
                    }

                    // Frame bottom-right
                    AddRuneAt(fright, fbottom, paddingRight > 0 ? paddingBottom > 0 ? lRCorner : hLine : rightChar);
                }

                // Outside right
                for (var c = fright + 1; c < fright + paddingRight; c++) AddRuneAt(c, fbottom, rightChar);
            }

            // Out bottom - ensure top is always drawn if we overlap
            if (paddingBottom > 0)
                for (var r = fbottom + 1; r < fbottom + paddingBottom; r++)
                for (var c = region.X; c < region.X + region.Width; c++)
                    AddRuneAt(c, r, bottomChar);
        }

        /// <summary>
        ///     Draws a frame on the specified region with the specified padding around the frame.
        /// </summary>
        /// <param name="region">Screen relative region where the frame will be drawn.</param>
        /// <param name="padding">Padding to add on the sides.</param>
        /// <param name="fill">
        ///     If set to <c>true</c> it will clear the contents with the current color, otherwise the contents will
        ///     be left untouched.
        /// </param>
        /// <remarks>This API has been superceded by <see cref="DrawWindowFrame(Rect, int, int, int, int, bool, bool)" />.</remarks>
        /// <remarks>
        ///     This API is equivalent to calling <c>DrawWindowFrame(Rect, p - 1, p - 1, p - 1, p - 1)</c>. In other words,
        ///     A padding value of 0 means there is actually a one cell border.
        /// </remarks>
        public virtual void DrawFrame(Rect region, int padding, bool fill)
        {
            // DrawFrame assumes the border is always at least one row/col thick
            // DrawWindowFrame assumes a padding of 0 means NO padding and no frame
            DrawWindowFrame(new Rect(region.X, region.Y, region.Width, region.Height),
                padding + 1, padding + 1, padding + 1, padding + 1, false, fill);
        }


        /// <summary>
        ///     Suspend the application, typically needs to save the state, suspend the app and upon return, reset the console
        ///     driver.
        /// </summary>
        public abstract void Suspend();

        /// <summary>
        ///     Start of mouse moves.
        /// </summary>
        public abstract void StartReportingMouseMoves();

        /// <summary>
        ///     Stop reporting mouses moves.
        /// </summary>
        public abstract void StopReportingMouseMoves();

        /// <summary>
        ///     Disables the cooked event processing from the mouse driver.  At startup, it is assumed mouse events are cooked.
        /// </summary>
        public abstract void UncookMouse();

        /// <summary>
        ///     Enables the cooked event processing from the mouse driver
        /// </summary>
        public abstract void CookMouse();

        /// <summary>
        ///     Make the attribute for the foreground and background colors.
        /// </summary>
        /// <param name="fore">Foreground.</param>
        /// <param name="back">Background.</param>
        /// <returns></returns>
        public abstract Attribute MakeAttribute(Color fore, Color back);
    }
}