using ImGuiNET;
using ProjectRunnerTest;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Num = System.Numerics;
using Forms = System.Windows.Forms;

namespace MonoEditorEndless.Editor.Components
{
    internal class ButtonBuild
    {
        private FileHandler _fileHandler;
        private string _folderPath;

        // True if the building process finished successfully
        private bool _isBuilt;

        private bool _showModal;
        // The thread which handles building task
        Thread _buildThread;
        public ButtonBuild()
        {
            _fileHandler = new FileHandler();
            _folderPath = "-";
            _showModal = false;


            //_buildThread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        }
        public void Draw()
        {
            // Build button style
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10.0f);    // Rounded corners
            ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.0f, 0.0f, 0.0f, 1f));    // Black text
            ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(1.0f, 0.84f, 0.08f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(0.8f, 0.64f, 0.08f, 1.0f));
            if (ImGui.Button("Build", new Num.Vector2(ImGui.GetContentRegionAvail().X, 40.0f)))
            {
                // Open the build process modal on click
                _showModal = true;
            }
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);
            if (!_showModal) { return; }
            else if (_showModal)
            {
                ImGui.OpenPopup("Build Details ##Modal");
            }
            // Modal of the build process
            ImGui.SetNextWindowSize(new Num.Vector2(350, 200));
            // Getting the folder for the output files
            if (ImGui.BeginPopupModal("Build Details ##Modal", ImGuiWindowFlags.AlwaysAutoResize))
            {
                // If user is in the beginning of build process
                if (!_isBuilt)
                {
                    ImGui.TextWrapped("Select the folder to create executable file:");
                    if (ImGui.Button("Select"))
                    {
                        _folderPath = _fileHandler.FolderSelector();
                    }
                    ImGui.TextWrapped("Selected Folder:");
                    ImGui.TextWrapped(_folderPath);
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10.0f);    // Rounded corners
                    ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.0f, 0.0f, 0.0f, 1f));    // Black text
                    ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(1.0f, 0.84f, 0.08f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(0.8f, 0.64f, 0.08f, 1.0f));
                    if (ImGui.Button("Build", new Num.Vector2(ImGui.GetContentRegionAvail().X, 40.0f)))
                    {
                        if (_folderPath == "" || _folderPath == "-")
                            Forms.MessageBox.Show("Please select a folder.\n\r");
                        else
                            BuildGame(_folderPath);
                    }
                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor(3);

                    // Close the popup and reset the process
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10.0f);    // Rounded corners
                    if (ImGui.Button("Close", new Num.Vector2(ImGui.GetContentRegionAvail().X, 30.0f)))
                    {
                        ImGui.CloseCurrentPopup();
                        _isBuilt = false;
                        _showModal = false;
                    }
                    ImGui.PopStyleVar();
                }
                // When build process finishes
                else
                {
                    ImGui.TextWrapped("Game built successfully at the address below:");
                    ImGui.TextWrapped(_folderPath);
                    // Needs admin permission
                    //ImGui.SameLine();
                    //if (ImGui.Button("Open file location"))
                    //{
                    //    Process.Start(_folderPath);
                    //}
                    if (ImGui.Button("Close"))
                    {
                        ImGui.CloseCurrentPopup();
                        _isBuilt = false;
                        _showModal = false;
                    }
                }

                ImGui.EndPopup();
            }
        }
        void BuildGame(string outputDirectory)
        {
            _showModal = false;
            ImGui.CloseCurrentPopup();
            ModalLoading.Instance.Start();
            // Start the building process
            _buildThread?.Join();
            _buildThread = new Thread(() =>
            {
                RunBuild();
            });
            _buildThread.Start();
        }
        void RunBuild()
        {
            // create a copy of the project file in the directory
            // This file will be used by the application during Release build
            _fileHandler.SaveXml(Application._project, "project_data.xml", _folderPath);
            // Current Directory is Debug build directory
            string CurrentDirectory = Environment.CurrentDirectory;
            // We going to root and run the project in Release mode
            CurrentDirectory = Path.Combine(CurrentDirectory, "..", "..", "..");
            var processInfo = new ProcessStartInfo("dotnet")
            {
                Arguments = $"publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained -o \"{_folderPath}\"",
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
            _isBuilt = true;
            _showModal = true;
            ModalLoading.Instance.Stop();
        }
    }
}
