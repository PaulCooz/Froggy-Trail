using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Runtime
{
    public sealed class InputListener : MonoBehaviour, IStartListener
    {
        private Coroutine _clickingRoutine;

        [SerializeField]
        private GamePipeline gamePipeline;
        [SerializeField]
        private InputAction pressAction;

        private float _span;
        private float _length;

        public void GameStart(LevelState state)
        {
            pressAction.started += _ =>
            {
                _span   = 0;
                _length = Random.Range(0.5f, 1.5f);

                _clickingRoutine = StartCoroutine(ClickingRoutine());
            };
            pressAction.canceled += _ =>
            {
                StopCoroutine(_clickingRoutine);

                gamePipeline.InvokeJumpInput(Mathf.Clamp01(_span / _length));
            };
        }

        private IEnumerator<WaitForSeconds> ClickingRoutine()
        {
            while (isActiveAndEnabled)
            {
                gamePipeline.InvokeChargingJumpInput(Mathf.Clamp01(_span / _length));

                yield return null;

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