using System;
using System.Linq;
using UnityEngine;

namespace Runtime
{
    public sealed class GamePipeline : MonoBehaviour
    {
        [SerializeField]
        private Transform contentTransform;
        [SerializeField]
        private Frog frogPrefab;

        private FrogFactory _frogFactory;
        private bool        _enableInput;

        public event Action<float> OnChargingJumpInput;
        public event Action<float> OnJumpInput;
        public event Action        OnJumpDone;
        public event Action        OnGameOver;

        private void Start()
        {
            var trailMaker = new Trail(0);

            _frogFactory = new FrogFactory(frogPrefab, contentTransform, this, trailMaker);
            var player = _frogFactory.Get();

            var level = new LevelState(trailMaker, player);
            foreach (IStartListener listener in FindObjectsOfType<MonoBehaviour>(true).Where(mb => mb is IStartListener))
            {
                listener.GameStart(level);
            }

            _enableInput = true;
        }

        public void Restart()
        {
            _frogFactory.Clear();
            foreach (Transform item in contentTransform)
            {
                Destroy(item.gameObject);
            }

            Start();
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