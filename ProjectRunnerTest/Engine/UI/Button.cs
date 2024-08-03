using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public string GetText() { return text; }
        public Button(string text, Texture2D indicatorTexture, Vector2 position, SpriteFont font)
        {
            this.indicatorTexture = indicatorTexture;
            this.position = position;
            this.text = text;
            this.font = font;

            isHovered = false;
        }
        public void updateHovered(bool status)
        {
            isHovered = status;
        }
        public bool GetIsHovered() { return isHovered; }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isHovered && indicatorTexture != null)
            {
                spriteBatch.Draw(indicatorTexture, new Rectangle((int)position.X, (int)position.Y,
                    indicatorTexture.Width, indicatorTexture.Height), Color.White);
            }
            if (indicatorTexture != null)
            {

                spriteBatch.DrawString(font, text, new Vector2(position.X + indicatorTexture.Width + 20, position.Y), Color.White,
                    0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X + 20, position.Y), Color.White,
                    0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1.0f);
            }
        }
    }
}