using UnityEngine;

public class MaterialApplier : MonoBehaviour
{
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material greenMaterial;

    public void ApplyGreen()
    {
        GetComponent<Renderer>().material = greenMaterial;
    }

    public void ApplyYellow()
    {
        GetComponent<Renderer>().material = yellowMaterial;
    }

    public void ApplyRed()
    {
        GetComponent<Renderer>().material = redMaterial;
    }

    public void ApplyDefault()
    {
        GetComponent<Renderer>().material = originalMaterial;
    }
}
