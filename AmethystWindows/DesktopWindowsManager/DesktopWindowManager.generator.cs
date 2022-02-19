using System;
using System.Collections.Generic;
using System.Drawing;
using Vanara.PInvoke;
using WindowsDesktop;

using AmethystWindows.Settings;

namespace AmethystWindows.DesktopWindowsManager
{
    partial class DesktopWindowsManager
    {
        public void ShrinkMainPane(Pair<VirtualDesktop, HMONITOR> key)
        {
            Factors[key] = ++Factors[key];
        }

        public void ExpandMainPane(Pair<VirtualDesktop, HMONITOR> key)
        {
            Factors[key] = --Factors[key];
        }

        public IEnumerable<Rectangle> GridGenerator(int mWidth, int mHeight, int windowsCount, int factor, Layout layout, int layoutPadding)
        {
            int i = 0;
            int j = 0;
            int horizStep;
            int vertStep;
            int tiles;
            int horizSize;
            int vertSize;
            bool isFirstLine;
            switch (layout)
            {
                case Layout.Horizontal:
                    horizSize = mWidth / windowsCount;
                    j = 0;
                    for (i = 0; i < windowsCount; i++)
                    {
                        int lastPadding = i == (windowsCount - 1) ? 0 : layoutPadding;
                        yield return new Rectangle(i * horizSize, j, horizSize - lastPadding, mHeight);
                    }
                    break;
                case Layout.Vertical:
                    vertSize = mHeight / windowsCount;
                    j = 0;
                    for (i = 0; i < windowsCount; i++)
                    {
                        int lastPadding = i == (windowsCount - 1) ? 0 : layoutPadding;
                        yield return new Rectangle(j, i * vertSize, mWidth, vertSize - lastPadding);
                    }
                    break;
                case Layout.HorizGrid:
                    horizStep = Math.Max((int)Math.Sqrt(windowsCount), 1);
                    vertStep = Math.Max(windowsCount / horizStep, 1);
                    tiles = horizStep * vertStep;
                    horizSize = mWidth / horizStep;
                    vertSize = mHeight / vertStep;
                    isFirstLine = true;

                    if (windowsCount != tiles || windowsCount == 3)
                    {
                        if (windowsCount == 3)
                        {
                            vertStep--;
                            vertSize = mHeight / vertStep;
                        }

                        while (windowsCount > 0)
                        {
                            int lastPaddingI = i == (horizStep - 1) ? 0 : layoutPadding;
                            int lastPaddingJ = j == (vertStep - 1) ? 0 : layoutPadding;
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize - lastPaddingI, vertSize - lastPaddingJ);
                            i++;
                            if (i >= horizStep)
                            {
                                i = 0;
                                j++;
                            }
                            if (j == vertStep - 1 && isFirstLine)
                            {
                                horizStep++;
                                horizSize = mWidth / horizStep;
                                isFirstLine = false;
                            }
                            windowsCount--;
                        }
                    }
                    else
                    {
                        while (windowsCount > 0)
                        {
                            int lastPaddingI = i == (horizStep - 1) ? 0 : layoutPadding;
                            int lastPaddingJ = j == (vertStep - 1) ? 0 : layoutPadding;
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize - lastPaddingI, vertSize - lastPaddingJ);
                            i++;
                            if (i >= horizStep)
                            {
                                i = 0;
                                j++;
                            }
                            windowsCount--;
                        }
                    }
                    break;
                case Layout.VertGrid:
                    vertStep = Math.Max((int)Math.Sqrt(windowsCount), 1);
                    horizStep = Math.Max(windowsCount / vertStep, 1);
                    tiles = horizStep * vertStep;
                    vertSize = mHeight / vertStep;
                    horizSize = mWidth / horizStep;
                    isFirstLine = true;

                    if (windowsCount != tiles || windowsCount == 3)
                    {
                        if (windowsCount == 3)
                        {
                            horizStep--;
                            horizSize = mWidth / horizStep;
                        }

                        while (windowsCount > 0)
                        {
                            int lastPaddingI = i == (horizStep - 1) ? 0 : layoutPadding;
                            int lastPaddingJ = j == (vertStep - 1) ? 0 : layoutPadding;
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize - lastPaddingI, vertSize - lastPaddingJ);
                            j++;
                            if (j >= vertStep)
                            {
                                j = 0;
                                i++;
                            }
                            if (i == horizStep - 1 && isFirstLine)
                            {
                                vertStep++;
                                vertSize = mHeight / vertStep;
                                isFirstLine = false;
                            }
                            windowsCount--;
                        }
                    }
                    else
                    {
                        while (windowsCount > 0)
                        {
                            int lastPaddingI = i == (horizStep - 1) ? 0 : layoutPadding;
                            int lastPaddingJ = j == (vertStep - 1) ? 0 : layoutPadding;
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize - lastPaddingI, vertSize - lastPaddingJ);
                            j++;
                            if (j >= vertStep)
                            {
                                j = 0;
                                i++;
                            }
                            windowsCount--;
                        }
                    }
                    break;
                case Layout.Monocle:
                    for (i = 0; i < windowsCount; i++)
                    {
                        yield return new Rectangle(0, 0, mWidth, mHeight);
                    }
                    break;
                case Layout.Wide:
                    if (windowsCount == 1) yield return new Rectangle(0, 0, mWidth, mHeight);
                    else
                    {
                        int size = mWidth / (windowsCount - 1);
                        for (i = 0; i < windowsCount - 1; i++)
                        {
                            int lastPaddingI = windowsCount == 1 ? 0 : layoutPadding;
                            int lastPaddingJ = i == (windowsCount - 2) ? 0 : layoutPadding;

                            if (i == 0) yield return new Rectangle(0, 0, mWidth, mHeight / 2 + factor * MySettings.Instance.Step - (lastPaddingI /2));
                            yield return new Rectangle(i * size, mHeight / 2 + factor * MySettings.Instance.Step + (lastPaddingI / 2), size - lastPaddingJ, mHeight / 2 - factor * MySettings.Instance.Step - (lastPaddingI /2));
                        }
                    }
                    break;
                case Layout.Tall:
                    if (windowsCount == 1) yield return new Rectangle(0, 0, mWidth, mHeight);
                    else
                    {
                        int size = mHeight / (windowsCount - 1);
                        for (i = 0; i < windowsCount - 1; i++)
                        {
                            int lastPaddingI = i == (windowsCount - 2) ? 0 : layoutPadding;
                            int lastPaddingJ = windowsCount == 1 ? 0 : layoutPadding;

                            if (i == 0) yield return new Rectangle(0, 0, mWidth / 2 + factor * MySettings.Instance.Step - (lastPaddingJ / 2), mHeight);
                            yield return new Rectangle(mWidth / 2 + factor * MySettings.Instance.Step + (lastPaddingJ / 2), i * size, mWidth / 2 - factor * MySettings.Instance.Step - (lastPaddingJ / 2), size - lastPaddingI);
                        }
                    }
                    break;
            }
        }

    }
}
