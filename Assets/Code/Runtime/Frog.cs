using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime
{
    public sealed class Frog : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private float maxJumpDist;
        [SerializeField]
        private AnimationCurve jumpCurve;
        [SerializeField]
        private float jumpHeight;
        [SerializeField]
        private LineRenderer lineRenderer;

        private Trail        _trail;
        private GamePipeline _gamePipeline;

        public void Setup(GamePipeline gamePipeline, Trail trail)
        {
            _gamePipeline = gamePipeline;

            _trail = trail;
            var pos = _trail.First();
            pos.y += 1;

            transform.position = pos;

            _gamePipeline.OnChargingJumpInput += UpdateNextPos;
            _gamePipeline.OnJumpInput         += SetNextPos;
        }

        public void Dispose()
        {
            _gamePipeline.OnChargingJumpInput -= UpdateNextPos;
            _gamePipeline.OnJumpInput         -= SetNextPos;
        }

        private void UpdateNextPos(float force)
        {
            var start = transform.position;
            start.y -= 0.4f;
            var end = start + new Vector3(maxJumpDist * force, 0, 0);

            var positions = new List<Vector3>();

            const int points = 10;
            for (var i = 0; i < points; i++)
            {
                var t   = i / (points - 1f);
                var pos = Vector3.Lerp(start, end, t);
                pos.y = start.y + jumpHeight * jumpCurve.Evaluate(t);
                positions.Add(pos);
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }

        private void SetNextPos(float force)
        {
            StartCoroutine(MoveRoutine(force));
        }

        private IEnumerator<YieldInstruction> MoveRoutine(float force)
        {
            var startPos = transform.position;

            var endPos = transform.position;
            endPos.x += maxJumpDist * force;

            var t = 0f;
            while (t < 1f)
            {
                var pos = Vector3.Lerp(startPos, endPos, t);
                pos.y = startPos.y + jumpHeight * jumpCurve.Evaluate(t);

                transform.position = pos;

                yield return null;

                t += Time.deltaTime;
            }

            transform.position = endPos;
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            var left = _trail.First();
            foreach (var t in _trail)
            {
                if (t.x > transform.position.x)
                    break;
                left = t;
            }

            var right = _trail.First();
            foreach (var t in _trail)
            {
                right = t;
                if (t.x >= transform.position.x)
                    break;
            }

            if (transform.position.x - left.x >= 1 && right.x - transform.position.x >= 1)
            {
                _gamePipeline.InvokeGameOver();
            }
            else
            {
                _gamePipeline.InvokeJumpDone();
            }
        }
    }
}