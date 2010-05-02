using System.Windows.Controls;
using System.Windows;
using System;

namespace PictureViewer
{
    public class MultiColumnsPanel : Panel
    {
        public MultiColumnsPanel() :
            base()
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size finalSize = new Size(0, 0);

            // loop thru childs
            int idx = 0;
            while (idx < this.Children.Count)
            {
                // reset measures for this line
                double width = 0.0;
                double height = 0.0;

                // limit to max columns 
                for (int col = 0; col < this.Columns; col++, idx++)
                {
                    if (idx >= this.Children.Count)
                        break;

                    // measure child
                    UIElement child = Children[idx];
                    if (null != child)
                    {
                        child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                        // record measures for this line (cumulate width, max height)
                        width += child.DesiredSize.Width;
                        height = Math.Max(height, child.DesiredSize.Height);
                    }
                }

                // final measures (max width, cumulate height)
                finalSize.Width = Math.Max(width, finalSize.Width);
                finalSize.Height += height;
            }

            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Point pos = new Point();

            // loop thru childs
            int idx = 0;
            while (idx < this.Children.Count)
            {
                // reset to beginning of line
                double height = 0.0;

                // limit to max columns 
                for (int col = 0; col < this.Columns; col++, idx++)
                {
                    if (idx >= this.Children.Count)
                        break;

                    // arrange child
                    UIElement child = Children[idx];
                    if (null != child)
                    {
                        child.Arrange(new Rect(pos, child.DesiredSize));

                        // record measures for this line (cumulate width, max height)
                        pos.X += child.DesiredSize.Width;
                        height = Math.Max(height, child.DesiredSize.Height);
                    }
                }

                // final measures (reset width, cumulate height)
                pos.X = 0.0;
                pos.Y += height;
            }

            return finalSize;
        }

        #region Columns
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                "Columns",
                typeof(int),
                typeof(MultiColumnsPanel),
                new PropertyMetadata(OnColumnsChanged));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiColumnsPanel ctrl = (MultiColumnsPanel)d;
            ctrl.OnColumnsChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnColumnsChanged(object oldHeader, object newHeader)
        {
        }
        #endregion Columns
    }
}
