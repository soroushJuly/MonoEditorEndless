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
using ProjectRunnerTest;

namespace MonoEditorEndless.Game
{
    internal class StatePlay : State
    {
        ContentManager Content;
        GraphicsDevice _graphicsDevice;
        SpriteBatch _spriteBatch;

        private Texture2D _heartTexture;
        private SpriteFont _font;

        private Vector2 _lastMouse;

        private float _actorScale;

        private Camera _camera;


        private Matrix _view;

        private World _world;
        private GameSession _gameSession;

        private PathManager _pathManager;
        // Example Model
        private Actor actor;
        private Actor road;
        private Actor corner;
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
        SoundEffect _bgMusicEffect;
        SoundEffect _collectedSound;
        SoundEffect _collidedSound;
        List<Song> _songList;

        private Engine.Plane _plane;

        private Vector3 translation = Vector3.Zero;

        // Transforming matrices
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 5000f);

        public event EventHandler<SessionArgs> SessionFinished;
        public StatePlay(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Name = "play";
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



            //_inputManager.AddMouseBinding(eMouseInputs.X_MOVE, MoveX);
            _inputManager.AddKeyboardBinding(Keys.D, TurnRight);
            _inputManager.AddKeyboardBinding(Keys.A, TurnLeft);

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
            actor.SetVelocity(Application._project._gameConfigs.characterMinSpeed);
            actor.SetForward(Vector3.UnitX);
            actor.SetRightVector(-Vector3.UnitZ);
            actor.SetPosition(0 * Vector3.UnitY);
            actor.SetName("character");
            actor._health = Application._project._gameConfigs.characterHealth;
            actor._maxVelocity = Application._project._gameConfigs.characterMaxSpeed;

            road = new Actor();
            corner = new Actor();
            collectable = new Actor();

            obstacle = new Actor();

            actor.CollisionHandler += this.CharacterCollisionHandler;
            actor.NoCollisionHandler += this.CharacterNoCollisionHandler;
            _camera = new Camera();

            _view = Matrix.CreateLookAt(_camera.GetPosition(), Vector3.Zero, Vector3.Up);


            _lastMouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            LoadContent();

            _gameSession.StartSession();
        }
        private void LoadContent()
        {
            // Character
            actor.LoadModel(Content.Load<Model>("Content/Model/" + Application._project._gameConfigs.characterModel));
            actor.SetScale(Application._project._gameConfigs.characterScale);
            actor.RotateY((Application._project._gameConfigs.characterRotateY / 180f) * (float)Math.PI);
            // Block straight
            road.LoadModel(Content.Load<Model>("Content/Model/" + Application._project._gameConfigs.blockStraightModel));
            road.SetScale(Application._project._gameConfigs.blockStraightScale);
            road.RotateY((90f / 180f) * (float)Math.PI);
            road.SetName("road-straight");
            // Collectable
            collectable.LoadModel(Content.Load<Model>("Content/Model/" + Application._project._gameConfigs.collectableModel));
            collectable.SetScale(Application._project._gameConfigs.collectableScale);
            collectable.SetPosition(collectable.GetPosition() + Vector3.UnitY * Application._project._gameConfigs.collectableOffset);
            collectable.SetName("collectable");
            collectable._isRotating = Application._project._gameConfigs.isCollectableRotating;
            // Obstacle
            obstacle.LoadModel(Content.Load<Model>("Content/Model/" + Application._project._gameConfigs.obstacleModel));
            obstacle.SetScale(Application._project._gameConfigs.obstacleScale);
            obstacle.SetName("obstacle");
            // Block turn
            corner.LoadModel(Content.Load<Model>("Content/Model/" + Application._project._gameConfigs.blockTurnModel));
            corner.SetScale(Application._project._gameConfigs.blockTurnScale);
            corner.SetName("road-corner");

            _world.AddActor(actor, true);

            _pathManager.AddObstacle(obstacle);
            _pathManager.AddCollectable(collectable);
            _pathManager.AddRoadBlock(road);
            _pathManager.AddTurnRight(corner);
            _pathManager.Initialize(20);

            // Audio loading
            _bgMusicEffect = Content.Load<SoundEffect>("Content/Audio/" + Application._project._gameConfigs.audioBackground);
            _bgMusicEffect.Play();
            _collectedSound = Content.Load<SoundEffect>("Content/Audio/" + Application._project._gameConfigs.audioCollected);
            _collidedSound = Content.Load<SoundEffect>("Content/Audio/" + Application._project._gameConfigs.audioCollided);

            _skyboxTextureList = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.skyFront),
                Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.skyBack),
                Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.skyLeft),
                Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.skyRight),
                Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.skyTop),
                Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.skyBottom)
            };
            _skybox = new Skybox(_graphicsDevice, _skyboxTextureList);

            Texture2D planeTexture = Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.planeTexture);
            _plane = new Engine.Plane(_graphicsDevice, planeTexture, 3000, 20);

            _font = Content.Load<SpriteFont>("Content/Font/File");

            _heartTexture = Content.Load<Texture2D>("Content/Texture/" + Application._project._gameConfigs.healthIcon);
            // Create a 1x1 white texture
            //_whiteTexture = new Texture2D(_graphicsDevice, 1, 1);
            //_whiteTexture.SetData(new[] { Color.White });
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            _mouseActiveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // third person camera
            // Here we update position of camera
            _camera.LookAtTarget(actor.GetPosition(),
                actor.GetForward(),
                Application._project._gameConfigs.distanceFromCharacter,
                Application._project._gameConfigs.cameraHeight,
                Application._project._gameConfigs.cameraLookDistance);
            // We need to update camera position again too, because skybox is dependent on it
            _camera.SetPosition(actor.GetPosition() - 50 * actor.GetForward());


            if (_prevKeyState.IsKeyDown(Keys.F) && Keyboard.GetState().IsKeyUp(Keys.F))
            {
                _pathManager.Generate();
            }
            _gameSession.Update(gameTime);

            _lastMouse.X = Mouse.GetState().X;
            _lastMouse.Y = Mouse.GetState().Y;
            _inputManager.Update();
            if (_gameSession.GetState() == GameSession.State.START)
            {
                float newSpeed = actor.GetVelocity() + 0.0002f * _gameSession.GetGameSpeed();
                if (Application._project._gameConfigs.characterHasMaxSpeed && newSpeed >= actor._maxVelocity) { actor.SetVelocity(actor._maxVelocity); }
                else if (newSpeed < actor._maxVelocity) { actor.SetVelocity(newSpeed); }
            }
            actor.SetVelocity(actor.GetVelocity() + 0.0002f * _gameSession.GetGameSpeed());
            curMouse = Mouse.GetState();
            float x = (curMouse.X - prevMouse.X) / 5f * Application._project._gameConfigs.characterMoveSensitivity;
            prevMouse = curMouse;
            // slide right and left
            if (_mouseActiveTimer > 2)
            {
                Vector3 position = actor.GetPosition() + -x * actor.GetRight();
                // Check if it is allowed to do the next move
                if (_pathManager.IsOnPath(position))
                    actor.SetPosition(position);
            }
            _pathManager.Update(gameTime, actor);
            //actor.Update(gameTime);
            _world.Update(gameTime);

            _prevKeyState = Keyboard.GetState();
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

            _pathManager.Draw(_camera.GetView(), projection);
            _world.Draw(_camera.GetView(), projection, GraphicsDevice);


            _skybox.Draw(_graphicsDevice, Matrix.CreateTranslation(_camera.GetPosition()), _camera.GetView(), projection);
            _plane.Draw(_graphicsDevice, Matrix.CreateTranslation(-100 * Vector3.UnitY), _camera.GetView(), projection);


            DrawHUD();

            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
            _graphicsDevice.RasterizerState = lastRasterizer;
            _graphicsDevice.DepthStencilState = lastDepthStencil;
            _graphicsDevice.BlendState = lastBlendState;
            _graphicsDevice.BlendFactor = lastBlendFactor;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
        private void DrawHUD()
        {

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.DepthRead);
            // Draw the health bar
            for (int i = 0; i < actor._health; i++)
            {
                _spriteBatch.Draw(_heartTexture, new Rectangle(
                    (int)Application._project._gameConfigs.healthPosition.X + (int)(i * _heartTexture.Width * Application._project._gameConfigs.healthScale),
                    (int)Application._project._gameConfigs.healthPosition.Y,
                    (int)(_heartTexture.Width * Application._project._gameConfigs.healthScale),
                    (int)(_heartTexture.Height * Application._project._gameConfigs.healthScale)),
                    Color.Gray); // Background
            }
            _spriteBatch.DrawString(_font, "Score:" + _gameSession.GetPoints(), Application._project._gameConfigs.scorePosition, Color.Black);
            _spriteBatch.End();
        }
        // TODO: These can go inside the pawn class
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
            if (!actor._isTurnAllowed)
            {
                return;
            }
            if (buttonState == eButtonState.PRESSED)
            {
                actor.SmoothRotateY(-(float)Math.PI / 2f, 0.04f);
            }
        }
        public void TurnLeft(eButtonState buttonState, Vector2 amount)
        {
            if (!actor._isTurnAllowed)
            {
                return;
            }
            if (buttonState == eButtonState.PRESSED)
            {
                actor.SmoothRotateY((float)Math.PI / 2f, 0.04f);
            }
        }
        void CharacterCollisionHandler(object sender, CollisionEventArgs e)
        {
            Actor character = sender as Actor;
            // Character collides with collectable item
            if (e._actor?.GetName() == "collectable")
            {
                _gameSession.AddPoint(Application._project._gameConfigs.itemValue);
                SoundEffectInstance collectionSound = _collectedSound.CreateInstance();
                collectionSound.Volume = Application._project._gameConfigs.audioCollectedVolume;
                collectionSound.Play();
                //_collectedSound.Play();
                _world.RemoveActor(e._actor);
            }
            if (e._actor?.GetName() == "obstacle" && e._actor._isActive == true)
            {
                // TODO: define an enum for this
                // 0 means health loss on collision
                if (Application._project._gameConfigs.obstacleBehavior == 0)
                {
                    actor._health--;
                    SoundEffectInstance collisionSound = _collidedSound.CreateInstance();
                    collisionSound.Volume = Application._project._gameConfigs.audioCollidedVolume;
                    collisionSound.Play();
                    e._actor._isActive = false;
                    if (actor._health == 0)
                    {
                        // Game over
                        SessionFinished(this, new SessionArgs(_gameSession.GetPoints(), _gameSession.GetTime()));
                    }
                }
                // 1 means game over on collision
                else if (Application._project._gameConfigs.obstacleBehavior == 1)
                {
                    SessionFinished(this, new SessionArgs(_gameSession.GetPoints(), _gameSession.GetTime()));
                }
            }
            if (e._actor?.GetName() == "road-corner")
            {
                character._lastCollisionSeen = "road-corner";
            }
        }
        void CharacterNoCollisionHandler(object sender, EventArgs e)
        {
            Actor character = sender as Actor;
            if (character._lastCollisionSeen == "road-corner")
            {
                SessionFinished(this, new SessionArgs(_gameSession.GetPoints(), _gameSession.GetTime()));
            }
        }
    }
}
