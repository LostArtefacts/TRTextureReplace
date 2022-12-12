using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TRTextureReplace.Utils;

public static class WindowUtils
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWind, bool bRevert);

    [DllImport("user32.dll")]
    private static extern IntPtr EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    [DllImport("user32.dll")]
    private static extern IntPtr RemoveMenu(IntPtr hWind, uint uPostition, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    public const uint WM_SYSCOMMAND = 0x112;
    public const uint MF_BYCOMMAND = 0x0;
    public const uint MF_BYPOSITION = 0x400;
    public const uint MF_REMOVE = 0x1000;
    public const uint MF_ENABLED = 0x0;
    public const uint MF_DISABLED = 0x2;
    public const uint MF_SEPARATOR = 0x800;
    public const uint SC_CLOSE = 0xF060;
    public const int GWL_STYLE = -16;
    public const int WS_MINIMIZE = 0x20000;

    public static IntPtr GetWindowHandle(Window window)
    {
        return new WindowInteropHelper(window).Handle;
    }

    public static void EnableCloseButton(Window w, bool enabled)
    {
        IntPtr h = GetSystemMenu(GetWindowHandle(w), false);
        uint cmd = MF_BYCOMMAND | (enabled ? MF_ENABLED : MF_DISABLED);
        EnableMenuItem(h, SC_CLOSE, cmd);

        if (enabled)
        {
            w.Closing -= W_Closing;
        }
        else
        {
            w.Closing += W_Closing;
        }
    }

    private static void W_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
    }

    public static void TidyMenu(Window w)
    {
        IntPtr h = GetSystemMenu(GetWindowHandle(w), false);
        if (w.ResizeMode == ResizeMode.NoResize)
        {
            RemoveMenu(h, 5, MF_BYPOSITION | MF_REMOVE); //Separator above Close
            RemoveMenu(h, 4, MF_BYPOSITION | MF_REMOVE); //Maximise
            RemoveMenu(h, 3, MF_BYPOSITION | MF_REMOVE); //Minimise
            RemoveMenu(h, 2, MF_BYPOSITION | MF_REMOVE); //Size
            RemoveMenu(h, 0, MF_BYPOSITION | MF_REMOVE); //Restore
        }
        else if (w.ResizeMode == ResizeMode.CanMinimize)
        {
            RemoveMenu(h, 4, MF_BYPOSITION | MF_REMOVE); //Maximise
            RemoveMenu(h, 2, MF_BYPOSITION | MF_REMOVE); //Size
            RemoveMenu(h, 0, MF_BYPOSITION | MF_REMOVE); //Restore
        }
        else
        {
            long value = GetWindowLong(h, GWL_STYLE);
            if (((int)value & ~WS_MINIMIZE) == 0)
            {
                RemoveMenu(h, 3, MF_BYPOSITION | MF_REMOVE); //Minimise
            }
        }
    }
}
