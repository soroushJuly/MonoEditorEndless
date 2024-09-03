using System;

namespace MonoEditorEndless.Editor
{
    public class ReplayEventArgs : EventArgs
    {
        private bool _isFromStart;
        public bool IsFromStart() { return _isFromStart; }
        public ReplayEventArgs(bool isFromStart)
        {
            _isFromStart = isFromStart;
        }
    }
    internal class ControlsAggregator
    {
        public event EventHandler PausePressed;
        public event EventHandler PlayPressed;
        public event EventHandler SpectatePressed;
        public event EventHandler HUDMakerPressed;
        public event EventHandler PlayFromStartPressed;
        public event EventHandler<ReplayEventArgs> RestartPressed;
        public event EventHandler MenuMakerPressed;
        public event EventHandler RefreshSpectate;
        // Is the last event fire from start or not
        public bool _isFromStart;

        public void RaisePlayPressed()
        {
            PlayPressed?.Invoke(this, EventArgs.Empty);
            _isFromStart = false;
        }
        public void RaisePlayFromStartPressed()
        {
            PlayFromStartPressed?.Invoke(this, EventArgs.Empty);
            _isFromStart = true;
        }
        public void RaiseLastEvent()
        {
            //if (_isFromStart)
            //{
            //    PausePressed?.Invoke(this, EventArgs.Empty);
            //    PlayFromStartPressed?.Invoke(this, EventArgs.Empty);
            //}
            //else
            //{
            RestartPressed?.Invoke(this, _isFromStart ? new ReplayEventArgs(true) : new ReplayEventArgs(false));
            //PlayPressed?.Invoke(this, EventArgs.Empty);
            _isFromStart = false;
            //}
        }
        public void RaisePausePressed()
        {
            PausePressed?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseSpectate()
        {
            SpectatePressed?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseMenuMaker()
        {
            MenuMakerPressed?.Invoke(this, EventArgs.Empty);
        }
        public void RaiseHUDMaker()
        {
            HUDMakerPressed?.Invoke(this, EventArgs.Empty);
        }
        // TODO: change the name of this method since we are not only refreshing the spectate
        public void RaiseRefreshSpectate()
        {
            RefreshSpectate?.Invoke(this, EventArgs.Empty);
        }
    }
}
