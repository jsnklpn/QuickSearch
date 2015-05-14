using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using QuickSearch.Models;

namespace QuickSearch.Helpers
{
    class ConfigHelper
    {
        private const string ApplicationName = "QuickSearch";
        private const string StartupRegKeyLocation = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static bool StartAppWithWindows
        {
            get
            {
                using (var startupRegistryKey = Registry.CurrentUser.OpenSubKey(StartupRegKeyLocation, false))
                {
                    return startupRegistryKey != null && startupRegistryKey.GetValue(ApplicationName) != null;
                }
            }
            set
            {
                using (var startupRegistryKey = Registry.CurrentUser.OpenSubKey(StartupRegKeyLocation, true))
                {
                    if (startupRegistryKey == null) return;
                    if (value)
                    {
                        // Add the value in the registry so that the application runs at startup
                        string applicationPath = string.Format("\"{0}\" -startminimized", Application.ExecutablePath.ToString());
                        startupRegistryKey.SetValue(ApplicationName, applicationPath);
                    }
                    else
                    {
                        // Remove the value from the registry so that the application doesn't start
                        startupRegistryKey.DeleteValue(ApplicationName, false);
                    }
                }
            }
        }

        public static List<string> SearchDirectories
        {
            get
            {
                var searchDirs = Properties.Settings.Default.SearchDirectories;

                if (searchDirs != null && searchDirs.Count > 0)
                {
                    foreach (var dir in searchDirs)
                    {
                        if (!Directory.Exists(dir))
                        {
                            // default to desktop
                            return new List<string>() { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) };
                        }
                    }

                    var list = searchDirs.Cast<string>().ToList();
                    return list;
                }
                else
                {
                    // default to desktop
                    return new List<string>() { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) };
                }
            }
            set 
            {
                var collection = new System.Collections.Specialized.StringCollection();
                collection.AddRange(value.ToArray());
                Properties.Settings.Default.SearchDirectories = collection;
            }

        }

        public static string SearchFilter
        {
            get
            {
                string searchFilter = Properties.Settings.Default.SearchFilter;

                if (!string.IsNullOrWhiteSpace(searchFilter))
                {
                    return searchFilter;
                }
                else
                {
                    // default to all files
                    return "*.*";
                }
            }
            set { Properties.Settings.Default.SearchFilter = value; }
        }

        public static bool AllowFolderSearch
        {
            get
            {
                bool allowFolderSearch = Properties.Settings.Default.AllowFolderSearch;
                return allowFolderSearch;
            }
            set { Properties.Settings.Default.AllowFolderSearch = value; }
        }

        public static ShortcutKeyCombo Hotkey
        {
            get
            {
                string hotkey = Properties.Settings.Default.Hotkey;

                if (!string.IsNullOrWhiteSpace(hotkey))
                {
                    return ShortcutKeyCombo.FromString(hotkey);
                }
                else
                {
                    // default WindowsKey + F12
                    return ShortcutKeyCombo.FromString("Win + F12");
                }
            }
            set { Properties.Settings.Default.Hotkey = value.ToString(); }
        }

        public static bool RunInSystemTray
        {
            get
            {
                bool sysTray = Properties.Settings.Default.RunInSystemTray;
                return sysTray;
            }
            set { Properties.Settings.Default.RunInSystemTray = value; }
        }

        public static bool ShowSmallIcons
        {
            get
            {
                bool smallIcons = Properties.Settings.Default.ShowSmallIcons;
                return smallIcons;
            }
            set { Properties.Settings.Default.ShowSmallIcons = value; }
        }

        public static int MaxResults
        {
            get 
            {
                int maxResults = Properties.Settings.Default.MaxResults;
                return maxResults;
            }
            set { Properties.Settings.Default.MaxResults = value; }
        }

        public static bool ShowIndexErrors
        {
            get
            {
                bool showIndexErrors = Properties.Settings.Default.ShowIndexErrors;
                return showIndexErrors;
            }
            set { Properties.Settings.Default.ShowIndexErrors = value; }
        }

        public static bool ShowFilePreview
        {
            get
            {
                bool setting = Properties.Settings.Default.ShowFilePreview;
                return setting;
            }
            set { Properties.Settings.Default.ShowFilePreview = value; }
        }

        public static void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}
