using System;

namespace M5x.Streams
{
    /// <summary>
    ///     Event that is used to report the progress for a stream operation.
    /// </summary>
    public class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Number of bytes that have been read.
        /// </summary>
        public long BytesRead;

        /// <summary>
        ///     Total size that needs to be read
        /// </summary>
        public long Length;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressChangedEventArgs" /> class.
        /// </summary>
        /// <param name="bytesRead">The bytes read.</param>
        /// <param name="length">The length.</param>
        public ProgressChangedEventArgs(long bytesRead, long length)
        {
            BytesRead = bytesRead;
            Length = length;
        }
    }
}