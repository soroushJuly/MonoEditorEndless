using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoEditorEndless.Engine
{
    internal class Plane
    {
        private VertexPositionNormalTexture[] _vertices;
        // Indices for the plane
        private Int16[] _indices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private BasicEffect _basicEffect;
        private float _scale = 2000f;
        private Texture2D _texture;
        private int _repeat;
        public Plane(GraphicsDevice graphicsDevice, Texture2D texture, int size, int repeat)
        {
            _repeat = repeat;
            _scale = size;
            // Define the vertices of a box (a simple cube)
            _vertices = new VertexPositionNormalTexture[]
            {
                // Front face
                new VertexPositionNormalTexture(_scale * new Vector3(-1, 0,  -1), Vector3.UnitY, new Vector2(0, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1, 0,  -1), Vector3.UnitY, new Vector2(_repeat * 1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1,  0,  1), Vector3.UnitY, new Vector2(_repeat * 1, _repeat * 1)),
                new VertexPositionNormalTexture(_scale * new Vector3(-1,  0,  1), Vector3.UnitY, new Vector2(0, _repeat * 1)),
            };

            // Define the indices for the box (two triangles per face, 12 triangles total)
            _indices = new Int16[]
            {
                0,  1,  2,        0,  2,  3
            };

            // Initialize the vertex buffer
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(_vertices);

            // Initialize the index buffer
            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(Int16), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            // Initialize the BasicEffect
            _basicEffect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true,
            };
            _texture = texture;
        }
        public void Draw(GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection)
        {
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            _basicEffect.Projection = projection;
            _basicEffect.View = view;
            _basicEffect.World = world;
            _basicEffect.Texture = _texture;

            for (int i = 0; i < 6; i++)
            {
                foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, i * 4, 0, 2);
                }
            }
        }
    }
}
