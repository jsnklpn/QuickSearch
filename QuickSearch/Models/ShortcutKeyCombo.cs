using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuickSearch.Infrastructure;

namespace QuickSearch.Models
{
    public class ShortcutKeyCombo
    {
        public ModifierKeys Modifier { get; set; }
        public Keys Key { get; set; }

        public bool IsNone
        {
            get { return Modifier == ModifierKeys.None && Key == Keys.None; }
        }

        public ShortcutKeyCombo(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public static ShortcutKeyCombo None
        {
            get { return new ShortcutKeyCombo(ModifierKeys.None, Keys.None); }
        }

        public static ShortcutKeyCombo FromString(string keyCombo)
        {
            if (string.IsNullOrWhiteSpace(keyCombo) || string.Equals(keyCombo, "None", StringComparison.InvariantCultureIgnoreCase))
            {
                return ShortcutKeyCombo.None;
            }

            ModifierKeys modifiers = ModifierKeys.None;
            Keys key = Keys.None;

            var items = keyCombo.Split('+');

            foreach (var item in items)
            {
                var tmpItem = item.Trim();
                ModifierKeys tmpMod;
                Keys tmpKey;
                if (Enum.TryParse<ModifierKeys>(tmpItem, true, out tmpMod))
                {
                    modifiers |= tmpMod;
                }
                else if (Enum.TryParse<Keys>(tmpItem, true, out tmpKey))
                {
                    if (key == Keys.None)
                        key = tmpKey;
                    else
                        throw new ArgumentException("The key \"" + item + "\" cannot be used in the combination because the primary key has already been assigned.", "keyCombo");
                }
                else
                {
                    throw new ArgumentException("The key \"" + item + "\" is not valid.", "keyCombo");
                }
            }

            return new ShortcutKeyCombo(modifiers, key);
        }

        public override string ToString()
        {
            if (IsNone) { return "None"; }

            String result = "";

            if ((Modifier & ModifierKeys.Win) == ModifierKeys.Win)
                result += ModifierKeys.Win.ToString() + " + ";
            if ((Modifier & ModifierKeys.Control) == ModifierKeys.Control)
                result += ModifierKeys.Control.ToString() + " + ";
            if ((Modifier & ModifierKeys.Alt) == ModifierKeys.Alt)
                result += ModifierKeys.Alt.ToString() + " + ";
            if ((Modifier & ModifierKeys.Shift) == ModifierKeys.Shift)
                result += ModifierKeys.Shift.ToString() + " + ";

            if (Key == Keys.None)
                result = result.TrimEnd(' ', '+');
            else
                result += Key.ToString();

            return result;
        }
    }
}
