using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using WindowsDesktop.Internal;

namespace WindowsDesktop
{
    public partial class VirtualDesktop
    {
        private IVirtualDesktop ivd;
        private VirtualDesktop(IVirtualDesktop desktop) => this.ivd = desktop;
        private static readonly ConcurrentDictionary<Guid, VirtualDesktop> _wrappers = new ConcurrentDictionary<Guid, VirtualDesktop>();

        public override int GetHashCode()
        { // get hash
            return ivd.GetHashCode();
        }

        public override bool Equals(object obj)
        { // compare with object
            var desk = obj as VirtualDesktop;
            return desk != null && object.ReferenceEquals(this.ivd, desk.ivd);
        }

        public static int Count
        { // return the number of desktops
            get { int count; DesktopManager.VirtualDesktopManagerInternal.GetDesktops(IntPtr.Zero).GetCount(out count); return count; }
        }

        public static VirtualDesktop Current
        { // returns current desktop
            get { return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop("")); }
        }

        public static VirtualDesktop FromIndex(int index)
        { // return desktop object from index (-> index = 0..Count-1)
            return new VirtualDesktop(DesktopManager.GetDesktop(index));
        }

        public static VirtualDesktop FromHwnd(HWND hWnd)
        { // return desktop object to desktop on which window <hWnd> is displayed
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            Guid id = DesktopManager.VirtualDesktopManager.GetWindowDesktopId(hWnd);
            return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.FindDesktop(id));
        }

        public static int FromDesktop(VirtualDesktop desktop)
        { // return index of desktop object or -1 if not found
            return DesktopManager.GetDesktopIndex(desktop.ivd);
        }

        public static string DesktopNameFromDesktop(VirtualDesktop desktop)
        { // return name of desktop or "Desktop n" if it has no name
            Guid guid = desktop.ivd.GetId();

            // read desktop name in registry
            string desktopName = null;
            try
            {
                desktopName = (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + guid.ToString() + "}", "Name", null);
            }
            catch { }

            // no name found, generate generic name
            if (string.IsNullOrEmpty(desktopName))
            { // create name "Desktop n" (n = number starting with 1)
                desktopName = "Desktop " + (DesktopManager.GetDesktopIndex(desktop.ivd) + 1).ToString();
            }
            return desktopName;
        }

        public static string DesktopNameFromIndex(int index)
        { // return name of desktop from index (-> index = 0..Count-1) or "Desktop n" if it has no name
            Guid guid = DesktopManager.GetDesktop(index).GetId();

            // read desktop name in registry
            string desktopName = null;
            try
            {
                desktopName = (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + guid.ToString() + "}", "Name", null);
            }
            catch { }

            // no name found, generate generic name
            if (string.IsNullOrEmpty(desktopName))
            { // create name "Desktop n" (n = number starting with 1)
                desktopName = "Desktop " + (index + 1).ToString();
            }
            return desktopName;
        }

        public static bool HasDesktopNameFromIndex(int index)
        { // return true is desktop is named or false if it has no name
            Guid guid = DesktopManager.GetDesktop(index).GetId();

            // read desktop name in registry
            string desktopName = null;
            try
            {
                desktopName = (string)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + guid.ToString() + "}", "Name", null);
            }
            catch { }

            // name found?
            if (string.IsNullOrEmpty(desktopName))
                return false;
            else
                return true;
        }

        public static int SearchDesktop(string partialName)
        { // get index of desktop with partial name, return -1 if no desktop found
            int index = -1;
            int count;
            DesktopManager.VirtualDesktopManagerInternal.GetDesktops(IntPtr.Zero).GetCount(out count);
            for (int i = 0; i < count; i++)
            { // loop through all virtual desktops and compare partial name to desktop name
                if (DesktopNameFromIndex(i).ToUpper().IndexOf(partialName.ToUpper()) >= 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static VirtualDesktop Create()
        { // create a new desktop
            return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.CreateDesktopW(""));
        }

        public void Remove(VirtualDesktop fallback = null)
        { // destroy desktop and switch to <fallback>
            IVirtualDesktop fallbackdesktop;
            if (fallback == null)
            { // if no fallback is given use desktop to the left except for desktop 0.
                VirtualDesktop dtToCheck = new VirtualDesktop(DesktopManager.GetDesktop(0));
                if (this.Equals(dtToCheck))
                { // desktop 0: set fallback to second desktop (= "right" desktop)
                    fallbackdesktop = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 4); // 4 = RightDirection
                }
                else
                { // set fallback to "left" desktop
                    fallbackdesktop = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 3); // 3 = LeftDirection
                }
            }
            else
                // set fallback desktop
                fallbackdesktop = fallback.ivd;

            DesktopManager.VirtualDesktopManagerInternal.RemoveDesktop(ivd, fallbackdesktop);
        }

        public void SetName(string Name)
        { // set name for desktop, empty string removes names
            if (DesktopManager.VirtualDesktopManagerInternal2 != null)
            { // only if interface to set name is present
                DesktopManager.VirtualDesktopManagerInternal2.SetName(this.ivd, Name);
            }
        }

        public bool IsVisible
        { // return true if this desktop is the current displayed one
            get { return object.ReferenceEquals(ivd, DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop("")); }
        }

        public void MakeVisible()
        { // make this desktop visible
            DesktopManager.VirtualDesktopManagerInternal.SwitchDesktop("", ivd);
        }

        public VirtualDesktop Left
        { // return desktop at the left of this one, null if none
            get
            {
                IVirtualDesktop desktop;
                desktop = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 3); // 3 = LeftDirection
                if (desktop != null)
                    return new VirtualDesktop(desktop);
                else
                    return null;
            }
        }

        public VirtualDesktop Right
        { // return desktop at the right of this one, null if none
            get
            {
                IVirtualDesktop desktop;
                desktop = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 4); // 4 = RightDirection
                if (desktop != null)
                    return new VirtualDesktop(desktop);
                else
                    return null;
            }
        }

        public void MoveWindow(HWND hWnd)
        { // move window to this desktop
            uint processId;
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            User32.GetWindowThreadProcessId(hWnd, out processId);

            if (System.Diagnostics.Process.GetCurrentProcess().Id == processId)
            { // window of process
                try // the easy way (if we are owner)
                {
                    DesktopManager.VirtualDesktopManager.MoveWindowToDesktop(hWnd, ivd.GetId());
                }
                catch // window of process, but we are not the owner
                {
                    IApplicationView view;
                    DesktopManager.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
                    DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, ivd);
                }
            }
            else
            { // window of other process
                IApplicationView view;
                DesktopManager.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
                try
                {
                    DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, ivd);
                }
                catch
                { // could not move active window, try main window (or whatever windows thinks is the main window)
                    DesktopManager.ApplicationViewCollection.GetViewForHwnd(System.Diagnostics.Process.GetProcessById((int)processId).MainWindowHandle, out view);
                    DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, ivd);
                }
            }
        }

        public void MoveActiveWindow()
        { // move active window to this desktop
            MoveWindow(User32.GetForegroundWindow());
        }

        public bool HasWindow(HWND hWnd)
        { // return true if window is on this desktop
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            return ivd.GetId() == DesktopManager.VirtualDesktopManager.GetWindowDesktopId(hWnd);
        }

        public static bool IsWindowPinned(HWND hWnd)
        { // return true if window is pinned to all desktops
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            return DesktopManager.VirtualDesktopPinnedApps.IsViewPinned(hWnd.GetApplicationView());
        }

        public static void PinWindow(HWND hWnd)
        { // pin window to all desktops
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            var view = hWnd.GetApplicationView();
            if (!DesktopManager.VirtualDesktopPinnedApps.IsViewPinned(view))
            { // pin only if not already pinned
                DesktopManager.VirtualDesktopPinnedApps.PinView(view);
            }
        }

        public static void UnpinWindow(HWND hWnd)
        { // unpin window from all desktops
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            var view = hWnd.GetApplicationView();
            if (DesktopManager.VirtualDesktopPinnedApps.IsViewPinned(view))
            { // unpin only if not already unpinned
                DesktopManager.VirtualDesktopPinnedApps.UnpinView(view);
            }
        }

        public static bool IsApplicationPinned(HWND hWnd)
        { // return true if application for window is pinned to all desktops
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            return DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned(DesktopManager.GetAppId(hWnd));
        }

        public static void PinApplication(HWND hWnd)
        { // pin application for window to all desktops
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            string appId = DesktopManager.GetAppId(hWnd);
            if (!DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
            { // pin only if not already pinned
                DesktopManager.VirtualDesktopPinnedApps.PinAppID(appId);
            }
        }

        public static void UnpinApplication(HWND hWnd)
        { // unpin application for window from all desktops
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            var view = hWnd.GetApplicationView();
            string appId = DesktopManager.GetAppId(hWnd);
            if (DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned(appId))
            { // unpin only if pinned
                DesktopManager.VirtualDesktopPinnedApps.UnpinAppID(appId);
            }
        }

        internal static VirtualDesktop FromComObject(IVirtualDesktop desktop)
        {
            var wrapper = _wrappers.GetOrAdd(desktop.GetId(), _ => new VirtualDesktop(desktop));
            return wrapper;
        }

        public override string ToString()
        {
            return $"{this.GetHashCode()}";
        }
    }

    public partial class VirtualDesktop
    {
        internal const int RPC_S_SERVER_UNAVAILABLE = unchecked((int)0x800706BA);
        private static uint? dwCookie;
        private static VirtualDesktopNotificationListener listener;

        /// <summary>
        /// Occurs when a virtual desktop is created.
        /// </summary>
        public static event EventHandler<VirtualDesktop> Created;
        public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyBegin;
        public static event EventHandler<VirtualDesktopDestroyEventArgs> DestroyFailed;

        /// <summary>
        /// Occurs when a virtual desktop is destroyed.
        /// </summary>
        public static event EventHandler<VirtualDesktopDestroyEventArgs> Destroyed;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static event EventHandler ApplicationViewChanged;

        /// <summary>
        /// Occurs when a current virtual desktop is changed.
        /// </summary>
        public static event EventHandler<VirtualDesktopChangedEventArgs> CurrentChanged;

        public static IDisposable RegisterListener()
        {
            var service = DesktopManager.VirtualDesktopNotificationService;
            listener = new VirtualDesktopNotificationListener();
            dwCookie = service.Register(listener);

            return Disposable.Create(() => {
                try
                {
                    service.Unregister(dwCookie.Value);
                }
                catch (COMException e) when (e.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    // no need to unregister when service is gone
                }
            });
        }

        private class VirtualDesktopNotificationListener : IVirtualDesktopNotification
        {
            static VirtualDesktop FromComObject(IVirtualDesktop virtualDesktop) =>
                VirtualDesktop.FromComObject(virtualDesktop);

            void IVirtualDesktopNotification.VirtualDesktopCreated(IVirtualDesktop pDesktop)
            {
                Created?.Invoke(this, FromComObject(pDesktop));
            }

            void IVirtualDesktopNotification.VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
            {
                var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
                DestroyBegin?.Invoke(this, args);
            }

            void IVirtualDesktopNotification.VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
            {
                var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
                DestroyFailed?.Invoke(this, args);
            }

            void IVirtualDesktopNotification.VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback)
            {
                var args = new VirtualDesktopDestroyEventArgs(FromComObject(pDesktopDestroyed), FromComObject(pDesktopFallback));
                Destroyed?.Invoke(this, args);
            }

            void IVirtualDesktopNotification.ViewVirtualDesktopChanged(IntPtr pView)
            {
                ApplicationViewChanged?.Invoke(this, EventArgs.Empty);
            }

            void IVirtualDesktopNotification.CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew)
            {
                var args = new VirtualDesktopChangedEventArgs(FromComObject(pDesktopOld), FromComObject(pDesktopNew));
                CurrentChanged?.Invoke(this, args);
            }

            void IVirtualDesktopNotification.UnknownProc8(IObjectArray p0, IVirtualDesktop p1, int p2, int p3)
            {
            }

            void IVirtualDesktopNotification.UnknownProc9(IVirtualDesktop p0, string p1)
            {
            }

            void IVirtualDesktopNotification.UnknownProc10(IApplicationView p0)
            {
             
            }

            void IVirtualDesktopNotification.UnknownProc12(IVirtualDesktop p0, string p1)
            {
            }
        }
    }
}

