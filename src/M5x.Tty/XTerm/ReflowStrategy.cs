using System.Collections.Generic;
using M5x.Tty.XTerm.Utils;

namespace M5x.Tty.XTerm
{
    internal abstract class ReflowStrategy
    {
        protected ReflowStrategy(Buffer buffer)
        {
            Buffer = buffer;
        }

        public Buffer Buffer { get; }

        public abstract void Reflow(int newCols, int newRows, int oldCols, int oldRows);

        protected static int GetWrappedLineTrimmedLength(CircularList<BufferLine> lines, int row, int cols)
        {
            return GetWrappedLineTrimmedLength(lines[row], row == lines.Length - 1 ? null : lines[row + 1], cols);
        }

        protected static int GetWrappedLineTrimmedLength(List<BufferLine> lines, int row, int cols)
        {
            return GetWrappedLineTrimmedLength(lines[row], row == lines.Count - 1 ? null : lines[row + 1], cols);
        }

        protected static int GetWrappedLineTrimmedLength(BufferLine line, BufferLine nextLine, int cols)
        {
            // If this is the last row in the wrapped line, get the actual trimmed length
            if (nextLine == null) return line.GetTrimmedLength();

            // Detect whether the following line starts with a wide character and the end of the current line
            // is null, if so then we can be pretty sure the null character should be excluded from the line
            // length]
            var endsInNull = !line.HasContent(cols - 1) && line.GetWidth(cols - 1) == 1;
            var followingLineStartsWithWide = nextLine.GetWidth(0) == 2;

            if (endsInNull && followingLineStartsWithWide) return cols - 1;

            return cols;
        }
    }
}