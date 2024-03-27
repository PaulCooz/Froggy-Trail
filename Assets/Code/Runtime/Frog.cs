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

        private Trail      _trail;
        private LevelState _state;

        int IStartListener.Order => -1;

        public void SubscribeToEvents()
        {
            gameLoop.OnChargingJumpInput += UpdateNextPos;
            gameLoop.OnJumpInput         += SetNextPos;
        }

        public void GameStart(LevelState state)
        {
            _state = state;
            _trail = _state.Trail;
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
                var t   = i / (points - 1f);
                var pos = GetArcPos(force, start, end, t);
                positions.Add(pos);
            }

            if (_state.IsBetweenBlocks(end))
                AddFallRay(force, start, end, positions);

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetAlpha(1);
            lineRenderer.SetPositions(positions.ToArray());
        }

        private void AddFallRay(float force, Vector3 start, Vector3 end, ICollection<Vector3> positions)
        {
            var last = GetArcPos(force, start, end, 1);
            var prev = GetArcPos(force, start, end, 0.99f);
            var ray  = new Ray(last, last - prev);
            positions.Add(ray.GetPoint(0));
            positions.Add(ray.GetPoint(100));
        }

        private Vector3 GetArcPos(float force, Vector3 start, Vector3 end, float t)
        {
            var pos    = Vector3.Lerp(start, end, t);
            var height = force * jumpHeight;
            pos.y = start.y + height * jumpCurve.Evaluate(t);
            return pos;
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

                await UniTask.NextFrame(gameLoop.RunLoopToken);

                t += Time.deltaTime;
            }

            transform.position = endPos;
            gameLoop.InvokeFrogMoved();
        }

        public void OnDestroy()
        {
            gameLoop.OnChargingJumpInput -= UpdateNextPos;
            gameLoop.OnJumpInput         -= SetNextPos;
        }
    }
}