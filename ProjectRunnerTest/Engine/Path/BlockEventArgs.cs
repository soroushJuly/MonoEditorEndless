using System;

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
