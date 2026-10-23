using UnityEngine;
using UnityEngine.Rendering;

public class WakesControler : MonoBehaviour
{
    [Header("Wake Shader Objects")]

    public Material wakesMat;

    public CustomRenderTexture WakesRT;

    public Camera wakesCamera;

    [Header("Game World Transforms")]

    public Transform wakesMeshTransform;

    public Transform cameraTransform;

    public Transform targetObject;

    

    [Header("Delta Variables")]
    public float steppScale;
    public float scale = 2;


    [Header("Delta Data")]
    private Vector2 lastPos;
    private Vector2 currentPos; //=> new Vector2( wakesCamera.transform.position.x, wakesCamera.transform.position.z);

    private Vector4 delta;
    private Vector4 worldDelta;




    private void OnEnable()
    {
        //RenderPipelineManager.beginCameraRendering += OnRender; 
    }

    private void OnDisable()
    {
        //RenderPipelineManager.beginCameraRendering -= OnRender; 
    }

   

    private void OnRender(ScriptableRenderContext a, Camera c)
    {
        if (c != wakesCamera)
            return;

        //UpdateDelta();
    }

    private void Update()
    {
        UpdateMeshPos();


    }

    private void LateUpdate()
    {
        UpdateFollow();
        UpdateCameraPos();

        UpdateDelta();
        UpdateRenderTarget();
    }

    private void UpdateFollow()
    {
        if (targetObject == null)
            return;
        if(steppScale == 0)
        {
            currentPos = new Vector2(targetObject.position.x,targetObject.position.z);

            return;
        }

        currentPos = new Vector2(Mathf.Round(targetObject.position.x / steppScale) * steppScale, Mathf.Round(targetObject.position.z / steppScale) * steppScale);

        //transform.position = new Vector3(steppedPos.x, 0, steppedPos.y);

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

    private void UpdateMeshPos()
    {
        if (wakesMeshTransform == null)
            return;

        wakesMeshTransform.position = new Vector3(currentPos.x,0,currentPos.y);

    }
    private void UpdateCameraPos()
    {
        if (cameraTransform == null)
            return;

        cameraTransform.position = new Vector3(currentPos.x, 0, currentPos.y);
    }
}

