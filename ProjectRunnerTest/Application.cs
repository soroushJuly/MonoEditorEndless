using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoEditorEndless;
using MonoEditorEndless.Editor;
using MonoEditorEndless.Editor.ImGuiTools;
using MonoEditorEndless.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using Forms = System.Windows.Forms;
using Num = System.Numerics;

namespace ProjectRunnerTest
{
    /// <summary>
    /// Simple FNA + ImGui example
    /// </summary>
    public class Application : Game
    {
        private GameHandle _gameHandle;
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;

        private Texture2D _xnaTexture;
        private IntPtr _imGuiTexture;

        private bool isFreeCamera = false;

        private string _gameTitle = "Untitled";

        private string _bgMusicName;
        private string _platform = "win-x64";

        private List<Asset> _assetList;

        private bool _isDebug = false;

        private List<Song> _songList;
        private List<SoundEffect> _soundList;

        public Application()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferMultiSampling = true;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // If in debug Mode Change the flag
            // Debug mode is editor mode
#if DEBUG
            _isDebug = true;
#endif

            // Get the TargetFramework attribute
            var targetFramework = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                .FirstOrDefault() as TargetFrameworkAttribute;

            if (targetFramework != null)
            {
                Debug.WriteLine($"Running on Target Framework: {targetFramework.FrameworkName}");
            }
            else
            {
                Debug.WriteLine("Target Framework attribute not found.");
            }
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();


            _songList = new List<Song>();
            _soundList = new List<SoundEffect>();
            // List of assets uploaded and used in the game
            _assetList = new List<Asset>();
            // Preoccupy the asset list with the current assets
            _assetList.Add(new AssetAudio("mario_coin_sound.mp3"));
            _assetList.Add(new AssetAudio("Titan.mp3"));
            _assetList.Add(new AssetModel("bridge-straight.fbx"));
            _assetList.Add(new AssetModel("Coin.fbx"));
            _assetList.Add(new AssetModel("Ship.fbx"));
            _assetList.Add(new AssetModel("rocks-small.fbx"));
            _assetList.Add(new AssetModel("wall-corner.fbx"));
            _assetList.Add(new AssetModel("wall-half.fbx"));
            _assetList.Add(new AssetModel("wall.fbx"));
            _assetList.Add(new AssetTexture("bg.jpg"));
            _assetList.Add(new AssetTexture("top.bmp"));
            _assetList.Add(new AssetTexture("right.bmp"));
            _assetList.Add(new AssetTexture("left.bmp"));
            _assetList.Add(new AssetTexture("front.bmp"));
            _assetList.Add(new AssetTexture("bottom.bmp"));
            _assetList.Add(new AssetTexture("back.bmp"));
            _assetList.Add(new AssetTexture("colormap.png"));
            _assetList.Add(new AssetTexture("Coin2_BaseColor.jpg"));
            _assetList.Add(new AssetTexture("ShipDiffuse.tga"));
            _assetList.Add(new AssetTexture("ShipDiffuse_0.tga"));
            _assetList.Add(new AssetTexture("heart.png"));
            _assetList.Add(new AssetTexture("grass.jpg"));
            _assetList.Add(new AssetFont("PeaberryBase.woff"));

            // Create
            foreach (Asset asset in _assetList)
            {
                File.AppendAllText(Routes.CONTENT_DIRECTORY, asset.GetContentText());
            }

            _bgMusicName = "";

            BuildContent();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            _xnaTexture = ImGuiRenderer.CreateTexture(GraphicsDevice, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

           

            _gameHandle = new GameHandle(Content, GraphicsDevice);


            //MediaPlayer.Play(_bgMusic);
            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            _gameHandle.Draw(GraphicsDevice);

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        private float f = 0.0f;

        private bool show_test_window = false;
        private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        private byte[] _textBuffer = new byte[100];

        protected virtual void ImGuiLayout()
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
                    });
                    thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                    thread.Start();
                    thread.Join(); //Wait for the thread to end
                }
                // Building the Content while app is open
                if (ImGui.Button("Rebuild Content!!!"))
                {
                    BuildContent();

                }
                if (ImGui.CollapsingHeader("Game Details"))
                {
                    ImGui.Text("Hello from game setting!");
                    ImGui.InputText("Game Title", ref _gameTitle, 100);
                    ImGui.Text("Game Icon:");
                }
                if (ImGui.CollapsingHeader("Camera"))
                {
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Gameplay"))
                {
                    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                    ImGui.ColorEdit3("clear color", ref clear_color);
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Lights"))
                {
                    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                    ImGui.ColorEdit3("clear color", ref clear_color);
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Character"))
                {
                    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                    ImGui.ColorEdit3("clear color", ref clear_color);
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("HUD"))
                {
                    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                    ImGui.ColorEdit3("clear color", ref clear_color);
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("MenuMaker"))
                {
                    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                    ImGui.ColorEdit3("clear color", ref clear_color);
                    ImGui.Text("Hello from camera setting!");
                }
                if (ImGui.CollapsingHeader("Audio"))
                {
                    // Main Background music
                    ImGui.Text("Load background music");
                    if (ImGui.Button("Load from computer"))
                    {
                        _bgMusicName = string.Empty;
                        _bgMusicName = LoadFile();

                    }
                    if (ImGui.Button("play it!!"))
                    {
                        SoundEffectInstance si = _soundList[0].CreateInstance();
                        si.Play();

                    }
                    //ImGui.Text(_bgMusicName);
                    // Menu Music

                    // Ending Music

                    ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
                    ImGui.ColorEdit3("clear color", ref clear_color);
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
                    ImGui.ColorEdit3("clear color", ref clear_color);
                    ImGui.Text("Hello from camera setting!");
                }
                //ImGui.Text(actor.GetPosition().ToString());
                ImGui.Text("Actors forward: ");
                //ImGui.Text(actor.GetForward().ToString());
                ImGui.Text("Actors Speed: ");
                //ImGui.Text(actor.GetVelocity().ToString());

                //ImGui.Text(actor.GetDimentions().ToString());

                //ImGui.InputFloat("Scale:", ref _actorScale);
                if (ImGui.Button("Set Scale"))
                {
                    //actor.SetScale(_actorScale);
                };

                ImGui.Text("POINTS");
                //ImGui.Text(_gameSession.GetPoints().ToString());



                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                if (ImGui.Button("Switch Camera")) isFreeCamera = !isFreeCamera;
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

                ImGui.InputText("Text input", _textBuffer, 100);

                ImGui.Text("Texture sample");
                ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
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
                if (ImGui.ImageButton("Play", _imGuiTexture, new Num.Vector2(30, 10)))
                {
                    _gameHandle.Start();
                }
                ImGui.Separator();
                ImGui.Text("swithch view");
                ImGui.Text("Build");
                //ImGui.ArrowButton("Play", ImGuiDir.Left);
                if (ImGui.CollapsingHeader("Game Details"))
                {
                    ImGui.Text("Hello from game setting!");
                    ImGui.InputText("Game Title", ref _gameTitle, 100);
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
                    ShowExampleMenuFile();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z")) { }
                    if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) { }  // Disabled item
                    ImGui.Separator();
                    if (ImGui.MenuItem("Cut", "CTRL+X")) { }
                    if (ImGui.MenuItem("Copy", "CTRL+C")) { }
                    if (ImGui.MenuItem("Paste", "CTRL+V")) { }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
            float height = ImGui.GetFrameHeight();
        }
        static void ShowExampleMenuFile()
        {
            //IMGUI_DEMO_MARKER("Examples/Menu");
            ImGui.MenuItem("(demo menu)", null, false, false);
            if (ImGui.MenuItem("New")) { }
            if (ImGui.MenuItem("Open", "Ctrl+O")) { }
            if (ImGui.BeginMenu("Open Recent"))
            {
                ImGui.MenuItem("fish_hat.c");
                ImGui.MenuItem("fish_hat.inl");
                ImGui.MenuItem("fish_hat.h");
                if (ImGui.BeginMenu("More.."))
                {
                    ImGui.MenuItem("Hello");
                    ImGui.MenuItem("Sailor");
                    if (ImGui.BeginMenu("Recurse.."))
                    {
                        ShowExampleMenuFile();
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            if (ImGui.MenuItem("Save", "Ctrl+S")) { }
            if (ImGui.MenuItem("Save As..")) { }

            ImGui.Separator();
            //IMGUI_DEMO_MARKER("Examples/Menu/Options");
            if (ImGui.BeginMenu("Options"))
            {
                //static bool enabled = true;
                //ImGui.MenuItem("Enabled", "", &enabled);
                //ImGui.BeginChild("child", ImVec2(0, 60), ImGuiChildFlags_Border);
                //for (int i = 0; i < 10; i++)
                //    ImGui.Text("Scrolling Text %d", i);
                //ImGui.EndChild();
                //static float f = 0.5f;
                //static int n = 0;
                //ImGui.SliderFloat("Value", &f, 0.0f, 1.0f);
                //ImGui.InputFloat("Input", &f, 0.1f);
                //ImGui.Combo("Combo", &n, "Yes\0No\0Maybe\0\0");
                ImGui.EndMenu();
            }

        }
        protected override void Update(GameTime gameTime)
        {
            _gameHandle.Update(gameTime);
        }
        private string LoadFile()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            var newPath = string.Empty;
            SoundEffect soundEffect = null;
            Thread thread = new Thread(() =>
            {

                using (Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    // Opens the Dialog 
                    if (openFileDialog.ShowDialog() == Forms.DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        _bgMusicName = filePath;

                        // Copying logic
                        //string programDataDir =
                        //Environment.GetFolderPath(Environment.CurrentDirectory);\

                        string[] paths = { Environment.CurrentDirectory, "Content", "Audio", Path.GetFileName(filePath) };

                        newPath = Path.Combine(paths);


                        //string newPath = @"C:\Users\";
                        //File.SetAttributes(newPath, FileAttributes.Normal);
                        try
                        {
                            // Check if the source file exists
                            if (File.Exists(filePath))
                            {
                                // Copy the source file to the destination file
                                File.Copy(filePath, newPath, true);

                                Console.WriteLine("File copied successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Source file does not exist.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions that may occur
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                    }

                }


                //Forms.MessageBox.Show(fileContent, "File Content at path: " + filePath, Forms.MessageBoxButtons.OK);
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
            soundEffect = SoundEffect.FromFile(newPath);

            _soundList.Add(soundEffect);
            return filePath;
        }
        private void BuildContent()
        {
            string contentProjectPath = Routes.CONTENT_DIRECTORY; // Path to content project
            Thread thread = new Thread(() =>
            {
                string CurrentDirectory = Environment.CurrentDirectory;
                string buildMode = _isDebug ? "Debug" : "Release";
                string platform = _isDebug ? "" : "/win-x64";
                //System.Runtime.
                CurrentDirectory = Path.Combine(CurrentDirectory, "..", "..", "..");
                var processInfo = new ProcessStartInfo("dotnet")
                {
                    //Arguments = "",
                    Arguments = $"mgcb /build /@:{contentProjectPath} /platform:DesktopGL /outputDir:bin/{buildMode}/{Routes.FRAMEWORK_TARGET} /intermediateDir:obj /quiet",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    //CreateNoWindow = true
                    //WorkingDirectory = CurrentDirectory,
                    //CreateNoWindow = true
                };
                using (var process = Process.Start(processInfo))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine(result);
                    }
                }
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end
        }
    }
}