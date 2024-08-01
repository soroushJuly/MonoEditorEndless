using Microsoft.Xna.Framework;

namespace MonoEditorEndless.Engine
{
    internal class GameSession
    {
        private float _sessionTimePassed;
        private float _points;
        private State _sessionState;
        private float _gameSpeed;
        private float _gameAccelerationRate;
        public enum State
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
        public float GetGameSpeed() { return _gameSpeed; }
        public State GetState() { return _sessionState; }

        public void StartSession()
        {
            _sessionState = State.START;
        }
        public void Update(GameTime gameTime)
        {
            if (_sessionState == State.STOP)
            {
                ResetSession();
            }
            if (_sessionState == State.START)
            {
                _sessionTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _gameSpeed += _gameAccelerationRate;
            }
            // If sessionState == PAUSE dont do anything
        }
        private void ResetSession()
        {
            _gameAccelerationRate = 0.0001f;
            _sessionTimePassed = 0f;
            _gameSpeed = 1f;
            _points = 0f;
            _sessionState = State.IDLE;
        }
        public void AddPoint(float value)
        {
            _points += value;
        }

    }
}
