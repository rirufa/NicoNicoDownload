using System;
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
            Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS  | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        public static void KeepSystemAwake()
        {
            Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        public static void PreventMonitorPowerdown()
        {
            Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void AllowMonitorPowerdown()
        {
            Win32Api.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }
    }
}
