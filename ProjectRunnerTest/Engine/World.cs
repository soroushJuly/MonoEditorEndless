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

        public World()
        {
            _actors = new List<Actor>();
            _collidableActors = new List<Actor>();
        }

        public void AddActor(Actor actor) { _actors.Add(actor); }
        public void AddCollidable(Actor actor) { _collidableActors.Add(actor); }
        // TODO: not sure if this is possible -> may iterate through the list instead
        public void RemoveCollidable(Actor actor) { _collidableActors.Remove(actor); }
        public void RemoveActor(Actor actor) { _actors.Remove(actor); }

        public void Draw()
        {

        }

    }
}
