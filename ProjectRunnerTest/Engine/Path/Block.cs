using System.Diagnostics;

namespace MonoEditorEndless.Engine.Path
{
    internal class Block : Actor
    {
        // Actor class Encapsulate model, position, rotation, etc. information
        // Direction
        private Directions _direction;
        public Block(Actor actor, Directions direction) : base(actor)
        {
            _direction = direction;
        }
        public override void OnCollision(Actor otherActor)
        {
            // TODO: if the block is corner block there is a punishment
            if (otherActor?.GetName() == "character" && this.GetName() != "corner")
            {
                otherActor._isTurnAllowed = false;
            }
            if (otherActor?.GetName() == "character" && this.GetName() == "corner")
            {
                otherActor._isTurnAllowed = true;
                otherActor._lastCollisionSeen = "corner";
            }
        }

        public Directions GetDirection() { return _direction; }
    }
}
