using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TwoPartVisionCone : MonoBehaviour
{
    [Header("Cone Settings")]
    public float outerRadius = 4f;
    public float innerRadius = 2f;
    [Range(3, 100)] public int segments = 30;
    [Range(1, 180)] public float angle = 60f;

    [Header("Materials")]
    public Material goodConeMaterial;
    public Material badConeMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private float lastOuterRadius, lastInnerRadius, lastAngle;
    private int lastSegments;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        GenerateConeMesh();
        AssignMaterials();
    }

    void Update()
    {
        if (outerRadius != lastOuterRadius || innerRadius != lastInnerRadius || angle != lastAngle || segments != lastSegments)
        {
            GenerateConeMesh();
            lastOuterRadius = outerRadius;
            lastInnerRadius = innerRadius;
            lastAngle = angle;
            lastSegments = segments;
        }
    }

    void GenerateConeMesh()
    {
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int totalVertices = (segments + 1) * 2 + 4; // Frustum + full bad cone
        Vector3[] vertices = new Vector3[totalVertices];

        int[] goodConeTriangles = new int[segments * 6]; // Good cone (frustum)
        int[] badConeTriangles = new int[(segments * 3) + 6]; // Bad cone (inner triangle + curved gap)

        float halfAngleRad = Mathf.Deg2Rad * (angle / 2);
        float gapSize = (outerRadius - innerRadius) * 0.1f; // Small gap adjustment

        // Create frustum vertices (Good Cone)
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = Mathf.Lerp(-halfAngleRad, halfAngleRad, t);
            float cosA = Mathf.Cos(currentAngle);
            float sinA = Mathf.Sin(currentAngle);

            // Outer and inner ring
            vertices[i] = new Vector3(sinA * outerRadius, 0, cosA * outerRadius);
            vertices[i + segments + 1] = new Vector3(sinA * (innerRadius - gapSize), 0, cosA * (innerRadius - gapSize));
        }

        // Full Bad Cone (inner triangle)
        Vector3 center = Vector3.zero; // Player position
        Vector3 tip = new Vector3(0, 0, innerRadius - gapSize); // Adjusted tip
        Vector3 leftSide = new Vector3(Mathf.Sin(-halfAngleRad) * (innerRadius - gapSize), 0, Mathf.Cos(-halfAngleRad) * (innerRadius - gapSize));
        Vector3 rightSide = new Vector3(Mathf.Sin(halfAngleRad) * (innerRadius - gapSize), 0, Mathf.Cos(halfAngleRad) * (innerRadius - gapSize));

        vertices[(segments + 1) * 2] = center;
        vertices[(segments + 1) * 2 + 1] = leftSide;
        vertices[(segments + 1) * 2 + 2] = rightSide;
        vertices[(segments + 1) * 2 + 3] = tip;

        // Generate triangles for Good Cone (frustum)
        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int v0 = i;
            int v1 = i + 1;
            int v2 = i + segments + 1;
            int v3 = v2 + 1;

            // First triangle
            goodConeTriangles[ti++] = v0;
            goodConeTriangles[ti++] = v1;
            goodConeTriangles[ti++] = v2;

            // Second triangle
            goodConeTriangles[ti++] = v2;
            goodConeTriangles[ti++] = v1;
            goodConeTriangles[ti++] = v3;
        }

        // Generate Bad Cone (Curved Gap + Inner Triangle)
        int bi = 0;
        for (int i = 0; i < segments; i++)
        {
            int v0 = i + segments + 1;
            int v1 = v0 + 1;
            int v2 = (segments + 1) * 2; // Center point

            badConeTriangles[bi++] = v0;
            badConeTriangles[bi++] = v1;
            badConeTriangles[bi++] = v2;
        }

        // Inner Triangle
        badConeTriangles[bi++] = (segments + 1) * 2; // Center
        badConeTriangles[bi++] = (segments + 1) * 2 + 1; // Left side
        badConeTriangles[bi++] = (segments + 1) * 2 + 3; // Tip

        badConeTriangles[bi++] = (segments + 1) * 2; // Center
        badConeTriangles[bi++] = (segments + 1) * 2 + 3; // Tip
        badConeTriangles[bi++] = (segments + 1) * 2 + 2; // Right side

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.subMeshCount = 2;
        mesh.SetTriangles(goodConeTriangles, 0); // Good Cone (Outer Frustum)
        mesh.SetTriangles(badConeTriangles, 1); // Full Bad Cone (Triangle + Gap)
        mesh.RecalculateNormals(); // Fix normals
    }




    void AssignMaterials()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { goodConeMaterial, badConeMaterial };
    }
}
