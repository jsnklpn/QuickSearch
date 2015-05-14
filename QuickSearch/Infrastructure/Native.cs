using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QuickSearch.Infrastructure
{
    public class Native
    {
        public static class Shell32
        {
            public const uint SHGFI_ICON = 0x000000100;
            public const uint SHGFI_DISPLAYNAME = 0x000000200;
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_ATTRIBUTES = 0x000000800;
            public const uint SHGFI_ICONLOCATION = 0x000001000;
            public const uint SHGFI_EXETYPE = 0x000002000;
            public const uint SHGFI_SYSICONINDEX = 0x000004000;
            public const uint SHGFI_LINKOVERLAY = 0x000008000;
            public const uint SHGFI_SELECTED = 0x000010000;
            public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;
            public const uint SHGFI_LARGEICON = 0x000000000;
            public const uint SHGFI_SMALLICON = 0x000000001;
            public const uint SHGFI_OPENICON = 0x000000002;
            public const uint SHGFI_SHELLICONSIZE = 0x000000004;
            public const uint SHGFI_PIDL = 0x000000008;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            public const uint SHGFI_ADDOVERLAYS = 0x000000020;
            public const uint SHGFI_OVERLAYINDEX = 0x000000040;

            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

            public const int MAX_PATH = 260;
            public const int NAMESIZE = 80;

            [DllImport("Shell32.dll")]
            public static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbFileInfo,
                uint uFlags
            );

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
                public string szTypeName;
            }
        }

        public static class User32
        {
            public const uint ECM_FIRST = 0x1500;
            public const uint EM_SETCUEBANNER = ECM_FIRST + 1;
            public const int WM_MOUSEWHEEL = 0x020A;
            public const int WM_MOUSEHWHEEL = 0x020E;
            public const int MOD_ALT = 0x1;
            public const int MOD_CONTROL = 0x2;
            public const int MOD_SHIFT = 0x4;
            public const int MOD_WIN = 0x8;
            public const int WM_HOTKEY = 0x312;
            public const int WM_VSCROLL = 0x115;

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, string lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr WindowFromPoint(POINT Point);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool DestroyIcon(IntPtr hIcon);

            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool SetWindowText(HandleRef hWnd, string text);

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }

                public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

                public static implicit operator System.Drawing.Point(POINT p)
                {
                    return new System.Drawing.Point(p.X, p.Y);
                }

                public static implicit operator POINT(System.Drawing.Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }

            #region Helper methods

            //public static void RegisterHotKey(System.Windows.Forms.Form f, System.Windows.Forms.Keys key)
            //{
            //    int modifiers = 0;

            //    if ((key & System.Windows.Forms.Keys.Alt) == System.Windows.Forms.Keys.Alt)
            //        modifiers = modifiers | MOD_ALT;

            //    if ((key & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control)
            //        modifiers = modifiers | MOD_CONTROL;

            //    if ((key & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
            //        modifiers = modifiers | MOD_SHIFT;

            //    System.Windows.Forms.Keys k = key & ~System.Windows.Forms.Keys.Control & ~System.Windows.Forms.Keys.Shift & ~System.Windows.Forms.Keys.Alt;
            //    keyId = f.GetHashCode(); // this should be a key unique ID, modify this if you want more than one hotkey
            //    RegisterHotKey((IntPtr)f.Handle, keyId, (int)modifiers, (int)k);
            //}

            //public static void UnregisterHotKey(System.Windows.Forms.Form f)
            //{
            //    UnregisterHotKey(f.Handle, keyId); // modify this if you want more than one hotkey
            //}

            #endregion
        }
    }
}
