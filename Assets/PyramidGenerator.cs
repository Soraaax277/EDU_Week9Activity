using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PyramidGenerator : MonoBehaviour
{
    public float baseLength = 1f;
    public float height = 1f;

    void Start()
    {
        GeneratePyramid();
    }

    void GeneratePyramid()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Define the vertices of the pyramid
        // A pyramid has a polygonal base and a common apex
        Vector3[] vertices = new Vector3[5];
        float offset = baseLength * 0.5f;

        // Base vertices (at y=0)
        vertices[0] = new Vector3(-offset, 0, -offset);
        vertices[1] = new Vector3(offset, 0, -offset);
        vertices[2] = new Vector3(offset, 0, offset);
        vertices[3] = new Vector3(-offset, 0, offset);
        // Apex vertex (at desired height)
        vertices[4] = new Vector3(0, height, 0);

        mesh.vertices = vertices;

        // Define the triangles (connecting vertices to form faces)
        int[] triangles = new int[]
        {
            // Base (two triangles)
            0, 2, 1, // Bottom-left, top-right, bottom-right
            0, 3, 2, // Bottom-left, top-left, top-right

            // Sides (four triangles meeting at the apex)
            0, 1, 4, // Front face
            1, 2, 4, // Right face
            2, 3, 4, // Back face
            3, 0, 4  // Left face
        };

        mesh.triangles = triangles;

        // Recalculate normals for proper lighting/shading
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}
