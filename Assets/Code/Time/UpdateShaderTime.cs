using UnityEngine;

public class UpdateShaderTime : MonoBehaviour
{
  

    // Update is called once per frame
    void Update()
    {
        if (TimeServiceLocator.Instance == null)
            return;

        if (!TimeServiceLocator.Instance.TryGet(out var service))
            return;

        Shader.SetGlobalFloat("_GlobalTime", (float)service.GetCurrentTimeFloatUnscaled());
    }
}
