using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ProjectRunnerTest;

using MonoEditorEndless.Engine.Collision;
using System;

namespace MonoEditorEndless.Engine
{
    internal class Actor
    {
        private string _name;
        private Vector3 _position;
        private float _velocity;
        // TODO: change to private later
        public float _maxVelocity;
        private Vector3 _forwardVector;
        private Vector3 _rightVector;
        private Model _model;
        private Vector3 _dimentions;
        private float _scale;
        public Vector3 _boundingScale = new Vector3(1f);
        private Matrix _scaleMatrix;
        private Matrix _rotationMatrix;
        private BoundingBox _boundingBox;
        // TODO: move this to another class
        public bool _isTurnAllowed = false;
        // TODO: move this to another class
        public int _health = 3;
        public bool _isActive = true;
        public bool _isRotating = false;
        public string _lastCollisionSeen = String.Empty;

        float _rotationYAnimation = 0f;
        float _rotationYAnimationMax = 0f;
        float _rotationSpeed = 0f;

        // Collision
        private Collidable _collidable;
        public event EventHandler<CollisionEventArgs> CollisionHandler;
        public event EventHandler NoCollisionHandler;

        public Actor()
        {
            _forwardVector = -Vector3.UnitZ;
            _rightVector = -Vector3.UnitX;
            _position = Vector3.Zero;
            _velocity = 0f;
            _scale = 1f;
            _scaleMatrix = Matrix.Identity;
            _rotationMatrix = Matrix.Identity;

            _collidable = new Collidable();
        }
        public Actor(Actor actor)
        {
            _name = actor._name;
            _forwardVector = actor._forwardVector;
            _rightVector = actor._rightVector;
            _position = actor._position;
            _velocity = actor._velocity;
            _scale = actor._scale;
            _scaleMatrix = actor._scaleMatrix;
            _rotationMatrix = actor._rotationMatrix;
            _dimentions = actor._dimentions;
            _model = actor._model;
            _boundingScale = actor._boundingScale;
            _collidable = new Collidable();
            _collidable.Initialize(_position, _dimentions, _boundingScale);

            _isRotating = actor._isRotating;
        }
        public Actor(Vector3 position)
        {
            _position = position;
            _forwardVector = -Vector3.UnitZ;
            _velocity = 0f;
            _scale = 1f;
            _scaleMatrix = Matrix.Identity;
            _rotationMatrix = Matrix.Identity;

            _collidable = new Collidable();
        }
        // Getters
        public string GetName() { return _name; }
        public Vector3 GetPosition() { return _position; }
        public Vector3 GetForward() { return _forwardVector; }
        public Vector3 GetRight() { return _rightVector; }
        public float GetVelocity() { return _velocity; }
        public Collidable GetCollidable() { return _collidable; }
        public Model GetModel() { return _model; }
        public float GetScale() { return _scale; }
        public Vector3 GetDimentions() { return _dimentions; }
        public Matrix GetScaleMatrix() { return _scaleMatrix; }
        public Matrix GetRotationMatrix() { return _rotationMatrix; }
        // Setters
        public void SetName(string name) { _name = name; }
        public void SetVelocity(float velocity) { _velocity = velocity; }
        public void SetPosition(Vector3 position)
        {
            _position = position;
            if (_model != null)
            {
                _boundingBox = GetBoundingBox(_model);
                _dimentions = (_boundingBox.Max - _boundingBox.Min) * _scale;
                // TODO: All actors have colliadable but deactive by default - change this
                // TODO: Colliadable initializes here - change this 
                _collidable.Initialize(_position, _dimentions, _boundingScale);
            }
        }
        public void SetColliadableY(float halfY) { _boundingScale.Y = halfY; }
        public void SetForward(Vector3 forwardVector) { _forwardVector = forwardVector; }
        public void SetRightVector(Vector3 rightVector) { _rightVector = rightVector; }
        public void SetScale(float scale)
        {
            _scale = scale;
            _boundingBox = GetBoundingBox(_model);
            _dimentions = (_boundingBox.Max - _boundingBox.Min) * _scale;
            _scaleMatrix = Matrix.CreateScale(_scale);
            // Update the collidable properties
            _collidable.Initialize(_position, _dimentions, _boundingScale);
        }


        public void RotateX(float amountRadian)
        {
            _rotationMatrix *= Matrix.CreateRotationX(amountRadian);
        }
        public void RotateY(float amountRadian)
        {
            _rotationMatrix *= Matrix.CreateRotationY(amountRadian);
        }
        public void SmoothRotateY(float amountRadian, float time)
        {
            _rotationSpeed = time;
            _rotationYAnimationMax = amountRadian;
        }
        public void RotateZ(float amountRadian)
        {
            _rotationMatrix *= Matrix.CreateRotationZ(amountRadian);
        }
        public bool CollisionTest(Actor otherActor)
        {
            return _collidable.CollisionTest(otherActor.GetCollidable());
        }
        /// <summary>
        /// Returns minimum points and maximum point in a 3D shape.
        /// Character scale has been considered during this
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private BoundingBox GetBoundingBox(Model model)
        {
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix meshTransform = boneTransforms[mesh.ParentBone.Index];

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // Get vertex data
                    VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.VertexBuffer.VertexCount];
                    part.VertexBuffer.GetData(vertexData);

                    // Transform each vertex position
                    foreach (VertexPositionNormalTexture vertex in vertexData)
                    {
                        Vector3 transformedPosition = Vector3.Transform(vertex.Position, meshTransform);

                        // Update min and max points
                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            return new BoundingBox(min, max);
        }

        public void LoadModel(Model model)
        {
            _model = model;
            _boundingBox = GetBoundingBox(_model);
            _dimentions = (_boundingBox.Max - _boundingBox.Min) * _scale;
            // TODO: All actors have colliadable but deactive by default - change this
            // TODO: Colliadable initializes here - change this 
            _collidable.Initialize(_position, _dimentions, _boundingScale);
        }
        public virtual void Update(GameTime gameTime)
        {
            if (_rotationYAnimationMax == 0)
            {
                _position += _forwardVector * _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_isRotating)
            {
                _rotationMatrix *= Matrix.CreateRotationY((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            // Update the collidable info if it was there
            _collidable?.Update(_position);
        }
        public virtual void OnCollision(Actor otherActor)
        {
            // If no subscriber this will return null
            if (CollisionHandler != null)
            {
                this.CollisionHandler(this, new CollisionEventArgs(otherActor));
            }
            if (otherActor == null && NoCollisionHandler != null)
            {
                this.NoCollisionHandler(this, EventArgs.Empty);
            }
        }
        public void Draw(Matrix world, Matrix view, Matrix projection, GraphicsDevice graphicsDevice = null, bool showCollisionBox = false)
        {
            //Model model = actor.GetModel();
            Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.World = world;
                    //effect.AmbientLightColor = new Vector3(1f, 0, 0);
                    // TODO: Add Rotation and Transformation here later
                    // TODO: fix this part later and move it to update method
                    Matrix smoothRotation = Matrix.Identity;
                    if (Math.Abs(_rotationYAnimation) < Math.Abs(_rotationYAnimationMax))
                    {
                        smoothRotation = Matrix.CreateRotationY(_rotationYAnimation);
                        if (_rotationYAnimationMax < 0)
                        {
                            _rotationYAnimation -= _rotationSpeed;
                            Matrix matrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, -_rotationSpeed);
                            if (Math.Abs(_rotationYAnimation) > Math.Abs(_rotationYAnimationMax))
                            {
                                matrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, -_rotationYAnimationMax + _rotationYAnimation);
                            }
                            _forwardVector = Vector3.TransformNormal(_forwardVector, matrix);
                            _rightVector = Vector3.TransformNormal(_rightVector, matrix);

                        }
                        else
                        {
                            _rotationYAnimation += _rotationSpeed;
                            Matrix matrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, _rotationSpeed);
                            if (Math.Abs(_rotationYAnimation) > Math.Abs(_rotationYAnimationMax))
                            {
                                matrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, (_rotationYAnimation - _rotationYAnimationMax));
                            }
                            _forwardVector = Vector3.TransformNormal(_forwardVector, matrix);
                            _rightVector = Vector3.TransformNormal(_rightVector, matrix);
                        }
                    }
                    else if (Math.Abs(_rotationYAnimation) > Math.Abs(_rotationYAnimationMax))
                    {
                        RotateY(_rotationYAnimationMax);
                        _rotationYAnimation = 0;
                        _forwardVector = new Vector3(
                            (float)Math.Round(_forwardVector.X),
                            (float)Math.Round(_forwardVector.Y),
                            (float)Math.Round(_forwardVector.Z));

                        _rightVector = new Vector3(
                            (float)Math.Round(_rightVector.X),
                            (float)Math.Round(_rightVector.Y),
                            (float)Math.Round(_rightVector.Z));

                        _rotationYAnimationMax = 0;
                    }
                    effect.World = GetScaleMatrix() * GetRotationMatrix() * smoothRotation * transforms[mesh.ParentBone.Index] * world;

                    // Use the matrices provided by the chase camera
                    effect.View = view;
                    effect.Projection = projection;
                    effect.TextureEnabled = true;
                    if (effect.Texture == null)
                    {
                        effect.TextureEnabled = false;
                    }
                    effect.FogEnabled = Application._project._gameConfigs.fogEnable;
                    effect.FogColor = Application._project._gameConfigs.fogColor; // For best results, make this color whatever your background is.
                    effect.FogStart = Application._project._gameConfigs.fogStartDistance;
                    effect.FogEnd = Application._project._gameConfigs.fogEndDistance;

                    //effect.EnableDefaultLighting();
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.Enabled = true;
                    // TODO: it should not be like this (Application in the actor class)
                    effect.DirectionalLight0.DiffuseColor = Application._project._gameConfigs.sunDiffuseColor;
                    effect.DirectionalLight0.SpecularColor = Application._project._gameConfigs.sunSpecularColor;
                    effect.DirectionalLight0.Direction = Application._project._gameConfigs.sunDirection;
                    // Default ambient color of monogame lights
                    effect.AmbientLightColor = new Vector3(0.05333332f, 0.09882354f, 0.1819608f);


                }
                mesh.Draw();
            }
            // draw the collision box
            if (showCollisionBox)
            {
                BasicEffect _collidableEffect = new BasicEffect(graphicsDevice)
                {
                    VertexColorEnabled = true,
                    World = Matrix.Identity,
                    View = view,
                    Projection = projection,
                };
                _collidable.DrawCollisionBox3D(graphicsDevice, _collidableEffect);
            }
        }
    }
}
