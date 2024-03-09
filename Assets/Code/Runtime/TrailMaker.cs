using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public sealed class TrailMaker : MonoBehaviour, IStartListener
    {
        [SerializeField]
        private GamePipeline gamePipeline;
        [SerializeField]
        private FloorCell cellPrefab;
        [SerializeField]
        private Transform contentTransform;

        private bool _playerMoved;

        public void GameStart(LevelState state)
        {
            gamePipeline.OnJumpInput += _ => _playerMoved = true;

            StartCoroutine(MakeTrail(state.Trail));
        }

        private IEnumerator<CustomYieldInstruction> MakeTrail(Trail trail)
        {
            var floorCellFactory = new FloorCellFactory(cellPrefab, contentTransform);
            var startCells       = 3;
            foreach (var pos in trail)
            {
                var cell = floorCellFactory.Get();
                cell.transform.position = pos;

                _playerMoved = false;
                if (--startCells > 0)
                    continue;

                yield return new WaitWhile(() => !_playerMoved);
            }
        }
    }
}