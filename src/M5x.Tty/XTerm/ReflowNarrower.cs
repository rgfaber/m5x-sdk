﻿using System;
using System.Collections.Generic;
using M5x.Tty.XTerm.Utils;

namespace M5x.Tty.XTerm
{
    internal class ReflowNarrower : ReflowStrategy
    {
        public ReflowNarrower(Buffer buffer) : base(buffer)
        {
        }

        public override void Reflow(int newCols, int newRows, int oldCols, int oldRows)
        {
            // Gather all BufferLines that need to be inserted into the Buffer here so that they can be
            // batched up and only committed once
            var toInsert = new List<InsertionSet>();
            var countToInsert = 0;

            // Go backwards as many lines may be trimmed and this will avoid considering them
            for (var y = Buffer.Lines.Length - 1; y >= 0; y--)
            {
                // Check whether this line is a problem or not, if not skip it
                var nextLine = Buffer.Lines[y];
                var lineLength = nextLine.GetTrimmedLength();
                if (!nextLine.IsWrapped && lineLength <= newCols) continue;

                // Gather wrapped lines and adjust y to be the starting line
                var wrappedLines = new List<BufferLine>();
                wrappedLines.Add(nextLine);
                while (nextLine.IsWrapped && y > 0)
                {
                    nextLine = Buffer.Lines[--y];
                    wrappedLines.Insert(0, nextLine);
                }

                // If these lines contain the cursor don't touch them, the program will handle fixing up
                // wrapped lines with the cursor
                var absoluteY = Buffer.YBase + Buffer.Y;

                if (absoluteY >= y && absoluteY < y + wrappedLines.Count) continue;

                var lastLineLength = wrappedLines[wrappedLines.Count - 1].GetTrimmedLength();
                var destLineLengths = GetNewLineLengths(wrappedLines, oldCols, newCols);
                var linesToAdd = destLineLengths.Length - wrappedLines.Count;

                int trimmedLines;
                if (Buffer.YBase == 0 &&
                    Buffer.Y != Buffer.Lines.Length - 1) // If the top section of the buffer is not yet filled
                    trimmedLines = Math.Max(0, Buffer.Y - Buffer.Lines.MaxLength + linesToAdd);
                else
                    trimmedLines = Math.Max(0, Buffer.Lines.Length - Buffer.Lines.MaxLength + linesToAdd);

                // Add the new lines
                var newLines = new List<BufferLine>();
                for (var i = 0; i < linesToAdd; i++)
                {
                    var newLine = Buffer.GetBlankLine(CharData.DefaultAttr, true);
                    newLines.Add(newLine);
                }

                if (newLines.Count > 0)
                {
                    toInsert.Add(new InsertionSet
                    {
                        Start = y + wrappedLines.Count + countToInsert,
                        Lines = newLines.ToArray()
                    });

                    countToInsert += newLines.Count;
                }

                newLines.ForEach(l => wrappedLines.Add(l));

                // Copy buffer data to new locations, this needs to happen backwards to do in-place
                var destLineIndex = destLineLengths.Length - 1; // Math.floor(cellsNeeded / newCols);
                var destCol = destLineLengths[destLineIndex]; // cellsNeeded % newCols;
                if (destCol == 0)
                {
                    destLineIndex--;
                    destCol = destLineLengths[destLineIndex];
                }

                var srcLineIndex = wrappedLines.Count - linesToAdd - 1;
                var srcCol = lastLineLength;
                while (srcLineIndex >= 0)
                {
                    var cellsToCopy = Math.Min(srcCol, destCol);
                    wrappedLines[destLineIndex].CopyCellsFrom(wrappedLines[srcLineIndex], srcCol - cellsToCopy,
                        destCol - cellsToCopy, cellsToCopy);
                    destCol -= cellsToCopy;
                    if (destCol == 0)
                    {
                        destLineIndex--;
                        if (destLineIndex >= 0)
                            destCol = destLineLengths[destLineIndex];
                    }

                    srcCol -= cellsToCopy;
                    if (srcCol == 0)
                    {
                        srcLineIndex--;
                        var wrappedLinesIndex = Math.Max(srcLineIndex, 0);
                        srcCol = GetWrappedLineTrimmedLength(wrappedLines, wrappedLinesIndex, oldCols);
                    }
                }

                // Null out the end of the line ends if a wide character wrapped to the following line
                for (var i = 0; i < wrappedLines.Count; i++)
                    if (destLineLengths[i] < newCols)
                        wrappedLines[i][destLineLengths[i]] = CharData.Null;

                // Adjust viewport as needed
                var viewportAdjustments = linesToAdd - trimmedLines;
                while (viewportAdjustments-- > 0)
                    if (Buffer.YBase == 0)
                    {
                        if (Buffer.Y < newRows - 1)
                        {
                            Buffer.Y++;
                            Buffer.Lines.Pop();
                        }
                        else
                        {
                            Buffer.YBase++;
                            Buffer.YDisp++;
                        }
                    }
                    else
                    {
                        // Ensure ybase does not exceed its maximum value
                        if (Buffer.YBase < Math.Min(Buffer.Lines.MaxLength, Buffer.Lines.Length + countToInsert) -
                            newRows)
                        {
                            if (Buffer.YBase == Buffer.YDisp) Buffer.YDisp++;

                            Buffer.YBase++;
                        }
                    }

                Buffer.SavedY = Math.Min(Buffer.SavedY + linesToAdd, Buffer.YBase + newRows - 1);
            }

            Rearrange(toInsert, countToInsert);
        }

        private void Rearrange(List<InsertionSet> toInsert, int countToInsert)
        {
            // Rearrange lines in the buffer if there are any insertions, this is done at the end rather
            // than earlier so that it's a single O(n) pass through the buffer, instead of O(n^2) from many
            // costly calls to CircularList.splice.
            if (toInsert.Count > 0)
            {
                // Record buffer insert events and then play them back backwards so that the indexes are
                // correct
                var insertEvents = new List<int>();

                // Record original lines so they don't get overridden when we rearrange the list
                var originalLines = new CircularList<BufferLine>(Buffer.Lines.MaxLength);
                for (var i = 0; i < Buffer.Lines.Length; i++) originalLines.Push(Buffer.Lines[i]);

                var originalLinesLength = Buffer.Lines.Length;

                var originalLineIndex = originalLinesLength - 1;
                var nextToInsertIndex = 0;
                var nextToInsert = toInsert[nextToInsertIndex];
                Buffer.Lines.Length = Math.Min(Buffer.Lines.MaxLength, Buffer.Lines.Length + countToInsert);

                var countInsertedSoFar = 0;
                for (var i = Math.Min(Buffer.Lines.MaxLength - 1, originalLinesLength + countToInsert - 1); i >= 0; i--)
                    if (!nextToInsert.IsNull && nextToInsert.Start > originalLineIndex + countInsertedSoFar)
                    {
                        // Insert extra lines here, adjusting i as needed
                        for (var nextI = nextToInsert.Lines.Length - 1; nextI >= 0; nextI--)
                        {
                            if (i < 0) // if we reflow and the content has to be scrolled back past the beginning
                                // of the buffer then we end up loosing those lines
                                break;

                            Buffer.Lines[i--] = nextToInsert.Lines[nextI];
                        }

                        i++;

                        // Create insert events for later
                        //insertEvents.Add ({
                        //	index: originalLineIndex + 1,
                        //	amount: nextToInsert.newLines.length
                        //});

                        countInsertedSoFar += nextToInsert.Lines.Length;
                        if (nextToInsertIndex < toInsert.Count - 1)
                            nextToInsert = toInsert[++nextToInsertIndex];
                        else
                            nextToInsert = InsertionSet.Null;
                    }
                    else
                    {
                        Buffer.Lines[i] = originalLines[originalLineIndex--];
                    }

                /*
                // Update markers
                let insertCountEmitted = 0;
                for (let i = insertEvents.length - 1; i >= 0; i--) {
                    insertEvents [i].index += insertCountEmitted;
                    this.lines.onInsertEmitter.fire (insertEvents [i]);
                    insertCountEmitted += insertEvents [i].amount;
                }

                const amountToTrim = Math.max (0, originalLinesLength + countToInsert - this.lines.maxLength);
                if (amountToTrim > 0) {
                    this.lines.onTrimEmitter.fire (amountToTrim);
                }
                */
            }
        }

        /// <summary>
        ///     Gets the new line lengths for a given wrapped line. The purpose of this function it to pre-
        ///     compute the wrapping points since wide characters may need to be wrapped onto the following line.
        ///     This function will return an array of numbers of where each line wraps to, the resulting array
        ///     will only contain the values `newCols` (when the line does not end with a wide character) and
        ///     `newCols - 1` (when the line does end with a wide character), except for the last value which
        ///     will contain the remaining items to fill the line.
        ///     Calling this with a `newCols` value of `1` will lock up.
        /// </summary>
        private int[] GetNewLineLengths(List<BufferLine> wrappedLines, int oldCols, int newCols)
        {
            var newLineLengths = new List<int>();

            var cellsNeeded = 0;
            for (var i = 0; i < wrappedLines.Count; i++)
                cellsNeeded += GetWrappedLineTrimmedLength(wrappedLines, i, oldCols);

            // Use srcCol and srcLine to find the new wrapping point, use that to get the cellsAvailable and
            // linesNeeded
            var srcCol = 0;
            var srcLine = 0;
            var cellsAvailable = 0;
            while (cellsAvailable < cellsNeeded)
            {
                if (cellsNeeded - cellsAvailable < newCols)
                {
                    // Add the final line and exit the loop
                    newLineLengths.Add(cellsNeeded - cellsAvailable);
                    break;
                }

                srcCol += newCols;
                var oldTrimmedLength = GetWrappedLineTrimmedLength(wrappedLines, srcLine, oldCols);
                if (srcCol > oldTrimmedLength)
                {
                    srcCol -= oldTrimmedLength;
                    srcLine++;
                }

                var endsWithWide = wrappedLines[srcLine].GetWidth(srcCol - 1) == 2;
                if (endsWithWide) srcCol--;

                var lineLength = endsWithWide ? newCols - 1 : newCols;
                newLineLengths.Add(lineLength);
                cellsAvailable += lineLength;
            }

            return newLineLengths.ToArray();
        }

        private struct InsertionSet
        {
            public BufferLine[] Lines;
            public int Start;
            public bool IsNull;
            public static InsertionSet Null => new() {IsNull = true};
        }
    }
}