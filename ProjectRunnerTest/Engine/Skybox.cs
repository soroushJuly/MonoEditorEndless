using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Engine
{
    internal class Skybox
    {
        private VertexPositionNormalTexture[] _vertices;
        // Indices for the cube
        private Int16[] _indices;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private BasicEffect _basicEffect;
        // Front, Back, Left, Right, Top, Bottom
        private List<Texture2D> _textures = new List<Texture2D>();

        private float _scale = 2000f;

        public Skybox(GraphicsDevice graphicsDevice, List<Texture2D> textures)
        {
            // Define the vertices of a box (a simple cube)
            _vertices = new VertexPositionNormalTexture[]
            {
                // Front face
                new VertexPositionNormalTexture(_scale * new Vector3(-1, -1,  1), -Vector3.UnitZ, new Vector2(0, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1, -1,  1), -Vector3.UnitZ, new Vector2(1, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1,  1,  1), -Vector3.UnitZ, new Vector2(1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3(-1,  1,  1), -Vector3.UnitZ, new Vector2(0, 0)),

                // Back face
                new VertexPositionNormalTexture(_scale * new Vector3(1, -1, -1), Vector3.UnitZ, new Vector2(0, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( -1, -1, -1), Vector3.UnitZ, new Vector2(1, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( -1,  1, -1), Vector3.UnitZ, new Vector2(1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3(1,  1, -1), Vector3.UnitZ, new Vector2(0, 0)),

                // Left face
                new VertexPositionNormalTexture(_scale * new Vector3(-1, -1, -1), Vector3.UnitX, new Vector2(0, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3(-1,  -1, 1), Vector3.UnitX, new Vector2(1, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3(-1,  1,  1), Vector3.UnitX, new Vector2(1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3(-1, 1,  -1), Vector3.UnitX, new Vector2(0, 0)),

                // Right face
                new VertexPositionNormalTexture(_scale * new Vector3( 1, -1,  1), -Vector3.UnitX, new Vector2(0, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1,  -1,  -1), -Vector3.UnitX, new Vector2(1, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1,  1, -1), -Vector3.UnitX, new Vector2(1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1, 1, 1), -Vector3.UnitX, new Vector2(0, 0)),

                // Top face
                new VertexPositionNormalTexture(_scale * new Vector3(-1,  1, 1), -Vector3.UnitY, new Vector2(0, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3(1,  1,  1), -Vector3.UnitY, new Vector2(1, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1,  1,  -1), -Vector3.UnitY, new Vector2(1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3( -1,  1, -1), -Vector3.UnitY, new Vector2(0, 0)),

                // Bottom face
                new VertexPositionNormalTexture(_scale * new Vector3(-1, -1,  -1), Vector3.UnitY, new Vector2(1, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3(1, -1, -1), Vector3.UnitY, new Vector2(0, 0)),
                new VertexPositionNormalTexture(_scale * new Vector3( 1, -1, 1), Vector3.UnitY, new Vector2(0, 1)),
                new VertexPositionNormalTexture(_scale * new Vector3( -1, -1,  1), Vector3.UnitY, new Vector2(1, 1))
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
            _textures = textures;
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix world, Matrix view, Matrix projection)
        {
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            _basicEffect.Projection = projection;
            _basicEffect.View = view;
            _basicEffect.World = world;

            for (int i = 0; i < 6; i++)
            {
                _basicEffect.Texture = _textures[i];
                foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, i * 4, 0, 2);
                }
            }
        }
    }
}
