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
        private Block blockPrefab;
        [SerializeField]
        private Transform contentTransform;
        [SerializeField]
        private float minChangeDist;

        private BlockFactory _blockFactory;

        private bool       _playerMoved;
        private LevelState _state;

        public void GameStart(LevelState state)
        {
            _state = state;

            gameLoop.OnJumpInput += _ => _playerMoved = true;
            gameLoop.OnFrogMoved += RefreshBlocks;
            gameLoop.OnGameOver  += OnGameOver;

            _blockFactory = new BlockFactory(gameLoop, blockPrefab, contentTransform);

            MakeTrail(_state.Trail, () => _state.Player.transform.position).Forget();
        }

        private async UniTask MakeTrail(Trail trail, Func<Vector3> currPosGetter)
        {
            var i = 0;
            foreach (var pos in trail)
            {
                var cell = _blockFactory.Get(i == _state.CellCount - 1);
                cell.transform.position = pos;
                i++;

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
            var distant = _blockFactory.Spawned
                .Where(cell => current.x - cell.transform.position.x >= minChangeDist)
                .ToArray();

            foreach (var cell in distant)
                _blockFactory.Destroy(cell);
        }

        private void RefreshBlocks()
        {
            foreach (var block in _blockFactory.Spawned)
            {
                var isJumpedOnCell = _state.IsFrogTouchBlock(block.transform.position);
                block.FrogMoved(isJumpedOnCell);
            }
        }

        private void OnGameOver()
        {
            _blockFactory.DestroyAll();
        }
    }
}