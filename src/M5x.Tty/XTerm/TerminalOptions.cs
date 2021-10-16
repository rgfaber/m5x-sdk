namespace M5x.Tty.XTerm
{
    public enum CursorStyle
    {
        BlinkBlock,
        SteadyBlock,
        BlinkUnderline,
        SteadyUnderline,
        BlinkingBar,
        SteadyBar
    }

    public class TerminalOptions
    {
        public int Cols, Rows;
        public bool ConvertEol = true, CursorBlink;
        public CursorStyle CursorStyle;
        public bool ScreenReaderMode;
        public string TermName;

        public TerminalOptions()
        {
            Cols = 80;
            Rows = 25;
            TermName = "xterm";
            Scrollback = 1000;
            TabStopWidth = 8;
        }

        public int? Scrollback { get; set; }
        public int? TabStopWidth { get; set; }
    }
}