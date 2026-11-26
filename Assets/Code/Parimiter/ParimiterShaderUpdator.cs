using UnityEngine;

public class ParimiterShaderUpdator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PerimeterBoundsServiceLocator.Instance == null)
            return;

        if (!PerimeterBoundsServiceLocator.Instance.TryGet(out var service))
            return;


        var radius = service.GetRadius();

        var origin = service.GetOrigin();

        Shader.SetGlobalFloat("_ParimiterRadius", radius);
        Shader.SetGlobalVector("_ParimiterOrigin", origin);
    }
}
