using ImGuiNET;
using System;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Components
{
    internal class Tooltip
    {
        private bool showModal;
        private static Tooltip instance = null;
        public static Tooltip Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Tooltip();
                }
                return instance;
            }
        }
        Tooltip() { showModal = false; }
        public void Start() { showModal = true; }
        public void Draw(string content)
        {
            if (EditorHandle._infoTexture != IntPtr.Zero)
            {
                ImGui.Image(EditorHandle._infoTexture, new Num.Vector2(15f));
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text(content);
                ImGui.EndTooltip();
            }
        }
        public void Stop()
        {
            showModal = false;
            ImGui.CloseCurrentPopup();
        }
    }
}
