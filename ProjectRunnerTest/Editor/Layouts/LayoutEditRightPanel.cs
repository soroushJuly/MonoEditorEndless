using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using ProjectRunnerTest;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Layouts
{
    internal class LayoutEditRightPanel
    {
        private ImGuiRenderer _imGuiRenderer;
        private GraphicsDeviceManager _graphics;
        private ControlsAggregator _controlsAggregator;
        private IntPtr _playTexture;
        private IntPtr _pauseTexture;
        public LayoutEditRightPanel(ImGuiRenderer imGuiRenderer, GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _imGuiRenderer = imGuiRenderer;
            _controlsAggregator = controlsAggregator;
            _graphics = graphics;
        }
        public void LoadContent(ContentManager content)
        {
            _playTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/play"));
            _pauseTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/pause"));
        }
        public void Draw()
        {
            float old = ImGui.GetFont().FontSize;
            ImGui.GetFont().FontSize *= 1.75f;
            ImGui.PushFont(ImGui.GetFont());
            ImGui.Text("Editor and Tools");
            ImGui.Separator();
            ImGui.GetFont().FontSize = old;
            ImGui.PopFont();
            ImGui.Text("Run Play:");
            ImGui.SameLine();
            if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(15, 15)))
            {
                _controlsAggregator.RaisePlayPressed();
            }
            ImGui.Text("Run Completely:");
            ImGui.SameLine();
            if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(15, 15)))
            {
                _controlsAggregator.RaisePlayFromStartPressed();
            }
            ImGui.Separator();
            ImGui.Text("View");
            ImGui.Separator();
            if (ImGui.CollapsingHeader("Spectate view", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Text("Move speed:");
                ImGui.SliderFloat("Move speed", ref Application._project._editorConfigs._spectateSensitivity, 0.01f, 2f);
                ImGui.Text("Rotation sensetivity:");
            }

            // Build the Release version from the Debug mode
            float itemSpacingY = ImGui.GetStyle().ItemSpacing.Y;  // Vertical spacing between items
            float windowPaddingY = ImGui.GetStyle().WindowPadding.Y;  // Padding within the window

            ImGui.SetCursorPosY(_graphics.PreferredBackBufferHeight - 40 - windowPaddingY - 2 * itemSpacingY - ImGui.GetFrameHeight());
            ImGui.Separator();
            ImGui.Spacing();
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10.0f);    // Rounded corners
            ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.0f, 0.0f, 0.0f, 1f));    // Black text
            ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(1.0f, 0.84f, 0.08f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(0.8f, 0.64f, 0.08f, 1.0f));
            // Get the available width for content in the current window
            float windowWidth = ImGui.GetContentRegionAvail().X;
            if (ImGui.Button("Build", new Num.Vector2(windowWidth, 40.0f)))
            {
                BuildGame();
            }
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);
            ImGui.Spacing();
        }
        void BuildGame()
        {
            Thread thread = new Thread(() =>
            {
                string CurrentDirectory = Environment.CurrentDirectory;
                CurrentDirectory = Path.Combine(CurrentDirectory, "..", "..", "..");
                var processInfo = new ProcessStartInfo("dotnet")
                {
                    Arguments = "publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained",
                    WorkingDirectory = CurrentDirectory,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(processInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                    }
                }
                Application._project = new Project();
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }
    }
}
