using Microsoft.Xna.Framework.Input;
using System;

namespace MonoEditorEndless.Engine.Input
{
    internal class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(eMouseInputs eMouseInput, MouseState currentMouseState, MouseState prevMouseState)
        {
            _currentState = currentMouseState;
            _prevState = prevMouseState;
            _eMouseInput = eMouseInput;
        }

        public readonly MouseState _currentState;
        public readonly MouseState _prevState;
        public readonly eMouseInputs _eMouseInput;
    }
}
