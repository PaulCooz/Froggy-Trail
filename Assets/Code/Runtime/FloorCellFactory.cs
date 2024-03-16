using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class FloorCellFactory
    {
        private readonly FloorCell       _prefab;
        private readonly Transform       _parent;
        private readonly List<FloorCell> _cells;

        public FloorCellFactory(FloorCell prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _cells  = new List<FloorCell>();
        }

        public FloorCell Get()
        {
            var cell = Object.Instantiate(_prefab, _parent);
            _cells.Add(cell);
            return cell;
        }

        public void DestroyAll()
        {
            foreach (var cell in _cells)
                Object.Destroy(cell.gameObject);
            _cells.Clear();
        }
    }
}