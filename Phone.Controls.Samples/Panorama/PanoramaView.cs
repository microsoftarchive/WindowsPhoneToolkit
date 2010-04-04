using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Phone.Controls.Samples
{
    internal delegate void AnimationCompleteEventHandler(object sender, AnimationCompleteEventArgs e);
    internal class AnimationCompleteEventArgs
    {
        public double Position;
        public AnimationCompleteEventArgs()
        {
        }
    }

    /// <summary>
    /// Helper class for PanoramaControl.
    /// Implements the UI logic.
    /// </summary>
    internal class PanoramaView
    {
        internal struct PanelHost
        {
            public TranslateTransform Transform;
            public double Width;
            public double Padding;
            public double Speed;
            public void Reset() { Width = 0.0;  Speed = 1.0; }
        }

        private const string LayoutRootName = "LayoutRoot";
        private const string PresenterBackgroundName = "BackgroundImagePresenter";
        private const string PresenterTitleName = "TitlePresenter";
        private const string PresenterItemsName = "ItemsPresenter";
        private const string PanelBackgroundName = "BackgroundImagePanel";
        private const string PanelTitleName = "TitlePanel";
        private const string PanelItemsName = "ItemsPanel";
        private const double AnimationDuration = 1000.0;

        private PanoramaControl Parent;
        private Panel LayoutRoot;

        private Storyboard Storyboard;
        public event AnimationCompleteEventHandler AnimationCompleted;

        private PanelHost Background;
        private PanelHost Title;
        private PanelHost Items;

        private bool ready = false;

        public PanoramaView(PanoramaControl parent, Panel root)
        {
            Parent = parent;
            LayoutRoot = root;

            Background.Transform = new TranslateTransform();
            Title.Transform = new TranslateTransform();
            Items.Transform = new TranslateTransform();
        }

        /// <summary>
        /// Initialize the panel hosting control presenters to duplicate (creating a virtual carousel)
        /// </summary>
        /// <param name="panel">Carousel panel</param>
        /// <param name="presenter">Main control presenter</param>
        /// <param name="left">Control to add to the left</param>
        /// <param name="right">Control to add to the right</param>
        /// <param name="pad">Number of empty padding pixels to add to 'left' and 'right'</param>
        private void InitializeHost(Panel panel, FrameworkElement presenter, FrameworkElement left, FrameworkElement right, double pad)
        {
            // reset/initialize layout with dummy values
            if (panel.Children.Count == 1)
            {
                panel.Children.Insert(0, new Rectangle());
                panel.Children.Add(new Rectangle());
            }
            panel.SetValue(Canvas.LeftProperty, 0.0);


            // insert items ?
            if (Parent.Items.Count > 0)
            {
                WriteableBitmap bitmap;
                Image image;
                int width;
                int height;

                // duplicate left
                width = (int)(left.ActualWidth + pad);
                height = (int)left.ActualHeight;
                bitmap = new WriteableBitmap(width, height);
                bitmap.Render(left, null);
                bitmap.Invalidate();
                image = new Image();
                image.Source = bitmap;
                panel.Children[0] = image;
                double offset = bitmap.PixelWidth;

                // duplicate right
                width = (int)(right.ActualWidth + pad);
                height = (int)right.ActualHeight;
                bitmap = new WriteableBitmap(width, height);
                bitmap.Render(right, new TranslateTransform() { X = pad });
                bitmap.Invalidate();
                image = new Image();
                image.Source = bitmap;
                panel.Children[2] = image;

                // adjust panel position
                panel.SetValue(Canvas.LeftProperty, -offset);
            }
        }

        public void Initialize()
        {
            if (!ready)
            {
                // fetch template elements
                ContentPresenter PresenterBackground = LayoutRoot.FindName(PresenterBackgroundName) as ContentPresenter;
                ContentPresenter PresenterTitle = LayoutRoot.FindName(PresenterTitleName) as ContentPresenter;
                ItemsPresenter PresenterItems = LayoutRoot.FindName(PresenterItemsName) as ItemsPresenter;
                Panel PanelBackground = LayoutRoot.FindName(PanelBackgroundName) as Panel;
                Panel PanelTitle = LayoutRoot.FindName(PanelTitleName) as Panel;
                Panel PanelItems = LayoutRoot.FindName(PanelItemsName) as Panel;
                if (null == PresenterBackground) throw new ArgumentException(string.Format("Cannot find {0}.", PresenterBackgroundName));
                if (null == PresenterTitle) throw new ArgumentException(string.Format("Cannot find {0}.", PresenterTitleName));
                if (null == PresenterItems) throw new ArgumentException(string.Format("Cannot find {0}.", PresenterItemsName));
                if (null == PanelBackground) throw new ArgumentException(string.Format("Cannot find {0}.", PanelBackgroundName));
                if (null == PanelTitle) throw new ArgumentException(string.Format("Cannot find {0}.", PanelTitleName));
                if (null == PanelItems) throw new ArgumentException(string.Format("Cannot find {0}.", PanelItemsName));

                // reset panelhosts
                Background.Reset();
                Title.Reset();
                Items.Reset();

                // create transforms
                PanelBackground.RenderTransform = Background.Transform;
                PanelTitle.RenderTransform = Title.Transform;
                PanelItems.RenderTransform = Items.Transform;

                // fetch items details
                FrameworkElement item0 = null;
                FrameworkElement itemN = null;
                if (Parent.Items.Count > 0)
                {
                    int index0 = 0;
                    int indexN = Parent.Items.Count - 1;
                    item0 = Parent.Items[index0] as FrameworkElement;
                    itemN = Parent.Items[indexN] as FrameworkElement;
                }

                // reset panelhosts layout
                Title.Padding = LayoutRoot.ActualWidth;
                InitializeHost(PanelBackground, PresenterBackground, PresenterBackground, PresenterBackground, Background.Padding);
                InitializeHost(PanelTitle, PresenterTitle, PresenterTitle, PresenterTitle, Title.Padding);
                InitializeHost(PanelItems, PresenterItems, itemN, item0, Items.Padding);

                if (Parent.Items.Count > 0)
                {
                    double maxN = Math.Max(itemN.Width, LayoutRoot.ActualWidth);

                    // panelhosts width
                    Background.Width = PresenterBackground.ActualWidth;
                    Title.Width = PresenterTitle.ActualWidth;
                    Items.Width = Parent.Items.GetTotalWidth() - itemN.Width + maxN;

                    // panelhosts speed
                    Title.Speed = Background.Width / Items.Width;
                    Title.Speed = Title.Width / Items.Width;
                    if (Items.Width > maxN)
                        Background.Speed = (Background.Width - maxN) / (Items.Width - maxN);
                }

                // done
                ready = true;
            }
        }

        public void Reset(bool lazy = true)
        {
            ready = false;

            // reset now ?
            if (!lazy) Initialize();
        }

        public void MoveTo(double pos)
        {
            Initialize();

            // nothing to do
            if (Parent.Items.Count == 0)
                return;

            // move to new position
            Position = pos;
        }

        public void ScrollTo(double pos)
        {
            Initialize();

            // nothing to do
            if (Parent.Items.Count == 0)
                return;

            // animate to new position
            this.AnimateStart(-pos, AnimationDuration);
        }

        public void ScrollSkip()
        {
            Initialize();

            // nothing to do
            if (Parent.Items.Count == 0)
                return;

            // storyboard not completed yet
            // speed it up
            if ((null != Storyboard) &&
                (Storyboard.GetCurrentState() == ClockState.Active))
            {
                Storyboard.SkipToFill();
                Storyboard_Completed(Storyboard, new EventArgs());
                Storyboard = null;
            }
        }

        public double Position
        {
            get
            {
                Initialize();

                return -Items.Transform.X / Items.Speed;
            }
            set
            {
                Initialize();

                // nothing to do
                if (Parent.Items.Count == 0)
                    return;

                // complete current animation
                ScrollSkip();

                // adjust transforms
                Background.Transform.X = -value * Background.Speed;
                Title.Transform.X = -value * Title.Speed;
                Items.Transform.X = -value * Items.Speed;
            }
        }

        private void AnimateStart(double value, double milliseconds = 0)
        {
            Initialize();

            // no animation : just change the values
            if (milliseconds == 0)
            {
                Position = -value;
                return;
            }

            // adjust speed
            double offset = Math.Abs(value - Position);
            if (offset < LayoutRoot.ActualWidth)
            {
                milliseconds *= offset / LayoutRoot.ActualWidth;
            }

            double offsetBackground = value * Background.Speed;
            double offsetTitle = value * Title.Speed;
            double offsetItems = value * Items.Speed;

            // back to last
            if (value > 0)
            {
                offsetBackground = Items.Width - Parent.Items.GetLastItemPosition();
                offsetTitle = (Items.Width - Parent.Items.GetLastItemPosition()) * Title.Speed + Title.Padding;
                if (Parent.Items.Count == 1)
                {
                    // only 1 item : scroll the entire background
                    offsetBackground = Background.Width;
                }
            }
            // back to first
            else if (value <= -Parent.Items.GetTotalWidth())
            {
                offsetBackground = -Background.Width;
                offsetTitle = -(Title.Width + Title.Padding);
            }

            // start a new storyboard
            Storyboard = new Storyboard();
            Storyboard.Completed += new EventHandler(Storyboard_Completed);
            Storyboard.Children.Add(CreateAnimation(Background.Transform, offsetBackground, milliseconds));
            Storyboard.Children.Add(CreateAnimation(Title.Transform, offsetTitle, milliseconds));
            Storyboard.Children.Add(CreateAnimation(Items.Transform, offsetItems, milliseconds));
            Storyboard.Begin();
        }

        private DoubleAnimation CreateAnimation(TranslateTransform transform, double value, double milliseconds)
        {
            CubicEase easing = new CubicEase() { EasingMode = EasingMode.EaseOut };
            DoubleAnimation animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                From = Convert.ToInt32(transform.X),
                To = Convert.ToDouble(value),
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = easing
            };
            Storyboard.SetTarget(animation, transform);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));

            return animation;
        }


        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Storyboard sb = sender as Storyboard;
            if (null != sb)
                sb.Completed -= new EventHandler(Storyboard_Completed);

            // raise event for any listener out there
            if (null != AnimationCompleted)
                AnimationCompleted(this, new AnimationCompleteEventArgs() { Position = this.Position });
        }
    }
}
