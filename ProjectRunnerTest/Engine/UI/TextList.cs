using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoEditorEndless.Engine.UI
{
    // A component for selection of a button in a list
    internal class TextList
    {
        List<Text> textList;
        SpriteFont font;
        Color color;
        int offsetX;
        int offsetY;
        int count;
        int paddings;

        public event EventHandler<Button> ButtonClicked;
        public TextList(int offsetX, int offsetY, SpriteFont font, Color color, int paddings)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.font = font;
            this.color = color;
            this.paddings = paddings;

            textList = new List<Text>();
        }

        public void AddText(string text)
        {
            textList.Add(new Text(text, new Vector2(offsetX, offsetY + textList.Count * paddings), font, color));
        }
        public void Update()
        {
        }
        public void Clear()
        {
            textList.Clear();
        }
        public int GetCount() { return textList.Count; }
        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (var button in textList)
            {
                button.Draw(_spriteBatch);
            }
        }

    }
}