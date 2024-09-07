using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace MonoEditorEndless.Engine
{
    internal class World
    {
        // Actors list
        List<Actor> _actors;

        // Collidables list
        List<Actor> _collidableActors;

        public World()
        {
            _actors = new List<Actor>();
            _collidableActors = new List<Actor>();
        }

        public void AddActor(Actor actor, bool bCollidable = false)
        {
            _actors.Add(actor);
            // If it's collidable add it to collision sensitive list
            if (bCollidable) { _collidableActors.Add(actor); }
        }
        // TODO: not sure if this is possible -> may iterate through the list instead
        public void RemoveCollidable(Actor actor) { _collidableActors.Remove(actor); }
        public void RemoveActor(Actor actor) { _actors.Remove(actor); _collidableActors.Remove(actor); }


        public void Update(GameTime gameTime)
        {
            foreach (Actor actor in _actors) { actor.Update(gameTime); }
            // Detect Collision
            // ToList() added to make a copy of the list each time
            // This way we wont get the collection modified error
            foreach (Actor collidable in _collidableActors.ToList())
            {
                bool hasCollision = false;
                foreach (Actor otherCollidable in _collidableActors.ToList())
                {
                    if (collidable.Equals(otherCollidable)) { continue; }
                    if (collidable.CollisionTest(otherCollidable))
                    {
                        hasCollision = true;
                        collidable.OnCollision(otherCollidable);
                    }
                }
                if (!hasCollision)
                {
                    collidable.OnCollision(null);
                }
            }
            RemoveFlaggedCollidables();
        }
        private void RemoveFlaggedCollidables()
        {
            List<Actor> removalList = new List<Actor>();
            foreach (Actor collidable in _collidableActors)
            {
                if (collidable.GetCollidable().GetRemoveFlag())
                {
                    removalList.Add(collidable);
                }
            }
            foreach (Actor collidable in removalList)
            {
                _collidableActors.Remove(collidable);
                _actors.Remove(collidable);
            }
        }

        public void Draw(Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            foreach (var actor in _actors)
            {
                actor.Draw(Matrix.CreateTranslation(actor.GetPosition()), view, projection, graphicsDevice);
            }
        }
    }
}
