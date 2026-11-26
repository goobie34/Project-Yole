using UnityEngine;

public class EffectMaterialInterface : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color OutlineColor;

    [ColorUsage(true, true)]
    public Color FlashColor;

    private MeshRenderer[] rendererChildren;

    private static readonly Color trans = new Color(0,0,0,0);

    private void Start()
    {
        rendererChildren = GetComponentsInChildren<MeshRenderer>();

    }

    private void OnEnable()
    {
        if (rendererChildren != null)
            return;

        rendererChildren = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnDisable()
    {
        if(rendererChildren == null)
            return;

        foreach (var child in rendererChildren)
        {
            SetFlashColor(child, trans);
            SetOutlineColor(child, trans);

        }

        rendererChildren = null;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var child in rendererChildren)
        {
            SetFlashColor(child, FlashColor);

            SetOutlineColor(child, OutlineColor);

        }
    }

    private void SetFlashColor(MeshRenderer renderer,Color color)
    {
        var mats = renderer.materials;

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor("_FlashColor_Interact", color);
        }
    }

    private void SetOutlineColor(MeshRenderer renderer, Color color)
    {
        var mats = renderer.materials;

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor("_OutlineColor_Interact", color);
        }
    }
}
