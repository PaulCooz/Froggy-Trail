using UnityEngine;

namespace Runtime
{
    public sealed class Block : MonoBehaviour
    {
        private static readonly int ColorProp = Shader.PropertyToID("_Color");

        private ICellHandler _handler;
        private Material     _material;

        [SerializeField]
        private MeshRenderer meshRenderer;

        public void Setup(LevelState level, ICellHandler handler)
        {
            _material             = Instantiate(meshRenderer.material);
            meshRenderer.material = _material;

            _handler = handler;
            _handler.Setup(level, this);
        }

        public void SetColor(Color color)
        {
            _material.SetColor(ColorProp, color);
        }
    }
}