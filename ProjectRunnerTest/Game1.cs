using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Forms = System.Windows.Forms;
using Num = System.Numerics;

using ImGuiNET;

using MonoEditorEndless.Engine;
using MonoEditorEndless.Engine.Path;
using MonoEditorEndless.Editor;
using MonoEditorEndless.Engine.Input;

namespace ProjectRunnerTest
{
    /// <summary>
    /// Simple FNA + ImGui example
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;

        private Texture2D _xnaTexture;
        private IntPtr _imGuiTexture;

        private bool isFreeCamera = false;

        private Engine.Camera _camera;

        private Vector2 _lastMouse;

        private float _actorScale;

        private World _world;
        private GameSession _gameSession;

        private PathManager _pathManager;
        private string _gameTitle = "Untitled";

        private float _mouseActiveTimer = 0f;

        KeyboardState _prevKeyState;

        private InputManager _inputManager;

        // Skybox
        Skybox _skybox;
        List<Texture2D> _skyboxTextureList;

        // Audio
        SoundEffect _soundEffect;
        List<SoundEffect> _soundEffectList;
        SoundEffectInstance _soundEffectInstance;
        Song _bgMusic;
        private string _bgMusicName;
        List<Song> _songList;

        MouseState curMouse;
        MouseState prevMouse;

        // Example Model
        private Actor actor;
        private Actor road;
        private Actor wall;
        private Actor corner;
        private Actor roadR;
        private Actor collectable;
        private Actor obstacle;

        private MonoEditorEndless.Engine.Plane _plane;

        private Vector3 translation = Vector3.Zero;

        // Transforming matrices
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 5000f);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferMultiSampling = true;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();

            _world = new World();
            _gameSession = new GameSession();
            _pathManager = new PathManager();
            _inputManager = new InputManager();
            //_inputManager.AddMouseBinding(eMouseInputs.X_MOVE, MoveX);
            _inputManager.AddKeyboardBinding(Keys.D, TurnRight);
            _inputManager.AddKeyboardBinding(Keys.A, TurnLeft);

            _pathManager.BlockAdded += (object sender, BlockEventArgs e) => { _world.AddActor(e.GetBlock(), true); };
            _pathManager.BlockRemoved += (object sender, BlockEventArgs e) => { _world.RemoveActor(e.GetBlock()); };

            actor = new Actor();
            actor.SetVelocity(80f);
            actor.SetForward(Vector3.UnitX);
            actor.SetRightVector(-Vector3.UnitZ);

            road = new Actor();
            wall = new Actor();
            roadR = new Actor();
            corner = new Actor();
            collectable = new Actor();

            obstacle = new Actor();

            //actor.CollisionHandler += this.CollisionHandler;
            collectable.CollisionHandler += this.CollisionHandler;
            //actor.CollisionHandler += this.CharacterCollisionHandler;

            _camera = new Engine.Camera();


            _lastMouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            _bgMusicName = "";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            actor.LoadModel(Content.Load<Model>("Content/FBX/Ship"));
            actor.SetScale(0.012f);
            actor.RotateY((-90f / 180f) * (float)Math.PI);
            road.LoadModel(Content.Load<Model>("Content/wall"));
            road.RotateY((90f / 180f) * (float)Math.PI);
            wall.LoadModel(Content.Load<Model>("Content/wall-half"));
            roadR.LoadModel(Content.Load<Model>("Content/wall"));
            wall.RotateY((180f / 180f) * (float)Math.PI);
            collectable.LoadModel(Content.Load<Model>("Content/FBX/Coin"));
            collectable.SetScale(.4f);
            collectable.SetPosition(new Vector3(4 * road.GetDimentions().Z, 0, 0));

            // Load the sound effect
            //_soundEffect = new SoundEffect();
            _soundEffect = Content.Load<SoundEffect>("Content/Audio/mario_coin_sound");
            _soundEffectInstance = _soundEffect.CreateInstance();
            _soundEffectInstance.Volume = .1f;

            obstacle.LoadModel(Content.Load<Model>("Content/rocks-small"));
            obstacle.SetScale(0.25f);
            obstacle.SetPosition(new Vector3(3 * road.GetDimentions().Z, 0, 0));

            _world.AddActor(actor, true);
            _world.AddActor(collectable, true);
            _world.AddActor(obstacle, true);

            corner.LoadModel(Content.Load<Model>("Content/wall-corner"));

            _pathManager.AddRoadBlock(road);
            _pathManager.AddWallBlock(wall);
            _pathManager.AddTurnRight(corner);
            _pathManager.Initialize(10);

            _bgMusic = Content.Load<Song>("Content/Audio/Titan");


            // Debug.WriteLine(bone.Transform);


            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            _xnaTexture = ImGuiRenderer.CreateTexture(GraphicsDevice, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });

            _skyboxTextureList = new List<Texture2D>();
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/front"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/back"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/left"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/right"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/top"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/bottom"));
            _skybox = new Skybox(GraphicsDevice, _skyboxTextureList);

            Texture2D grass = Content.Load<Texture2D>("Content/grass");
            _plane = new MonoEditorEndless.Engine.Plane(GraphicsDevice, grass, 3000, 10);

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

            //MediaPlayer.Play(_bgMusic);
            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            //actor.Draw(world, _camera.GetView(), projection);
            _pathManager.Draw(world, _camera.GetView(), projection);
            //obstacle.Draw(Matrix.CreateTranslation(Vector3.Zero), _camera.GetView(), projection);
            _world.Draw(_camera.GetView(), projection);


            _skybox.Draw(GraphicsDevice, Matrix.CreateTranslation(_camera.GetPosition()), _camera.GetView(), projection);
            _plane.Draw(GraphicsDevice, Matrix.CreateTranslation(-100 * Vector3.UnitY), _camera.GetView(), projection);
            //_skybox.Draw(GraphicsDevice, Matrix.CreateTranslation(Vector3.Zero), _camera.GetView(), projection);
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
                        Thread thread = new Thread(() =>
                        {
                            var fileContent = string.Empty;
                            var filePath = string.Empty;

                            using (Forms.OpenFileDialog openFileDialog = new Forms.OpenFileDialog())
                            {
                                openFileDialog.InitialDirectory = "c:\\";
                                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                                openFileDialog.FilterIndex = 2;
                                openFileDialog.RestoreDirectory = true;

                                if (openFileDialog.ShowDialog() == Forms.DialogResult.OK)
                                {
                                    //Get the path of specified file
                                    filePath = openFileDialog.FileName;
                                    _bgMusicName = filePath;

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
                    }
                    ImGui.Text(_bgMusicName);
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
                ImGui.Text(actor.GetPosition().ToString());
                ImGui.Text("Actors forward: ");
                ImGui.Text(actor.GetForward().ToString());

                ImGui.Text(actor.GetDimentions().ToString());

                ImGui.InputFloat("Scale:", ref _actorScale);
                if (ImGui.Button("Set Scale"))
                {
                    actor.SetScale(_actorScale);
                };

                ImGui.Text("POINTS");
                ImGui.Text(_gameSession.GetPoints().ToString());



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
                ImGui.ImageButton("Play", _imGuiTexture, new Num.Vector2(10, 10));
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
            _mouseActiveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // ------------ Editor Controls ----------- //
            // Free Camera Control
            if (isFreeCamera)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    _camera.MoveLeft(.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    _camera.MoveLeft(-.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                }
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    _camera.Rotate(Mouse.GetState().X - _lastMouse.X, Mouse.GetState().Y - _lastMouse.Y);
                }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        _camera.MoveUp(.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        _camera.MoveUp(-.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        _camera.MoveForward(.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        _camera.MoveForward(-.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                    }
                }
            }
            // Else third person camera
            else
            {
                _camera.SetPosition(actor.GetPosition() - 50 * actor.GetForward());
                _camera.LookAtTarget(actor.GetPosition() + 100 * actor.GetForward(), actor.GetForward(), 300f, 100f);
            }

            if (_prevKeyState.IsKeyDown(Keys.F) && Keyboard.GetState().IsKeyUp(Keys.F))
            {
                _pathManager.Generate();
            }

            _lastMouse.X = Mouse.GetState().X;
            _lastMouse.Y = Mouse.GetState().Y;
            _inputManager.Update();
            actor.SetVelocity(actor.GetVelocity() * _gameSession.GetGameSpeed());
            curMouse = Mouse.GetState();
            float x = (curMouse.X - prevMouse.X) / 5f;
            prevMouse = curMouse;
            if (_mouseActiveTimer > 2)
            {
                actor.SetPosition(actor.GetPosition() + -x * actor.GetRight());
            }
            _pathManager.Update(gameTime, actor);
            //actor.Update(gameTime);
            _world.Update(gameTime);
            world = Matrix.CreateTranslation(actor.GetPosition());

            _prevKeyState = Keyboard.GetState();
        }
        void CollisionHandler(object sender, EventArgs e)
        {
            Actor collectableItem = sender as Actor;
            _gameSession.AddPoint(10f);
            collectableItem.GetCollidable().SetRemoveFlag(true);
            _soundEffectInstance.Play();
        }
        void CharacterCollisionHandler(object sender, EventArgs e)
        {
            Actor character = sender as Actor;
            if (e.ToString() != "")
            {
                _gameSession.AddPoint(10f);
                character.SetVelocity(0);
            }
        }

        public void MoveX(eButtonState buttonState, Vector2 amount)
        {
            float x = amount.X / 20f;
            if (true)
            {
                actor.SetPosition(actor.GetPosition() + -x * actor.GetRight());
            }
        }
        public void TurnRight(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                actor.SmoothRotateY(-(float)Math.PI / 2f, 0.04f);
            }
        }
        public void TurnLeft(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                actor.SmoothRotateY((float)Math.PI / 2f, 0.04f);
            }
        }
    }
}