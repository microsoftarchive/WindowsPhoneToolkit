using Phone.Controls.Samples;

namespace System.Windows.Controls
{
    public static class ItemCollectionExtensions
    {
        public static PanoramaItem GetItem(this ItemCollection items, int index)
        {
            if ((index >= 0) && (index < items.Count))
                return (PanoramaItem)items[index];

            return null;
        }

        public static PanoramaItem GetLastItem(this ItemCollection items)
        {
            if (items.Count == 0)
                return null;

            return (PanoramaItem)items[items.Count - 1];
        }

        public static int GetIndexOfPosition(this ItemCollection items, double position)
        {
            if (items.Count == 0)
                return -1;

            // far left : back to last item
            if (position < 0)
                return items.Count - 1;

            double start = 0.0;
            for (int i = 0; i < items.Count; i++)
            {
                PanoramaItem item = (PanoramaItem)items[i];
                if ((position >= start) && (position < start + item.Width))
                    return i;

                start += item.Width;
            }

            // far right : assume first
            return 0;
        }

        public static double GetItemPosition(this ItemCollection items, int index)
        {
            double position = 0.0;
            if ((index >= 0) && (index < items.Count))
            {
                for (int i = 0; i != index; i++)
                {
                    PanoramaItem item = (PanoramaItem)items[i];
                    position += item.Width;
                }
            }

            return position;
        }

        public static double GetLastItemPosition(this ItemCollection items)
        {
            return items.GetItemPosition(items.Count - 1);
        }

        public static double GetItemWidth(this ItemCollection items, int index)
        {
            if ((index >= 0) && (index < items.Count))
                return ((PanoramaItem)items[index]).Width;

            return 0.0;
        }

        public static double GetTotalWidth(this ItemCollection items)
        {
            PanoramaItem item = items.GetLastItem();
            if (null == item)
                return 0.0;

            return items.GetLastItemPosition() + item.Width;
        }
    }
}
