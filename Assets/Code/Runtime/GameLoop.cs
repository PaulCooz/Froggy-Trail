using System;
using System.Linq;
using UnityEngine;

namespace Runtime
{
    public sealed class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private Transform contentTransform;
        [SerializeField]
        private Frog frog;

        private bool _enableInput;

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

            frog.SetTrail(trailMaker);

            var level = new LevelState(trailMaker, frog);
            foreach (IStartListener listener in FindObjectsOfType<MonoBehaviour>(true).Where(mb => mb is IStartListener))
            {
                listener.GameStart(level);
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
            OnGameOver?.Invoke();
        }
    }
}