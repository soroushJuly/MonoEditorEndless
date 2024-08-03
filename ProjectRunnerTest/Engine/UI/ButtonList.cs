using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using MonoEditorEndless.Engine.Input;

namespace MonoEditorEndless.Engine.UI
{
    // A component for selection of a button in a list
    internal class ButtonList
    {
        List<Button> buttonList;
        Texture2D buttonIndicator;
        SpriteFont font;
        int offsetX;
        int offsetY;
        int count;
        int paddings;

        int currentButtonIndex;

        private InputManager inputCommandManager;
        public event EventHandler<Button> ButtonClicked;
        public event EventHandler ButtonSwitched;
        public ButtonList(Texture2D buttonIndicator, int offsetX, int offsetY, SpriteFont font, int paddings)
        {
            this.buttonIndicator = buttonIndicator;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.font = font;
            this.paddings = paddings;

            buttonList = new List<Button>();
            count = 0;

            inputCommandManager = new InputManager();

            inputCommandManager.AddKeyboardBinding(Keys.Up, OnKeyUp);
            inputCommandManager.AddKeyboardBinding(Keys.Down, OnKeyDown);
            inputCommandManager.AddKeyboardBinding(Keys.Enter, OnSelect);
        }

        public void AddButton(string text)
        {
            buttonList.Add(new Button(text, buttonIndicator, new Vector2(offsetX, offsetY + buttonList.Count * paddings), font));
        }
        public void Update()
        {
            inputCommandManager?.Update();
        }
        public void Clear()
        {
            buttonList.Clear();
        }
        public int GetCount() { return buttonList.Count; }
        public void Draw(SpriteBatch _spriteBatch)
        {
            // check active (hovered) button
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (i == currentButtonIndex)
                    buttonList[i].updateHovered(true);
                else
                    buttonList[i].updateHovered(false);
            }
            foreach (var button in buttonList)
            {
                button.Draw(_spriteBatch);
            }
        }
        private void UpdateCurrentButton(int index)
        {
            currentButtonIndex += index;
            if (currentButtonIndex > buttonList.Count - 1)
            {
                currentButtonIndex = 0;
                return;
            }
            if (currentButtonIndex < 0)
            {
                currentButtonIndex = buttonList.Count - 1;
                return;
            }
        }

        #region INPUT HANDLERS
        private void OnKeyDown(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                UpdateCurrentButton(+1);
                if (ButtonSwitched != null)
                {
                    ButtonSwitched(this, EventArgs.Empty);
                }
            }
        }
        private void OnKeyUp(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                UpdateCurrentButton(-1);
                if (ButtonSwitched != null)
                {
                    ButtonSwitched(this, EventArgs.Empty);
                }
            }
        }
        private void OnSelect(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
                ButtonClicked(this, buttonList[currentButtonIndex]);
        }
        #endregion
    }
}