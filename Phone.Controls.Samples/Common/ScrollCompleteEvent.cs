using System.Windows.Media;

namespace Phone.Controls.Samples
{
    internal delegate void ScrollCompleteEventHandler(object sender, ScrollCompleteEventArgs e);

    internal class ScrollCompleteEventArgs
    {
        public int SelectedIndex;
        public ScrollCompleteEventArgs()
        {
        }
    }

    internal struct ScrollHost
    {
        public TranslateTransform Transform;
        public double Width;
        public double Padding;
        public double Speed;
        public void Reset() { Width = 0.0; Speed = 1.0; }
    }

}
