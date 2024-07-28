using Microsoft.Xna.Framework;

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
        public void SetY(float halfY)
        {
            _halfY = halfY;
        }
    }
}
