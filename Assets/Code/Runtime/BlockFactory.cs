using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class BlockFactory
    {
        private readonly Block      _prefab;
        private readonly Transform _parent;

        public List<Block> Spawned { get; private set; }

        public BlockFactory(Block prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            Spawned = new List<Block>();
        }

        public Block Get(LevelState state, bool isWinCell)
        {
            var cell = Object.Instantiate(_prefab, _parent);
            cell.Setup(state, isWinCell ? new GameWinCellHandler() : new CommonCellHandler());
            Spawned.Add(cell);
            return cell;
        }

        public void Destroy(Block block)
        {
            Spawned.Remove(block);
            Object.Destroy(block.gameObject);
        }

        public void DestroyAll()
        {
            foreach (var cell in Spawned.ToArray())
                Destroy(cell);
        }
    }
}