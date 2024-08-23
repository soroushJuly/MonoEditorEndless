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
        private Texture2D _replayTexture;
        private IntPtr _replayTextureGui;
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
            _replayTexture = content.Load<Texture2D>("Content/Editor/Texture/replay");
            _replayTextureGui = _imGuiRenderer.BindTexture(_replayTexture);
            _pauseTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/pause"));
        }
        public void Unload()
        {
            _replayTextureGui = IntPtr.Zero;
            _pauseTexture = IntPtr.Zero;
        }
        public virtual void Draw()
        {
            // In game state in Debug mode
            float itemSpacingY = ImGui.GetStyle().ItemSpacing.Y;  // Vertical spacing between items
            float itemSpacingX = ImGui.GetStyle().ItemSpacing.X;  // Horizontal spacing between items
            float windowPaddingY = ImGui.GetStyle().WindowPadding.Y;  // Padding within the window
            float windowPaddingX = ImGui.GetStyle().WindowPadding.X;  // Padding within the window
            float width = 2 * 30 + 3 * itemSpacingX + 2 * windowPaddingX;
            ImGui.SetNextWindowPos(new Num.Vector2(_graphics.PreferredBackBufferWidth - width - ImGui.GetFrameHeight(), ImGui.GetFrameHeight()));
            //Debug.WriteLine("pause:");
            //Debug.WriteLine(_pauseTexture);
            //Debug.WriteLine("Play Again:");
            //Debug.WriteLine(_replayTextureGui);
            ImGui.SetNextWindowSize(new Num.Vector2(width, 30 + itemSpacingY * 2 + windowPaddingY * 2));
            if (ImGui.Begin("Play", ImGuiWindowFlags.NoTitleBar))
            {
                if (ImGui.ImageButton("Pause", _pauseTexture, new Num.Vector2(30, 30)))
                {
                    _controlsAggregator.RaisePausePressed();
                }
                ImGui.SameLine();
                if (ImGui.ImageButton("Play Again", _replayTextureGui, new Num.Vector2(30, 30)))
                {
                    _controlsAggregator.RaiseLastEvent();
                    //_gameHandle.Start();
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
