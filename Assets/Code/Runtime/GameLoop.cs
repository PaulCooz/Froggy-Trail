using System;
using System.Linq;
using UnityEngine;

namespace Runtime
{
    public sealed class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private Frog frog;

        private LevelState _state;
        private bool       _enableInput;

        public event Action<float> OnChargingJumpInput;
        public event Action<float> OnJumpInput;
        public event Action        OnJumpDone;
        public event Action        OnGameOver;

        private void Awake()
        {
            frog.SubscribeToEvents();
            Setup();
        }

        private void Setup()
        {
            var trailMaker = new Trail(0);

            _state = new LevelState(trailMaker, frog);
            var startListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<IStartListener>();
            foreach (var listener in startListeners)
            {
                listener.GameStart(_state);
            }

            _enableInput = true;
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

        public void InvokeJumpDone()
        {
            OnJumpDone?.Invoke();
            _enableInput = true;
        }

        public void InvokeGameOver()
        {
            _state.Trail.Dispose();
            OnGameOver?.Invoke();
        }
    }
}