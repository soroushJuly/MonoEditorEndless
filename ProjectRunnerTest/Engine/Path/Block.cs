using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoEditorEndless.Engine.Path
{
    internal class Block : Actor
    {
        // Actor class Encapsulate model, position, rotation, etc. information
        // Direction
        private Directions _direction;
        public List<Actor> _collectables;
        public List<Actor> _obstacles;
        public bool _isSeenByCharacter;
        public Block(Actor actor, Directions direction) : base(actor)
        {
            _collectables = new List<Actor>();
            _obstacles = new List<Actor>();
            _direction = direction;
        }
        public override void OnCollision(Actor otherActor)
        {
            // TODO: if the block is corner block there is a punishment
            if (otherActor?.GetName() == "character" && this.GetName() != "road-corner")
            {
                this._isSeenByCharacter = true;
                otherActor._isTurnAllowed = false;
            }
            if (otherActor?.GetName() == "character" && this.GetName() == "road-corner")
            {
                this._isSeenByCharacter = true;
                otherActor._isTurnAllowed = true;
                otherActor._lastCollisionSeen = "road-corner";
            }
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var collectable in _collectables)
            {
                collectable.Update(gameTime);
            }
        }
        public Directions GetDirection() { return _direction; }
        public void InitializeObstacles(float randomNumber, float randomNumber2, List<Actor> obstacleTypes)
        {
            Actor actor = obstacleTypes[0];
            Vector3 straightDirection = Vector3.Zero;
            Vector3 rightDirection = Vector3.Zero;
            if (_direction == Directions.NORTH)
            {
                straightDirection = Vector3.UnitX * GetDimentions().X / 2;
                rightDirection = -Vector3.UnitZ * GetDimentions().Z / 2;
            }
            else if (_direction == Directions.SOUTH)
            {
                straightDirection = -Vector3.UnitX * GetDimentions().X / 2;
                rightDirection = Vector3.UnitZ * GetDimentions().Z / 2;
            }
            else if (_direction == Directions.EAST)
            {
                straightDirection = Vector3.UnitZ * GetDimentions().Z / 2;
                rightDirection = Vector3.UnitX * GetDimentions().X / 2;
            }
            else if (_direction == Directions.WEST)
            {
                straightDirection = -Vector3.UnitZ * GetDimentions().Z / 2;
                rightDirection = -Vector3.UnitX * GetDimentions().X / 2;
            }
            actor.SetPosition(this.GetPosition() +
                new Vector3(0, this.GetDimentions().Y, 0) +
                rightDirection * randomNumber2 +
                straightDirection * randomNumber
                );
            _collectables.Add(new Actor(actor));
        }
        public void InitializeCollectables(int numberOfItems, float randomNumber, List<Actor> collectableTypes)
        {
            Actor actor = collectableTypes[0];
            Vector3 straightDirection = Vector3.Zero;
            Vector3 rightDirection = Vector3.Zero;
            if (_direction == Directions.NORTH)
            {
                straightDirection = Vector3.UnitX;
                rightDirection = -Vector3.UnitZ * GetDimentions().Z / 2;
            }
            else if (_direction == Directions.SOUTH)
            {
                straightDirection = -Vector3.UnitX;
                rightDirection = Vector3.UnitZ * GetDimentions().Z / 2;
            }
            else if (_direction == Directions.EAST)
            {
                straightDirection = Vector3.UnitZ;
                rightDirection = Vector3.UnitX * GetDimentions().X / 2;
            }
            else if (_direction == Directions.WEST)
            {
                straightDirection = -Vector3.UnitZ;
                rightDirection = -Vector3.UnitX * GetDimentions().X / 2;
            }
            for (int i = 0; i < numberOfItems; i++)
            {
                actor.SetPosition(this.GetPosition() +
                    new Vector3(0, this.GetDimentions().Y, 0) +
                    rightDirection * randomNumber +
                    i * 20 * straightDirection);
                _collectables.Add(new Actor(actor));
            }
        }
    }
}
