using UnityEngine;

public class LineGen : MonoBehaviour
{
    public Material material;

    [Header("Sphere Settings")]
    public Vector3 spherePos;
    public float sphereRadius = 1.0f;
    [Range(4, 32)] public int sphereLatitudes = 8;  
    [Range(4, 32)] public int sphereLongitudes = 12; 
    public Vector3 sphereRot;
    public Vector3 sphereRotSpeed;

    [Header("Cube Settings")]
    public Vector3 cubePos;
    public float cubeSize = 1.0f;
    public Vector3 cubeRot;
    public Vector3 cubeRotSpeed;

    [Header("Pyramid Settings")]
    public Vector3 pyramidPos;
    public float pyramidSize = 1.0f;
    public Vector3 pyramidRot;
    public Vector3 pyramidRotSpeed;

    [Header("Capsule Settings")]
    public Vector3 capsulePos;
    public float capsuleRadius = 0.5f;
    public float capsuleHeight = 2.0f;
    public Vector3 capsuleRot;
    public Vector3 capsuleRotSpeed;

    private void Update()
    {
        sphereRot += sphereRotSpeed * Time.deltaTime;
        cubeRot += cubeRotSpeed * Time.deltaTime;
        pyramidRot += pyramidRotSpeed * Time.deltaTime;
        capsuleRot += capsuleRotSpeed * Time.deltaTime;
    }

    private void OnPostRender()
    {
        if (material == null) return;

        material.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        DrawWireSphere(spherePos, sphereRadius, sphereLatitudes, sphereLongitudes, sphereRot);
        DrawWireCube(cubePos, cubeSize, cubeRot);
        DrawWirePyramid(pyramidPos, pyramidSize, pyramidRot);
        DrawWireCapsule(capsulePos, capsuleRadius, capsuleHeight, 16, capsuleRot);

        GL.End();
        GL.PopMatrix();
    }

    #region Sphere Logic


    private void DrawWireSphere(Vector3 center, float radius, int lats, int longs, Vector3 rotation)
    {
        for (int i = 1; i < lats; i++)
        {
            float phi = Mathf.PI * i / lats;
            float y = Mathf.Cos(phi) * radius;
            float ringRadius = Mathf.Sin(phi) * radius;

            DrawCircleXZ(center, ringRadius, y, longs * 2, rotation);
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

                DrawLine3D(center + RotatePoint(p1, rotation), center + RotatePoint(p2, rotation));
            }
        }
    }

    private void DrawCircleXZ(Vector3 center, float radius, float yOffset, int segments, Vector3 rotation)
    {
        for (int i = 0; i < segments; i++)
        {
            float a1 = (float)i / segments * Mathf.PI * 2;
            float a2 = (float)(i + 1) / segments * Mathf.PI * 2;

            Vector3 p1 = new Vector3(Mathf.Cos(a1) * radius, yOffset, Mathf.Sin(a1) * radius);
            Vector3 p2 = new Vector3(Mathf.Cos(a2) * radius, yOffset, Mathf.Sin(a2) * radius);

            DrawLine3D(center + RotatePoint(p1, rotation), center + RotatePoint(p2, rotation));
        }
    }

    #endregion

    #region Other Shapes Logic

    private void DrawWireCube(Vector3 center, float size, Vector3 rotation)
    {
        float h = size * 0.5f;
        Vector3[] v = {
            RotatePoint(new Vector3(-h, -h, -h), rotation), RotatePoint(new Vector3(h, -h, -h), rotation),
            RotatePoint(new Vector3(h, h, -h), rotation), RotatePoint(new Vector3(-h, h, -h), rotation),
            RotatePoint(new Vector3(-h, -h, h), rotation), RotatePoint(new Vector3(h, -h, h), rotation),
            RotatePoint(new Vector3(h, h, h), rotation), RotatePoint(new Vector3(-h, h, h), rotation)
        };
        DrawLine3D(center + v[0], center + v[1]); DrawLine3D(center + v[1], center + v[2]); DrawLine3D(center + v[2], center + v[3]); DrawLine3D(center + v[3], center + v[0]);
        DrawLine3D(center + v[4], center + v[5]); DrawLine3D(center + v[5], center + v[6]); DrawLine3D(center + v[6], center + v[7]); DrawLine3D(center + v[7], center + v[4]);
        DrawLine3D(center + v[0], center + v[4]); DrawLine3D(center + v[1], center + v[5]); DrawLine3D(center + v[2], center + v[6]); DrawLine3D(center + v[3], center + v[7]);
    }


    private void DrawWirePyramid(Vector3 pos, float size, Vector3 rotation)
    {
        float h = size * 0.5f;
        Vector3 apex = RotatePoint(Vector3.up * h, rotation);
        Vector3[] b = { 
            RotatePoint(new Vector3(-h, -h, -h), rotation), 
            RotatePoint(new Vector3(h, -h, -h), rotation), 
            RotatePoint(new Vector3(h, -h, h), rotation), 
            RotatePoint(new Vector3(-h, -h, h), rotation) 
        };
        for (int i = 0; i < 4; i++)
        {
            DrawLine3D(pos + b[i], pos + b[(i + 1) % 4]);
            DrawLine3D(pos + b[i], pos + apex);
        }
    }


    private void DrawWireCapsule(Vector3 pos, float radius, float height, int segments, Vector3 rotation)
    {
        float cylH = Mathf.Max(0, height - (radius * 2));
        Vector3 topOffset = Vector3.up * (cylH * 0.5f);
        Vector3 botOffset = -Vector3.up * (cylH * 0.5f);

        DrawLine3D(pos + RotatePoint(topOffset + Vector3.left * radius, rotation), pos + RotatePoint(botOffset + Vector3.left * radius, rotation));
        DrawLine3D(pos + RotatePoint(topOffset + Vector3.right * radius, rotation), pos + RotatePoint(botOffset + Vector3.right * radius, rotation));
        DrawLine3D(pos + RotatePoint(topOffset + Vector3.forward * radius, rotation), pos + RotatePoint(botOffset + Vector3.forward * radius, rotation));
        DrawLine3D(pos + RotatePoint(topOffset + Vector3.back * radius, rotation), pos + RotatePoint(botOffset + Vector3.back * radius, rotation));

        for (int i = 0; i < segments; i++)
        {
            float a1 = (float)i / segments * Mathf.PI;
            float a2 = (float)(i + 1) / segments * Mathf.PI;

            Vector3 pTop1 = topOffset + new Vector3(Mathf.Cos(a1) * radius, Mathf.Sin(a1) * radius, 0);
            Vector3 pTop2 = topOffset + new Vector3(Mathf.Cos(a2) * radius, Mathf.Sin(a2) * radius, 0);
            DrawLine3D(pos + RotatePoint(pTop1, rotation), pos + RotatePoint(pTop2, rotation));

            Vector3 pTop3 = topOffset + new Vector3(0, Mathf.Sin(a1) * radius, Mathf.Cos(a1) * radius);
            Vector3 pTop4 = topOffset + new Vector3(0, Mathf.Sin(a2) * radius, Mathf.Cos(a2) * radius);
            DrawLine3D(pos + RotatePoint(pTop3, rotation), pos + RotatePoint(pTop4, rotation));

            Vector3 pBot1 = botOffset + new Vector3(Mathf.Cos(a1 + Mathf.PI) * radius, Mathf.Sin(a1 + Mathf.PI) * radius, 0);
            Vector3 pBot2 = botOffset + new Vector3(Mathf.Cos(a2 + Mathf.PI) * radius, Mathf.Sin(a2 + Mathf.PI) * radius, 0);
            DrawLine3D(pos + RotatePoint(pBot1, rotation), pos + RotatePoint(pBot2, rotation));

            Vector3 pBot3 = botOffset + new Vector3(0, Mathf.Sin(a1 + Mathf.PI) * radius, Mathf.Cos(a1 + Mathf.PI) * radius);
            Vector3 pBot4 = botOffset + new Vector3(0, Mathf.Sin(a2 + Mathf.PI) * radius, Mathf.Cos(a2 + Mathf.PI) * radius);
            DrawLine3D(pos + RotatePoint(pBot3, rotation), pos + RotatePoint(pBot4, rotation));
        }
        DrawCircleXZ(pos, radius, topOffset.y, segments, rotation);
        DrawCircleXZ(pos, radius, botOffset.y, segments, rotation);
    }

    private Vector3 RotatePoint(Vector3 p, Vector3 rotation)
    {
        float radX = rotation.x * Mathf.Deg2Rad;
        float sinX = Mathf.Sin(radX);
        float cosX = Mathf.Cos(radX);
        float py = p.y * cosX - p.z * sinX;
        float pz = p.y * sinX + p.z * cosX;
        p.y = py; p.z = pz;

        float radY = rotation.y * Mathf.Deg2Rad;
        float sinY = Mathf.Sin(radY);
        float cosY = Mathf.Cos(radY);
        float px = p.x * cosY + p.z * sinY;
        pz = -p.x * sinY + p.z * cosY;
        p.x = px; p.z = pz;

        float radZ = rotation.z * Mathf.Deg2Rad;
        float sinZ = Mathf.Sin(radZ);
        float cosZ = Mathf.Cos(radZ);
        px = p.x * cosZ - p.y * sinZ;
        py = p.x * sinZ + p.y * cosZ;
        p.x = px; p.y = py;

        return p;
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