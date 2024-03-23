using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Runtime
{
    public sealed class InputListener : MonoBehaviour, IStartListener
    {
        [SerializeField]
        private GameLoop gameLoop;
        [SerializeField]
        private InputAction pressAction;

        private float _span;
        private float _maxDuration;
        private bool  _pressing;

        public void GameStart(LevelState state)
        {
            pressAction.started += _ =>
            {
                _span        = 0;
                _maxDuration = Random.Range(state.MinMaxInputDuration.x, state.MinMaxInputDuration.y);

                _pressing = true;
            };
            pressAction.canceled += _ =>
            {
                _pressing = false;

                gameLoop.InvokeJumpInput(Mathf.Clamp01(_span / _maxDuration));
            };
        }

        private void Update()
        {
            if (_pressing)
            {
                gameLoop.InvokeChargingJumpInput(Mathf.Clamp01(_span / _maxDuration));
                _span += Time.deltaTime;
            }
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