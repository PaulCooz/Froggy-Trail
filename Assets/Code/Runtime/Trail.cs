using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class Trail : IEnumerable<Vector3>
    {
        private readonly int _seed;

        public Trail(int seed)
        {
            _seed = seed;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            var current = new Vector3(0, 0, 0);
            var rand    = new System.Random(_seed);
            while (true)
            {
                yield return current;

                current += new Vector3(rand.Next(3, 6), 0, 0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}