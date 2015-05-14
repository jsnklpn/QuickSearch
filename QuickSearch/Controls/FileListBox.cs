using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using QuickSearch.Infrastructure;
using QuickSearch.Helpers;

namespace QuickSearch.Controls
{
    public class FileListBox : ListBox
    {
        private Font _smallFont;
        private bool _drawIcon;
        private bool _drawSmallIcons;

        private readonly Color _darkYellow = Color.FromArgb(255, 229, 195, 101);
        private readonly Color _lightYellow = Color.FromArgb(255, 255, 242, 157);
        private readonly Color _lightBlue = Color.FromArgb(255, 234, 240, 255);
        private readonly Color _mediumBlue = Color.FromArgb(255, 214, 219, 233);
        private readonly Color _mediumGreen = Color.FromArgb(255, 24, 130, 24);
        private Brush _largeTextBrush = Brushes.Black;
        private Brush _smallTextBrush;
        private Pen _darkYellowPen;
        private Pen _mediumBluePen;

        private Point _oldMouseLocation = Point.Empty;

        public bool DrawSmallIcons
        {
            get { return _drawSmallIcons; }
            set { _drawSmallIcons = value; }
        }

        public FileListBox()
        {
            _darkYellowPen = new Pen(_darkYellow);
            _mediumBluePen = new Pen(_mediumBlue);
            _smallTextBrush = new SolidBrush(Color.Gray);

            this.DoubleBuffered = true;
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.BackColor = _lightBlue;
            _smallFont = new Font(this.Font.FontFamily, 7f, FontStyle.Regular);

            _drawIcon = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _smallFont.Dispose();
                _smallTextBrush.Dispose();
                _darkYellowPen.Dispose();
                _mediumBluePen.Dispose();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // Detect right mouse click on item and show file context menu
            if (e.Button == MouseButtons.Right)
            {
                int index = this.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    var item = this.Items[index] as FileInfo;
                    if (item != null)
                    {
                        ShellContextMenu scm = new ShellContextMenu();
                        FileInfo[] files = new FileInfo[1];
                        files[0] = item;
                        scm.ShowContextMenu(files, Cursor.Position);
                        return;
                    }

                    var dirItem = this.Items[index] as DirectoryInfo;
                    if (dirItem != null)
                    {
                        ShellContextMenu scm = new ShellContextMenu();
                        DirectoryInfo[] dirs = new DirectoryInfo[1];
                        dirs[0] = dirItem;
                        scm.ShowContextMenu(dirs, Cursor.Position);
                        return;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Location != _oldMouseLocation)
            {
                int index = this.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    this.SelectedIndex = index;
                }    
            }

            _oldMouseLocation = e.Location;
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = _drawSmallIcons ? 20 : 32;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            // Draw background for item
            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            Color backColor = selected ? _lightYellow : _lightBlue;
            e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, DrawItemState.None, e.ForeColor, backColor);
            e.DrawBackground();

            var fileItem = this.Items[e.Index] as FileInfo;
            if (fileItem != null)
            {
                DrawFile(e, fileItem);
                DrawBorders(e, selected);
            }
            else
            {
                var dirItem = this.Items[e.Index] as DirectoryInfo;
                if (dirItem != null)
                {
                    if (dirItem.Parent != null)
                        DrawFolder(e, dirItem);
                    else
                        DrawFolderWithNoParent(e, dirItem);
                    DrawBorders(e, selected);
                }
                else
                {
                    base.OnDrawItem(e);
                }
            }
        }

        private void DrawBorders(DrawItemEventArgs e, bool selected)
        {
            if (_drawSmallIcons)
            {
                if (selected)
                {
                    e.Graphics.DrawRectangle(_darkYellowPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                }
            }
            else
            {
                e.Graphics.DrawLine(_mediumBluePen, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);
                e.Graphics.DrawLine(_mediumBluePen, e.Bounds.X, e.Bounds.Y + e.Bounds.Height, e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height);
            }
        }

        private void DrawFolderWithNoParent(DrawItemEventArgs e, DirectoryInfo item)
        {
            StringFormat format =
            new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisPath,
                LineAlignment = StringAlignment.Center
            };

            int iconSize = _drawIcon ? e.Bounds.Height : 0;

            // draw filename and directory strings
            if (_drawSmallIcons)
            {
                var textRect1 = new Rectangle(e.Bounds.X + 1 + iconSize, e.Bounds.Y + 2, e.Bounds.Width - 1 - iconSize, e.Bounds.Height - 2);
                e.Graphics.DrawString(item.Name, this.Font, _largeTextBrush, textRect1, format);
            }
            else
            {
                var textRect1 = new Rectangle(e.Bounds.X + 1 + iconSize, e.Bounds.Y, e.Bounds.Width - 1 - iconSize,
                    e.Bounds.Height);

                e.Graphics.DrawString(item.Name, this.Font, _largeTextBrush, textRect1, format);
            }

            if (_drawIcon)
            {
                // draw icon
                var icon = _drawSmallIcons ? IconHelper.SmallFolderIcon : IconHelper.LargeFolderIcon;
                if (icon != null)
                {
                    int yPadding = _drawSmallIcons ? 2 : 0;
                    int xPadding = _drawSmallIcons ? 1 : 0;
                    e.Graphics.DrawIcon(icon, e.Bounds.X + xPadding, e.Bounds.Y + yPadding);
                }
            }
        }

        private void DrawFolder(DrawItemEventArgs e, DirectoryInfo item)
        {
            StringFormat format =
            new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisPath
            };

            int iconSize = _drawIcon ? e.Bounds.Height : 0;

            // draw filename and directory strings
            if (_drawSmallIcons)
            {
                var textRect1 = new Rectangle(e.Bounds.X + 1 + iconSize, e.Bounds.Y + 2, e.Bounds.Width - 1 - iconSize, e.Bounds.Height - 2);
                e.Graphics.DrawString(item.Name, this.Font, _largeTextBrush, textRect1, format);

                var pathLength = e.Graphics.MeasureString(item.Name, this.Font, textRect1.Width, format);
                var totalFileTextLength = iconSize + 1 + (int)pathLength.Width;

                StringFormat format2 =
                new StringFormat
                {
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisPath,
                    Alignment = StringAlignment.Far
                };
                var textRect2 = new Rectangle(e.Bounds.X + totalFileTextLength, e.Bounds.Y + 2, e.Bounds.Width - totalFileTextLength, e.Bounds.Height - 2);
                var dirTextLength = e.Graphics.MeasureString(item.Parent.Name, this.Font, textRect2.Width, format2);

                // If there is room, draw the folder name and folder icon on the right side
                if (totalFileTextLength + IconHelper.SmallFolderIcon.Width + dirTextLength.Width <= e.Bounds.Width)
                {
                    e.Graphics.DrawString(item.Parent.Name, this.Font, _smallTextBrush, textRect2, format2);
                    e.Graphics.DrawIcon(IconHelper.SmallFolderIcon, e.Bounds.Right - (int)dirTextLength.Width - IconHelper.SmallFolderIcon.Width, e.Bounds.Y + 2);
                }
            }
            else
            {
                var textRect1 = new Rectangle(e.Bounds.X + 1 + iconSize, e.Bounds.Y + 3, e.Bounds.Width - 1 - iconSize,
                    (e.Bounds.Height / 2));
                var textRect2 = new Rectangle(e.Bounds.X + iconSize, e.Bounds.Y + (e.Bounds.Height / 2) + 3,
                    e.Bounds.Width - iconSize, (e.Bounds.Height / 2) - 3);
                e.Graphics.DrawString(item.Name, this.Font, _largeTextBrush, textRect1, format);
                e.Graphics.DrawString("   " + item.Parent.FullName, _smallFont, _smallTextBrush, textRect2, format);
            }

            if (_drawIcon)
            {
                // draw icon
                var icon = _drawSmallIcons ? IconHelper.SmallFolderIcon : IconHelper.LargeFolderIcon;
                if (icon != null)
                {
                    int yPadding = _drawSmallIcons ? 2 : 0;
                    int xPadding = _drawSmallIcons ? 1 : 0;
                    e.Graphics.DrawIcon(icon, e.Bounds.X + xPadding, e.Bounds.Y + yPadding);
                }
            }
        }

        private void DrawFile(DrawItemEventArgs e, FileInfo item)
        {
            StringFormat format =
            new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap,
                Trimming = StringTrimming.EllipsisPath
            };

            int iconSize = _drawIcon ? e.Bounds.Height : 0;

            // draw filename and directory strings
            if (_drawSmallIcons)
            {
                var textRect1 = new Rectangle(e.Bounds.X + 1 + iconSize, e.Bounds.Y + 2, e.Bounds.Width - 1 - iconSize, e.Bounds.Height - 2);
                e.Graphics.DrawString(item.Name, this.Font, _largeTextBrush, textRect1, format);

                var pathLength = e.Graphics.MeasureString(item.Name, this.Font, textRect1.Width, format);
                var totalFileTextLength = iconSize + 1 + (int)pathLength.Width;

                StringFormat format2 =
                new StringFormat
                {
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisPath,
                    Alignment = StringAlignment.Far
                };
                var textRect2 = new Rectangle(e.Bounds.X + totalFileTextLength, e.Bounds.Y + 2, e.Bounds.Width - totalFileTextLength, e.Bounds.Height - 2);
                var dirTextLength = e.Graphics.MeasureString(item.Directory.Name, this.Font, textRect2.Width, format2);

                // If there is room, draw the folder name and folder icon on the right side
                if (totalFileTextLength + IconHelper.SmallFolderIcon.Width + dirTextLength.Width <= e.Bounds.Width)
                {
                    e.Graphics.DrawString(item.Directory.Name, this.Font, _smallTextBrush, textRect2, format2);
                    e.Graphics.DrawIcon(IconHelper.SmallFolderIcon, e.Bounds.Right - (int)dirTextLength.Width - IconHelper.SmallFolderIcon.Width, e.Bounds.Y + 2);
                }
            }
            else
            {
                var textRect1 = new Rectangle(e.Bounds.X + 1 + iconSize, e.Bounds.Y + 3, e.Bounds.Width - 1 - iconSize,
                    (e.Bounds.Height / 2));
                var textRect2 = new Rectangle(e.Bounds.X + iconSize, e.Bounds.Y + (e.Bounds.Height / 2) + 3,
                    e.Bounds.Width - iconSize, (e.Bounds.Height / 2) - 3);
                e.Graphics.DrawString(item.Name, this.Font, _largeTextBrush, textRect1, format);
                e.Graphics.DrawString("   " + item.DirectoryName, _smallFont, _smallTextBrush, textRect2, format);
            }

            if (_drawIcon)
            {
                // draw icon
                var ext = string.IsNullOrWhiteSpace(item.Extension) ? " " : item.Extension;
                var icon = IconHelper.GetIconFromExtension(ext, !_drawSmallIcons);
                if (icon != null)
                {
                    int yPadding = _drawSmallIcons ? 2 : 0;
                    int xPadding = _drawSmallIcons ? 1 : 0;
                    e.Graphics.DrawIcon(icon, e.Bounds.X + xPadding, e.Bounds.Y + yPadding);
                }
            }
        }
    }
}
