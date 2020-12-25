using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace WindowsDesktop
{
    #region COM API
    internal static class Guids
    {
        public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
        public static readonly Guid CLSID_VirtualDesktopManagerInternal = new Guid("C5E0CDCA-7B6E-41B2-9FC4-D93975CC467B");
        public static readonly Guid CLSID_VirtualDesktopManager = new Guid("AA509086-5CA9-4C25-8F95-589D3C07B48A");
        public static readonly Guid CLSID_VirtualDesktopPinnedApps = new Guid("B5A399E7-1C87-46B8-88E9-FC5747B171BD");
        public static readonly Guid CLSID_VirtualDesktopNotificationService = new Guid("a501fdec-4a09-464c-ae4e-1b9c21b84918");
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Size
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    internal enum APPLICATION_VIEW_CLOAK_TYPE : int
    {
        AVCT_NONE = 0,
        AVCT_DEFAULT = 1,
        AVCT_VIRTUAL_DESKTOP = 2
    }

    internal enum APPLICATION_VIEW_COMPATIBILITY_POLICY : int
    {
        AVCP_NONE = 0,
        AVCP_SMALL_SCREEN = 1,
        AVCP_TABLET_SMALL_SCREEN = 2,
        AVCP_VERY_SMALL_SCREEN = 3,
        AVCP_HIGH_SCALE_FACTOR = 4
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    [Guid("372E1D3B-38D3-42E4-A15B-8AB2B178F513")]
    internal interface IApplicationView
    {
        int SetFocus();
        int SwitchTo();
        int TryInvokeBack(IntPtr /* IAsyncCallback* */ callback);
        int GetThumbnailWindow(out IntPtr hwnd);
        int GetMonitor(out IntPtr /* IImmersiveMonitor */ immersiveMonitor);
        int GetVisibility(out int visibility);
        int SetCloak(APPLICATION_VIEW_CLOAK_TYPE cloakType, int unknown);
        int GetPosition(ref Guid guid /* GUID for IApplicationViewPosition */, out IntPtr /* IApplicationViewPosition** */ position);
        int SetPosition(ref IntPtr /* IApplicationViewPosition* */ position);
        int InsertAfterWindow(IntPtr hwnd);
        int GetExtendedFramePosition(out Rect rect);
        int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string id);
        int SetAppUserModelId(string id);
        int IsEqualByAppUserModelId(string id, out int result);
        int GetViewState(out uint state);
        int SetViewState(uint state);
        int GetNeediness(out int neediness);
        int GetLastActivationTimestamp(out ulong timestamp);
        int SetLastActivationTimestamp(ulong timestamp);
        int GetVirtualDesktopId(out Guid guid);
        int SetVirtualDesktopId(ref Guid guid);
        int GetShowInSwitchers(out int flag);
        int SetShowInSwitchers(int flag);
        int GetScaleFactor(out int factor);
        int CanReceiveInput(out bool canReceiveInput);
        int GetCompatibilityPolicyType(out APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
        int SetCompatibilityPolicyType(APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
        int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out Size size1, out Size size2);
        int GetSizeConstraintsForDpi(uint uint1, out Size size1, out Size size2);
        int SetSizeConstraintsForDpi(ref uint uint1, ref Size size1, ref Size size2);
        int OnMinSizePreferencesUpdated(IntPtr hwnd);
        int ApplyOperation(IntPtr /* IApplicationViewOperation* */ operation);
        int IsTray(out bool isTray);
        int IsInHighZOrderBand(out bool isInHighZOrderBand);
        int IsSplashScreenPresented(out bool isSplashScreenPresented);
        int Flash();
        int GetRootSwitchableOwner(out IApplicationView rootSwitchableOwner);
        int EnumerateOwnershipTree(out IObjectArray ownershipTree);
        int GetEnterpriseId([MarshalAs(UnmanagedType.LPWStr)] out string enterpriseId);
        int IsMirrored(out bool isMirrored);
        int Unknown1(out int unknown);
        int Unknown2(out int unknown);
        int Unknown3(out int unknown);
        int Unknown4(out int unknown);
        int Unknown5(out int unknown);
        int Unknown6(int unknown);
        int Unknown7();
        int Unknown8(out int unknown);
        int Unknown9(int unknown);
        int Unknown10(int unknownX, int unknownY);
        int Unknown11(int unknown);
        int Unknown12(out Size size1);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("1841C6D7-4F9D-42C0-AF41-8747538F10E5")]
    internal interface IApplicationViewCollection
    {
        int GetViews(out IObjectArray array);
        int GetViewsByZOrder(out IObjectArray array);
        int GetViewsByAppUserModelId(string id, out IObjectArray array);
        int GetViewForHwnd(HWND hwnd, out IApplicationView view);
        int GetViewForApplication(object application, out IApplicationView view);
        int GetViewForAppUserModelId(string id, out IApplicationView view);
        int GetViewInFocus(out IntPtr view);
        int Unknown1(out IntPtr view);
        void RefreshCollection();
        int RegisterForApplicationViewChanges(object listener, out int cookie);
        int UnregisterForApplicationViewChanges(int cookie);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("FF72FFDD-BE7E-43FC-9C03-AD81681E88E4")]
    internal interface IVirtualDesktop
    {
        bool IsViewVisible(IApplicationView view);
        Guid GetId();
    }

    /*
	IVirtualDesktop2 not used now (available since Win 10 2004), instead reading names out of registry for compatibility reasons
	Excample code:
	IVirtualDesktop2 ivd2;
	string desktopName;
	ivd2.GetName(out desktopName);
	Console.WriteLine("Name of desktop: " + desktopName);

		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("31EBDE3F-6EC3-4CBD-B9FB-0EF6D09B41F4")]
		internal interface IVirtualDesktop2
		{
			bool IsViewVisible(IApplicationView view);
			Guid GetId();
			void GetName([MarshalAs(UnmanagedType.HString)] out string name);
		}
	*/

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("F31574D6-B682-4CDC-BD56-1827860ABEC6")]
    internal interface IVirtualDesktopManagerInternal
    {
        int GetCount();
        void MoveViewToDesktop(IApplicationView view, IVirtualDesktop desktop);
        bool CanViewMoveDesktops(IApplicationView view);
        IVirtualDesktop GetCurrentDesktop();
        void GetDesktops(out IObjectArray desktops);
        [PreserveSig]
        int GetAdjacentDesktop(IVirtualDesktop from, int direction, out IVirtualDesktop desktop);
        void SwitchDesktop(IVirtualDesktop desktop);
        IVirtualDesktop CreateDesktop();
        void RemoveDesktop(IVirtualDesktop desktop, IVirtualDesktop fallback);
        IVirtualDesktop FindDesktop(ref Guid desktopid);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0F3A72B0-4566-487E-9A33-4ED302F6D6CE")]
    internal interface IVirtualDesktopManagerInternal2
    {
        int GetCount();
        void MoveViewToDesktop(IApplicationView view, IVirtualDesktop desktop);
        bool CanViewMoveDesktops(IApplicationView view);
        IVirtualDesktop GetCurrentDesktop();
        void GetDesktops(out IObjectArray desktops);
        [PreserveSig]
        int GetAdjacentDesktop(IVirtualDesktop from, int direction, out IVirtualDesktop desktop);
        void SwitchDesktop(IVirtualDesktop desktop);
        IVirtualDesktop CreateDesktop();
        void RemoveDesktop(IVirtualDesktop desktop, IVirtualDesktop fallback);
        IVirtualDesktop FindDesktop(ref Guid desktopid);
        void Unknown1(IVirtualDesktop desktop, out IntPtr unknown1, out IntPtr unknown2);
        void SetName(IVirtualDesktop desktop, [MarshalAs(UnmanagedType.HString)] string name);
    }

    //[ComImport]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[Guid("A5CD92FF-29BE-454C-8D04-D82879FB3F1B")]
    //internal interface IVirtualDesktopManager
    //{
    //	bool IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow);
    //	Guid GetWindowDesktopId(IntPtr topLevelWindow);
    //	void MoveWindowToDesktop(IntPtr topLevelWindow, ref Guid desktopId);
    //}

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("4CE81583-1E4C-4632-A621-07A53543148F")]
    internal interface IVirtualDesktopPinnedApps
    {
        bool IsAppIdPinned(string appId);
        void PinAppID(string appId);
        void UnpinAppID(string appId);
        bool IsViewPinned(IApplicationView applicationView);
        void PinView(IApplicationView applicationView);
        void UnpinView(IApplicationView applicationView);
    }

    [ComImport]
    [Guid("c179334c-4295-40d3-bea1-c654d965605a")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopNotification
    {
        void VirtualDesktopCreated(IVirtualDesktop pDesktop);
        void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);
        void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);
        void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);
        void ViewVirtualDesktopChanged(IntPtr pView);
        void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);
    }

    [ComImport]
    [Guid("0cd45e71-d927-4f15-8b0a-8fef525337bf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopNotificationService
    {
        uint Register(IVirtualDesktopNotification pNotification);
        void Unregister(uint dwCookie);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9")]
    internal interface IObjectArray
    {
        void GetCount(out int count);
        void GetAt(int index, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object obj);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    internal interface IServiceProvider10
    {
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object QueryService(ref Guid service, ref Guid riid);
    }
    #endregion

    #region COM wrapper
    internal static class DesktopManager
    {
        static DesktopManager()
        {
            var shell = (IServiceProvider10)Activator.CreateInstance(Type.GetTypeFromCLSID(Guids.CLSID_ImmersiveShell));
            VirtualDesktopManagerInternal = (IVirtualDesktopManagerInternal)shell.QueryService(Guids.CLSID_VirtualDesktopManagerInternal, typeof(IVirtualDesktopManagerInternal).GUID);
            try
            {
                VirtualDesktopManagerInternal2 = (IVirtualDesktopManagerInternal2)shell.QueryService(Guids.CLSID_VirtualDesktopManagerInternal, typeof(IVirtualDesktopManagerInternal2).GUID);
            }
            catch
            {
                VirtualDesktopManagerInternal2 = null;
            }
            VirtualDesktopManager = (Shell32.IVirtualDesktopManager)Activator.CreateInstance(Type.GetTypeFromCLSID(Guids.CLSID_VirtualDesktopManager));
            ApplicationViewCollection = (IApplicationViewCollection)shell.QueryService(typeof(IApplicationViewCollection).GUID, typeof(IApplicationViewCollection).GUID);
            VirtualDesktopPinnedApps = (IVirtualDesktopPinnedApps)shell.QueryService(Guids.CLSID_VirtualDesktopPinnedApps, typeof(IVirtualDesktopPinnedApps).GUID);
            VirtualDesktopNotificationService = (IVirtualDesktopNotificationService)shell.QueryService(Guids.CLSID_VirtualDesktopNotificationService, typeof(IVirtualDesktopNotificationService).GUID);
        }

        internal static IVirtualDesktopManagerInternal VirtualDesktopManagerInternal;
        internal static IVirtualDesktopManagerInternal2 VirtualDesktopManagerInternal2;
        internal static Shell32.IVirtualDesktopManager VirtualDesktopManager;
        internal static IApplicationViewCollection ApplicationViewCollection;
        internal static IVirtualDesktopPinnedApps VirtualDesktopPinnedApps;
        internal static IVirtualDesktopNotificationService VirtualDesktopNotificationService;

        internal static IVirtualDesktop GetDesktop(int index)
        {   // get desktop with index
            int count = VirtualDesktopManagerInternal.GetCount();
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
            IObjectArray desktops;
            VirtualDesktopManagerInternal.GetDesktops(out desktops);
            object objdesktop;
            desktops.GetAt(index, typeof(IVirtualDesktop).GUID, out objdesktop);
            Marshal.ReleaseComObject(desktops);
            return (IVirtualDesktop)objdesktop;
        }

        internal static int GetDesktopIndex(IVirtualDesktop desktop)
        { // get index of desktop
            int index = -1;
            Guid IdSearch = desktop.GetId();
            IObjectArray desktops;
            VirtualDesktopManagerInternal.GetDesktops(out desktops);
            object objdesktop;
            for (int i = 0; i < VirtualDesktopManagerInternal.GetCount(); i++)
            {
                desktops.GetAt(i, typeof(IVirtualDesktop).GUID, out objdesktop);
                if (IdSearch.CompareTo(((IVirtualDesktop)objdesktop).GetId()) == 0)
                {
                    index = i;
                    break;
                }
            }
            Marshal.ReleaseComObject(desktops);
            return index;
        }

        internal static IApplicationView GetApplicationView(this HWND hWnd)
        { // get application view to window handle
            IApplicationView view;
            ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
            return view;
        }

        internal static string GetAppId(HWND hWnd)
        { // get Application ID to window handle
            string appId;
            hWnd.GetApplicationView().GetAppUserModelId(out appId);
            return appId;
        }
    }
    #endregion

    #region public interface
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
            get { return DesktopManager.VirtualDesktopManagerInternal.GetCount(); }
        }

        public static VirtualDesktop Current
        { // returns current desktop
            get { return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop()); }
        }

        public static VirtualDesktop FromIndex(int index)
        { // return desktop object from index (-> index = 0..Count-1)
            return new VirtualDesktop(DesktopManager.GetDesktop(index));
        }

        public static VirtualDesktop FromHwnd(HWND hWnd)
        { // return desktop object to desktop on which window <hWnd> is displayed
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            Guid id = DesktopManager.VirtualDesktopManager.GetWindowDesktopId(hWnd);
            return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.FindDesktop(ref id));
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

            for (int i = 0; i < DesktopManager.VirtualDesktopManagerInternal.GetCount(); i++)
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
            return new VirtualDesktop(DesktopManager.VirtualDesktopManagerInternal.CreateDesktop());
        }

        public void Remove(VirtualDesktop fallback = null)
        { // destroy desktop and switch to <fallback>
            IVirtualDesktop fallbackdesktop;
            if (fallback == null)
            { // if no fallback is given use desktop to the left except for desktop 0.
                VirtualDesktop dtToCheck = new VirtualDesktop(DesktopManager.GetDesktop(0));
                if (this.Equals(dtToCheck))
                { // desktop 0: set fallback to second desktop (= "right" desktop)
                    DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 4, out fallbackdesktop); // 4 = RightDirection
                }
                else
                { // set fallback to "left" desktop
                    DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 3, out fallbackdesktop); // 3 = LeftDirection
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
            get { return object.ReferenceEquals(ivd, DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop()); }
        }

        public void MakeVisible()
        { // make this desktop visible
            DesktopManager.VirtualDesktopManagerInternal.SwitchDesktop(ivd);
        }

        public VirtualDesktop Left
        { // return desktop at the left of this one, null if none
            get
            {
                IVirtualDesktop desktop;
                int hr = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 3, out desktop); // 3 = LeftDirection
                if (hr == 0)
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
                int hr = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop(ivd, 4, out desktop); // 4 = RightDirection
                if (hr == 0)
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
    #endregion

    public class Disposable
    {
        public static IDisposable Create(Action dispose)
        {
            return new AnonymousDisposable(dispose);
        }

        private class AnonymousDisposable : IDisposable
        {
            private bool _isDisposed;
            private readonly Action _dispose;

            public AnonymousDisposable(Action dispose)
            {
                this._dispose = dispose;
            }

            public void Dispose()
            {
                if (this._isDisposed) return;

                this._isDisposed = true;
                this._dispose();
            }
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
        }
    }

    public class VirtualDesktopDestroyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the virtual desktop that was destroyed.
        /// </summary>
        public VirtualDesktop Destroyed { get; }

        /// <summary>
        /// Gets the virtual desktop to be displayed after <see cref="Destroyed" /> is destroyed.
        /// </summary>
        public VirtualDesktop Fallback { get; }

        public VirtualDesktopDestroyEventArgs(VirtualDesktop destroyed, VirtualDesktop fallback)
        {
            this.Destroyed = destroyed;
            this.Fallback = fallback;
        }
    }

    public class VirtualDesktopChangedEventArgs : EventArgs
    {
        public VirtualDesktop OldDesktop { get; }
        public VirtualDesktop NewDesktop { get; }

        public VirtualDesktopChangedEventArgs(VirtualDesktop oldDesktop, VirtualDesktop newDesktop)
        {
            this.OldDesktop = oldDesktop;
            this.NewDesktop = newDesktop;
        }
    }
}

