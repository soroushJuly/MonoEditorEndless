using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using MonoEditorEndless.Engine;
using MonoEditorEndless.Engine.Path;
using MonoEditorEndless.Engine.Input;
using MonoEditorEndless.Engine.Collision;
using MonoEditorEndless.Engine.StateManager;

using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MonoEditorEndless.Game
{
    internal class StateSpectate : State
    {
        ContentManager Content;
        GraphicsDevice _graphicsDevice;

        private Vector2 _lastMouse;

        private float _actorScale;

        private Camera _camera;


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
            _gameSession = new GameSession();
            _pathManager = new PathManager();
            _inputManager = new InputManager();
            //_inputManager.AddMouseBinding(eMouseInputs.X_MOVE, MoveX);

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

            obstacle = new Actor();

            _camera = new Camera();


            _lastMouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            LoadContent();
        }
        private void LoadContent()
        {
            actor.LoadModel(Content.Load<Model>("Content/FBX/Ship"));
            actor.SetScale(0.012f);
            actor.RotateY((-90f / 180f) * (float)Math.PI);
            road.LoadModel(Content.Load<Model>("Content/wall"));
            road.RotateY((90f / 180f) * (float)Math.PI);
            road.SetName("road-straight");
            wall.LoadModel(Content.Load<Model>("Content/wall-half"));
            roadR.LoadModel(Content.Load<Model>("Content/wall"));
            wall.RotateY((180f / 180f) * (float)Math.PI);
            collectable.LoadModel(Content.Load<Model>("Content/FBX/Coin"));
            collectable.SetScale(.1f);
            collectable.SetName("collectable");

            // Load the sound effect
            //_soundEffect = new SoundEffect();
            _soundEffect = Content.Load<SoundEffect>("Content/Audio/mario_coin_sound");
            _soundEffectInstance = _soundEffect.CreateInstance();
            _soundEffectInstance.Volume = .1f;

            obstacle.LoadModel(Content.Load<Model>("Content/rocks-small"));
            obstacle.SetScale(0.15f);
            obstacle.SetName("obstacle");

            _world.AddActor(actor, true);

            corner.LoadModel(Content.Load<Model>("Content/wall-corner"));
            corner.SetName("road-corner");

            _pathManager.AddObstacle(obstacle);
            _pathManager.AddCollectable(collectable);
            _pathManager.AddRoadBlock(road);
            _pathManager.AddWallBlock(wall);
            _pathManager.AddTurnRight(corner);
            _pathManager.Initialize(20);

            _bgMusic = Content.Load<Song>("Content/Audio/Titan");

            _skyboxTextureList = new List<Texture2D>();
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/front"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/back"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/left"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/right"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/top"));
            _skyboxTextureList.Add(Content.Load<Texture2D>("Content/Skybox/bottom"));
            _skybox = new Skybox(_graphicsDevice, _skyboxTextureList);

            Texture2D grass = Content.Load<Texture2D>("Content/grass");
            _plane = new MonoEditorEndless.Engine.Plane(_graphicsDevice, grass, 3000, 20);
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            _mouseActiveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // ------------ Editor Controls ----------- //
            // Free Camera Control

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

            _lastMouse.X = Mouse.GetState().X;
            _lastMouse.Y = Mouse.GetState().Y;

            curMouse = Mouse.GetState();
            prevMouse = curMouse;

            world = Matrix.CreateTranslation(actor.GetPosition());

            _prevKeyState = Keyboard.GetState();
        }
        public override void Exit(object owner) { }
        public override void Draw(GraphicsDevice GraphicsDevice = null, SpriteBatch spriteBatch = null)
        {
            //actor.Draw(world, _camera.GetView(), projection);
            _pathManager.Draw(world, _camera.GetView(), projection);
            //obstacle.Draw(Matrix.CreateTranslation(Vector3.Zero), _camera.GetView(), projection);
            _world.Draw(_camera.GetView(), projection);


            _skybox.Draw(_graphicsDevice, Matrix.CreateTranslation(_camera.GetPosition()), _camera.GetView(), projection);
            _plane.Draw(_graphicsDevice, Matrix.CreateTranslation(-100 * Vector3.UnitY), _camera.GetView(), projection);
        }
        
        //private Vector2 ProjectToScreen(Vector3 worldPosition)
        //{
        //    Viewport viewport = _graphicsDevice.Viewport;
        //    Vector3 projectedPosition = viewport.Project(worldPosition, projection, _view, Matrix.Identity);
        //    return new Vector2(projectedPosition.X, projectedPosition.Y);
        //}
    }
}
