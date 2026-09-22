using UnityEngine;
using UnityEngine.UIElements;

public class InstancedShellTexture : MonoBehaviour
{
    [Header("transform")]
    [SerializeField] protected Transform targetObject;

    [Header("mesh")]
    [SerializeField] protected Mesh shellMesh;

    [SerializeField] protected Material material;

    [SerializeField] protected int shell_count = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RenderGrass();
    }

    public void RenderGrass()
    {
        Vector3 b = shellMesh.bounds.size;
        b.Scale(transform.lossyScale * 10);
        RenderParams r = new RenderParams(material);
        r.worldBounds = new Bounds(transform.position, b);
        r.matProps = new MaterialPropertyBlock();
        r.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
        r.matProps.SetFloat("_Layer_Count", shell_count);
        r.matProps.SetFloat("_Layer_ID", -1);

        Graphics.RenderMeshPrimitives(r, shellMesh, 0, shell_count);
    }
}
