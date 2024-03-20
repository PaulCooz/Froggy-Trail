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

        int IStartListener.Order => -1;

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

                if (transform is null || !transform)
                {
                    Debug.Log(gameLoop.RunLoopToken.IsCancellationRequested);
                    Debug.Log(gameLoop.RunLoopToken.CanBeCanceled);
                    Debug.Log(gameLoop.RunLoopToken.ToString());
                }
                transform.position = pos;

                await UniTask.NextFrame(gameLoop.RunLoopToken);

                t += Time.deltaTime;
            }

            transform.position = endPos;
            gameLoop.CheckGameOver();
        }

        public void OnDestroy()
        {
            gameLoop.OnChargingJumpInput -= UpdateNextPos;
            gameLoop.OnJumpInput         -= SetNextPos;
        }
    }
}