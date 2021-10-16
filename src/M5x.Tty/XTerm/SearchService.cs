using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace M5x.Tty.XTerm
{
    /// <summary>
    ///     Handles searching in a terminal
    /// </summary>
    public class SearchService
    {
        private readonly SelectionService selection;
        private SearchSnapshot cache;

        public SearchService(Terminal terminal)
        {
            selection = new SelectionService(terminal);
        }

        /// <summary>
        ///     This event is triggered when the current search snapshot has been invalidated
        /// </summary>
        public event Action<SearchService, string> Invalidated;

        /// <summary>
        ///     Gets a snapshot that can be used to perform searches. A snapshot is only useful if the buffer content or dimensions
        ///     have not changed.
        /// </summary>
        public SearchSnapshot GetSnapshot()
        {
            if (cache == null)
            {
                var result = CalculateSnapshot();
                cache = result;
            }

            return cache;
        }

        /// <summary>
        ///     Invalidates the current search snapshot due to content or size changes.
        ///     The cache should be invalidated when either the content of the buffer or the buffer dimensions change
        ///     because the snapshot has direct mappings to buffer line and locations.
        /// </summary>
        public void Invalidate()
        {
            // TODO: ideally this would be private and handled completely by the search service and consumers don't have to call this
            Invalidated?.Invoke(this, cache?.LastSearch);
            cache = null;
        }

        private SearchSnapshot CalculateSnapshot()
        {
            selection.SelectAll();

            var lines = selection.GetSelectedLines();

            return CalculateSnapshot(lines);
        }

        private SearchSnapshot CalculateSnapshot(Line[] lines)
        {
            var result = new SearchSnapshot(lines);

            return result;
        }
    }

    public class SearchSnapshot
    {
        private readonly Line[] lines;

        /// <summary>
        ///     Gets the index of the current search result
        /// </summary>
        public int CurrentSearchResult;

        public SearchSnapshot(Line[] lines)
        {
            this.lines = lines;
            Text = GetTextFromLines(lines);
        }

        public string Text { get; }

        /// <summary>
        ///     Gets the last used search term
        /// </summary>
        public string LastSearch { get; private set; }

        /// <summary>
        ///     Gets the last search results
        /// </summary>
        public SearchResult[] LastSearchResults { get; private set; }

        /// <summary>
        ///     Given a string, returns start and end points in the buffer that contain that text
        /// </summary>
        public int FindText(string txt)
        {
            LastSearch = txt;
            CurrentSearchResult = -1;

            if (string.IsNullOrEmpty(txt))
            {
                LastSearchResults = Array.Empty<SearchResult>();
                return 0;
            }

            // simple search for now, we might be able to just do a single scan of the buffer
            // but if we want to support regex then this might be the better way
            // a quick look at xterm.js and they still get a copy of the buffer and translate it to string
            // so this is similar, maybe(?) caching more than we ultimately need.
            var results = new List<SearchResult>();

            var baseLineIndex = 0;
            var baseCount = 0;
            var index = Text.IndexOf(txt, 0, StringComparison.CurrentCultureIgnoreCase);
            while (index >= 0)
            {
                // found a result
                var result = new SearchResult();
                // whats the start and end pos of this text
                // we can assume that it's on the same line, unless we are doing a regex because
                // the user can't enter a \n as part of the seach term without regex

                // count the lines up to index
                var count = baseCount;
                for (var i = baseLineIndex; i < lines.Length; i++)
                {
                    count += lines[i].Length;
                    if (count > index)
                    {
                        // found text is on line i
                        // the x position is the delta between the line start and index
                        // we can assume for now that the end is on the same line, since we do not yet support regex

                        var lineStartCount = count - lines[i].Length;

                        // we need to offset the points depending on whether the line fragment is wrapped or not
                        var startFragmentIndex = lines[i].GetFragmentIndexForPosition(index - lineStartCount);
                        var startFragment = lines[i].GetFragment(startFragmentIndex);

                        // number of chars before this fragment, but on this line
                        var startOffset = 0;
                        for (var fi = 0; fi < startFragmentIndex; fi++) startOffset += lines[i].GetFragment(fi).Length;


                        result.Start = new Point(index - lineStartCount - startOffset, startFragment.Line);

                        var endFragmentIndex =
                            lines[i].GetFragmentIndexForPosition(index - lineStartCount + txt.Length - 1);
                        var endFragment = lines[i].GetFragment(endFragmentIndex);

                        var endOffset = 0;
                        for (var fi = 0; fi < endFragmentIndex; fi++) endOffset += lines[i].GetFragment(fi).Length;

                        result.End = new Point(index - lineStartCount + txt.Length - endOffset, endFragment.Line);

                        // now, we need to fix up the end points because we might be on wrapped line
                        // which line fragment is the text on

                        results.Add(result);

                        break;
                    }

                    // update base counts so that next time we loop we don't have to count these lines again
                    baseCount += lines[i].Length;
                    baseLineIndex++;
                }

                // search again
                index = Text.IndexOf(txt, index + txt.Length, StringComparison.CurrentCultureIgnoreCase);
            }

            LastSearchResults = results.ToArray();
            CurrentSearchResult = -1;

            return LastSearchResults.Length;
        }

        public SearchResult FindNext()
        {
            if (LastSearchResults == null || LastSearchResults.Length == 0) return null;

            CurrentSearchResult++;
            if (CurrentSearchResult > LastSearchResults.Length - 1)
                CurrentSearchResult = 0;

            return LastSearchResults[CurrentSearchResult];
        }

        public SearchResult FindPrevious()
        {
            if (LastSearchResults == null || LastSearchResults.Length == 0) return null;

            CurrentSearchResult--;
            if (CurrentSearchResult < 0)
                CurrentSearchResult = LastSearchResults.Length - 1;

            return LastSearchResults[CurrentSearchResult];
        }

        private string GetTextFromLines(Line[] lines)
        {
            if (lines.Length == 0)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var line in lines) line.GetFragmentStrings(builder);

            return builder.ToString();
        }

        [DebuggerDisplay("{Start}, {End}")]
        public class SearchResult
        {
            public Point End;
            public Point Start;
        }
    }
}