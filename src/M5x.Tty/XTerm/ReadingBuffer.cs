using System;

namespace M5x.Tty.XTerm
{
    /// <summary>
    ///     Buffer for processing input
    /// </summary>
    /// <remarks>
    ///     Because data might not be complete, we need to put back data that we read to process on
    ///     a future read.  To prepare for reading, on every call to parse, the prepare method is
    ///     given the new buffer to read from.
    ///     the `hasNext` describes whether there is more data left on the buffer, and `bytesLeft`
    ///     returnes the number of bytes left.   The `getNext` method fetches either the next
    ///     value from the putback buffer, or when it is empty, it returns it from the buffer that
    ///     was passed during prepare.
    ///     Additionally, the terminal parser needs to reset the parser state on demand, and
    ///     that is surfaced via reset
    /// </remarks>
    internal class ReadingBuffer
    {
        private unsafe byte* buffer;
        private int bufferStart;
        private int index;
        private byte[] putbackBuffer = new byte [0];
        private int totalCount;

        public unsafe void Prepare(byte* data, int start, int length)
        {
            buffer = data;
            bufferStart = start;

            index = 0;
            totalCount = putbackBuffer.Length + length;
        }

        public int BytesLeft()
        {
            return totalCount - index;
        }

        public bool HasNext()
        {
            return index < totalCount;
        }

        public unsafe byte GetNext()
        {
            byte val;
            if (index < putbackBuffer.Length) // grab from putback buffer
                val = putbackBuffer[index];
            else // grab from the prepared buffer
                val = buffer[bufferStart + (index - putbackBuffer.Length)];

            index++;
            return val;
        }

        /// <summary>
        ///     Puts back code and the remainder of the buffer
        /// </summary>
        public void Putback(byte code)
        {
            var left = BytesLeft();
            var newPutback = new byte[left + 1];
            newPutback[0] = code;

            for (var i = 0; i < left; i++) newPutback[i + 1] = GetNext();

            putbackBuffer = newPutback;
        }

        public unsafe void Done()
        {
            if (index < putbackBuffer.Length)
            {
                var newPutback = new byte [putbackBuffer.Length - index];
                Array.Copy(putbackBuffer, index, newPutback, 0, newPutback.Length);
                putbackBuffer = newPutback;
            }
            else
            {
                putbackBuffer = new byte [0];
            }

            buffer = null;
        }

        public void Reset()
        {
            putbackBuffer = new byte [0];
            index = 0;
        }
    }
}