using UnityEngine;
using UnityEngine.Rendering;

public class WakesControler : MonoBehaviour
{
   

    public Material wakesMat;

    public CustomRenderTexture WakesRT;

    public Camera wakesCamera;

    public Transform wakesMesh;

    public Transform targetObject;
    public float steppScale;

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
        wakesMesh.transform.parent = null;
    }

    private void OnRender(ScriptableRenderContext a, Camera c)
    {
        if (c != wakesCamera)
            return;

        //UpdateDelta();
    }

    private void Update()
    {
        wakesMesh.position = transform.position;



    }

    private void LateUpdate()
    {
        UpdateFollow();
        UpdateDelta();
        UpdateRenderTarget();
    }

    private void UpdateFollow()
    {
        if (targetObject == null)
            return;
        if(steppScale == 0)
        {
            transform.position = targetObject.position;

            return;
        }

        var steppedPos = new Vector2(Mathf.Round(targetObject.position.x / steppScale) * steppScale, Mathf.Round(targetObject.position.z / steppScale) * steppScale);

        transform.position = new Vector3(steppedPos.x, 0, steppedPos.y);

    }

    private void UpdateRenderTarget()
    {
       
        wakesCamera.Render();
        WakesRT.Update();

    }

    private void UpdateDelta()
    {
        
        worldDelta = currentPos - lastPos;

        delta = worldDelta / (wakesCamera.orthographicSize * scale) ;

        lastPos = currentPos;

        wakesMat.SetVector("_Delta", delta);

    }
}

