using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using ProjectRunnerTest;
using System;
using System.Diagnostics;
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
        public virtual void Draw()
        {
            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            //int windowFlags =| ImGuiWindowFlags.
            // Left panel - Game Setting
            // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
            ImGui.SetNextWindowPos(new Num.Vector2(0f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(350f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Game Settings", ImGuiWindowFlags.NoTitleBar))
            {
                float old = ImGui.GetFont().Scale;
                ImGui.GetFont().Scale *= 1.25f;
                ImGui.PushFont(ImGui.GetFont());
                ImGui.Text("Game Settings");
                ImGui.GetFont().Scale = old;
                ImGui.PopFont();
                ImGui.Separator();
                // Build the Release version from the Debug mode
                if (ImGui.Button("Build"))
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
            ImGui.SetNextWindowPos(new Num.Vector2(_graphics.PreferredBackBufferWidth - 350f, ImGui.GetFrameHeight()));
            ImGui.SetNextWindowSize(new Num.Vector2(350f, _graphics.PreferredBackBufferHeight));
            if (ImGui.Begin("Editor", ImGuiWindowFlags.NoTitleBar))
            {
                float old = ImGui.GetFont().Scale;
                ImGui.GetFont().Scale *= 1.25f;
                ImGui.PushFont(ImGui.GetFont());
                ImGui.Text("Editor");
                ImGui.GetFont().Scale = old;
                ImGui.PopFont();
                ImGui.Text("Run:");
                if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(30, 30)))
                {
                    _controlsAggregator.RaisePlayPressed();
                }
                ImGui.SameLine();
                if (ImGui.ImageButton("Pause", _pauseTexture, new Num.Vector2(30, 30)))
                {
                    //_gameHandle.Start();
                }
                ImGui.Text("Play from main menu:");
                if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(30, 30)))
                {
                    _controlsAggregator.RaisePlayFromStartPressed();
                }
                ImGui.SameLine();
                if (ImGui.ImageButton("Pause", _pauseTexture, new Num.Vector2(30, 30)))
                {
                    //_gameHandle.Start();
                }
                ImGui.Separator();
                ImGui.Text("swithch view");
                ImGui.Text("Build");
                //ImGui.ArrowButton("Play", ImGuiDir.Left);
                if (ImGui.CollapsingHeader("Game Details"))
                {
                    ImGui.Text("Hello from game setting!");
                    //ImGui.InputText("Game Title", ref _gameTitle, 100);
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
                if (ImGui.BeginMenu("Edit"))
                {
                    ShowMenuEdit();
                    ImGui.EndMenu();
                }
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
            //IMGUI_DEMO_MARKER("Examples/Menu");
            ImGui.MenuItem("(demo menu)", null, false, false);
            // Create a new default project and replace current project
            if (ImGui.MenuItem("New"))
            {
                Application._project = new Project();
                Application._project.CreateDefault();
                Application.UpdateContent(Application._project.GetAllAsset());
                Application.BuildContent();
            }
            // Open dialog and replace current project with new Project class
            if (ImGui.MenuItem("Open"))
            {
                string file = _fileHandler.LoadFileFromComputerNoCopy();
                if (Path.GetExtension(file) != ".xml")
                {
                    Forms.MessageBox.Show("Please select proper file type: .xml");
                }
                Application._project = _fileHandler.LoadClassXml<Project>(Application._project, file);
                Application.UpdateContent(Application._project.GetAllAsset());
                Application.BuildContent();
            }
            // Save the current project in the recent project location
            if (ImGui.MenuItem("Save"))
            {

            }
            // Open new window before saving for getting a new name
            if (ImGui.MenuItem("Save As..")) { }
            ImGui.Separator();
            // Exit the application
            if (ImGui.MenuItem("Exit"))
            {
                Environment.Exit(0);
            }
        }
        private void ShowMenuEdit()
        {
            if (ImGui.MenuItem("Undo", "CTRL+Z")) { }
            if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) { }
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
    }
}
