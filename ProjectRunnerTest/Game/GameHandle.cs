using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Engine.StateManager;

namespace MonoEditorEndless.Game
{
    internal class GameHandle
    {
        private FSM _fsm;
        private bool _isPlaying;
        private ContentManager _contentManger;
        private GraphicsDevice _graphicsDevice;
        public GameHandle(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _isPlaying = false;
            _contentManger = content;
            _graphicsDevice = graphicsDevice;

            _fsm = new FSM(this);

            State play = new StatePlay(_contentManger, _graphicsDevice);
            play.Name = "play";



            _fsm.AddState(play);

            _fsm.Initialise("play");

        }
        public void Start()
        {
            _isPlaying = true;

        }
        public void Stop()
        {
            _isPlaying = false;
        }
        public void Update(GameTime gameTime)
        {
            if (_isPlaying)
            {
                _fsm.Update(gameTime);
            }
        }
        public void Draw(GraphicsDevice graphicsDevice)
        {
            _fsm.GetCurrentState().Draw(graphicsDevice);
        }
    }
}
