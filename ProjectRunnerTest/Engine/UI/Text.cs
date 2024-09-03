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
        // Text size
        float size;

        public Text(string text, Vector2 position, SpriteFont font, Color color, float size = 1f)
        {
            this.position = position;
            this.text = text;
            this.font = font;
            this.color = color;
            this.size = size;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X, position.Y), color,
                0.0f, Vector2.Zero, size, SpriteEffects.None, 1.0f);
        }

    }
}