using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NicoNicoDownloader
{
    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }

    class Win32Api
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }

    class PowerManagement
    {
        public static void PreventSleep()
        {
            // Prevent Idle-to-Sleep (monitor not affected) (see note above)
            bool result = Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS  | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED| EXECUTION_STATE.ES_DISPLAY_REQUIRED) != 0;
            if (!result)
                throw new Win32Exception();
        }

        public static void KeepSystemAwake()
        {
            bool result = Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED) != 0;
            if (!result)
                throw new Win32Exception();
        }

        public static void PreventMonitorPowerdown()
        {
           bool result = Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS) != 0;
            if (!result)
                throw new Win32Exception();
        }

        public static void AllowMonitorPowerdown()
        {
            bool result = Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS) != 0;
            if (!result)
                throw new Win32Exception();
        }
    }
}
