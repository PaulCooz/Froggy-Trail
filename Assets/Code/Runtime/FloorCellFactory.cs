using UnityEngine;

namespace Runtime
{
    public sealed class FloorCellFactory
    {
        private readonly FloorCell _prefab;
        private readonly Transform _parent;

        public FloorCellFactory(FloorCell prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public FloorCell Get()
        {
            var cell = Object.Instantiate(_prefab, _parent);
            return cell;
        }
    }
}