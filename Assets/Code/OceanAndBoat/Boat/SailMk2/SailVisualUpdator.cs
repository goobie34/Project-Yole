using UnityEngine;

public class SailVisualUpdator : MonoBehaviour
{

    public SailPhysics sailSimulator;
    public SailWindManager windManager;

    public Transform sailRotator;
    public MeshRenderer sailRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(sailSimulator == null)
            sailSimulator = GetComponentInParent<SailPhysics>();

        if(windManager == null)
            windManager = GetComponentInParent<SailWindManager>();

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateSailRotation();
        UpdateSailMaterial();
    
    }

    private void UpdateSailRotation()
    {
        if(sailSimulator == null || sailRotator == null)
            return;

        sailRotator.localRotation = sailSimulator.sailRotation;
    }

    private void UpdateSailMaterial()
    {
        if (sailRenderer == null  ||  windManager == null)
            return;

        var wind = windManager.GetCurrentWind();    

        sailRenderer.material.SetVector("_WindDirection", wind.normalized);
        sailRenderer.material.SetFloat("_WindMagnitude", wind.magnitude);
    }
}
