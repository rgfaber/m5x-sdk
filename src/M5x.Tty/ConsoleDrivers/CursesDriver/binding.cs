//
// TODO:
// * FindNCurses needs to remove the old probing code
// * Removal of that proxy code
// * Need to implement reading pointers with the new API
// * Can remove the manual Dlopen features
// * initscr() diagnostics based on DLL can be fixed
//
// binding.cs.in: Core binding for curses.
//
// This file attempts to call into ncurses without relying on Mono's
// dllmap, so it will work with .NET Core.  This means that it needs
// two sets of bindings, one for "ncurses" which works on OSX, and one
// that works against "libncursesw.so.5" which is what you find on
// assorted Linux systems.
//
// Additionally, I do not want to rely on an external native library
// which is why all this pain to bind two separate ncurses is here.
//
// Authors:
//   Miguel de Icaza (miguel.de.icaza@gmail.com)
//
// Copyright (C) 2007 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;

namespace M5x.Tty.ConsoleDrivers.CursesDriver
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public partial class Curses
    {
        // We encode ESC + char (what Alt-char generates) as 0x2000 + char
        public const int KeyAlt = 0x2000;

        private static int lines, cols;
        private static Window main_window;
        private static IntPtr curses_handle, curscr_ptr, lines_ptr, cols_ptr;

        // If true, uses the DllImport into "ncurses", otherwise "libncursesw.so.5"
        //static bool use_naked_driver;

        private static UnmanagedLibrary curses_library;
        private static NativeMethods methods;

        private static char[] r = new char [1];

        private static IntPtr stdscr;

        public static int Lines => lines;

        public static int Cols => cols;

        public static bool HasColors => methods.has_colors();
        public static int ColorPairs => methods.COLOR_PAIRS();


        [DllImport("libc")]
        public static extern int setlocale(int cate, [MarshalAs(UnmanagedType.LPStr)] string locale);

        private static void LoadMethods()
        {
            var libs = UnmanagedLibrary.IsMacOSPlatform
                ? new[] {"libncurses.dylib"}
                : new[] {"libncursesw.so.6", "libncursesw.so.5"};
            curses_library = new UnmanagedLibrary(libs, false);
            methods = new NativeMethods(curses_library);
        }

        private static void FindNCurses()
        {
            LoadMethods();
            curses_handle = methods.UnmanagedLibrary.NativeLibraryHandle;

            stdscr = read_static_ptr("stdscr");
            curscr_ptr = get_ptr("curscr");
            lines_ptr = get_ptr("LINES");
            cols_ptr = get_ptr("COLS");
        }

        public static Window initscr()
        {
            setlocale(LC_ALL, "");
            FindNCurses();

            main_window = new Window(methods.initscr());
            try
            {
                console_sharp_get_dims(out lines, out cols);
            }
            catch (DllNotFoundException)
            {
                endwin();
                Console.Error.WriteLine("Unable to find the @MONO_CURSES@ native library\n" +
                                        "this is different than the managed mono-curses.dll\n\n" +
                                        "Typically you need to install to a LD_LIBRARY_PATH directory\n" +
                                        "or DYLD_LIBRARY_PATH directory or run /sbin/ldconfig");
                Environment.Exit(1);
            }

            return main_window;
        }

        //
        // Returns true if the window changed since the last invocation, as a
        // side effect, the Lines and Cols properties are updated
        //
        public static bool CheckWinChange()
        {
            int l, c;

            console_sharp_get_dims(out l, out c);
            if (l != lines || c != cols)
            {
                lines = l;
                cols = c;
                return true;
            }

            return false;
        }

        public static int addstr(string format, params object[] args)
        {
            var s = string.Format(format, args);
            return addwstr(s);
        }

        //
        // Have to wrap the native addch, as it can not
        // display unicode characters, we have to use addstr
        // for that.   but we need addch to render special ACS
        // characters
        //
        public static int addch(int ch)
        {
            if (ch < 127 || ch > 0xffff)
                return methods.addch(ch);
            var c = (char) ch;
            return addwstr(new string(c, 1));
        }

        private static IntPtr get_ptr(string key)
        {
            var ptr = curses_library.LoadSymbol(key);

            if (ptr == IntPtr.Zero)
                throw new Exception("Could not load the key " + key);
            return ptr;
        }

        internal static IntPtr read_static_ptr(string key)
        {
            var ptr = get_ptr(key);
            return Marshal.ReadIntPtr(ptr);
        }

        internal static IntPtr console_sharp_get_stdscr()
        {
            return stdscr;
        }


        internal static IntPtr console_sharp_get_curscr()
        {
            return Marshal.ReadIntPtr(curscr_ptr);
        }

        internal static void console_sharp_get_dims(out int lines, out int cols)
        {
            lines = Marshal.ReadInt32(lines_ptr);
            cols = Marshal.ReadInt32(cols_ptr);
        }

        public static Event mousemask(Event newmask, out Event oldmask)
        {
            IntPtr e;
            var ret = (Event) methods.mousemask((IntPtr) newmask, out e);
            oldmask = (Event) e;
            return ret;
        }

        public static int IsAlt(int key)
        {
            if ((key & KeyAlt) != 0)
                return key & ~KeyAlt;
            return 0;
        }

        public static int StartColor()
        {
            return methods.start_color();
        }

        public static int InitColorPair(short pair, short foreground, short background)
        {
            return methods.init_pair(pair, foreground, background);
        }

        public static int UseDefaultColors()
        {
            return methods.use_default_colors();
        }

        //
        // The proxy methods to call into each version
        //
        public static int endwin()
        {
            return methods.endwin();
        }

        public static bool isendwin()
        {
            return methods.isendwin();
        }

        public static int cbreak()
        {
            return methods.cbreak();
        }

        public static int nocbreak()
        {
            return methods.nocbreak();
        }

        public static int echo()
        {
            return methods.echo();
        }

        public static int noecho()
        {
            return methods.noecho();
        }

        public static int halfdelay(int t)
        {
            return methods.halfdelay(t);
        }

        public static int raw()
        {
            return methods.raw();
        }

        public static int noraw()
        {
            return methods.noraw();
        }

        public static void noqiflush()
        {
            methods.noqiflush();
        }

        public static void qiflush()
        {
            methods.qiflush();
        }

        public static int typeahead(IntPtr fd)
        {
            return methods.typeahead(fd);
        }

        public static int timeout(int delay)
        {
            return methods.timeout(delay);
        }

        public static int wtimeout(IntPtr win, int delay)
        {
            return methods.wtimeout(win, delay);
        }

        public static int notimeout(IntPtr win, bool bf)
        {
            return methods.notimeout(win, bf);
        }

        public static int keypad(IntPtr win, bool bf)
        {
            return methods.keypad(win, bf);
        }

        public static int meta(IntPtr win, bool bf)
        {
            return methods.meta(win, bf);
        }

        public static int intrflush(IntPtr win, bool bf)
        {
            return methods.intrflush(win, bf);
        }

        public static int clearok(IntPtr win, bool bf)
        {
            return methods.clearok(win, bf);
        }

        public static int idlok(IntPtr win, bool bf)
        {
            return methods.idlok(win, bf);
        }

        public static void idcok(IntPtr win, bool bf)
        {
            methods.idcok(win, bf);
        }

        public static void immedok(IntPtr win, bool bf)
        {
            methods.immedok(win, bf);
        }

        public static int leaveok(IntPtr win, bool bf)
        {
            return methods.leaveok(win, bf);
        }

        public static int wsetscrreg(IntPtr win, int top, int bot)
        {
            return methods.wsetscrreg(win, top, bot);
        }

        public static int scrollok(IntPtr win, bool bf)
        {
            return methods.scrollok(win, bf);
        }

        public static int nl()
        {
            return methods.nl();
        }

        public static int nonl()
        {
            return methods.nonl();
        }

        public static int setscrreg(int top, int bot)
        {
            return methods.setscrreg(top, bot);
        }

        public static int refresh()
        {
            return methods.refresh();
        }

        public static int doupdate()
        {
            return methods.doupdate();
        }

        public static int wrefresh(IntPtr win)
        {
            return methods.wrefresh(win);
        }

        public static int redrawwin(IntPtr win)
        {
            return methods.redrawwin(win);
        }

        //static public int wredrawwin (IntPtr win, int beg_line, int num_lines) => methods.wredrawwin (win, beg_line, num_lines);
        public static int wnoutrefresh(IntPtr win)
        {
            return methods.wnoutrefresh(win);
        }

        public static int move(int line, int col)
        {
            return methods.move(line, col);
        }

        //static public int addch (int ch) => methods.addch (ch);
        public static int addwstr(string s)
        {
            return methods.addwstr(s);
        }

        public static int wmove(IntPtr win, int line, int col)
        {
            return methods.wmove(win, line, col);
        }

        public static int waddch(IntPtr win, int ch)
        {
            return methods.waddch(win, ch);
        }

        public static int attron(int attrs)
        {
            return methods.attron(attrs);
        }

        public static int attroff(int attrs)
        {
            return methods.attroff(attrs);
        }

        public static int attrset(int attrs)
        {
            return methods.attrset(attrs);
        }

        public static int getch()
        {
            return methods.getch();
        }

        public static int get_wch(out int sequence)
        {
            return methods.get_wch(out sequence);
        }

        public static int ungetch(int ch)
        {
            return methods.ungetch(ch);
        }

        public static int mvgetch(int y, int x)
        {
            return methods.mvgetch(y, x);
        }

        public static bool has_colors()
        {
            return methods.has_colors();
        }

        public static int start_color()
        {
            return methods.start_color();
        }

        public static int init_pair(short pair, short f, short b)
        {
            return methods.init_pair(pair, f, b);
        }

        public static int use_default_colors()
        {
            return methods.use_default_colors();
        }

        public static int COLOR_PAIRS()
        {
            return methods.COLOR_PAIRS();
        }

        public static uint getmouse(out MouseEvent ev)
        {
            return methods.getmouse(out ev);
        }

        public static uint ungetmouse(ref MouseEvent ev)
        {
            return methods.ungetmouse(ref ev);
        }

        public static int mouseinterval(int interval)
        {
            return methods.mouseinterval(interval);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseEvent
        {
            public short ID;
            public int X, Y, Z;
            public Event ButtonState;
        }
    }

#pragma warning disable RCS1102 // Make class static.
    internal class Delegates
    {
        public delegate int addch(int ch);

        public delegate int addwstr([MarshalAs(UnmanagedType.LPWStr)] string s);

        public delegate int attroff(int attrs);

        public delegate int attron(int attrs);

        public delegate int attrset(int attrs);

        public delegate int cbreak();

        public delegate int clearok(IntPtr win, bool bf);

        public delegate int COLOR_PAIRS();

        public delegate int doupdate();

        public delegate int echo();

        public delegate int endwin();

        public delegate int get_wch(out int sequence);

        public delegate int getch();

        public delegate uint getmouse(out Curses.MouseEvent ev);

        public delegate int halfdelay(int t);

        public delegate bool has_colors();

        public delegate void idcok(IntPtr win, bool bf);

        public delegate int idlok(IntPtr win, bool bf);

        public delegate void immedok(IntPtr win, bool bf);

        public delegate int init_pair(short pair, short f, short b);

        public delegate IntPtr initscr();

        public delegate int intrflush(IntPtr win, bool bf);

        public delegate bool isendwin();

        public delegate int keypad(IntPtr win, bool bf);

        public delegate int leaveok(IntPtr win, bool bf);

        public delegate int meta(IntPtr win, bool bf);

        public delegate int mouseinterval(int interval);

        public delegate IntPtr mousemask(IntPtr newmask, out IntPtr oldMask);

        public delegate int move(int line, int col);

        public delegate int mvgetch(int y, int x);

        public delegate int nl();

        public delegate int nocbreak();

        public delegate int noecho();

        public delegate int nonl();

        public delegate void noqiflush();

        public delegate int noraw();

        public delegate int notimeout(IntPtr win, bool bf);

        public delegate void qiflush();

        public delegate int raw();

        public delegate int redrawwin(IntPtr win);

        public delegate int refresh();

        public delegate int scrollok(IntPtr win, bool bf);

        public delegate int setscrreg(int top, int bot);

        public delegate int start_color();

        public delegate int timeout(int delay);

        public delegate int typeahead(IntPtr fd);

        public delegate int ungetch(int ch);

        public delegate uint ungetmouse(ref Curses.MouseEvent ev);

        public delegate int use_default_colors();

        public delegate int waddch(IntPtr win, int ch);

        public delegate int wmove(IntPtr win, int line, int col);

        //public delegate int wredrawwin (IntPtr win, int beg_line, int num_lines);
        public delegate int wnoutrefresh(IntPtr win);

        public delegate int wrefresh(IntPtr win);

        public delegate int wsetscrreg(IntPtr win, int top, int bot);

        public delegate int wtimeout(IntPtr win, int delay);
#pragma warning restore RCS1102 // Make class static.
    }

    internal class NativeMethods
    {
        public readonly Delegates.addch addch;
        public readonly Delegates.addwstr addwstr;
        public readonly Delegates.attroff attroff;
        public readonly Delegates.attron attron;
        public readonly Delegates.attrset attrset;
        public readonly Delegates.cbreak cbreak;
        public readonly Delegates.clearok clearok;
        public readonly Delegates.COLOR_PAIRS COLOR_PAIRS;
        public readonly Delegates.doupdate doupdate;
        public readonly Delegates.echo echo;
        public readonly Delegates.endwin endwin;
        public readonly Delegates.get_wch get_wch;
        public readonly Delegates.getch getch;
        public readonly Delegates.getmouse getmouse;
        public readonly Delegates.halfdelay halfdelay;
        public readonly Delegates.has_colors has_colors;
        public readonly Delegates.idcok idcok;
        public readonly Delegates.idlok idlok;
        public readonly Delegates.immedok immedok;
        public readonly Delegates.init_pair init_pair;
        public readonly Delegates.initscr initscr;
        public readonly Delegates.intrflush intrflush;
        public readonly Delegates.isendwin isendwin;
        public readonly Delegates.keypad keypad;
        public readonly Delegates.leaveok leaveok;
        public readonly Delegates.meta meta;
        public readonly Delegates.mouseinterval mouseinterval;
        public readonly Delegates.mousemask mousemask;
        public readonly Delegates.move move;
        public readonly Delegates.mvgetch mvgetch;
        public readonly Delegates.nl nl;
        public readonly Delegates.nocbreak nocbreak;
        public readonly Delegates.noecho noecho;
        public readonly Delegates.nonl nonl;
        public readonly Delegates.noqiflush noqiflush;
        public readonly Delegates.noraw noraw;
        public readonly Delegates.notimeout notimeout;
        public readonly Delegates.qiflush qiflush;
        public readonly Delegates.raw raw;
        public readonly Delegates.redrawwin redrawwin;
        public readonly Delegates.refresh refresh;
        public readonly Delegates.scrollok scrollok;
        public readonly Delegates.setscrreg setscrreg;
        public readonly Delegates.start_color start_color;
        public readonly Delegates.timeout timeout;
        public readonly Delegates.typeahead typeahead;
        public readonly Delegates.ungetch ungetch;
        public readonly Delegates.ungetmouse ungetmouse;
        public readonly Delegates.use_default_colors use_default_colors;
        public readonly Delegates.waddch waddch;

        public readonly Delegates.wmove wmove;

        //public readonly Delegates.wredrawwin wredrawwin;
        public readonly Delegates.wnoutrefresh wnoutrefresh;
        public readonly Delegates.wrefresh wrefresh;
        public readonly Delegates.wsetscrreg wsetscrreg;
        public readonly Delegates.wtimeout wtimeout;
        public UnmanagedLibrary UnmanagedLibrary;

        public NativeMethods(UnmanagedLibrary lib)
        {
            UnmanagedLibrary = lib;
            initscr = lib.GetNativeMethodDelegate<Delegates.initscr>("initscr");
            endwin = lib.GetNativeMethodDelegate<Delegates.endwin>("endwin");
            isendwin = lib.GetNativeMethodDelegate<Delegates.isendwin>("isendwin");
            cbreak = lib.GetNativeMethodDelegate<Delegates.cbreak>("cbreak");
            nocbreak = lib.GetNativeMethodDelegate<Delegates.nocbreak>("nocbreak");
            echo = lib.GetNativeMethodDelegate<Delegates.echo>("echo");
            noecho = lib.GetNativeMethodDelegate<Delegates.noecho>("noecho");
            halfdelay = lib.GetNativeMethodDelegate<Delegates.halfdelay>("halfdelay");
            raw = lib.GetNativeMethodDelegate<Delegates.raw>("raw");
            noraw = lib.GetNativeMethodDelegate<Delegates.noraw>("noraw");
            noqiflush = lib.GetNativeMethodDelegate<Delegates.noqiflush>("noqiflush");
            qiflush = lib.GetNativeMethodDelegate<Delegates.qiflush>("qiflush");
            typeahead = lib.GetNativeMethodDelegate<Delegates.typeahead>("typeahead");
            timeout = lib.GetNativeMethodDelegate<Delegates.timeout>("timeout");
            wtimeout = lib.GetNativeMethodDelegate<Delegates.wtimeout>("wtimeout");
            notimeout = lib.GetNativeMethodDelegate<Delegates.notimeout>("notimeout");
            keypad = lib.GetNativeMethodDelegate<Delegates.keypad>("keypad");
            meta = lib.GetNativeMethodDelegate<Delegates.meta>("meta");
            intrflush = lib.GetNativeMethodDelegate<Delegates.intrflush>("intrflush");
            clearok = lib.GetNativeMethodDelegate<Delegates.clearok>("clearok");
            idlok = lib.GetNativeMethodDelegate<Delegates.idlok>("idlok");
            idcok = lib.GetNativeMethodDelegate<Delegates.idcok>("idcok");
            immedok = lib.GetNativeMethodDelegate<Delegates.immedok>("immedok");
            leaveok = lib.GetNativeMethodDelegate<Delegates.leaveok>("leaveok");
            wsetscrreg = lib.GetNativeMethodDelegate<Delegates.wsetscrreg>("wsetscrreg");
            scrollok = lib.GetNativeMethodDelegate<Delegates.scrollok>("scrollok");
            nl = lib.GetNativeMethodDelegate<Delegates.nl>("nl");
            nonl = lib.GetNativeMethodDelegate<Delegates.nonl>("nonl");
            setscrreg = lib.GetNativeMethodDelegate<Delegates.setscrreg>("setscrreg");
            refresh = lib.GetNativeMethodDelegate<Delegates.refresh>("refresh");
            doupdate = lib.GetNativeMethodDelegate<Delegates.doupdate>("doupdate");
            wrefresh = lib.GetNativeMethodDelegate<Delegates.wrefresh>("wrefresh");
            redrawwin = lib.GetNativeMethodDelegate<Delegates.redrawwin>("redrawwin");
            //wredrawwin = lib.GetNativeMethodDelegate<Delegates.wredrawwin> ("wredrawwin");
            wnoutrefresh = lib.GetNativeMethodDelegate<Delegates.wnoutrefresh>("wnoutrefresh");
            move = lib.GetNativeMethodDelegate<Delegates.move>("move");
            addch = lib.GetNativeMethodDelegate<Delegates.addch>("addch");
            addwstr = lib.GetNativeMethodDelegate<Delegates.addwstr>("addwstr");
            wmove = lib.GetNativeMethodDelegate<Delegates.wmove>("wmove");
            waddch = lib.GetNativeMethodDelegate<Delegates.waddch>("waddch");
            attron = lib.GetNativeMethodDelegate<Delegates.attron>("attron");
            attroff = lib.GetNativeMethodDelegate<Delegates.attroff>("attroff");
            attrset = lib.GetNativeMethodDelegate<Delegates.attrset>("attrset");
            getch = lib.GetNativeMethodDelegate<Delegates.getch>("getch");
            get_wch = lib.GetNativeMethodDelegate<Delegates.get_wch>("get_wch");
            ungetch = lib.GetNativeMethodDelegate<Delegates.ungetch>("ungetch");
            mvgetch = lib.GetNativeMethodDelegate<Delegates.mvgetch>("mvgetch");
            has_colors = lib.GetNativeMethodDelegate<Delegates.has_colors>("has_colors");
            start_color = lib.GetNativeMethodDelegate<Delegates.start_color>("start_color");
            init_pair = lib.GetNativeMethodDelegate<Delegates.init_pair>("init_pair");
            use_default_colors = lib.GetNativeMethodDelegate<Delegates.use_default_colors>("use_default_colors");
            COLOR_PAIRS = lib.GetNativeMethodDelegate<Delegates.COLOR_PAIRS>("COLOR_PAIRS");
            getmouse = lib.GetNativeMethodDelegate<Delegates.getmouse>("getmouse");
            ungetmouse = lib.GetNativeMethodDelegate<Delegates.ungetmouse>("ungetmouse");
            mouseinterval = lib.GetNativeMethodDelegate<Delegates.mouseinterval>("mouseinterval");
            mousemask = lib.GetNativeMethodDelegate<Delegates.mousemask>("mousemask");
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}