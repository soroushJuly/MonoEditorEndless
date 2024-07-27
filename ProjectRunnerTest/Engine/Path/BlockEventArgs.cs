using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEditorEndless.Engine.Path
{
    internal class BlockEventArgs : EventArgs
    {
        private Block _block;
        public Block GetBlock() { return _block; }

        public BlockEventArgs(Block block)
        {
            _block = block;
        }
    }
}
