using ImGuiNET;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using MonoEditorEndless.Engine;
using MonoEditorEndless.Engine.Input;
using MonoEditorEndless.Engine.Path;
using MonoEditorEndless.Engine.StateManager;
using ProjectRunnerTest;

using System;
using System.Collections.Generic;
// TODO: importing this might cause problems later
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MonoEditorEndless.Game
{
    internal class StateSpectate : State
    {
        private ContentManager Content;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        private Vector2 _lastMouse;

        private float _actorScale;

        private Camera _camera;

        // The most recent view that user was in. 3D or 2D
        private int _lastView;


        private World _world;
        private GameSession _gameSession;

        private PathManager _pathManager;
        // Example Model
        private Actor actor;
        private Actor road;
        private Actor wall;
        private Actor corner;
        private Actor roadR;
        private Actor collectable;
        private Actor obstacle;

        private Actor cameraModel;

        private float _mouseActiveTimer = 0f;

        KeyboardState _prevKeyState;

        private InputManager _inputManager;

        MouseState curMouse;
        MouseState prevMouse;

        // Skybox
        Skybox _skybox;
        List<Texture2D> _skyboxTextureList;

        // Audio
        SoundEffect _soundEffect;
        List<SoundEffect> _soundEffectList;
        SoundEffectInstance _soundEffectInstance;
        Song _bgMusic;
        List<Song> _songList;

        private MonoEditorEndless.Engine.Plane _plane;
        private SpriteFont _font;


        private Vector3 translation = Vector3.Zero;

        // Transforming matrices
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 5000f);
        public StateSpectate(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Name = "spectate";
            Content = content;
            _graphicsDevice = graphicsDevice;
        }
        public override void Enter(object owner)
        {
            _world = new World();
            _gameSession = new GameSession(Application._project._gameConfigs.gameAcceleration);
            _pathManager = new PathManager(Application._project._gameConfigs.obstacleChance, Application._project._gameConfigs.collectableChance);
            _inputManager = new InputManager();

            _spriteBatch = new SpriteBatch(_graphicsDevice);

            _pathManager.BlockAdded += (object sender, BlockEventArgs e) =>
            {
                _world.AddActor(e.GetBlock(), true);
                foreach (Actor collectable in e.GetBlock()._collectables)
                {
                    _world.AddActor(collectable, true);
                }
                foreach (Actor obstacle in e.GetBlock()._obstacles)
                {
                    _world.AddActor(obstacle, true);
                }

            };
            _pathManager.BlockRemoved += (object sender, BlockEventArgs e) =>
            {
                _world.RemoveActor(e.GetBlock());
                foreach (Actor collectable in e.GetBlock()._collectables)
                {
                    _world.RemoveActor(collectable);
                }
                foreach (Actor obstacle in e.GetBlock()._obstacles)
                {
                    _world.RemoveActor(obstacle);
                }
            };

            actor = new Actor();
            actor.SetVelocity(160f);
            actor.SetForward(Vector3.UnitX);
            actor.SetRightVector(-Vector3.UnitZ);
            actor.SetPosition(0 * Vector3.UnitY);
            actor.SetName("character");

            road = new Actor();
            wall = new Actor();
            roadR = new Actor();
            corner = new Actor();
            collectable = new Actor();

            // Editor
            cameraModel = new Actor();

            obstacle = new Actor();

            _camera = new Camera();


            _lastMouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            LoadContent();
        }
        private void LoadContent()
        {
            actor.LoadModel(Content.Load<Model>("Content/Model/Ship"));
            actor.SetScale(0.012f);
            actor.RotateY((-90f / 180f) * (float)Math.PI);
            road.LoadModel(Content.Load<Model>("Content/Model/wall"));
            road.RotateY((90f / 180f) * (float)Math.PI);
            road.SetName("road-straight");
            wall.LoadModel(Content.Load<Model>("Content/Model/wall-half"));
            roadR.LoadModel(Content.Load<Model>("Content/Model/wall"));
            wall.RotateY((180f / 180f) * (float)Math.PI);
            collectable.LoadModel(Content.Load<Model>("Content/Model/" + Path.GetFileNameWithoutExtension(Application._project._gameConfigs.collectableModel)));
            collectable.SetScale(Application._project._gameConfigs.collectableScale);
            collectable.SetPosition(collectable.GetPosition() + Vector3.UnitY * Application._project._gameConfigs.collectableOffset);
            collectable.SetName("collectable");
            cameraModel.LoadModel(Content.Load<Model>("Content/Editor/Model/camera"));

            obstacle.LoadModel(Content.Load<Model>("Content/Model/" + Path.GetFileNameWithoutExtension(Application._project._gameConfigs.obstacleModel)));
            obstacle.SetScale(Application._project._gameConfigs.obstacleScale);
            obstacle.SetName("obstacle");

            _world.AddActor(actor, true);

            corner.LoadModel(Content.Load<Model>("Content/Model/wall-corner"));
            corner.SetName("road-corner");

            _pathManager.AddObstacle(obstacle);
            _pathManager.AddCollectable(collectable);
            _pathManager.AddRoadBlock(road);
            _pathManager.AddWallBlock(wall);
            _pathManager.AddTurnRight(corner);
            _pathManager.Initialize(20);

            _skyboxTextureList = new List<Texture2D>();
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Texture/front"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Texture/back"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Texture/left"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Texture/right"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Texture/top"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Texture/bottom"));
            _skybox = new Skybox(_graphicsDevice, _skyboxTextureList);

            Texture2D grass = Content.Load<Texture2D>("Content/Texture/grass");

            _font = Content.Load<SpriteFont>("Content/Font/File");

            _plane = new MonoEditorEndless.Engine.Plane(_graphicsDevice, grass, 3000, 20);
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            cameraModel.SetPosition(actor.GetPosition()
                + Application._project._gameConfigs.cameraHeight * Vector3.UnitY
                - Application._project._gameConfigs.distanceFromCharacter * actor.GetForward());
            _mouseActiveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Camera switched to 2D
            if (Application._project._editorConfigs._selectedView == 1 && _lastView == 0)
            {
                _camera.LookAtTarget(new Vector3(_camera.GetPosition().X, _camera.GetPosition().Y, _camera.GetPosition().Z), -Vector3.UnitY, 0, 0);
                _camera._frontVector = -Vector3.UnitY;
                _camera._rightVector = Vector3.UnitZ;
                _camera._upVector = Vector3.UnitX;
            }
            // Save the latest camera view
            _lastView = Application._project._editorConfigs._selectedView;
            // ------------ Editor Controls ----------- //
            // Free Camera Control
            _camera._sensitivity = Application._project._editorConfigs._spectateSensitivity;
            _camera._speed = Application._project._editorConfigs._spectateMoveSpeed;
            // 3D camera control
            if (Application._project._editorConfigs._selectedView == 0)
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
            // 2D Camera Control - Top-down view
            else if (Application._project._editorConfigs._selectedView == 1)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    _camera.MoveLeft(.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    _camera.MoveUp(.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    _camera.MoveUp(-.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    _camera.MoveLeft(-.1f * (float)gameTime.ElapsedGameTime.Milliseconds);
                }
                // Zoom In/Zoom Out
                float diff = Mouse.GetState().ScrollWheelValue - prevMouse.ScrollWheelValue;
                _camera.MoveForward(diff * 0.005f * (float)gameTime.ElapsedGameTime.Milliseconds);
            }

            curMouse = Mouse.GetState();
            _lastMouse.X = curMouse.X;
            _lastMouse.Y = curMouse.Y;
            prevMouse = curMouse;

            world = Matrix.CreateTranslation(actor.GetPosition());

            _prevKeyState = Keyboard.GetState();

            _camera.Update();
        }
        public override void Exit(object owner) { Content.Unload(); }
        public override void Draw(GraphicsDevice GraphicsDevice = null, SpriteBatch spriteBatch = null)
        {
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;
            var lastRasterizer = _graphicsDevice.RasterizerState;
            var lastDepthStencil = _graphicsDevice.DepthStencilState;
            var lastBlendFactor = _graphicsDevice.BlendFactor;
            var lastBlendState = _graphicsDevice.BlendState;
            var lastSamplerStates = _graphicsDevice.SamplerStates;

            _pathManager.Draw(world, _camera.GetView(), projection);
            _world.Draw(_camera.GetView(), projection);

            // Draw debug camera
            cameraModel.Draw(Matrix.CreateTranslation(cameraModel.GetPosition()), _camera.GetView(), projection);
            // Draw the line that connects the camera to the point it look at
            DrawCameraRay(GraphicsDevice);


            _skybox.Draw(_graphicsDevice, Matrix.CreateTranslation(_camera.GetPosition()), _camera.GetView(), projection);
            _plane.Draw(_graphicsDevice, Matrix.CreateTranslation(-100 * Vector3.UnitY), _camera.GetView(), projection);

            if (Application._project._editorConfigs._showInstructions)
                DrawInstructions();

            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
            _graphicsDevice.RasterizerState = lastRasterizer;
            _graphicsDevice.DepthStencilState = lastDepthStencil;
            _graphicsDevice.BlendState = lastBlendState;
            _graphicsDevice.BlendFactor = lastBlendFactor;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
        private void DrawInstructions()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.DepthRead);
            _spriteBatch.DrawString(_font, "press W,A,S,D to move around the map", new Vector2(355, ImGui.GetFrameHeight() + 5), Color.White,
                0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_font, "Hold right click and move the mouse to look around", new Vector2(355, ImGui.GetFrameHeight() + 30), Color.White,
                0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            if (Application._project._editorConfigs._selectedView == 1)
                _spriteBatch.DrawString(_font, "Mouse scroll wheel to zoom in/out", new Vector2(355, ImGui.GetFrameHeight() + 55), Color.White,
                0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            _spriteBatch.End();
        }
        private void DrawCameraRay(GraphicsDevice GraphicsDevice)
        {
            BasicEffect _basicEffect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = projection,
                View = _camera.GetView(),
                World = world
            };
            // The position of the camera
            Vector3 point1 = cameraModel.GetPosition();
            // To where camera look at
            Vector3 point2 = actor.GetPosition() + Application._project._gameConfigs.cameraLookDistance * actor.GetForward();

            // Define vertices for the line
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(point1, Color.Red);
            vertices[1] = new VertexPositionColor(point2, Color.Red);

            // Draw the line
            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            }
        }
    }
}
