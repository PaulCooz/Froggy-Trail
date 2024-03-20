using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Runtime
{
    public sealed class InputListener : MonoBehaviour, IStartListener
    {
        private CancellationTokenSource _cancelSource;

        [SerializeField]
        private GameLoop gameLoop;
        [SerializeField]
        private InputAction pressAction;

        private float _span;
        private float _maxDuration;

        public void GameStart(LevelState state)
        {
            pressAction.started += _ =>
            {
                _span        = 0;
                _maxDuration = Random.Range(state.MinMaxInputDuration.x, state.MinMaxInputDuration.y);

                ClickingRoutine().Forget();
            };
            pressAction.canceled += _ =>
            {
                _cancelSource.Cancel();

                gameLoop.InvokeJumpInput(Mathf.Clamp01(_span / _maxDuration));
            };
        }

        private async UniTask ClickingRoutine()
        {
            _cancelSource = new CancellationTokenSource();
            while (isActiveAndEnabled)
            {
                gameLoop.InvokeChargingJumpInput(Mathf.Clamp01(_span / _maxDuration));

                await UniTask.NextFrame();

                if (_cancelSource.IsCancellationRequested)
                    return;

                _span += Time.deltaTime;
            }
            _cancelSource.Dispose();
        }

        private void OnEnable()
        {
            pressAction.Enable();
        }

        private void OnDisable()
        {
            pressAction.Disable();
        }
    }
}