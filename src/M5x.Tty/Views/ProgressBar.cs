﻿using M5x.Tty.Core;
using M5x.Tty.Types;

namespace M5x.Tty.Views
{
    /// <summary>
    ///     A Progress Bar view that can indicate progress of an activity visually.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ProgressBar" /> can operate in two modes, percentage mode, or
    ///         activity mode.  The progress bar starts in percentage mode and
    ///         setting the Fraction property will reflect on the UI the progress
    ///         made so far.   Activity mode is used when the application has no
    ///         way of knowing how much time is left, and is started when the <see cref="Pulse" /> method is called.
    ///         Call <see cref="Pulse" /> repeatedly as progress is made.
    ///     </para>
    /// </remarks>
    public class ProgressBar : View
    {
        private int activityPos, delta;

        private float fraction;
        private bool isActivity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBar" /> class, starts in percentage mode with an absolute
        ///     position and size.
        /// </summary>
        /// <param name="rect">Rect.</param>
        public ProgressBar(Rect rect) : base(rect)
        {
            CanFocus = false;
            fraction = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBar" /> class, starts in percentage mode and uses relative
        ///     layout.
        /// </summary>
        public ProgressBar()
        {
            CanFocus = false;
            fraction = 0;
        }

        /// <summary>
        ///     Gets or sets the <see cref="ProgressBar" /> fraction to display, must be a value between 0 and 1.
        /// </summary>
        /// <value>The fraction representing the progress.</value>
        public float Fraction
        {
            get => fraction;
            set
            {
                fraction = value;
                isActivity = false;
                SetNeedsDisplay();
            }
        }

        /// <summary>
        ///     Notifies the <see cref="ProgressBar" /> that some progress has taken place.
        /// </summary>
        /// <remarks>
        ///     If the <see cref="ProgressBar" /> is is percentage mode, it switches to activity
        ///     mode.   If is in activity mode, the marker is moved.
        /// </remarks>
        public void Pulse()
        {
            if (!isActivity)
            {
                isActivity = true;
                activityPos = 0;
                delta = 1;
            }
            else
            {
                activityPos += delta;
                if (activityPos < 0)
                {
                    activityPos = 1;
                    delta = 1;
                }
                else if (activityPos >= Frame.Width)
                {
                    activityPos = Frame.Width - 2;
                    delta = -1;
                }
            }

            SetNeedsDisplay();
        }

        /// <inheritdoc />
        public override void Redraw(Rect region)
        {
            Driver.SetAttribute(ColorScheme.Normal);

            var top = Frame.Width;
            if (isActivity)
            {
                Move(0, 0);
                for (var i = 0; i < top; i++)
                    if (i == activityPos)
                        Driver.AddRune(Driver.Stipple);
                    else
                        Driver.AddRune(' ');
            }
            else
            {
                Move(0, 0);
                var mid = (int) (fraction * top);
                int i;
                for (i = 0; i < mid; i++)
                    Driver.AddRune(Driver.Stipple);
                for (; i < top; i++)
                    Driver.AddRune(' ');
            }
        }
    }
}