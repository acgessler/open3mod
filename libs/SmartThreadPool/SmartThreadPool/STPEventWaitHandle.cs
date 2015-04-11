#if !(_WINDOWS_CE)

using System;
using System.Threading;

namespace Amib.Threading.Internal
{
#if _WINDOWS ||  WINDOWS_PHONE
    internal static class STPEventWaitHandle
    {
        public const int WaitTimeout = Timeout.Infinite;

        internal static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            // WaitAll() does not support waiting for multiple handles on STA threads.
            // http://stackoverflow.com/questions/4192834/
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                foreach (var e in waitHandles)
                {
                    e.WaitOne(millisecondsTimeout);
                }
                return true;
            }
            return WaitHandle.WaitAll(waitHandles, millisecondsTimeout);
        }

        internal static int WaitAny(WaitHandle[] waitHandles)
        {
            return WaitHandle.WaitAny(waitHandles);
        }

        internal static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return WaitHandle.WaitAny(waitHandles, millisecondsTimeout);
        }

        internal static bool WaitOne(WaitHandle waitHandle, int millisecondsTimeout, bool exitContext)
        {
            return waitHandle.WaitOne(millisecondsTimeout);
        }
    }
#else
    internal static class STPEventWaitHandle
    {
        public const int WaitTimeout = Timeout.Infinite;

        internal static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return WaitHandle.WaitAll(waitHandles, millisecondsTimeout, exitContext);
        }

        internal static int WaitAny(WaitHandle[] waitHandles)
        {
            return WaitHandle.WaitAny(waitHandles);
        }

        internal static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return WaitHandle.WaitAny(waitHandles, millisecondsTimeout, exitContext);
        }

        internal static bool WaitOne(WaitHandle waitHandle, int millisecondsTimeout, bool exitContext)
        {
            return waitHandle.WaitOne(millisecondsTimeout, exitContext);
        }
    }
#endif

}

#endif