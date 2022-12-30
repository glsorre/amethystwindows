using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AmethystWindows.GridGenerator
{
    public static class LayoutActions
    {
        public static Rectangle[] Vertical(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int horizSize = gridDescription.MWidth / gridDescription.WindowsCount;
            int j = 0;

            for (int i = 0; i <= gridDescription.WindowsMaxIndex; i++)
            {
                int lastPadding = i == (gridDescription.WindowsMaxIndex) ? 0 : gridDescription.LayoutPadding / 2;
                result[i] = new Rectangle(i * horizSize, j, horizSize - lastPadding, gridDescription.MHeight);
            }

            return result;
        }
        
        public static Rectangle[] Horizontal(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int vertSize = gridDescription.MHeight / gridDescription.WindowsCount;
            int j = 0;
            for (int i = 0; i <= gridDescription.WindowsMaxIndex; i++)
            {
                int lastPadding = i == (gridDescription.WindowsMaxIndex) ? 0 : gridDescription.LayoutPadding / 2;
                result[i] = new Rectangle(j, i * vertSize, gridDescription.MWidth, vertSize - lastPadding);
            }

            return result;
        }

        public static Rectangle[] Monocle(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            for (int i = 0; i <= gridDescription.WindowsMaxIndex; i++)
            {
                result[i] = new Rectangle(0, 0, gridDescription.MWidth, gridDescription.MHeight);
            }

            return result;
        }

        public static Rectangle[] WideTop(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MWidth / (gridDescription.WindowsMaxIndex);
            int mainPaneResize = gridDescription.Step * gridDescription.Factor;
            
            result[0] = new Rectangle(0, 0, gridDescription.MWidth, gridDescription.MHeight / 2 - gridDescription.LayoutPadding / 2 + mainPaneResize);
            Rectangle[] subResult = Vertical(new GridDescripton(gridDescription.MWidth, gridDescription.MHeight / 2 + gridDescription.LayoutPadding / 2 - mainPaneResize, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                subResult[i].Y += gridDescription.MHeight / 2 + mainPaneResize;
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Rectangle[] WideBottom(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MWidth / (gridDescription.WindowsMaxIndex);
            int mainPaneResize = gridDescription.Step * gridDescription.Factor;

            result[0] = new Rectangle(0, gridDescription.MHeight / 2 + gridDescription.LayoutPadding / 2 + mainPaneResize, gridDescription.MWidth, gridDescription.MHeight / 2 - mainPaneResize);
            Rectangle[] subResult = Vertical(new GridDescripton(gridDescription.MWidth, gridDescription.MHeight / 2 + mainPaneResize, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Rectangle[] TallLeft(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            int mainPaneResize = gridDescription.Step * gridDescription.Factor;

            result[0] = new Rectangle(0, 0, gridDescription.MWidth / 2 - gridDescription.LayoutPadding / 2 + mainPaneResize, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 2 - gridDescription.LayoutPadding / 2 - mainPaneResize, gridDescription.MHeight, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                subResult[i].X += gridDescription.MWidth / 2 + gridDescription.LayoutPadding / 2 + mainPaneResize;
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Rectangle[] TallRight(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            int mainPaneResize = gridDescription.Step * gridDescription.Factor;

            result[0] = new Rectangle(gridDescription.MWidth / 2 + gridDescription.LayoutPadding / 2 - mainPaneResize, 0, gridDescription.MWidth / 2 - gridDescription.LayoutPadding / 2 + mainPaneResize, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 2 - gridDescription.LayoutPadding / 2 - mainPaneResize, gridDescription.MHeight, gridDescription.WindowsCount - 1, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                result[i + 1] = subResult[i];
            }

            return result;
        }

        public static Rectangle[] ThreeTallLeft(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            int mainPaneResize = gridDescription.Step * gridDescription.Factor;

            result[1] = new Rectangle(0, 0, gridDescription.MWidth / 3 - gridDescription.LayoutPadding / 2 - mainPaneResize, gridDescription.MHeight);
            result[0] = new Rectangle(gridDescription.MWidth / 3 + gridDescription.LayoutPadding / 2 - mainPaneResize, 0, gridDescription.MWidth / 3 - gridDescription.LayoutPadding + mainPaneResize * 2, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 3 - gridDescription.LayoutPadding / 2 + mainPaneResize, gridDescription.MHeight, gridDescription.WindowsCount - 2, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                subResult[i].X += 2 * gridDescription.MWidth / 3 + gridDescription.LayoutPadding / 2 + mainPaneResize;
                subResult[i].Width -= mainPaneResize * 2;
                result[i + 2] = subResult[i];
            }

            return result;
        }

        public static Rectangle[] ThreeTallRight(GridDescripton gridDescription)
        {
            Rectangle[] result = new Rectangle[gridDescription.WindowsCount];

            int size = gridDescription.MHeight / (gridDescription.WindowsMaxIndex);
            int mainPaneResize = gridDescription.Step * gridDescription.Factor;

            result[1] = new Rectangle(2 * gridDescription.MWidth / 3 + gridDescription.LayoutPadding / 2 + mainPaneResize, 0, gridDescription.MWidth / 3 - gridDescription.LayoutPadding / 2 - mainPaneResize, gridDescription.MHeight);
            result[0] = new Rectangle(gridDescription.MWidth / 3 + gridDescription.LayoutPadding / 2 - mainPaneResize, 0, gridDescription.MWidth / 3 - gridDescription.LayoutPadding + mainPaneResize * 2, gridDescription.MHeight);
            Rectangle[] subResult = Horizontal(new GridDescripton(gridDescription.MWidth / 3 - gridDescription.LayoutPadding / 2 - mainPaneResize, gridDescription.MHeight, gridDescription.WindowsCount - 2, gridDescription.LayoutPadding, gridDescription.Factor, gridDescription.Step));
            for (int i = 0; i < subResult.Length; i++)
            {
                result[i + 2] = subResult[i];
            }

            return result;
        }
    };

    public static class LayoutBackup
    {
        public static Func<GridDescripton, Rectangle[]> Horizontal = LayoutActions.Horizontal;
        public static Func<GridDescripton, Rectangle[]> Vertical = LayoutActions.Vertical;
        public static Func<GridDescripton, Rectangle[]> Monocle = LayoutActions.Monocle;
        public static Func<GridDescripton, Rectangle[]> WideTop = LayoutActions.Horizontal;
        public static Func<GridDescripton, Rectangle[]> WideBottom = LayoutActions.Horizontal;
        public static Func<GridDescripton, Rectangle[]> TallLeft = LayoutActions.Vertical;
        public static Func<GridDescripton, Rectangle[]> TallRight = LayoutActions.Vertical;
        public static Func<GridDescripton, Rectangle[]> ThreeTallLeft = LayoutActions.Vertical;
        public static Func<GridDescripton, Rectangle[]> ThreeTallRight = LayoutActions.Vertical;
    }
}
