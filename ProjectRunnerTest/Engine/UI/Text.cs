using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoEditorEndless.Engine.UI
{
    internal class Text
    {
        private string text;
        Vector2 position;

        // The font used to display UI elements  
        SpriteFont font;
        // Text Color
        Color color;

        public Text(string text, Vector2 position, SpriteFont font, Color color)
        {
            this.position = position;
            this.text = text;
            this.font = font;
            this.color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y), color,
                0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1.0f);
        }

    }
}