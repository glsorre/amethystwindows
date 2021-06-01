using AmethystWindowsSystray;
using DesktopMonitorManager.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AmethystWindowsSystrayTests
{
    [TestClass]
    public class AmethystWindowsSystrayTests
    {
        Layout layout;

        [TestMethod]
        public void GridGeneratorCountOne()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Vertical;
            gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.HorizGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.VertGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Monocle;
            gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Tall;
            gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Wide;
            gridGenerator = GridGenerator.Generator(1000, 1000, 1, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
        }

        [TestMethod]
        public void GridGeneratorCountTwo()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Vertical;
            gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 1000, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.VertGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.HorizGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 1000, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Monocle;
            gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Wide;
            gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 1000, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Tall;
            gridGenerator = GridGenerator.Generator(1000, 1000, 2, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
        }

        [TestMethod]
        public void GridGeneratorCountThree()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 333, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(333, 0, 333, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(666, 0, 333, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Vertical;
            gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 333, 1000, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 666, 1000, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.VertGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.HorizGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Monocle;
            gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 0, 1000, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Wide;
            gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Tall;
            gridGenerator = GridGenerator.Generator(1000, 1000, 3, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
        }

        [TestMethod]
        public void GridGeneratorCountFour()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 250, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(250, 0, 250, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 0, 250, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(750, 0, 250, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Vertical;
            gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 250));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 250, 1000, 250));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 500, 1000, 250));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(0, 750, 1000, 250));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.VertGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.HorizGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 500, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Monocle;
            gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 0, 1000, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 0, 1000, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Wide;
            gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(333, 500, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(666, 500, 333, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Tall;
            gridGenerator = GridGenerator.Generator(1000, 1000, 4, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 333, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 666, 500, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
        }

        [TestMethod]
        public void GridGeneratorCountFive()
        {
            layout = Layout.Horizontal;
            IEnumerable<Rectangle> gridGenerator = GridGenerator.Generator(1000, 1000, 5, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 200, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(200, 0, 200, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(400, 0, 200, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(600, 0, 200, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(800, 0, 200, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Vertical;
            gridGenerator = GridGenerator.Generator(1000, 1000, 5, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 200));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 200, 1000, 200));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 400, 1000, 200));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(0, 600, 1000, 200));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(0, 800, 1000, 200));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.VertGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 5, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 0, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 333, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(500, 666, 500, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.HorizGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 5, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 500, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(333, 500, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(666, 500, 333, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Wide;
            gridGenerator = GridGenerator.Generator(1000, 1000, 5, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 1000, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 250, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(250, 500, 250, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 250, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(750, 500, 250, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Tall;
            gridGenerator = GridGenerator.Generator(1000, 1000, 5, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 1000));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 250));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(500, 250, 500, 250));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 500, 500, 250));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(500, 750, 500, 250));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
        }

        [TestMethod]
        public void GridGeneratorCountSix()
        {
            layout = Layout.VertGrid;
            IEnumerable<Rectangle> gridGenerator = GridGenerator.Generator(1000, 1000, 6, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(0, 500, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(333, 0, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(333, 500, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(666, 0, 333, 500));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[5], new Rectangle(666, 500, 333, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[6]);
            layout = Layout.HorizGrid;
            gridGenerator = GridGenerator.Generator(1000, 1000, 6, 0, layout, 0);
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[0], new Rectangle(0, 0, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[1], new Rectangle(500, 0, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[2], new Rectangle(0, 333, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[3], new Rectangle(500, 333, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[4], new Rectangle(0, 666, 500, 333));
            Assert.AreEqual<Rectangle>(gridGenerator.ToArray()[5], new Rectangle(500, 666, 500, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[6]);
        }
    }
}
