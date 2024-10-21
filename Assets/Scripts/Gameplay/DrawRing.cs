using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawRing : MonoBehaviour
{
    public float radius = 5f; // ������ ������
    public int segments = 50; // ���������� ��������� ��� �����
    public Color ringColor = Color.green; // ���� ������
    public float lineWidth = 0.05f; // ������� ������

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
        lineRenderer.useWorldSpace = false; // ���������� ��������� ����������
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

    // ����� ��� ��������� ������� �����������
    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        SetCircleStart();
    }
}
