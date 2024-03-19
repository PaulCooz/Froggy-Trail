using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public static class Turns
    {
        public static UniTask TurnAlpha(this LineRenderer lineRenderer, float endValue, float duration)
        {
            return Turn(
                () => lineRenderer.startColor.a,
                a =>
                {
                    var color = lineRenderer.startColor;
                    color.a = a;

                    lineRenderer.startColor = color;
                    lineRenderer.endColor   = color;
                },
                endValue,
                duration,
                Mathf.Lerp
            );
        }

        private static async UniTask Turn<T>(Func<T> getter, Action<T> setter, T endVal, float dur, Func<T, T, float, T> lerp)
        {
            var time       = 0f;
            var startValue = getter();
            while (time < dur)
            {
                setter(lerp(startValue, endVal, time / dur));
                await UniTask.NextFrame();
                time += Time.deltaTime;
            }
            setter(lerp(startValue, endVal, 1f));
        }
    }
}