//
// ListView.cs: ListView control
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
//
// TODO:
//   - Should we support multiple columns, if so, how should that be done?
//   - Show mark for items that have been marked.
//   - Mouse support
//   - Scrollbars?
//
// Column considerations:
//   - Would need a way to specify widths
//   - Should it automatically extract data out of structs/classes based on public fields/properties?
//   - It seems that this would be useful just for the "simple" API, not the IListDAtaSource, as that one has full support for it.
//   - Should a function be specified that retrieves the individual elements?
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.Tty.Core;
using M5x.Tty.Types;
using NStack;

namespace M5x.Tty.Views
{
    /// <summary>
    ///     Implement <see cref="IListDataSource" /> to provide custom rendering for a <see cref="ListView" />.
    /// </summary>
    public interface IListDataSource
    {
        /// <summary>
        ///     Returns the number of elements to display
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     This method is invoked to render a specified item, the method should cover the entire provided width.
        /// </summary>
        /// <returns>The render.</returns>
        /// <param name="container">The list view to render.</param>
        /// <param name="driver">The console driver to render.</param>
        /// <param name="selected">Describes whether the item being rendered is currently selected by the user.</param>
        /// <param name="item">The index of the item to render, zero for the first item and so on.</param>
        /// <param name="col">The column where the rendering will start</param>
        /// <param name="line">The line where the rendering will be done.</param>
        /// <param name="width">The width that must be filled out.</param>
        /// <remarks>
        ///     The default color will be set before this method is invoked, and will be based on whether the item is selected or
        ///     not.
        /// </remarks>
        void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width);

        /// <summary>
        ///     Should return whether the specified item is currently marked.
        /// </summary>
        /// <returns><c>true</c>, if marked, <c>false</c> otherwise.</returns>
        /// <param name="item">Item index.</param>
        bool IsMarked(int item);

        /// <summary>
        ///     Flags the item as marked.
        /// </summary>
        /// <param name="item">Item index.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        void SetMark(int item, bool value);

        /// <summary>
        ///     Return the source as IList.
        /// </summary>
        /// <returns></returns>
        IList ToList();
    }

    /// <summary>
    ///     ListView <see cref="View" /> renders a scrollable list of data where each item can be activated to perform an
    ///     action.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="ListView" /> displays lists of data and allows the user to scroll through the data.
    ///         Items in the can be activated firing an event (with the ENTER key or a mouse double-click).
    ///         If the <see cref="AllowsMarking" /> property is true, elements of the list can be marked by the user.
    ///     </para>
    ///     <para>
    ///         By default <see cref="ListView" /> uses <see cref="object.ToString" /> to render the items of any
    ///         <see cref="IList" /> object (e.g. arrays, <see cref="List{T}" />,
    ///         and other collections). Alternatively, an object that implements the <see cref="IListDataSource" />
    ///         interface can be provided giving full control of what is rendered.
    ///     </para>
    ///     <para>
    ///         <see cref="ListView" /> can display any object that implements the <see cref="IList" /> interface.
    ///         <see cref="string" /> values are converted into <see cref="ustring" /> values before rendering, and other
    ///         values are
    ///         converted into <see cref="string" /> by calling <see cref="object.ToString" /> and then converting to
    ///         <see cref="ustring" /> .
    ///     </para>
    ///     <para>
    ///         To change the contents of the ListView, set the <see cref="Source" /> property (when
    ///         providing custom rendering via <see cref="IListDataSource" />) or call <see cref="SetSource" />
    ///         an <see cref="IList" /> is being used.
    ///     </para>
    ///     <para>
    ///         When <see cref="AllowsMarking" /> is set to true the rendering will prefix the rendered items with
    ///         [x] or [ ] and bind the SPACE key to toggle the selection. To implement a different
    ///         marking style set <see cref="AllowsMarking" /> to false and implement custom rendering.
    ///     </para>
    /// </remarks>
    public class ListView : View
    {
        private bool allowsMarking;
        private bool allowsMultipleSelection = true;

        private int lastSelectedItem = -1;
        private int selected;

        private IListDataSource source;
        private int top;

        /// <summary>
        ///     Initializes a new instance of <see cref="ListView" /> that will display the contents of the object implementing the
        ///     <see cref="IList" /> interface,
        ///     with relative positioning.
        /// </summary>
        /// <param name="source">
        ///     An <see cref="IList" /> data source, if the elements are strings or ustrings, the string is
        ///     rendered, otherwise the ToString() method is invoked on the result.
        /// </param>
        public ListView(IList source) : this(MakeWrapper(source))
        {
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="ListView" /> that will display the provided data source, using relative
        ///     positioning.
        /// </summary>
        /// <param name="source">
        ///     <see cref="IListDataSource" /> object that provides a mechanism to render the data.
        ///     The number of elements on the collection should not change, if you must change, set
        ///     the "Source" property to reset the internal settings of the ListView.
        /// </param>
        public ListView(IListDataSource source)
        {
            this.source = source;
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="ListView" />. Set the <see cref="Source" /> property to display something.
        /// </summary>
        public ListView()
        {
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="ListView" /> that will display the contents of the object implementing the
        ///     <see cref="IList" /> interface with an absolute position.
        /// </summary>
        /// <param name="rect">Frame for the listview.</param>
        /// <param name="source">
        ///     An IList data source, if the elements of the IList are strings or ustrings, the string is
        ///     rendered, otherwise the ToString() method is invoked on the result.
        /// </param>
        public ListView(Rect rect, IList source) : this(rect, MakeWrapper(source))
        {
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="ListView" /> with the provided data source and an absolute position
        /// </summary>
        /// <param name="rect">Frame for the listview.</param>
        /// <param name="source">
        ///     IListDataSource object that provides a mechanism to render the data. The number of elements on the
        ///     collection should not change, if you must change, set the "Source" property to reset the internal settings of the
        ///     ListView.
        /// </param>
        public ListView(Rect rect, IListDataSource source) : base(rect)
        {
            this.source = source;
            Initialize();
        }

        /// <summary>
        ///     Gets or sets the <see cref="IListDataSource" /> backing this <see cref="ListView" />, enabling custom rendering.
        /// </summary>
        /// <value>The source.</value>
        /// <remarks>
        ///     Use <see cref="SetSource" /> to set a new <see cref="IList" /> source.
        /// </remarks>
        public IListDataSource Source
        {
            get => source;
            set
            {
                source = value;
                top = 0;
                selected = 0;
                lastSelectedItem = -1;
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     Gets or sets whether this <see cref="ListView" /> allows items to be marked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if allows marking elements of the list; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     If set to true, <see cref="ListView" /> will render items marked items with "[x]", and unmarked items with "[ ]"
        ///     spaces. SPACE key will toggle marking.
        /// </remarks>
        public bool AllowsMarking
        {
            get => allowsMarking;
            set
            {
                allowsMarking = value;
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     If set to true allows more than one item to be selected. If false only allow one item selected.
        /// </summary>
        public bool AllowsMultipleSelection
        {
            get => allowsMultipleSelection;
            set
            {
                allowsMultipleSelection = value;
                if (Source != null && !allowsMultipleSelection) // Clear all selections except selected 
                    for (var i = 0; i < Source.Count; i++)
                        if (Source.IsMarked(i) && i != selected)
                            Source.SetMark(i, false);
            }
        }

        /// <summary>
        ///     Gets or sets the item that is displayed at the top of the <see cref="ListView" />.
        /// </summary>
        /// <value>The top item.</value>
        public int TopItem
        {
            get => top;
            set
            {
                if (source == null)
                    return;

                if (top < 0 || top >= source.Count)
                    throw new ArgumentException("value");
                top = value;
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     Gets or sets the index of the currently selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public int SelectedItem
        {
            get => selected;
            set
            {
                if (source == null || source.Count == 0) return;
                if (value < 0 || value >= source.Count) throw new ArgumentException("value");
                selected = value;
                OnSelectedChanged();
            }
        }

        /// <summary>
        ///     Sets the source of the <see cref="ListView" /> to an <see cref="IList" />.
        /// </summary>
        /// <value>An object implementing the IList interface.</value>
        /// <remarks>
        ///     Use the <see cref="Source" /> property to set a new <see cref="IListDataSource" /> source and use custome
        ///     rendering.
        /// </remarks>
        public void SetSource(IList source)
        {
            if (source == null)
                Source = null;
            else
                Source = MakeWrapper(source);
        }

        /// <summary>
        ///     Sets the source to an <see cref="IList" /> value asynchronously.
        /// </summary>
        /// <value>An item implementing the IList interface.</value>
        /// <remarks>
        ///     Use the <see cref="Source" /> property to set a new <see cref="IListDataSource" /> source and use custome
        ///     rendering.
        /// </remarks>
        public Task SetSourceAsync(IList source)
        {
            return Task.Factory.StartNew(() =>
            {
                if (source == null)
                    Source = null;
                else
                    Source = MakeWrapper(source);
                return source;
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }


        private static IListDataSource MakeWrapper(IList source)
        {
            return new ListWrapper(source);
        }

        private void Initialize()
        {
            Source = source;
            CanFocus = true;
        }

        /// <inheritdoc />
        public override void Redraw(Rect bounds)
        {
            var current = ColorScheme.Focus;
            Driver.SetAttribute(current);
            Move(0, 0);
            var f = Frame;
            if (selected < top)
                top = selected;
            else if (selected >= top + f.Height) top = selected;
            var item = top;
            var focused = HasFocus;
            var col = allowsMarking ? 2 : 0;

            for (var row = 0; row < f.Height; row++, item++)
            {
                var isSelected = item == selected;

                var newcolor = focused ? isSelected ? ColorScheme.Focus :
                    ColorScheme.Normal :
                    isSelected ? ColorScheme.HotNormal : ColorScheme.Normal;
                if (newcolor != current)
                {
                    Driver.SetAttribute(newcolor);
                    current = newcolor;
                }

                Move(0, row);
                if (source == null || item >= source.Count)
                {
                    for (var c = 0; c < f.Width; c++)
                        Driver.AddRune(' ');
                }
                else
                {
                    if (allowsMarking)
                    {
                        Driver.AddRune(source.IsMarked(item) ? AllowsMultipleSelection ? Driver.Checked :
                            Driver.Selected :
                            AllowsMultipleSelection ? Driver.UnChecked : Driver.UnSelected);
                        Driver.AddRune(' ');
                    }

                    Source.Render(this, Driver, isSelected, item, col, row, f.Width - col);
                }
            }
        }

        /// <summary>
        ///     This event is raised when the selected item in the <see cref="ListView" /> has changed.
        /// </summary>
        public event Action<ListViewItemEventArgs> SelectedItemChanged;

        /// <summary>
        ///     This event is raised when the user Double Clicks on an item or presses ENTER to open the selected item.
        /// </summary>
        public event Action<ListViewItemEventArgs> OpenSelectedItem;

        /// <inheritdoc />
        public override bool ProcessKey(KeyEvent kb)
        {
            if (source == null)
                return base.ProcessKey(kb);

            switch (kb.Key)
            {
                case Key.CursorUp:
                case Key.P | Key.CtrlMask:
                    return MoveUp();

                case Key.CursorDown:
                case Key.N | Key.CtrlMask:
                    return MoveDown();

                case Key.V | Key.CtrlMask:
                case Key.PageDown:
                    return MovePageDown();

                case Key.PageUp:
                    return MovePageUp();

                case Key.Space:
                    if (MarkUnmarkRow())
                        return true;
                    else
                        break;

                case Key.Enter:
                    OnOpenSelectedItem();
                    break;

                case Key.End:
                    return MoveEnd();

                case Key.Home:
                    return MoveHome();
            }

            return base.ProcessKey(kb);
        }

        /// <summary>
        ///     Prevents marking if it's not allowed mark and if it's not allows multiple selection.
        /// </summary>
        /// <returns></returns>
        public virtual bool AllowsAll()
        {
            if (!allowsMarking)
                return false;
            if (!AllowsMultipleSelection)
                for (var i = 0; i < Source.Count; i++)
                    if (Source.IsMarked(i) && i != selected)
                    {
                        Source.SetMark(i, false);
                        return true;
                    }

            return true;
        }

        /// <summary>
        ///     Marks an unmarked row.
        /// </summary>
        /// <returns></returns>
        public virtual bool MarkUnmarkRow()
        {
            if (AllowsAll())
            {
                Source.SetMark(SelectedItem, !Source.IsMarked(SelectedItem));
                SetNeedsDisplay();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Moves the selected item index to the next page.
        /// </summary>
        /// <returns></returns>
        public virtual bool MovePageUp()
        {
            var n = selected - Frame.Height;
            if (n < 0)
                n = 0;
            if (n != selected)
            {
                selected = n;
                top = selected;
                OnSelectedChanged();
                SetNeedsDisplay();
            }

            return true;
        }

        /// <summary>
        ///     Moves the selected item index to the previous page.
        /// </summary>
        /// <returns></returns>
        public virtual bool MovePageDown()
        {
            var n = selected + Frame.Height;
            if (n > source.Count)
                n = source.Count - 1;
            if (n != selected)
            {
                selected = n;
                if (source.Count >= Frame.Height)
                    top = selected;
                else
                    top = 0;
                OnSelectedChanged();
                SetNeedsDisplay();
            }

            return true;
        }

        /// <summary>
        ///     Moves the selected item index to the next row.
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveDown()
        {
            if (source.Count == 0) // Do we set lastSelectedItem to -1 here?
                return false; //Nothing for us to move to
            if (selected >= source.Count)
            {
                // If for some reason we are currently outside of the
                // valid values range, we should select the bottommost valid value.
                // This can occur if the backing data source changes.
                selected = source.Count - 1;
                OnSelectedChanged();
                SetNeedsDisplay();
            }
            else if (selected + 1 < source.Count)
            {
                //can move by down by one.
                selected++;

                if (selected >= top + Frame.Height)
                    top++;
                OnSelectedChanged();
                SetNeedsDisplay();
            }
            else if (selected == 0)
            {
                OnSelectedChanged();
                SetNeedsDisplay();
            }

            return true;
        }

        /// <summary>
        ///     Moves the selected item index to the previous row.
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveUp()
        {
            if (source.Count == 0) // Do we set lastSelectedItem to -1 here?
                return false; //Nothing for us to move to
            if (selected >= source.Count)
            {
                // If for some reason we are currently outside of the
                // valid values range, we should select the bottommost valid value.
                // This can occur if the backing data source changes.
                selected = source.Count - 1;
                OnSelectedChanged();
                SetNeedsDisplay();
            }
            else if (selected > 0)
            {
                selected--;
                if (selected > Source.Count) selected = Source.Count - 1;
                if (selected < top)
                    top = selected;
                OnSelectedChanged();
                SetNeedsDisplay();
            }

            return true;
        }

        /// <summary>
        ///     Moves the selected item index to the last row.
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveEnd()
        {
            if (selected != source.Count - 1)
            {
                selected = source.Count - 1;
                top = selected;
                OnSelectedChanged();
                SetNeedsDisplay();
            }

            return true;
        }

        /// <summary>
        ///     Moves the selected item index to the first row.
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveHome()
        {
            if (selected != 0)
            {
                selected = 0;
                top = selected;
                OnSelectedChanged();
                SetNeedsDisplay();
            }

            return true;
        }

        /// <summary>
        ///     Invokes the SelectedChanged event if it is defined.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnSelectedChanged()
        {
            if (selected != lastSelectedItem)
            {
                var value = source?.Count > 0 ? source.ToList()[selected] : null;
                SelectedItemChanged?.Invoke(new ListViewItemEventArgs(selected, value));
                lastSelectedItem = selected;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Invokes the OnOpenSelectedItem event if it is defined.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnOpenSelectedItem()
        {
            if (source.Count <= selected || selected < 0) return false;
            var value = source.ToList()[selected];
            OpenSelectedItem?.Invoke(new ListViewItemEventArgs(selected, value));

            return true;
        }

        /// <inheritdoc />
        public override bool OnEnter(View view)
        {
            if (lastSelectedItem == -1)
            {
                OnSelectedChanged();
                return true;
            }

            return base.OnEnter(view);
        }

        /// <inheritdoc />
        public override bool OnLeave(View view)
        {
            if (lastSelectedItem > -1)
            {
                lastSelectedItem = -1;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override void PositionCursor()
        {
            if (allowsMarking)
                Move(0, selected - top);
            else
                Move(Bounds.Width - 1, selected - top);
        }

        /// <inheritdoc />
        public override bool MouseEvent(MouseEvent me)
        {
            if (!me.Flags.HasFlag(MouseFlags.Button1Clicked) && !me.Flags.HasFlag(MouseFlags.Button1DoubleClicked) &&
                me.Flags != MouseFlags.WheeledDown && me.Flags != MouseFlags.WheeledUp)
                return false;

            if (!HasFocus && CanFocus) SetFocus();

            if (source == null) return false;

            if (me.Flags == MouseFlags.WheeledDown)
            {
                MoveDown();
                return true;
            }

            if (me.Flags == MouseFlags.WheeledUp)
            {
                MoveUp();
                return true;
            }

            if (me.Y + top >= source.Count) return true;

            selected = top + me.Y;
            if (AllowsAll())
            {
                Source.SetMark(SelectedItem, !Source.IsMarked(SelectedItem));
                SetNeedsDisplay();
                return true;
            }

            OnSelectedChanged();
            SetNeedsDisplay();
            if (me.Flags == MouseFlags.Button1DoubleClicked) OnOpenSelectedItem();

            return true;
        }
    }

    /// <summary>
    ///     Implements an <see cref="IListDataSource" /> that renders arbitrary <see cref="IList" /> instances for
    ///     <see cref="ListView" />.
    /// </summary>
    /// <remarks>Implements support for rendering marked items.</remarks>
    public class ListWrapper : IListDataSource
    {
        private readonly int count;
        private readonly BitArray marks;
        private readonly IList src;

        /// <summary>
        ///     Initializes a new instance of <see cref="ListWrapper" /> given an <see cref="IList" />
        /// </summary>
        /// <param name="source"></param>
        public ListWrapper(IList source)
        {
            if (source != null)
            {
                count = source.Count;
                marks = new BitArray(count);
                src = source;
            }
        }

        /// <summary>
        ///     Gets the number of items in the <see cref="IList" />.
        /// </summary>
        public int Count => src != null ? src.Count : 0;

        /// <summary>
        ///     Renders a <see cref="ListView" /> item to the appropriate type.
        /// </summary>
        /// <param name="container">The ListView.</param>
        /// <param name="driver">The driver used by the caller.</param>
        /// <param name="marked">Informs if it's marked or not.</param>
        /// <param name="item">The item.</param>
        /// <param name="col">The col where to move.</param>
        /// <param name="line">The line where to move.</param>
        /// <param name="width">The item width.</param>
        public void Render(ListView container, ConsoleDriver driver, bool marked, int item, int col, int line,
            int width)
        {
            container.Move(col, line);
            var t = src[item];
            if (t == null)
            {
                RenderUstr(driver, ustring.Make(""), col, line, width);
            }
            else
            {
                if (t is ustring)
                    RenderUstr(driver, (ustring) t, col, line, width);
                else if (t is string)
                    RenderUstr(driver, (string) t, col, line, width);
                else
                    RenderUstr(driver, t.ToString(), col, line, width);
            }
        }

        /// <summary>
        ///     Returns true if the item is marked, false otherwise.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c>If is marked.<c>false</c>otherwise.</returns>
        public bool IsMarked(int item)
        {
            if (item >= 0 && item < count)
                return marks[item];
            return false;
        }

        /// <summary>
        ///     Sets the item as marked or unmarked based on the value is true or false, respectively.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="value"><true>Marks the item.</true><false>Unmarked the item.</false>The value.</param>
        public void SetMark(int item, bool value)
        {
            if (item >= 0 && item < count)
                marks[item] = value;
        }

        /// <summary>
        ///     Returns the source as IList.
        /// </summary>
        /// <returns></returns>
        public IList ToList()
        {
            return src;
        }

        private void RenderUstr(ConsoleDriver driver, ustring ustr, int col, int line, int width)
        {
            var byteLen = ustr.Length;
            var used = 0;
            for (var i = 0; i < byteLen;)
            {
                var (rune, size) = Utf8.DecodeRune(ustr, i, i - byteLen);
                var count = Rune.ColumnWidth(rune);
                if (used + count > width)
                    break;
                driver.AddRune(rune);
                used += count;
                i += size;
            }

            for (; used < width; used++) driver.AddRune(' ');
        }
    }

    /// <summary>
    ///     <see cref="EventArgs" /> for <see cref="ListView" /> events.
    /// </summary>
    public class ListViewItemEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="ListViewItemEventArgs" />
        /// </summary>
        /// <param name="item">The index of the the <see cref="ListView" /> item.</param>
        /// <param name="value">The <see cref="ListView" /> item</param>
        public ListViewItemEventArgs(int item, object value)
        {
            Item = item;
            Value = value;
        }

        /// <summary>
        ///     The index of the <see cref="ListView" /> item.
        /// </summary>
        public int Item { get; }

        /// <summary>
        ///     The the <see cref="ListView" /> item.
        /// </summary>
        public object Value { get; }
    }
}