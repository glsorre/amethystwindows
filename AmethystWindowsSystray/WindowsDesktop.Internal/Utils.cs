using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDesktop.Internal
{
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
