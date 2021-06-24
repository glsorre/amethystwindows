using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
namespace WindowsDesktop.Internal
{
    internal static class Guids
    {
        public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
        // AA509086-5CA9-4C25-8F95-589D3C07B48A,
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
    [Guid("536d3495-b208-4cc9-ae26-de8111275bf8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IVirtualDesktop
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsViewVisible(IApplicationView pView);

        Guid GetId();

        void GetName([MarshalAs(UnmanagedType.HString)] out string name);

        int Unknown1();

    }
    /*
	IVirtualDesktop2 not used now (available since Win 10 2004), instead reading names out of registry for compatibility reasons
	Excample code:
	IVirtualDesktop2 ivd2;
	string desktopName;
	ivd2.GetName(out desktopName);

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
    [Guid("B2F925B9-5A0F-4D2E-9F4D-2B1507593C10")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopManagerInternal
    {
        //  HRESULT Proc3(/* Stack Offset: 8 */ [In] FC_USER_MARSHAL* p0, /* Stack Offset: 16 */ [Out] int* p1);
        int GetCount(IntPtr hWndOrMon);
        //  HRESULT Proc4(/* Stack Offset: 8 */ [In] IApplicationView* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1);
        void MoveViewToDesktop(IApplicationView pView, IVirtualDesktop desktop);

        //  HRESULT Proc5(/* Stack Offset: 8 */ [In] IApplicationView* p0, /* Stack Offset: 16 */ [Out] int* p1);
        [return: MarshalAs(UnmanagedType.Bool)]
        bool CanViewMoveDesktops(IApplicationView pView);
        //  HRESULT Proc6(/* Stack Offset: 8 */ [In] FC_USER_MARSHAL* p0, /* Stack Offset: 16 */ [Out] IVirtualDesktop** p1);
        IVirtualDesktop GetCurrentDesktop(string s);
        // HRESULT Proc7(/* Stack Offset: 8 */ [In] FC_USER_MARSHAL* p0, /* Stack Offset: 16 */ [Out] IObjectArray** p1);
        IObjectArray GetDesktops(IntPtr s);
        // HRESULT Proc8(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] int p1, /* Stack Offset: 24 */ [Out] IVirtualDesktop** p2);
        IVirtualDesktop GetAdjacentDesktop(IVirtualDesktop pDesktopReference, int uDirection);

        // HRESULT Proc9(/* Stack Offset: 8 */ [In] FC_USER_MARSHAL* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1);
        void SwitchDesktop(string s, IVirtualDesktop desktop);

        // HRESULT Proc10(/* Stack Offset: 8 */ [In] FC_USER_MARSHAL* p0, /* Stack Offset: 16 */ [Out] IVirtualDesktop** p1);
        IVirtualDesktop CreateDesktopW(string name);

        // HRESULT Proc11(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] FC_USER_MARSHAL* p1, /* Stack Offset: 24 */ [In] int p2);
        void Unknown11(IVirtualDesktop data, string s, int i);

       //  HRESULT Proc12(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1);
        void RemoveDesktop(IVirtualDesktop pRemove, IVirtualDesktop pFallbackDesktop);

        // HRESULT Proc13(/* Stack Offset: 8 */ [In] GUID* p0, /* Stack Offset: 16 */ [Out] IVirtualDesktop** p1);
        IVirtualDesktop FindDesktop([In, MarshalAs(UnmanagedType.LPStruct)] Guid desktopId);

        // HRESULT Proc14(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [Out] IObjectArray** p1, /* Stack Offset: 24 */ [Out] IObjectArray** p2);
        void UnknownProc14(IVirtualDesktop desktop, out IObjectArray out1, out IObjectArray out2);

        // HRESULT Proc15(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] FC_USER_MARSHAL* p1);
        void SetName(IVirtualDesktop desktop, string name);

        //         HRESULT Proc16(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] FC_USER_MARSHAL* p1);
        void UnknownProc16(IVirtualDesktop p0, string s);

        //         HRESULT Proc17(/* Stack Offset: 8 */ [In] FC_USER_MARSHAL* p0);
        void UnknownProc17(string s);

        // HRESULT Proc18(/* Stack Offset: 8 */ [In] IApplicationView* p0, /* Stack Offset: 16 */ [In] IApplicationView* p1);
        void UnknownProc18(IApplicationView pView1, IApplicationView pView2);
        // HRESULT Proc19(/* Stack Offset: 8 */ [Out] int* p0);
        // int GetCount();
        //HRESULT Proc20(/* Stack Offset: 8 */ [In] int p0);




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

    //[Guid("cd403e52-deed-4c13-b437-b98380f2b1e8")]
    //interface IVirtualDesktopNotification : IUnknown
    //{
    //    HRESULT Proc3(/* Stack Offset: 8 */ [In] IObjectArray* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1);
    //    HRESULT Proc4(/* Stack Offset: 8 */ [In] IObjectArray* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1, /* Stack Offset: 24 */ [In] IVirtualDesktop* p2);
    //    HRESULT Proc5(/* Stack Offset: 8 */ [In] IObjectArray* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1, /* Stack Offset: 24 */ [In] IVirtualDesktop* p2);
    //    HRESULT Proc6(/* Stack Offset: 8 */ [In] IObjectArray* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1, /* Stack Offset: 24 */ [In] IVirtualDesktop* p2);
    //    HRESULT Proc7(/* Stack Offset: 8 */ [In] int p0);
    //    HRESULT Proc8(/* Stack Offset: 8 */ [In] IObjectArray* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1, /* Stack Offset: 24 */ [In] int p2, /* Stack Offset: 32 */ [In] int p3);
    //    HRESULT Proc9(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] FC_USER_MARSHAL* p1);
    //    HRESULT Proc10(/* Stack Offset: 8 */ [In] IApplicationView* p0);
    //    HRESULT Proc11(/* Stack Offset: 8 */ [In] IObjectArray* p0, /* Stack Offset: 16 */ [In] IVirtualDesktop* p1, /* Stack Offset: 24 */ [In] IVirtualDesktop* p2);
    //    HRESULT Proc12(/* Stack Offset: 8 */ [In] IVirtualDesktop* p0, /* Stack Offset: 16 */ [In] FC_USER_MARSHAL* p1);
    //}

    [ComImport]
    [Guid("cd403e52-deed-4c13-b437-b98380f2b1e8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVirtualDesktopNotification
    {
        void VirtualDesktopCreated( IVirtualDesktop P1);
        //void VirtualDesktopCreated(IVirtualDesktop pDesktop);

        void VirtualDesktopDestroyBegin(IVirtualDesktop p1, IVirtualDesktop p2);
        //void VirtualDesktopDestroyBegin(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

        void VirtualDesktopDestroyFailed(IVirtualDesktop p1, IVirtualDesktop p2);
        //void VirtualDesktopDestroyFailed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

        void VirtualDesktopDestroyed(IVirtualDesktop p1, IVirtualDesktop p2);
        //void VirtualDesktopDestroyed(IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback);

        void ViewVirtualDesktopChanged(IntPtr p0);
        //void ViewVirtualDesktopChanged(IntPtr pView);

        void UnknownProc8(IObjectArray p0, IVirtualDesktop p1, int p2, int p3);
        void UnknownProc9(IVirtualDesktop p0, string p1);
        void UnknownProc10(IApplicationView p0);
        void CurrentVirtualDesktopChanged(IVirtualDesktop p1, IVirtualDesktop p2);
        //void CurrentVirtualDesktopChanged(IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew);

        void UnknownProc12(IVirtualDesktop p0, string p1);

    }

    //[Guid("0cd45e71-d927-4f15-8b0a-8fef525337bf")]
    //interface IVirtualDesktopNotificationService : IUnknown
    //{
    //    HRESULT Proc3(/* Stack Offset: 8 */ [In] IVirtualDesktopNotification* p0, /* Stack Offset: 16 */ [Out] int* p1);
    //    HRESULT Proc4(/* Stack Offset: 8 */ [In] int p0);
    //}
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
}
