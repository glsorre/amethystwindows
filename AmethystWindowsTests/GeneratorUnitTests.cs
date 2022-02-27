using AmethystWindows.DesktopWindowsManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Xunit;

namespace AmethystWindowsTests
{
    public class GeneratorUnitTests
    {
        DesktopWindowsManager desktopWindowsManager = new DesktopWindowsManager();
        Layout layout;

        [Fact]
        public void GridGeneratorCountOne()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
        }

        [Fact]
        public void GridGeneratorCountTwo()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 1000, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 1000, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 1000, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
        }

        [Fact]
        public void GridGeneratorCountThree()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 333, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(333, 0, 333, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(666, 0, 333, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 333, 1000, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 666, 1000, 333));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 0, 1000, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
        }

        [Fact]
        public void GridGeneratorCountFour()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 250, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(250, 0, 250, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 0, 250, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(750, 0, 250, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 250));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 250, 1000, 250));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 500, 1000, 250));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(0, 750, 1000, 250));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 500, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 500, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 500, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 0, 1000, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 0, 1000, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(0, 0, 1000, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(333, 500, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(666, 500, 333, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 333, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 666, 500, 333));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
        }

        [Fact]
        public void GridGeneratorCountFive()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 200, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(200, 0, 200, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(400, 0, 200, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(600, 0, 200, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(800, 0, 200, 1000));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 200));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 200, 1000, 200));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 400, 1000, 200));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(0, 600, 1000, 200));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(0, 800, 1000, 200));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 0, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 333, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(500, 666, 500, 333));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 500, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(333, 500, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(666, 500, 333, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 250, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(250, 500, 250, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 250, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(750, 500, 250, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 250));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 250, 500, 250));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 500, 250));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(500, 750, 500, 250));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
        }

        [Fact]
        public void GridGeneratorCountSix()
        {
            layout = Layout.VertGrid;
            IEnumerable<Rectangle> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 6, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(333, 0, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(333, 500, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(666, 0, 333, 500));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[5], new Rectangle(666, 500, 333, 500));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[6]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 6, 0, layout, 0);
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 333, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 333, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(0, 666, 500, 333));
            Assert.Equal<Rectangle>(gridGenerator.ToArray()[5], new Rectangle(500, 666, 500, 333));
            Assert.Throws<IndexOutOfRangeException>(() => gridGenerator.ToArray()[6]);
        }
    }
}