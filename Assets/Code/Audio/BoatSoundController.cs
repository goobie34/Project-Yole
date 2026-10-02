using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BoatSoundController : MonoBehaviour
{
    [SerializeField]
    EventReference eventReference;
    EventInstance eventInstance;

    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float maxVelocity;

    private void Awake()
    {
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, this.gameObject);
    }
    private void Start()
    {
        eventInstance.start();
    }

    private void FixedUpdate()
    {
        float paramVal = rb.linearVelocity.magnitude / maxVelocity;
        paramVal = Mathf.Clamp01(paramVal);

        eventInstance.setParameterByName("velocity", paramVal);
        Debug.Log(paramVal);
    }

    private void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.release();
    }
}
