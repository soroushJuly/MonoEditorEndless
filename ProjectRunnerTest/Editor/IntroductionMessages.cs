using ImGuiNET;
using System;
using System.Collections.Generic;
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
        private int currentStep = 0;
        private IntroductionSteps _currentStep;
        private bool showModal = true;

        public event EventHandler IntroductionFinished;

        public void Draw()
        {
            //if (ImGui.Button("Open Modal"))
            //{
            //    // Show the modal when the button is clicked
            //    ImGui.OpenPopup("IntroductionModal");
            //    showModal = true;
            //}
            // Modal window
            //ImGui.SetNextWindowSize(new Num.Vector2(10f));
            switch (_currentStep)
            {
                case IntroductionSteps.VIEW:
                    ImGui.SetNextWindowPos(new Num.Vector2(Statics.RIGHT_PANEL_WIDTH + currentStep * 10));
                    break;
                case IntroductionSteps.RIGHT:
                    ImGui.SetNextWindowPos(new Num.Vector2(Statics.RIGHT_PANEL_WIDTH + currentStep * 10));
                    break;
                case IntroductionSteps.LEFT:
                    ImGui.SetNextWindowPos(new Num.Vector2(Statics.RIGHT_PANEL_WIDTH + currentStep * 10));
                    break;
                case IntroductionSteps.BUILD:
                    ImGui.SetNextWindowPos(new Num.Vector2(Statics.RIGHT_PANEL_WIDTH + currentStep * 10));
                    break;
                default:
                    break;
            }
            // Step 1
            if (ImGui.BeginPopupModal("Introduction ##Modal", ref showModal))
            {
                //ImGui.Begin("Step-by-Step Instructions");

                // Display the current step
                switch (_currentStep)
                {
                    case IntroductionSteps.VIEW:
                        ImGui.Text("Preview ");
                        ImGui.Text("Make sure everything is set up properly.");

                        break;
                    case IntroductionSteps.RIGHT:
                        ImGui.Text("You can change everything related to the editor.");
                        ImGui.Text("Load the configurations and resources.");
                        break;
                    case IntroductionSteps.LEFT:
                        ImGui.Text("You can change everything you want in the game here");
                        ImGui.Text("Run the application's main functionality.");
                        break;
                    case IntroductionSteps.BUILD:
                        ImGui.Text("After you finished your changes Click on this button to create the production ready files.");
                        break;
                    default:
                        ImGui.Text("All steps completed!");
                        break;
                }

                // Navigation buttons
                if (currentStep > 0)
                {
                    if (ImGui.Button("Previous"))
                    {
                        currentStep--;
                    }
                }

                ImGui.SameLine();

                if (currentStep < 3)
                {
                    if (ImGui.Button("Next"))
                    {
                        currentStep++;
                    }
                }
                else if (currentStep == 3)
                {
                    if (ImGui.Button("Finish"))
                    {
                        IntroductionFinished?.Invoke(this, EventArgs.Empty);
                        currentStep = 0;
                    }
                }

                ImGui.EndPopup();
            }
        }
    }
}
