using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public sealed class TrailMaker : MonoBehaviour, IStartListener
    {
        private FloorCellFactory _floorCellFactory;

        [SerializeField]
        private GameLoop gameLoop;
        [SerializeField]
        private FloorCell cellPrefab;
        [SerializeField]
        private Transform contentTransform;

        private bool _playerMoved;

        public void GameStart(LevelState state)
        {
            gameLoop.OnJumpInput += _ => _playerMoved = true;
            gameLoop.OnGameOver  += OnGameOver;

            _floorCellFactory = new FloorCellFactory(cellPrefab, contentTransform);

            MakeTrail(state.Trail).Forget();
        }

        private async UniTask MakeTrail(Trail trail)
        {
            var startCells = 3;
            foreach (var pos in trail)
            {
                var cell = _floorCellFactory.Get();
                cell.transform.position = pos;

                _playerMoved = false;
                if (--startCells > 0)
                    continue;

                await UniTask.WaitWhile(() => !_playerMoved);
            }
        }

        private void OnGameOver()
        {
            _floorCellFactory.DestroyAll();
        }
    }
}