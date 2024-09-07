using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoEditorEndless.Engine.Collision
{
    internal class Collidable
    {
        // Here the Colliadable type is AABB
        // TODO: can have multiple types in future
        float _halfX;
        float _halfY;
        float _halfZ;
        public float Xmax;
        public float Ymax;
        public float Zmax;
        public float Xmin;
        public float Ymin;
        public float Zmin;
        // This flag indicates if this collidable should be removed or not
        private bool _removalFlag;
        private Vector3 _boundingScale = new Vector3(1f);
        public bool GetRemoveFlag() { return _removalFlag; }
        public void SetRemoveFlag(bool value)
        {
            _removalFlag = value;
        }
        public void Initialize(Vector3 BasePosition, Vector3 dimentions, Vector3 boundingScale)
        {
            _boundingScale = boundingScale;
            _halfX = dimentions.X / 2 * boundingScale.X;
            _halfY = dimentions.Y / 2 * boundingScale.Y;
            _halfZ = dimentions.Z / 2 * boundingScale.Z;
            Zmax = BasePosition.Z + _halfZ;
            Zmin = BasePosition.Z - _halfZ;
            Xmax = BasePosition.X + _halfX;
            Xmin = BasePosition.X - _halfX;
            Ymax = BasePosition.Y + 2 * _halfY;
            Ymin = BasePosition.Y;
        }
        public void Update(Vector3 BasePosition)
        {
            Zmax = BasePosition.Z + _halfZ;
            Zmin = BasePosition.Z - _halfZ;
            Xmax = BasePosition.X + _halfX;
            Xmin = BasePosition.X - _halfX;
            Ymax = BasePosition.Y + 2 * _halfY;
            Ymin = BasePosition.Y;
        }
        public bool CollisionTest(Collidable collidable)
        {
            // AABB collision testing
            if (
                collidable.Xmax > this.Xmin &&
                collidable.Xmin < this.Xmax &&
                collidable.Ymax > this.Ymin &&
                collidable.Ymin < this.Ymax &&
                collidable.Zmax > this.Zmin &&
                collidable.Zmin < this.Zmax
                )
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Draws the collision box having 8 vertices
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="basicEffect"></param>
        public void DrawCollisionBox3D(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[8];

            // Define the 8 corners of the 3D bounding box
            vertices[0] = new VertexPositionColor(new Vector3(Xmin, Ymin, Zmin), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(Xmax, Ymin, Zmin), Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(Xmax, Ymax, Zmin), Color.Red);
            vertices[3] = new VertexPositionColor(new Vector3(Xmin, Ymax, Zmin), Color.Red);

            vertices[4] = new VertexPositionColor(new Vector3(Xmin, Ymin, Zmax), Color.Red);
            vertices[5] = new VertexPositionColor(new Vector3(Xmax, Ymin, Zmax), Color.Red);
            vertices[6] = new VertexPositionColor(new Vector3(Xmax, Ymax, Zmax), Color.Red);
            vertices[7] = new VertexPositionColor(new Vector3(Xmin, Ymax, Zmax), Color.Red);

            short[] indices = new short[]
            {
        0, 1, 1, 2, 2, 3, 3, 0,  // Front face
        4, 5, 5, 6, 6, 7, 7, 4,  // Back face
        0, 4, 1, 5, 2, 6, 3, 7   // Connecting edges
            };

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertices,
                    0,
                    8,
                    indices,
                    0,
                    12
                );
            }
        }
    }
}
