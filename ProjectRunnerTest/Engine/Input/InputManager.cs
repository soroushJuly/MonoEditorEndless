using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoEditorEndless.Engine.Input
{
    public delegate void GameAction(eButtonState eButtonState, Vector2 amount);
    internal class InputManager
    {
        private Dictionary<Keys, GameAction> _keyBindings;
        private Dictionary<Chord, GameAction> _chordBindings;
        private Dictionary<eMouseInputs, GameAction> _mouseBindings;


        private InputListener _inputListener;


        public InputManager()
        {
            _inputListener = new InputListener();

            _mouseBindings = new Dictionary<eMouseInputs, GameAction>();
            _keyBindings = new Dictionary<Keys, GameAction>();

            _inputListener.OnKeyDown += OnKeyDown;
            _inputListener.OnKeyUp += OnKeyUp;
            _inputListener.OnKeyPressed += OnKeyPressed;

            _inputListener.OnMouseLeftClick += OnMouseLeftClick;
            _inputListener.OnMouseXChange += OnMouseXChange;
            _inputListener.OnMouseYChange += OnMouseYChange;
        }

        public void Update()
        {
            _inputListener.Update();
        }
        // Add new keys to listen for
        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            // Add key to listen for when polling 
            _inputListener.AddKey(key);

            // Add the binding to the command map 
            _keyBindings.Add(key, action);
        }
        // Add new mouse keys to listen for
        public void AddMouseBinding(eMouseInputs eMouseInput, GameAction action)
        {
            // Add the binding to the command map 
            _mouseBindings.Add(eMouseInput, action);
        }

        #region KEYBOARD EVENTS

        void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            GameAction action = _keyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.DOWN, new Vector2(1.0f));
            }
        }
        void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            GameAction action = _keyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.UP, new Vector2(1.0f));
            }
        }
        void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            GameAction action = _keyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.PRESSED, new Vector2(1.0f));
            }
        }
        #endregion
        #region MOUSE EVENTS
        void OnMouseXChange(object sender, MouseEventArgs e)
        {
            GameAction action = _mouseBindings.ContainsKey(e._eMouseInput) ? _mouseBindings[e._eMouseInput] : null;

            if (action != null)
            {
                int change = e._currentState.X - e._prevState.X;
                action(eButtonState.NONE, new Vector2(change, 0));
            }
        }
        void OnMouseYChange(object sender, MouseEventArgs e)
        {
            GameAction action = _mouseBindings.ContainsKey(e._eMouseInput) ? _mouseBindings[e._eMouseInput] : null;

            if (action != null)
            {
                int change = e._currentState.Y - e._prevState.Y;
                action(eButtonState.NONE, new Vector2(0, change));
            }
        }
        void OnMouseLeftClick(object sender, MouseEventArgs e)
        {
            GameAction action = _mouseBindings.ContainsKey(e._eMouseInput) ? _mouseBindings[e._eMouseInput] : null;

            if (action != null)
            {
                action(eButtonState.PRESSED, new Vector2(1.0f));
            }
        }
        #endregion
    }
}
