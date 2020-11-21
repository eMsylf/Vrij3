/**
 * Based on a script by Steffen (http://forum.unity3d.com/threads/torus-in-unity.8487/) (in $primitives_966_104.zip, originally named "Primitives.cs")
 *
 * Editted by Michael Zoller on December 6, 2015.
 * It was shortened by about 30 lines (and possibly sped up by a factor of 2) by consolidating math & loops and removing intermediate Collections.
 */
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Torus: MonoBehaviour {
    [Min(0.001f)]
	public float radius = 1f;
    [Min(0.001f)]
	public float thickness = 0.4f;
    [Min(3)]
	public int segments = 32;
    [Min(3)]
	public int segmentDetail = 12;

    Mesh mesh;
    public Mesh GetMesh()
    {
        if (mesh == null)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = "Torus";
            }
        }
        return mesh;
    }

    public void UpdateMesh(Vector3[] vertices, int[] triangleIndices)
    {
        Mesh _mesh = GetMesh();
        _mesh.vertices = vertices;
        _mesh.triangles = triangleIndices;

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.Optimize();
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh = _mesh;
    }

    public void Recalculate(out Vector3[] vertices, out int[] triangleIndices)
    {
        // Total vertices
        int totalVertices = segments * segmentDetail;

        // Total primitives
        int totalPrimitives = totalVertices * 2;

        // Total indices
        int totalIndices = totalPrimitives * 3;

        // Init the vertex and triangle arrays
        vertices = new Vector3[totalVertices];
        triangleIndices = new int[totalIndices];

        // Calculate size of a segment and a tube
        float segmentSize = 2 * Mathf.PI / (float)segments;
        float tubeSize = 2 * Mathf.PI / (float)segmentDetail;

        // Create floats for our xyz coordinates
        float x, y, z;

        // Begin loop that fills in both arrays
        for (int i = 0; i < segments; i++)
        {
            // Find next (or first) segment offset
            int n = (i + 1) % segments; // changed segmentList.Count to numSegments

            // Find the current and next segments
            int currentTubeOffset = i * segmentDetail;
            int nextTubeOffset = n * segmentDetail;

            for (int j = 0; j < segmentDetail; j++)
            {
                // Find next (or first) vertex offset
                int m = (j + 1) % segmentDetail; // changed currentTube.Count to numTubes

                // Find the 4 vertices that make up a quad
                int iv1 = currentTubeOffset + j;
                int iv2 = currentTubeOffset + m;
                int iv3 = nextTubeOffset + m;
                int iv4 = nextTubeOffset + j;

                // Calculate X, Y, Z coordinates.
                x = (radius + thickness * Mathf.Cos(j * tubeSize)) * Mathf.Cos(i * segmentSize);
                z = (radius + thickness * Mathf.Cos(j * tubeSize)) * Mathf.Sin(i * segmentSize);
                y = thickness * Mathf.Sin(j * tubeSize);

                // Add the vertex to the vertex array
                vertices[iv1] = new Vector3(x, y, z);

                // "Draw" the first triangle involving this vertex
                triangleIndices[iv1 * 6] = iv1;
                triangleIndices[iv1 * 6 + 1] = iv2;
                triangleIndices[iv1 * 6 + 2] = iv3;
                // Finish the quad
                triangleIndices[iv1 * 6 + 3] = iv3;
                triangleIndices[iv1 * 6 + 4] = iv4;
                triangleIndices[iv1 * 6 + 5] = iv1;
            }
        }
    }
}
