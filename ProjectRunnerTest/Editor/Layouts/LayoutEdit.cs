﻿using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoEditorEndless.Editor.Components;
using ProjectRunnerTest;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Forms = System.Windows.Forms;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Layouts
{
    internal class LayoutEdit
    {
        private GraphicsDeviceManager _graphics;

        private byte[] _textBuffer = new byte[100];
        private bool show_test_window = false;

        private LayoutEditRightPanel _rightPanel;

        private FileHandler _fileHandler;

        private bool _showSaveModal;

        ControlsAggregator _controlsAggregator;
        public LayoutEdit(GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _graphics = graphics;
            _controlsAggregator = controlsAggregator;
            _rightPanel = new LayoutEditRightPanel(_graphics, _controlsAggregator);

            _showSaveModal = false;

            _fileHandler = new FileHandler();
        }
        unsafe
        public void Draw()
        {
            //var io = ImGui.GetIO();
            //io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            //string patsh = Path.Combine(Routes.ROOT_DIRECTORY, "Content", "Font", "PeaberryBase.ttf");
            //ImFontPtr headerFont = io.Fonts.AddFontFromFileTTF(patsh, 60);
            // Left panel - Game Setting
            ImGui.SetNextWindowPos(new Num.Vector2(0f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(350f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Game Settings", ImGuiWindowFlags.NoTitleBar))
            {
                ImGui.Text("Game Settings");
                ImGui.Separator();
                if (ImGui.CollapsingHeader("Game Details", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Text("Title:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Type the title of your game.");
                    ImGui.InputText("##Text input", ref Application._project._gameConfigs._title, 100);
                }
                if (ImGui.CollapsingHeader("Camera", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Text("Distance from character:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Enter the distance from camera to Type the title of your game.");
                    ImGui.InputFloat("##camera_distance_input", ref Application._project._gameConfigs.distanceFromCharacter, 5f);
                    ImGui.Text("Height of camera:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change how high is the camera relative to the character.");
                    ImGui.InputFloat("##camera_height_input", ref Application._project._gameConfigs.cameraHeight, 5f);
                    ImGui.Text("Camera target:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change how far from the character the camera should look at.");
                    ImGui.InputFloat("##camera_look_distance_input", ref Application._project._gameConfigs.cameraLookDistance, 5f);
                }
                if (ImGui.CollapsingHeader("Gameplay", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.SeparatorText("Character");
                    ImGui.Text("Character starting speed:");
                    ImGui.Text("Character maximum speed:");
                    ImGui.Text("Character health:");
                    ImGui.Text("Character movements mouse sensitivity:");
                    ImGui.SeparatorText("Items");
                    ImGui.Text("Items value:");
                    ImGui.Text("How often items will appear:");
                    ImGui.Text("How often obstacles will appear:");
                    ImGui.Text("Obstacle behavioy");
                    ImGui.SeparatorText("Game Speed");
                    ImGui.Text("Game progress pace:");
                }
                if (ImGui.CollapsingHeader("Environemnt"))
                {
                    ImGui.Text("Sky");
                    ImGui.Text("Plane");
                    ImGui.SeparatorText("fdf");
                    ImGui.Text("Road Generator");
                }
                if (ImGui.CollapsingHeader("Change 3D models"))
                {
                    ImGui.Text("Road Models");
                    ImGui.Text("Character");
                    ImGui.Text("items");
                    ImGui.Text("obstacles");
                }
                if (ImGui.CollapsingHeader("Lights"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Fog"))
                {
                    ImGui.Text("fog");
                }
                if (ImGui.CollapsingHeader("HUD"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("MenuMaker"))
                {
                    _controlsAggregator.RaiseMenuMaker();
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Audio"))
                {
                    // Main Background music
                    ImGui.Text("Load background music");
                    if (ImGui.Button("Load from computer"))
                    {
                        // TODO: check the validity here when we get the file
                        // by passing an optional file desired file type
                        string path = _fileHandler.LoadFileFromComputer();
                        if (_fileHandler.CheckValidity(path, AssetAudio._allowedExtentions))
                            Application._project.AddAssetAudio(new AssetAudio(Path.GetFileName(path)));
                    }
                    //if (ImGui.Button("play it!!"))
                    //{
                    //    SoundEffectInstance si = _soundList[0].CreateInstance();
                    //    si.Play();

                    //}
                    //ImGui.Text(_bgMusicName);
                    // Menu Music

                    // Ending Music

                    ImGui.Text("Hello from camera setting!");
                }


                //ImGui.InputFloat("Scale:", ref _actorScale);
                if (ImGui.Button("Set Scale"))
                {
                    //actor.SetScale(_actorScale);
                };


                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

                ImGui.Text("Texture sample");
                //ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
                ImGui.End();
            }


            // Right panel - Editor
            ImGui.SetNextWindowPos(new Num.Vector2(_graphics.PreferredBackBufferWidth - 300f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(300f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Editor and Tools", ImGuiWindowFlags.NoTitleBar))
            {
                _rightPanel.Draw();
                ImGui.End();
            }

            // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            if (show_test_window)
            {
                ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref show_test_window);
            }

            // Create main Manu bar
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    ShowMenuFile();
                    ImGui.EndMenu();
                }
                // TODO: add Undo and Redo logic
                if (ImGui.BeginMenu("About"))
                {
                    ShowMenuAbout();
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
            // Modal that notifies users to save before leave
            // This call should be OUTSIDE of the mainmenubar definition
            if (_showSaveModal)
            {
                ImGui.OpenPopup("Save Reminder ##modal");
            }
            SaveReminder();
            // Loading modal - can be controlled from anywhere
            ModalLoading.Instance.Draw();
            float height = ImGui.GetFrameHeight();
        }
        private void ShowMenuFile()
        {
            // Create a new default project and replace current project
            if (ImGui.MenuItem("New"))
            {
                Application._project = new Project();
                Application._project.CreateDefault();
                SaveAs(false);
                Application.UpdateContent(Application._project.GetAllAsset());
                Application.BuildContent();
            }
            // Open dialog and replace current project with new Project class
            if (ImGui.MenuItem("Open"))
            {
                string file = _fileHandler.LoadFileFromComputerNoCopy(Routes.SAVED_PROJECTS);
                if (Path.GetExtension(file) != ".xml")
                {
                    Forms.MessageBox.Show("Please select proper file type: .xml");
                }
                Application._project = _fileHandler.LoadClassXml<Project>(Application._project, file);
                // Update recent project
                _fileHandler.SaveXml<string>(Path.GetFileName(file), "recent_project.xml", Routes.SAVED_PROJECTS);
                Application.UpdateContent(Application._project.GetAllAsset());
                Application.BuildContent();
            }
            // Save the current project in the recent project location
            if (ImGui.MenuItem("Save")) { Save(); }
            // Open new window before saving for getting a new name
            if (ImGui.MenuItem("Save As..")) { SaveAs(); }
            ImGui.Separator();
            // User click Exit application
            if (ImGui.MenuItem("Exit"))
            {
                // Reminding user to save before leaving the application
                _showSaveModal = true;
            }
        }
        private void ShowMenuAbout()
        {
            if (ImGui.MenuItem("Creator", "link"))
            {
                string target = "https://www.soroushjuly.com/";
                try
                {
                    Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
                }
                catch (System.ComponentModel.Win32Exception noBrowser)
                {
                    if (noBrowser.ErrorCode == -2147467259)
                        Forms.MessageBox.Show(noBrowser.Message);
                }
                catch (System.Exception other)
                {
                    Forms.MessageBox.Show(other.Message);
                }
            }
        }
        private void SaveReminder()
        {
            // Modal of the build process
            ImGui.SetNextWindowSize(new Num.Vector2(450, 100));
            if (ImGui.BeginPopupModal("Save Reminder ##modal", ref _showSaveModal))
            {
                ImGui.TextWrapped("Do you want to save changes?");
                ImGui.NewLine();
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 10.0f);     // Rounded corners

                if (ImGui.Button("Don't Save")) { Environment.Exit(0); }

                // Calculate the position for the second button
                float windowWidth = ImGui.GetWindowSize().X;
                float buttonWidth = ImGui.CalcTextSize("Save ChangesCancel").X + ImGui.GetStyle().FramePadding.X * 2 * 2 + ImGui.GetStyle().ItemSpacing.X;
                float availableWidth = ImGui.GetContentRegionAvail().X;

                ImGui.SameLine();
                ImGui.SetCursorPosX(windowWidth - buttonWidth - ImGui.GetStyle().WindowPadding.X);

                if (ImGui.Button("Cancel")) { ImGui.CloseCurrentPopup(); _showSaveModal = false; }
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, new Num.Vector4(0.0f, 0.0f, 0.0f, 1f));    // Black text
                ImGui.PushStyleColor(ImGuiCol.Button, new Num.Vector4(1.0f, 0.84f, 0.08f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Num.Vector4(0.8f, 0.64f, 0.08f, 1.0f));
                if (ImGui.Button("Save Changes")) { Save(); Environment.Exit(0); }
                ImGui.PopStyleVar();
                ImGui.PopStyleColor(3);
                ImGui.EndPopup();
            }
        }
        private void Loading()
        {
            // Modal of the build process
            ImGui.SetNextWindowSize(new Num.Vector2(450, 100));
            if (ImGui.BeginPopupModal("Loading Modal"))
            {
                ImGui.NewLine();
                ImGui.ProgressBar((float)ImGui.GetTime() * -0.4f, Num.Vector2.Zero, "Loading...");
                ImGui.NewLine();
                ImGui.EndPopup();
            }
        }
        private void Save()
        {
            string recentProjectName = null;
            recentProjectName = _fileHandler.LoadClassXml(recentProjectName, Path.Combine(Routes.SAVED_PROJECTS, "recent_project.xml"));
            if (recentProjectName == "default_project.xml")
            {
                SaveAs();
            }
            else if (_fileHandler.SaveXml<Project>(Application._project, recentProjectName, Routes.SAVED_PROJECTS))
            {
                Forms.MessageBox.Show("Project Saved");
            }
        }
        private void SaveAs(bool isNotify = true)
        {
            Forms.Form form = new Forms.Form();
            Forms.Label label = new Forms.Label();
            Forms.TextBox textBox = new Forms.TextBox();
            Forms.Button buttonOk = new Forms.Button();
            Forms.Button buttonCancel = new Forms.Button();

            form.Text = "Save As..";
            label.Text = "Project Name:";

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = Forms.DialogResult.OK;
            buttonCancel.DialogResult = Forms.DialogResult.Cancel;

            label.SetBounds(20, 20, 250, 13);
            textBox.SetBounds(20, 40, 250, 20);
            buttonOk.SetBounds(20, 80, 80, 30);
            buttonCancel.SetBounds(120, 80, 80, 30);

            label.AutoSize = true;
            form.ClientSize = new Size(300, 150);
            form.FormBorderStyle = Forms.FormBorderStyle.FixedDialog;
            form.StartPosition = Forms.FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            form.Controls.AddRange(new Forms.Control[] { label, textBox, buttonOk, buttonCancel });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            Forms.DialogResult dialogResult = form.ShowDialog();

            if (dialogResult == Forms.DialogResult.OK)
            {
                string projectName = textBox.Text;
                if (projectName == "")
                    Forms.MessageBox.Show("Please choose a name for the project");
                else if (_fileHandler.SaveXml<Project>(Application._project, projectName + ".xml", Routes.SAVED_PROJECTS))
                {
                    if (isNotify)
                        Forms.MessageBox.Show("Project Saved");
                    // Update recent project
                    _fileHandler.SaveXml<string>(projectName + ".xml", "recent_project.xml", Routes.SAVED_PROJECTS);
                }
            }
            return;

        }
    }
}
