using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoEditorEndless.Engine.Path
{
    // This class is in charge of creating, managing and drawing the blocks in the path 
    internal class PathManager
    {
        Queue<Block> _activeBlocks;
        List<Actor> _roadBlocks;
        List<Actor> _turnRightBlocks;
        List<Actor> _wallBlocks;
        // Different collectable that might appear in the path
        public List<Actor> _collectableTypes;
        private int _collectableChance;
        // Different obstacles that might appear in the path
        List<Actor> _obstacleTypes;
        private int _obstacleChance;
        // Starting position of drawing active blocks
        Vector3 _startingPosition;
        // The chance that a wall appears. Between 0 to 100
        int _wallChance;
        // The chance that a turn appears. Between 0 to 100
        const int TURN_CHANCE_MAX = 8;
        int _turnChance;
        // To generate random numbers
        Random _random;

        private Directions _pathDirection;

        private Vector3 _latestPosition = Vector3.Zero;

        const int CHANCE_RANGE = 100;

        public event EventHandler<BlockEventArgs> BlockAdded;
        public event EventHandler<BlockEventArgs> BlockRemoved;

        public PathManager(int obstacleChance, int collectableChance)
        {
            _activeBlocks = new Queue<Block>();
            _roadBlocks = new List<Actor>();
            _wallBlocks = new List<Actor>();
            _turnRightBlocks = new List<Actor>();
            _collectableTypes = new List<Actor>();
            _obstacleTypes = new List<Actor>();
            _startingPosition = Vector3.Zero;
            // Turn chance starts with zero and get max in Initialize
            _turnChance = 0;
            _wallChance = 0;
            // Chances that an obstavle appears on the path
            _obstacleChance = obstacleChance;
            _collectableChance = collectableChance;
            // Initial path direction is North
            _pathDirection = Directions.NORTH;
            _random = new Random();
        }
        // Add a sample road block to block list to generate path from them
        public void AddRoadBlock(Actor actor)
        {
            actor.SetColliadableY(1000f);
            _roadBlocks.Add(actor);
        }
        // Add a wall block to generate walls with two direction to go
        public void AddWallBlock(Actor actor)
        {
            _wallBlocks.Add(actor);
        }
        public void AddTurnRight(Actor actor)
        {
            actor.SetColliadableY(1000f);
            _turnRightBlocks.Add(actor);
        }
        public void AddCollectable(Actor actor)
        {
            _collectableTypes.Add(actor);
        }
        public void AddObstacle(Actor actor)
        {
            _obstacleTypes.Add(actor);
        }
        public void Initialize(int numberOfBlocks)
        {
            for (int i = 0; i < numberOfBlocks; i++)
            {
                Generate();
            }
            _turnChance = TURN_CHANCE_MAX;
        }

        public void Generate()
        {
            int randomNumber = _random.Next(CHANCE_RANGE);
            // Straight blocks
            if (randomNumber >= _wallChance + _turnChance)
            {
                Actor rotatedActor = new Actor(_roadBlocks[0]);
                switch (_pathDirection)
                {
                    case Directions.NORTH:
                        break;
                    case Directions.SOUTH:
                        rotatedActor.RotateY((float)Math.PI);
                        break;
                    case Directions.EAST:
                        rotatedActor.RotateY((float)Math.PI / 2);
                        break;
                    case Directions.WEST:
                        rotatedActor.RotateY(-(float)Math.PI / 2);
                        break;
                    default:
                        break;
                }
                Vector3 nextPosition = GenerateNextPosition(rotatedActor, _pathDirection);
                _latestPosition += nextPosition;
                Actor actor2 = new Actor(rotatedActor);
                actor2.SetPosition(_latestPosition + -new Vector3(0, rotatedActor.GetDimentions().Y, 0));
                AddNewActiveBlock(new Block(actor2, _pathDirection));
            }
            // Two way block
            else if (_wallChance + _turnChance > randomNumber && randomNumber > _turnChance)
                CreateWall();
            // One way turns
            else CreateTurn(randomNumber);
            if (_turnChance < TURN_CHANCE_MAX && _turnChance != 0)
            {
                _turnChance++;
            }
        }
        private Vector3 GenerateNextPosition(Actor actor, Directions direction)
        {
            Vector3 nextPostion = Vector3.Zero;
            switch (_pathDirection)
            {
                case Directions.NORTH:
                    nextPostion += new Vector3(actor.GetDimentions().Z, 0, 0);
                    break;
                case Directions.SOUTH:
                    nextPostion += new Vector3(-actor.GetDimentions().Z, 0, 0);
                    break;
                case Directions.EAST:
                    nextPostion += new Vector3(0, 0, actor.GetDimentions().X);
                    break;
                case Directions.WEST:
                    nextPostion += new Vector3(0, 0, -actor.GetDimentions().X);
                    break;
                default:
                    break;
            }
            return nextPostion;
        }
        private void CreateWall()
        {
            Block newBlock = new Block(new Actor(_wallBlocks[0]), _pathDirection);
            AddNewActiveBlock(newBlock);
        }
        private void AddNewActiveBlock(Block newBlock)
        {
            int randomNumber = _random.Next(100);
            int randomNumber3 = _random.Next(100);
            int randomNumber4 = _random.Next(100);
            int randomNumber5 = _random.Next(100);
            int collectableChance = 20;
            if (randomNumber3 < _obstacleChance)
            {
                newBlock.InitializeObstacles(randomNumber5 / 100f, randomNumber4 / 100f, _obstacleTypes);
            }
            if (randomNumber < collectableChance && newBlock.GetName() == "road-straight")
            {
                int randomNumber2 = _random.Next(100);
                newBlock.InitializeCollectables(5, randomNumber2 / 100f, _collectableTypes);
            }
            _activeBlocks.Enqueue(newBlock);
            BlockAdded(this, new BlockEventArgs(newBlock));
        }
        private void CreateTurn(int randomNumber)
        {
            _turnChance = (int)Math.Floor((double)_turnChance / 2);
            // Turn Right
            if (randomNumber > _turnChance / 2)
            {
                // Rotated the actor based on the path direction
                Actor rotatedActor = new Actor(_turnRightBlocks[0]);
                // TODO: something is wrong with this 
                // TODO: (IMPORTANT) probably wont work with other 3d models
                switch (_pathDirection)
                {
                    case Directions.NORTH:
                        break;
                    case Directions.SOUTH:
                        rotatedActor.RotateY((float)Math.PI);
                        break;
                    case Directions.EAST:
                        rotatedActor.RotateY(-(float)Math.PI / 2);
                        break;
                    case Directions.WEST:
                        rotatedActor.RotateY((float)Math.PI / 2);
                        break;
                    default:
                        break;
                }
                Vector3 nextPosition = GenerateNextPosition(rotatedActor, _pathDirection);
                _latestPosition += nextPosition;
                rotatedActor.SetPosition(_latestPosition + -new Vector3(0, rotatedActor.GetDimentions().Y, 0));
                // Add the corner block
                AddNewActiveBlock(new Block(rotatedActor, _pathDirection));
                // Then change path
                _pathDirection++;
                if (_pathDirection == Directions.LAST) { _pathDirection = Directions.FIRST + 1; }
            }
            // Turn Left
            else
            {
                // Rotated the actor based on the path direction
                // create a turn left block and add it to the active blocks
                // Actor's Copy constructor
                Actor rotatedActor = new Actor(_turnRightBlocks[0]);
                rotatedActor.RotateY(-(float)Math.PI / 2f);
                switch (_pathDirection)
                {
                    case Directions.NORTH:
                        break;
                    case Directions.SOUTH:
                        rotatedActor.RotateY((float)Math.PI);
                        break;
                    case Directions.EAST:
                        rotatedActor.RotateY(-(float)Math.PI / 2);
                        break;
                    case Directions.WEST:
                        rotatedActor.RotateY((float)Math.PI / 2);
                        break;
                    default:
                        break;
                }
                Vector3 nextPosition = GenerateNextPosition(rotatedActor, _pathDirection);
                _latestPosition += nextPosition;
                rotatedActor.SetPosition(_latestPosition + -new Vector3(0, rotatedActor.GetDimentions().Y, 0));
                AddNewActiveBlock(new Block(rotatedActor, _pathDirection));
                // Change path
                _pathDirection--;
                if (_pathDirection == Directions.FIRST) { _pathDirection = Directions.LAST - 1; }
            }
        }
        public void RemoveLastBlock()
        {
            // To compensate their position in the current position
            // TODO: this can be saved during draw and be used here
            // TODO: removed block direction should be considerate when removing
            Block removedBlock = _activeBlocks.Dequeue();
            BlockRemoved(this, new BlockEventArgs(removedBlock));
        }
        public void Draw(Matrix world, Matrix view, Matrix projection)
        {
            foreach (Block block in _activeBlocks)
            {
                block.Draw(Matrix.CreateTranslation(block.GetPosition()), view, projection);
            }
        }
        public void Update(GameTime gameTime, Actor character)
        {
            int seenBlocks = 0;
            foreach (Block block in _activeBlocks)
            {
                block.Update(gameTime);
                if (!block._isSeenByCharacter)
                {
                    break;
                }
                if (block._isSeenByCharacter)
                {
                    seenBlocks++;
                }

            }
            if (_activeBlocks.Count - seenBlocks < 30)
            {
                Generate();
            }
            // TODO: maybe consider cutting the _activeBlocks to half instead
            // of this for better performance
            if (seenBlocks > 10)
            {
                RemoveLastBlock();
            }
        }
        public bool IsOnPath(Vector3 position)
        {
            bool isOnPath = false;
            foreach (Block block in _activeBlocks)
            {
                Collision.Collidable collidable = block.GetCollidable();
                if (position.X < collidable.Xmax && position.X > collidable.Xmin && position.Z > collidable.Zmin && position.Z < collidable.Zmax)
                {
                    isOnPath = true;
                }
            }
            return isOnPath;
        }
    }
}
