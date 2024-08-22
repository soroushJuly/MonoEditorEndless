using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using MonoEditorEndless.Editor.Layouts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Num = System.Numerics;


namespace MonoEditorEndless.Editor
{
    internal class EditorHandle
    {
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;
        private Texture2D _xnaTexture;

        private LayoutEdit _layoutEdit;
        private LayoutPlay _layoutPlay;
        private List<Asset> _assets;

        private bool _isPlaying;
        public List<Asset> GetAssets() { return _assets; }

        //ControlsAggregator _controlsAggregator;
        public EditorHandle(Microsoft.Xna.Framework.Game game, GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _imGuiRenderer = new ImGuiRenderer(game);
            _imGuiRenderer.RebuildFontAtlas();
            _graphics = graphics;

            _layoutEdit = new LayoutEdit(_imGuiRenderer, _graphics, controlsAggregator);
            _layoutPlay = new LayoutPlay(_imGuiRenderer, _graphics, controlsAggregator);

            //_controlsAggregator = controlsAggregator;
            controlsAggregator.PlayPressed += (object sender, EventArgs e) => { _isPlaying = true; };
            controlsAggregator.PlayFromStartPressed += (object sender, EventArgs e) => { _isPlaying = true; };

            controlsAggregator.PausePressed += (object sender, EventArgs e) => { _isPlaying = false; };

            _isPlaying = false;

            _assets = new List<Asset>();
            _assets.Add(new AssetTexture("play.png", false, true));
            _assets.Add(new AssetTexture("replay.png", false, true));
            _assets.Add(new AssetTexture("info.png", false, true));
            _assets.Add(new AssetTexture("pause.png", false, true));
        }

        public void LoadContent(ContentManager content)
        {
            _layoutPlay.LoadContent(content);
            _layoutEdit.LoadContent(content);
        }

        public void Draw(GameTime gameTime)
        {
            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            if (_isPlaying)
                _layoutPlay.Draw();
            else
                _layoutEdit.Draw();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();
        }


    }
}
