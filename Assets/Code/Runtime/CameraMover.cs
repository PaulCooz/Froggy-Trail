using UnityEngine;

namespace Runtime
{
    public sealed class CameraMover : MonoBehaviour, IStartListener
    {
        private Transform _player;

        [SerializeField]
        private Camera cam;
        [SerializeField]
        private Vector3 padding;

        public void GameStart(LevelState state)
        {
            _player = state.Player.transform;
        }

        private void Update()
        {
            cam.transform.position = _player.position + padding;
            cam.transform.LookAt(_player);
        }
    }
}