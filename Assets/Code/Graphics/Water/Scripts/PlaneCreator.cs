using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// Builds a large plane, with variable vertex density
/// 
/// </summary>
public class PlaneCreator : MonoBehaviour
{

    public int PlaneLength = 500;
    public float PlaneQuadRes = 1;

    public Mesh mesh { get; private set; }
    public string meshName = "PlaneMesh";

    private void BuildWaterMesh()
    {
        var MeshComponent = GetComponent<MeshFilter>();
        if (MeshComponent == null)
            return;

        MeshComponent.mesh = CreatePlaneMesh(PlaneLength, PlaneQuadRes);
    }

    // Source (modified): Acerola, https://github.com/GarrettGunnell/Water
    private Mesh CreatePlaneMesh(int length, float quadRes)
    {
        Mesh mesh = new Mesh();
        mesh.name = meshName;

        mesh.indexFormat = IndexFormat.UInt32;

        float halfLength = length * 0.5f;
        int sideVertCount = (int)(length * quadRes);

        Vector3[] vertices = new Vector3[(sideVertCount + 1) * (sideVertCount + 1)]; ;
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, x = 0; x <= sideVertCount; ++x)
        {
            for (int z = 0; z <= sideVertCount; ++z, ++i)
            {
                vertices[i] = new Vector3(((float)x / sideVertCount * length) - halfLength, 0, ((float)z / sideVertCount * length) - halfLength);
                uv[i] = new Vector2((float)x / sideVertCount, (float)z / sideVertCount);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[sideVertCount * sideVertCount * 6];

        for (int ti = 0, vi = 0, x = 0; x < sideVertCount; ++vi, ++x)
        {
            for (int z = 0; z < sideVertCount; ti += 6, ++vi, ++z)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + 1;
                triangles[ti + 2] = vi + sideVertCount + 2;
                triangles[ti + 3] = vi;
                triangles[ti + 4] = vi + sideVertCount + 2;
                triangles[ti + 5] = vi + sideVertCount + 1;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnEnable()
    {
        BuildWaterMesh();
    }

    private void OnDisable()
    {
        if (mesh != null)
        {
            Destroy(mesh);
            mesh = null;
        }
    }

}
