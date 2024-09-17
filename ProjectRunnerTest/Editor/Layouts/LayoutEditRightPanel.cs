using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoEditorEndless.Editor.Components;
using ProjectRunnerTest;
using System;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Layouts
{
    internal class LayoutEditRightPanel
    {
        private GraphicsDeviceManager _graphics;
        private ControlsAggregator _controlsAggregator;

        private ButtonBuild _buttonBuild;

        private bool _is3DView;
        private bool _is2DView;

        static int selectedView = 0;
        public LayoutEditRightPanel(GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _controlsAggregator = controlsAggregator;
            _graphics = graphics;

            _buttonBuild = new ButtonBuild();

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
            Tooltip.Instance.Draw("This will run the game from after pressing start.");
            ImGui.SameLine();
            ImGui.Text("Run Play:");
            ImGui.SameLine();
            if (EditorHandle._playTexture != IntPtr.Zero)
            {
                if (ImGui.ImageButton("Play Game", EditorHandle._playTexture, new Num.Vector2(15, 15)))
                {
                    _controlsAggregator.RaisePlayPressed();
                }
            }
            Tooltip.Instance.Draw("This will run the game from menu.");
            ImGui.SameLine();
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
                _controlsAggregator.RaiseSpectate();
                selectedView = 0;
                Application._project._editorConfigs._selectedView = 0;
            }
            if (ImGui.RadioButton("2D View", selectedView == 1))
            {
                _controlsAggregator.RaiseSpectate();
                selectedView = 1;
                Application._project._editorConfigs._selectedView = 1;
            }
            if (ImGui.RadioButton("Heads-up Display Creating Tool", selectedView == 2))
            {
                selectedView = 2;
                _controlsAggregator.RaiseHUDMaker();
                Application._project._editorConfigs._selectedView = 2;
            }
            if (ImGui.RadioButton("Menu Creating Tool", selectedView == 3))
            {
                selectedView = 3;
                Application._project._editorConfigs._selectedView = 3;
                _controlsAggregator.RaiseMenuMaker();
            }
            ImGui.Separator();
            ImGui.Text("Settings:");
            ImGui.Spacing();
            if (ImGui.CollapsingHeader("View", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.SetNextItemWidth(windowWidth);
                // Movement speed
                ImGui.Text("Move speed:");
                ImGui.SameLine();
                Tooltip.Instance.Draw("This will change the speed you can move around the map.");
                ImGui.SliderFloat("##Speed", ref Application._project._editorConfigs._spectateMoveSpeed, 0.1f, 20f);
                // Sensitivity
                ImGui.Text("Rotation sensetivity:");
                ImGui.SameLine();
                Tooltip.Instance.Draw("This will change the speed you can look around with mouse.");
                ImGui.SliderFloat("##Sensitivity", ref Application._project._editorConfigs._spectateSensitivity, 0.01f, 2f);
                ImGui.Spacing();
                // On Screen instructions
                ImGui.Checkbox("Show instructions", ref Application._project._editorConfigs._showInstructions);
                ImGui.SameLine();
                Tooltip.Instance.Draw("Show/Hide the instruction on top left corner.");
                // Collision box
                ImGui.Checkbox("Show collision box", ref Application._project._editorConfigs._showCollisionBoxes);
                ImGui.SameLine();
                Tooltip.Instance.Draw("Show/Hide the box around the models indicating their collidable area.");
            }

            // Build the Release version from the Debug mode
            float itemSpacingY = ImGui.GetStyle().ItemSpacing.Y;  // Vertical spacing between items
            float windowPaddingY = ImGui.GetStyle().WindowPadding.Y;  // Padding within the window

            ImGui.SetCursorPosY(_graphics.PreferredBackBufferHeight - 40 - windowPaddingY - 2 * itemSpacingY - ImGui.GetFrameHeight());
            ImGui.Separator();
            ImGui.Spacing();
            _buttonBuild.Draw();
            ImGui.Spacing();
        }
    }
}
