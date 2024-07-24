using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace MonoEditorEndless.Engine.Input
{
    // Right now chords in the engine are just pairs
    // It can be more than two keys
    internal class Chord
    {
        // keyboard chords
        private KeyValuePair<Keys, Keys> keys;
        public Chord(Keys keyFirst, Keys keySecond)
        {
            keys = new KeyValuePair<Keys, Keys>(keyFirst, keySecond);
        }
        
    }
}
