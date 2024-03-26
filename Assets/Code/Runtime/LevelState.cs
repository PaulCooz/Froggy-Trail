using System.Linq;
using UnityEngine;

namespace Runtime
{
    public sealed class LevelState
    {
        private readonly float _cellSize;

        public int   CellCount { get; private set; }
        public Trail Trail     { get; private set; }
        public Frog  Player    { get; private set; }

        public Vector2 MinMaxInputDuration { get; private set; }

        public LevelState(float cellSize, int count, Trail trail, Frog player, Vector2 minMaxInputDuration)
        {
            _cellSize = cellSize;

            CellCount = count;
            Trail     = trail;
            Player    = player;

            MinMaxInputDuration = minMaxInputDuration;
        }

        public bool IsNotOnCell(Vector3 pos)
        {
            var (left, right) = FindAdjacentPosFromPlayer();
            return pos.x - left.x >= _cellSize && right.x - pos.x >= _cellSize;
        }

        private (Vector3 left, Vector3 right) FindAdjacentPosFromPlayer()
        {
            var posX = Player.transform.position.x;
            var left = Trail.First();

            using var en = Trail.GetEnumerator();
            while (en.MoveNext() && en.Current.x <= posX)
                left = en.Current;

            var right = en.Current;

            return (left, right);
        }
    }
}