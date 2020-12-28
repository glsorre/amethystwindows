using AmethystWindowsSystray;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmethystWindowsSystrayTests
{
    [TestClass]
    public class AmethystWindowsSystrayTests
    {
        DesktopWindowsManager desktopWindowsManager = new DesktopWindowsManager();
        Layout layout;

        [TestMethod]
        public void GridGeneratorWindowCountOne()
        {
            layout = Layout.Horizontal;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 1, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[1]);
        }

        [TestMethod]
        public void GridGeneratorCountTwo()
        {
            layout = Layout.Horizontal;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 1000, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 1000, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 1000, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 2, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[2]);
        }

        [TestMethod]
        public void GridGeneratorCountThree()
        {
            layout = Layout.Horizontal;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 333, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(333, 0, 333, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(666, 0, 333, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 333, 1000, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 666, 1000, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 3, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[3]);
        }

        [TestMethod]
        public void GridGeneratorCountFour()
        {
            layout = Layout.Horizontal;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 250, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(250, 0, 250, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 0, 250, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(750, 0, 250, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 250));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 250, 1000, 250));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 500, 1000, 250));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(0, 750, 1000, 250));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 500, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 500, 500, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Monocle;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(0, 0, 1000, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(333, 500, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(666, 500, 333, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 333, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 666, 500, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
        }

        [TestMethod]
        public void GridGeneratorCountFive()
        {
            layout = Layout.Horizontal;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 200, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(200, 0, 200, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(400, 0, 200, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(600, 0, 200, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(800, 0, 200, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Vertical;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 200));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 200, 1000, 200));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 400, 1000, 200));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(0, 600, 1000, 200));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(0, 800, 1000, 200));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.HorizGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 0, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 333, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(500, 666, 500, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 500, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(333, 500, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(666, 500, 333, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Tall;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 1000, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 250, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(250, 500, 250, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 500, 250, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(750, 500, 250, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
            layout = Layout.Wide;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 5, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 250));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 250, 500, 250));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 500, 500, 250));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(500, 750, 500, 250));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[5]);
        }

        [TestMethod]
        public void GridGeneratorCountSix()
        {
            layout = Layout.HorizGrid;
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 6, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(0, 500, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(333, 0, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(333, 500, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(666, 0, 333, 500));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[5], new Tuple<int, int, int, int>(666, 500, 333, 500));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[6]);
            layout = Layout.VertGrid;
            gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 6, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(500, 0, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(0, 333, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(500, 333, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[4], new Tuple<int, int, int, int>(0, 666, 500, 333));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[5], new Tuple<int, int, int, int>(500, 666, 500, 333));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[6]);
        }
    }
}
