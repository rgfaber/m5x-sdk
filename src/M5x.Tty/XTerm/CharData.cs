﻿using System;
using System.Diagnostics;

namespace M5x.Tty.XTerm
{
    // MIGUEL TODO:
    // The original code used Rune + Code, but it really makes no sense to keep those separate, excpt for null that has a
    // zero-width thing for code 0.
    [DebuggerDisplay("[CharData (Attr={Attribute},Rune={Rune},W={Width},Code={Code})]")]
    public struct CharData
    {
        public int Attribute;
        public Rune Rune;
        public int Width;
        public int Code;

        // ((int)flags << 18) | (fg << 9) | bg;

        public const int DefaultAttr = (Renderer.Renderer.DefaultColor << 9) | (256 << 0);

        public const int InvertedAttr = (Renderer.Renderer.InvertedDefaultColor << 9) | (256 << 0) |
                                        Renderer.Renderer.InvertedDefaultColor;

        public static CharData Null = new(DefaultAttr, '\u0200', 1, 0);
        public static CharData WhiteSpace = new(DefaultAttr, ' ', 1, 32);
        public static CharData LeftBrace = new(DefaultAttr, '{', 1, 123);
        public static CharData RightBrace = new(DefaultAttr, '}', 1, 125);
        public static CharData LeftBracket = new(DefaultAttr, '[', 1, 91);
        public static CharData RightBracket = new(DefaultAttr, ']', 1, 93);
        public static CharData LeftParenthesis = new(DefaultAttr, '(', 1, 40);
        public static CharData RightParenthesis = new(DefaultAttr, ')', 1, 41);
        public static CharData Period = new(DefaultAttr, '.', 1, 46);

        public CharData(int attribute, Rune rune, int width, int code)
        {
            Attribute = attribute;
            Rune = rune;
            Width = width;
            Code = code;
        }

        // Returns an empty CharData with the specified attribute
        public CharData(int attribute)
        {
            Attribute = attribute;
            Rune = '\u0200';
            Width = 1;
            Code = 0;
        }

        /// <summary>
        ///     Returns true if this CharData matches the given Rune, irrespective of character attributes
        /// </summary>
        public bool MatchesRune(Rune rune)
        {
            return rune == Rune;
        }

        /// <summary>
        ///     Returns true if this CharData matches the given Rune, irrespective of character attributes
        /// </summary>
        public bool MatchesRune(CharData chr)
        {
            return Rune == chr.Rune;
        }

        /// <summary>
        ///     returns true if this CharData matches Null or has a code of 0
        /// </summary>
        public bool IsNullChar()
        {
            return Rune == Null.Rune || Code == 0;
        }
    }
}