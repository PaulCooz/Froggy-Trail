using UnityEngine;

namespace Runtime
{
    public sealed class GameWinCellHandler : ICellHandler
    {
        public void Setup(LevelState level, Block block)
        {
            block.SetColor(Color.yellow);
        }
    }
}