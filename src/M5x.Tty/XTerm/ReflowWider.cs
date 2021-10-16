﻿using System;
using System.Collections.Generic;
using M5x.Tty.XTerm.Utils;

namespace M5x.Tty.XTerm
{
    internal class ReflowWider : ReflowStrategy
    {
        public ReflowWider(Buffer buffer) : base(buffer)
        {
        }

        public override void Reflow(int newCols, int newRows, int oldCols, int oldRows)
        {
            var toRemove = GetLinesToRemove(Buffer.Lines, oldCols, newCols, Buffer.YBase + Buffer.Y, CharData.Null);
            if (toRemove.Length > 0)
            {
                var newLayoutResult = CreateNewLayout(Buffer.Lines, toRemove);
                ApplyNewLayout(Buffer.Lines, newLayoutResult.Layout);
                AdjustViewport(newCols, newRows, newLayoutResult.RemovedCount);
            }
        }

        /// <summary>
        ///     Evaluates and returns indexes to be removed after a reflow larger occurs. Lines will be removed
        ///     when a wrapped line unwraps.
        /// </summary>
        /// <param name="lines">The buffer lines</param>
        /// <param name="oldCols">The columns before resize</param>
        /// <param name="newCols">The columns after resize</param>
        /// <param name="bufferAbsoluteY"></param>
        /// <param name="nullCharacter"></param>
        private int[] GetLinesToRemove(CircularList<BufferLine> lines, int oldCols, int newCols, int bufferAbsoluteY,
            CharData nullCharacter)
        {
            // Gather all BufferLines that need to be removed from the Buffer here so that they can be
            // batched up and only committed once
            var toRemove = new List<int>();

            for (var y = 0; y < lines.Length - 1; y++)
            {
                // Check if this row is wrapped
                var i = y;
                var nextLine = lines[++i];
                if (!nextLine.IsWrapped) continue;

                // Check how many lines it's wrapped for
                var wrappedLines = new List<BufferLine>(lines.Length - y);
                wrappedLines.Add(lines[y]);
                while (i < lines.Length && nextLine.IsWrapped)
                {
                    wrappedLines.Add(nextLine);
                    nextLine = lines[++i];
                }

                // If these lines contain the cursor don't touch them, the program will handle fixing up wrapped
                // lines with the cursor
                if (bufferAbsoluteY >= y && bufferAbsoluteY < i)
                {
                    y += wrappedLines.Count - 1;
                    continue;
                }

                // Copy buffer data to new locations
                var destLineIndex = 0;
                var destCol = GetWrappedLineTrimmedLength(Buffer.Lines, destLineIndex, oldCols);
                var srcLineIndex = 1;
                var srcCol = 0;
                while (srcLineIndex < wrappedLines.Count)
                {
                    var srcTrimmedTineLength = GetWrappedLineTrimmedLength(wrappedLines, srcLineIndex, oldCols);
                    var srcRemainingCells = srcTrimmedTineLength - srcCol;
                    var destRemainingCells = newCols - destCol;
                    var cellsToCopy = Math.Min(srcRemainingCells, destRemainingCells);

                    wrappedLines[destLineIndex].CopyCellsFrom(wrappedLines[srcLineIndex], srcCol, destCol, cellsToCopy);

                    destCol += cellsToCopy;
                    if (destCol == newCols)
                    {
                        destLineIndex++;
                        destCol = 0;
                    }

                    srcCol += cellsToCopy;
                    if (srcCol == srcTrimmedTineLength)
                    {
                        srcLineIndex++;
                        srcCol = 0;
                    }

                    // Make sure the last cell isn't wide, if it is copy it to the current dest
                    if (destCol == 0 && destLineIndex != 0)
                        if (wrappedLines[destLineIndex - 1].GetWidth(newCols - 1) == 2)
                        {
                            wrappedLines[destLineIndex].CopyCellsFrom(wrappedLines[destLineIndex - 1], newCols - 1,
                                destCol++, 1);
                            // Null out the end of the last row
                            wrappedLines[destLineIndex - 1].ReplaceCells(newCols - 1, 1, nullCharacter);
                        }
                }

                // Clear out remaining cells or fragments could remain;
                wrappedLines[destLineIndex].ReplaceCells(destCol, newCols, nullCharacter);

                // Work backwards and remove any rows at the end that only contain null cells
                var countToRemove = 0;
                for (var ix = wrappedLines.Count - 1; ix > 0; ix--)
                    if (ix > destLineIndex || wrappedLines[ix].GetTrimmedLength() == 0)
                        countToRemove++;
                    else
                        break;

                if (countToRemove > 0)
                {
                    toRemove.Add(y + wrappedLines.Count - countToRemove); // index
                    toRemove.Add(countToRemove);
                }

                y += wrappedLines.Count - 1;
            }

            return toRemove.ToArray();
        }

        private LayoutResult CreateNewLayout(CircularList<BufferLine> lines, int[] toRemove)
        {
            var layout = new CircularList<int>(lines.Length);

            // First iterate through the list and get the actual indexes to use for rows
            var nextToRemoveIndex = 0;
            var nextToRemoveStart = toRemove[nextToRemoveIndex];
            var countRemovedSoFar = 0;

            for (var i = 0; i < lines.Length; i++)
                if (nextToRemoveStart == i)
                {
                    var countToRemove = toRemove[++nextToRemoveIndex];

                    // Tell markers that there was a deletion
                    //lines.onDeleteEmitter.fire ({
                    //	index: i - countRemovedSoFar,
                    //	amount: countToRemove
                    //});

                    i += countToRemove - 1;
                    countRemovedSoFar += countToRemove;

                    nextToRemoveStart = int.MaxValue;
                    if (nextToRemoveIndex < toRemove.Length - 1)
                        nextToRemoveStart = toRemove[++nextToRemoveIndex];
                }
                else
                {
                    layout.Push(i);
                }

            return new LayoutResult
            {
                Layout = layout.ToArray(),
                RemovedCount = countRemovedSoFar
            };
        }

        private void ApplyNewLayout(CircularList<BufferLine> lines, int[] newLayout)
        {
            var newLayoutLines = new CircularList<BufferLine>(lines.Length);

            for (var i = 0; i < newLayout.Length; i++) newLayoutLines.Push(lines[newLayout[i]]);

            // Rearrange the list
            for (var i = 0; i < newLayoutLines.Length; i++) lines[i] = newLayoutLines[i];

            lines.Length = newLayout.Length;
        }

        private void AdjustViewport(int newCols, int newRows, int countRemoved)
        {
            var viewportAdjustments = countRemoved;
            while (viewportAdjustments-- > 0)
                if (Buffer.YBase == 0)
                {
                    if (Buffer.Y > 0) Buffer.Y--;

                    if (Buffer.Lines.Length < newRows) // Add an extra row at the bottom of the viewport
                        Buffer.Lines.Push(new BufferLine(newCols, CharData.Null));
                }
                else
                {
                    if (Buffer.YDisp == Buffer.YBase) Buffer.YDisp--;

                    Buffer.YBase--;
                }

            Buffer.SavedY = Math.Max(Buffer.SavedY - countRemoved, 0);
        }

        private struct LayoutResult
        {
            public int[] Layout;
            public int RemovedCount;
        }
    }
}