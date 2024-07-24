using Microsoft.Xna.Framework.Input;
using System;

namespace MonoEditorEndless.Engine.Input
{
    internal class KeyboardEventArgs : EventArgs
    {
        public KeyboardEventArgs(Keys key, KeyboardState currentKeyboardState, KeyboardState prevKeyboardState)
        {
            CurrentState = currentKeyboardState;
            PrevState = prevKeyboardState;
            Key = key;
        }

        public readonly KeyboardState CurrentState;
        public readonly KeyboardState PrevState;
        public readonly Keys Key;
    }
}
