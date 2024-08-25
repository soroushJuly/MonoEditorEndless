using ImGuiNET;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Components
{
    public class ModalLoading
    {
        private bool showModal;
        private static ModalLoading instance = null;
        public static ModalLoading Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModalLoading();
                }
                return instance;
            }
        }
        ModalLoading() { showModal = false; }
        public void Start() { showModal = true; }
        public void Draw()
        {
            if (!showModal)
            {
                return;
            }
            ImGui.OpenPopup("Loading ##modal");
            // Modal of the build process
            ImGui.SetNextWindowSize(new Num.Vector2(450, ImGui.GetStyle().WindowPadding.Y * 2 + 40f));
            if (ImGui.BeginPopupModal("Loading ##modal", ref showModal, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize))
            {
                ImGui.ProgressBar((float)ImGui.GetTime() * -0.4f, new Num.Vector2(ImGui.GetWindowWidth() - 2 * ImGui.GetStyle().WindowPadding.X, 40f), "Loading...");
                ImGui.EndPopup();
            }
        }
        public void Stop()
        {
            showModal = false;
            ImGui.CloseCurrentPopup();
        }
    }
}
