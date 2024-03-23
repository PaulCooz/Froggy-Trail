using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class Trail : IEnumerable<Vector3>, IDisposable
    {
        private readonly int   _seed;
        private readonly float _cellSize;

        private bool _disposed;

        public Trail(float cellSize, int seed)
        {
            _seed     = seed;
            _cellSize = cellSize;
            _disposed = false;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            var current = new Vector3(0, 0, 0);
            var rand    = new System.Random(_seed);
            while (!_disposed)
            {
                yield return current;

                current += new Vector3(_cellSize * rand.Next(2, 9), 0, 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() => _disposed = true;
    }
}