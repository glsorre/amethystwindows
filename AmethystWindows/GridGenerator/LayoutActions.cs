using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AmethystWindows.GridGenerator
{
    static class LayoutActions
    {
        public static Rectangle[] vertical(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int horizSize = gridDescription.MWidth / gridDescription.WindowsCount;
            int j = 0;
            for (int i = 0; i <= gridDescription.WindowsMaxIndex; i++)
            {
                int lastPadding = i == (gridDescription.WindowsMaxIndex) ? 0 : gridDescription.LayoutPadding;
                result[i] = new Rectangle(i * horizSize, j, horizSize - lastPadding, gridDescription.MHeight);
            }

            return result;
        }
        
        public static Func<GridDescripton, Rectangle[]> Vertical = vertical;

        public static Rectangle[] horizontal(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int vertSize = gridDescription.MHeight / gridDescription.WindowsCount;
            int j = 0;
            for (int i = 0; i <= gridDescription.WindowsMaxIndex; i++)
            {
                int lastPadding = i == (gridDescription.WindowsMaxIndex) ? 0 : gridDescription.LayoutPadding;
                result[i] = new Rectangle(j, i * vertSize, gridDescription.MWidth, vertSize - lastPadding);
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> Horizontal = horizontal;

        public static Rectangle[] monocle(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            for (int i = 0; i <= gridDescription.WindowsMaxIndex; i++)
            {
                result[i] = new Rectangle(0, 0, gridDescription.MWidth, gridDescription.MHeight);
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> Monocle = monocle;

        public static Rectangle[] wideTop(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MWidth / (gridDescription.WindowsMaxIndex);
            
            result[0] = new Rectangle(0, 0, gridDescription.MWidth, gridDescription.MHeight / 2);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth, gridDescription.MHeight / 2, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                subResult[i].Y += gridDescription.MHeight / 2;
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> WideTop = wideTop;

        public static Rectangle[] wideBottom(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MWidth / (gridDescription.WindowsMaxIndex);

            result[0] = new Rectangle(0, gridDescription.MHeight / 2, gridDescription.MWidth, gridDescription.MHeight / 2);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth, gridDescription.MHeight / 2, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> WideBottom = wideBottom;

        public static Rectangle[] tallLeft(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            result[0] = new Rectangle(0, 0, gridDescription.MWidth / 2, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 2, gridDescription.MHeight, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                subResult[i].X += gridDescription.MWidth / 2;
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> TallLeft = tallLeft;

        public static Rectangle[] tallRight(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            result[0] = new Rectangle(gridDescription.MWidth / 2, 0, gridDescription.MWidth / 2, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 2, gridDescription.MHeight, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> TallRight = tallRight;

        public static Rectangle[] threeTallLeft(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            result[0] = new Rectangle(0, 0, gridDescription.MWidth / 3, gridDescription.MHeight);
            result[1] = new Rectangle(gridDescription.MWidth / 3, 0, gridDescription.MWidth / 3, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 3, gridDescription.MHeight, gridDescription.WindowsCount - 2, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                subResult[i].X += 2 * gridDescription.MWidth / 3;
                result[i + 2] = subResult[i];
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> ThreeTallLeft = threeTallLeft;

        public static Rectangle[] threeTallRight(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            result[0] = new Rectangle(2 * gridDescription.MWidth / 3, 0, gridDescription.MWidth / 3, gridDescription.MHeight);
            result[1] = new Rectangle(gridDescription.MWidth / 3, 0, gridDescription.MWidth / 3, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 3, gridDescription.MHeight, gridDescription.WindowsCount - 2, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                result[i + 2] = subResult[i];
            }

            return result;
        }

        public static Func<GridDescripton, Rectangle[]> ThreeTallRight = threeTallRight;

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
    };
}
