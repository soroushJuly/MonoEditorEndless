using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoEditorEndless.Engine.StateManager
{
    public class FSM
    {
        // The object that owns the state machine
        private object m_owner;
        // states of the owner object
        private List<State> m_states;
        // Current State of the state machine
        private State currentState;
        public State GetCurrentState() { return currentState; }

        public FSM(object owner)
        {
            m_owner = owner;
            m_states = new List<State>();
            currentState = null;
        }
        public void Initialise(string stateName)
        {
            currentState = m_states.Find(state => state.Name.Equals(stateName));
            if (currentState != null)
            {
                currentState.Enter(m_owner);
            }
        }
        public void AddState(State state) { m_states.Add(state); }
        public void Update(GameTime gameTime)
        {
            if (currentState == null) return;

            // First check if we need transition and then execute current transition
            foreach (Transition transition in currentState.GetTransitions())
            {
                // Execute the condition function
                if (transition.Condition())
                {
                    // Change the state if condition met
                    currentState.Exit(m_owner);
                    currentState = transition.NextState;
                    currentState.Enter(m_owner);
                    break;
                }
            }

            // Execute the current state 
            currentState.Execute(m_owner, gameTime);
        }
    }
}
