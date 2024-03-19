using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class Trail : IEnumerable<Vector3>, IDisposable
    {
        public const float CellSize = 2.7f;

        private readonly int _seed;

        private bool _disposed;

        public Trail(int seed)
        {
            _seed     = seed;
            _disposed = false;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            var current = new Vector3(0, 0, 0);
            var rand    = new System.Random(_seed);
            while (!_disposed)
            {
                yield return current;

                current += new Vector3(CellSize * rand.Next(2, 6), 0, 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            _disposed = true;
        }
    }
}