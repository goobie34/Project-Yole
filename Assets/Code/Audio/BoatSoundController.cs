using UnityEngine;
using FMODUnity;
using FMOD.Studio;
[RequireComponent(typeof(Rigidbody))]
public class BoatSoundController : MonoBehaviour
{
    [SerializeField]
    EventReference eventReference;
    EventInstance eventInstance;

    PARAMETER_ID parameterID;

    [SerializeField]
    string parameterName;

    [SerializeField]
    float maxVelocity;

    Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, this.gameObject);
        parameterID = AudioManager.GetParameterID(eventInstance, parameterName);
    }
    private void Start()
    {
        eventInstance.start();
    }

    private void FixedUpdate()
    {
        float paramVal = rb.linearVelocity.magnitude / maxVelocity;
        paramVal = Mathf.Clamp01(paramVal);

        eventInstance.setParameterByID(parameterID, paramVal);

        //Debug.Log(paramVal);
    }

    private void OnDestroy()
    {
        AudioManager.CleanUp(eventInstance);
    }
}
