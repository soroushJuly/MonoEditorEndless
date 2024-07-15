using Microsoft.Xna.Framework;

namespace MonoEditorEndless.Engine
{
    internal class GameSession
    {
        private float _sessionTimePassed;
        private float _points;
        private State _sessionState;
        private enum State
        {
            IDLE,
            STOP,
            START,
            PAUSE
        };
        public GameSession()
        {
            ResetSession();
        }

        public float GetPoints() { return _points; }

        public void StartSession()
        {
            _sessionState = State.START;
        }
        public void OnUpdate(GameTime gameTime)
        {
            if (_sessionState == State.STOP)
            {
                ResetSession();
            }
            if (_sessionState == State.START)
            {
                _sessionTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            // If sessionState == PAUSE dont do anything
        }
        private void ResetSession()
        {
            _sessionTimePassed = 0f;
            _points = 0f;
            _sessionState = State.IDLE;
        }
        public void AddPoint(float value)
        {
            _points += value;
        }

    }
}
