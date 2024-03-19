using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public sealed class Frog : MonoBehaviour, IStartListener
    {
        [SerializeField]
        private float maxJumpDist;
        [SerializeField]
        private AnimationCurve jumpCurve;
        [SerializeField]
        private float jumpHeight;
        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        private GameLoop gameLoop;

        private Trail _trail;

        public void SubscribeToEvents()
        {
            gameLoop.OnChargingJumpInput += UpdateNextPos;
            gameLoop.OnJumpInput         += SetNextPos;
        }

        public void GameStart(LevelState state)
        {
            _trail = state.Trail;
            var pos = _trail.First();
            pos.y += 1;

            transform.position = pos;
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
                var t      = i / (points - 1f);
                var pos    = Vector3.Lerp(start, end, t);
                var height = force * jumpHeight;
                pos.y = start.y + height * jumpCurve.Evaluate(t);
                positions.Add(pos);
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetAlpha(1);
            lineRenderer.SetPositions(positions.ToArray());
        }

        private void SetNextPos(float force)
        {
            lineRenderer.TurnAlpha(0, 0.3f).Forget();
            MoveAsync(force).Forget();
        }

        private async UniTaskVoid MoveAsync(float force)
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

                await UniTask.NextFrame(); // TODO stop on game over

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

            const float cellSizeX = Trail.CellSize;
            if (transform.position.x - left.x >= cellSizeX && right.x - transform.position.x >= cellSizeX)
                gameLoop.InvokeGameOver();
            else
                gameLoop.InvokeJumpDone();
        }

        public void OnDestroy()
        {
            gameLoop.OnChargingJumpInput -= UpdateNextPos;
            gameLoop.OnJumpInput         -= SetNextPos;
        }
    }
}