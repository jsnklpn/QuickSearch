using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Policy;
using QuickSearch.Controls;
using QuickSearch.Helpers;
using QuickSearch.Infrastructure;
using QuickSearch.Models;
using QuickSearch.Properties;

namespace QuickSearch.Forms
{
    public partial class SeachForm : Form
    {
        private const int SEARCH_WIDTH = 500;
        private const int SEARCH_HEIGHT = 20;

        private FilePreviewForm _filePreviewForm;
        private List<FileSystemInfo> _directoryData;
        private KeyboardHook _keyboardHook;
        private List<FileSystemWatcher> _fileSystemWatchers;
        private List<FileSystemItemError> _indexBuildErrors;

        private bool _splitStringSearch;
        private SearchTextBox _inputBox;
        private FileListBox _autoCompleteBox;
        private NotifyIcon _notifyIcon;
        private bool _systrayExitClicked;
        private bool _showBalloonTip;

        private System.Threading.Thread _searchThread;
        private AutoResetEvent _newSearchEvent;
        private string _searchText;
        private bool _cancelSearch;

        // Cached settings so we don't need to keep accessing the config file
        private List<string> _searchDirectories;
        private bool _allowFolderSearch;
        private string _searchFilter;
        private bool _showSmallIcons;
        private ShortcutKeyCombo _hotkey;
        private bool _runInSystemTray;
        private int _maxResults;
        private bool _showIndexErrors;
        private bool _showFilePreview;

        private readonly string TipFormat = Resources.SearchTipFormat;

        public SeachForm()
        {
            InitializeComponent();
            LoadSettings();

            #region UI Controls

            this.Width = SEARCH_WIDTH;
            this.Text = Resources.ApplicationName;
            this.Icon = Properties.Resources.icon2;
            //this.TopMost = true;
            this.Resize += SeachForm_Resize;
            this.FormClosing += Form_FormClosing;
            this.Move += SeachForm_Move;
            this.Load += SeachForm_Load;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.toolStripProgressBar.Visible = false;
            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.BalloonTipTitle = Resources.QuickSearch_is_still_running;
            _notifyIcon.BalloonTipText = Resources.To_close_right_click_and_choose_Exit;
            _notifyIcon.Icon = this.Icon;
            _notifyIcon.Text = Resources.ApplicationName;
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(new MenuItem(Resources.OptionsMenuText, OptionsMenuItemClicked));
            _notifyIcon.ContextMenu.MenuItems.Add(new MenuItem(Resources.ExitMenuText, ExitMenuItemClicked));

            _inputBox = new SearchTextBox();
            _inputBox.ReadOnly = false;
            _inputBox.Text = "";
            _inputBox.Top = 0;
            _inputBox.Left = 0;
            _inputBox.Dock = DockStyle.Top;
            _inputBox.AutoSize = false;
            _inputBox.TextAlign = HorizontalAlignment.Left;
            _inputBox.TextChanged += InputTextChanged;
            _inputBox.KeyDown += InputBox_KeyDown;
            _inputBox.MouseWheel += InputBox_MouseWheel;
            _inputBox.LostFocus += InputBox_LostFocus;
            SetSearchCue();
            this.Controls.Add(_inputBox);

            _autoCompleteBox = new FileListBox();
            _autoCompleteBox.DrawSmallIcons = _showSmallIcons;
            _autoCompleteBox.SelectionMode = SelectionMode.One;
            _autoCompleteBox.Width = _inputBox.Width;
            _autoCompleteBox.Height = this.ClientSize.Height - _inputBox.Height - statusStrip1.Height;
            _autoCompleteBox.Left = _inputBox.Left;
            _autoCompleteBox.Top = _inputBox.Bottom;
            _autoCompleteBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _autoCompleteBox.DoubleClick += AutoCompleteBox_DoubleClick;
            //_autoCompleteBox.MouseWheel += InputBox_MouseWheel;
            _autoCompleteBox.KeyDown += InputBox_KeyDown;
            _autoCompleteBox.SelectedValueChanged += AutoCompleteBox_SelectedValueChanged;
            this.Controls.Add(_autoCompleteBox);

            _inputBox.ControlToScroll = _autoCompleteBox;
            _inputBox.Focus();

            #endregion

            _filePreviewForm = new FilePreviewForm();
            _filePreviewForm.GotFocus += (s, a) => { this.Focus(); };

            Application.ApplicationExit += ApplicationExit;
            _directoryData = new List<FileSystemInfo>();
            _indexBuildErrors = new List<FileSystemItemError>();
            _splitStringSearch = true;
            _systrayExitClicked = false;
            _showBalloonTip = true;

            InitDirectoryWatchers();
            InitHotkeyHook();

            ShowAutoComplete(false);

            _newSearchEvent = new AutoResetEvent(false);
            _searchThread = new Thread(SearchThread);
            _searchThread.IsBackground = true;
            _searchThread.Start();

            RebuildIndexOnBackgroundThread();
        }

        private void LoadSettings()
        {
            _searchDirectories = ConfigHelper.SearchDirectories;
            _allowFolderSearch = ConfigHelper.AllowFolderSearch;
            _searchFilter = ConfigHelper.SearchFilter;
            _showSmallIcons = ConfigHelper.ShowSmallIcons;
            _hotkey = ConfigHelper.Hotkey;
            _runInSystemTray = ConfigHelper.RunInSystemTray;
            _maxResults = ConfigHelper.MaxResults;
            _showIndexErrors = ConfigHelper.ShowIndexErrors;
            _showFilePreview = ConfigHelper.ShowFilePreview;
        }

        void ApplicationExit(object sender, EventArgs e)
        {
            // There is a bug that the system tray icon doesn't leave when the app closes
            // Hopefully, this will fix it
 	        _notifyIcon.Visible = false;
            _notifyIcon.Dispose();

            if (_keyboardHook == null)
            {
                _keyboardHook.Dispose();
            }
        }

        void SeachForm_Load(object sender, EventArgs e)
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Contains("-startminimized"))
            {
                _showBalloonTip = false;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        void SeachForm_Move(object sender, EventArgs e)
        {
            SetFilePreviewLocation();
        }

        void AutoCompleteBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_showFilePreview)
                UpdateFilePreview();
        }

        void UpdateFilePreview()
        {
            if (_showFilePreview)
            {
                FileSystemInfo item = _autoCompleteBox.SelectedItem as FileSystemInfo;
                if (item != null)
                {
                    SetFilePreviewText(item);
                }
                else
                {
                    SetFilePreviewText(null);
                }
            }
            else
            {
                SetFilePreviewText(null);
            }
        }

        void SetFilePreviewText(FileSystemInfo item)
        {
            if (item == null)
            {
                _filePreviewForm.Hide();
                return;
            }

            var ext = string.IsNullOrWhiteSpace(item.Extension) ? " " : item.Extension;
            var icon = item is DirectoryInfo ? IconHelper.LargeFolderIcon : IconHelper.GetIconFromExtension(ext, true);

            _filePreviewForm.SuspendLayout();
            SetFilePreviewLocation();
            _filePreviewForm.LabelText = item.Name;
            _filePreviewForm.SetLabelIcon(icon);
            _filePreviewForm.ResumeLayout();
            _filePreviewForm.Show();
            //_filePreviewForm.Activate(); // This will fix the issue with the window being hidden, but it causes flicker
        }

        void SetFilePreviewLocation()
        {
            _filePreviewForm.Left = this.Left + ((this.Width - _filePreviewForm.Width) / 2);
            _filePreviewForm.Top = this.Top - _filePreviewForm.Height;
        }

        void InitHotkeyHook()
        {
            if (_keyboardHook == null)
            {
                _keyboardHook = new KeyboardHook();
                _keyboardHook.KeyPressed += keyboardHook_KeyPressed;
            }
            else
            {
                _keyboardHook.UnregisterAllHotKeys();
            }

            ShortcutKeyCombo hotkey = _hotkey;
            if (!hotkey.IsNone)
            {
                _keyboardHook.RegisterHotKey(hotkey.Modifier, hotkey.Key);

                // Set help text so users know what the hotkey is
                _notifyIcon.BalloonTipText = string.Format(TipFormat, hotkey);
            }
        }

        void InitDirectoryWatchers()
        {
            if (_fileSystemWatchers != null)
            {
                foreach (var fsw in _fileSystemWatchers)
                {
                    fsw.EnableRaisingEvents = false;
                    fsw.Changed -= OnFileChanged;
                    fsw.Created -= OnFileChanged;
                    fsw.Deleted -= OnFileChanged;
                    fsw.Renamed -= OnFileRenamed;
                    fsw.Dispose();
                }
                _fileSystemWatchers.Clear();
            }

            _fileSystemWatchers = new List<FileSystemWatcher>();

            foreach (var dir in _searchDirectories)
            {
                var fsw = new FileSystemWatcher(dir)
                {
                    InternalBufferSize = 0x4000,
                    IncludeSubdirectories = true,
                };

                fsw.Changed += OnFileChanged;
                fsw.Created += OnFileChanged;
                fsw.Deleted += OnFileChanged;
                fsw.Renamed += OnFileRenamed;

                fsw.EnableRaisingEvents = true;

                _fileSystemWatchers.Add(fsw);
            }
        }

        void SetSearchCue()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Search for files{0} in ", (_allowFolderSearch ? " and folders" : string.Empty));
            foreach (var dir in _searchDirectories)
            {
                sb.AppendFormat("\\{0}, ", Path.GetFileName(dir));
            }
            var cue = sb.ToString().TrimEnd(' ', ',');
            _inputBox.Cue = cue;
        }

        void keyboardHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            _inputBox.Text = "";
            if (this.WindowState == FormWindowState.Minimized)
            {
                ShowSearchForm(true);
            }
            else
            {
                this.Activate();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        void OptionsMenuItemClicked(object sender, EventArgs e)
        {
            ShowOptions();
        }

        void ExitMenuItemClicked(object sender, EventArgs e)
        {
            _systrayExitClicked = true;
            this.Close();
        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowSearchForm(true);
        }

        void SeachForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                ShowSearchForm(false, true);
            }
        }

        void ShowSearchForm(bool show, bool alreadyMinimized = false)
        {
            if (!show && !alreadyMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                return;
            }

            if (show)
            {
                _inputBox.Text = "";
                this.Show();
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                _inputBox.Focus();
                _notifyIcon.Visible = false;
            }
            else
            {
                if (_runInSystemTray)
                {
                    //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    _notifyIcon.Visible = true;
                    if (_showBalloonTip)
                    {
                        _notifyIcon.ShowBalloonTip(1000);

                        // Only show the tip once. Otherwise it gets annoying..
                        _showBalloonTip = false; 
                    }
                    this.ShowInTaskbar = false;
                    this.Hide();
                    this.Visible = false;
                }
            }
        }

        public void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Only allow the user to quit through the system tray menu
            if (_runInSystemTray &&
                !_systrayExitClicked &&
                e.CloseReason != CloseReason.TaskManagerClosing &&
                e.CloseReason != CloseReason.WindowsShutDown)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                // Allow the app to close

                _notifyIcon.Visible = false;
                _keyboardHook.Dispose();
                _fileSystemWatchers.ForEach(fsw => fsw.Dispose());
                _notifyIcon.Dispose();

                // TODO: Save any big indexes to a file.
            }
        }

        private void ShowAutoComplete(bool show)
        {
            if (show && _inputBox.Text.Any())
            {
                _inputBox.Dock = DockStyle.Top;
                _autoCompleteBox.Visible = true;
                SetToolStripText(Resources.Press_Ctrl_Enter_to_open_file_location);
                this.ClientSize = new Size(SEARCH_WIDTH, Screen.PrimaryScreen.Bounds.Height / 2 - 50);

                UpdateFilePreview();
            }
            else
            {
                _inputBox.Dock = DockStyle.Fill;
                _autoCompleteBox.Visible = false;
                SetToolStripText(Resources.Press_F1_for_options);
                this.ClientSize = new Size(SEARCH_WIDTH, SEARCH_HEIGHT + statusStrip1.Height);

                SetFilePreviewText(null);
            }
        }

        private void SetToolStripText(string text, bool showLoadingBar=false)
        {
            this.toolStripStatusLabel1.Text = text;
            this.toolStripProgressBar.Visible = showLoadingBar;
        }

        private void AutoCompleteBox_DoubleClick(object sender, EventArgs args)
        {
            if (_autoCompleteBox.SelectedIndex != -1)
            {
                var fileinfo = _autoCompleteBox.SelectedItem as FileSystemInfo;
                OpenFile(fileinfo, true);
            }
        }

        private void InputBox_MouseWheel(object sender, MouseEventArgs args)
        {
            // The inputbox mousewheel message is getting redirected to the file listbox

            //HandledMouseEventArgs he = args as HandledMouseEventArgs;
            //if (args.Delta != 0)
            //{
            //    MoveAutoCompleteBox(args.Delta > 0);
            //}
            //he.Handled = true;
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.F1:
                    ShowOptions();
                    args.Handled = true;
                    break;
                case Keys.Up:
                    MoveAutoCompleteBox(true);
                    args.Handled = true;
                    break;
                case Keys.Tab:
                case Keys.Down:
                    MoveAutoCompleteBox(false);
                    args.Handled = true;
                    break;
                case Keys.PageDown:
                    MoveAutoCompleteBox(false, 10);
                    args.Handled = true;
                    break;
                case Keys.PageUp:
                    MoveAutoCompleteBox(true, 10);
                    args.Handled = true;
                    break;
                case Keys.Return:
                    if (!_inputBox.Text.Any())
                        return;
                    FileSystemInfo item = _autoCompleteBox.SelectedItem as FileSystemInfo;
                    if (item != null)
                    {
                        // First, hide the form
                        if (_runInSystemTray)
                            ShowSearchForm(false);
                        else
                            _inputBox.Text = "";

                        bool openFolder = (args.Shift || args.Control || args.Alt);
                        OpenFile(item, !openFolder);
                    }
                    args.Handled = true;
                    break;
                case Keys.Escape:
                    this.Close();
                    args.Handled = true;
                    break;
                default:
                    break;
            }
        }

        void InputBox_LostFocus(object sender, EventArgs e)
        {
            _inputBox.Focus();
        }

        private void ShowOptions()
        {
            using (var frm = new OptionsForm())
            {
                // Suspend the keyboard hook for now, so the options will work correctly.
                _keyboardHook.UnregisterAllHotKeys();

                if (this.WindowState == FormWindowState.Minimized)
                    frm.StartPosition = FormStartPosition.CenterScreen;

                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Re-cache the settings.
                    LoadSettings();

                    _autoCompleteBox.DrawSmallIcons = _showSmallIcons;
                    _inputBox.Text = ""; // Clear the textbox

                    if (frm.RebuildIndexRequired)
                    {
                        SetSearchCue();

                        // Settings were changed, rebuild the index.
                        InitDirectoryWatchers();
                        RebuildIndexOnBackgroundThread();
                    }
                }

                // Create the hotkey hook again
                InitHotkeyHook();
            }
        }

        private void MoveAutoCompleteBox(bool up, int amount = 1)
        {
            if (_autoCompleteBox.SelectedIndex == -1)
                return;

            if (up)
            {
                if (_autoCompleteBox.SelectedIndex - amount >= 0)
                    _autoCompleteBox.SelectedIndex -= amount;
                else
                    _autoCompleteBox.SelectedIndex = 0;
            }
            else
            {
                if (_autoCompleteBox.SelectedIndex + amount <= _autoCompleteBox.Items.Count - 1)
                    _autoCompleteBox.SelectedIndex += amount;
                else
                    _autoCompleteBox.SelectedIndex = _autoCompleteBox.Items.Count - 1;
            }
        }

        private void InputTextChanged(object sender, EventArgs args)
        {
            if (_inputBox.Text.Length > 0)
            {
                StartNewSearch(_inputBox.Text);
            }
            else
            {
                ShowAutoComplete(false);
            }
        }

        private void StartNewSearch(string text)
        {
            SetToolStripText(Resources.Searching, true);

            _searchText = text;
            _cancelSearch = true;
            _newSearchEvent.Set();
        }

        private void SearchThread()
        {
            try
            {
                while (true)
                {
                    _newSearchEvent.WaitOne();
                    _cancelSearch = false;
                    
                    // Do search
                    var autoCompleteList = GetAutoComplete(_searchText);

                    if (autoCompleteList == null)
                    {
                        // search was cancelled and taken over by another search
                    }
                    else
                    {
                        if (autoCompleteList.Any())
                        {
                            this.Invoke((Action) (() =>
                            {
                                _autoCompleteBox.SuspendLayout();
                                _autoCompleteBox.DataSource = autoCompleteList.Take(_maxResults).ToList();
                                _autoCompleteBox.ResumeLayout();
                                ShowAutoComplete(true);                              
                            }));
                            
                        }
                        else
                        {
                            this.Invoke((Action) (() => ShowAutoComplete(false)));
                        }
                    }
                }
            }
            catch { }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    CreateFileInIndex(args.FullPath);
                    break;
                case WatcherChangeTypes.Deleted:
                    DeleteFileInIndex(args.FullPath);
                    break;
                //case WatcherChangeTypes.Renamed:
                    // We shouldn't be here.
                    //break;
                default:
                    break;
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs args)
        {
            DeleteFileInIndex(args.OldFullPath);
            CreateFileInIndex(args.FullPath);
        }

        private void CreateFileInIndex(string fullPath)
        {
            lock (_directoryData)
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(fullPath);
                    bool isDir = ((attr & FileAttributes.Directory) == FileAttributes.Directory);

                    if (isDir && !_allowFolderSearch)
                        return;

                    if (!isDir && !DoesFileMatchFilter(fullPath))
                        return;

                    if (_directoryData.Where(f => f.FullName == fullPath).Any())
                        return; // The file already exists.

                    if (isDir)
                        _directoryData.Add(new DirectoryInfo(fullPath));
                    else
                        _directoryData.Add(new FileInfo(fullPath));

                    SortIndex();
                }
                catch (Exception)
                {
                    // This can fail when the file being added cannot be accessed due to security                    
                }   
            }
        }

        private void DeleteFileInIndex(string fullPath)
        {
            lock (_directoryData)
            {
                _directoryData.RemoveAll(f => f.FullName == fullPath);
            }
        }

        private void OpenFile(FileSystemInfo file, bool execute)
        {
            if (file == null) return;

            try
            {
                if (execute)
                    Process.Start(file.FullName);
                else
                    Process.Start("explorer.exe", "/select, \"" + file.FullName + "\"");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Failed to open file \"{0}\".\r\n\r\n{1}", file.FullName, e.Message), "Failed to open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RebuildIndexOnBackgroundThread()
        {
            _inputBox.Enabled = false;
            this.Focus(); // Move focus away from input box
            SetToolStripText(Resources.Building_search_index, true);

            System.Threading.ThreadPool.QueueUserWorkItem(
                o =>
                    {
                        RebuildIndex();

                        // Done indexing. Update UI on main thread.
                        this.Invoke((Action) (() =>
                        {
                            if (_showIndexErrors)
                            {
                                lock (_indexBuildErrors)
                                {
                                    if (_indexBuildErrors.Any())
                                    {
                                        using (var frm = new ErrorListForm(Resources.ErrorsWhileBuildingTheSearchIndex, _indexBuildErrors))
                                        {
                                            frm.ShowDialog();
                                            if (frm.DontShowAgain && _showIndexErrors)
                                            {
                                                ConfigHelper.ShowIndexErrors = false;
                                                ConfigHelper.SaveSettings();
                                                _showIndexErrors = false;
                                            }
                                        }
                                    }
                                }
                            }
                            _inputBox.Enabled = true;
                            _inputBox.Focus();
                            SetToolStripText(Resources.Press_F1_for_options, false);
                        }));
                    });
        }

        private void RebuildIndex()
        {
            lock (_indexBuildErrors)
            {
                _indexBuildErrors.Clear();
            }

            lock (_directoryData)
            {
                _directoryData.Clear();

                foreach (var dir in _searchDirectories)
                {
                    AddEntriesToIndex(dir, true);
                }

                SortIndex();
            } 
        }

        private void SortIndex()
        {
            // Sort the list by short filename
            _directoryData.Sort((f1, f2) =>
            {
                if (f1.Name == null && f2.Name == null) return 0;
                else if (f1.Name == null) return -1;
                else if (f2.Name == null) return 1;
                else return f1.Name.CompareTo(f2.Name);
            });
        }

        private void AddEntriesToIndex(string path, bool recursive)
        {
            SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // Add the root dir
            if ((File.GetAttributes(path) & System.IO.FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                return;
            else
            {
                AddEntryToIndex(new DirectoryInfo(path));
            }

            var allItems = GetAllFileSystemInfos(path, _searchFilter, searchOption, _allowFolderSearch);

            foreach (var item in allItems)
            {
                AddEntryToIndex(item);
            }
        }

        List<FileSystemInfo> GetAllFileSystemInfos(string dir, string searchPattern, SearchOption option, bool includeDirectories)
        {
            var items = new List<FileSystemInfo>();

            // Don't look inside of false directories like "C:\Users\Bob\Documents\My Music"
            if ((File.GetAttributes(dir) & System.IO.FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                return items;

            try
            {
                if (includeDirectories)
                {
                    foreach (var subDir in Directory.EnumerateDirectories(dir))
                    {
                        var di = new DirectoryInfo(subDir);
                        if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            items.Add(di);
                        }
                    }
                }

                foreach (string filter in searchPattern.Split(';'))
                {
                    foreach (var fileName in Directory.EnumerateFiles(dir, filter.Trim(), SearchOption.TopDirectoryOnly))
                    {
                        var fi = new FileInfo(fileName);
                        if (((fi.Attributes & FileAttributes.Archive) == FileAttributes.Archive) ||
                            ((fi.Attributes & FileAttributes.Normal) == FileAttributes.Normal))
                        {
                            items.Add(fi);
                        }
                    }
                }

                if (option == SearchOption.AllDirectories)
                {
                    foreach (var subDir in Directory.EnumerateDirectories(dir))
                    {
                        items.AddRange(GetAllFileSystemInfos(subDir, searchPattern, option, includeDirectories));
                    }
                }
            }
            catch (Exception e)
            {
                lock (_indexBuildErrors)
                {
                    _indexBuildErrors.Add(new FileSystemItemError(dir, e));
                }
            }

            return items;
        }

        private void AddEntryToIndex(FileSystemInfo info)
        {
            _directoryData.Add(info);
        }

        private List<FileSystemInfo> GetAutoComplete(string userText)
        {
            var result = new List<FileSystemInfo>();
            string userTextLower = userText.Trim().ToLower();
            lock (_directoryData)
            {
                foreach (var item in _directoryData)
                {
                    if (_cancelSearch)
                    {
                        return null;
                    }

                    string itemLower = item.Name.ToLower();

                    if (itemLower.StartsWith(userTextLower) ||
                        (userTextLower.Length >= 3 && itemLower.Contains(userTextLower)))
                    {
                        result.Add(item);
                    }
                    else if (_splitStringSearch)
                    {
                        if (userTextLower.Length >= 3)
                        {
                            string onlySpaces =
                                itemLower.Replace('_', ' ').Replace('-', ' ').Replace('.', ' ').Replace("  ", " ");
                            if (onlySpaces.Contains(userTextLower))
                            {
                                result.Add(item);
                            }
                        }

                        // If not found, try splitting the filename and search each section
                        var subs = itemLower.Split(' ', '_', '-', '.');
                        if (subs.Length > 0)
                        {
                            for (int i = 1; i < subs.Length; i++)
                            {
                                if (subs[i].StartsWith(userTextLower))
                                {
                                    result.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool DoesFileMatchFilter(FileSystemInfo fileInfo)
        {
            return DoesFileMatchFilter(fileInfo.FullName);
        }

        private bool DoesFileMatchFilter(string fileName)
        {
            var filter = _searchFilter;

            if (filter.Trim(' ', '\t', '*') == ".")
                return true;

            foreach (string fileExtention in filter.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string ext = fileExtention.Trim().TrimStart('*');
                if (fileName.Trim().EndsWith(ext))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
