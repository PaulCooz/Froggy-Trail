using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class BlockFactory
    {
        private readonly Block     _prefab;
        private readonly Transform _parent;
        private readonly GameLoop  _loop;

        public List<Block> Spawned { get; private set; }

        public BlockFactory(GameLoop loop, Block prefab, Transform parent)
        {
            _loop   = loop;
            _prefab = prefab;
            _parent = parent;
            Spawned = new List<Block>();
        }

        public Block Get(bool isWinCell)
        {
            var cell = Object.Instantiate(_prefab, _parent);
            cell.Setup(_loop, isWinCell ? new GameWinCellHandler() : new CommonCellHandler());
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