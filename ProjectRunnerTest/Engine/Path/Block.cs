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

        public Directions GetDirection() { return _direction; }
    }
}
