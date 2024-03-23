using UnityEngine;

namespace Runtime
{
    public sealed class CameraMover : MonoBehaviour, ISerializationCallbackReceiver
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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (cam && target)
                Update();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}