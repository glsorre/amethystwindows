using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AmethystWindows.GridGenerator
{
    static class EnhancedRectangle
    {
        public static Rectangle Padding(this Rectangle value, Point p)
        {
            return new Rectangle(value.X + p.X, value.Y + p.Y, value.Width - 2 * p.X, value.Height - 2 * p.Y);
        }
        public static Rectangle Offset(this Rectangle value, Point tl, Point br)
        {
            return new Rectangle(value.X + tl.X, value.Y + tl.Y, value.Width - tl.X + br.X, value.Height - tl.Y + br.Y);
        }

        public static Rectangle Translate(this Rectangle value, Point p)
        {
            return new Rectangle(value.X + p.X, value.Y + p.Y, value.Width, value.Height);
        }

    }
    
    class GridRectangle {
        private Point _padding;
        private Point _offsetTL;
        private Point _offsetBR;
        private Rectangle _window;
        private Point _monitor;
        private Point _borders;
        
        public Point Padding { get => _padding; set => _padding = value; }
        public Rectangle Window { get => _window; set => _window = value; }
        public Point Borders { get => _borders; set => _borders = value; }
        public Point Monitor { get => _monitor; set => _monitor = value; }
        public Point OffsetTL { get => _offsetTL; set => _offsetTL = value; }
        public Point OffsetBR { get => _offsetBR; set => _offsetBR = value; }

        public GridRectangle(Rectangle window, Point monitor, Point borders, Point offsetTL, Point offsetBR, Point padding)
        {
            _window = window;
            _monitor = monitor;
            _borders = borders;
            _padding = padding;
            _offsetTL = offsetTL;
            _offsetBR = offsetBR;
        }

        public Rectangle Position() {
            return Window.Translate(Monitor).Offset(OffsetTL, OffsetBR).Padding(Padding);
        }
    }
}
