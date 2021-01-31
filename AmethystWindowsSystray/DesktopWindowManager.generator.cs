using DesktopWindowManager.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop;

[assembly: InternalsVisibleTo("AmethystWindowsSystrayTests")]
namespace AmethystWindowsSystray
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

        public IEnumerable<Rectangle> GridGenerator(int mWidth, int mHeight, int windowsCount, int factor, Layout layout)
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
                        yield return new Rectangle(i * horizSize, j, horizSize, mHeight);
                    }
                    break;
                case Layout.Vertical:
                    vertSize = mHeight / windowsCount;
                    j = 0;
                    for (i = 0; i < windowsCount; i++)
                    {
                        yield return new Rectangle(j, i * vertSize, mWidth, vertSize);
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
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize, vertSize);
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
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize, vertSize);
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
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize, vertSize);
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
                            yield return new Rectangle(i * horizSize, j * vertSize, horizSize, vertSize);
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
                            if (i == 0) yield return new Rectangle(0, 0, mWidth, mHeight / 2 + factor * Properties.Settings.Default.Step);
                            yield return new Rectangle(i * size, mHeight / 2 + factor * Properties.Settings.Default.Step, size, mHeight / 2 - factor * Properties.Settings.Default.Step);
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
                            if (i == 0) yield return new Rectangle(0, 0, mWidth / 2 + factor * Properties.Settings.Default.Step, mHeight);
                            yield return new Rectangle(mWidth / 2 + factor * Properties.Settings.Default.Step, i * size, mWidth / 2 - factor * Properties.Settings.Default.Step, size);
                        }
                    }
                    break;
            }
        }

    }
}
