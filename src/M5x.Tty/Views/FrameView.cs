//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// NOTE: FrameView is functionally identical to Window with the following exceptions. 
//  - Is not a Toplevel
//  - Does not support mouse dragging
//  - Does not support padding (but should)
//  - Does not support IEnumerable
// Any udpates done here should probably be done in Window as well; TODO: Merge these classes

using M5x.Tty.Core;
using M5x.Tty.Types;
using NStack;

namespace M5x.Tty.Views
{
    /// <summary>
    ///     The FrameView is a container frame that draws a frame around the contents. It is similar to
    ///     a GroupBox in Windows.
    /// </summary>
    public class FrameView : View
    {
        private readonly View contentView;
        private ustring title;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FrameView" /> class using <see cref="LayoutStyle.Absolute" /> layout.
        /// </summary>
        /// <param name="frame">Frame.</param>
        /// <param name="title">Title.</param>
        public FrameView(Rect frame, ustring title = null) : base(frame)
        {
            var cFrame = new Rect(1, 1, frame.Width - 2, frame.Height - 2);
            this.title = title;
            contentView = new ContentView(cFrame);
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FrameView" /> class using <see cref="LayoutStyle.Computed" /> layout.
        /// </summary>
        /// <param name="frame">Frame.</param>
        /// <param name="title">Title.</param>
        /// ///
        /// <param name="views">Views.</param>
        public FrameView(Rect frame, ustring title, View[] views) : this(frame, title)
        {
            foreach (var view in views) contentView.Add(view);
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FrameView" /> class using <see cref="LayoutStyle.Computed" /> layout.
        /// </summary>
        /// <param name="title">Title.</param>
        public FrameView(ustring title)
        {
            this.title = title;
            contentView = new ContentView
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(1),
                Height = Dim.Fill(1)
            };
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FrameView" /> class using <see cref="LayoutStyle.Computed" /> layout.
        /// </summary>
        public FrameView() : this(string.Empty)
        {
        }

        /// <summary>
        ///     The title to be displayed for this <see cref="FrameView" />.
        /// </summary>
        /// <value>The title.</value>
        public ustring Title
        {
            get => title;
            set
            {
                title = value;
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     The text displayed by the <see cref="Label" />.
        /// </summary>
        public override ustring Text
        {
            get => contentView.Text;
            set
            {
                base.Text = value;
                if (contentView != null) contentView.Text = value;
            }
        }

        /// <summary>
        ///     Controls the text-alignment property of the label, changing it will redisplay the <see cref="Label" />.
        /// </summary>
        /// <value>The text alignment.</value>
        public override TextAlignment TextAlignment
        {
            get => contentView.TextAlignment;
            set => base.TextAlignment = contentView.TextAlignment = value;
        }

        private void Initialize()
        {
            if (Subviews?.Count == 0)
            {
                base.Add(contentView);
                contentView.Text = base.Text;
            }
        }

        private void DrawFrame()
        {
            DrawFrame(new Rect(0, 0, Frame.Width, Frame.Height), 0, true);
        }

        /// <summary>
        ///     Add the specified <see cref="View" /> to this container.
        /// </summary>
        /// <param name="view"><see cref="View" /> to add to this container</param>
        public override void Add(View view)
        {
            contentView.Add(view);
            if (view.CanFocus)
                CanFocus = true;
        }


        /// <summary>
        ///     Removes a <see cref="View" /> from this container.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public override void Remove(View view)
        {
            if (view == null)
                return;

            SetNeedsDisplay();
            var touched = view.Frame;
            contentView.Remove(view);

            if (contentView.InternalSubviews.Count < 1)
                CanFocus = false;
        }

        /// <summary>
        ///     Removes all <see cref="View" />s from this container.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public override void RemoveAll()
        {
            contentView.RemoveAll();
        }

        /// <inheritdoc />
        public override void Redraw(Rect bounds)
        {
            var padding = 0;
            Application.CurrentView = this;
            var scrRect = ViewToScreen(new Rect(0, 0, Frame.Width, Frame.Height));

            if (!NeedDisplay.IsEmpty)
            {
                Driver.SetAttribute(ColorScheme.Normal);
                Driver.DrawWindowFrame(scrRect, padding + 1, padding + 1, padding + 1, padding + 1, true, true);
            }

            var savedClip = ClipToBounds();
            contentView.Redraw(contentView.Bounds);
            Driver.Clip = savedClip;

            ClearNeedsDisplay();
            Driver.SetAttribute(ColorScheme.Normal);
            Driver.DrawWindowFrame(scrRect, padding + 1, padding + 1, padding + 1, padding + 1);

            if (HasFocus)
                Driver.SetAttribute(ColorScheme.HotNormal);
            Driver.DrawWindowTitle(scrRect, Title, padding, padding, padding, padding);
            Driver.SetAttribute(ColorScheme.Normal);
        }

        /// <summary>
        ///     ContentView is an internal implementation detail of Window. It is used to host Views added with
        ///     <see cref="Add(View)" />.
        ///     Its ONLY reason for being is to provide a simple way for Window to expose to those SubViews that the Window's
        ///     Bounds
        ///     are actually deflated due to the border.
        /// </summary>
        private class ContentView : View
        {
            public ContentView(Rect frame) : base(frame)
            {
            }

            public ContentView()
            {
            }
        }
    }
}