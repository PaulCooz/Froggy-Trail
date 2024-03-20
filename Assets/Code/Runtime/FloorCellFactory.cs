using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class FloorCellFactory
    {
        private readonly FloorCell _prefab;
        private readonly Transform _parent;

        public List<FloorCell> Spawned { get; private set; }

        public FloorCellFactory(FloorCell prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            Spawned = new List<FloorCell>();
        }

        public FloorCell Get()
        {
            var cell = Object.Instantiate(_prefab, _parent);
            Spawned.Add(cell);
            return cell;
        }

        public void Destroy(FloorCell cell)
        {
            Spawned.Remove(cell);
            Object.Destroy(cell.gameObject);
        }

        public void DestroyAll()
        {
            foreach (var cell in Spawned.ToArray())
                Destroy(cell);
        }
    }
}