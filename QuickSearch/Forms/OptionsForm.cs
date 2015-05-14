using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Remoting.Messaging;
using QuickSearch.Helpers;
using QuickSearch.Infrastructure;
using Ookii.Dialogs;

namespace QuickSearch.Forms
{
    public partial class OptionsForm : Form
    {
        #region filters

        static readonly string[] VideoFilters = new string[] 
        {
            "*.avi",
            "*.m4v",
            "*.mkv",
            "*.mov",
            "*.mp4",
            "*.mpeg",
            "*.mpg",
            "*.wmv",
            "*.qt"
        };

        static readonly string[] ImageFilters = new string[] 
        {
            "*.jpeg",
            "*.jpg",
            "*.bmp",
            "*.gif",
            "*.png",
            "*.tif",
            "*.tiff",
            "*.raw",
        };

        static readonly string[] AudioFilters = new string[] 
        {
            "*.mp3",
            "*.wma",
            "*.flac",
            "*.wav",
            "*.aiff",
            "*.m4a"
        };

        #endregion

        private Bitmap _folderIcon;
        private string _desktopLocation;
        private Bitmap _desktopLocationIcon;
        private string _myDocumentsLocation;
        private Bitmap _myDocumentsLocationIcon;
        private string _myPicturesLocation;
        private Bitmap _myPicturesLocationIcon;
        private string _myMusicLocation;
        private Bitmap _myMusicLocationIcon;
        private string _myVideosLocation;
        private Bitmap _myVideosLocationIcon;

        /// <summary>
        /// Returns true if an option was changed which requires an index rebuild.
        /// </summary>
        public bool RebuildIndexRequired { get; private set; }

        public OptionsForm()
        {
            InitializeComponent();

            RebuildIndexRequired = false;

            tbFilter.LostFocus += new EventHandler(tbFilter_LostFocus);
            tbFilter.KeyDown += new KeyEventHandler(tbFilter_KeyDown);

            _folderIcon = GetFolderIconBitmap(null);

            SetLocationAndIcon(Environment.SpecialFolder.DesktopDirectory, out _desktopLocation, out _desktopLocationIcon);
            SetLocationAndIcon(Environment.SpecialFolder.MyDocuments, out _myDocumentsLocation, out _myDocumentsLocationIcon);
            SetLocationAndIcon(Environment.SpecialFolder.MyMusic, out _myMusicLocation, out _myMusicLocationIcon);
            SetLocationAndIcon(Environment.SpecialFolder.MyPictures, out _myPicturesLocation, out _myPicturesLocationIcon);
            SetLocationAndIcon(Environment.SpecialFolder.MyVideos, out _myVideosLocation, out _myVideosLocationIcon);

            var addMenuStrip = new ContextMenuStrip();
            var seperator = new ToolStripSeparator();
            addMenuStrip.Items.Add("Select directory...", null, btnAddDir_Click);
            addMenuStrip.Items.Add(seperator);
            addMenuStrip.Items.Add("Desktop", _desktopLocationIcon, (s, e) => TryToAddDirectory(_desktopLocation));
            addMenuStrip.Items.Add("My Documents", _myDocumentsLocationIcon, (s, e) => TryToAddDirectory(_myDocumentsLocation));
            addMenuStrip.Items.Add("My Music", _myMusicLocationIcon, (s, e) => TryToAddDirectory(_myMusicLocation));
            addMenuStrip.Items.Add("My Pictures", _myPicturesLocationIcon, (s, e) => TryToAddDirectory(_myPicturesLocation));
            addMenuStrip.Items.Add("My Videos", _myVideosLocationIcon, (s, e) => TryToAddDirectory(_myVideosLocation));
            this.btnAddDir.Menu = addMenuStrip;

            this.Icon = Properties.Resources.icon2;
        }

        private static void SetLocationAndIcon(Environment.SpecialFolder folder, out string location, out Bitmap icon)
        {
            location = Environment.GetFolderPath(folder);
            icon = GetFolderIconBitmap(location);
        }

        protected override void  OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            _folderIcon.Dispose();
            _desktopLocationIcon.Dispose();
            _myDocumentsLocationIcon.Dispose();
            _myPicturesLocationIcon.Dispose();
            _myMusicLocationIcon.Dispose();
            _myVideosLocationIcon.Dispose();
        }

        void tbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Delete)
            //{
            //    tbFilter.Text = "*.*";
            //    e.Handled = true;
            //}
        }

        void tbFilter_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFilter.Text))
            {
                tbFilter.Text = "*.*";
            }
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            btnRemoveDir.Enabled = false;
            ConfigHelper.SearchDirectories.ForEach(d => lbDirectories.Items.Add(d));
            cbAllowFolderSearch.Checked = ConfigHelper.AllowFolderSearch;
            tbFilter.Text = ConfigHelper.SearchFilter;
            tbHotkey.ShortcutKeys = ConfigHelper.Hotkey;
            cbSysTray.Checked = ConfigHelper.RunInSystemTray;
            tbHotkey.Enabled = cbSysTray.Checked;
            cbSmallIcons.Checked = ConfigHelper.ShowSmallIcons;
            cbLaunchWithWindows.Checked = ConfigHelper.StartAppWithWindows;
            cbShowIndexErrors.Checked = ConfigHelper.ShowIndexErrors;
            cbShowFilePreview.Checked = ConfigHelper.ShowFilePreview;
            numMaxResults.Value = ConfigHelper.MaxResults;
        }

        private bool ValidateOptions()
        {
            if (lbDirectories.Items.Count == 0)
            {
                MessageBox.Show("You need to specify a search directory.");
                lbDirectories.Focus();
                return false;
            }

            foreach (var dir in lbDirectories.Items.Cast<string>())
            {
                if (dir != null && !Directory.Exists(dir))
                {
                    MessageBox.Show("The directory \"" + dir + "\" does not exist.");
                    lbDirectories.Focus();
                    return false;
                }
            }

            // Check for sub-directories
            foreach (var dir1 in lbDirectories.Items.Cast<string>())
            {
                foreach (var dir2 in lbDirectories.Items.Cast<string>())
                {
                    if (dir1 != dir2)
                    {
                        if (dir1.Contains(dir2) || dir2.Contains(dir1))
                        {
                            MessageBox.Show("Please make sure that your directory list doesn't contain subdirectory conflicts. Please correct these entries:\r\n\r\n" + dir1 + "\r\n" + dir2);
                            lbDirectories.Focus();
                            return false;
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(tbFilter.Text))
            {
                tbFilter.Text = "*.*";
            }

            foreach (var c in tbFilter.Text)
            {
                if (c == '*') continue;
                if (Path.GetInvalidFileNameChars().Contains(c))
                {
                    MessageBox.Show("You cannot use any of the following characters in the search filter:\n" +
                        string.Join(" ", Path.GetInvalidFileNameChars().Where(ch => ch != '*')));
                    tbFilter.Focus();
                    return false;
                }
            }

            if (tbHotkey.ShortcutKeys.Modifier == Infrastructure.ModifierKeys.None &&
                tbHotkey.ShortcutKeys.Key != Keys.None)
            {
                MessageBox.Show("The shortcut combination \"" + tbHotkey.ShortcutKeys.ToString() + "\" is not valid.");
                tbHotkey.Focus();
                return false;
            }

            if (tbHotkey.ShortcutKeys.Modifier != Infrastructure.ModifierKeys.None &&
                tbHotkey.ShortcutKeys.Key == Keys.None)
            {
                //if (cbSysTray.Checked)
                //{
                //    MessageBox.Show("The shortcut combination \"" + tbHotkey.ShortcutKeys.ToString() + "\" is not valid.");
                //    tbHotkey.Focus();
                //    return false;
                //}

                tbHotkey.ShortcutKeys = null; // Clear the shortcut key
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateOptions())
                return;

            bool searchDirectoriesHaveChanged = HasDirectoriesChanged();

            if (!searchDirectoriesHaveChanged &&
                ConfigHelper.AllowFolderSearch == cbAllowFolderSearch.Checked &&
                ConfigHelper.SearchFilter == tbFilter.Text &&
                ConfigHelper.Hotkey.ToString() == tbHotkey.ShortcutKeys.ToString() &&
                ConfigHelper.RunInSystemTray == cbSysTray.Checked &&
                ConfigHelper.ShowSmallIcons == cbSmallIcons.Checked &&
                ConfigHelper.StartAppWithWindows == cbLaunchWithWindows.Checked &&
                ConfigHelper.ShowFilePreview == cbShowFilePreview.Checked &&
                ConfigHelper.ShowIndexErrors == cbShowIndexErrors.Checked &&
                ConfigHelper.MaxResults == (int)numMaxResults.Value)
            {
                // No changes, so make this look like a cancel.
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                // Determine if the index needs to be rebuilt
                this.RebuildIndexRequired = (searchDirectoriesHaveChanged ||
                                        ConfigHelper.AllowFolderSearch != cbAllowFolderSearch.Checked ||
                                        ConfigHelper.SearchFilter != tbFilter.Text);

                // There were changes, so save the new settings
                ConfigHelper.SearchDirectories = lbDirectories.Items.Cast<string>().ToList();
                ConfigHelper.AllowFolderSearch = cbAllowFolderSearch.Checked;
                ConfigHelper.SearchFilter = tbFilter.Text;
                ConfigHelper.Hotkey = tbHotkey.ShortcutKeys;
                ConfigHelper.RunInSystemTray = cbSysTray.Checked;
                ConfigHelper.ShowSmallIcons = cbSmallIcons.Checked;
                ConfigHelper.StartAppWithWindows = cbLaunchWithWindows.Checked;
                ConfigHelper.ShowFilePreview = cbShowFilePreview.Checked;
                ConfigHelper.ShowIndexErrors = cbShowIndexErrors.Checked;
                ConfigHelper.MaxResults = (int)numMaxResults.Value;
                ConfigHelper.SaveSettings();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool HasDirectoriesChanged()
        {
            int itemsCount = ConfigHelper.SearchDirectories.Count;
            if (itemsCount != lbDirectories.Items.Count)
            {
                return true;
            }
           
            foreach (var dir in ConfigHelper.SearchDirectories)
            {
                foreach (var item in lbDirectories.Items.Cast<string>())
                {
                    if (string.Equals(dir, item, StringComparison.InvariantCultureIgnoreCase))
                    {
                        itemsCount--;
                    }
                }
            }

            return (itemsCount != 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbSysTray_CheckedChanged(object sender, EventArgs e)
        {
            tbHotkey.Enabled = cbSysTray.Checked;
        }

        private void btnRemoveDir_Click(object sender, EventArgs e)
        {
            if (lbDirectories.SelectedIndex >= 0)
            {
                lbDirectories.Items.RemoveAt(lbDirectories.SelectedIndex);
            }
        }

        private void btnAddDir_Click(object sender, EventArgs e)
        {
            using (var dlg = new VistaFolderBrowserDialog())
            {
                dlg.Description = "Note: Sub-folders are added automatically.";
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    TryToAddDirectory(dlg.SelectedPath);
                }
            }
        }

        private void TryToAddDirectory(string dir)
        {
            if (IsRootDir(dir))
            {
                MessageBox.Show(
                    string.Format("Sorry, the path \"{0}\" is a root directory and cannot be added.\r\nPlease select a different directory.",
                        dir), "Cannot Add Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<object> dirsToRemove = new List<object>();
            foreach (var existingDir in lbDirectories.Items)
            {
                string strExistingDir = existingDir as string;
                if (strExistingDir == null) continue;
                if (new DirectoryInfo(strExistingDir).FullName == new DirectoryInfo(dir).FullName)
                {
                    //MessageBox.Show("The selected path is already included in the search index.", "Directory Already Included", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (IsSubfolder(strExistingDir, dir))
                {
                    MessageBox.Show("The selected path is a sub-folder of \"" + strExistingDir + "\" and will already be included in the search.", "Directory Already Included", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (IsSubfolder(dir, strExistingDir))
                {
                    dirsToRemove.Add(existingDir);
                }
            }

            foreach (var dirToRemove in dirsToRemove)
                lbDirectories.Items.Remove(dirToRemove);

            lbDirectories.Items.Add(dir);
        }

        private static bool IsRootDir(string path)
        {
            return new DirectoryInfo(path).Parent == null;
        }

        private static bool IsSubfolder(string parentPath, string childPath)
        {
            var parentUri = new Uri(parentPath);

            var childUri = new DirectoryInfo(childPath).Parent;

            while (childUri != null)
            {
                if (new Uri(childUri.FullName) == parentUri)
                {
                    return true;
                }

                childUri = childUri.Parent;
            }

            return false;
        }

        private void lbDirectories_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemoveDir.Enabled = (lbDirectories.SelectedIndex != -1);
        }

        private void cbVideo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null) return;

            if (CheckTag(cb)) return;

            tbFilter.Text = MergeFilterStrings(tbFilter.Text, VideoFilters, cb.Checked);
        }

        private void cbImages_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null) return;

            if (CheckTag(cb)) return;

            tbFilter.Text = MergeFilterStrings(tbFilter.Text, ImageFilters, cb.Checked);
        }

        private void cbLaunchWithWindows_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbSmallIcons_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbAudio_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null) return;

            if (CheckTag(cb)) return;

            tbFilter.Text = MergeFilterStrings(tbFilter.Text, AudioFilters, cb.Checked);
        }

        private bool CheckTag(CheckBox cb)
        {
            if ((cb.Tag as bool?) != null && ((bool?)cb.Tag) == true)
            {
                cb.Tag = false;
                return true;
            }
            return false;
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            if (cbVideo.Checked && !HasAllFilters(tbFilter.Text, VideoFilters))
            {
                cbVideo.Tag = true;
                cbVideo.Checked = false;
            }
            if (cbImages.Checked && !HasAllFilters(tbFilter.Text, ImageFilters))
            {
                cbImages.Tag = true;
                cbImages.Checked = false;
            }
            if (cbAudio.Checked && !HasAllFilters(tbFilter.Text, AudioFilters))
            {
                cbAudio.Tag = true;
                cbAudio.Checked = false;
            }
            if (string.IsNullOrWhiteSpace(tbFilter.Text))
            {
                tbFilter.Text = "*.*";
            }
        }

        private bool HasAllFilters(string userFilter, string[] systemFilters)
        {
            userFilter = userFilter.ToLower();
            var userExts = userFilter.Split(new char[] { ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var ext in systemFilters)
            {
                if (!userExts.Contains(ext))
                {
                    return false;
                }
            }
            return true;
        }

        private string MergeFilterStrings(string userFilter, string[] systemFilters, bool merge)
        {
            userFilter = userFilter.ToLower();
            var userExts = userFilter.Split(new char[] { ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!merge)
            {
                userExts.RemoveAll(e => systemFilters.Contains(e));
            }
            else if (merge)
            {
                userExts.RemoveAll(e => e.Trim('*', '.', ' ') == "");
                foreach (var ext in systemFilters)
                {
                    if (!userExts.Contains(ext))
                    {
                        userExts.Add(ext);
                    }
                }
            }

            return string.Join(";", userExts);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            CustomMessageBox.Show("QuickSearch " + version + "\r\n© 2015 Jason Kalpin (Jsnklpn@gmail.com)\r\n\r\nA fast, lightweight, and convenient search tool.™", "About", CustomMessageBox.eDialogButtons.OK, Properties.Resources.icon2.ToBitmap());
        }

        private static Bitmap GetFolderIconBitmap(string dir, bool large = false)
        {
            return GetFolderIcon(dir, large).ToBitmap();
        }

        private static System.Drawing.Icon GetFolderIcon(string dir, bool large = false)
        {
            Native.Shell32.SHFILEINFO shfi = new Native.Shell32.SHFILEINFO();
            uint flags = Native.Shell32.SHGFI_ICON;
            if (dir == null) flags |= Native.Shell32.SHGFI_USEFILEATTRIBUTES;

            /* Check the size specified for return. */
            if (large)
            {
                flags += Native.Shell32.SHGFI_LARGEICON;  // include the large icon flag
            }
            else
            {
                flags += Native.Shell32.SHGFI_SMALLICON; // include the small icon flag
            }

            if (dir == null || !Directory.Exists(dir))
            {
                dir = Path.GetDirectoryName(Application.ExecutablePath.ToString());    
            }
            Native.Shell32.SHGetFileInfo(dir,
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbDirectories.Items.Clear();
        }
    }
}
