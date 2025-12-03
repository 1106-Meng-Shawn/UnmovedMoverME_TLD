using System.Collections.Generic;
using UnityEngine;

public struct Face
{
    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh m_mesh;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private List<Face> m_faces;

    public Material material;
    public float innerSize = 0.5f;
    public float outerSize = 1f;
    public float height = 1f;
    public bool isFlatTopped = true;

    private void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();

        m_mesh = new Mesh();
        m_mesh.name = "Hex";
        m_meshFilter.mesh = m_mesh;

        if (material == null)
        {
            // ??????????????
            material = new Material(Shader.Find("Standard"));
            material.color = Color.green;
        }
        m_meshRenderer.material = material;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && m_mesh != null)
        {
            DrawMesh();
        }
    }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        m_faces = new List<Face>();

        // ????
        for (int i = 0; i < 6; i++)
        {
            m_faces.Add(CreateFace(innerSize, outerSize, height / 2f, -height / 2f, i));
        }
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < m_faces.Count; i++)
        {
            Face face = m_faces[i];
            int vertexOffset = vertices.Count;

            vertices.AddRange(face.vertices);
            uvs.AddRange(face.uvs);

            foreach (int index in face.triangles)
            {
                triangles.Add(vertexOffset + index);
            }
        }

        m_mesh.Clear();
        m_mesh.vertices = vertices.ToArray();
        m_mesh.triangles = triangles.ToArray();
        m_mesh.uv = uvs.ToArray();
        m_mesh.RecalculateNormals();
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int pointIndex, bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightA, pointIndex);
        Vector3 pointB = GetPoint(innerRad, heightA, (pointIndex + 1) % 6);
        Vector3 pointC = GetPoint(innerRad, heightB, (pointIndex + 1) % 6);
        Vector3 pointD = GetPoint(innerRad, heightB, pointIndex);

        List<Vector3> verts = new List<Vector3> { pointA, pointB, pointC, pointD };
        List<int> tris = new List<int> { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uv = new List<Vector2> {
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(1, 0), new Vector2(0, 0)
        };

        if (reverse)
        {
            verts.Reverse();
        }

        return new Face(verts, tris, uv);
    }

    private Vector3 GetPoint(float radius, float y, int index)
    {
        float angle_deg = isFlatTopped ? 60f * index : 60f * index - 30f;
        float angle_rad = Mathf.Deg2Rad * angle_deg;
        return new Vector3(radius * Mathf.Cos(angle_rad), y, radius * Mathf.Sin(angle_rad));
    }
}
