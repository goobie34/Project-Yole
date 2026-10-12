using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;


public class WorldDepthBaker : MonoBehaviour
{
    public int _mapSize = 10000;
    public int _maxDepth = 100;


    public int _rendererIndex = 1;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();


        var cam = InitializeCamera();
        Debug.Log("DepthBaker - Camera created");


        var texture2d = RenderToTexture(cam, _mapSize, _mapSize);

        

        Debug.Log("DepthBaker - Depth Captured");



        Destroy(cam.gameObject);

        Debug.Log("DepthBaker - Camera Destroyed");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Camera InitializeCamera()
    {
        GameObject cameraObj = new GameObject("DepthCamera");
        cameraObj.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        cameraObj.transform.parent = transform;

        var depthCamera = cameraObj.AddComponent<Camera>();
        depthCamera.orthographic = true;
        depthCamera.orthographicSize = _mapSize / 4f;
        depthCamera.farClipPlane = _maxDepth;

        depthCamera.GetUniversalAdditionalCameraData().SetRenderer(_rendererIndex);

        return depthCamera;
    }

    private Texture2D RenderToTexture(Camera cam, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        // Restore previous state
        RenderTexture.active = previousActive;
        cam.targetTexture = null;
        rt.Release();

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(Vector3.zero + Vector3.down * (_maxDepth / 2f), new Vector3(_mapSize / 2f, _maxDepth, _mapSize / 2f));
    }
}
