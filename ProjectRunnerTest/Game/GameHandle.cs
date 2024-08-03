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
        private SpriteBatch _spriteBatch;
        public GameHandle(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _isPlaying = false;
            _contentManger = content;
            _graphicsDevice = graphicsDevice;

            _spriteBatch = new SpriteBatch(_graphicsDevice);


            _fsm = new FSM(this);

            State play = new StatePlay(_contentManger, _graphicsDevice);
            State menu = new StateMenu(_contentManger, _graphicsDevice);
            State menuMaker = new StateMenuMaker(_contentManger, _graphicsDevice);

            State spectate = new StateSpectate(_contentManger, _graphicsDevice);

            spectate.AddTransition(new Transition(play, () => { return _isPlaying; }));

            _fsm.AddState(play);
            _fsm.AddState(spectate);
            _fsm.AddState(menu);
            _fsm.AddState(menuMaker);

            _fsm.Initialise("menu-maker");

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

            _fsm.Update(gameTime);

        }
        public void Draw(GraphicsDevice graphicsDevice)
        {
            _fsm.GetCurrentState().Draw(graphicsDevice, _spriteBatch);
        }
    }
}
