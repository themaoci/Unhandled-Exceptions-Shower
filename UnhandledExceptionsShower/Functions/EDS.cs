using UnityEngine;

namespace UnhandledExceptionHandler.Functions
{
    public static class EDS
    {
        private static Texture2D _coloredLineTexture;
        private static Color _coloredLineColor;
        #region DrawPixel
        public static void P(Vector2 Position, Color color, float thickness)
        {
            if (_coloredLineTexture == null || _coloredLineColor != color)
            {
                _coloredLineColor = color;
                _coloredLineTexture = new Texture2D(1, 1);
                _coloredLineTexture.SetPixel(0, 0, _coloredLineColor);
                _coloredLineTexture.wrapMode = 0;
                _coloredLineTexture.Apply();
            }
            if (thickness < 1)
            {
                thickness = 1;
            }
            float yOffset = Mathf.Ceil(thickness / 2f);
            GUI.DrawTexture(new Rect(Position.x, Position.y - (float)yOffset, (float)thickness, (float)thickness), _coloredLineTexture);
        }
        #endregion
        #region DrawLine
        public static void L(Vector2 lineStart, Vector2 lineEnd, Color color, float thickness)
        {
            if (_coloredLineTexture == null || _coloredLineColor != color)
            {
                _coloredLineColor = color;
                _coloredLineTexture = new Texture2D(1, 1);
                _coloredLineTexture.SetPixel(0, 0, _coloredLineColor);
                _coloredLineTexture.wrapMode = 0;
                _coloredLineTexture.Apply();
            }

            var vector = lineEnd - lineStart;
            float pivot = 57.29578f * Mathf.Atan(vector.y / vector.x);
            if (vector.x < 0f)
            {
                pivot += 180f;
            }
            if (thickness < 1)
            {
                thickness = 1;
            }
            float yOffset = Mathf.Ceil(thickness / 2f);
            GUIUtility.RotateAroundPivot(pivot, lineStart);
            GUI.DrawTexture(new Rect(lineStart.x, lineStart.y - (float)yOffset, (float)thickness, (float)thickness), _coloredLineTexture);
            GUIUtility.RotateAroundPivot(-pivot, lineStart);
        }
        #endregion
    }
}
