using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Engine.Collision
{
    internal class CollisionEventArgs
         : EventArgs
    {
        private Actor actor;
        public CollisionEventArgs(Actor actor)
        {

            this.actor = actor;
        }
    }
}
