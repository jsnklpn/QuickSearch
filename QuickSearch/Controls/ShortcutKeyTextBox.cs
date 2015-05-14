using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickSearch.Infrastructure;
using QuickSearch.Models;

namespace QuickSearch.Controls
{
    public class ShortcutKeyTextBox : TextBox
    {
        private ShortcutKeyCombo _shortcutKeys;
        private bool _winRKeyDown;
        private bool _winLKeyDown;

        public ShortcutKeyCombo ShortcutKeys
        {
            get
            {
                return _shortcutKeys;
            }
            set
            {
                if (value == null)
                {
                    _shortcutKeys = new ShortcutKeyCombo(Infrastructure.ModifierKeys.None, Keys.None);
                    this.Text = Keys.None.ToString();
                }
                else
                {
                    _shortcutKeys = value;
                    this.Text = _shortcutKeys.ToString();
                }
            }
        }

        public ShortcutKeyTextBox()
        {
            _winLKeyDown = false;
            _winRKeyDown = false;

            this.ReadOnly = true;
            _shortcutKeys = new ShortcutKeyCombo(Infrastructure.ModifierKeys.None, Keys.None);
            this.Text = Keys.None.ToString();
        }

        protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
        {
            switch(keyData)
            {
                //case Keys.Tab:
                case Keys.Shift:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.Control:
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.Alt:
                case Keys.LWin:
                case Keys.RWin:
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            // If the user does a system-reserved hotkey, delete it.
            if (_winLKeyDown || _winRKeyDown)
            {
                _winLKeyDown = false;
                _winRKeyDown = false;
                this.ShortcutKeys = null;
            }

            base.OnLostFocus(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyData == Keys.LWin || e.KeyData == Keys.RWin)
            {
                _winLKeyDown = (e.KeyData == Keys.LWin) ? false : _winLKeyDown;
                _winRKeyDown = (e.KeyData == Keys.RWin) ? false : _winRKeyDown;
            }

            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!Enabled) return;

            if (e.KeyData == Keys.LWin || e.KeyData == Keys.RWin)
            {
                _winLKeyDown = (e.KeyData == Keys.LWin) ? true : _winLKeyDown;
                _winRKeyDown = (e.KeyData == Keys.RWin) ? true : _winRKeyDown;
            }

            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
            {
                ShortcutKeys = null; // clear the shortcut
                return;
            }

            ModifierKeys mods = Infrastructure.ModifierKeys.None;

            if (e.Shift) mods |= Infrastructure.ModifierKeys.Shift;
            if (e.Control) mods |= Infrastructure.ModifierKeys.Control;
            if (e.Alt) mods |= Infrastructure.ModifierKeys.Alt;
            if (_winLKeyDown || _winRKeyDown) mods |= Infrastructure.ModifierKeys.Win;

            Keys primaryKey = StripModifiers(e.KeyData);

            ShortcutKeyCombo combo = new ShortcutKeyCombo(mods, primaryKey);

            this.ShortcutKeys = combo;

            base.OnKeyDown(e);
        }

        private Keys StripModifiers(Keys key)
        {
            key = key & ~Keys.Modifiers;

            for (Keys k = Keys.A; k <= Keys.Z; k++)
            {
                if (key == k)
                {
                    return k;
                }
            }
            for (Keys k = Keys.F1; k <= Keys.F12; k++)
            {
                if (key == k)
                {
                    return k;
                }
            }

            return Keys.None;
        }
    }
}
