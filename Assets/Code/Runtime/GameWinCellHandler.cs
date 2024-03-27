using UnityEngine;

namespace Runtime
{
    public sealed class GameWinCellHandler : ICellHandler
    {
        private GameLoop _loop;

        public void Setup(GameLoop loop, Block block)
        {
            _loop = loop;
            block.SetColor(Color.yellow);
        }

        public void FrogMoved(bool isJumpedOnCell)
        {
            if (isJumpedOnCell)
                _loop.InvokeLevelWin();
        }
    }
}