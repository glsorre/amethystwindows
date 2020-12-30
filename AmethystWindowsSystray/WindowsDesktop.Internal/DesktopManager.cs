using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsDesktop.Internal;

namespace WindowsDesktop.Internal
{
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
}
