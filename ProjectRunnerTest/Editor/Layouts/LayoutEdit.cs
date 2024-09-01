using ImGuiNET;
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

        // Tracking changes in the editor
        private float _pathChangeTimer;

        // Obstacle behavior combo
        private string[] _obstacleBehaviors = { "Character loss health", "Character dies instantly" };

        private bool _isTimerActive;

        private FileHandler _fileHandler;

        private bool _showSaveModal;

        ControlsAggregator _controlsAggregator;
        public LayoutEdit(GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _graphics = graphics;
            _controlsAggregator = controlsAggregator;
            _rightPanel = new LayoutEditRightPanel(_graphics, _controlsAggregator);

            _showSaveModal = false;

            _pathChangeTimer = 0f;
            _isTimerActive = false;

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
                if (ImGui.CollapsingHeader("Camera", ImGuiTreeNodeFlags.None))
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
                if (ImGui.CollapsingHeader("Gameplay", ImGuiTreeNodeFlags.None))
                {
                    // Character
                    ImGui.SeparatorText("Character");
                    // Min speed
                    ImGui.Text("Character starting speed:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Speed that character start with in the game.");
                    ImGui.InputFloat("##character_min_speed", ref Application._project._gameConfigs.characterMinSpeed, 5f);
                    // Max speed
                    ImGui.Text("Character maximum speed:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Maximum speed that character can reach in the game.");
                    ImGui.Checkbox("Character has max speed", ref Application._project._gameConfigs.characterHasMaxSpeed);
                    if (Application._project._gameConfigs.characterHasMaxSpeed)
                    {
                        ImGui.InputFloat("##character_max_speed", ref Application._project._gameConfigs.characterMaxSpeed, 5f);
                    }
                    // Character health
                    ImGui.Text("Character health:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("How many hits or falling the character can take before game over.");
                    ImGui.InputInt("##maximum_health", ref Application._project._gameConfigs.characterHealth);
                    // Character movement sensitivity
                    ImGui.Text("Character movements mouse sensitivity:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Determines how sensitive is the character movements to the sides to mouse movements.");
                    ImGui.InputFloat("##mouse_sensitivity", ref Application._project._gameConfigs.characterMoveSensitivity, .1f);
                    // Items
                    ImGui.SeparatorText("Items");
                    ImGui.Text("Items value:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Defines how many points players will get collecting the items.");
                    ImGui.InputInt("##item_value", ref Application._project._gameConfigs.itemValue);
                    // Collectable number
                    ImGui.Text("How often collectable items will appear:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Chance that an obstacle appears on the path. From 1 to 100");
                    if (ImGui.SliderInt("##collectable_chance", ref Application._project._gameConfigs.collectableChance, 0, 100))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    // Obstacle number
                    ImGui.Text("How often obstacle items will appear:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Chance that an obstacle appears on the path. From 1 to 100");
                    if (ImGui.SliderInt("##obstacle_chance", ref Application._project._gameConfigs.obstacleChance, 0, 100))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    // Obstacle behavior
                    ImGui.Text("Obstacle behavior");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Select what do you want to happen when character hits an obstacle.");
                    // Create a combo box
                    ImGui.Combo("Select Option", ref Application._project._gameConfigs.obstacleBehavior, _obstacleBehaviors, _obstacleBehaviors.Length);
                    // Game speed
                    ImGui.SeparatorText("Game Speed");
                    ImGui.Text("Game progress pace:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("How fast the game speed increases over time.");
                    ImGui.SliderFloat("##game_speed", ref Application._project._gameConfigs.gameAcceleration, 0, 2);
                }
                if (ImGui.CollapsingHeader("Environment", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    // Sky
                    ImGui.Text("Sky");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The images in far distance surrounding the game world.");
                    // Sky Front
                    ImGui.Text("Change the front image: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.skyFront);
                    if (ImGui.Button("Browse Computer ##sky_front"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "skyFront");
                        }
                    }
                    // Sky Back
                    ImGui.Text("Change the back image: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.skyBack);
                    if (ImGui.Button("Browse Computer ##sky_back"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "skyBack");
                        }
                    }
                    // Sky Left
                    ImGui.Text("Change the left image: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.skyLeft);
                    if (ImGui.Button("Browse Computer ##sky_left"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "skyLeft");
                        }
                    }
                    // Sky right
                    ImGui.Text("Change the right image: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.skyRight);
                    if (ImGui.Button("Browse Computer ##sky_right"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "skyRight");
                        }
                    }
                    // Sky top
                    ImGui.Text("Change the top image: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.skyTop);
                    if (ImGui.Button("Browse Computer ##sky_top"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "skyTop");
                        }
                    }
                    // Sky bottom
                    ImGui.Text("Change the bottom image: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.skyBottom);
                    if (ImGui.Button("Browse Computer ##sky_bottom"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "skyBottom");
                        }
                    }
                    ImGui.Separator();
                    // Plane texture
                    ImGui.Text("Plane");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The plane surrounding the character, path, and etc. usually a big rectangle with a repetitve image.");
                    ImGui.Text("Change the plane texture: ");
                    ImGui.SameLine();
                    ImGui.Text(Application._project._gameConfigs.planeTexture);
                    if (ImGui.Button("Browse Computer ##plane_texture"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.TEXTURE);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetTexture(new AssetTexture(Path.GetFileName(filePath), true), "planeTexture");
                        }
                    }
                }
                if (ImGui.CollapsingHeader("Change 3D models", ImGuiTreeNodeFlags.None))
                {
                    ImGui.NewLine();
                    if (ImGui.Button("Search 3D models", new Num.Vector2(ImGui.GetContentRegionAvail().X, 30)))
                    {
                        string target = "https://sketchfab.com/features/free-3d-models";
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
                    ImGui.NewLine();
                    // Road blocks
                    ImGui.SeparatorText("Road Models");
                    // Road blocks - straight
                    ImGui.Text("Straight Blocks");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The parts of the road on which the player moves straight.");
                    ImGui.Text("Upload new model:");
                    if (ImGui.Button("Browse Computer ##block_straight_scale"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.MODEL);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetModel(new AssetModel(Path.GetFileName(filePath), true), "blockStraightModel");
                        }
                    }
                    ImGui.Text("Current model: " + Application._project._gameConfigs.blockStraightModel);
                    ImGui.Text("Scale:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change the size of the block.");
                    if (ImGui.InputFloat("##block_straight_scale", ref Application._project._gameConfigs.blockStraightScale, .1f))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    // Road blocks - Turn
                    ImGui.Separator();
                    ImGui.Text("Turn Blocks");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The parts of the road on which the player turns to a new direction.");
                    ImGui.Text("Upload new model:");
                    if (ImGui.Button("Browse Computer##block_turn_scale"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.MODEL);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetModel(new AssetModel(Path.GetFileName(filePath), true), "blockTurnModel");
                        }
                    }
                    ImGui.Text("Current model: " + Application._project._gameConfigs.blockTurnModel);
                    ImGui.Text("Scale:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change the size of the block.");
                    if (ImGui.InputFloat("##block_turn_scale", ref Application._project._gameConfigs.blockTurnScale, .1f))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    // Character
                    ImGui.SeparatorText("Character Model");
                    ImGui.Text("Upload new model:");
                    if (ImGui.Button("Browse Computer##character_scale"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.MODEL);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetModel(new AssetModel(Path.GetFileName(filePath), true), "characterModel");
                        }
                    }
                    ImGui.Text("Current model: " + Application._project._gameConfigs.characterModel);
                    ImGui.Text("Character scale:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change the size of the character.");
                    if (ImGui.InputFloat("##character_scale", ref Application._project._gameConfigs.characterScale, .1f))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    ImGui.Text("Rotation:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Rotate the character horizontally.");
                    if (ImGui.SliderFloat("##character_rotate_y", ref Application._project._gameConfigs.characterRotateY, -180f, 180f))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    // Collectable
                    ImGui.SeparatorText("Collectable Model");
                    ImGui.Text("Upload new model for collectable:");
                    if (ImGui.Button("Browse Computer##collectable_scale"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.MODEL);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetModel(new AssetModel(Path.GetFileName(filePath), true), "collectableModel");
                        }
                    }
                    ImGui.Text("Current model: " + Application._project._gameConfigs.collectableModel);
                    ImGui.Text("Collectable scale:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change the size of the collectable.");
                    if (ImGui.InputFloat("##collectable_scale", ref Application._project._gameConfigs.collectableScale, .1f))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                    // Obstacle
                    ImGui.SeparatorText("Obstacle Model");
                    ImGui.Text("Upload new model for obstacle:");
                    if (ImGui.Button("Browse Computer##obstacle_scale"))
                    {
                        string filePath = "";
                        filePath = _fileHandler.LoadFileFromComputer(AssetType.MODEL);
                        if (filePath != "")
                        {
                            ModalLoading.Instance.Start();
                            Application._project.AddAssetModel(new AssetModel(Path.GetFileName(filePath), true), "obstacleModel");
                        }
                    }
                    ImGui.Text("Current model: " + Application._project._gameConfigs.obstacleModel);
                    ImGui.Text("Obstacle scale:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("Change the size of the obstacle model.");
                    if (ImGui.InputFloat("##obstacle_scale", ref Application._project._gameConfigs.obstacleScale, .1f))
                    {
                        _isTimerActive = true;
                        _pathChangeTimer = 1f;
                    }
                }
                if (ImGui.CollapsingHeader("Lights", ImGuiTreeNodeFlags.None))
                {
                    ImGui.SeparatorText("Scene light");
                    ImGui.Text("Scene light diffuse color:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("General illumination of a scene and gives objects their basic color under the light.");
                    ImGui.ColorEdit3("##Scene_Diffuse_color", ref Application._project._gameConfigs.sunDiffuseColor);

                    ImGui.Text("Scene light specular color:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The bright highlight seen on shiny surfaces, where light is reflected directly towards the viewer. Usually the color of the light source itself");
                    ImGui.ColorEdit3("##Scene_Diffuse_color", ref Application._project._gameConfigs.sunSpecularColor);

                    ImGui.Text("Scene light source direction:");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The direction in which light source emits light");
                    ImGui.SliderFloat("Down-Up", ref Application._project._gameConfigs.sunDirection.Y, -1, 1);
                    ImGui.SliderFloat("backward-forward", ref Application._project._gameConfigs.sunDirection.X, -1, 1);
                    ImGui.SliderFloat("Left-Right", ref Application._project._gameConfigs.sunDirection.Z, -1, 1);
                }
                if (ImGui.CollapsingHeader("Fog", ImGuiTreeNodeFlags.None))
                {
                    ImGui.SeparatorText("Fog Effect");
                    ImGui.Checkbox("Enable Fog", ref Application._project._gameConfigs.fogEnable);

                    ImGui.Text("Fog color");
                    ImGui.ColorEdit3("##distance", ref Application._project._gameConfigs.fogColor);

                    ImGui.Text("Fog starting distance");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The distance from camera at which fog start appearing");
                    ImGui.InputFloat("##fog_start_distance", ref Application._project._gameConfigs.fogStartDistance, 10f);

                    ImGui.Text("Fog ending distance");
                    ImGui.SameLine();
                    Tooltip.Instance.Draw("The distance from camera at which fog disappears");
                    ImGui.InputFloat("##fog_end_distance", ref Application._project._gameConfigs.fogEndDistance, 10f);
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
                        //string path = _fileHandler.LoadFileFromComputer();
                        //if (_fileHandler.CheckValidity(path, AssetAudio._allowedExtentions))
                        //    Application._project.AddAssetAudio(new AssetAudio(Path.GetFileName(path)));
                    }
                    //if (ImGui.Button("play it!!"))
                    //{
                    //    SoundEffectInstance si = _soundList[0].CreateInstance();
                    //    si.Play();

                    //}

                    // Ending Music

                    ImGui.Text("Hello from camera setting!");
                }


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
        public void Update(GameTime gameTime)
        {
            // Refresh the path if the obstacle or collectable chances changed
            if (_isTimerActive)
                _pathChangeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_pathChangeTimer < 0f)
            {
                _isTimerActive = false;
                _pathChangeTimer = 0;
                _controlsAggregator.RaiseRefreshSpectate();
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
