using UnityEngine;

public class CircleMesh : MonoBehaviour
{
    public int segments = 32; 
    public float radius = 1f; 

    internal void MakeCircle()
    {
        GenerateCircleMesh();
    }

    void GenerateCircleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "CircleMeshWithUV";
       
        Vector3[] vertices = new Vector3[segments + 1];
        Vector2[] uvs = new Vector2[vertices.Length];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float angleIncrement = 2 * Mathf.PI / segments;
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleIncrement * (i - 1);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices[i] = new Vector3(x, y, 0);
            uvs[i] = new Vector2((x / (2 * radius)) + 0.5f, (y / (2 * radius)) + 0.5f); // Map to UV space (0 to 1)
            if (transform.GetSiblingIndex() == 1)
            {
                MarkTRItem markTRItem = Instantiate(GameController.Inst.stretchController.markPrefab, GameController.Inst.stretchController.markTRParent);
                markTRItem.transform.localPosition = Vector3.zero;
                markTRItem.vertexIndex = i;
                markTRItem.realPos = vertices[i];
                GameController.Inst.stretchController.markTRItems.Add(markTRItem);
            }
        }

        int[] triangles = new int[segments * 3];
        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            triangles[i * 3] = 0; 
            triangles[i * 3 + 1] = next;
            triangles[i * 3 + 2] = current;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
