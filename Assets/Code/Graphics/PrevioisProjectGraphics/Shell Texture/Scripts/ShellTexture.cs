using System.Collections.Generic;
using UnityEngine;

public class ShellTexture : MonoBehaviour
{
    [Header("transform")]
    [SerializeField] protected Transform targetObject;

    [Header("mesh")]
    [SerializeField] protected Mesh shellMesh;

    [SerializeField] protected Material material;

    [SerializeField] protected int shell_count = 5;


    private GameObject[] shells;



    protected void OnEnable()
    {
        Initialize();

        if (shellMesh == null || material == null) return;

        shells = new GameObject[shell_count];

        for (int i = 0; i < shell_count; i++)
        {
            shells[i] = new GameObject("Shell: " + i);

            shells[i].transform.parent = targetObject;
            shells[i].transform.localPosition = Vector3.zero;
            shells[i].transform.localRotation = Quaternion.identity;
            shells[i].transform.localScale = Vector3.one;


            var shell_filter = shells[i].AddComponent<MeshFilter>();
            var shell_renderer = shells[i].AddComponent<MeshRenderer>();

            shell_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


            shell_filter.mesh = shellMesh;
            shell_renderer.material = material;

            shell_renderer.material.SetFloat("_Layer_Count", shell_count);
            shell_renderer.material.SetFloat("_Layer_ID", i);
        }
    }

    protected void OnDisable()
    {
        if (shells == null) return;

        foreach (var sh in shells)
        {
            if(sh == null) continue;
            Destroy(sh);
        }

        //for (int i = 0; i < shells.Length; i++)
        //{
        //    Destroy(shells[i]);
        //}

        shells = null;
    }



    protected virtual void Initialize()
    {
        if (targetObject == null) targetObject = transform;

        if (shellMesh == null) shellMesh = GetComponent<MeshFilter>().mesh;

        if (material == null) 
        {
            MeshRenderer materialRenderer = GetComponent<MeshRenderer>();

            material = materialRenderer.material;
        }
    }
}

