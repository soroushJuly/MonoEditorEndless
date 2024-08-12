using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Editor
{
    internal class ControlsAggregator
    {
        public event EventHandler PausePressed;
        public event EventHandler PlayPressed;
        public event EventHandler PlayFromStartPressed;

        public void RaisePlayPressed()
        {
            PlayPressed?.Invoke(this, EventArgs.Empty);
        }
        public void RaisePlayFromStartPressed()
        {
            PlayFromStartPressed?.Invoke(this, EventArgs.Empty);
        }
        public void RaisePausePressed()
        {
            PausePressed?.Invoke(this, EventArgs.Empty);
        }
    }
}
