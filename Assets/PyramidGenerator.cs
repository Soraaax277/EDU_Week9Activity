using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PyramidGenerator : MonoBehaviour
{
    public float baseLength = 1f;
    public float height = 1f;
    public float rotSpeedX = 0f;
    public float rotSpeedY = 0f;
    public float rotSpeedZ = 0f;

    void Start()
    {
        GeneratePyramid();
    }

    void Update()
    {
        transform.Rotate(rotSpeedX * Time.deltaTime, rotSpeedY * Time.deltaTime, rotSpeedZ * Time.deltaTime);
    }

    void GeneratePyramid()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[5];
        float offset = baseLength * 0.5f;

        vertices[0] = new Vector3(-offset, 0, -offset);
        vertices[1] = new Vector3(offset, 0, -offset);
        vertices[2] = new Vector3(offset, 0, offset);
        vertices[3] = new Vector3(-offset, 0, offset);
        vertices[4] = new Vector3(0, height, 0);

        mesh.vertices = vertices;

        int[] triangles = new int[]
        {
            0, 2, 1, 
            0, 3, 2,

            0, 1, 4, 
            1, 2, 4, 
            2, 3, 4, 
            3, 0, 4  
        };

        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}
