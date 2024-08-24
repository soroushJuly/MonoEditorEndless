using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using ProjectRunnerTest;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Layouts
{
    internal class LayoutEditRightPanel
    {
        private GraphicsDeviceManager _graphics;
        private ControlsAggregator _controlsAggregator;

        private bool _is3DView;
        private bool _is2DView;

        static int selectedView = 0;
        static int selected2DView = 0;
        public LayoutEditRightPanel(GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _controlsAggregator = controlsAggregator;
            _graphics = graphics;

            _is3DView = true;
        }
        public void Draw()
        {
            _is2DView = !_is3DView;
            float old = ImGui.GetFont().FontSize;
            // Get the available width for content in the current window
            float windowWidth = ImGui.GetContentRegionAvail().X;
            ImGui.GetFont().FontSize *= 1.75f;
            ImGui.PushFont(ImGui.GetFont());
            ImGui.Text("Editor and Tools");
            ImGui.Separator();
            ImGui.GetFont().FontSize = old;
            ImGui.PopFont();
            ImGui.Text("Test the game");
            ImGui.Spacing();
            ImGui.Text("Run Play:");

            ImGui.SameLine();
            if (EditorHandle._playTexture != IntPtr.Zero)
            {
                if (ImGui.ImageButton("Play Game", EditorHandle._playTexture, new Num.Vector2(15, 15)))
                {
                    _controlsAggregator.RaisePlayPressed();
                }
            }
            ImGui.Text("Run Completely:");
            ImGui.SameLine();
            if (EditorHandle._playTexture != IntPtr.Zero)
            {
                if (ImGui.ImageButton("Play Complete", EditorHandle._playTexture, new Num.Vector2(15, 15)))
                {
                    _controlsAggregator.RaisePlayFromStartPressed();
                }
            }
            ImGui.Separator();
            ImGui.Text("View");
            ImGui.Spacing();
            if (ImGui.RadioButton("Free 3D", selectedView == 0))
            {
                selectedView = 0;
                Application._project._editorConfigs._selectedView = 0;
            }
            if (ImGui.RadioButton("2D View", selectedView == 1))
            {
                selectedView = 1;
                Application._project._editorConfigs._selectedView = 1;
            }
            //if (selectedView == 1)
            //{
            //    // Create a group of radio buttons
            //    if (ImGui.RadioButton("Top-down", selected2DView == 0))
            //    {
            //        selected2DView = 0;  // Set the selected option to 0
            //    }
            //    ImGui.SameLine();
            //    if (ImGui.RadioButton("Side", selected2DView == 1))
            //    {
            //        selected2DView = 1;  // Set the selected option to 1
            //    }
            //}
            ImGui.Separator();
            ImGui.Text("Settings:");
            ImGui.Spacing();
            if (ImGui.CollapsingHeader("View", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.SetNextItemWidth(windowWidth);
                // Movement speed
                ImGui.Text("Move speed:");
                ImGui.SameLine();
                if (EditorHandle._infoTexture != IntPtr.Zero)
                {
                    ImGui.Image(EditorHandle._infoTexture, new Num.Vector2(15f));
                }
                //ImGui.Image(EditorHandle._infoTexture, new Num.Vector2(15f));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("This will change the speed you can move around the map.");
                    ImGui.EndTooltip();
                }
                ImGui.SliderFloat("##Speed", ref Application._project._editorConfigs._spectateMoveSpeed, 0.1f, 20f);
                // Sensitivity
                ImGui.Text("Rotation sensetivity:");
                ImGui.SameLine();
                ImGui.Image(EditorHandle._infoTexture, new Num.Vector2(15f));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("This will change the speed you can look around with mouse.");
                    ImGui.EndTooltip();
                }
                ImGui.SliderFloat("##Sensitivity", ref Application._project._editorConfigs._spectateSensitivity, 0.01f, 2f);
                ImGui.Spacing();
                // On Screen instructions
                ImGui.Checkbox("Show instructions", ref Application._project._editorConfigs._showInstructions);
                ImGui.SameLine();
                ImGui.Image(EditorHandle._infoTexture, new Num.Vector2(15f));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Show/Hide the instruction on top left corner.");
                    ImGui.EndTooltip();
                }
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
