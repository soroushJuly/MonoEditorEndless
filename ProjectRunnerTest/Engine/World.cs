using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Engine
{
    internal class World
    {
        // Actors list
        List<Actor> _actors;

        // Collidables list
        List<Actor> _collidableActors;

        //List<Collision> _collisions;

        public World()
        {
            _actors = new List<Actor>();
            _collidableActors = new List<Actor>();
        }

        public void AddActor(Actor actor, bool bCollidable = false)
        {
            _actors.Add(actor); 
            if (bCollidable) { _collidableActors.Add(actor); }
        }
        // TODO: not sure if this is possible -> may iterate through the list instead
        public void RemoveCollidable(Actor actor) { _collidableActors.Remove(actor); }
        public void RemoveActor(Actor actor) { _actors.Remove(actor); }


        public void Update(GameTime gameTime)
        {
            foreach (Actor actor in _actors) { actor.Update(gameTime); }
            // Detect Collision
            foreach (Actor collidable in _collidableActors)
            {
                foreach (Actor otherCollidable in _collidableActors)
                {
                    if (collidable.Equals(otherCollidable)) { break; }
                    if (collidable.CollisionTest(otherCollidable))
                    {
                        collidable.OnCollision(otherCollidable);
                    }
                }
            }
        }

        public void Draw()
        {

        }

    }
}
