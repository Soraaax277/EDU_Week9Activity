using UnityEngine;

public class LineGen : MonoBehaviour
{
    public Material material;

    [Header("Sphere Settings")]
    public Vector3 spherePos;
    public float sphereRadius = 1.0f;
    [Range(4, 32)] public int sphereLatitudes = 8;  
    [Range(4, 32)] public int sphereLongitudes = 12; 

    [Header("Cube Settings")]
    public Vector3 cubePos;
    public float cubeSize = 1.0f;

    [Header("Pyramid Settings")]
    public Vector3 pyramidPos;
    public float pyramidSize = 1.0f;

    [Header("Capsule Settings")]
    public Vector3 capsulePos;
    public float capsuleRadius = 0.5f;
    public float capsuleHeight = 2.0f;

    private void OnPostRender()
    {
        if (material == null) return;

        material.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        DrawWireSphere(spherePos, sphereRadius, sphereLatitudes, sphereLongitudes);
        DrawWireCube(cubePos, cubeSize);
        DrawWirePyramid(pyramidPos, pyramidSize);
        DrawWireCapsule(capsulePos, capsuleRadius, capsuleHeight, 16);

        GL.End();
        GL.PopMatrix();
    }

    #region Sphere Logic


    private void DrawWireSphere(Vector3 center, float radius, int lats, int longs)
    {
        for (int i = 1; i < lats; i++)
        {
            float phi = Mathf.PI * i / lats;
            float y = Mathf.Cos(phi) * radius;
            float ringRadius = Mathf.Sin(phi) * radius;

            DrawCircleXZ(center + new Vector3(0, y, 0), ringRadius, longs * 2);
        }

        for (int i = 0; i < longs; i++)
        {
            float theta = 2.0f * Mathf.PI * i / longs;
            float cosTheta = Mathf.Cos(theta);
            float sinTheta = Mathf.Sin(theta);

            for (int j = 0; j < lats; j++)
            {
                float phi1 = Mathf.PI * j / lats;
                float phi2 = Mathf.PI * (j + 1) / lats;

                Vector3 p1 = new Vector3(Mathf.Sin(phi1) * cosTheta, Mathf.Cos(phi1), Mathf.Sin(phi1) * sinTheta) * radius;
                Vector3 p2 = new Vector3(Mathf.Sin(phi2) * cosTheta, Mathf.Cos(phi2), Mathf.Sin(phi2) * sinTheta) * radius;

                DrawLine3D(center + p1, center + p2);
            }
        }
    }

    private void DrawCircleXZ(Vector3 center, float radius, int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            float a1 = (float)i / segments * Mathf.PI * 2;
            float a2 = (float)(i + 1) / segments * Mathf.PI * 2;
            DrawLine3D(center + new Vector3(Mathf.Cos(a1) * radius, 0, Mathf.Sin(a1) * radius),
                       center + new Vector3(Mathf.Cos(a2) * radius, 0, Mathf.Sin(a2) * radius));
        }
    }

    #endregion

    #region Other Shapes Logic

    private void DrawWireCube(Vector3 center, float size)
    {
        float h = size * 0.5f;
        Vector3[] v = {
            new Vector3(center.x-h, center.y-h, center.z-h), new Vector3(center.x+h, center.y-h, center.z-h),
            new Vector3(center.x+h, center.y+h, center.z-h), new Vector3(center.x-h, center.y+h, center.z-h),
            new Vector3(center.x-h, center.y-h, center.z+h), new Vector3(center.x+h, center.y-h, center.z+h),
            new Vector3(center.x+h, center.y+h, center.z+h), new Vector3(center.x-h, center.y+h, center.z+h)
        };
        DrawLine3D(v[0], v[1]); DrawLine3D(v[1], v[2]); DrawLine3D(v[2], v[3]); DrawLine3D(v[3], v[0]);
        DrawLine3D(v[4], v[5]); DrawLine3D(v[5], v[6]); DrawLine3D(v[6], v[7]); DrawLine3D(v[7], v[4]);
        DrawLine3D(v[0], v[4]); DrawLine3D(v[1], v[5]); DrawLine3D(v[2], v[6]); DrawLine3D(v[3], v[7]);
    }


    private void DrawWirePyramid(Vector3 pos, float size)
    {
        float h = size * 0.5f;
        Vector3 apex = pos + Vector3.up * h;
        Vector3[] b = { pos + new Vector3(-h, -h, -h), pos + new Vector3(h, -h, -h), pos + new Vector3(h, -h, h), pos + new Vector3(-h, -h, h) };
        for (int i = 0; i < 4; i++)
        {
            DrawLine3D(b[i], b[(i + 1) % 4]);
            DrawLine3D(b[i], apex);
        }
    }


    private void DrawWireCapsule(Vector3 pos, float radius, float height, int segments)
    {
        float cylH = Mathf.Max(0, height - (radius * 2));
        Vector3 top = pos + Vector3.up * (cylH * 0.5f);
        Vector3 bot = pos - Vector3.up * (cylH * 0.5f);

        DrawLine3D(top + Vector3.left * radius, bot + Vector3.left * radius);
        DrawLine3D(top + Vector3.right * radius, bot + Vector3.right * radius);
        DrawLine3D(top + Vector3.forward * radius, bot + Vector3.forward * radius);
        DrawLine3D(top + Vector3.back * radius, bot + Vector3.back * radius);

        for (int i = 0; i < segments; i++)
        {
            float a1 = (float)i / segments * Mathf.PI;
            float a2 = (float)(i + 1) / segments * Mathf.PI;

            DrawLine3D(top + new Vector3(Mathf.Cos(a1) * radius, Mathf.Sin(a1) * radius, 0), top + new Vector3(Mathf.Cos(a2) * radius, Mathf.Sin(a2) * radius, 0));
            DrawLine3D(top + new Vector3(0, Mathf.Sin(a1) * radius, Mathf.Cos(a1) * radius), top + new Vector3(0, Mathf.Sin(a2) * radius, Mathf.Cos(a2) * radius));

            DrawLine3D(bot + new Vector3(Mathf.Cos(a1 + Mathf.PI) * radius, Mathf.Sin(a1 + Mathf.PI) * radius, 0), bot + new Vector3(Mathf.Cos(a2 + Mathf.PI) * radius, Mathf.Sin(a2 + Mathf.PI) * radius, 0));
            DrawLine3D(bot + new Vector3(0, Mathf.Sin(a1 + Mathf.PI) * radius, Mathf.Cos(a1 + Mathf.PI) * radius), bot + new Vector3(0, Mathf.Sin(a2 + Mathf.PI) * radius, Mathf.Cos(a2 + Mathf.PI) * radius));
        }
        DrawCircleXZ(top, radius, segments);
        DrawCircleXZ(bot, radius, segments);
    }

    private void DrawLine3D(Vector3 p1, Vector3 p2)
    {
        float pers1 = PerspectiveCamera.Instance.GetPerspective(p1.z);
        float pers2 = PerspectiveCamera.Instance.GetPerspective(p2.z);
        GL.Vertex(new Vector2(p1.x * pers1, p1.y * pers1));
        GL.Vertex(new Vector2(p2.x * pers2, p2.y * pers2));
    }

    #endregion
}