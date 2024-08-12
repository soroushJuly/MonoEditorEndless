using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Layouts
{
    internal class LayoutPlay
    {
        private ImGuiRenderer _imGuiRenderer;
        private GraphicsDeviceManager _graphics;
        private IntPtr _playTexture;
        private IntPtr _pauseTexture;
        private byte[] _textBuffer = new byte[100];

        ControlsAggregator _controlsAggregator;
        public LayoutPlay(ImGuiRenderer imGuiRenderer, GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _imGuiRenderer = imGuiRenderer;
            _graphics = graphics;
            _controlsAggregator = controlsAggregator;
        }
        public void LoadContent(ContentManager content)
        {
            _playTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/play"));
            _pauseTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/pause"));
        }
        public virtual void Draw()
        {
            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            //int windowFlags =| ImGuiWindowFlags.
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"

            // Right panel - Editor
            ImGui.SetNextWindowPos(new Num.Vector2(_graphics.PreferredBackBufferWidth - 350f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(350f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Editor", ImGuiWindowFlags.NoTitleBar))
            {
                float old = ImGui.GetFont().Scale;
                ImGui.GetFont().Scale *= 1.25f;
                ImGui.PushFont(ImGui.GetFont());
                ImGui.Text("Editor");
                ImGui.GetFont().Scale = old;
                ImGui.PopFont();
                ImGui.Text("Run:");
                if (ImGui.ImageButton("Play Again", _playTexture, new Num.Vector2(30, 30)))
                {
                    //_gameHandle.Start();
                }
                ImGui.SameLine();
                if (ImGui.ImageButton("Pause", _pauseTexture, new Num.Vector2(30, 30)))
                {
                    _controlsAggregator.RaisePausePressed();
                }
                ImGui.Separator();
                ImGui.Text("swithch view");
                ImGui.Text("Build");
                //ImGui.ArrowButton("Play", ImGuiDir.Left);
                if (ImGui.CollapsingHeader("Game Details"))
                {
                    ImGui.Text("Hello from game setting!");
                    //ImGui.InputText("Game Title", ref _gameTitle, 100);
                }
                ImGui.End();
            }
        }
        static void ShowExampleMenuFile()
        {
            //IMGUI_DEMO_MARKER("Examples/Menu");
            ImGui.MenuItem("(demo menu)", null, false, false);
            if (ImGui.MenuItem("New")) { }
            if (ImGui.MenuItem("Open", "Ctrl+O")) { }
            if (ImGui.BeginMenu("Open Recent"))
            {
                ImGui.MenuItem("fish_hat.c");
                ImGui.MenuItem("fish_hat.inl");
                ImGui.MenuItem("fish_hat.h");
                if (ImGui.BeginMenu("More.."))
                {
                    ImGui.MenuItem("Hello");
                    ImGui.MenuItem("Sailor");
                    if (ImGui.BeginMenu("Recurse.."))
                    {
                        ShowExampleMenuFile();
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            if (ImGui.MenuItem("Save", "Ctrl+S")) { }
            if (ImGui.MenuItem("Save As..")) { }

            ImGui.Separator();
            //IMGUI_DEMO_MARKER("Examples/Menu/Options");
            if (ImGui.BeginMenu("Options"))
            {
                //static bool enabled = true;
                //ImGui.MenuItem("Enabled", "", &enabled);
                //ImGui.BeginChild("child", ImVec2(0, 60), ImGuiChildFlags_Border);
                //for (int i = 0; i < 10; i++)
                //    ImGui.Text("Scrolling Text %d", i);
                //ImGui.EndChild();
                //static float f = 0.5f;
                //static int n = 0;
                //ImGui.SliderFloat("Value", &f, 0.0f, 1.0f);
                //ImGui.InputFloat("Input", &f, 0.1f);
                //ImGui.Combo("Combo", &n, "Yes\0No\0Maybe\0\0");
                ImGui.EndMenu();
            }

        }
    }
}
