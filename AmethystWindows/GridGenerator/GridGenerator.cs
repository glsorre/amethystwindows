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

    static class LayoutBackup
    {
        public static Func<GridDescripton, Rectangle[]> Horizontal = LayoutActions.horizontal;
        public static Func<GridDescripton, Rectangle[]> Vertical = LayoutActions.vertical;
        public static Func<GridDescripton, Rectangle[]> Monocle = LayoutActions.monocle;
        public static Func<GridDescripton, Rectangle[]> WideTop = LayoutActions.horizontal;
        public static Func<GridDescripton, Rectangle[]> WideBottom = LayoutActions.horizontal;
        public static Func<GridDescripton, Rectangle[]> TallLeft = LayoutActions.vertical;
        public static Func<GridDescripton, Rectangle[]> TallRight = LayoutActions.vertical;
        public static Func<GridDescripton, Rectangle[]> ThreeTallLeft = LayoutActions.vertical;
        public static Func<GridDescripton, Rectangle[]> ThreeTallRight = LayoutActions.vertical;

        public static Func<GridDescripton, Rectangle[]>[] ToArray() => new[] {
            Horizontal,
            Vertical,
            Monocle,
            WideTop,
            WideBottom,
            TallLeft,
            TallRight,
            ThreeTallLeft,
            ThreeTallRight,
        };
    }

    class GridDescripton
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
        Dictionary<Layout, Func<GridDescripton, Rectangle[]>> layouts;
        Dictionary<Layout, Func<GridDescripton, Rectangle[]>> backupLayouts;

        public GridGenerator()
        {
            layouts = new Dictionary<Layout, Func<GridDescripton, Rectangle[]>>();
            backupLayouts = new Dictionary<Layout, Func<GridDescripton, Rectangle[]>>();
    
            IEnumerable<Layout> layoutValues = Enum.GetValues(typeof(Layout)).Cast<Layout>();
            Func<GridDescripton, Rectangle[]>[] layoutFuncs = LayoutActions.ToArray();
            Func<GridDescripton, Rectangle[]>[] layoutFuncsBackups = LayoutBackup.ToArray();

            foreach (Layout layout in layoutValues)
            {
                layouts.Add(layout, layoutFuncs[((ushort)layout)]);
                backupLayouts.Add(layout, layoutFuncsBackups[((ushort)layout)]);
            }
        }

        public Rectangle[] Generate(Layout layout, int mWidth, int mHeight, int windowsCount, int layoutPadding, int factor, int step) {
            if (windowsCount == 0) return Array.Empty<Rectangle>();
            if (windowsCount == 1) return new[] { new Rectangle(0, 0, mWidth, mHeight) };
            
            GridDescripton gridDescripton = new GridDescripton(mWidth, mHeight, windowsCount, layoutPadding, factor, step);

            if (windowsCount == 2) return backupLayouts[layout](gridDescripton);

            Rectangle[] grid = layouts[layout](gridDescripton);

            foreach (var item in grid)
            {
                System.Diagnostics.Debug.WriteLine(item.ToString());
            }

            return grid;
        }
    }
}
