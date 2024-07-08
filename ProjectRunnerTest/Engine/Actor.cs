using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MonoEditorEndless.Engine
{
    internal class Actor
    {
        Vector3 _position;
        float _velocity;
        Vector3 _forwardVector;
        Model _model;
        Vector3 _dimentions;
        Vector3 _Scale;

        public Actor()
        {
            _forwardVector = -Vector3.UnitZ;
            _position = Vector3.Zero;
            _velocity = 0f;
        }
        public Actor(Vector3 position)
        {
            _position = position;
        }
        // Getters
        public Vector3 GetPosition() { return _position; }
        public Vector3 GetForward() { return _forwardVector; }
        public Model GetModel() { return _model; }
        public Vector3 GetDimentions() { return _dimentions; }
        // Setters
        public void SetVelocity(float velocity) { _velocity = velocity; }
        public void SetForward(Vector3 forwardVector) { _forwardVector = forwardVector; }

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
            BoundingBox boundingBox = GetBoundingBox(model);
            _dimentions = boundingBox.Max - boundingBox.Min;
        }
        public void Update(GameTime gameTime)
        {
            _position += _forwardVector * _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
