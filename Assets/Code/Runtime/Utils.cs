using UnityEngine;

namespace Runtime
{
    public static class Utils
    {
        public static void SetAlpha(this LineRenderer lineRenderer, float value)
        {
            var color = lineRenderer.startColor;
            color.a = value;

            lineRenderer.startColor = lineRenderer.endColor = color;
        }
    }
}