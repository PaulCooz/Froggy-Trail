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

        public void Setup(GameLoop loop, ICellHandler handler)
        {
            _material             = Instantiate(meshRenderer.material);
            meshRenderer.material = _material;

            _handler = handler;
            _handler.Setup(loop, this);
        }

        public void SetColor(Color color)
        {
            _material.SetColor(ColorProp, color);
        }

        public void FrogMoved(bool isJumpedOnCell)
        {
            _handler.FrogMoved(isJumpedOnCell);
        }
    }
}