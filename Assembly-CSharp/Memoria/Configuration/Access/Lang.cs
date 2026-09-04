using Memoria.Assets;
using Memoria.Prime;
using System;
using UnityEngine;
using static UIKeyTrigger;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class Lang
        {
            /// <summary>Modes: (0) no dual language / (1) switch by pressing a key / (2) both texts displayed</summary>
            public static Int32 DualLanguageMode => String.IsNullOrEmpty(DualLanguage) || DualLanguage == Localization.CurrentSymbol ? 0 : Instance._lang.DualLanguageMode;
            public static String DualLanguage => Instance._lang.DualLanguage.Value.Trim().Trim('"');

            private static KeyCode _cachedKey = KeyCode.None;
            private static Boolean _virtualToggleState = false;

            public static Boolean IsSwitchKeyActive()
            {
                if (_cachedKey == KeyCode.None)
                {
                    try
                    {
                        _cachedKey = (KeyCode)Enum.Parse(typeof(KeyCode), Instance._lang.KeyDualLanguage.Value.Trim().Trim('"'), true);
                    }
                    catch
                    {
                        _cachedKey = KeyCode.CapsLock;
                        Log.Message("[Lang] Error : Can't parse the new bind for DualLanguage => Use CapsLock by default.");
                    }
                }

                if (_cachedKey == KeyCode.CapsLock) return IsKeyLocked(LockKey.Caps);
                if (_cachedKey == KeyCode.Numlock) return IsKeyLocked(LockKey.Num);
                if (_cachedKey == KeyCode.ScrollLock) return IsKeyLocked(LockKey.Scroll);

                if (Input.GetKeyDown(_cachedKey))
                    _virtualToggleState = !_virtualToggleState;

                return _virtualToggleState;
            }
        }
    }
}
