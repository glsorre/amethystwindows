using AmethystWindows.GridGenerator;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using Vanara.Extensions;
using Vanara.Extensions.Reflection;

namespace AmethystWindows.GridGenerator
{
    public enum Layout : ushort
    {
        Horizontal = 0,
        Vertical = 1,
        Monocle = 2,
        WideTop = 3,
        WideBottom = 4,
        TallLeft = 5,
        TallRight = 6,
        ThreeTallLeft = 7,
        ThreeTallRight = 8,
    }

    public static class LayoutExtension
    {
        public static Func<GridDescripton, Rectangle[]> GetLayoutAction(this Layout layout)
        {
            return (Func<GridDescripton, Rectangle[]>)Delegate.CreateDelegate(typeof(Func<GridDescripton, Rectangle[]>), typeof(LayoutActions).GetMethod(layout.ToString()));
            }

        public static Func<GridDescripton, Rectangle[]> GetLayoutBackup(this Layout layout)
        {
            return typeof(LayoutBackup).GetField(layout.ToString()).GetValue(null) as Func<GridDescripton, Rectangle[]>;
        }
    }

    public class LayoutDescription
    {
        public Layout Layout { get; set; }
        public Func<GridDescripton, Rectangle[]> LayoutAction { get; set; }
        public Func<GridDescripton, Rectangle[]> LayoutActionBackup { get; set; }

        public LayoutDescription(Layout layout, Func<GridDescripton, Rectangle[]> layoutAction, Func<GridDescripton, Rectangle[]> layoutActionBackup)
        {
            Layout = layout;
            LayoutAction = layoutAction;
            LayoutActionBackup = layoutActionBackup;
        }
    }

    public class GridDescripton
    {
        int _mWidth;
        int _mHeight;
        int _windowsCount;
        int _layoutPadding;
        int _factor;
        int _step;
        int _windowsMaxIndex;

        public int MWidth { get => _mWidth; set => _mWidth = value; }
        public int MHeight { get => _mHeight; set => _mHeight = value; }
        public int WindowsCount { get => _windowsCount; set => _windowsCount = value; }

        public int WindowsMaxIndex  { get => _windowsMaxIndex; }
        public int LayoutPadding { get => _layoutPadding; set => _layoutPadding = value; }
        public int Factor { get => _factor; set => _factor = value; }
        public int Step { get => _step; set => _step = value; }

        public GridDescripton(int mWidth, int mHeight, int windowsCount, int layoutPadding, int factor, int step)
        {
            _mWidth = mWidth;
            _mHeight = mHeight;
            _windowsCount = windowsCount;
            _windowsMaxIndex = windowsCount - 1;
            _layoutPadding = layoutPadding;
            _factor = factor;
            _step = step;
        }
    }

    public class GridGenerator
    {
        LayoutDescription[] layouts;

        public GridGenerator()
        {
            IEnumerable<Layout> layoutValues = Enum.GetValues(typeof(Layout)).Cast<Layout>();

            layouts = new LayoutDescription[layoutValues.Count()];

            foreach (Layout layout in layoutValues)
            {
                layouts[(int)layout] = new LayoutDescription(layout, layout.GetLayoutAction(), layout.GetLayoutBackup());
            }
        }

        public Rectangle[] Generate(Layout layout, int mWidth, int mHeight, int windowsCount, int layoutPadding, int factor, int step) {
            if (windowsCount == 0) return Array.Empty<Rectangle>();
            if (windowsCount == 1) return new[] { new Rectangle(0, 0, mWidth, mHeight) };
            
            GridDescripton gridDescripton = new GridDescripton(mWidth, mHeight, windowsCount, layoutPadding, factor, step);

            if (windowsCount == 2) return layouts[(int)layout].LayoutActionBackup(gridDescripton);

            Rectangle[] grid = layouts[(int)layout].LayoutAction(gridDescripton);

            foreach (var item in grid)
            {
                System.Diagnostics.Debug.WriteLine(item.ToString());
            }

            return grid;
        }
    }
}
