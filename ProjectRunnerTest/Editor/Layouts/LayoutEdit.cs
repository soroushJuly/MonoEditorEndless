using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using ProjectRunnerTest;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using Forms = System.Windows.Forms;
using Num = System.Numerics;

namespace MonoEditorEndless.Editor.Layouts
{
    internal class LayoutEdit
    {
        private ImGuiRenderer _imGuiRenderer;
        private GraphicsDeviceManager _graphics;
        private IntPtr _playTexture;
        private IntPtr _pauseTexture;
        private byte[] _textBuffer = new byte[100];
        private bool show_test_window = false;

        FileHandler _fileHandler;

        ControlsAggregator _controlsAggregator;
        public LayoutEdit(ImGuiRenderer imGuiRenderer, GraphicsDeviceManager graphics, ControlsAggregator controlsAggregator)
        {
            _imGuiRenderer = imGuiRenderer;
            _graphics = graphics;
            _controlsAggregator = controlsAggregator;

            _fileHandler = new FileHandler();
        }
        public void LoadContent(ContentManager content)
        {
            _playTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/play"));
            _pauseTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/pause"));
        }
        unsafe
        public virtual void Draw()
        {
            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            //string patsh = Path.Combine(Routes.ROOT_DIRECTORY, "Content", "Font", "PeaberryBase.ttf");
            //ImFontPtr headerFont = io.Fonts.AddFontFromFileTTF(patsh, 60);
            // Left panel - Game Setting
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            ImGui.SetNextWindowPos(new Num.Vector2(0f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(350f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Game Settings", ImGuiWindowFlags.NoTitleBar))
            {
                //ImGui.PushFont(headerFont);
                ImGui.Text("Game Settings");
                //ImGui.PopFont();
                ImGui.Separator();
                // Building the Content while app is open
                if (ImGui.Button("Rebuild Content!!!"))
                {
                    //BuildContent();

                }
                bool gameDetailVisible = true;
                if (ImGui.CollapsingHeader("Game Details", ref gameDetailVisible, ImGuiTreeNodeFlags.Selected))
                {
                    ImGui.InputText("Text input", ref Application._project._gameConfigs._title, 100);
                }
                if (ImGui.CollapsingHeader("Camera"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Gameplay"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Lights"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Character"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("HUD"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("MenuMaker"))
                {
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
                if (ImGui.CollapsingHeader("Environemnt"))
                {
                    ImGui.Text("Skybox");
                    ImGui.SeparatorText("fdf");
                    ImGui.Text("Sourandings");
                    ImGui.Separator();
                    ImGui.Text("Road Generator");
                    ImGui.Separator();
                    ImGui.Text("Road Models");
                    ImGui.Separator();
                    ImGui.Text("Pickups/Obstacle Models");
                    ImGui.Text("Hello from camera setting!");
                }

                //ImGui.InputFloat("Scale:", ref _actorScale);
                if (ImGui.Button("Set Scale"))
                {
                    //actor.SetScale(_actorScale);
                };

                ImGui.Text("POINTS");
                //ImGui.Text(_gameSession.GetPoints().ToString());



                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

                ImGui.Text("Texture sample");
                //ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
                ImGui.End();
            }


            // Right panel - Editor
            ImGui.SetNextWindowPos(new Num.Vector2(_graphics.PreferredBackBufferWidth - 300f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(300f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Editor", ImGuiWindowFlags.NoTitleBar))
            {
                float old = ImGui.GetFont().FontSize;
                ImGui.GetFont().FontSize *= 1.75f;
                ImGui.PushFont(ImGui.GetFont());
                ImGui.Text("Editor");
                ImGui.Separator();
                ImGui.GetFont().FontSize = old;
                ImGui.PopFont();
                ImGui.Text("Run:");
                ImGui.SameLine();
                if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(15, 15)))
                {
                    _controlsAggregator.RaisePlayPressed();
                }
                ImGui.Text("Run full game:");
                ImGui.SameLine();
                if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(15, 15)))
                {
                    _controlsAggregator.RaisePlayFromStartPressed();
                }
                ImGui.Separator();
                ImGui.Text("Build");
                // Build the Release version from the Debug mode
                if (ImGui.Button("Create excutable files"))
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
                ImGui.Separator();
                if (ImGui.CollapsingHeader("Spectate view", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Text("Move speed:");
                    ImGui.SliderFloat("Move speed", ref Application._project._editorConfigs._spectateSensitivity, 0.01f, 2f);
                    ImGui.Text("Rotation sensetivity:");

                }

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
            float height = ImGui.GetFrameHeight();
        }
        private void ShowMenuFile()
        {
            // Create a new default project and replace current project
            if (ImGui.MenuItem("New"))
            {
                Application._project = new Project();
                Application._project.CreateDefault();
                SaveAs();
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
            if (ImGui.MenuItem("Save"))
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
            // Open new window before saving for getting a new name
            if (ImGui.MenuItem("Save As.."))
            {
                SaveAs();
            }
            ImGui.Separator();
            // Exit the application
            if (ImGui.MenuItem("Exit"))
            {
                Environment.Exit(0);
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

        private void SaveAs()
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
                    Forms.MessageBox.Show("Project Saved");
                    // Update recent project
                    _fileHandler.SaveXml<string>(projectName + ".xml", "recent_project.xml", Routes.SAVED_PROJECTS);
                }
            }
            return;

        }
    }
}
