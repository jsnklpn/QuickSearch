using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickSearch.Infrastructure;

namespace QuickSearch.Controls
{
    public class SearchTextBox : TextBox
    {
        private string _cue;

        public Control ControlToScroll { get; set; }

        public string Cue
        {
            get { return _cue; }
            set
            {
                _cue = value ?? string.Empty;
                UpdateCue();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == Native.User32.WM_MOUSEWHEEL)
            {
                // send this to other control possibly
                if (ControlToScroll != null)
                    Native.User32.SendMessage(ControlToScroll.Handle, Native.User32.WM_MOUSEWHEEL, m.WParam, m.LParam);
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                return true;
            }
            else
            {
                return base.IsInputKey(keyData);
            }
        }

        private void UpdateCue()
        {
            if (IsHandleCreated && _cue != null)
            {
                Native.User32.SendMessage(Handle, Native.User32.EM_SETCUEBANNER, (IntPtr)1, _cue);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateCue();
        }

    }
}
