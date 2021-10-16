using System;
using System.Threading;
using M5x.Tty.Core;

namespace M5x.Tty.ConsoleDrivers.FakeDriver
{
    /// <summary>
    ///     Mainloop intended to be used with the .NET System.Console API, and can
    ///     be used on Windows and Unix, it is cross platform but lacks things like
    ///     file descriptor monitoring.
    /// </summary>
    /// <remarks>
    ///     This implementation is used for FakeDriver.
    /// </remarks>
    public class FakeMainLoop : IMainLoopDriver
    {
        private readonly Func<ConsoleKeyInfo> consoleKeyReaderFn;

        private readonly AutoResetEvent keyReady = new(false);
        private readonly AutoResetEvent waitForProbe = new(false);

        /// <summary>
        ///     Invoked when a Key is pressed.
        /// </summary>
        public Action<ConsoleKeyInfo> KeyPressed;

        private ConsoleKeyInfo? keyResult;
        private MainLoop mainLoop;

        /// <summary>
        ///     Initializes the class.
        /// </summary>
        /// <remarks>
        ///     Passing a consoleKeyReaderfn is provided to support unit test scenarios.
        /// </remarks>
        /// <param name="consoleKeyReaderFn">The method to be called to get a key from the console.</param>
        public FakeMainLoop(Func<ConsoleKeyInfo> consoleKeyReaderFn = null)
        {
            if (consoleKeyReaderFn == null) throw new ArgumentNullException("key reader function must be provided.");
            this.consoleKeyReaderFn = consoleKeyReaderFn;
        }

        void IMainLoopDriver.Setup(MainLoop mainLoop)
        {
            this.mainLoop = mainLoop;
            var readThread = new Thread(WindowsKeyReader);
            readThread.Start();
        }

        void IMainLoopDriver.Wakeup()
        {
        }

        bool IMainLoopDriver.EventsPending(bool wait)
        {
            var now = DateTime.UtcNow.Ticks;

            int waitTimeout;
            if (mainLoop.timeouts.Count > 0)
            {
                waitTimeout = (int) ((mainLoop.timeouts.Keys[0] - now) / TimeSpan.TicksPerMillisecond);
                if (waitTimeout < 0)
                    return true;
            }
            else
            {
                waitTimeout = -1;
            }

            if (!wait)
                waitTimeout = 0;

            keyResult = null;
            waitForProbe.Set();
            keyReady.WaitOne(waitTimeout);
            return keyResult.HasValue;
        }

        void IMainLoopDriver.MainIteration()
        {
            if (keyResult.HasValue)
            {
                KeyPressed?.Invoke(keyResult.Value);
                keyResult = null;
            }
        }

        private void WindowsKeyReader()
        {
            while (true)
            {
                waitForProbe.WaitOne();
                keyResult = consoleKeyReaderFn();
                keyReady.Set();
            }
        }
    }
}