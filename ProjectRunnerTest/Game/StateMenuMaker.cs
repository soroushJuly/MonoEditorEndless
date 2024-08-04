﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoEditorEndless.Engine.StateManager;
using MonoEditorEndless.Engine.UI;
using System;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace MonoEditorEndless.Game
{
    internal class StateMenuMaker : State
    {
        private Texture2D _background;
        // TODO: no panel for now
        private Texture2D _panel;
        private Text _title;
        private ButtonList _buttonList;
        private SpriteFont _font;
        float scale = 1f;

        ContentManager Content;
        GraphicsDevice _graphicDevice;



        // Events generated by clicking buttons
        public event EventHandler GameStart;
        public event EventHandler HighScores;
        public event EventHandler Controls;
        public event EventHandler ExitGame;
        public StateMenuMaker(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Name = "menu-maker";
            Content = content;
            _graphicDevice = graphicsDevice;

        }
        public override void Enter(object owner)
        {
            _font = Content.Load<SpriteFont>("Content/Fonts/File");
            _background = Content.Load<Texture2D>("Content/Textures/bg");
            // Initialize the button list with button indicator and padding between buttons
            _buttonList = new ButtonList(null, 10, 50, _font, 50);
            _title = new Text("Game Title", new Vector2(10, 10), _font, Color.Gainsboro);
            LoadMainButtons();
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            // Zoom In/Out
            float step = 0.0005f;
            float minScroll = 1 / (4 * step);
            int mouseWheelValue = Mouse.GetState().ScrollWheelValue;
            if (mouseWheelValue >= minScroll)
            {
                scale = (float)Mouse.GetState().ScrollWheelValue * step;
            }
            else
            {
                scale = minScroll * step;
            }
            // Get Keys for buttons
            _buttonList.Update();
        }
        public override void Exit(object owner) { _buttonList.Clear(); }
        public override void Draw(GraphicsDevice graphicsDevice = null, SpriteBatch spriteBatch = null)
        {

            Matrix transform = Matrix.CreateScale(scale);
            transform *= Matrix.CreateTranslation(
                graphicsDevice.Viewport.Width / 2 - graphicsDevice.Viewport.Width * scale / 2,
                graphicsDevice.Viewport.Height / 2 - graphicsDevice.Viewport.Height * scale / 2,
                0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transform);
            spriteBatch.Draw(_background,
                new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
                Color.White);
            _title.Draw(spriteBatch);
            _buttonList.Draw(spriteBatch);
            spriteBatch.End();
            //base.Draw(owner, GraphicsDevice, spriteBatch);
        }
        private void LoadMainButtons()
        {
            Texture2D btnTexture = new Texture2D(_graphicDevice, 1, 1);
            btnTexture.SetData(new[] { Color.White });

            _buttonList.AddButton("Start", btnTexture, new Vector2(100, 20));
            _buttonList.AddButton("High Scores", btnTexture, new Vector2(100, 20));
            _buttonList.AddButton("Controls", btnTexture, new Vector2(100, 20));
            _buttonList.AddButton("Exit", btnTexture, new Vector2(100, 20));
            // Handle button selection
            _buttonList.ButtonClicked += this.HandleButtonSelection;
            // Play sound on button switch
            //_buttonList.ButtonSwitched += (object sender, EventArgs e) => { switchSound.Play(); };
        }
        private void HandleButtonSelection(object sender, Button button)
        {
            // Handle the button selection based on the name of the button
            switch (button.GetText())
            {
                case "Start":
                    if (GameStart != null)
                        GameStart(this, EventArgs.Empty);
                    break;
                case "High Scores":
                    if (HighScores != null)
                        HighScores(this, EventArgs.Empty);
                    break;
                case "Controls":
                    if (Controls != null)
                        Controls(this, EventArgs.Empty);
                    break;
                case "Exit":
                    if (ExitGame != null)
                        ExitGame(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
            // Play the sound effect related to the select button
            //selectSound.Play();
        }
    }
}
