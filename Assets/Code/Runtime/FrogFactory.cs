using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime
{
    public sealed class FrogFactory
    {
        private readonly Frog      _prefab;
        private readonly Transform _parent;

        private readonly GamePipeline _gamePipeline;
        private readonly Trail        _trail;

        private readonly List<Frog> _frogs;

        public FrogFactory(Frog prefab, Transform parent, GamePipeline gamePipeline, Trail trail)
        {
            _prefab = prefab;
            _parent = parent;

            _gamePipeline = gamePipeline;
            _trail        = trail;

            _frogs = new List<Frog>();
        }

        public Frog Get()
        {
            var frog = Object.Instantiate(_prefab, _parent);
            frog.Setup(_gamePipeline, _trail);
            _frogs.Add(frog);
            return frog;
        }

        public void Clear()
        {
            foreach (var frog in _frogs)
                frog.Dispose();
            _frogs.Clear();
        }
    }
}