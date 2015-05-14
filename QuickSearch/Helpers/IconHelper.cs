using QuickSearch.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickSearch.Helpers
{
    public static class IconHelper
    {
        private static Dictionary<string, Icon> _icons;
        private static Dictionary<string, Icon> _smallIcons;
        private static Icon _smallFolderIcon;
        private static Icon _largeFolderIcon;

        static IconHelper()
        {
            _icons = new Dictionary<string, Icon>();
            _smallIcons = new Dictionary<string, Icon>();

            _smallFolderIcon = GetFolderIcon(false);
            _largeFolderIcon = GetFolderIcon(true);
        }

        public static Icon SmallFolderIcon
        {
            get { return _smallFolderIcon; }
        }

        public static Icon LargeFolderIcon
        {
            get { return _largeFolderIcon; }
        }

        public static Icon GetIconFromExtension(string extension, bool large)
        {
            Dictionary<string, Icon> icons = large ? _icons : _smallIcons;

            if (icons.ContainsKey(extension))
            {
                return icons[extension];
            }

            var icon = GetFileIcon(extension, large, false);
            icons[extension] = icon;
            return icon;
        }

        private static System.Drawing.Icon GetFileIcon(string name, bool large, bool linkOverlay)
        {
            Native.Shell32.SHFILEINFO shfi = new Native.Shell32.SHFILEINFO();
            uint flags = Native.Shell32.SHGFI_ICON | Native.Shell32.SHGFI_USEFILEATTRIBUTES;

            if (true == linkOverlay) flags += Native.Shell32.SHGFI_LINKOVERLAY;

            /* Check the size specified for return. */
            if (large)
            {
                flags += Native.Shell32.SHGFI_LARGEICON;  // include the large icon flag
            }
            else
            {
                flags += Native.Shell32.SHGFI_SMALLICON; // include the small icon flag
            }

            Native.Shell32.SHGetFileInfo(name,
                Native.Shell32.FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);


            // Copy (clone) the returned icon to a new object, thus allowing us 
            // to call DestroyIcon immediately
            System.Drawing.Icon icon = (System.Drawing.Icon)
                                System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            Native.User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }

        private static System.Drawing.Icon GetFolderIcon(bool large)
        {
            Native.Shell32.SHFILEINFO shfi = new Native.Shell32.SHFILEINFO();
            uint flags = Native.Shell32.SHGFI_ICON | Native.Shell32.SHGFI_USEFILEATTRIBUTES;

            /* Check the size specified for return. */
            if (large)
            {
                flags += Native.Shell32.SHGFI_LARGEICON;  // include the large icon flag
            }
            else
            {
                flags += Native.Shell32.SHGFI_SMALLICON; // include the small icon flag
            }

            string myDir = Path.GetDirectoryName(Application.ExecutablePath.ToString());
            Native.Shell32.SHGetFileInfo(myDir,
                Native.Shell32.FILE_ATTRIBUTE_DIRECTORY,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);


            // Copy (clone) the returned icon to a new object, thus allowing us 
            // to call DestroyIcon immediately
            System.Drawing.Icon icon = (System.Drawing.Icon)
                                System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            Native.User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }

        public static void Dispose()
        {
            _smallFolderIcon.Dispose();
            _largeFolderIcon.Dispose();

            if (_icons != null)
            {
                foreach (var icon in _icons)
                {
                    icon.Value.Dispose();
                }
                _icons = null;
            }
            if (_smallIcons != null)
            {
                foreach (var icon in _smallIcons)
                {
                    icon.Value.Dispose();
                }
                _smallIcons = null;
            }
        }
    }
}
