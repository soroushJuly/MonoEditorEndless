using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Engine.Components
{
    interface ICollision 
    {
        public void CollisionTest()
        {

        }
        public void OnCollision() { }
    }
}
