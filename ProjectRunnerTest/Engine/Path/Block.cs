namespace MonoEditorEndless.Engine.Path
{
    internal class Block
    {
        // Encapsulate model, position, rotation, etc. information
        private Actor _actor;
        // Direction
        private Directions _direction;
        public Block(Actor actor, Directions direction)
        {
            _direction = direction;
            _actor = actor;
        }

        public Actor GetActor()
        {
            return _actor;
        }
        public Directions GetDirection() { return _direction; }
    }
}
