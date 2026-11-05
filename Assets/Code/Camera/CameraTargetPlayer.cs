using Unity.Cinemachine;
using UnityEngine;

public class CameraTargetPlayer : MonoBehaviour
{
    public bool boatOrIsland;
    public Transform player;
    public CinemachineCamera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(cam == null)
            cam = GetComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (boatOrIsland)
        {
            if (BoatCameraTargetSingleton.Instance == null)
            {
                player = null;
                return;
            }

            player = BoatCameraTargetSingleton.Instance.transform;
        }
        else
        {
            if(MarinerCameraTargetSingleton.Instance == null)
            {
                player = null;
                return;
            }

            player = MarinerCameraTargetSingleton.Instance.transform;
        }

        cam.Target.TrackingTarget = player.transform;
    }
}
