using System.Linq;
using UnityEngine;

namespace Runtime
{
    public sealed class LevelState
    {
        public Trail Trail  { get; private set; }
        public Frog  Player { get; private set; }

        public Vector2 MinMaxInputDuration { get; private set; }

        public LevelState(Trail trail, Frog player, Vector2 minMaxInputDuration)
        {
            Trail               = trail;
            Player              = player;
            MinMaxInputDuration = minMaxInputDuration;
        }

        public Vector3 FindLeftCellPos()
        {
            var left = Trail.First();
            var pos  = Player.transform.position;
            foreach (var t in Trail)
            {
                if (t.x > pos.x)
                    break;
                left = t;
            }
            return left;
        }

        public Vector3 FindRightCellPos()
        {
            var right = Trail.First();
            var pos   = Player.transform.position;
            foreach (var t in Trail)
            {
                right = t;
                if (t.x >= pos.x)
                    break;
            }
            return right;
        }
    }
}