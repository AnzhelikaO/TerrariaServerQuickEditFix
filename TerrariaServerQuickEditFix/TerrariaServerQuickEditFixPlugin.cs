#region Using
using System;
using System.Runtime.InteropServices;
using Terraria;
using TerrariaApi.Server;
#endregion
namespace TerrariaServerQuickEditFix
{
    [ApiVersion(2, 1)]
    public class TerrariaServerQuickEditFixPlugin : TerrariaPlugin
    {
        #region Data

        public override string Name => "TerrariaServer.exe QuickEdit fix";
        public override string Author => "https://stackoverflow.com/a/36720802 implemented by Anzhelika";
        public override Version Version => new Version(1, 0, 0, 0);
        public override string Description => "Enables QuickEdit mode in TerrariaServer.exe application";
        public TerrariaServerQuickEditFixPlugin(Main Game) : base(Game) { }

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #endregion
        #region Initialize, Dispose

        public override void Initialize() =>
            SwitchQuickEditMode(true);

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
                SwitchQuickEditMode(false);
            base.Dispose(Disposing);
        }

        #endregion
        #region SwitchQuickEditMode

        public static void SwitchQuickEditMode(bool Enable)
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // Get current console mode
            if (!GetConsoleMode(consoleHandle, out uint consoleMode))
            {
                // ERROR: Unable to get console mode.
                return;
            }

            if (Enable)
            {
                // Enable the quick edit bit in the mode flags
                consoleMode |= ENABLE_QUICK_EDIT;
            }
            else
            {
                // Clear the quick edit bit in the mode flags
                consoleMode &= ~ENABLE_QUICK_EDIT;
            }

            // Set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return;
            }
        }

        #endregion
    }
}