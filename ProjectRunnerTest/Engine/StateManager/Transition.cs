using System;

namespace MonoEditorEndless.Engine.StateManager
{
    /// <summary>
    /// Code from "Computer Games Technology" module, City,University of London
    /// </summary>
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