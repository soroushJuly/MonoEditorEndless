using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;

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
        public void Initialize(Vector3 BasePosition, Vector3 dimentions)
        {
            _halfX = dimentions.X / 2;
            _halfY = dimentions.Y / 2;
            _halfZ = dimentions.Z / 2;
            Zmax = BasePosition.Z + _halfZ;
            Zmin = BasePosition.Z - _halfZ;
            Xmax = BasePosition.X + _halfX;
            Xmin = BasePosition.X - _halfX;
            Ymax = BasePosition.Y + dimentions.Y;
            Ymin = BasePosition.Y;
        }
        public void Update(Vector3 BasePosition)
        {
            Zmax = BasePosition.Z + _halfZ;
            Zmin = BasePosition.Z - _halfZ;
            Xmax = BasePosition.X + _halfX;
            Xmin = BasePosition.X - _halfX;
            Ymax = BasePosition.Y + _halfY * 2;
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
    }
}
