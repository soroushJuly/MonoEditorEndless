using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Num = System.Numerics;

using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

using MonoEditorEndless.Engine;

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

        private bool isFreeCamera = true;

        private Engine.Camera _camera;

        private Vector2 _lastMouse;

        private string _gameTitle = "Untitled";

        // Example Model
        private Model model;
        private Actor actor;

        private Vector3 translation = Vector3.Zero;

        // Transforming matrices
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 10, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 1000f);

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

            actor = new Actor();
            actor.SetVelocity(2f);
            actor.SetForward(Vector3.UnitX);
            _camera = new Engine.Camera();

            _lastMouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Texture loading example
            model = Content.Load<Model>("Content/flag-wide");
            actor.LoadModel(model);

            foreach (ModelBone bone in model.Bones)
            {
                Debug.WriteLine(bone.Name);
                Debug.WriteLine(bone.ModelTransform);
                Debug.WriteLine(bone.Transform);
            }


            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            _xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            _imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            DrawModel(model, world,
                _camera.GetView(), projection);

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGuiLayout();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.TextureEnabled = true;
                    effect.EnableDefaultLighting();
                    //effect.LightingEnabled = true; // turn on the lighting subsystem.

                    //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0, 0); // a red light
                    //effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis
                    //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights

                    //effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    //effect.EmissiveColor = new Vector3(1, 0, 0);
                }

                mesh.Draw();
            }
        }


        // Direct port of the example at https://github.com/ocornut/imgui/blob/master/examples/sdl_opengl2_example/main.cpp
        private float f = 0.0f;

        private bool show_test_window = false;
        private bool show_another_window = false;
        private Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        private byte[] _textBuffer = new byte[100];

        protected virtual void ImGuiLayout()
        {
            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            //int windowFlags =| ImGuiWindowFlags.
            //io.ConfigFlags
            // Left panel - Game Setting
            // 1. Show a simple window
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

                ImGui.Text(Mouse.GetState().X.ToString());
                ImGui.Text(Mouse.GetState().Y.ToString());


                if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
                if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
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
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }

        protected override void Update(GameTime gameTime)
        {
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
                _camera.LookAtTarget(actor.GetPosition() + 50 * actor.GetForward(), actor.GetForward(), 200f, 150f);
            }

            _lastMouse.X = Mouse.GetState().X;
            _lastMouse.Y = Mouse.GetState().Y;
            actor.Update(gameTime);
            world = Matrix.CreateTranslation(actor.GetPosition());
        }
    }
}