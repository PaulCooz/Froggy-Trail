using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public sealed class TrailMaker : MonoBehaviour, IStartListener
    {
        [SerializeField]
        private GameLoop gameLoop;
        [SerializeField]
        private FloorCell cellPrefab;
        [SerializeField]
        private Transform contentTransform;
        [SerializeField]
        private float minChangeDist;

        private FloorCellFactory _floorCellFactory;
        private bool _playerMoved;

        public void GameStart(LevelState state)
        {
            gameLoop.OnJumpInput += _ => _playerMoved = true;
            gameLoop.OnGameOver  += OnGameOver;

            _floorCellFactory = new FloorCellFactory(cellPrefab, contentTransform);

            MakeTrail(state.Trail, () => state.Player.transform.position).Forget();
        }

        private async UniTask MakeTrail(Trail trail, Func<Vector3> currPosGetter)
        {
            foreach (var pos in trail)
            {
                var cell = _floorCellFactory.Get();
                cell.transform.position = pos;

                if (Vector3.Distance(currPosGetter(), pos) < minChangeDist)
                    continue;

                _playerMoved = false;
                await UniTask.WaitWhile(() => !_playerMoved, cancellationToken: gameLoop.RunLoopToken);
                if (gameLoop.RunLoopToken.IsCancellationRequested)
                    return;

                DestroyDistant(currPosGetter());
            }
        }

        private void DestroyDistant(Vector3 current)
        {
            var distant = _floorCellFactory.Spawned
                .Where(cell => current.x - cell.transform.position.x >= minChangeDist)
                .ToArray();

            foreach (var cell in distant)
                _floorCellFactory.Destroy(cell);
        }

        private void OnGameOver()
        {
            _floorCellFactory.DestroyAll();
        }
    }
}