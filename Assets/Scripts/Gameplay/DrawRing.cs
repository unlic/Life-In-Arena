using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawRing : MonoBehaviour
{
    public float radius = 5f; // Радиус обруча
    public int segments = 50; // Количество сегментов для круга
    public Color ringColor = Color.green; // Цвет обруча
    public float lineWidth = 0.05f; // Толщина обруча

    private LineRenderer lineRenderer;

    private void Start()
    {
        SetCircleStart();
    }
    private void SetCircleStart()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = false; // Используем локальные координаты
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = ringColor;
        lineRenderer.endColor = ringColor;

        CreateCircle();
    }

    private void CreateCircle()
    {
        float angle = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angle);
            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    // Метод для изменения радиуса динамически
    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        SetCircleStart();
    }
}
