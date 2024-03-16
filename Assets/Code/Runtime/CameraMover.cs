using UnityEngine;

namespace Runtime
{
    public sealed class CameraMover : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;
        [SerializeField]
        private Vector3 padding;
        [SerializeField]
        private Transform target;

        private void Update()
        {
            cam.transform.position = target.position + padding;
            cam.transform.LookAt(target);
        }
    }
}