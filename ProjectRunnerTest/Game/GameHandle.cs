using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoEditorEndless.Engine;
using MonoEditorEndless.Engine.StateManager;
using ProjectRunnerTest;
using System;
using System.Threading.Tasks;

namespace MonoEditorEndless.Game
{
    public class GameHandle
    {
        private FSM _fsm;
        private bool _isPlaying;
        private bool _isSpectate;
        private bool _isHUDMaking;
        private bool _isMenuMaking;
        private bool _isFromStartPlaying;
        private bool _isFinish;
        private ContentManager _contentManger;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        public SessionArgs _sessionArgs;

        public event EventHandler ExitGame;
        public event EventHandler RaisePause;
        public GameHandle(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _isFinish = false;
            _isPlaying = false;
            _isFromStartPlaying = false;
            //_isGameRestart = false;

            _isSpectate = false;
            _isHUDMaking = false;
            _isMenuMaking = false;
            _contentManger = content;
            _graphicsDevice = graphicsDevice;

            _spriteBatch = new SpriteBatch(_graphicsDevice);


            _fsm = new FSM(this);

            StatePlay play = new StatePlay(_contentManger, _graphicsDevice);
            play.SessionFinished += (object sender, SessionArgs e) => { _isFinish = true; _sessionArgs = e; };
            StateMenu menu = new StateMenu(_contentManger, _graphicsDevice);
            menu.GameStart += (object sender, EventArgs e) => { _isPlaying = true; };
            menu.ExitGame += (object sender, EventArgs e) =>
            {
                if (Application._isDebug)
                {
                    _isPlaying = false;
                    _isFinish = false;
                    _isSpectate = true;
                    RaisePause?.Invoke(this, EventArgs.Empty);
                }
                else Environment.Exit(0);
            };
            StateMenuMaker menuMaker = new StateMenuMaker(_contentManger, _graphicsDevice);
            StateHUDMaker hudMaker = new StateHUDMaker(_contentManger, _graphicsDevice);
            StateFinish finish = new StateFinish(_contentManger, _graphicsDevice, _sessionArgs);
            // In the editor is should go to spectate mode in the actual game it should call game exit
            finish.ExitGame += (object sender, EventArgs e) =>
            {
                if (Application._isDebug)
                {
                    _isPlaying = false;
                    _isFinish = false;
                    _isSpectate = true;
                    RaisePause?.Invoke(this, EventArgs.Empty);
                }
                else Environment.Exit(0);
            };
            finish.GameRestart += (object sender, EventArgs e) =>
            {
                _isFinish = false;
                _isSpectate = false;
                _isPlaying = true;
            };
            finish.ToMenu += (object sender, EventArgs e) =>
            {
                _isPlaying = false;
                _isSpectate = false;
                _isFinish = false;
                _isFromStartPlaying = true;
            };

            StateSpectate spectate = new StateSpectate(_contentManger, _graphicsDevice);

            spectate.AddTransition(new Transition(play, () => { return _isPlaying; }));
            spectate.AddTransition(new Transition(menu, () => { return _isFromStartPlaying; }));
            spectate.AddTransition(new Transition(menuMaker, () => { return _isMenuMaking; }));
            spectate.AddTransition(new Transition(hudMaker, () => { return _isHUDMaking; }));
            hudMaker.AddTransition(new Transition(spectate, () => { return _isSpectate; }));
            hudMaker.AddTransition(new Transition(menuMaker, () => { return _isMenuMaking; }));
            hudMaker.AddTransition(new Transition(play, () => { return _isPlaying; }));
            hudMaker.AddTransition(new Transition(play, () => { return _isFromStartPlaying; }));
            menuMaker.AddTransition(new Transition(spectate, () => { return _isSpectate; }));
            menuMaker.AddTransition(new Transition(hudMaker, () => { return _isHUDMaking; }));
            menuMaker.AddTransition(new Transition(play, () => { return _isPlaying; }));
            menuMaker.AddTransition(new Transition(menu, () => { return _isFromStartPlaying; }));
            play.AddTransition(new Transition(finish, () => { return _isFinish; }));
            play.AddTransition(new Transition(spectate, () => { return !_isPlaying && _isSpectate; }));
            play.AddTransition(new Transition(hudMaker, () => { return !_isPlaying && _isHUDMaking; }));
            play.AddTransition(new Transition(menuMaker, () => { return !_isPlaying && _isMenuMaking; }));
            finish.AddTransition(new Transition(spectate, () => { return !_isFinish && !_isPlaying && _isSpectate; }));
            finish.AddTransition(new Transition(play, () => { return !_isFinish && _isPlaying; }));
            finish.AddTransition(new Transition(menu, () => { return !_isPlaying && _isFromStartPlaying && !_isSpectate; }));
            menu.AddTransition(new Transition(play, () => { return _isPlaying; }));
            menu.AddTransition(new Transition(spectate, () => { return !_isFromStartPlaying && _isSpectate; }));
            menu.AddTransition(new Transition(hudMaker, () => { return !_isFromStartPlaying && _isHUDMaking; }));
            menu.AddTransition(new Transition(menuMaker, () => { return !_isFromStartPlaying && _isMenuMaking; }));

            _fsm.AddState(play);
            _fsm.AddState(spectate);
            _fsm.AddState(menu);
            _fsm.AddState(menuMaker);
            _fsm.AddState(hudMaker);
            _fsm.AddState(finish);

            if (Application._isDebug)
            {
                _isSpectate = true;
                _fsm.Initialise("spectate");
            }
            else
            {
                _fsm.Initialise("menu");
            }

        }
        public void Start(bool isFromStart = false)
        {
            if (isFromStart)
                _isFromStartPlaying = true;
            else
                _isPlaying = true;
        }
        public void MenuMaker()
        {
            _isSpectate = false;
            _isHUDMaking = false;
            _isMenuMaking = true;
        }
        public void HUDMaker()
        {
            _isSpectate = false;
            _isMenuMaking = false;
            _isHUDMaking = true;
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
        public void Refresh()
        {
            _fsm.GetCurrentState().Exit(this);
            _fsm.GetCurrentState().Enter(this);
        }
        public void Stop()
        {
            _isPlaying = false;
            _isFinish = false;
            _isFromStartPlaying = false;
        }
        public void Spectate()
        {
            _isMenuMaking = false;
            _isHUDMaking = false;
            _isSpectate = true;
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
