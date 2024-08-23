using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Engine;
using MonoEditorEndless.Engine.StateManager;
using System;
using System.Threading.Tasks;

namespace MonoEditorEndless.Game
{
    internal class GameHandle
    {
        private FSM _fsm;
        private bool _isPlaying;
        private bool _isFromStartPlaying;
        private bool _isFinish;
        private ContentManager _contentManger;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        public SessionArgs _sessionArgs;

        public event EventHandler ExitGame;
        public GameHandle(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _isPlaying = false;
            _isFromStartPlaying = false;
            _isFinish = false;
            _contentManger = content;
            _graphicsDevice = graphicsDevice;

            _spriteBatch = new SpriteBatch(_graphicsDevice);


            _fsm = new FSM(this);

            StatePlay play = new StatePlay(_contentManger, _graphicsDevice);
            play.SessionFinished += (object sender, SessionArgs e) => { _isFinish = true; _sessionArgs = e; };
            StateMenu menu = new StateMenu(_contentManger, _graphicsDevice);
            menu.GameStart += (object sender, EventArgs e) => { _isPlaying = true; };
            StateMenuMaker menuMaker = new StateMenuMaker(_contentManger, _graphicsDevice);
            StateFinish finish = new StateFinish(_contentManger, _graphicsDevice, _sessionArgs);
            // In the editor is should go to spectate mode in the actual game it should call game exit
            finish.ExitGame += (object sender, EventArgs e) =>
            {
                _isPlaying = false;
            };

            StateSpectate spectate = new StateSpectate(_contentManger, _graphicsDevice);

            spectate.AddTransition(new Transition(play, () => { return _isPlaying; }));
            spectate.AddTransition(new Transition(menu, () => { return _isFromStartPlaying; }));
            play.AddTransition(new Transition(finish, () => { return _isFinish; }));
            play.AddTransition(new Transition(spectate, () => { return !_isPlaying; }));
            finish.AddTransition(new Transition(spectate, () => { return !_isPlaying; }));
            menu.AddTransition(new Transition(play, () => { return _isPlaying; }));
            menu.AddTransition(new Transition(spectate, () => { return !_isFromStartPlaying; }));

            _fsm.AddState(play);
            _fsm.AddState(spectate);
            _fsm.AddState(menu);
            _fsm.AddState(menuMaker);
            _fsm.AddState(finish);

            _fsm.Initialise("spectate");

        }
        public void Start(bool isFromStart = false)
        {
            if (isFromStart)
                _isFromStartPlaying = true;
            else
                _isPlaying = true;

        }
        async public void Restart(bool isFromStart = false)
        {
            _isPlaying = false;
            _isFromStartPlaying = false;
            await Task.Delay(1000);
            // TODO: Showing Loading here
            if (isFromStart)
                _isFromStartPlaying = true;
            else
                _isPlaying = true;


        }
        public void Stop()
        {
            _isPlaying = false;
            _isFromStartPlaying = false;
            //_fsm.Initialise("spectate");
        }
        public void Update(GameTime gameTime)
        {
            _fsm.Update(gameTime);
        }
        public void Draw(GraphicsDevice graphicsDevice)
        {
            _fsm.GetCurrentState().Draw(_graphicsDevice, _spriteBatch);
        }
    }
}
