using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[AddComponentMenu("UI/Effects/MultiColorGradient")]
public class UIMultiColorGradient : BaseMeshEffect
{
    public List<Color> colors = new List<Color> { Color.white, Color.black };
    [Range(-180f, 180f)]
    public float m_angle = 0f;
    public bool m_ignoreRatio = true;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!enabled || colors.Count < 2)
            return;

        Rect rect = graphic.rectTransform.rect;
        Vector2 dir = UIGradientUtils.RotationDir(m_angle);

        if (!m_ignoreRatio)
            dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

        // Вычисляем матрицу для преобразования координат
        UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

        UIVertex vertex = default(UIVertex);
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            Vector2 localPosition = localPositionMatrix * vertex.position;

            // Нормализуем позицию в диапазоне [0,1] на основе проекции
            float t = (localPosition.x + localPosition.y) * 0.5f + 0.5f;

            // Ограничиваем t в диапазоне [0,1] для предотвращения артефактов
            t = Mathf.Clamp01(t);

            // Интерполируем цвет
            vertex.color *= GetInterpolatedColor(t);
            vh.SetUIVertex(vertex, i);
        }
    }

    private Color GetInterpolatedColor(float t)
    {
        if (colors.Count == 2)
        {
            return Color.Lerp(colors[0], colors[1], t);
        }

        float segment = 1f / (colors.Count - 1);
        int index = Mathf.Min(Mathf.FloorToInt(t / segment), colors.Count - 2);
        float localT = (t - index * segment) / segment;

        return Color.Lerp(colors[index], colors[index + 1], localT);
    }

    public void AddColor(Color color)
    {
        colors.Add(color);
        graphic.SetVerticesDirty();
    }

    public void RemoveColor(int index)
    {
        if (index >= 0 && index < colors.Count)
        {
            colors.RemoveAt(index);
            graphic.SetVerticesDirty();
        }
    }

    public void SetColors(List<Color> newColors)
    {
        colors = new List<Color>(newColors);
        graphic.SetVerticesDirty();
    }
}