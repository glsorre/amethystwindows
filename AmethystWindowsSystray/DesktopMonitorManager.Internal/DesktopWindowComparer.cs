using DesktopMonitorManager.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMonitorManager.Internal
{
    // Custom comparer for the Product class
    class DesktopWindowComparer : IEqualityComparer<DesktopWindow>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(DesktopWindow x, DesktopWindow y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Window == y.Window;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(DesktopWindow desktopWindow)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(desktopWindow, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = desktopWindow.Window == null ? 0 : desktopWindow.Window.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName;
        }
    }
}
