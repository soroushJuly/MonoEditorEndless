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
using System.Reflection.Metadata;
using System.Threading;
using Num = System.Numerics;


namespace MonoEditorEndless.Editor
{
    internal class EditorHandle
    {
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;
        private Texture2D _xnaTexture;
        private ContentManager _content;

        private LayoutEdit _layoutEdit;
        private LayoutPlay _layoutPlay;
        private List<Asset> _assets;

        private bool _isPlaying;
        private bool _lastState;

        // Icons used in the editor
        public static IntPtr _playTexture;
        public static IntPtr _infoTexture;
        public static IntPtr _replayTexture;
        public static IntPtr _pauseTexture;

        public List<Asset> GetAssets() { return _assets; }

        //ControlsAggregator _controlsAggregator;
        public EditorHandle(Microsoft.Xna.Framework.Game game, GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _imGuiRenderer = new ImGuiRenderer(game);
            _imGuiRenderer.RebuildFontAtlas();
            _graphics = graphics;

            _layoutEdit = new LayoutEdit(_graphics, controlsAggregator);
            _layoutPlay = new LayoutPlay(_graphics, controlsAggregator);

            //_controlsAggregator = controlsAggregator;
            controlsAggregator.PlayPressed += (object sender, EventArgs e) => { _isPlaying = true; };
            controlsAggregator.PlayFromStartPressed += (object sender, EventArgs e) => { _isPlaying = true; };

            controlsAggregator.PausePressed += (object sender, EventArgs e) => { _isPlaying = false; };

            _isPlaying = false;
            _lastState = _isPlaying;

            _assets = new List<Asset>();
            _assets.Add(new AssetTexture("play.png", false, true));
            _assets.Add(new AssetTexture("replay.png", false, true));
            _assets.Add(new AssetTexture("info.png", false, true));
            _assets.Add(new AssetTexture("pause.png", false, true));
        }

        public void LoadContent(ContentManager content)
        {
            _content = content;
            BindTextures();

        }
        private void BindTextures()
        {
            _playTexture = _imGuiRenderer.BindTexture(_content.Load<Texture2D>("Content/Editor/Texture/play"));
            _infoTexture = _imGuiRenderer.BindTexture(_content.Load<Texture2D>("Content/Editor/Texture/info"));
            _pauseTexture = _imGuiRenderer.BindTexture(_content.Load<Texture2D>("Content/Editor/Texture/pause"));
            _replayTexture = _imGuiRenderer.BindTexture(_content.Load<Texture2D>("Content/Editor/Texture/replay"));
        }
        public void Update(GameTime gameTime)
        {
            if (_isPlaying && (_lastState != _isPlaying))
            {
                // It was editor switched to playing
            }
            else if (!_isPlaying && (_lastState != _isPlaying))
            {
                // It was playing switched to editor
            }
            _lastState = _isPlaying;
        }
        public void Draw(GameTime gameTime)
        {
            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // TODO: find a better fix than binding everytime
            BindTextures();
            // Draw our UI
            if (_isPlaying)
            {
                _layoutPlay.Draw();
            }
            else if (!_isPlaying)
            {
                _layoutEdit.Draw();
            }
            ImGui.End();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();
        }
    }
}
