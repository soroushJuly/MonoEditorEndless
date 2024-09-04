using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoEditorEndless.Engine.UI
{
    internal class Button
    {
        string text;
        // TODO: use button box to sense mouse hover over button
        Rectangle buttonBox;
        bool isHovered;
        Vector2 position;

        // The font used to display UI elements  
        SpriteFont font;
        // TODO: button texture to use on the background of button, It's tranparent only right now
        Texture2D buttonTexture;
        // Little indicator on the left side of button
        Texture2D indicatorTexture;
        // TODO: use these variables later to have fixed size buttons
        int width;
        int height;

        public event EventHandler MouseHover;
        public event EventHandler MouseHoverExit;

        public string GetText() { return text; }
        public Button(string text, Texture2D indicatorTexture, Texture2D buttonTexture, Vector2 position, SpriteFont font, Vector2 dimensions)
        {
            this.indicatorTexture = indicatorTexture;
            this.buttonTexture = buttonTexture;
            this.position = position;
            this.text = text;
            this.font = font;
            this.width = (int)dimensions.X;
            this.height = (int)dimensions.Y;
            buttonBox = new Rectangle((int)position.X, (int)position.Y, width, height);

            isHovered = false;
        }
        public void updateHovered(bool status)
        {
            isHovered = status;
        }
        public void Update(Rectangle mouseBox)
        {
            // If already hovered
            if (isHovered)
            {
                // Check when it stopped
                if (!buttonBox.Intersects(mouseBox))
                {
                    MouseHoverExit(this, EventArgs.Empty);
                }
            }
            // Check if hovering
            if (buttonBox.Intersects(mouseBox))
            {
                MouseHover(this, EventArgs.Empty);
            }

        }
        public bool GetIsHovered() { return isHovered; }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background
            spriteBatch.Draw(buttonTexture, new Rectangle((int)position.X, (int)position.Y,
                    width, height), Color.White);

            // Draw the text
            Color color = Color.White;
            if (isHovered)
            {
                color = Color.Blue;
            }
            if (isHovered && indicatorTexture != null)
            {
                spriteBatch.Draw(indicatorTexture, new Rectangle((int)position.X, (int)position.Y,
                    indicatorTexture.Width, indicatorTexture.Height), color);
            }
            if (indicatorTexture != null)
            {

                spriteBatch.DrawString(font, text, new Vector2(position.X + indicatorTexture.Width + 20, position.Y), color,
                    0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X + 20, position.Y), color,
                    0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1.0f);
            }
        }
    }
}