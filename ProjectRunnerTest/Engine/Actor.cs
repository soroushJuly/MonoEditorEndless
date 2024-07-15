using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoEditorEndless.Engine.Collision;
using System;

namespace MonoEditorEndless.Engine
{
    internal class Actor
    {
        Vector3 _position;
        float _velocity;
        Vector3 _forwardVector;
        Model _model;
        Vector3 _dimentions;
        public float _scale;
        Matrix _scaleMatrix;


        // Collision
        private bool _bCollisionEnabled;
        Collidable _colliadable;
        public event EventHandler<CollisionEventArgs> CollisionHandler;

        public Actor()
        {
            _forwardVector = -Vector3.UnitZ;
            _position = Vector3.Zero;
            _velocity = 0f;
            _scale = 1f;
            _scaleMatrix = Matrix.Identity;

            _bCollisionEnabled = false;
            _colliadable = new Collidable();
        }
        public Actor(Vector3 position)
        {
            _position = position;
            _forwardVector = -Vector3.UnitZ;
            _velocity = 0f;
            _scale = 1f;
            _scaleMatrix = Matrix.Identity;

            _bCollisionEnabled = false;
            _colliadable = new Collidable();
        }
        // Getters
        public Vector3 GetPosition() { return _position; }
        public Vector3 GetForward() { return _forwardVector; }
        public Collidable GetCollidable() { return _colliadable; }
        public Model GetModel() { return _model; }
        public float GetScale() { return _scale; }
        public Vector3 GetDimentions() { return _dimentions; }
        public Matrix GetScaleMatrix() { return _scaleMatrix; }
        public float GetColliadableZ() { return _colliadable.Xmax; }
        // Setters
        public void SetVelocity(float velocity) { _velocity = velocity; }
        public void SetPosition(Vector3 position) { _position = position; }
        public void SetForward(Vector3 forwardVector) { _forwardVector = forwardVector; }
        public void SetScale(float scale)
        {
            _scale = scale;
            BoundingBox boundingBox = GetBoundingBox(_model);
            _dimentions = boundingBox.Max - boundingBox.Min;
            _scaleMatrix = Matrix.CreateScale(_scale);
            // Update the collidable properties
            _colliadable.Initialize(_position, _dimentions);
        }

        public void EnableCollision() { _bCollisionEnabled = true; }
        public bool CollisionTest(Actor otherActor)
        {
            return _colliadable.CollisionTest(otherActor.GetCollidable());
        }

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
                        min = Vector3.Min(min, transformedPosition) * _scale;
                        max = Vector3.Max(max, transformedPosition) * _scale;
                    }
                }
            }

            return new BoundingBox(min, max);
        }

        public void LoadModel(Model model)
        {
            _model = model;
            BoundingBox boundingBox = GetBoundingBox(model);
            _dimentions = boundingBox.Max - boundingBox.Min;
            // TODO: All actors have colliadable but deactive by default - change this
            // TODO: Colliadable initializes here - change this 
            _colliadable.Initialize(_position, _dimentions);
        }
        public void Update(GameTime gameTime)
        {
            _position += _forwardVector * _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_bCollisionEnabled)
            {
                _colliadable.Update(_position);
            }
        }
        public void OnCollision(Actor otherActor)
        {
            // If no subscriber this will return null
            this.CollisionHandler(this, new CollisionEventArgs(otherActor));
        }
    }
}
