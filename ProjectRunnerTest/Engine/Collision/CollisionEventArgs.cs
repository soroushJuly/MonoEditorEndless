using System;

namespace MonoEditorEndless.Engine.Collision
{
    internal class CollisionEventArgs : EventArgs
    {
        public CollisionEventArgs(Actor actor)
        {
            this._actor = actor;
        }
        public Actor _actor;
    }
}
