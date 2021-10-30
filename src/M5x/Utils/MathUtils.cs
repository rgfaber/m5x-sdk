using System.IO;
using System.Text.RegularExpressions;

namespace M5x.Utils
{
    /// <summary>
    /// </summary>
    public static class MathUtils
    {
        public static byte[] ToByteArray (this decimal src)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(src);
            return stream.ToArray();
        }
        
        
        /// <summary>
        ///     Determines whether the specified STR text entry is numeric.
        /// </summary>
        /// <param name="strTextEntry">The STR text entry.</param>
        /// <returns>
        ///     <c>true</c> if the specified STR text entry is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(string strTextEntry)
        {
            var objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strTextEntry);
        }
    }
}