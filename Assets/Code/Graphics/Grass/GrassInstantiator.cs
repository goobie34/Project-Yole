using UnityEngine;

public class GrassInstantiator : MonoBehaviour
{
    public Material _material;

    public Mesh _mesh;

    public Transform _camera;

    public float boundsSize = 10;

    public int sideCount = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        RenderGrass(sideCount * sideCount);
    }

    public void RenderGrass(int count)
    {
        if (boundsSize == 0)
            return;

        if (count == 0)
            return;

        Vector2 bounds = Vector2.one * boundsSize;

        Vector3 pos = transform.position;

        pos /= boundsSize / (float)sideCount;

        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);

        pos *= boundsSize / (float)sideCount;

        _camera.position = pos;


RenderParams r = new RenderParams(_material);
        r.worldBounds = new Bounds(pos, Vector3.one * boundsSize * 2);
        r.matProps = new MaterialPropertyBlock();
        r.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.identity);
        //r.matProps.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
        r.matProps.SetFloat("_NumInstances", count);
        r.matProps.SetVector("_BoundsSize", bounds);

        r.layer = gameObject.layer;


        Graphics.RenderMeshPrimitives(r, _mesh, 0, count);
    }
}
