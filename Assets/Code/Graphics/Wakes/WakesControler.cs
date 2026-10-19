using UnityEngine;
using UnityEngine.Rendering;

public class WakesControler : MonoBehaviour
{
    public Material wakesMat;

    public CustomRenderTexture WakesRT;

    public Camera wakesCamera;

    public Transform wakesMesh;

    private Vector2 lastPos;
    private Vector2 currentPos => new Vector2( wakesCamera.transform.position.x, wakesCamera.transform.position.z);

    public Vector4 delta;
    public Vector4 worldDelta;

    public float scale = 2;


    private void OnEnable()
    {
        //RenderPipelineManager.beginCameraRendering += OnRender; 
    }

    private void OnDisable()
    {
        //RenderPipelineManager.beginCameraRendering -= OnRender; 
    }

    private void Start()
    {
    }

    private void OnRender(ScriptableRenderContext a, Camera c)
    {
        if (c != wakesCamera)
            return;

        UpdateDelta();
    }

    private void LateUpdate()
    {
        UpdateDelta();
        UpdateRenderTarget();
    }

    private void UpdateRenderTarget()
    {
       
        wakesCamera.Render();
        WakesRT.Update();


    }

    private void UpdateDelta()
    {
        
        worldDelta = currentPos - lastPos;

        delta = worldDelta / (wakesCamera.orthographicSize * scale);

        lastPos = currentPos;

        wakesMat.SetVector("_Delta", delta);

    }
}

