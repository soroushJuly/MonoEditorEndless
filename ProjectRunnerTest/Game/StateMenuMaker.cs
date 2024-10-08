﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoEditorEndless.Engine.StateManager;
using MonoEditorEndless.Engine.UI;
using ProjectRunnerTest;
using System;

namespace MonoEditorEndless.Game
{
    internal class StateMenuMaker : State
    {
        private Texture2D _background;
        // TODO: no panel for now
        private Texture2D _panel;
        private Text _title;
        private TextList _controls;
        private ButtonList _buttonList;
        private SpriteFont _font;
        float scale = 1f;

        ContentManager Content;
        GraphicsDevice _graphicsDevice;

        // Events generated by clicking buttons
        public event EventHandler GameStart;
        public event EventHandler HighScores;
        public event EventHandler Controls;
        public event EventHandler ExitGame;
        public StateMenuMaker(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Name = "menu-maker";
            Content = content;
            _graphicsDevice = graphicsDevice;
        }
        public override void Enter(object owner)
        {
            _font = Content.Load<SpriteFont>("Content/Font/File");
            _background = Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.mainMenuBackground);
            // Initialize the button list with button indicator and padding between buttons
            _buttonList = new ButtonList(null,
                (int)Application._project._gameConfigs.listPosition.X,
                (int)Application._project._gameConfigs.listPosition.Y,
                _font,
                (int)Application._project._gameConfigs.listPadding);
            _title = new Text(Application._project._gameConfigs._title,
                Application._project._gameConfigs.titlePosition,
                _font,
                new Color(Application._project._gameConfigs.titleColor),
                Application._project._gameConfigs.titleSize);
            _controls = new TextList((int)Application._project._gameConfigs.controlsPosition.X,
                (int)Application._project._gameConfigs.controlsPosition.Y,
                _font,
                new Color(Application._project._gameConfigs.controlsColor),
                (int)Application._project._gameConfigs.controlsPadding);
            LoadMainButtons();
            LoadControls();
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            // Zoom In/Out
            float step = 0.0005f;
            float minScroll = 1 / (3 * step);
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
        public override void Exit(object owner) { _buttonList.Clear(); Content.Unload(); }
        public override void Draw(GraphicsDevice graphicsDevice = null, SpriteBatch spriteBatch = null)
        {
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;
            var lastRasterizer = _graphicsDevice.RasterizerState;
            var lastDepthStencil = _graphicsDevice.DepthStencilState;
            var lastBlendFactor = _graphicsDevice.BlendFactor;
            var lastBlendState = _graphicsDevice.BlendState;
            var lastSamplerStates = _graphicsDevice.SamplerStates;

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
            _controls.Draw(spriteBatch);
            spriteBatch.End();

            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
            _graphicsDevice.RasterizerState = lastRasterizer;
            _graphicsDevice.DepthStencilState = lastDepthStencil;
            _graphicsDevice.BlendState = lastBlendState;
            _graphicsDevice.BlendFactor = lastBlendFactor;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
        private void LoadMainButtons()
        {
            Texture2D btnTexture = new Texture2D(_graphicsDevice, 1, 1);
            btnTexture.SetData(new[] { new Color(
                Application._project._gameConfigs.buttonColor.X,
                Application._project._gameConfigs.buttonColor.Y,
                Application._project._gameConfigs.buttonColor.Z
                ) });

            _buttonList.AddButton("Start", btnTexture, Application._project._gameConfigs.buttonSize);
            //_buttonList.AddButton("Controls", btnTexture, Application._project._gameConfigs.buttonSize);
            _buttonList.AddButton("Exit", btnTexture, Application._project._gameConfigs.buttonSize);
        }
        private void LoadControls()
        {
            _controls.AddText("Use Mouse to move left and right");
            _controls.AddText("Press A and D to turn left and right");
        }
    }
}
