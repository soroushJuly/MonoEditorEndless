using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoEditorEndless.Engine.Input;
using System;
using System.Collections.Generic;

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

        public void AddButton(string text, Texture2D btnTexture, Vector2 dimensions)
        {
            Button button = new Button(text, buttonIndicator, btnTexture, new Vector2(offsetX, offsetY + buttonList.Count * paddings), font, dimensions);
            button.MouseHover += OnMouseHover;
            buttonList.Add(button);

        }
        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Rectangle mouseBox = new Rectangle(mouse.X, mouse.Y, 10, 10);
            foreach (Button btn in buttonList)
                btn.Update(mouseBox);
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
        private void OnMouseHover(object sender, EventArgs e)
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (sender == buttonList[i])
                {
                    currentButtonIndex = i;
                    buttonList[i].updateHovered(true);
                }
                else
                    buttonList[i].updateHovered(false);
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