using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Editor.ImGuiTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Num = System.Numerics;


namespace MonoEditorEndless.Editor
{
    internal class EditorHandle
    {
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;
        private Texture2D _xnaTexture;
        private IntPtr _playTexture;
        private IntPtr _pauseTexture;

        private List<Asset> _assets;
        public List<Asset> GetAssets() { return _assets; }

        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        private float f = 0.0f;

        private bool show_test_window = false;
        private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        private byte[] _textBuffer = new byte[100];
        public EditorHandle(Microsoft.Xna.Framework.Game game, GraphicsDeviceManager graphics)
        {
            _imGuiRenderer = new ImGuiRenderer(game);
            _imGuiRenderer.RebuildFontAtlas();
            _graphics = graphics;

            _assets = new List<Asset>();
            _assets.Add(new AssetTexture("play.png", false, true));
            _assets.Add(new AssetTexture("pause.png", false, true));
        }

        public void LoadContent(ContentManager content)
        {
            //_playTexture = ;
            //_pauseTexture = ;
            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            //_xnaTexture = ImGuiRenderer.CreateTexture(GraphicsDevice, 300, 150, pixel =>
            //{
            //    var red = (pixel % 300) / 2;
            //    return new Color(red, 1, 1);
            //});

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            _playTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/play"));
            _pauseTexture = _imGuiRenderer.BindTexture(content.Load<Texture2D>("Content/Editor/Texture/pause"));
        }

        public void Draw(GameTime gameTime)
        {
            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();
        }
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
                    //BuildContent();

                }
                if (ImGui.CollapsingHeader("Game Details"))
                {
                    ImGui.Text("Hello from game setting!");
                    //ImGui.InputText("Game Title", ref _gameTitle, 100);
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
                        //_bgMusicName = string.Empty;
                        //_bgMusicName = LoadFile();

                    }
                    //if (ImGui.Button("play it!!"))
                    //{
                    //    SoundEffectInstance si = _soundList[0].CreateInstance();
                    //    si.Play();

                    //}
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
                ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

                ImGui.InputText("Text input", _textBuffer, 100);

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
                if (ImGui.ImageButton("Play", _playTexture, new Num.Vector2(30, 30)))
                {
                    //_gameHandle.Start();
                }
                ImGui.SameLine();
                if (ImGui.ImageButton("Play", _pauseTexture, new Num.Vector2(30, 30)))
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


    }
}
