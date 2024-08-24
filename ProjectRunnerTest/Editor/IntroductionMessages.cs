using ImGuiNET;
using ProjectRunnerTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Num = System.Numerics;


namespace MonoEditorEndless.Editor
{
    enum IntroductionSteps
    {
        VIEW,
        RIGHT,
        LEFT,
        BUILD
    }
    internal class IntroductionMessages
    {
        private IntroductionSteps _currentStep;
        private bool showModal = true;
        private float MODAL_WIDTH = 400;
        private float MODAL_HEIGHT = 170;

        public event EventHandler IntroductionFinished;

        public void Draw()
        {
            // Set the modals size
            ImGui.SetNextWindowSize(new Num.Vector2(MODAL_WIDTH, MODAL_HEIGHT));
            // Change the position of the modal based on the step
            switch (_currentStep)
            {
                case IntroductionSteps.VIEW:
                    ImGui.SetNextWindowPos(new Num.Vector2(
                        Application._graphics.PreferredBackBufferWidth / 2 - ImGui.GetWindowWidth() / 2,
                        Application._graphics.PreferredBackBufferHeight / 2 - ImGui.GetWindowHeight() / 2));
                    break;
                case IntroductionSteps.RIGHT:
                    ImGui.SetNextWindowPos(new Num.Vector2(Application._graphics.PreferredBackBufferWidth - 2 * Statics.RIGHT_PANEL_WIDTH, 40));
                    break;
                case IntroductionSteps.LEFT:
                    ImGui.SetNextWindowPos(new Num.Vector2(Statics.LEFT_PANEL_WIDTH, 40));
                    break;
                case IntroductionSteps.BUILD:
                    ImGui.SetNextWindowPos(new Num.Vector2(
                        Application._graphics.PreferredBackBufferWidth - 2 * Statics.RIGHT_PANEL_WIDTH,
                        Application._graphics.PreferredBackBufferHeight - ImGui.GetWindowHeight() / 2));
                    break;
                default:
                    break;
            }
            if (ImGui.BeginPopupModal("Introduction ##Modal", ref showModal, ImGuiWindowFlags.NoTitleBar))
            {
                // Display the current step
                switch (_currentStep)
                {
                    case IntroductionSteps.VIEW:
                        // Changed the color of introduction text
                        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1.0f, 0.64f, 0.0f, 1.0f)); // Red color
                        ImGui.TextWrapped("Introduction");
                        ImGui.PopStyleColor();
                        ImGui.TextWrapped("1/4 Preview");
                        ImGui.TextWrapped("In the middle of the screen you can see a preview of the game environment.");
                        ImGui.TextWrapped("You can freely explore the map following instructions on top left corner.");
                        ImGui.NewLine();
                        break;
                    case IntroductionSteps.RIGHT:
                        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1.0f, 0.64f, 0.0f, 1.0f)); // Red color
                        ImGui.TextWrapped("Introduction");
                        ImGui.PopStyleColor();
                        ImGui.TextWrapped("2/4 Editor Settings");
                        ImGui.TextWrapped("Here you can change how you want to work with this application.");
                        break;
                    case IntroductionSteps.LEFT:
                        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1.0f, 0.64f, 0.0f, 1.0f)); // Red color
                        ImGui.TextWrapped("Introduction");
                        ImGui.PopStyleColor();
                        ImGui.TextWrapped("3/4 Game Settings");
                        ImGui.TextWrapped("Here you can define how you want your to be.");
                        ImGui.TextWrapped("Each section has parameters and small details helping you getting the result you want.");
                        break;
                    case IntroductionSteps.BUILD:
                        ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(1.0f, 0.64f, 0.0f, 1.0f)); // Red color
                        ImGui.TextWrapped("Introduction");
                        ImGui.PopStyleColor();
                        ImGui.TextWrapped("4/4 Publish!");
                        ImGui.TextWrapped("It is that easy!");
                        ImGui.TextWrapped("After you finished your changes Click on this button to create the production ready files.");
                        break;
                    default:
                        break;
                }

                // Navigation buttons
                if (_currentStep > 0)
                {
                    ImGui.NewLine();
                    if (ImGui.Button("Previous"))
                    {
                        _currentStep--;
                    }
                    ImGui.SameLine();
                }


                if (_currentStep < IntroductionSteps.BUILD)
                {
                    if (ImGui.Button("Next"))
                    {
                        _currentStep++;
                    }
                }
                else if (_currentStep == IntroductionSteps.BUILD)
                {
                    if (ImGui.Button("Finish"))
                    {
                        IntroductionFinished?.Invoke(this, EventArgs.Empty);
                        ImGui.CloseCurrentPopup();
                        showModal = false;
                        _currentStep = 0;
                    }
                }

                ImGui.EndPopup();
            }
        }
    }
}
