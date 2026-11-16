using UnityEngine;

public class GrassInstantiator : MonoBehaviour
{
    public Material _material;

    public Mesh _mesh;

    public int count = 10;

    public float boundsSize = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RenderGrass(count);
    }

    public void RenderGrass(int count)
    {
        Vector2 bounds = Vector2.one * boundsSize;


        RenderParams r = new RenderParams(_material);
        r.worldBounds = new Bounds(transform.position, Vector3.one * boundsSize * 2);
        r.matProps = new MaterialPropertyBlock();
        r.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.identity);
        //r.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
        r.matProps.SetFloat("_NumInstances", count);
        r.matProps.SetVector("_BoundsSize", bounds);

        r.layer = gameObject.layer;


        Graphics.RenderMeshPrimitives(r, _mesh, 0, count);
    }
}
