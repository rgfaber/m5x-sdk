namespace M5x.Tty.XTerm
{
    public struct ControlCodes
    {
        public static uint NUL = 0x00;
        public static uint BEL = 0x07;
        public static uint BS = 0x08;
        public static uint HT = 0x09;
        public static uint LF = 0x0a;
        public static uint VT = 0x0b;
        public static uint FF = 0x0c;
        public static uint CR = 0x0d;
        public static uint SO = 0x0e;
        public static uint SI = 0x0f;
        public static uint CAN = 0x18;
        public static uint SUB = 0x1a;
        public static uint ESC = 0x1b;
        public static uint SP = 0x20;
        public static uint DEL = 0x7f;

        public bool Send8bit { get; set; }

        public string PAD => Send8bit ? "\u0080" : "\u001b@";
        public string HOP => Send8bit ? "\u0081" : "\u001bA";
        public string BPH => Send8bit ? "\u0082" : "\u001bB";
        public string NBH => Send8bit ? "\u0083" : "\u001bC";
        public string IND => Send8bit ? "\u0084" : "\u001bD";
        public string NEL => Send8bit ? "\u0085" : "\u001bE";
        public string SSA => Send8bit ? "\u0086" : "\u001bF";
        public string ESA => Send8bit ? "\u0087" : "\u001bG";
        public string HTS => Send8bit ? "\u0088" : "\u001bH";
        public string HTJ => Send8bit ? "\u0089" : "\u001bI";
        public string VTS => Send8bit ? "\u008a" : "\u001bJ";
        public string PLD => Send8bit ? "\u008b" : "\u001bK";
        public string PLU => Send8bit ? "\u008c" : "\u001bL";
        public string RI => Send8bit ? "\u008d" : "\u001bM";
        public string SS2 => Send8bit ? "\u008e" : "\u001bN";
        public string SS3 => Send8bit ? "\u008f" : "\u001bO";
        public string DCS => Send8bit ? "\u0090" : "\u001bP";
        public string PU1 => Send8bit ? "\u0091" : "\u001bQ";
        public string PU2 => Send8bit ? "\u0092" : "\u001bR";
        public string STS => Send8bit ? "\u0093" : "\u001bS";
        public string CCH => Send8bit ? "\u0094" : "\u001bT";
        public string MW => Send8bit ? "\u0095" : "\u001bU";
        public string SPA => Send8bit ? "\u0096" : "\u001bV";
        public string EPA => Send8bit ? "\u0097" : "\u001bW";
        public string SOS => Send8bit ? "\u0098" : "\u001bX";
        public string SGCI => Send8bit ? "\u0099" : "\u001bY";
        public string SCI => Send8bit ? "\u009a" : "\u001bZ";
        public string CSI => Send8bit ? "\u009b" : "\u001b[";
        public string ST => Send8bit ? "\u009c" : "\u001b\\";
        public string OSC => Send8bit ? "\u009d" : "\u001b]";
        public string PM => Send8bit ? "\u009e" : "\u001b^";
        public string APC => Send8bit ? "\u009f" : "\u001b_";
    }
}