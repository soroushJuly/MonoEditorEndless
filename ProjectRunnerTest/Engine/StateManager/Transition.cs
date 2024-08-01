using System;
using MonoEditorEndless.Engine.StateManager;
using Microsoft.Xna.Framework;

namespace MonoEndlesssRunner.Engine.StateManager
{
    public class Transition
    {
        public readonly State NextState;
        public readonly Func<bool> Condition;

        public Transition(State nextState, Func<bool> condition)
        {
            NextState = nextState;
            Condition = condition;
        }
    }
}