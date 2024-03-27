using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Runtime
{
    public sealed class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private Frog frog;
        [SerializeField]
        private Vector2 minMaxInputDuration;
        [SerializeField]
        private float cellSize = 2.7f;

        private LevelState _state;
        private bool       _enableInput;

        private CancellationTokenSource _loopCancellation;

        public CancellationToken RunLoopToken => _loopCancellation.Token;

        public event Action<float> OnChargingJumpInput;
        public event Action<float> OnJumpInput;
        public event Action        OnJumpDone;
        public event Action        OnFrogMoved;
        public event Action        OnGameOver;

        private void Awake()
        {
            frog.SubscribeToEvents();
            Setup();
        }

        private void Setup()
        {
            _loopCancellation = new CancellationTokenSource();

            const int maxCount = 5;

            var trailMaker = new Trail(cellSize, maxCount, 0);
            _state = new LevelState(cellSize, maxCount, trailMaker, frog, minMaxInputDuration);
            var startListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IStartListener>().ToList();
            startListeners.Sort((a, b) => a.Order.CompareTo(b.Order));
            foreach (var listener in startListeners)
                listener.GameStart(_state);

            _enableInput = true;
        }

        public void InvokeFrogMoved()
        {
            OnFrogMoved?.Invoke();
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            var playerPos   = _state.Player.transform.position;
            var isNotOnCell = _state.IsBetweenBlocks(playerPos);
            if (isNotOnCell)
                InvokeGameOver();
            else
                InvokeJumpDone();
        }

        public void Restart()
        {
            Setup();
        }

        public void InvokeChargingJumpInput(float force)
        {
            if (!_enableInput)
                return;
            OnChargingJumpInput?.Invoke(force);
        }

        public void InvokeJumpInput(float force)
        {
            if (!_enableInput)
                return;
            _enableInput = false;
            OnJumpInput?.Invoke(force);
        }

        private void InvokeJumpDone()
        {
            OnJumpDone?.Invoke();
            _enableInput = true;
        }

        public void InvokeLevelWin()
        {
            Debug.Log("you win!");
        }

        private void InvokeGameOver()
        {
            _loopCancellation.Cancel();
            _loopCancellation.Dispose();
            _loopCancellation = null;

            _state.Trail.Dispose();
            OnGameOver?.Invoke();
        }
    }
}