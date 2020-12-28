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
        Layout layout = Layout.Horizontal;

        [TestMethod]
        public void GridGeneratorHorizontalTest()
        {
            IEnumerable<Tuple<int, int, int, int>> gridGenerator = desktopWindowsManager.GridGenerator(1000, 1000, 4, layout);
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[0], new Tuple<int, int, int, int>(0, 0, 250, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[1], new Tuple<int, int, int, int>(250, 0, 250, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[2], new Tuple<int, int, int, int>(500, 0, 250, 1000));
            Assert.AreEqual<Tuple<int, int, int, int>>(gridGenerator.ToArray()[3], new Tuple<int, int, int, int>(750, 0, 250, 1000));
            Assert.ThrowsException<IndexOutOfRangeException>(() => gridGenerator.ToArray()[4]);
        }
    }
}
