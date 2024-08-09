﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Engine.StateManager;
using MonoEditorEndless.Engine.UI;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace MonoEditorEndless.Game
{
    internal class StateMenu : State
    {
        private Texture2D _background;
        private Texture2D _panel;
        private Text _title;
        private ButtonList _buttonList;
        private SpriteFont _font;

        ContentManager Content;
        GraphicsDevice _graphicsDevice;


        // Events generated by clicking buttons
        public event EventHandler GameStart;
        public event EventHandler HighScores;
        public event EventHandler Controls;
        public event EventHandler ExitGame;
        public StateMenu(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Name = "menu";
            Content = content;
            _graphicsDevice = graphicsDevice;
        }
        public override void Enter(object owner)
        {
            _font = Content.Load<SpriteFont>("Content/Fonts/File");
            _background = Content.Load<Texture2D>("Content/Texture/bg");
            // Initialize the button list with button indicator and padding between buttons
            _buttonList = new ButtonList(null, 10, 10, _font, 50);
            LoadMainButtons();
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            _buttonList.Update();
        }
        public override void Exit(object owner)
        {
            _buttonList.Clear();
            Content.Unload();
        }
        public override void Draw(GraphicsDevice graphicsDevice = null, SpriteBatch spriteBatch = null)
        {
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;
            var lastRasterizer = _graphicsDevice.RasterizerState;
            var lastDepthStencil = _graphicsDevice.DepthStencilState;
            var lastBlendFactor = _graphicsDevice.BlendFactor;
            var lastBlendState = _graphicsDevice.BlendState;
            var lastSamplerStates = _graphicsDevice.SamplerStates;

            float scale = .5f;
            Matrix transform = Matrix.CreateScale(scale);
            transform *= Matrix.CreateTranslation(
                graphicsDevice.Viewport.Width / 2 - graphicsDevice.Viewport.Width * scale / 2,
                graphicsDevice.Viewport.Height / 2 - graphicsDevice.Viewport.Height * scale / 2,
                0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transform);
            spriteBatch.Draw(_background,
                new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
                Color.White);
            _buttonList.Draw(spriteBatch);
            spriteBatch.End();

            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
            _graphicsDevice.RasterizerState = lastRasterizer;
            _graphicsDevice.DepthStencilState = lastDepthStencil;
            _graphicsDevice.BlendState = lastBlendState;
            _graphicsDevice.BlendFactor = lastBlendFactor;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            //base.Draw(owner, GraphicsDevice, spriteBatch);
        }
        private void LoadMainButtons()
        {
            Texture2D btnTexture = new Texture2D(_graphicsDevice, 1, 1);
            btnTexture.SetData(new[] { Color.White });

            _buttonList.AddButton("Start", btnTexture, new Vector2(100, 10));
            // Todo: add highscore later if there was time
            //_buttonList.AddButton("High Scores", btnTexture, new Vector2(100, 10));
            _buttonList.AddButton("Controls", btnTexture, new Vector2(100, 10));
            _buttonList.AddButton("Exit", btnTexture, new Vector2(100, 10));
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
                    GameStart(this, EventArgs.Empty);
                    break;
                //case "High Scores":
                //    HighScores(this, EventArgs.Empty);
                //    break;
                case "Controls":
                    Controls(this, EventArgs.Empty);
                    break;
                case "Exit":
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
