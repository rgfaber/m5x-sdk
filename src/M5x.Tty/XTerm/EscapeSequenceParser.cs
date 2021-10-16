using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M5x.Tty.XTerm
{
    public interface IDcsHandler
    {
        void Hook(string collect, int[] parameters, int flag);
        unsafe void Put(byte* data, int start, int end);
        void Unhook();
    }

    // Dummy DCS Handler as defaulta fallback
    internal class DcsDummy : IDcsHandler
    {
        public void Hook(string collect, int[] parameters, int flag)
        {
        }

        public unsafe void Put(byte* data, int start, int end)
        {
        }

        public void Unhook()
        {
        }
    }

    public enum ParserState
    {
        Invalid = -1,
        Ground = 0,
        Escape,
        EscapeIntermediate,
        CsiEntry,
        CsiParam,
        CsiIntermediate,
        CsiIgnore,
        SosPmApcString,
        OscString,
        DcsEntry,
        DcsParam,
        DcsIgnore,
        DcsIntermediate,
        DcsPassthrough
    }

    public class ParsingState
    {
        // should abort (default: false)
        public bool Abort;

        /// <summary>
        ///     Actual character code
        /// </summary>
        public int Code;

        /// <summary>
        ///     Collect buffer with intermediate characters
        /// </summary>
        public string Collect;

        /// <summary>
        ///     Current Parser State
        /// </summary>
        public ParserState CurrentState;

        /// <summary>
        ///     Buffer start index (-1 for not set)
        /// </summary>
        public int Dcs;

        /// <summary>
        ///     Osc string buffer
        /// </summary>
        public string Osc;

        /// <summary>
        ///     Parameters buffer
        /// </summary>
        public int[] Parameters;

        /// <summary>
        ///     Position in Parse String
        /// </summary>
        public int Position;

        /// <summary>
        ///     Print buffer start index (-1 for not set)
        /// </summary>
        public int Print;
    }

    //
    // EscapeSequenceParser.
    // This class implements the ANSI/DEC compatible parser described by
    // Paul Williams (https://vt100.net/emu/dec_ansi_parser).
    // To implement custom ANSI compliant escape sequences it is not needed to
    // alter this parser, instead consider registering a custom handler.
    // For non ANSI compliant sequences change the transition table with
    // the optional `transitions` contructor argument and
    // reimplement the `parse` method.
    // NOTE: The parameter element notation is currently not supported.
    // TODO: implement error recovery hook via error handler return values
    // 
    public class EscapeSequenceParser : IDisposable
    {
        public delegate void CsiHandler(int[] parameters, string collect);

        public delegate void EscHandler(string collect, int flag);

        public delegate void ExecuteHandler();

        public delegate void OscHandler(string data);

        public unsafe delegate void PrintHandler(byte* data, int start, int end);

        private const int NonAsciiPrintable = 0xa0;
        private static readonly int[] PRINTABLES = r(0x20, 0x7f);
        private static readonly int[] EXECUTABLES = r(0x00, 0x19).Concat(r(0x1c, 0x20)).ToArray();


        private readonly TransitionTable table;
        private string _collect;

        // buffers over several parser calls
        private string _osc;
        private List<int> _pars;
        public IDcsHandler ActiveDcsHandler;

        public Action<string, int[], int> CsiHandlerFallback = (collect, parameters, flag) =>
        {
            Console.WriteLine("Can not handle ESC-[" + flag);
        };

        // Handler lookup container
        public Dictionary<byte, List<CsiHandler>> CsiHandlers;
        public IDcsHandler DcsHandlerFallback = new DcsDummy();
        public Dictionary<string, IDcsHandler> DcsHandlers;
        public Func<ParsingState, ParsingState> ErrorHandler;
        public Func<ParsingState, ParsingState> ErrorHandlerFallback = state => state;
        public EscHandler EscHandlerFallback = (collect, flag) => { };
        public Dictionary<string, EscHandler> EscHandlers;
        public Action<byte> ExecuteHandlerFallback = EmptyExecuteHandler;
        public Dictionary<byte, ExecuteHandler> ExecuteHandlers;

        public ParserState initialState, currentState;
        public Action<int, string> OscHandlerFallback = (identifier, data) => { };
        public Dictionary<int, List<OscHandler>> OscHandlers;

        unsafe private PrintHandler printHandler = (_, start, end) => { };


    // Fallback handlers
        unsafe public PrintHandler PrintHandlerFallback = (_, start, end) => { };
        public Action PrintStateReset = () => { };

        public EscapeSequenceParser()
        {
            table = BuildVt500TransitionTable();
            CsiHandlers = new Dictionary<byte, List<CsiHandler>>();
            OscHandlers = new Dictionary<int, List<OscHandler>>();
            ExecuteHandlers = new Dictionary<byte, ExecuteHandler>();
            EscHandlers = new Dictionary<string, EscHandler>();
            DcsHandlers = new Dictionary<string, IDcsHandler>();

            initialState = ParserState.Ground;
            currentState = initialState;
            _osc = "";
            _pars = new List<int> {0};
            _collect = "";
            SetEscHandler("\\", EscHandlerFallback);
        }

        public void Dispose()
        {
            CsiHandlers = null;
            OscHandlers = null;
            ExecuteHandlers = null;
            EscHandlers = null;
            DcsHandlers = null;
            ActiveDcsHandler = null;
            ErrorHandler = null;
            PrintHandlerFallback = null;
            ExecuteHandlerFallback = null;
            CsiHandlerFallback = null;
            EscHandlerFallback = null;
            OscHandlerFallback = null;
            DcsHandlerFallback = null;
            ErrorHandlerFallback = null;
            printHandler = null;
        }

        private static int[] r(int low, int high)
        {
            var c = high - low;
            var arr = new int [c];
            while (c-- > 0)
                arr[c] = --high;
            return arr;
        }

        private static ParserState[] r(ParserState low, ParserState high)
        {
            var c = high - low;
            var arr = new ParserState [c];
            while (c-- > 0)
                arr[c] = --high;
            return arr;
        }

        private static TransitionTable BuildVt500TransitionTable()
        {
            var table = new TransitionTable(4095);
            var states = r(ParserState.Ground, ParserState.DcsPassthrough + 1);

            // table with default transition
            foreach (var state in states)
                for (var code = 0; code <= NonAsciiPrintable; ++code)
                    table.Add(code, state, ParserAction.Error, ParserState.Ground);
            // printables
            table.Add(PRINTABLES, ParserState.Ground, ParserAction.Print, ParserState.Ground);

            // global anwyhere rules
            foreach (var state in states)
            {
                table.Add(new[] {0x18, 0x1a, 0x99, 0x9a}, state, ParserAction.Execute, ParserState.Ground);
                table.Add(r(0x80, 0x90), state, ParserAction.Execute, ParserState.Ground);
                table.Add(r(0x90, 0x98), state, ParserAction.Execute, ParserState.Ground);
                table.Add(0x9c, state, ParserAction.Ignore, ParserState.Ground); // ST as terminator
                table.Add(0x1b, state, ParserAction.Clear, ParserState.Escape); // ESC
                table.Add(0x9d, state, ParserAction.OscStart, ParserState.OscString); // OSC
                table.Add(new[] {0x98, 0x9e, 0x9f}, state, ParserAction.Ignore, ParserState.SosPmApcString);
                table.Add(0x9b, state, ParserAction.Clear, ParserState.CsiEntry); // CSI
                table.Add(0x90, state, ParserAction.Clear, ParserState.DcsEntry); // DCS
            }

            // rules for executable and 0x7f
            table.Add(EXECUTABLES, ParserState.Ground, ParserAction.Execute, ParserState.Ground);
            table.Add(EXECUTABLES, ParserState.Escape, ParserAction.Execute, ParserState.Escape);
            table.Add(0x7f, ParserState.Escape, ParserAction.Ignore, ParserState.Escape);
            table.Add(EXECUTABLES, ParserState.OscString, ParserAction.Ignore, ParserState.OscString);
            table.Add(EXECUTABLES, ParserState.CsiEntry, ParserAction.Execute, ParserState.CsiEntry);
            table.Add(0x7f, ParserState.CsiEntry, ParserAction.Ignore, ParserState.CsiEntry);
            table.Add(EXECUTABLES, ParserState.CsiParam, ParserAction.Execute, ParserState.CsiParam);
            table.Add(0x7f, ParserState.CsiParam, ParserAction.Ignore, ParserState.CsiParam);
            table.Add(EXECUTABLES, ParserState.CsiIgnore, ParserAction.Execute, ParserState.CsiIgnore);
            table.Add(EXECUTABLES, ParserState.CsiIntermediate, ParserAction.Execute, ParserState.CsiIntermediate);
            table.Add(0x7f, ParserState.CsiIntermediate, ParserAction.Ignore, ParserState.CsiIntermediate);
            table.Add(EXECUTABLES, ParserState.EscapeIntermediate, ParserAction.Execute,
                ParserState.EscapeIntermediate);
            table.Add(0x7f, ParserState.EscapeIntermediate, ParserAction.Ignore, ParserState.EscapeIntermediate);
            // osc
            table.Add(0x5d, ParserState.Escape, ParserAction.OscStart, ParserState.OscString);
            table.Add(PRINTABLES, ParserState.OscString, ParserAction.OscPut, ParserState.OscString);
            table.Add(0x7f, ParserState.OscString, ParserAction.OscPut, ParserState.OscString);
            table.Add(new[] {0x9c, 0x1b, 0x18, 0x1a, 0x07}, ParserState.OscString, ParserAction.OscEnd,
                ParserState.Ground);
            table.Add(r(0x1c, 0x20), ParserState.OscString, ParserAction.Ignore, ParserState.OscString);
            // sos/pm/apc does nothing
            table.Add(new[] {0x58, 0x5e, 0x5f}, ParserState.Escape, ParserAction.Ignore, ParserState.SosPmApcString);
            table.Add(PRINTABLES, ParserState.SosPmApcString, ParserAction.Ignore, ParserState.SosPmApcString);
            table.Add(EXECUTABLES, ParserState.SosPmApcString, ParserAction.Ignore, ParserState.SosPmApcString);
            table.Add(0x9c, ParserState.SosPmApcString, ParserAction.Ignore, ParserState.Ground);
            table.Add(0x7f, ParserState.SosPmApcString, ParserAction.Ignore, ParserState.SosPmApcString);
            // csi entries
            table.Add(0x5b, ParserState.Escape, ParserAction.Clear, ParserState.CsiEntry);
            table.Add(r(0x40, 0x7f), ParserState.CsiEntry, ParserAction.CsiDispatch, ParserState.Ground);
            table.Add(r(0x30, 0x3a), ParserState.CsiEntry, ParserAction.Param, ParserState.CsiParam);
            table.Add(0x3b, ParserState.CsiEntry, ParserAction.Param, ParserState.CsiParam);
            table.Add(new[] {0x3c, 0x3d, 0x3e, 0x3f}, ParserState.CsiEntry, ParserAction.Collect, ParserState.CsiParam);
            table.Add(r(0x30, 0x3a), ParserState.CsiParam, ParserAction.Param, ParserState.CsiParam);
            table.Add(0x3b, ParserState.CsiParam, ParserAction.Param, ParserState.CsiParam);
            table.Add(r(0x40, 0x7f), ParserState.CsiParam, ParserAction.CsiDispatch, ParserState.Ground);
            table.Add(new[] {0x3a, 0x3c, 0x3d, 0x3e, 0x3f}, ParserState.CsiParam, ParserAction.Ignore,
                ParserState.CsiIgnore);
            table.Add(r(0x20, 0x40), ParserState.CsiIgnore, ParserAction.Ignore, ParserState.CsiIgnore);
            table.Add(0x7f, ParserState.CsiIgnore, ParserAction.Ignore, ParserState.CsiIgnore);
            table.Add(r(0x40, 0x7f), ParserState.CsiIgnore, ParserAction.Ignore, ParserState.Ground);
            table.Add(0x3a, ParserState.CsiEntry, ParserAction.Ignore, ParserState.CsiIgnore);
            table.Add(r(0x20, 0x30), ParserState.CsiEntry, ParserAction.Collect, ParserState.CsiIntermediate);
            table.Add(r(0x20, 0x30), ParserState.CsiIntermediate, ParserAction.Collect, ParserState.CsiIntermediate);
            table.Add(r(0x30, 0x40), ParserState.CsiIntermediate, ParserAction.Ignore, ParserState.CsiIgnore);
            table.Add(r(0x40, 0x7f), ParserState.CsiIntermediate, ParserAction.CsiDispatch, ParserState.Ground);
            table.Add(r(0x20, 0x30), ParserState.CsiParam, ParserAction.Collect, ParserState.CsiIntermediate);
            // escIntermediate
            table.Add(r(0x20, 0x30), ParserState.Escape, ParserAction.Collect, ParserState.EscapeIntermediate);
            table.Add(r(0x20, 0x30), ParserState.EscapeIntermediate, ParserAction.Collect,
                ParserState.EscapeIntermediate);
            table.Add(r(0x30, 0x7f), ParserState.EscapeIntermediate, ParserAction.EscDispatch, ParserState.Ground);
            table.Add(r(0x30, 0x50), ParserState.Escape, ParserAction.EscDispatch, ParserState.Ground);
            table.Add(r(0x51, 0x58), ParserState.Escape, ParserAction.EscDispatch, ParserState.Ground);
            table.Add(new[] {0x59, 0x5a, 0x5c}, ParserState.Escape, ParserAction.EscDispatch, ParserState.Ground);
            table.Add(r(0x60, 0x7f), ParserState.Escape, ParserAction.EscDispatch, ParserState.Ground);
            // dcs entry
            table.Add(0x50, ParserState.Escape, ParserAction.Clear, ParserState.DcsEntry);
            table.Add(EXECUTABLES, ParserState.DcsEntry, ParserAction.Ignore, ParserState.DcsEntry);
            table.Add(0x7f, ParserState.DcsEntry, ParserAction.Ignore, ParserState.DcsEntry);
            table.Add(r(0x1c, 0x20), ParserState.DcsEntry, ParserAction.Ignore, ParserState.DcsEntry);
            table.Add(r(0x20, 0x30), ParserState.DcsEntry, ParserAction.Collect, ParserState.DcsIntermediate);
            table.Add(0x3a, ParserState.DcsEntry, ParserAction.Ignore, ParserState.DcsIgnore);
            table.Add(r(0x30, 0x3a), ParserState.DcsEntry, ParserAction.Param, ParserState.DcsParam);
            table.Add(0x3b, ParserState.DcsEntry, ParserAction.Param, ParserState.DcsParam);
            table.Add(new[] {0x3c, 0x3d, 0x3e, 0x3f}, ParserState.DcsEntry, ParserAction.Collect, ParserState.DcsParam);
            table.Add(EXECUTABLES, ParserState.DcsIgnore, ParserAction.Ignore, ParserState.DcsIgnore);
            table.Add(r(0x20, 0x80), ParserState.DcsIgnore, ParserAction.Ignore, ParserState.DcsIgnore);
            table.Add(r(0x1c, 0x20), ParserState.DcsIgnore, ParserAction.Ignore, ParserState.DcsIgnore);
            table.Add(EXECUTABLES, ParserState.DcsParam, ParserAction.Ignore, ParserState.DcsParam);
            table.Add(0x7f, ParserState.DcsParam, ParserAction.Ignore, ParserState.DcsParam);
            table.Add(r(0x1c, 0x20), ParserState.DcsParam, ParserAction.Ignore, ParserState.DcsParam);
            table.Add(r(0x30, 0x3a), ParserState.DcsParam, ParserAction.Param, ParserState.DcsParam);
            table.Add(0x3b, ParserState.DcsParam, ParserAction.Param, ParserState.DcsParam);
            table.Add(new[] {0x3a, 0x3c, 0x3d, 0x3e, 0x3f}, ParserState.DcsParam, ParserAction.Ignore,
                ParserState.DcsIgnore);
            table.Add(r(0x20, 0x30), ParserState.DcsParam, ParserAction.Collect, ParserState.DcsIntermediate);
            table.Add(EXECUTABLES, ParserState.DcsIntermediate, ParserAction.Ignore, ParserState.DcsIntermediate);
            table.Add(0x7f, ParserState.DcsIntermediate, ParserAction.Ignore, ParserState.DcsIntermediate);
            table.Add(r(0x1c, 0x20), ParserState.DcsIntermediate, ParserAction.Ignore, ParserState.DcsIntermediate);
            table.Add(r(0x20, 0x30), ParserState.DcsIntermediate, ParserAction.Collect, ParserState.DcsIntermediate);
            table.Add(r(0x30, 0x40), ParserState.DcsIntermediate, ParserAction.Ignore, ParserState.DcsIgnore);
            table.Add(r(0x40, 0x7f), ParserState.DcsIntermediate, ParserAction.DcsHook, ParserState.DcsPassthrough);
            table.Add(r(0x40, 0x7f), ParserState.DcsParam, ParserAction.DcsHook, ParserState.DcsPassthrough);
            table.Add(r(0x40, 0x7f), ParserState.DcsEntry, ParserAction.DcsHook, ParserState.DcsPassthrough);
            table.Add(EXECUTABLES, ParserState.DcsPassthrough, ParserAction.DcsPut, ParserState.DcsPassthrough);
            table.Add(PRINTABLES, ParserState.DcsPassthrough, ParserAction.DcsPut, ParserState.DcsPassthrough);
            table.Add(0x7f, ParserState.DcsPassthrough, ParserAction.Ignore, ParserState.DcsPassthrough);
            table.Add(new[] {0x1b, 0x9c}, ParserState.DcsPassthrough, ParserAction.DcsUnhook, ParserState.Ground);
            table.Add(NonAsciiPrintable, ParserState.OscString, ParserAction.OscPut, ParserState.OscString);

            return table;
        }

        private static void EmptyExecuteHandler(byte code)
        {
        }

        private static ParsingState EmptyErrorHandler(ParsingState state)
        {
            return state;
        }

        public void SetPrintHandler(PrintHandler printHandler)
        {
            this.printHandler = printHandler;
        }

        public void ClearPrintHandler()
        {
            printHandler = PrintHandlerFallback;
        }

        public void SetExecuteHandler(byte flag, ExecuteHandler handler)
        {
            ExecuteHandlers[flag] = handler;
        }

        public void ClearExecuteHandler(byte flag)
        {
            ExecuteHandlers.Remove(flag);
        }

        public void SetExecuteHandlerFallback(Action<byte> fallback)
        {
            ExecuteHandlerFallback = fallback;
        }

        public void SetEscHandler(string flag, EscHandler callback)
        {
            EscHandlers[flag] = callback;
        }

        public void ClearEscHandler(string flag)
        {
            EscHandlers.Remove(flag);
        }

        public void SetEscHandlerFallback(EscHandler fallback)
        {
            EscHandlerFallback = fallback;
        }

        private IDisposable AddCsiHandler(byte flag, CsiHandler callback)
        {
            List<CsiHandler> list;

            if (!CsiHandlers.ContainsKey(flag))
            {
                list = new List<CsiHandler>();
                CsiHandlers[flag] = list;
            }
            else
            {
                list = CsiHandlers[flag];
            }

            list.Add(callback);
            return new CsiHandlerRemover
            {
                Container = list,
                ToRemove = callback
            };
        }

        public void SetCsiHandler(char flag, CsiHandler callback)
        {
            CsiHandlers[(byte) flag] = new List<CsiHandler> {callback};
        }

        public void ClearCsiHandler(byte flag)
        {
            CsiHandlers.Remove(flag);
        }

        public void SetCsiHandlerFallback(Action<string, int[], int> fallback)
        {
            CsiHandlerFallback = fallback;
        }

        private IDisposable AddOscHandler(int identifier, OscHandler callback)
        {
            List<OscHandler> list;

            if (!OscHandlers.ContainsKey(identifier))
            {
                list = new List<OscHandler>();
                OscHandlers[identifier] = list;
            }
            else
            {
                list = OscHandlers[identifier];
            }

            list.Add(callback);
            return new OscHandlerRemover
            {
                Container = list,
                ToRemove = callback
            };
        }

        public void SetOscHandler(int identifier, OscHandler callback)
        {
            OscHandlers[identifier] = new List<OscHandler> {callback};
        }

        public void ClearOscHandler(int identifier)
        {
            OscHandlers.Remove(identifier);
        }

        public void SetOscHandlerFallback(Action<int, string> fallback)
        {
            OscHandlerFallback = fallback;
        }

        public void SetDcsHandler(string flag, IDcsHandler handler)
        {
            DcsHandlers[flag] = handler;
        }

        public void ClearDcsHandler(string flag)
        {
            DcsHandlers.Remove(flag);
        }

        public void SetDcsHandlerFallback(IDcsHandler fallback)
        {
            DcsHandlerFallback = fallback;
        }

        public void SetErrorHandler(Func<ParsingState, ParsingState> errorHandler)
        {
            ErrorHandler = errorHandler;
        }

        public void ClearErrorHandler()
        {
            ErrorHandler = EmptyErrorHandler;
        }

        public void Reset()
        {
            currentState = initialState;
            _osc = "";
            _pars.Clear();
            _pars.Add(0);
            _collect = "";
            ActiveDcsHandler = null;
            PrintStateReset();
        }

        public unsafe void Parse(byte* data, int len)
        {
            byte code = 0;
            var transition = 0;
            var error = false;
            var currentState = this.currentState;
            var print = -1;
            var dcs = -1;
            var osc = _osc;
            var collect = _collect;
            var pars = _pars;
            var dcsHandler = ActiveDcsHandler;

            // process input string
            for (var i = 0; i < len; ++i)
            {
                code = data[i];

#if false
				// shortcut for most chars (print action)
				if (currentState == ParserState.Ground && code > 0x1f && code < 0x80) {
					print = (~print != 0) ? print : i;
					do { i++; } while (i < len && data [i] > 0x1f && data [i] < 0x80);
					i--;
					continue;
				}
#else
                // This version eliminates the check for < 0x80, as we allow any UTF8 sequences.
                if (currentState == ParserState.Ground && code > 0x1f)
                {
                    print = ~print != 0 ? print : i;
                    do
                    {
                        i++;
                    } while (i < len && data[i] > 0x1f);

                    i--;
                    continue;
                }

#endif

                // shorcut for CSI params
                if (currentState == ParserState.CsiParam && code > 0x2f && code < 0x39)
                {
                    pars[pars.Count - 1] = pars[pars.Count - 1] * 10 + code - 48;
                    continue;
                }

                // Normal transition and action lookup
                transition = table[((int) currentState << 8) | (code < 0xa0 ? code : NonAsciiPrintable)];
                var action = (ParserAction) (transition >> 4);
                switch (action)
                {
                    case ParserAction.Print:
                        print = ~print != 0 ? print : i;
                        break;
                    case ParserAction.Execute:
                        if (~print != 0)
                        {
                            printHandler(data, print, i);
                            print = -1;
                        }

                        if (ExecuteHandlers.TryGetValue(code, out var callback))
                            callback();
                        else
                            ExecuteHandlerFallback(code);
                        break;
                    case ParserAction.Ignore:
                        // handle leftover print or dcs chars
                        if (~print != 0)
                        {
                            printHandler(data, print, i);
                            print = -1;
                        }
                        else if (~dcs != 0)
                        {
                            dcsHandler?.Put(data, dcs, i);
                            dcs = -1;
                        }

                        break;
                    case ParserAction.Error:
                        // chars higher than 0x9f are handled by this action
                        // to keep the transition table small
                        if (code > 0x9f)
                            switch (currentState)
                            {
                                case ParserState.Ground:
                                    print = ~print != 0 ? print : i;
                                    break;
                                case ParserState.CsiIgnore:
                                    transition |= (int) ParserState.CsiIgnore;
                                    break;
                                case ParserState.DcsIgnore:
                                    transition |= (int) ParserState.DcsIgnore;
                                    break;
                                case ParserState.DcsPassthrough:
                                    dcs = ~dcs != 0 ? dcs : i;
                                    transition |= (int) ParserState.DcsPassthrough;
                                    break;
                                default:
                                    error = true;
                                    break;
                            }
                        else
                            error = true;

                        // if we end up here a real error happened
                        if (error)
                        {
                            var inject = ErrorHandler(new ParsingState
                            {
                                Position = i,
                                Code = code,
                                CurrentState = currentState,
                                Print = print,
                                Dcs = dcs,
                                Osc = osc,
                                Collect = collect
                            });
                            if (inject.Abort)
                                return;
                            error = false;
                        }

                        break;
                    case ParserAction.CsiDispatch:
                        // Trigger CSI handler
                        if (CsiHandlers.TryGetValue(code, out var csiHandlers))
                        {
                            var jj = csiHandlers.Count - 1;
                            for (; jj >= 0; jj--) csiHandlers[jj](pars.ToArray(), collect);
                        }
                        else
                        {
                            CsiHandlerFallback(collect, pars.ToArray(), code);
                        }

                        break;
                    case ParserAction.Param:
                        if (code == 0x3b)
                            pars.Add(0);
                        else
                            pars[pars.Count - 1] = pars[pars.Count - 1] * 10 + code - 48;
                        break;
                    case ParserAction.Collect:
                        // AUDIT: make collect a ustring
                        collect += (char) code;
                        break;
                    case ParserAction.EscDispatch:
                        if (EscHandlers.TryGetValue(collect + (char) code, out var ehandler))
                            ehandler(collect, code);
                        else
                            EscHandlerFallback(collect, code);
                        break;
                    case ParserAction.Clear:
                        if (~print != 0)
                        {
                            printHandler(data, print, i);
                            print = -1;
                        }

                        osc = "";
                        pars.Clear();
                        pars.Add(0);
                        collect = "";
                        dcs = -1;
                        PrintStateReset();
                        break;
                    case ParserAction.DcsHook:
                        if (DcsHandlers.TryGetValue(collect + (char) code, out dcsHandler))
                            dcsHandler.Hook(collect, pars.ToArray(), code);
                        else
                            DcsHandlerFallback.Hook(collect, pars.ToArray(), code);
                        break;
                    case ParserAction.DcsPut:
                        dcs = ~dcs != 0 ? dcs : i;
                        break;
                    case ParserAction.DcsUnhook:
                        if (dcsHandler != null)
                        {
                            if (~dcs != 0)
                                dcsHandler.Put(data, dcs, i);
                            dcsHandler.Unhook();
                            dcsHandler = null;
                        }

                        if (code == 0x1b)
                            transition |= (int) ParserState.Escape;
                        osc = "";
                        pars.Clear();
                        pars.Add(0);
                        collect = "";
                        dcs = -1;
                        PrintStateReset();
                        break;
                    case ParserAction.OscStart:
                        if (~print != 0)
                        {
                            printHandler(data, print, i);
                            print = -1;
                        }

                        osc = "";
                        break;
                    case ParserAction.OscPut:
                        for (var j = i;; j++)
                            if (j > len || data[j] < 0x20 || data[j] > 0x7f && data[j] < 0x9f)
                            {
                                var block = new byte [j - (i + 1)];
                                for (var k = i + 1; k < j; k++)
                                    block[k - i - 1] = data[k];
                                // TODO: Audit, the code below as I would not like the code below to abort on invalid UTF8
                                // So we need a way of producing memory blocks.
                                osc += Encoding.UTF8.GetString(block);

                                i = j - 1;
                                break;
                            }

                        break;
                    case ParserAction.OscEnd:
                        if (osc != "" && code != 0x18 && code != 0x1a)
                        {
                            // NOTE: OSC subparsing is not part of the original parser
                            // we do basic identifier parsing here to offer a jump table for OSC as well
                            var idx = osc.IndexOf(';');
                            if (idx == -1)
                            {
                                OscHandlerFallback(-1, osc); // this is an error mal-formed OSC
                            }
                            else
                            {
                                // Note: NaN is not handled here
                                // either catch it with the fallback handler
                                // or with an explicit NaN OSC handler
                                var identifier = 0;
                                int.TryParse(osc.Substring(0, idx), out identifier);
                                var content = osc.Substring(idx + 1);
                                // Trigger OSC handler
                                var c = -1;
                                if (OscHandlers.TryGetValue(identifier, out var ohandlers))
                                {
                                    c = ohandlers.Count - 1;
                                    for (; c >= 0; c--)
                                    {
                                        ohandlers[c](content);
                                        break;
                                    }
                                }

                                if (c < 0)
                                    OscHandlerFallback(identifier, content);
                            }
                        }

                        if (code == 0x1b)
                            transition |= (int) ParserState.Escape;
                        osc = "";
                        pars.Clear();
                        pars.Add(0);
                        collect = "";
                        dcs = -1;
                        PrintStateReset();
                        break;
                }

                currentState = (ParserState) (transition & 15);
            }

            // push leftover pushable buffers to terminal
            if (currentState == ParserState.Ground && ~print != 0)
                printHandler(data, print, len);
            else if (currentState == ParserState.DcsPassthrough && ~dcs != 0 && dcsHandler != null)
                dcsHandler.Put(data, dcs, len);

            // save non pushable buffers
            _osc = osc;
            _collect = collect;
            _pars = pars;

            // save active dcs handler reference
            ActiveDcsHandler = dcsHandler;

            // save state
            this.currentState = currentState;
        }


        private enum ParserAction
        {
            Ignore,
            Error,
            Print,
            Execute,
            OscStart,
            OscPut,
            OscEnd,
            CsiDispatch,
            Param,
            Collect,
            EscDispatch,
            Clear,
            DcsHook,
            DcsPut,
            DcsUnhook
        }

        private class TransitionTable
        {
            // data is packed like this:
            // currentState << 8 | characterCode  -->  action << 4 | nextState
            private readonly byte[] table;

            public TransitionTable(int length)
            {
                table = new byte [length];
            }

            public byte this[int idx] => table[idx];

            public void Add(int code, ParserState state, ParserAction action, ParserState next = ParserState.Invalid)
            {
                table[((int) state << 8) | code] =
                    (byte) (((int) action << 4) | (int) (next == ParserState.Invalid ? state : next));
            }

            public void Add(int[] codes, ParserState state, ParserAction action, ParserState next = ParserState.Invalid)
            {
                foreach (var c in codes)
                    Add(c, state, action, next);
            }
        }

        private class CsiHandlerRemover : IDisposable
        {
            public List<CsiHandler> Container;
            public CsiHandler ToRemove;

            void IDisposable.Dispose()
            {
                Container.Remove(ToRemove);
            }
        }

        private class OscHandlerRemover : IDisposable
        {
            public List<OscHandler> Container;
            public OscHandler ToRemove;

            void IDisposable.Dispose()
            {
                Container.Remove(ToRemove);
            }
        }
    }
}